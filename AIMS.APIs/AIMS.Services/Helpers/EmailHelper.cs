﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AIMS.APIs.Helpers
{
    public class EmailHelper
    {
        public void SendEmail(List<string> sendTo, string body, string subject)
        {
            SmtpClient client = this.GetEmailClient();
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("whoever@me.com");
            mailMessage.To.Add("receiver@me.com");
            mailMessage.Body = "body";
            mailMessage.Subject = "subject";
            client.Send(mailMessage);
        }

        private SmtpClient GetEmailClient()
        {
            SmtpClient client = new SmtpClient("mysmtpserver");
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("username", "password");
            return client;
        }
    }
}