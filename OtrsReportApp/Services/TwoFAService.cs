using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OtrsReportApp.Data;
using OtrsReportApp.Models.Account;
using OtrsReportApp.Services.EmailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Services
{
  public class TwoFAService
  {
    private readonly AccountDbContext _db;
    private readonly IEmailService _emailService;
    private UserManager<AccountUser> _userManager;

    public TwoFAService(AccountDbContext db, IEmailService emailService, UserManager<AccountUser> userManager)
    {
      _db = db;
      _emailService = emailService;
      _userManager = userManager;
    }

    public async Task<OtpCode> GenerateCodeForeUser(AccountUser user)
    {
      Random generator = new Random();
      string randomValue = generator.Next(0, 999999).ToString("D6");
      OtpCode otp = new OtpCode
      {
        Code = randomValue,
        UserId = user.Id,
        Expires = DateTime.Now.AddMinutes(2)
      };
      _db.OtpCodes.Add(otp);
      await _emailService.SendEmailAsync(user.Email, otp.Code);
      await _db.SaveChangesAsync();
      return otp;
    }

    public async Task<TwoFaResult> ConfirmOtp(AccountUser user, int otpId, string otp)
    {
      var otpCode = await _db.OtpCodes.FindAsync(otpId);
      if(otpCode != null &&  otpCode.UserId == user.Id)
      {
        if(otpCode.Code == otp) {
          if (DateTime.Now.Subtract(otpCode.Expires).Ticks >= 0)
          {
            await _db.SaveChangesAsync();
            return new TwoFaResult { Succeeded = false, Error = "Otp code is expired" };
          }
          _db.Remove(otpCode);
          await _db.SaveChangesAsync();
          return new TwoFaResult { Succeeded = true, Error = "" };
        }
        if (user.AccessFailedCount >= 3)
        {
          await _db.SaveChangesAsync();
          await _userManager.ResetAccessFailedCountAsync(user);
          return new TwoFaResult { Succeeded = false, Error = "Otp code is wrong" };
        }
        else
        {
          await _userManager.AccessFailedAsync(user);
          return new TwoFaResult { Succeeded = false, Error = "Otp code is wrong" };
        }
      }
      return new TwoFaResult{ Succeeded = false, Error = "" };
    }

    public async Task RemoveOtp(int otpId)
    {
      var otp = await _db.OtpCodes.FindAsync(otpId);
      if (otp != null)
      {
        var otps = _db.OtpCodes.Where(p => p.UserId == otp.UserId).ToList();
        _db.OtpCodes.RemoveRange(otps);
        var user = await _userManager.FindByIdAsync(otp.UserId);
        await _userManager.ResetAccessFailedCountAsync(user);
        await _db.SaveChangesAsync();
      }
    }
  }
}
