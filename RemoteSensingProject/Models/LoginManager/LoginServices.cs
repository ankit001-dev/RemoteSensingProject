using DocumentFormat.OpenXml.Bibliography;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Configuration;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static RemoteSensingProject.Models.LoginManager.main;

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
        public Credentials Login(string username, string password)
        {
            try
            {
                Credentials cr = new Credentials();

                using (var cmd = new NpgsqlCommand("SELECT * FROM sp_manageloginmaster(@action, @userid, @username, @password)", con))
                {
                    cmd.Parameters.AddWithValue("@action", "loginUser");
                    cmd.Parameters.AddWithValue("@userid", 0);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    con.Open();

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            rd.Read();
                            cr.userId = rd["userid"] != DBNull.Value ? Convert.ToInt32(rd["userid"]) : 0;
                            cr.username = rd["username"].ToString();
                            cr.password = rd["password"].ToString();
                            cr.Emp_Id = rd["emp_id"] != DBNull.Value ? Convert.ToInt32(rd["emp_id"]) : 0;
                            cr.Emp_Name = rd["name"] != DBNull.Value ? rd["name"].ToString() : "Admin";
                            cr.profilePath = rd["profile"] != DBNull.Value ? rd["profile"].ToString() : "/ProjectContent/Admin/Employee_Image/img.png";
                            cr.role = rd["userrole"].ToString();
                        }
                    }
                }

                return cr;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login: " + ex.Message, ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        public string GenerateToken(Credentials cr)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256Signature,  // Signature algorithm
                SecurityAlgorithms.Sha256Digest          // Digest algorithm
            );

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, cr.username),
                new Claim("role", cr.role),
                new Claim("userId", cr.userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Credentials ValidateUserFromEmail(string Email)
        {
            try
            {
                Credentials cr = new Credentials();
                using (var cmd = new NpgsqlCommand("SELECT * FROM sp_manageloginmaster(@action, @userid, @username, @password)", con))
                {
                    cmd.Parameters.AddWithValue("@action", "getUserRole");
                    cmd.Parameters.AddWithValue("@userid", 0);
                    cmd.Parameters.AddWithValue("@username", Email);
                    cmd.Parameters.AddWithValue("@password", "");
                    con.Open();
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            rd.Read();
                            cr.userId = rd["userid"] != DBNull.Value ? Convert.ToInt32(rd["userid"]) : 0;
                            cr.username = rd["username"].ToString();
                            cr.role = rd["userrole"].ToString();
                            cr.Email = rd["username"] != DBNull.Value ? rd["username"].ToString() : "";
                        }
                    }
                }
                return cr;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during user validation: " + ex.Message, ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        public bool ValidateUserFromEmailPassword(string Email,string oldpassword)
        {
            try
            {
                Credentials cr = new Credentials();
                using (var cmd = new NpgsqlCommand("SELECT * FROM sp_manageloginmaster(@action, @userid, @username, @password)", con))
                {
                    cmd.Parameters.AddWithValue("@action", "checkoldpassword");
                    cmd.Parameters.AddWithValue("@userid", 0);
                    cmd.Parameters.AddWithValue("@username", Email);
                    cmd.Parameters.AddWithValue("@password", oldpassword);
                    con.Open();
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during user validation: " + ex.Message, ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        public bool ChangePassword(Credentials userdata)
        {
            try
            {
                using (cmd = new NpgsqlCommand(
                    "CALL sp_login(:p_id, :p_newpassword, :p_email, :p_action)",
                    con))
                {
                    cmd.CommandType = CommandType.Text; // must be Text for CALL


                    cmd.Parameters.AddWithValue("p_id", 0);
                    cmd.Parameters.AddWithValue("p_email", userdata.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_newpassword", userdata.newPassword ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_action", "changepassword");


                    con.Open();
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}