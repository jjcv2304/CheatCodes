using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Api.Logs.Extensions;
using Api.Logs.Filters;
using Api.Logs.Middleware;
using Api.Security;
using Api.Utils;
using Application.Utils;
using Application.Utils.Interfaces;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
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
      services.AddSingleton<IScopeInformation, ScopeInformation>();

      services.AddCors(o =>
      {
        o.AddPolicy("ApiCorsPolicy", builder =>
        {
          builder.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(origin => origin == "http://localhost:4200")
            .AllowCredentials();
        });
      });

      services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
        .AddIdentityServerAuthentication(options =>
        {
          //options.Authority = "http://localhost:5000";
          //options.Authority = "https://localhost:5002";
          //options.Authority = "https://sts";
          options.Authority = "http://localhost:5002";
          options.ApiName = "mainApp-api";
          options.RequireHttpsMetadata = false;
        });

      services.AddMvc(options =>
      {
        var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
        options.Filters.Add(typeof(TrackActionPerformanceFilter));
      }
      ).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


      var connectionString = Configuration.GetConnectionString("CheatCodesDatabase");
      var con = new DatabaseSetting(connectionString);
      services.AddSingleton(con);
      var queriesConnectionString = new QueriesConnectionString(connectionString);
      services.AddSingleton(queriesConnectionString);
      services.AddTransient<IUnitOfWork, UnitOfWork>();
      services.AddTransient<ICategoryQueryRepository, CategoryQueryRepository>();
      services.AddTransient<ICategoryCommandRepository, CategoryCommandRepository>();


      services.AddSingleton<Messages>();


      services.AddHandlers();

      // Register the Swagger generator, defining 1 or more Swagger documents
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
          Version = "v1",
          Title = "CheatCodes API",
          Description = "A simple example ASP.NET Core Web API",
          TermsOfService = new Uri("https://example.com/terms"),
          Contact = new OpenApiContact
          {
            Name = "Juan",
            Email = "jj@fakemail.com"
          },
          License = new OpenApiLicense
          {
            Name = "Use under ...",
            Url = new Uri("https://example.com/license")
          }
        });
          // Set the comments path for the Swagger JSON and UI.
          var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
      });
      services.Configure<MyConfig>(Configuration.GetSection("MyConfig"));
      ConfigureAdditionalServices(services);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseCors("ApiCorsPolicy");

      app.UseSecurityHeaders();
      app.UseStaticFiles();
      app.UseApiExceptionHandler(options => options.AddResponseDetails = UpdateApiErrorResponse);

      app.UseSwagger();
      app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

      //app.UseHsts();
      //app.UseHttpsRedirection();
      app.UseAuthentication();
      app.UseMvc();
    }

    protected virtual void ConfigureAdditionalServices(IServiceCollection services)
    {
    }

    private void UpdateApiErrorResponse(HttpContext context, Exception ex, ApiError error)
    {
      if (ex.GetType().Name == typeof(SqlException).Name) error.Detail = "Exception was a database exception!";

      //error.Links = "https://gethelpformyerror.com/";
    }
  }
}