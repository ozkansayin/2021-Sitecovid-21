using System;
using Sitecore.Configuration;

namespace Feature.SmartNavigation.Configuration
{
    public class SuggestionEngineConfigurationProvider : ISuggestionEngineConfigurationProvider
    {
        private const string FilePathSettingKey = "SmartNavigation.DbFilePath";
        private const string ItemToItemWeightKey = "SmartNavigation.ItemToItemWeight";
        private const string ParentToItemWeightKey = "SmartNavigation.ParentToItemWeight";
        private const string ParentToParentWeightKey = "SmartNavigation.ParentToParentWeight";

        private readonly Lazy<ISuggestionEngineConfiguration> configuration = new Lazy<ISuggestionEngineConfiguration>(GenerateConfiguration);

        public ISuggestionEngineConfiguration Configuration => configuration.Value;

        private static ISuggestionEngineConfiguration GenerateConfiguration() => 
            new SuggestionEngineConfiguration(Settings.GetFileSetting(FilePathSettingKey), 
                (decimal)Settings.GetDoubleSetting(ItemToItemWeightKey, 1),
                (decimal)Settings.GetDoubleSetting(ParentToItemWeightKey, 0.75),
                (decimal)Settings.GetDoubleSetting(ParentToParentWeightKey, 0.5));
    }
}