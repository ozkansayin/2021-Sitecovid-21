using System;

namespace Feature.SmartNavigation.Models
{
    public class NavigationItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Guid ItemId { get; set; }
        public string IconUrl { get; set; }
    }
}