namespace Feature.SmartNavigation.Configuration
{
    public interface ISuggestionEngineConfiguration
    {
        string DatabaseFilePath { get; }
        decimal ItemToItemWeight { get; }
        decimal ParentToItemWeight { get; }
        decimal ParentToParentWeight { get; }
    }
}