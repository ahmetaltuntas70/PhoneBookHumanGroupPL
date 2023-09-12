using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PhoneBookHumanGroupBL.EmailSenderManager;
using PhoneBookHumanGroupBL.IEmailSender;
using PhoneBookHumanGroupBL.InterfacesOfManagers;
using PhoneBookHumanGroupEL.ViewModels;
using PhoneBookHumanGroupPL.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PhoneBookHumanGroupPL.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IMemberManager _memberManager;
        private readonly IEmailSender _emailSender;
        const int keySize = 64;
        const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public AccountController(ILogger<AccountController> logger, IMemberManager memberManager, IEmailSender emailSender)
        {
            _logger = logger;
            _memberManager = memberManager;
            _emailSender = emailSender;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var isSameEmail = _memberManager.GetByCondition(x => x.Email.ToLower() == model.Email.ToLower());

                if (isSameEmail.IsSuccess)
                {
                    //Demekki sistemde o email kayıtlı!
                    ModelState.AddModelError("", "Bu email sistemde mevcuttur! Şifrenizi unuttuysanız Şifremi Unuttum Seçeneğine tıklayınız!");
                    return View(model);
                }

                //1. yol
                MemberVM m = new MemberVM()
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    BirthDate = model.BirthDate,
                    Gender = model.Gender,
                    IsActive = true,
                    Email = model.Email,
                    CreatedDate = DateTime.Now
                };


                m.PasswordHash = HashPasword(model.Password, out byte[] salt);
                m.Salt = salt;

                //2. yol Automapper ile dönüşümü daha kısa yapabilir miyiz?

                if (_memberManager.Add(m).IsSuccess)
                {
                    //Login sayfasına gitsin
                    return RedirectToAction("Login", "Account", new { email = model.Email });
                }
                else
                {

                    ModelState.AddModelError("", "Ekleme başarısız!");
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu! Tekrar deneyiniz!");
                return View(model);
            }
        }



        public IActionResult Login(string? email)
        {
            _logger.LogInformation("Login sayfasını açtı");
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password) //LoginViewModel
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    return View();
                }

                //Önceki projelerden hashlem mekanizmasını alalım
                var isUser = _memberManager.GetByCondition(x => x.Email.ToLower() == email.ToLower()).Data;

                if (isUser == null)
                {
                    ModelState.AddModelError("", "Email ya da şifreniz yanlış! Tekrar deneyiniz!");
                    return View();

                }
                var isTruePassword = VerifyPassword(password, isUser.PasswordHash, isUser.Salt);
                if (!isTruePassword)
                {
                    ModelState.AddModelError("", "Email ya da şifreniz yanlış! Tekrar deneyiniz!");
                    return View();
                }

                //Logged in user oturuma kayıt etmek lazım 
                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                //Claim loginClaim = new Claim(ClaimTypes.Name, email);
                var claims = new List<Claim>
                {
                 new Claim(ClaimTypes.Name, isUser.Email),
                 new Claim("FullName", $"{isUser.Name} {isUser.Surname}"),
               //  new Claim(ClaimTypes.Role, "Administrator"), //Benim PhoneBook prpjemde rol ile ilgili bir tablo yok bir yapı yok Ozaman benim Role diye bir claim oluşturmama gerek yok.
               };

                identity.AddClaims(claims);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return RedirectToAction("Phones", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu! Tekrar deneyiniz!");
                return View();
            }
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(string? email)
        {
            try
            {
                var user = _memberManager.GetByCondition(x => x.Email.ToLower() == email.ToLower()).Data;
                if (user == null) 
                {
                    TempData["ForgotPasswordFailedMsg"] = "Sisteme bu emaille kayıtlı olduğunuz emin olunuz! ";
                    return RedirectToAction("Login", "Account");
                }

                var randomPassword = CreatePassword(); 
                user.PasswordHash = HashPasword(randomPassword, out byte[] salt);
                user.Salt = salt;

                var updateResult=_memberManager.Update(user);
                if (!updateResult.IsSuccess)
                {
                    TempData["ForgotPasswordFailedMsg"] = "Bir sorun oluştu! Tekrar deneyiniz!";
                    return RedirectToAction("Login", "Account");
                }
                _emailSender.SendEmail(new EmailMessageModel(){

                    Subject= "PhoneBookHumanGroup Şifremi Unuttum SendEmail",
                    Body = $"<b>Merhaba {user.Name} {user.Surname},</b><br/>" +
                    $"Yeni şifreniz :{randomPassword} <br/>" +
                    $"Giriş yapmak için <a href='http://localhost:5071/Account/Login?email={user.Email}'>buraya</a> tıklayınız",
                    To =user.Email
                });

                _emailSender.SendMailAsync(new EmailMessageModel()
                {
                    Subject = "PhoneBookHumanGroup Şifremi Unuttum SendMailAsync",
                    Body=$"<b>Merhaba {user.Name} {user.Surname},</b><br/>" +
                    $"Yeni şifreniz :{randomPassword} <br/>" +
                    $"Giriş yapmak için <a href='http://localhost:5071/Account/Login?email={user.Email}'>buraya</a> tıklayınız",
                    To = user.Email
                });
                

                TempData["ForgotPasswordSuccessMsg"] = "Emailinize birkaç dakika içinde yeni şifreniz gelecektir.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                TempData["ForgotPasswordFailedMsg"] = "Beklenmedik bir sorun var!";
                return RedirectToAction("Login", "Account");
            }
        }

        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            //return LocalRedirect("http://localhost:5071/Account/Login");
            return RedirectToAction("Login", "Account");
        }

        private string HashPasword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
            salt,
                iterations,
                hashAlgorithm,
                keySize);
            return Convert.ToHexString(hash);
        }
        private bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);

            return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
        }


        public string CreatePassword(int length=8)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%-_";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
