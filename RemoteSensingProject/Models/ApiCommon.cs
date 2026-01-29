// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.ApiCommon
using RemoteSensingProject.Models;

namespace RemoteSensingProject.Models
{
	public class ApiCommon
	{
		public class ApiResponse<T>
		{
			public int StatusCode { get; set; }

			public bool Status { get; set; }

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