using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemoteSensingProject.Models.TechnicalCell
{
    public class main
    {
        public class DynamicFormate
        {
            public int Id { get; set; } 
            public string FormatName { get; set; }
            public string TableName { get; set; }
            public List<string> ColumnName { get; set;}
        }
    }
}