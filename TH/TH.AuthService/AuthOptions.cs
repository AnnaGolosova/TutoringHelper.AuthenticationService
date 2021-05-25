using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TH.AuthService
{
    public class AuthOptions
    {
        public const string ISSUER = "TH.Auth"; 
        public const string AUDIENCE = "TH.API"; 
        const string KEY = "123dsa23d21sa4f3d2"; 
        // TODO: move lifetime to serrings
        public const int LIFETIME = 5; // 5 minutes
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
