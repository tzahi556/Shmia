using FarmsApi.DataModels;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Owin;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Security;

[assembly: OwinStartup(typeof(FarmsApi.Startup))]

namespace FarmsApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            app.UseCors(CorsOptions.AllowAll);
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            ConfigureOAuth(app);
            app.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(365),
                Provider = new SimpleAuthorizationServerProvider(),
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }
    }

    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Run(() => { context.Validated(); });
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

           

            var form = await context.Request.ReadFormAsync();
            var isfakeuser = form["isfakeuser"];

            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            await Task.Run(() =>
            {

              

                

                 int FakeuserId = Helper.ConvertToInt(isfakeuser);

               
            


                var UserResult = GetUserWithDepartments(context.UserName, context.Password, FakeuserId);

                if (UserResult.User == null)
                {
                    context.SetError("invalid_grant", "שם משתמש או סיסמה אינם נכונים");
                    return;
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                identity.AddClaim(new Claim("sub", UserResult.User.Email));




                identity.AddClaim(new Claim("UserObj", JsonConvert.SerializeObject(UserResult.User)));
                identity.AddClaim(new Claim("UserPermissions", JsonConvert.SerializeObject(UserResult.Departments)));
                //identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));

                context.Validated(identity);

                //using (var Context = new Context())
                //{
                //    var UserResultFromSql = Context.Database.SqlQuery<UserResult>(
                //             "EXEC dbo.GetUser @Email,@Password",
                //              new SqlParameter("@Email", context.UserName),
                //              new SqlParameter("@Password", context.Password)

                //         ).FirstOrDefault();

                //    //var user = Context.Users.SingleOrDefault(u => u.Email == context.UserName);
                //    if (UserResultFromSql.User == null)
                //    {
                //        context.SetError("invalid_grant", "שם משתמש או סיסמה אינם נכונים");
                //        return;
                //    }

                //    //Context.SaveChanges();

                //    var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                //    identity.AddClaim(new Claim("sub", UserResultFromSql.User.Email));




                //    identity.AddClaim(new Claim("UserObj", JsonConvert.SerializeObject(UserResultFromSql.User)));
                //    identity.AddClaim(new Claim("UserPermissions", JsonConvert.SerializeObject(UserResultFromSql.UsersDepartments)));
                //    //identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));

                //    context.Validated(identity);
                //}
            });
        }

        public UserResult GetUserWithDepartments(string email, string password,int FakeuserId)
        {


            using (var Context = new Context())
            {


                var result = new UserResult();

                // שלב 1: חילוץ connection string מתוך ה־DbContext
                var connection = (SqlConnection)Context.Database.Connection;

                // שלב 2: ודא שהחיבור פתוח
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var cmd = new SqlCommand("dbo.GetUser", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@FakeuserId", FakeuserId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        // תוצאה ראשונה: משתמש
                        if (reader.Read())
                        {
                            result.User = new UserDto
                            {
                                Id = (int)reader["Id"],
                                Email = reader["Email"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                FarmId = (int)reader["FarmId"],
                                RolesId = (int)reader["RolesId"],
                                StatusId = (int)reader["StatusId"],
                                HomePage = reader["HomePage"]?.ToString()
                            };
                        }

                        // תוצאה שנייה: רשימת מחלקות
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                result.Departments.Add(new Departments
                                {
                                    Id = (int)reader["Id"],
                                    Name = reader["Name"].ToString(),
                                    FarmId = (int)reader["FarmId"],
                                    TypeId = (int)reader["TypeId"],
                                    StatusId = (int)reader["StatusId"]
                                });
                            }

                        }
                    }
                }

                return result;
            }
        }
    }
}
