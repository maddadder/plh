using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using plhhoa.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using plhhoa.HealthChecks;
using Lib;

namespace plhhoa
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var bindAppSecrets = new AppSecrets();
            Configuration.Bind("AppSecrets", bindAppSecrets);
            services.AddSingleton(bindAppSecrets);
            List<UserToken> tokenCache = new List<UserToken>();
            services.AddSingleton(tokenCache);
            services.AddHttpClient<UserProfileService>();
            services.AddHttpClient<UserLinkService>();
            services.AddHttpClient<UserMessageService>();
            services.AddHttpClient<GameEntryService>();
            services.AddHealthChecks().AddTypeActivatedCheck<CouchClientHealthCheck>(
                "Sample",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "sample" },
                args: new object[] {  });
            // This is required to be instantiated before the OpenIdConnectOptions starts getting configured.
            // By default, the claims mapping will map claim names in the old format to accommodate older SAML applications.
            // 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' instead of 'roles'
            // This flag ensures that the ClaimsIdentity claims collection will be built from the claims in the token.
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            services.AddMicrosoftIdentityWebAppAuthentication(Configuration, "AzureAd"); 
            services.AddControllersWithViews(options =>
            {
                /*var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));*/
            }).AddMicrosoftIdentityUI();
            services.AddHttpContextAccessor();
            services.AddSingleton<IAuthorizationMiddlewareResultHandler,
                          MyAuthorizationMiddlewareResultHandler>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // must happen before HSTS
                app.UseForwardedHeaders();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // caused the reply_url to start with https
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
