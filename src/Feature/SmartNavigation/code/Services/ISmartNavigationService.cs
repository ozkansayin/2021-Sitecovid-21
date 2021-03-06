using System;
using Feature.SmartNavigation.Models;
using Sitecore.Data.Items;

namespace Feature.SmartNavigation.Services
{
    public interface ISmartNavigationService
    {
        /// <summary>
        /// Event handler for generic item events
        /// </summary>
        /// <param name="item">Related item</param>
        void HandleItemEvent(Item item);

        /// <summary>
        /// Event handler for item removed event
        /// </summary>
        /// <param name="removedItemId">Removed item id</param>
        void HandleItemRemoved(Guid removedItemId);

        /// <summary>
        /// Get suggestions by item, limit by count
        /// </summary>
        /// <param name="item">Source item to collect suggestions related to it</param>
        /// <param name="count">Suggestion item limit count</param>
        /// <returns>Set of suggestions</returns>
        SuggestionSet GetSuggestions(Item item, int count);
    }
}