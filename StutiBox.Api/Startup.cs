using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using StutiBox.Api.Config;
using StutiBox.Api.Actors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.SpaServices.Webpack;

namespace StutiBox.Api
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
            services.AddControllers();
            services.AddSwaggerGen(options=>{
                options.SwaggerDoc("v2", new OpenApiInfo{Title="Stuti Box 2", Version="2.0.0"});
            });
            services.AddCors(c=>
            {
                c.AddPolicy("AllowOrigin", options => 
                {
                    options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });
            services.AddControllersWithViews()
                .AddNewtonsoftJson();
            services.AddRazorPages();
            //Spa Service
            services.AddNodeServices();
            services.AddSpaPrerenderer();

            //setup DI Here
            ConfigureService(services);
        }

        private void ConfigureService(IServiceCollection services)
        {
            //configuration
            services.Configure<LibraryConfiguration>(Configuration.GetSection("libraryConfiguration"));
            services.AddSingleton<IBassActor, BassActor>();
            services.AddSingleton<ILibraryActor,LibraryActor>();
            services.AddSingleton<IPlayerActor,PlayerActor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions{
                    HotModuleReplacement=true,
                    ReactHotModuleReplacement=true
                });
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors(options => options.AllowAnyOrigin()); 
            app.UseSwagger();
            app.UseSwaggerUI(builder=>{
                builder.SwaggerEndpoint("/swagger/v2/swagger.json", "StutiBox API V2");
            });

            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapFallbackToController("Index", "Home");
            });
        }
    }
}