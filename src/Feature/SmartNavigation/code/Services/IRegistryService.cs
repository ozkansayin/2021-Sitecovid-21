using System;

namespace Feature.SmartNavigation.Services
{
    public interface IRegistryService
    {
        string GetValue(string key);
        Guid? TryGetGuid(string key);
        void SaveValue(string key, string value);
    }
}