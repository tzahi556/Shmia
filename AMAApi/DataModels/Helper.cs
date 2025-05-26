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


        public static int ConvertToInt(string value)
        {

            int myInt;

            if (Int32.TryParse(value, out myInt))
            {
                return myInt;
            }

            return 0;


        }



    }
}