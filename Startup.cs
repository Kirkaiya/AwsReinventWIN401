using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CognitoGroupAuthorizer;
using Microsoft.AspNetCore.Authorization;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.DataProtection.Repositories;
using CartService.Session;
using System;
using Microsoft.AspNetCore.DataProtection;

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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder => {
                    builder.AllowAnyOrigin();
                });
            });

            services.AddSingleton<IXmlRepository, DdbXmlRepository>();

            services.AddDistributedDynamoDbCache(o => {
                o.TableName = "TechSummitSessionState";
                o.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddSession(o => {
                o.IdleTimeout = TimeSpan.FromMinutes(30);
                o.Cookie.HttpOnly = false;
            });

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

            //add DynamoDB to DI
            services.AddAWSService<IAmazonDynamoDB>();

            //Explicitly set the DataProtection middleware to store cookie encryption keys in DynamoDB
            var sp = services.BuildServiceProvider();
            services.AddDataProtection()
                .AddKeyManagementOptions(o => o.XmlRepository = sp.GetService<IXmlRepository>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAllOrigins");
            app.UseMvc();
        }
    }
}
