using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Feature.SmartNavigation.Configuration
{
    public class SuggestionEngineConfiguration : ISuggestionEngineConfiguration
    {
        public SuggestionEngineConfiguration(string databaseFilePath, decimal itemToItemWeight, decimal parentToItemWeight, decimal parentToParentWeight)
        {
            DatabaseFilePath = databaseFilePath;
            ItemToItemWeight = itemToItemWeight;
            ParentToItemWeight = parentToItemWeight;
            ParentToParentWeight = parentToParentWeight;
        }

        public string DatabaseFilePath { get; }
        public decimal ItemToItemWeight { get; }
        public decimal ParentToItemWeight { get; }
        public decimal ParentToParentWeight { get; }
    }
}