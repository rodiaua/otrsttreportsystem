using OtrsReportApp.Models;
using OtrsReportApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OtrsReportApp.Models.DTO;
using System.IO;

namespace OtrsReportApp.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class OtrsTicketController: Controller
  {
   private readonly OtrsTicketService _otrsServcie;

    public OtrsTicketController(OtrsTicketService otrs)
    {
      _otrsServcie = otrs;
    }

    //[HttpPost("[action]")]
    //public IEnumerable<TTDynamicFields> GetTTDynamicFields([FromBody] Period period)
    //{
    //  return _otrsServcie.GetTTDynamicFields(period);
    //}

    [HttpPost("[action]")]
    public IEnumerable<TicketReportDTO> GetTicketsReport([FromBody] Period period)
    {
      return _otrsServcie.GetTicketsReport(period);
    }


    [HttpPost("[action]")]
    public IEnumerable<TicketReportDTO> GetFilteredTicketsReport([FromBody] Filters filters)
    {
      return _otrsServcie.GetFilteredTicketsReport(filters);
    }

    [HttpPost("[action]")]
    public long GetTicketTotalNumber([FromBody] Period period)
    {
      return _otrsServcie.GetTicketTotalNumber(period);
    }

    [HttpPost("[action]")]
    public FilteringItems GetFilteringItems([FromBody] Period period)
    {
      return _otrsServcie.GetFilteringItems(period);
    }

    [HttpPost("[action]")]
    public FileStreamResult DownloadReport([FromBody] Filters filters)
    {
      var report = _otrsServcie.DownloadReport(filters);
      return report;
    }

  }
}
