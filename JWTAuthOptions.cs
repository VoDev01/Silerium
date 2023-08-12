using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Silerium
{
    public class JWTAuthOptions
    {
        public const string ISSUER = "VODev"; // token issuer
        public const string AUDIENCE = "Silerium"; // token consumer
        const string KEY = "silerium_secret123";   // crypt key
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}