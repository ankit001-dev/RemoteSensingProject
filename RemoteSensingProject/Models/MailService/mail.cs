using System;
using System.Net.Mail;

namespace RemoteSensingProject.Models.MailService
{
    public class mail
    {

        public bool SendMail(string name, string email, string subject, string message)
        {
            try
            {
                string pass = "lmrs wdni jxbh ggzi";
                string emailFrom = "mohdsahbag0786@gmail.com";
                MailMessage mail = new MailMessage(emailFrom, email);
                mail.Subject = subject;
                var userName = name != null ? name : email;
                mail.Body = $"" +
                        $"<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"max-width: 600px; margin: auto; background-color: #f9f9f9; border: 1px solid #ddd;\">\r\n        <tr>\r\n            <td style=\"background-color: #0044cc; color: #fff; padding: 10px; text-align: center;\">\r\n                <h1 style=\"margin: 0;\">Remote Sensing</h1>\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <td style=\"padding: 20px; background-color: #fff;\">\r\n                <p style=\"margin: 0 0 10px 0;\">Hello {userName},</p>\r\n                <p style=\"margin: 0 0 10px 0;\">We hope this message finds you well.\r\n {message}.</p>\r\n<p style=\"margin: 0 0 10px 0;\">For more information, please visit our website <img src=\"https://macreel.co.in/assets/macreel-Infosoft.png\" width=\"60\">\r\n or contact us at sales@macreel.co.in.</p>\r\n                <p style=\"margin: 0 0 10px 0;\">Thank you for your attention!</p>\r\n                <p style=\"margin: 0;\">Best regards,<br>\r\n                RemoteSensing<br>\r\n                </p>\r\n                   </tr>\r\n    </table>" +
                        $"";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(emailFrom, pass);
                smtp.EnableSsl = true;
                int emailSend = 0;
                smtp.Send(mail);
                emailSend++;
                return emailSend > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool SendOtpMail(string userEmail, string otp)
        {
            string subject = "Your OTP Code";

            string body = $@"
<p>Your One-Time Password (OTP) is:</p>

<h2 style='color:#0044cc; margin:10px 0;'>{otp}</h2>

<p>This OTP is valid for <b>5 minutes</b>.</p>

<p>If you did not request this, please ignore this email.</p>";

            return SendMail("", userEmail, subject, body);
        }


        public bool SendPasswordChangedMail(string userEmail, string newPassword)
        {
            string subject = "Your Password Has Been Changed";

            string body = $@"
<p>Your password has been changed successfully.</p>

<p><strong>New Password:</strong></p>

<h3 style='color:#0044cc; margin:10px 0;'>{newPassword}</h3>

<p>If you did not request this change, contact support immediately.</p>";

            return SendMail("", userEmail, subject, body);
        }


    }
}