using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Services.EmailService
{
  public class LinuxEmailService : IEmailService
  {

    public async Task SendEmailAsync(string to, string body)
    {
      try
      {
        string from = "otrsreport@gms-worldwide.com";
        string subject = "OTRS report 2FA";
        string command = $"\"echo \\\"{body}\\\" | EMAIL=\\\"{from}\\\" mutt -s \\\"{subject}\\\" {to}\"";

        var process = new Process()
        {
          StartInfo = new ProcessStartInfo
          {
            FileName = "/bin/bash",
            Arguments = $"-c {command}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
          }
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        await process.WaitForExitAsync();
      }catch (Exception ex)
      {
        throw ex;
      }
    }
  }
}
