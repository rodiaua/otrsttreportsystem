using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Services.EmailService
{
  public interface IEmailService
  {
    public Task SendEmailAsync(string to, string body);
  }
}
