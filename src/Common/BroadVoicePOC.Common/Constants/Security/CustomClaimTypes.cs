namespace BroadVoicePOC.Common.Constants.Security
{
    public static class CustomClaimTypes
    {
        // Additional information we want to include in the JWT token, for example:
        // public const string UserCode = "user_code";

        // NL 2018-08-30: since updating to OWIN 4+ we've have strange behaviour in the JWT tokens: ClaimTypes.Role yields "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" instead of "role". similar thing happens with ClaimTypes.Name
        public const string Role = "role";
    }
}
