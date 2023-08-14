using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Silerium
{
    public class JWTAuthOptions
    {
        public const string ISSUER = "VODev"; // token issuer
        public const string AUDIENCE = "Silerium"; // token consumer
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SILERIUM_JWT")));
    }
}
