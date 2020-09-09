using static Models.Constants.ApplicationConstants;

namespace Models.Constants
{
    public class ApiConstants
    {
        public static readonly string AuthenticationSessionCookieName = ApplicationName.ToLower();

        public const string SiteEmail = "amir@hesamian.com";
        
        public const string SiteUrl = "http://www.milwaukeeinternationals.com";

        public static readonly string[] AdminEmail =
        {
            "amirhesamyan@gmail.com"
        };
    }
}