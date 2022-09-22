using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Storage;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using NLog;
using System;
using WelNetworks.BidWel.Portal.Core.Contracts;
using WelNetworks.BidWel.Portal.Core.Hubs;
using WelNetworks.BidWel.Portal.Core.Jobs;
using WelNetworks.BidWel.Portal.Core.Services;
using WelNetworks.BidWel.Portal.Policies;

namespace WelNetworks.BidWel.Portal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LogManager.Configuration.Variables["instrumentationKey"] = Configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");
            LogManager.KeepVariablesOnReload = true;
            LogManager.Configuration.Reload();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            var styleSources = Configuration.GetValue("StartUp:StyleSources", "").Split(",");
            var scriptSources = Configuration.GetValue("StartUp:ScriptSources", "").Split(",");
            var defaultSources = Configuration.GetValue("StartUp:DefaultSources", "").Split(",");
            var imageSources = Configuration.GetValue("StartUp:ImageSources", "").Split(",");
            var allowedDestinations = Configuration.GetValue("StartUp:AllowedRedirectDestinations", "").Split(",");

            app.UseRedirectValidation(options => options.AllowedDestinations(allowedDestinations)); //Register this earlier if there's middleware that might redirect.

            // Middleware order is important - https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/index?view=aspnetcore-3.0#order
            ConfigureHangfire(app);
            if (env != null && env.EnvironmentName.Equals("Development", StringComparison.InvariantCultureIgnoreCase))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
                app.UseHsts();
            }

            //Registered before static files to always set header
            app.UseXContentTypeOptions();
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXfo(options => options.SameOrigin());

            app.UseReferrerPolicy(opts => opts.NoReferrerWhenDowngrade());

            app.UseCsp(options => options
                .DefaultSources(s => s.Self().CustomSources(defaultSources))
                .StyleSources(s => s.Self().CustomSources(styleSources).UnsafeInline())
                .ScriptSources(s => s.Self().CustomSources(scriptSources).UnsafeInline().UnsafeEval())
                .ImageSources(s => s.Self().CustomSources(imageSources))
                .FrameSources(s => s.Self())
                .ObjectSources(s => s.Self())
                .ConnectSources(s => s.Self())
            );

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseCors();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            //Registered after static files, to set headers for dynamic content.
            app.UseXfo(xfo => xfo.Deny());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });

            lifetime.ApplicationStarted.Register(LogApplicationEvent, "Application started");
            lifetime.ApplicationStopped.Register(LogApplicationEvent, "Application Stopped");
            lifetime.ApplicationStopping.Register(LogApplicationEvent, "Application Stopping");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var hstsMaxDays = Configuration.GetValue<int?>("HstsMaxDays") ?? 30;

            services.AddSignalR();


            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApp(Configuration, "AzureAd", "OpenIdConnect", "Cookies");

            services.AddAuthorization(options => options.AddPolicy("BidWelRoleRequired", policy => policy.Requirements.Add(new BidWelRoleRequirement())));

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.EnableEndpointRouting = false;
            });


            services.AddTransient<IDataAccess, DataAccessService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IHeartbeatJob, HeartbeatCheck>();

            services.AddCors();
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(hstsMaxDays);
            });
            var hangfireEnabled = Configuration.GetValue("HangfireEnabled", true);
            if (hangfireEnabled)
            {
                services.AddHangfire(configuration => configuration
                   .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                   .UseSimpleAssemblyNameTypeSerializer()
                   .UseRecommendedSerializerSettings()
                   .UseSqlServerStorage(Configuration.GetConnectionString("PORTAL"), new SqlServerStorageOptions
                   {
                       SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                       QueuePollInterval = TimeSpan.Zero,
                       UseRecommendedIsolationLevel = true,
                       UsePageLocksOnDequeue = true,
                       CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                       DisableGlobalLocks = true,
                       SchemaName = "hangfire",
                   }));

                services.AddHangfireServer();
            }
            services.AddApplicationInsightsTelemetry();
        }

        internal void ConfigureHangfire(IApplicationBuilder app)
        {
            var hangfireEnabled = Configuration.GetValue("HangfireEnabled", true);
            if (hangfireEnabled)
            {
                var hangfireRoute = Configuration.GetValue("HangfireUIRoute", "");

                var enableHangfireUI = !string.IsNullOrWhiteSpace(hangfireRoute);
                if (enableHangfireUI)
                {
                    app.UseHangfireDashboard(hangfireRoute, new DashboardOptions { Authorization = new[] { new HangfireAuthorizationFilter() } });
                }

                var hangfireRebuildJobs = Configuration.GetValue("HangfireRebuildJobs", false);

                if (hangfireRebuildJobs)
                {
                    using var connection = JobStorage.Current.GetConnection();
                    foreach (var recurringJob in StorageConnectionExtensions.GetRecurringJobs(connection))
                    {
                        RecurringJob.RemoveIfExists(recurringJob.Id);
                    }
                }

                var heartbeat = Configuration.GetValue("HangfireHeartbeatExpression", "* * * * * *");
                RecurringJob.AddOrUpdate<IHeartbeatJob>(j => j.ExecuteAsync(true), heartbeat, null);
            }
        }

        private void LogApplicationEvent(object message)
        {
            LogManager.GetCurrentClassLogger().Info(message as string);
            LogManager.Flush();
        }
    }
}