using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Api.Security;
using CheatCodes.Search.DB;
using CheatCodes.Search.Logs.Extensions;
using CheatCodes.Search.Logs.Middleware;
using CheatCodes.Search.RabbitMQ.Handlers;
using CheatCodes.Search.Repositories;
using CheatCodes.Search.Security;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace CheatCodes.Search
{
  public class Startup
  {
    //public static readonly ILoggerFactory MyLoggerFactory
    //  = LoggerFactory.Create(builder => { builder.AddEventLog(); });

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
          var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
          options.EnableEndpointRouting = false;
          options.Filters.Add(new AuthorizeFilter(policy));
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

      services.AddTransient<ICategoriesSearchRepository, CategoriesSearchRepository>();
      services.AddTransient<ICategoriesChangesRepository, CategoriesChangesRepository>();
      services.AddTransient<INewCategoryEventHandler, NewCategoryEventHandler>();
      services.AddTransient<IUpdateCategoryEventHandler, UpdateCategoryEventHandler>();
      services.AddTransient<IDeleteCategoryEventHandler, DeleteCategoryEventHandler>();

      var connectionString = Configuration.GetConnectionString("CheatCodesDatabase");
      services.AddDbContext<CheatCodesDbContext>(options =>options
       // options.UseLoggerFactory(MyLoggerFactory)
          .UseSqlite(connectionString));

      services.AddDbContext<CheatCodesDbContext2>(options =>options
       // options.UseLoggerFactory(MyLoggerFactory)
          .UseSqlite(connectionString));

      //services.AddAuthentication("Bearer")
      //  .AddJwtBearer("Bearer", options =>
      //  {
      //    options.Authority = "http://localhost:5000";
      //    options.Audience = "mainApp-api";
      //    options.RequireHttpsMetadata = false;
      //  });
      services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
        .AddIdentityServerAuthentication(options =>
        {
          options.Authority = "https://localhost:5002";
          options.ApiName = "mainApp-api";
          options.RequireHttpsMetadata = false;
        });
      
      //services.AddAuthorization(options =>
      //  options.AddPolicy("RequireAuth", policy => policy.RequireAuthenticatedUser()));
      services.AddAuthorization(options =>
      {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
          .RequireAuthenticatedUser()
          .Build();
      });

      services.AddControllers().AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
      
      services.AddControllers();

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
          Version = "v1",
          Title = "CheatCodes Search API",
          Description = "Api for the search screen",
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
      app.UseAuthentication();
      
      app.UseRouting();
      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });
      app.UseSwagger();
      app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
      
      app.UseAuthorization();
      app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
    private void UpdateApiErrorResponse(HttpContext context, Exception ex, ApiError error)
    {
      if (ex.GetType().Name == typeof(SqlException).Name) error.Detail = "Exception was a database exception!";

      //error.Links = "https://gethelpformyerror.com/";
    }
  }
}