using Silerium.Models;

namespace Silerium.ViewModels
{
    public class UserViewModel
    {
        public User User { get; set; }
        public IEnumerable<Order> UserOrders { get; set; }
        public IFormFile? PfpFile { get; set; }
    }
}
