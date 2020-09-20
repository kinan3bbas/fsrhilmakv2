using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using fsrhilmakv2.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;

namespace fsrhilmakv2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

                ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
                builder.EntitySet<UserWork>("UserWorks");
                builder.EntitySet<ApplicationUser>("Users");
                builder.EntitySet<Service>("Services");
                builder.EntitySet<ServicePath>("ServicePaths");
                builder.EntitySet<UserWorkBinding>("UserWorkBindings");
                //builder.EntitySet<ApplicationUser>("Users");
                builder.EntitySet<Attachment>("Attachments");
                builder.EntitySet<ServiceComment>("ServiceComments");
                builder.EntitySet<UsersDeviceTokens>("UsersDeviceTokens");
                builder.EntitySet<Transaction>("Transactions");
                builder.EntitySet<Payment>("Payments");
                builder.EntitySet<IdentityUserClaim>("Claims");

            config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling= Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }
    }
}
