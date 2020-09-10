using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TopSaloon.API.AutoMapperConfig;
using TopSaloon.API.Extensions;
using TopSaloon.Core;
using TopSaloon.Core.Managers;
using TopSaloon.DAL;
using TopSaloon.Entities.Models;

namespace TopSaloon.API
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        { 

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("Server=BOLT-PC12\\SQLEXPRESS;Initial Catalog=TOPSALOON;Persist Security Info=False;User ID=sa;Password=P@ssw0rd;"));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<ApplicationUserManager>();

            services.AddCors(options =>
                options.AddDefaultPolicy(builder =>
                    builder.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));

            //services.AddCors(options =>
            //   options.AddDefaultPolicy(builder =>
            //       builder.WithOrigins("http://192.168.5.201:8005",
            //                           "http://192.168.5.201:8006",
            //                           "http://192.168.5.201:8009")
            //       .WithMethods("POST", "GET", "PUT")
            //       .WithHeaders("*")
            //       .AllowCredentials()
            //     ));

            services.AddAutoMapper(typeof(AutoMapperProfile));

            services.AddScoped<UnitOfWork>();

            services.AddBusinessServices();

            services.AddControllers();

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDeveloperExceptionPage();

           // app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            //app.UseCors(policy => policy
            //   .WithOrigins("http://192.168.5.201:8005",
            //               "http://192.168.5.201:8006",
            //               "http://192.168.5.201:8009")
            //   .WithMethods("POST", "GET", "PUT")
            //   .WithHeaders("*")
            //   .AllowCredentials()
            // );

            app.UseCors(policy => policy
           .AllowAnyHeader()
           .AllowAnyMethod()
           .WithOrigins("http://localhost:4200")
           .AllowCredentials());

    

            app.UseRouting();


            //Remove when publishing .
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
