using Silerium.Models;

namespace Silerium.ViewModels
{
    public class UserViewModel
    {
        public User User { get; set; }
        public IFormFile? PfpFile { get; set; }
    }
}
