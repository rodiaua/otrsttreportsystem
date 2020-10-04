using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OtrsReportApp.Models;
using OtrsReportApp.Models.Account;
using System.Transactions;
using OtrsReportApp.Data;

namespace OtrsReportApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AppUserController : Controller
  {
    private UserManager<AccountUser> _userManager;
    private RoleManager<AccountRole> _roleManager;
    private SignInManager<AccountUser> _signInManager;
    private IMapper _mapper;
    private readonly ApplicationSettings _appSettings;
    private readonly AccountDbContext _accountDbContext;

    public AppUserController(UserManager<AccountUser> userManager, SignInManager<AccountUser> signInManager,
      IMapper mapper, IOptions<ApplicationSettings> applicationSetting, RoleManager<AccountRole> roleManager, AccountDbContext accountDbContext)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _mapper = mapper;
      _appSettings = applicationSetting.Value;
      _roleManager = roleManager;
      _accountDbContext = accountDbContext;
    }

    [HttpGet("[action]")]
    [Authorize(Roles = "Admin")]
    public async Task<IEnumerable<UserTableModel>> GetUsers()
    {
      var users = (from user in _userManager.Users
                   select user).ToList();
      var result = new List<UserTableModel>();
      foreach (var user in users)
      {
        var roles = await _userManager.GetRolesAsync(user);
        result.Add(new UserTableModel
        {
          Id = user.Id,
          UserName = user.UserName,
          Email = user.Email,
          FirstName = user.FirstName,
          LastName = user.LastName,
          Role = roles.FirstOrDefault()
        });
      }
      return result;
    }

    [HttpGet("[action]/{Id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(string Id)
    {
      try
      {
        var user = await _userManager.FindByIdAsync(Id);

        var result = await _userManager.DeleteAsync(user);

        return Ok(result);

      }catch(Exception ex)
      {
        throw ex;
      }
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
      var user = await _userManager.FindByIdAsync(model.Id);

      var result = await _userManager.RemovePasswordAsync(user);
      if (result.Succeeded)
      {
        var setPasswordResult = await _userManager.AddPasswordAsync(user, model.Password);
        return Ok(setPasswordResult);
      }
      return Ok(result);
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser([FromBody] UserTableModel model)
    {
      try
      {
        var user = await _userManager.FindByIdAsync(model.Id);
        user.Email = model.Email;
        user.UserName = model.UserName;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        var updateUserProfileResult = await _userManager.UpdateAsync(user);
        var userRoles = await _userManager.GetRolesAsync(user);
        _userManager.RemoveFromRolesAsync(user, userRoles).GetAwaiter().GetResult();
        var updateUserRoleResult = await _userManager.AddToRoleAsync(user, model.Role);
        return Ok(new { updateUserProfileResult, updateUserRoleResult });
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register(RegistrationUserModel model)
    {
      var accountUser = _mapper.Map<AccountUser>(model);
      try
      {
        var result = await _userManager.CreateAsync(accountUser, model.Password);
        await _userManager.AddToRoleAsync(accountUser, model.Role);
        return Ok(result);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    

    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginUserModel model)
    {
      var user = await _userManager.FindByNameAsync(model.UserName);
      if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
      {
        //get the role assighned to the user
        var roles = await _userManager.GetRolesAsync(user);
        IdentityOptions _options = new IdentityOptions();
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
          Subject = new ClaimsIdentity(new Claim[]
          {
            new Claim("userId", user.Id.ToString()),
            new Claim(_options.ClaimsIdentity.RoleClaimType, roles.FirstOrDefault())
          }),
          Expires = DateTime.UtcNow.AddHours(12),
          SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Gj3du91iGAl34bnd90Dv3BNhdakh3Hha6chV")), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);
        return Ok(new { token });
      }
      return BadRequest(new { message = "Username or password is incorrect." });
    }


  }
}
