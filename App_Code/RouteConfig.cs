//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : ADRIAN QUIALA

//REFERENCIAS UTILIZADAS

using Microsoft.AspNet.FriendlyUrls;
using System.Web.Routing;

namespace WebSite1
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var settings = new FriendlyUrlSettings();
            //settings.AutoRedirectMode = RedirectMode.Permanent;
            routes.EnableFriendlyUrls(settings);
        }
    }
}