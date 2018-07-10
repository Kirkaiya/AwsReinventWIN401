using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CognitoGroupAuthorizer;
using Microsoft.AspNetCore.Authorization;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System;
using Microsoft.AspNetCore.DataProtection;
using Website.Session;
using Amazon.SimpleSystemsManagement;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

            services.AddSingleton<IXmlRepository, PsXmlRepository>();

            services.AddDistributedDynamoDbCache(o => {
                o.TableName = "TechSummitSessionState";
                o.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddSession(o => {
                o.IdleTimeout = TimeSpan.FromMinutes(30);
                o.Cookie.HttpOnly = false;
            });

            // Add cognito group authorization requirements for SiteAdmin and RegisteredUser
            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("IsSiteAdmin", policy => policy.Requirements.Add(new CognitoGroupAuthorizationRequirement("SiteAdmin")));
                    options.AddPolicy("IsRegisteredUser", policy => policy.Requirements.Add(new CognitoGroupAuthorizationRequirement("RegisteredUser")));
                }
            );

            services.AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = "288seubnkumcdnj3odpftsvbjl";
                    options.Authority = "https://cognito-idp.us-west-2.amazonaws.com/us-west-2_megh2msxd";
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                });

            // Add a single instance (singleton) of the cognito authorization handler
            services.AddSingleton<IAuthorizationHandler, CognitoGroupAuthorizationHandler>();

            services.AddMvc();
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());

            //add DynamoDB and SSM to DI
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddAWSService<IAmazonSimpleSystemsManagement>();

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
