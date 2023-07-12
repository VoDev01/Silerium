using Silerium.Models;

namespace Silerium.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<Subcategory> Subcategories { get; set; }
        public IEnumerable<IFormFile> FormImages { get; set; } 
    }
}
