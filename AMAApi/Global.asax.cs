using FarmsApi.Services;
using System;
using System.Web;
using System.Web.Caching;

namespace FarmsApi
{
    public class Global : System.Web.HttpApplication
    {
     //   private static CacheItemRemovedCallback OnCacheRemove = null;
        protected void Application_Start(object sender, EventArgs e)
        {
          
        }   

     
        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}