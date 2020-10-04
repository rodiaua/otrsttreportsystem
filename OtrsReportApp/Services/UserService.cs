using Microsoft.AspNetCore.Identity;
using OtrsReportApp.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Services
{
  public class UserService
  {

    private UserManager<AccountUser> _userManager;
    private RoleManager<AccountRole> _roleManager;

    public UserService(UserManager<AccountUser> userManager, RoleManager<AccountRole> roleManager)
    {
      _userManager = userManager;
      _roleManager = roleManager;
    }

    public async Task InitDb()
    {
      if (_roleManager.Roles.Count() < 1)
      {
        await _roleManager.CreateAsync(new AccountRole
        {
          Name = "Admin",
          NormalizedName = "Admin"
        });
        await _roleManager.CreateAsync(new AccountRole
        {
          Name = "User",
          NormalizedName = "User"
        });
      }
      if (_userManager.Users.Count() < 1)
      {
        try
        {
          var user = new AccountUser
          {
            Email = "admin@admin.com",
            UserName = "admin"
          };
          var result = await _userManager.CreateAsync(user, "qwerty");
          await _userManager.AddToRoleAsync(user, "Admin");
        }
        catch (Exception ex)
        {
          throw ex;
        }
      }
    }
  }
}
