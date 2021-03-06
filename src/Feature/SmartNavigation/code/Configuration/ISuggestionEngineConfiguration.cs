namespace Feature.SmartNavigation.Configuration
{
    public interface ISuggestionEngineConfiguration
    {
        /// <summary>
        /// LiteDB database file path in the server
        /// </summary>
        string DatabaseFilePath { get; }

        /// <summary>
        /// Weight multiplier value to use while evaluating "from item to item" relations in suggestion engine
        /// </summary>
        decimal ItemToItemWeight { get; }

        /// <summary>
        /// Weight multiplier value to use while evaluating "from parent to item" relations in suggestion engine
        /// </summary>
        decimal ParentToItemWeight { get; }

        /// <summary>
        /// Weight multiplier value to use while evaluating "from parent to parent" relations in suggestion engine
        /// </summary>
        decimal ParentToParentWeight { get; }
    }
}