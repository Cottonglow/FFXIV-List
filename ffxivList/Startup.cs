using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ffxivList.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using Auth0.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace ffxivList
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<FfListContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // Add authentication services
            services.AddAuthentication(
                options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

            // Configure OIDC
            services.Configure<OpenIdConnectOptions>(options =>
            {
                // Specify Authentication Scheme
                options.AuthenticationScheme = "Auth0";

                // Set the authority to your Auth0 domain
                options.Authority = $"https://{Configuration["auth0:domain"]}";

                // Configure the Auth0 Client ID and Client Secret
                options.ClientId = Configuration["auth0:clientId"];
                options.ClientSecret = Configuration["auth0:clientSecret"];

                // Do not automatically authenticate and challenge
                options.AutomaticAuthenticate = false;
                options.AutomaticChallenge = false;

                // Set response type to code
                options.ResponseType = "code";

                // Set the callback path, so Auth0 will call back to http://localhost:5000/signin-auth0 
                // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard 
                options.CallbackPath = new PathString("/signin-auth0");

                // Configure the Claims Issuer to be Auth0
                options.ClaimsIssuer = "Auth0";

                options.SaveTokens = true;
                
                options.Events = new OpenIdConnectEvents
                {
                    // handle the logout redirection 
                    OnRedirectToIdentityProviderForSignOut = HandleRedirectToIdentityProviderForSignOut,
                    OnTicketReceived = OnTicketReceived
                };

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("name");
                options.Scope.Add("email");
                options.Scope.Add("picture");
                options.Scope.Add("role");
                options.Scope.Add("username");
            });

            // Add framework services.
            services.AddMvc();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy => policy.Requirements.Add(new AdminRequirement("Admin")));
            });

            services.AddSingleton<IAuthorizationHandler, AdminRoleHandler>();

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            // Add the Auth0 Settings object so it can be injected
            services.Configure<Auth0Settings>(Configuration.GetSection("Auth0"));
        }

        public Task HandleRedirectToIdentityProviderForSignOut(RedirectContext context)
        {
            var logoutUri = $"https://{Configuration["auth0:domain"]}/v2/logout?client_id={Configuration["auth0:clientId"]}";

            var postLogoutUri = context.Properties.RedirectUri;
            if (!string.IsNullOrEmpty(postLogoutUri))
            {
                if (postLogoutUri.StartsWith("/"))
                {
                    // transform to absolute
                    var request = context.Request;
                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                }
                logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
            }

            context.Response.Redirect(logoutUri);
            context.HandleResponse();

            return Task.CompletedTask;
        }

        public Task OnTicketReceived(TicketReceivedContext context)
        {
            // Get the ClaimsIdentity
             var identity = context.Principal.Identity as ClaimsIdentity;
             if (identity != null)
             {
                 // Add the Name ClaimType. This is required if we want User.Identity.Name to actually return something!
                 if (!context.Principal.HasClaim(c => c.Type == ClaimTypes.Name) &&
                     identity.HasClaim(c => c.Type == "name"))
                     identity.AddClaim(new Claim(ClaimTypes.Name, identity.FindFirst("name").Value));
 
                 // Check if token names are stored in Properties
                 if (context.Properties.Items.ContainsKey(".TokenNames"))
                 {
                     // Token names a semicolon separated
                     string[] tokenNames = context.Properties.Items[".TokenNames"].Split(';');
 
                     // Add each token value as Claim
                     foreach (var tokenName in tokenNames)
                     {
                         // Tokens are stored in a Dictionary with the Key ".Token.<token name>"
                         string tokenValue = context.Properties.Items[$".Token.{tokenName}"];
 
                         identity.AddClaim(new Claim(tokenName, tokenValue));
                     }
                 }
             }
 
             return Task.CompletedTask;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, FfListContext dbContext, IOptions<Auth0Settings> auth0Settings, IOptions<OpenIdConnectOptions> options)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Add the cookie middleware
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseOpenIdConnectAuthentication(options.Value);

            app.UseStatusCodePagesWithRedirects("/Home/ErrorFound/{0}");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //DbInitializer.Initialize(dbContext);
        }
    }
}