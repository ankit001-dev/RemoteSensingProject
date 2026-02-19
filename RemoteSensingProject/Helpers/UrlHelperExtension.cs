using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RemoteSensingProject.Helpers
{
    public static class UrlHelperExtension
    {
        public static string EncryptedAction(this UrlHelper urlhelper, string action, string controller, object routeValues)
        {
            var Dict = new RouteValueDictionary(routeValues);
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);

            foreach(var item in Dict)
            {
                queryString[item.Key] = item.Value?.ToString() ?? string.Empty;

            }

            string plainQuery = queryString.ToString();
            string encrypted = UrlEncryption.Encrypt(plainQuery);

            return urlhelper.Action(action, controller, new { q = encrypted });
        }
    }
}