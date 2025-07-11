using FarmsApi.DataModels;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Owin;
using System;
using System.Data.SqlClient;
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
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            await Task.Run(() =>
            {
                using (var Context = new Context())
                {
                    var user = Context.Database.SqlQuery<User>(
                             "EXEC dbo.GetUser @Email,@Password",
                              new SqlParameter("@Email", context.UserName),
                              new SqlParameter("@Password", context.Password)

                         ).FirstOrDefault();

                    //var user = Context.Users.SingleOrDefault(u => u.Email == context.UserName);
                    if (user == null)
                    {
                        context.SetError("invalid_grant", "שם משתמש או סיסמה אינם נכונים");
                        return;
                    }

                    //Context.SaveChanges();

                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                    identity.AddClaim(new Claim("sub", user.Email));

                    identity.AddClaim(new Claim("UserObj", JsonConvert.SerializeObject(user)));

                    identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));

                    context.Validated(identity);
                }
            });
        }
    }
}
