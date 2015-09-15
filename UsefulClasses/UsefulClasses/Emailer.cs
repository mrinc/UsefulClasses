using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Useful_Classes
{
    public class Emailer
    {
        public string Host { get; set; }
        private int PPort = 25;
        public int Port { get { return PPort; } set { PPort = value; } }
        //public bool RequireSSL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public EmailResponce Send(EmailMessage message)
        {
            EmailResponce responce = new EmailResponce();

            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(GetEmail(message.From));

                SmtpClient SmtpServer = new SmtpClient(Host, Port);

                //SmtpServer.EnableSsl = ((RequireSSL != null) ? RequireSSL : false);

                foreach (EmailClient email in message.to)
                {
                    if (email.Type == EmailClientType.TO)
                        mail.To.Add(GetEmail(email.Email));
                    else if (email.Type == EmailClientType.CC)
                        mail.CC.Add(GetEmail(email.Email));
                    else if (email.Type == EmailClientType.BCC)
                        mail.Bcc.Add(GetEmail(email.Email));
                }

                mail.Subject = message.Subject;

                mail.IsBodyHtml = message.IsHtml;
                mail.Body = message.Message;

                if (Username != null)
                    SmtpServer.Credentials = new System.Net.NetworkCredential(Username, Password);
                else
                    SmtpServer.UseDefaultCredentials = true;

                SmtpServer.Send(mail);

                responce.Title = "SUCCESS";
                responce.Message = "Sent successfully";
                responce.WasSuccess = true;
            }
            catch (Exception eee)
            {
                responce.Title = "ERROR";
                responce.Message = eee.Message;
                responce.WasSuccess = false;
            }

            return responce;
        }

        private string GetEmail(EmailAddress address)
        {
            string client = address.Email;

            if (address.Name != null)
                client = address.Name + "<" + client + ">";

            return client;
        }

        private string GetEmail(EmailClient address)
        {
            string client = address.Email.Email;

            if (address.Email.Name != null)
                client = address.Email.Name + "<" + client + ">";

            return client;
        }
    }

    public class EmailResponce
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool WasSuccess { get; set; }
    }

    public class EmailMessage
    {
        public EmailClient[] to { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsHtml { get; set; }
        public EmailAddress From { get; set; }
    }

    public class EmailAddress
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class EmailClient
    {
        public EmailAddress Email { get; set; }
        public EmailClientType Type { get; set; }
    }

    public enum EmailClientType
    {
        TO,
        CC,
        BCC
    }
}
