using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Services.EmailService
{
  public class WindowsEmailService : IEmailService
  {
    public async Task SendEmailAsync(string to, string body)
    {
      var email = new MimeMessage();
      email.Sender = MailboxAddress.Parse("ruby34@ethereal.email");
      email.To.Add(MailboxAddress.Parse("ruby34@ethereal.email"));
      email.Subject = "OTRS report 2FA";
      var builder = new BodyBuilder();
      builder.HtmlBody = body;
      email.Body = builder.ToMessageBody();
      using var smtp = new SmtpClient();
      smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
      smtp.Authenticate("ruby34@ethereal.email", "V9NhZsUnzmJ9FTkP44");
      await smtp.SendAsync(email);
      Console.WriteLine($"To: ruby34@ethereal.email\nSent: {body}");
      smtp.Disconnect(true);
    }
  }
}
