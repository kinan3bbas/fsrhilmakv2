using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fsrhilmakv2.Extra
{
    public static class UrlHelperExtensions
    {
        //public static string AbsoluteRouteUrl(
        //this UrlHelper urlHelper,
        //string routeName,
        //object routeValues = null)
        //{
        //    string scheme = urlHelper.RequestContext.HttpContext.Request.Url.Scheme;
        //    return urlHelper.RouteUrl(routeName, routeValues, scheme);
        //}

        public static string AbsoluteRouteUrl(
        this UrlHelper url,
        string actionName,
        string controllerName,
        object routeValues = null)
    {
        return url.Action(actionName, controllerName, routeValues, url.RequestContext.HttpContext.Request.Url.Scheme);
    }
    }
}