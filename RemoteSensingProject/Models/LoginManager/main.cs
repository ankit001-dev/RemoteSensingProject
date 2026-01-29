// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.LoginManager.main
using System;

namespace RemoteSensingProject.Models.LoginManager
{
	public class main
	{
		public class Credentials
		{
			public string username { get; set; }

			public string password { get; set; }

			public string Emp_Name { get; set; }

			public string role { get; set; }

			public string profilePath { get; set; }

			public int userId { get; set; }

			public int Emp_Id { get; set; }

			public string Email { get; set; }

			public string confirmPassword { get; set; }

			public string newPassword { get; set; }

			public string oldPassword { get; set; }

			public string otp { get; set; }

			public Guid refreshtoken { get; set; }
		}

		public class VerifyOtpRequest
		{
			public string Email { get; set; }

			public string Otp { get; set; }
		}
	}

}