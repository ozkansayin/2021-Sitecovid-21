using System;
using Feature.SmartNavigation.Models;
using Sitecore.Data.Items;

namespace Feature.SmartNavigation.Services
{
    public interface ISmartNavigationService
    {
        void HandleItemEvent(Item item);
        void HandleItemRemoved(Guid removedItemId);
        SuggestionSet GetSuggestions(Item item, int count);
    }
}