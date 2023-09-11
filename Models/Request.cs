using Microsoft.EntityFrameworkCore;

namespace Silerium.Models
{
    [Index(nameof(IdempotentToken), IsUnique = true)]
    public class Request
    {
        public int Id { get; set; }
        public string IdempotentToken { get; set; }

        public static string New()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
