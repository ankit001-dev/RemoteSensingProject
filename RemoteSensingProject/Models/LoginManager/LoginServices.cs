using System;
using System.Configuration;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocumentFormat.OpenXml.Bibliography;
using Npgsql;
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


    }
}