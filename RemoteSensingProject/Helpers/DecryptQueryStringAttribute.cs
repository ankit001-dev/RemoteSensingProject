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

                    var query = HttpUtility.ParseQueryString(decryptedQuery);  // Check this.
                    var parameters = filterContext.ActionDescriptor
                               .GetParameters();
                    foreach (string key in query.AllKeys)
                    {
                        var param = parameters.FirstOrDefault(p => p.ParameterName == key);

                        if (param != null)
                        {
                            var targetType = param.ParameterType;

                            // Handle Nullable<T>
                            if (Nullable.GetUnderlyingType(targetType) != null)
                            {
                                targetType = Nullable.GetUnderlyingType(targetType);

                                if (string.IsNullOrEmpty(query[key]))
                                {
                                    filterContext.ActionParameters[key] = null;
                                    return;
                                }
                            }

                            var convertedValue = Convert.ChangeType(query[key], targetType);

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