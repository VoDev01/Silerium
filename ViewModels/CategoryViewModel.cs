using Silerium.Models;

namespace Silerium.ViewModels
{
    public class CategoryViewModel
    {
        public Category Category { get; set; } = new Category();
        public IFormFile FormImage { get; set; }
    }
}
