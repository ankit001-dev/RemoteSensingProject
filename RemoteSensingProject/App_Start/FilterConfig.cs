using RemoteSensingProject.Helpers;
using System.Web;
using System.Web.Mvc;

namespace RemoteSensingProject
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new DecryptQueryStringAttribute());
        }
    }
}
