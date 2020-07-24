using System.Text;

namespace Models.Constants
{
    public static class ApplicationConstants
    {
        public const string ApplicationName = "Anahita";

        public static readonly string ConfigFile = $"{ApplicationName.ToLower()}.json";
        
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public const string SetupUserRecipe = "UserSetup.json";
    }
}