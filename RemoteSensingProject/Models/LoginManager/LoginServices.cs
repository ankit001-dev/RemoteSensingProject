// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.LoginManager.LoginServices
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RemoteSensingProject.Models.LoginManager
{
	public class LoginServices : DataFactory
	{
		private readonly string _secretKey;

		private readonly string _issuer;

		private readonly string _audience;

		public LoginServices()
		{
			_secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
			_issuer = ConfigurationManager.AppSettings["JwtIssuer"];
			_audience = ConfigurationManager.AppSettings["JwtAudience"];
		}

		public main.Credentials Login(string username, string password)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			try
			{
				main.Credentials cr = new main.Credentials();
				NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM sp_manageloginmaster(@action, @userid, @username, @password)", con);
				try
				{
					cmd.Parameters.AddWithValue("@action", (object)"loginUser");
					cmd.Parameters.AddWithValue("@userid", (object)0);
					cmd.Parameters.AddWithValue("@username", (object)username);
					cmd.Parameters.AddWithValue("@password", (object)password);
					((DbConnection)(object)con).Open();
					NpgsqlDataReader rd = cmd.ExecuteReader();
					try
					{
						if (((DbDataReader)(object)rd).HasRows)
						{
							((DbDataReader)(object)rd).Read();
							var roleValue = rd["userrole"];
							cr.userId = ((((DbDataReader)(object)rd)["userid"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)rd)["userid"]) : 0);
							cr.username = ((DbDataReader)(object)rd)["username"].ToString();
							cr.password = ((DbDataReader)(object)rd)["password"].ToString();
							cr.Emp_Id = ((((DbDataReader)(object)rd)["emp_id"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)rd)["emp_id"]) : 0);
							cr.Emp_Name = ((((DbDataReader)(object)rd)["name"] != DBNull.Value) ? ((DbDataReader)(object)rd)["name"].ToString() : "Admin");
							cr.profilePath = ((((DbDataReader)(object)rd)["profile"] != DBNull.Value) ? ((DbDataReader)(object)rd)["profile"].ToString() : "/ProjectContent/Admin/Employee_Image/img.png");
							cr.role = roleValue != DBNull.Value
								? roleValue.ToString()
									.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
									.Select(r => r.Trim())
									.ToArray()
								: Array.Empty<string>();
						}
					}
					finally
					{
						((IDisposable)rd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				return cr;
			}
			catch (Exception ex)
			{
				throw new Exception("Error during login: " + ex.Message, ex);
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
			}
		}

		public string GenerateToken(main.Credentials cr)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Expected O, but got Unknown
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
			SigningCredentials creds = new SigningCredentials((SecurityKey)(object)key, "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256", "http://www.w3.org/2001/04/xmlenc#sha256");
            var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, cr.username),
				new Claim("userId", cr.userId.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};
            foreach (var role in cr.role)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            string issuer = _issuer;
			string audience = _audience;
			DateTime? dateTime = DateTime.Now.AddMinutes(1.0);
			SigningCredentials val = creds;
			JwtSecurityToken token = new JwtSecurityToken(issuer, audience, (IEnumerable<Claim>)claims, (DateTime?)null, dateTime, val);
			return ((SecurityTokenHandler)new JwtSecurityTokenHandler()).WriteToken((SecurityToken)(object)token);
		}

		public main.Credentials ValidateUserFromEmail(string Email)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			try
			{
				main.Credentials cr = new main.Credentials();
				NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM sp_manageloginmaster(@action, @userid, @username, @password)", con);
				try
				{
					cmd.Parameters.AddWithValue("@action", (object)"getUserRole");
					cmd.Parameters.AddWithValue("@userid", (object)0);
					cmd.Parameters.AddWithValue("@username", (object)Email);
					cmd.Parameters.AddWithValue("@password", (object)"");
					((DbConnection)(object)con).Open();
					NpgsqlDataReader rd = cmd.ExecuteReader();
					try
					{
						if (((DbDataReader)(object)rd).HasRows)
						{
							var roleValue = rd["userrole"];
                            ((DbDataReader)(object)rd).Read();
							cr.userId = ((((DbDataReader)(object)rd)["userid"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)rd)["userid"]) : 0);
							cr.username = ((DbDataReader)(object)rd)["username"].ToString();
							cr.role = roleValue != DBNull.Value ? roleValue.ToString()
								.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
								.Select(r => r.Trim())
								.ToArray()
							: Array.Empty<string>();
							cr.Email = ((((DbDataReader)(object)rd)["username"] != DBNull.Value) ? ((DbDataReader)(object)rd)["username"].ToString() : "");
						}
					}
					finally
					{
						((IDisposable)rd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				return cr;
			}
			catch (Exception ex)
			{
				throw new Exception("Error during user validation: " + ex.Message, ex);
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
			}
		}

		public bool ValidateUserFromEmailPassword(string Email, string oldpassword)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			try
			{
				main.Credentials cr = new main.Credentials();
				NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM sp_manageloginmaster(@action, @userid, @username, @password)", con);
				try
				{
					cmd.Parameters.AddWithValue("@action", (object)"checkoldpassword");
					cmd.Parameters.AddWithValue("@userid", (object)0);
					cmd.Parameters.AddWithValue("@username", (object)Email);
					cmd.Parameters.AddWithValue("@password", (object)oldpassword);
					((DbConnection)(object)con).Open();
					NpgsqlDataReader rd = cmd.ExecuteReader();
					try
					{
						if (((DbDataReader)(object)rd).HasRows)
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)rd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				return false;
			}
			catch (Exception ex)
			{
				throw new Exception("Error during user validation: " + ex.Message, ex);
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
			}
		}

		public bool ChangePassword(main.Credentials userdata)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			//IL_001a: Expected O, but got Unknown
			try
			{
				NpgsqlCommand val = new NpgsqlCommand("CALL sp_login(:p_id, :p_newpassword, :p_email, :p_action)", con);
				NpgsqlCommand val2 = val;
				cmd = val;
				NpgsqlCommand val3 = val2;
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("p_id", (object)0);
					cmd.Parameters.AddWithValue("p_email", ((object)userdata.Email) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_newpassword", ((object)userdata.newPassword) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_action", (object)"changepassword");
					((DbConnection)(object)con).Open();
					((DbCommand)(object)cmd).ExecuteNonQuery();
					return true;
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}

}