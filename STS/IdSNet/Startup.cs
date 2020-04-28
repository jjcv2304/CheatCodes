// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdSNet.Data;
using IdSNet.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdSNet
{
  public class Startup
  {
    public IWebHostEnvironment Environment { get; }
    public IConfiguration Configuration { get; }

    public Startup(IWebHostEnvironment environment, IConfiguration configuration)
    {
      Environment = environment;
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews();
      services.AddMvc();

      services.AddHttpsRedirection(httpsOpts => { httpsOpts.HttpsPort = 5002; });

      // configures IIS out-of-proc settings (see https://github.com/aspnet/AspNetCore/issues/14882)
      services.Configure<IISOptions>(iis =>
      {
        iis.AuthenticationDisplayName = "Windows";
        iis.AutomaticAuthentication = false;
      });

      services.AddTransient<IEmailSender, EmailSender>();
      services.Configure<AuthMessageSenderOptions>(Configuration);

      // configures IIS in-proc settings
      services.Configure<IISServerOptions>(iis =>
      {
        iis.AuthenticationDisplayName = "Windows";
        iis.AutomaticAuthentication = false;
      });

      services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

      services.AddIdentity<ApplicationUser, IdentityRole>(config =>
        {
          config.SignIn.RequireConfirmedEmail = true;
          config.SignIn.RequireConfirmedAccount = true;
        })
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();

      var builder = services.AddIdentityServer(options =>
        {
          options.IssuerUri = "http://localhost:80";
          //options.PublicOrigin = "https://localhost:5002";
          options.Events.RaiseErrorEvents = true;
          options.Events.RaiseInformationEvents = true;
          options.Events.RaiseFailureEvents = true;
          options.Events.RaiseSuccessEvents = true;
          options.Authentication.CookieLifetime = TimeSpan.FromMinutes(15);
        })
          .AddInMemoryIdentityResources(Config.Ids)
          .AddInMemoryApiResources(Config.Apis)
          .AddInMemoryClients(Config.Clients)
          .AddAspNetIdentity<ApplicationUser>();

      // not recommended for production - you need to store your key material somewhere secure
      builder.AddDeveloperSigningCredential();

      //services.AddAuthentication()
      //    .AddGoogle(options =>
      //    {
      //        // register your IdentityServer with Google at https://console.developers.google.com
      //        // enable the Google+ API
      //        // set the redirect URI to http://localhost:5000/signin-google
      //        options.ClientId = "copy client ID from Google here";
      //        options.ClientSecret = "copy client secret from Google here";
      //    });
    }

    public void Configure(IApplicationBuilder app)
    {
      if (Environment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }

      app.UseStaticFiles();

      app.UseRouting();
      app.UseHsts();
      //app.UseHttpsRedirection();
      app.UseIdentityServer();
      app.UseAuthorization();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapDefaultControllerRoute();
        endpoints.MapRazorPages();
      });
    }
  }
}