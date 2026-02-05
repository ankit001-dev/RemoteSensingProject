using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using static RemoteSensingProject.Models.ApiCommon;

namespace RemoteSensingProject.Models
{
    public static class CommonHelper
    {
        //public enum ApiStatusCode
        //{
        //    Success = 200,
        //    Created = 201,
        //    NoContent = 204,
        //    BadRequest = 400,
        //    Unauthorized = 401,
        //    Forbidden = 403,
        //    NotFound = 404,
        //    Conflict = 409,
        //    UnprocessableEntity = 422,
        //    InternalServerError = 500
        //}
        public static IHttpActionResult Success<T>(
        ApiController controller,
        T data,
        string message = "Success",
        int statusCode = 200,
        PaginationInfo pagination = null)
        {
            var response = new ApiResponse<T>
            {
                StatusCode = statusCode,
                Status = true,
                Message = message,
                Pagination = pagination,
                data = data
            };

            return new ResponseMessageResult(controller.Request.CreateResponse((HttpStatusCode)statusCode, response));
        }


        public static IHttpActionResult NoData(
            ApiController controller,
            string message = "No data Found !",
            int statusCode = 200
            )
        {
            var response = new ApiResponse<Object>
            {
                StatusCode = statusCode,
                Status = true,
                Message = message,
                data = null
            };

            return new ResponseMessageResult(controller.Request.CreateResponse((HttpStatusCode)statusCode, response));
        }

        public static IHttpActionResult Error(
            ApiController controller,
            string message,
            int statusCode = 500,
            Exception exception = null)
        {
            var response = new ApiResponse<object>
            {
                StatusCode = statusCode,
                Status = false,
                Message = message ?? "An unexpected error occurred.",
                data = null
            };

            return new ResponseMessageResult(controller.Request.CreateResponse((HttpStatusCode)statusCode, response));
        }
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