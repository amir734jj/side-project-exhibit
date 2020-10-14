using static Models.Constants.ApplicationConstants;

namespace Models.Constants
{
    public class ApiConstants
    {
        public static readonly string AuthenticationSessionCookieName = ApplicationName.ToLower();

        public const string SiteEmail = "admin@anahita.dev";
        
        public const string SiteUrl = "https://www.anahita.dev";

        public static readonly string[] AdminEmail =
        {
            "admin@anahita.dev",
        };
    }
}