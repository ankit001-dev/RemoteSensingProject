using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RemoteSensingProject.Helpers
{
    public class DecryptQueryStringAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var encrypted = filterContext.HttpContext.Request.QueryString["q"];

            if (!string.IsNullOrWhiteSpace(encrypted))
            {
                try
                {
                    string decryptedQuery = UrlEncryption.Decrypt(encrypted);

                    var query = HttpUtility.ParseQueryString(decryptedQuery);
                    var parameters = filterContext.ActionDescriptor
                               .GetParameters();
                    foreach (string key in query.AllKeys)
                    {
                        var param = parameters.FirstOrDefault(p => p.ParameterName == key);
                        if (param != null)
                        {
                            var convertedValue = Convert.ChangeType(
                                query[key],
                                param.ParameterType
                            );

                            filterContext.ActionParameters[key] = convertedValue;
                        }
                    }
                }
                catch
                {
                    filterContext.Result = new HttpStatusCodeResult(400);
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}