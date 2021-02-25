using OtrsReportApp.Models;
using OtrsReportApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OtrsReportApp.Models.DTO;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using OtrsReportApp.Models.Account;
using System.Text;
using OtrsReportApp.Data.Logging;
using System.Security.Claims;
using Org.BouncyCastle.Asn1.Cmp;

namespace OtrsReportApp.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class OtrsTicketController: Controller
  {
   private readonly OtrsTicketService _otrsServcie;
    private readonly Logger _logger;
    private UserManager<AccountUser> _userManager;

    public OtrsTicketController(OtrsTicketService otrs, Logger logger, UserManager<AccountUser> userManager)
    {
      _otrsServcie = otrs;
      _logger = logger;
      _userManager = userManager;
    }

    //[HttpPost("[action]")]
    //public IEnumerable<TTDynamicFields> GetTTDynamicFields([FromBody] Period period)
    //{
    //  return _otrsServcie.GetTTDynamicFields(period);
    //}

    [HttpPost("[action]")]
    [Authorize(Roles = "Admin,User")]
    public IEnumerable<TicketReportDTO> GetTicketsReport([FromBody] Period period)
    {
      return _otrsServcie.GetTicketsReport(period);
    }


    [HttpPost("[action]")]
    [Authorize(Roles = "Admin,User")]
    public Report GetFilteredTicketsReportBulk([FromBody] Filters filters)
    {
      return _otrsServcie.GetFilteredTicketsReportBulk(filters);
    }

    

    [HttpPost("[action]")]
    [Authorize(Roles = "Admin,User")]
    public IEnumerable<TicketReportDTO> GetFilteredTicketsReport([FromBody] Filters filters)
    {
      return _otrsServcie.GetFilteredTicketsReport(filters);
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "Admin,User")]
    public long GetTicketTotalNumber([FromBody] Period period)
    {
      return _otrsServcie.GetTicketTotalNumber(period);
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "Admin,User")]
    public FilteringItems GetFilteringItems([FromBody] Period period)
    {
      return _otrsServcie.GetFilteringItems(period);
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "Admin,User")]
    public FileStreamResult DownloadReport([FromBody] Filters filters)
    {
      var report = _otrsServcie.DownloadReport(filters);
      return report;
    }

    [HttpGet("[action]")]
    [Authorize(Roles = "Admin,User")]
    public IEnumerable<object> GetPendingTickets()
    {
      return _otrsServcie.GetPendingTickets();
    }

    [HttpGet("[action]")]
    [Authorize(Roles = "Admin,User")]
    public IEnumerable<object> GetAcknowledgedTickets()
    {
      return _otrsServcie.GetAcknowledgedTickets();
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "Admin,User")]
    public async Task SaveAcknowledgedTickets([FromBody] IEnumerable<long> ids)
    {
      InitializeUserForLogging();
      var result = await _otrsServcie.SaveAcknowledgedTickets(ids);
      foreach(var x in result)
      {
        await _logger.Log($"Acknowlaged [TT#{x.Tn}] {x.Title}");
      }
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "Admin,User")]
    public async Task RemoveAcknowledgedTickets([FromBody] IEnumerable<long> ids)
    {
      InitializeUserForLogging();
      if (ids.Count() == 1)
      {
        var result = await _otrsServcie.RemoveAcknowledgedTickets(ids.First());
        await _logger.Log($"Undo acknowlagment of [TT#{result.Tn}] {result.Title}");
      }
      else
      {
        var result = await _otrsServcie.RemoveAcknowledgedTickets(ids);
        foreach (var x in result)
        {
          await _logger.Log($"Undo acknowlagment of [TT#{x.Tn}] {x.Title}");
        }
      }
    }

    [HttpGet("[action]")]
    [Authorize(Roles = "Admin,User")]
    public string GetLogs()
    {
      using (var stream = System.IO.File.Open("Logs/Log-20210224.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      {
        List<byte> bytes = new List<byte>(); 
        while (stream.CanRead)
        {
          bytes.Add((byte)stream.ReadByte());
        }
        return Encoding.UTF8.GetString(bytes.ToArray());
      }
    }

    private void InitializeUserForLogging()
    {
      var userId = User.Claims.First(c => c.Type.Equals("userId")).Value;
      var user = _userManager.FindByIdAsync(userId).Result;
      _logger.User = user;
    }

  }
}
