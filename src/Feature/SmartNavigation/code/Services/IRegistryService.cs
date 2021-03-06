using System;

namespace Feature.SmartNavigation.Services
{
    public interface IRegistryService
    {
        /// <summary>
        /// Get value of the item by key
        /// </summary>
        /// <param name="key">Key of the item</param>
        /// <returns>Value of the item</returns>
        string GetValue(string key);

        /// <summary>
        /// Get Guid value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Value of the item as nullable Guid</returns>
        Guid? TryGetGuid(string key);
        
        /// <summary>
        /// Set value of the item by key
        /// </summary>
        /// <param name="key">Key of the item</param>
        /// <param name="value">Value of the item to be set</param>
        void SaveValue(string key, string value);
    }
}