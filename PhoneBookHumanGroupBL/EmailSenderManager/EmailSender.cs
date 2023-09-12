using PhoneBookHumanGroupBL.IEmailSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookHumanGroupBL.EmailSenderManager
{
    public class EmailSender : IEmailSender
    {
        public bool SendEmail(EmailMessageModel model)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("rico.schinner38@ethereal.email");
                message.To.Add(new MailAddress(model.To));
                message.Subject = model.Subject;
                message.Body = model.Body;
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("rico.schinner38@ethereal.email", "wXftdJ7Jdnp79WWQa2");
                client.Port = 587;
                client.Host = "smtp.ethereal.email";
                client.EnableSsl = true;

                client.Send(message);
                return true;
            }
            catch (Exception)
            {
                return false;
                //loglasın
            }
        }

        public async Task SendMailAsync(EmailMessageModel model)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("rico.schinner38@ethereal.email");
                message.To.Add(new MailAddress(model.To));
                message.Subject = model.Subject;
                message.Body = model.Body;
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("rico.schinner38@ethereal.email", "wXftdJ7Jdnp79WWQa2");
                client.Port = 587;
                client.Host = "smtp.ethereal.email";
                client.EnableSsl = true;

                await client.SendMailAsync(message);

            }
            catch (Exception)
            {

                //loglasın
            }
        }



    }
}
