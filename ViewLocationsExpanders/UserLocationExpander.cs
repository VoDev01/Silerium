using Microsoft.AspNetCore.Mvc.Razor;

namespace Silerium.ViewLocationsExpanders
{
    public class UserLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            List<string> locations = new List<string>
            {
                "~/Views/User/Authentication/{0}.cshtml"
            };
            locations.AddRange(viewLocations);
            return locations;
        }

        public void PopulateValues(ViewLocationExpanderContext context){}
    }
}
