using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace AD_CF.Utilities
{
    public class EmailServices
    {
        public void sendEmail(string receiver, string cmd,int? number)
        {
            var senderEmail = new MailAddress("team10gdipsa48@gmail.com");
            var receiverEmail = new MailAddress(receiver);
            var password = "Sa48team10";

            if (cmd == "Requested")
            {
                var sub = "New Request No" +number+ " has been sent for approval";
                var body = "You have a new Product Request "+number+ " from your department Staffs";
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };

                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = sub,
                    Body = body
                })
                {
                    smtp.Send(mess);
                }
            }

            if (cmd == "Approve")
            {
                var sub = "Req No "+number+ " has been Approved";
                var body = "Your Product Request number " + number + " has been approved";
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };

                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = sub,
                    Body = body
                })
                {
                    smtp.Send(mess);
                }
            }

            if (cmd == "Reject")
            {
                var sub = "Request No " + number +" Rejected";
                var body = "Your Product Request No " + number + " has been Rejected";
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };

                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = sub,
                    Body = body
                })
                {
                    smtp.Send(mess);
                }
            }
        }
    }
}