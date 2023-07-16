using Silerium.Models;

namespace Silerium.ViewModels
{
    public class SubcategoryViewModel
    {
        public Subcategory Subcategory { get; set; } = new Subcategory();
        public IEnumerable<Category> Categories { get; set; }
        public IFormFile FormImage { get; set; }
    }
}
