using System;
using System.Web.Services.Discovery;

namespace FarmsApi.DataModels
{
    public static class Helper
    {
       

        public static bool ConvertToBool(string value)
        {
           
            Boolean myBool;

            if (Boolean.TryParse(value, out myBool))
            {
                return myBool;
            }

            return false;


        }



    }
}