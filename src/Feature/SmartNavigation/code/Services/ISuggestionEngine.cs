using System;
using System.Collections.Generic;
using Feature.SmartNavigation.Models;

namespace Feature.SmartNavigation.Services
{
    public interface ISuggestionEngine
    {
        /// <summary>
        /// Add new entry to the weighted navigation history
        /// </summary>
        /// <param name="fromId">Item id that navigated from</param>
        /// <param name="fromParentId">Parent item id of the item navigated from</param>
        /// <param name="toId">Item id that navigated to</param>
        /// <param name="toParentId">Parent item id of the item navigated to</param>
        void AddEntry(Guid fromId, Guid fromParentId, Guid toId, Guid toParentId);
        
        /// <summary>
        /// Clear weighted navigation history that contains itemId
        /// </summary>
        /// <param name="itemId">The id of the item to remove related history items</param>
        void Clear(Guid itemId);

        /// <summary>
        /// Recalculate the weights of navigation
        /// </summary>
        void Recalculate();

        /// <summary>
        /// Get all weighted navigation history entries
        /// </summary>
        /// <returns>List of entries</returns>
        IEnumerable<EntryModel> GetAll();

        /// <summary>
        /// Predict the navigation items by the source item information
        /// </summary>
        /// <param name="fromId">Item id that navigated from</param>
        /// <param name="fromParentId">Parent id of the item navigated from </param>
        /// <param name="limit">Count of items to limit results </param>
        /// <returns>List of item id as Guid</returns>
        IEnumerable<Guid> PredictFromItem(Guid fromId, Guid fromParentId, int? limit);

        /// <summary>
        /// Predict the navigation items by the target item information
        /// </summary>
        /// <param name="toId">Item id that navigated to</param>
        /// <param name="toParentId">Parent item id of the item navigated to</param>
        /// <param name="limit">Count of items to limit results</param>
        /// <returns>List of item id as Guid</returns>
        IEnumerable<Guid> PredictToItem(Guid toId, Guid toParentId, int? limit);
    }
}