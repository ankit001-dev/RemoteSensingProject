using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemoteSensingProject.Models
{
    public class ApiCommon
    {
        public class ApiResponse<T>
        {
            public int StatusCode { get; set; }
            public bool Status { get; set; }  // "success" or "error"
            public string Message { get; set; }
            public T data { get; set; }
            public PaginationInfo Pagination { get; set; }
        }

        public class PaginationInfo
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalRecords { get; set; }
            public int TotalPages { get; set; }
        }
        public class FilterTypes
        {
            public int? division { get; set; }
        }
        public class ColumnMapping
        {
            public string Header { get; set; }
            public string PropertyName { get; set; }
        }
    }
}