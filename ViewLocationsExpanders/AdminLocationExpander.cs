using Microsoft.AspNetCore.Mvc.Razor;

namespace Silerium.ViewLocationsExpanders
{
    public class AdminLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            List<string> locations = new List<string>
            {
                "/Views/Admin/CategoriesControl/{0}.cshtml",
                "/Views/Admin/OrdersControl/{0}.cshtml",
                "/Views/Admin/SubcategoriesControl/{0}.cshtml",
                "/Views/Admin/RolesControl/{0}.cshtml",
                "/Views/Admin/PermissionsControl/{0}.cshtml",
                "/Views/Admin/ProductsControl/{0}.cshtml",
                "/Views/Admin/UsersControl/{0}.cshtml"
            };
            locations.AddRange(viewLocations);
            return locations;
        }

        public void PopulateValues(ViewLocationExpanderContext context) {}
    }
}
