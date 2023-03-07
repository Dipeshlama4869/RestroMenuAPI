using Microsoft.Extensions.Options;
using System.Net.Mail;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Security.Authentication;

namespace RestroMenu.Helpers
{
    public class EmailHelper
    {
        private readonly MailSetting _emailSettings;

        public EmailHelper(IOptions<MailSetting> MailSettings)
        {
            _emailSettings = MailSettings.Value;
        }

        public void Send(string from, string to, string subject, string html)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.SslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
            smtp.Connect(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.Auto);
            smtp.Authenticate(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public void SendEmail(string receiverAddress, string mailSubject, string body, string filepath = "")//if you have attachment then filepath 
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_emailSettings.SmtpUser);//senderaddress
                    mail.To.Add(receiverAddress);
                    //mail.To.Add("bpos100@zohomail.in");


                    mail.Subject = mailSubject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    System.Net.Mail.Attachment attachment;
                    //attachment = new System.Net.Mail.Attachment(filepath);
                    //mail.Attachments.Add(attachment);
                    using (System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort))//port change as sender email type
                    {
                        SmtpServer.UseDefaultCredentials = false; //Need to overwrite this
                        SmtpServer.EnableSsl = true;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass);//senderaddress and password
                        SmtpServer.Send(mail);
                        //delete the file after sending mail
                        //deletefile(filepath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
