using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneBookHumanGroupAPI.Models;
using PhoneBookHumanGroupBL.InterfacesOfManagers;
using PhoneBookHumanGroupEL.ViewModels;
using Serilog.Core;
using System.Numerics;
using AutoMapper;
using System;

namespace PhoneBookHumanGroupAPI.Controllers
{
    [Route("api/rehber")]
    [ApiController]
    public class PhoneController : ControllerBase
    {
        private readonly ILogger<PhoneController> _logger;
        private readonly IMemberManager _memberManager;
        private readonly IPhoneGroupManager _phoneGroupManager;
        private readonly IMemberPhoneManager _memberPhoneManager;
        private readonly IMapper _mapper;

        public PhoneController(ILogger<PhoneController> logger, IMemberManager memberManager, IPhoneGroupManager phoneGroupManager, IMemberPhoneManager memberPhoneManager, IMapper mapper)
        {
            _logger = logger;
            _memberManager = memberManager;
            _phoneGroupManager = phoneGroupManager;
            _memberPhoneManager = memberPhoneManager;
            _mapper = mapper;
        }



        //Bir kişinin emailini parametre olarak gönderdiğimizde kaç tane rehnerinde kişi ekli olduğunu bulalım
        [HttpGet]
        [Route("gcc")]
        public IActionResult GetContactCount(string? email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogError($"HATA: PhoneController/GetContactCount email BOŞ ya da null:{email}");
                    return Ok("Email parametresi gereklidir!");

                }

                var user = _memberManager.GetByCondition(x => x.Email.ToLower() == email.ToLower()).Data;
                if (user == null)
                {
                    _logger.LogError($"HATA: PhoneController/GetContactCount user null geldi:{email}");
                    return Ok("Rehber sistemine kayıt olduğunuza emin olunuz!");
                }

                var result = _memberPhoneManager.GetAll(x => x.MemberId == user.Id).Data;
                if (result == null)
                {
                    _logger.LogInformation($"PhoneController/GetContactCount {user.Id} idli userın rehberinde henüz kimse yok.");
                    return Ok($"{user.Name} {user.Surname} adlı kişinin Rehberinde sıfır kişi kayıtlıdır!");

                }
                else
                {
                    _logger.LogInformation($"PhoneController/GetContactCount {user.Id} idli userın rehberinde {result.Count} adet kişi kayıtlıdır.");

                    return Ok($"{user.Name} {user.Surname} adlı kişinin Rehberinde {result.Count} adet kişi kayıtlıdır!");

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"HATA: PhoneController/GetContactCount email:{email}");
                return Problem("Beklenmedik hata oluştu");
            }
        }


        //Bir telefon numarasının kimlerin rehberinde - nasıl kayıtlı olduğunu bulalım
        //05396796650 şeklindeki numarayı kim nasıl kaydetmiş?

        [HttpGet]
        [Route("gnas")]
        public IActionResult GetNumberAsSaved(string phone)
        {
            try
            {
                if (string.IsNullOrEmpty(phone))
                {
                    _logger.LogError($"HATA: PhoneController/GetNumberAsSaved phone BOŞ ya da null:{phone}");
                    return Ok("Phone parametresi gereklidir!");

                }
                //telefon içinde rakam harici birşey olmamalı 

                //1. yol daha uzun çünkü gidip model oluştrduk
                List<GetNumberAsSavedViewModel> result = new List<GetNumberAsSavedViewModel>();
                var data = _memberPhoneManager.GetAll(x => x.PhoneNumber == phone).Data;

                if (data.Count == 0)
                {
                    _logger.LogError($"HATA: PhoneController/GetNumberAsSaved data null phone:{phone}");
                    return Ok("Bu telefon sistemde hiç kimsenin rehberinde kaydedilmemiştir.");

                }
                foreach (var item in data)
                {
                    result.Add(new GetNumberAsSavedViewModel()
                    {
                        WhoSaved = $"{item.Member.Name} {item.Member.Surname}",
                        HowSaved = $"{item.PhoneGroup.Name}",
                    });
                }

                _logger.LogInformation($"PhoneController/GetNumberAsSaved {phone} şeklindeki numara {result.Count} adet kişinin rehberinde kayıtlıymış.");
                return Ok(result);


                //2. yol daha kısa  MemberPhoneVM



            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"HATA: PhoneController/GetNumberAsSaved phone:{phone}");
                return Problem("Beklenmedik hata oluştu");
            }
        }



        //Rehbere yeni kişi ekleyelim
        [HttpPost]
        [Route("anc")]
        public IActionResult AddNewContact(AddNewContactViewModel model)
        {
            try
            {
                //1. gelen numara kimin rehberine kayıt olacak?
                var user = _memberManager.GetbyId(model.MemberId).Data;
                if (user == null)
                {
                    _logger.LogError( $"HATA: PhoneController/AddNewContact model:{model}");
                    return Problem("Lütfen rehber sistemine üye olduğunuza emin olunuz!");

                }

                //2 numara rehberde başka birine ait mi?
                var isNumberBelongAnother = _memberPhoneManager.GetByCondition(x => 
                x.MemberId == user.Id && x.PhoneNumber == model.PhoneNumber).Data;

                if (isNumberBelongAnother != null)
                {
                    _logger.LogError($"HATA: PhoneController/AddNewContact model:{model}");
                    return Problem($"{model.PhoneNumber} şeklinde numara rehberinize {isNumberBelongAnother.PhoneGroupNameSurname} olarak zaten kayıtlıdır!");

                }


                //3 phonegroupid sıfır geldiyse yeni bir grup ekliyor
                if (model.PhoneGroupId == 0)
                {
                    var phoneGrp = new PhoneGroupVM()
                    {
                        CreatedDate = DateTime.Now,
                        Name = model.AnotherPhoneGroupName,
                        IsActive = true
                    };

                    var isSamephoneGrp = _phoneGroupManager.GetByCondition(x => x.Name.ToLower() == model.AnotherPhoneGroupName.ToLower() && x.IsActive).Data;

                    if (isSamephoneGrp == null)
                    {
                        var result = _phoneGroupManager.Add(phoneGrp).Data;
                        model.PhoneGroupId = result.Id;
                    }
                    else
                    {
                        model.PhoneGroupId = isSamephoneGrp.Id;
                    }
                }
                else
                {
                    //4 ya phoneGroup'unu 9999 verirse?
                    var phonegrp=_phoneGroupManager.GetbyId(model.PhoneGroupId).Data;
                    if (phonegrp==null)
                    {
                        _logger.LogError($"HATA: PhoneController/AddNewContact model:{model}");
                        return Ok($"{model.PhoneGroupId} şeklinde gönderdiğiniz rehber grubu sistemde mevcut değildir! Farklı bir grup oluşturmak için anotherphonegroupname'i kullanabilirsiniz");

                    }
                }

                //var newContact = _mapper.ConfigurationProvider.CreateMapper().Map<AddNewContactViewModel, MemberPhoneVM>(model);

                var newContact = _mapper.Map<AddNewContactViewModel, MemberPhoneVM>(model);

                newContact.CreatedDate = DateTime.Now;

                if (_memberPhoneManager.Add(newContact).IsSuccess)
                {
                    _logger.LogInformation($"PhoneController/AddNewContact EKLENDI... model:{model}");
                    return Ok($"Rehbere ekledik.");
                }
                else
                {
                    _logger.LogError($"HATA: PhoneController/AddNewContact model:{model}");
                    return Ok($"Ekleme başarısız!");
                }

                }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"HATA: PhoneController/AddNewContact model:{model}");
                return Problem("Beklenmedik hata oluştu");
            }
        }
    }
}
