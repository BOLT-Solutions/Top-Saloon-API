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

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("Server=BOLT-PC20\\SQLEXPRESS; Database=TOPSALOON;User ID=sa;Password=A_12345;"));

          //  services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("Server = 138.201.213.62\\SQL2019; Database = TOPSALON; User ID = sa; password = P@$$w0rd; ", builder =>
          //{
          //    builder.EnableRetryOnFailure(2, TimeSpan.FromSeconds(10), null);
          //}));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<ApplicationUserManager>();

            services.AddCors(options =>
                options.AddDefaultPolicy(builder =>
                    builder.WithOrigins("http://localhost:4200", "http://localhost:4201", "http://localhost:4471", "http://localhost:4472")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));

            //services.AddCors(options =>
            //   options.AddDefaultPolicy(builder =>
            //       builder.WithOrigins("http://adminsalon.boltsmartsolutions.com",
            //                           "https://adminsalon.boltsmartsolutions.com",
            //                           "http://usersalon.boltsmartsolutions.com",
            //                           "https://usersalon.boltsmartsolutions.com")
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

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

          

            app.UseCors(policy => policy
           .AllowAnyHeader()
           .AllowAnyMethod()
           .WithOrigins("http://localhost:4200", "http://localhost:4201", "http://localhost:4471", "http://localhost:4472")
           .AllowCredentials());


            //app.UseCors(policy => policy
            //   .WithOrigins("http://adminsalon.boltsmartsolutions.com",
            //                "https://adminsalon.boltsmartsolutions.com",
            //                "http://usersalon.boltsmartsolutions.com",
            //                "https://usersalon.boltsmartsolutions.com")
            //   .WithMethods("POST", "GET", "PUT")
            //   .WithHeaders("*")
            //   .AllowCredentials()
            // );


            app.UseRouting();

            //Remove when publishing .
           // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
