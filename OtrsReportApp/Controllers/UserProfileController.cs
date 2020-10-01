using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OtrsReportApp.Models.Account;

namespace OtrsReportApp.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UserProfileController : Controller
  {
    private UserManager<AccountUser> _userManager;
    private IMapper _mapper;
    public UserProfileController(UserManager<AccountUser> userManager, IMapper mapper)
    {
      _userManager = userManager;
      _mapper = mapper;
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
      var userId = User.Claims.First(c => c.Type.Equals("userId")).Value;
      var user = await _userManager.FindByIdAsync(userId);
      try
      {
        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        return Ok(result);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> UpdateProfile([FromBody]UpdateUserModel model)
    {
      var userId = User.Claims.First(c => c.Type.Equals("userId")).Value;
      var user = await _userManager.FindByIdAsync(userId);
      user.Email = model.Email;
      user.UserName = model.UserName;
      user.FirstName = model.FirstName;
      user.LastName = model.LastName;
      try
      {
        var result = await _userManager.UpdateAsync(user);
        return Ok(result);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    [HttpGet("[action]")]
    [Authorize(Roles ="Admin,User")]
    public async Task<Object> GetUserProfile()
    {
      string userId = User.Claims.First(c => c.Type.Equals("userId")).Value;
      var user = await _userManager.FindByIdAsync(userId);
      return new
      {
        user.Email,
        user.UserName,
        user.FirstName,
        user.LastName
      };
    }
  }
}
