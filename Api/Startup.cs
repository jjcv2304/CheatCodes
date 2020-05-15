using System;
using System.Data.Common;
using Api.Logs.Extensions;
using Api.Logs.Filters;
using Api.Logs.Middleware;
using Api.Security;
using Api.Utils;
using Application.Utils;
using Application.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistance;
using Persistance.Utils;

namespace Api
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

      services.AddMvc(options =>
        {
          options.EnableEndpointRouting = false;
          options.Filters.Add(typeof(TrackActionPerformanceFilter));
        }
      ).SetCompatibilityVersion(CompatibilityVersion.Latest);

      services.AddSingleton<IScopeInformation, ScopeInformation>();

      services.AddCors(o =>
      {
        o.AddPolicy("ApiCorsPolicy", builder =>
        {
          builder.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(origin => origin == "http://localhost:3214")
            .AllowCredentials();
        });
      });

      var connectionString = Configuration.GetConnectionString("CheatCodesDatabase");
      var con = new DatabaseSetting(connectionString);
      services.AddSingleton(con);
      var queriesConnectionString = new QueriesConnectionString(connectionString);
      services.AddSingleton(queriesConnectionString);
      // services.AddTransient<IDbTransaction, DbTransaction>();
      services.AddTransient<IUnitOfWork, UnitOfWork>();
      services.AddTransient<ICategoryQueryRepository, CategoryQueryRepository>();
      services.AddTransient<ICategoryCommandRepository, CategoryCommandRepository>();


      services.AddSingleton<Messages>();


      services.AddHandlers();

      services.Configure<MyConfig>(Configuration.GetSection("MyConfig"));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseCors("ApiCorsPolicy");

      app.UseSecurityHeaders();
      app.UseStaticFiles();
      app.UseApiExceptionHandler(options => options.AddResponseDetails = UpdateApiErrorResponse);

      app.UseHsts();
      app.UseHttpsRedirection();
      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });

      app.UseRouting();
    }

    private void UpdateApiErrorResponse(HttpContext context, Exception ex, ApiError error)
    {
      if (ex.GetType().Name == nameof(DbException)) error.Detail = "Exception was a database exception!";

      //error.Links = "https://gethelpformyerror.com/";
    }
  }
}