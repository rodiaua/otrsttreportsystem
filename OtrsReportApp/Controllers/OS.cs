using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class OS : Controller
  {
    [HttpGet("[action]")]
    public bool IsLinux()
    {
      return OperatingSystem.IsLinux();
    }
  }
}
