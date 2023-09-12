using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using PhoneBookHumanGroupBL.EmailSenderManager;
using PhoneBookHumanGroupBL.IEmailSender;

namespace PhoneBookHumanGroupAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailsender;

        public EmailController(IEmailSender emailsender)
        {
            _emailsender = emailsender;
        }

        [HttpPost]
        public IActionResult SendEmail(EmailMessageModel model)
        {
            _emailsender.SendEmail(model);
            return Ok(new { Message = "Mailiniz başarıyla gönderildi!" });
        }
    }
}
