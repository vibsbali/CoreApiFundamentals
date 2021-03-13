using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace CoreCodeCamp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CampContext>();
            services.AddScoped<ICampRepository, CampRepository>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddControllers();
            //services.AddMvc();

            //creates swagger gen
            //services.AddSwaggerGen(setupAction =>
            //{
            //    setupAction.SwaggerDoc(
            //      "LibraryOpenAPISpecification",
            //      new OpenApiInfo
            //      {
            //          Title = "Camps API",
            //          Version = "1"
            //      });
            //});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //calls into swashbuckle
            //app.UseSwagger();

            //app.UseSwaggerUI(setupAction =>
            //{
            //    setupAction.SwaggerEndpoint(
            //        "/swagger/LibraryOpenAPISpecification/swagger.json",
            //        "Library API");
            //});

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseMvc();
            app.UseEndpoints(cfg =>
            {
                cfg.MapControllers();
            });
        }
    }
}
