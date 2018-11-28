using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;
using CognitoGroupAuthorizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.Repositories;
using ProductService.Session;
using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Swashbuckle.AspNetCore.Swagger;

namespace ProductService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder => {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowCredentials();
                });
            });

            // Add cognito group authorization requirements for SiteAdmin and RegisteredUser
            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("IsSiteAdmin", policy => policy.Requirements.Add(new CognitoGroupAuthorizationRequirement("SiteAdmin")));
                    options.AddPolicy("IsRegisteredUser", policy => policy.Requirements.Add(new CognitoGroupAuthorizationRequirement("RegisteredUser")));
                }
            );

            services.AddSingleton<IXmlRepository, DdbXmlRepository>();

            services.AddDistributedDynamoDbCache(o => {
                o.TableName = "TechSummitSessionState";
                o.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddSession(o => {
                o.IdleTimeout = TimeSpan.FromMinutes(30);
                o.Cookie.HttpOnly = true;
                o.Cookie.Domain = ".awstechsummit.com";
            });

            services.AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = "288seubnkumcdnj3odpftsvbjl";
                    options.Authority = "https://cognito-idp.us-west-2.amazonaws.com/us-west-2_megh2msxd";
                    options.RequireHttpsMetadata = false;   //set this to true for prod environments!
                    options.SaveToken = true;
                });

            // Add a single instance (singleton) of the cognito authorization handler
            services.AddSingleton<IAuthorizationHandler, CognitoGroupAuthorizationHandler>();

            //configure XRay, and register all AWS service calls to be traced
            AWSXRayRecorder.InitializeInstance(Configuration); // pass IConfiguration object that reads appsettings.json file
            AWSSDKHandler.RegisterXRayForAllServices();

            //add DynamoDB and SSM to DI
            services.AddAWSService<IAmazonDynamoDB>();
            //services.AddAWSService<IAmazonSimpleSystemsManagement>(); // only required if using ParameterStore to store keys (PsXmlRepository)

            services.AddMvc();
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());

            //Explicitly set the DataProtection middleware to store cookie encryption keys in DynamoDB
            var sp = services.BuildServiceProvider();
            services.AddDataProtection()
                .AddKeyManagementOptions(o => o.XmlRepository = sp.GetService<IXmlRepository>());
            
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("docs", new Info { Title = "re:Invent WIN401 Products API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddAWSProvider(Configuration.GetAWSLoggingConfigSection());
            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Enable middleware to serve generated Swagger as a JSON endpoint, and generate swagger-ui (HTML, etc)
            app.UseSwagger(x => x.RouteTemplate = "api/products/{documentName}/swagger.json");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/products/docs/swagger.json", "re:Invent WIN401 Products API");
                c.RoutePrefix = "api/products/swagger";
            });

            app.UseXRay("ProductService");
            app.UseAuthentication();
            app.UseCors("AllowAllOrigins");
            app.UseMvc();
        }
    }
}
