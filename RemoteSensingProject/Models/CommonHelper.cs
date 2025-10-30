using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace RemoteSensingProject.Models
{
    public static class CommonHelper
    {
        public static List<object> SelectProperties<T>(IEnumerable<T> sourceList, IEnumerable<string> propertyNames)
        {
            var result = new List<object>();
            var propInfos = typeof(T)
                .GetProperties()
                .Where(p => propertyNames.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
                .ToList();

            foreach (var item in sourceList)
            {
                IDictionary<string, object> expando = new ExpandoObject();

                foreach (var prop in propInfos)
                {
                    var value = prop.GetValue(item);
                    expando[prop.Name] = value;
                }

                result.Add(expando);
            }

            return result;
        }
    }
}