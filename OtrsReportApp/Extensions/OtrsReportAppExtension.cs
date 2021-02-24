using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OtrsReportApp.Data;
using OtrsReportApp.Services.EmailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Extensions
{
  public static class OtrsReportAppExtension
  {
    public static IApplicationBuilder UpdateOtsTciketDb(this IApplicationBuilder app)
    {
      var provider = app.ApplicationServices;
      using (var scope = provider.CreateScope())
      {
        var context = scope.ServiceProvider.GetRequiredService<TicketDbContext>();
        context.Database.Migrate();
      }
      return app;
    }

    public static IServiceCollection AddEmailService(this IServiceCollection services)
    {
      if (OperatingSystem.IsWindows())
      {
        services.AddScoped<IEmailService, WindowsEmailService>();
      }else if (OperatingSystem.IsLinux())
      {
        services.AddScoped<IEmailService, LinuxEmailService>();
      }
      return services;
    }
  }
}
