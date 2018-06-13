using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CognitoGroupAuthorizer;
using Microsoft.AspNetCore.Authorization;
using Amazon.DynamoDBv2;

namespace CartService
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
            // Add cognito group authorization requirements for SiteAdmin and LoggedInUser
            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("InSiteAdminGroup", policy => policy.Requirements.Add(new CognitoGroupAuthorizationRequirement("SiteAdmin")));
                    options.AddPolicy("InLoggedInUserGroup", policy => policy.Requirements.Add(new CognitoGroupAuthorizationRequirement("LoggedInUser")));
                }
            );

            // Add a single instance (singleton) of the cognito authorization handler
            services.AddSingleton<IAuthorizationHandler, CognitoGroupAuthorizationHandler>();

            services.AddMvc();
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonDynamoDB>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
