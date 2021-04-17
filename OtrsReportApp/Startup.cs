using OtrsReportApp.Models;
using OtrsReportApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using OtrsReportApp.Data;
using OtrsReportApp.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Diagnostics.SymbolStore;
using Microsoft.IdentityModel.Tokens;
using System;
using OtrsReportApp.Configuration;
using OtrsReportApp.Controllers;
using OtrsReportApp.Services.EmailService;
using OtrsReportApp.Extensions;
using Microsoft.Extensions.Logging;
using System.IO;
using OtrsReportApp.Data.Logging;
using OtrsReportApp.Services.LoggingHubService;

namespace OtrsReportApp
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      //Inject AppSettings
      //services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));


      var cfg = new MapperConfiguration(mc =>
      {
        mc.AddProfile(new MappingProfile());
      });
      services.AddSingleton(cfg.CreateMapper());

      services.AddDbContextsBaseOnOS(Configuration);

      services.AddTransient<Logger>();

      services.AddSignalR();

      services.AddTransient<ILoggingDatabase, SqlLoggingDatabase>();

      services.AddIdentity<AccountUser, AccountRole>().
        AddEntityFrameworkStores<AccountDbContext>();

      services.Configure<IdentityOptions>(options =>
      {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        //added
        options.Lockout.MaxFailedAccessAttempts = 10;
        options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(10);
      }
      );

      services.AddScoped<UserService>();
      services.AddScoped<TwoFAService>();
      services.AddEmailService();

      services.AddControllers()
        .AddNewtonsoftJson();

      services.AddSingleton<OtrsTicketService>();
      // In production, the Angular files will be served from this directory
      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "wwwroot";
      });

      //Jwt Auth

      var appSettingsSection = Configuration.GetSection("ApplicationSettings");
      services.Configure<AuthSettings>(appSettingsSection);
      var appSettings = appSettingsSection.Get<AuthSettings>();
      var key = Encoding.UTF8.GetBytes(appSettings.JWTSecret);

      services.AddAuthentication(x =>
      {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(x => {
        x.RequireHttpsMetadata = false;
        x.SaveToken = false;
        x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false,
          ClockSkew = TimeSpan.Zero
        };
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
      /*var path = Directory.GetCurrentDirectory();
      loggerFactory.AddFile("/Logs/Log.txt");*/
      /*app.AddLoggingToFile(loggerFactory);*/
      app.UpdateOtrsTciketDb();

      app.RunBackgroundTask();

      if (env.IsDevelopment())
      {
        //app.Debug();
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      if (!env.IsDevelopment())
      {
        app.UseStaticFiles();
      }

      app.UseCors(builder => builder.WithOrigins("http://localhost:4200")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials()).UseCors("CorsPolicy");

      /*app.UpdateOtsTciketDb();*/

      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapHub<LoggingHub>("/logs");
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller}/{action=Index}/{id?}");
      });

      app.UseSpa(spa =>
      {
        // To learn more about options for serving an Angular SPA from ASP.NET Core,
        // see https://go.microsoft.com/fwlink/?linkid=864501
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment())
        {
          spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
        }
      });

      var provider = app.ApplicationServices;
      using (var scope = provider.CreateScope())
      {
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();
        userService.InitDb().GetAwaiter().GetResult();
      }
      }
    }
  }

