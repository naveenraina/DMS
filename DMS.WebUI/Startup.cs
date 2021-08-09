using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using DMS.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ReflectionIT.Mvc.Paging;

namespace DMS
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.LoginPath = new PathString("/Auth/Index/");
                        options.AccessDeniedPath = new PathString("/Auth/Forbidden/");
                    });

            services.AddDbContext<DMSContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DmsDb"), b=>b.UseRowNumberForPaging()));
            
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromDays(30);
                
            });
            services.AddSingleton<IFileProvider>(
            new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddMvc();
            services.AddPaging();
            services.AddAuthorization(options => {
               
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "Admin");
                });
                options.AddPolicy("User", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "User");
                });

            });


        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Auth/Error");
            }
            app.UseAuthentication();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    if (!ctx.Context.User.Identity.IsAuthenticated && ctx.Context.Request.Path.Value.Contains("/Documents/"))
                    {
                        // respond HTTP 401 Unauthorized, and...
                        //ctx.Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                        ctx.Context.Response.Redirect("/");

                        // Append following 2 lines to drop body from static files middleware!
                        ctx.Context.Response.ContentLength = 0;
                        ctx.Context.Response.Body = Stream.Null;

                        ctx.Context.Response.Headers.Add("Cache-Control", "no-store");
                    }
                }
            });
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Auth}/{action=Index}/{id?}");
            });


        }
    }
}
