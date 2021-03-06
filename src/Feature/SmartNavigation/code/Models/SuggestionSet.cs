using System.Collections.Generic;
using System.Linq;

namespace Feature.SmartNavigation.Models
{
    public class SuggestionSet
    {
        public SuggestionSet()
        {
            NavigationsToItem = Enumerable.Empty<NavigationItem>();
            NavigationsFromItem = Enumerable.Empty<NavigationItem>();
        }

        public IEnumerable<NavigationItem> NavigationsToItem { get; set; }
        public IEnumerable<NavigationItem> NavigationsFromItem { get; set; }
        public NavigationItem LastItem { get; set; }
    }
}