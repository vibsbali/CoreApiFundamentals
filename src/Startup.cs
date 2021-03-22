﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Controllers;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;

namespace CoreCodeCamp
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<CampContext>();
      services.AddScoped<ICampRepository, CampRepository>();

      services.AddAutoMapper(Assembly.GetExecutingAssembly());

      services.AddApiVersioning(opt =>
      {
          opt.AssumeDefaultVersionWhenUnspecified = true;
          opt.DefaultApiVersion = new ApiVersion(1,0);
          opt.ReportApiVersions = true;
          opt.ApiVersionReader = ApiVersionReader.Combine(
              new HeaderApiVersionReader("X-Version"),
              new QueryStringApiVersionReader("ver"));

          opt.Conventions.Controller<TalksController>()
              .Action(c => c.Delete(default(string), default(int)))
              .MapToApiVersion(1, 1);

      });

      services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(cfg =>
      {
        cfg.MapControllers();
      });
    }
  }
}
