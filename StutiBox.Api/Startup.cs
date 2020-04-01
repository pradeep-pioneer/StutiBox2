using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StutiBox.Api.Config;
using StutiBox.Api.Actors;
using StutiBox.Api.Hubs;
using Microsoft.AspNetCore.SpaServices.Webpack;
using StutiBox.Api.Workers;
using System;

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
            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v2", new OpenApiInfo { Title = "Stuti Box 2", Version = "2.0.0" });
            });
            services.AddCors(c =>
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
            /*
             * ToDo: Remove this junk if no longer needed
            //Spa Service
            services.AddNodeServices();
            services.AddSpaPrerenderer();
            */
            services.AddLogging();

            //setup DI Here
            ConfigureService(services);
        }

        private void ConfigureService(IServiceCollection services)
        {
            var envVar = Environment.GetEnvironmentVariable("DEVICE");
            var device = string.IsNullOrWhiteSpace(envVar) ? "mac" : envVar;
            //configuration
            services.Configure<LibraryConfiguration>(Configuration.GetSection("libraryConfiguration"));
            services.AddSingleton<IBassActor, BassActor>();
            services.AddSingleton<ILibraryActor, LibraryActor>();
            services.AddSingleton<IPlayerActor, PlayerActor>();
            services.AddSingleton<IShutdownActor, ShutdownActor>();
            services.AddSignalR();
            if (device != "mac")
            {
                services.AddHostedService<PushButtonMonitor>();
            }

            services.AddSingleton<MorningAlarmWorker>();
            services.AddHostedService<StatusNotificationWorker>();

            services.AddHostedService(provider => provider.GetService<MorningAlarmWorker>());
            services.AddHostedService<RestartWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                /*
                 * ToDo: Remove this junk if no longer needed
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });*/
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors(options => options.AllowAnyOrigin());
            app.UseSwagger();
            app.UseSwaggerUI(builder =>
            {
                builder.SwaggerEndpoint("/swagger/v2/swagger.json", "StutiBox API V2");
            });

            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapFallbackToController("Index", "Home");
                endpoints.MapHub<PlayerStatusHub>("/playerstatus");
                endpoints.MapHub<LibraryStatusHub>("/librarystatus");
            });
        }
    }
}
