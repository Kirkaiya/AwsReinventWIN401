using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartService.Session
{
    public static class AppAuthExtensions
    {
        public static IApplicationBuilder MergeAuthorizationHeaderWithSession(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (!context.Session.IsAvailable)
                    context.Session.LoadAsync().Wait();

                //if no JWT passed, but there is one stored in session, inject it in header to allow authorization based on that token
                if (!context.Request.Headers.ContainsKey("Authorization") && context.Session.Keys.Contains("Authorization"))
                {
                    context.Request.Headers.Add("Authorization", context.Session.GetString("Authorization"));
                }
                else if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    //if JWT is passed, store/update the one in session
                    context.Session.SetString("Authorization", context.Request.Headers["Authorization"]);
                }

                await next();
            });

            return app;
        }
    }
}
