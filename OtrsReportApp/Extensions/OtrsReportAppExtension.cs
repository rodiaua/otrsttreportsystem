using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OtrsReportApp.Data;
using OtrsReportApp.Models;
using OtrsReportApp.Models.OtrsTicket;
using OtrsReportApp.Services.EmailService;
using System;
using System.Collections.Generic;
using System.IO;
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
      }
      else if (OperatingSystem.IsLinux())
      {
        services.AddScoped<IEmailService, LinuxEmailService>();
      }
      return services;
    }


    public static IApplicationBuilder AddLoggingToFile(this IApplicationBuilder app, ILoggerFactory lf)
    {
      var filePath = "Logs/Log.txt";
      Directory.CreateDirectory("Logs");
      if (!File.Exists(filePath))
      {
        File.Create(filePath);
      }
      lf.AddFile(filePath, LogLevel.Information, null, false, 1073741824l, 31, "{Timestamp:o} {Message}" + "\n");
      return app;
    }

    public static IServiceCollection AddDbContextsBaseOnOS(this IServiceCollection services, IConfiguration configuration)
    {


      services.AddDbContext<ApplicationDbContext>(options =>
      {
        var cs = configuration.GetConnectionString("Otrs");
        options.UseMySql(cs);
      });
      if (OperatingSystem.IsWindows())
      {
        services.AddDbContext<AccountDbContext>(options =>
        {
          var cs = configuration.GetConnectionString("ReportSystemAccountsWindows");
          options.UseMySql(cs);
        }
        , ServiceLifetime.Scoped);

        services.AddDbContext<TicketDbContext>(options =>
        {
          var cs = configuration.GetConnectionString("OtrsTicketsWindows");
          options.UseMySql(cs);
        }
        , ServiceLifetime.Scoped);

        services.AddDbContext<LoggingDbContext>(options =>
        {
          var cs = configuration.GetConnectionString("LoggingDbWindows");
          options.UseMySql(cs);
        }
        , ServiceLifetime.Scoped);
      }
      else if (OperatingSystem.IsLinux())
      {
        services.AddDbContext<AccountDbContext>(options =>
        {
          var cs = configuration.GetConnectionString("ReportSystemAccountsLinux");
          options.UseMySql(cs);
        }
        , ServiceLifetime.Scoped);
        services.AddDbContext<TicketDbContext>(options =>
        {
          var cs = configuration.GetConnectionString("OtrsTicketsLinux");
          options.UseMySql(cs);
        }
        , ServiceLifetime.Scoped);

        services.AddDbContext<LoggingDbContext>(options =>
        {
          var cs = configuration.GetConnectionString("LoggingDbLinux");
          options.UseMySql(cs);
        }
        , ServiceLifetime.Scoped);
      }
      return services;
    }
  }
}
