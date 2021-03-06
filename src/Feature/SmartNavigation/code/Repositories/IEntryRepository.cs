using System;
using System.Collections.Generic;
using Feature.SmartNavigation.Models;

namespace Feature.SmartNavigation.Repositories
{
    public interface IEntryRepository
    {
        /// <summary>
        /// Insert new item to the entries table
        /// </summary>
        /// <param name="fromId">Source item id</param>
        /// <param name="toId">Destination item id</param>
        /// <param name="hitPoints">Array of hit point values</param>
        /// <param name="calculatedHitPoint">Calculated cumulative hit point value</param>
        void Insert(Guid fromId, Guid toId, int[] hitPoints, decimal calculatedHitPoint);

        /// <summary>
        /// Get all items in entries table
        /// </summary>
        /// <returns>List of EntryModel items</returns>
        IEnumerable<EntryModel> GetAll();

        /// <summary>
        /// Get top N(count) items ordered by the calculated hit point value by source item and parent of the source item
        /// </summary>
        /// <param name="fromId">Source item id</param>
        /// <param name="fromParentId">Parent item id of the source item</param>
        /// <param name="count">Limit for the result count</param>
        /// <returns>List of EntryModel items</returns>
        IEnumerable<EntryModel> GetTopItemsByFromId(Guid fromId, Guid fromParentId, int? count);

        /// <summary>
        /// Get top N(count) items ordered by the calculated hit point value by destination item and parent of the destination item
        /// </summary>
        /// <param name="toId">Destination item id</param>
        /// <param name="toParentId">Parent item id of the destination item</param>
        /// <param name="count"></param>
        /// <returns>List of EntryModel items</returns>
        IEnumerable<EntryModel> GetTopItemsByToId(Guid toId, Guid toParentId, int? count);

        /// <summary>
        /// Get item by the source item id and the destination item id
        /// </summary>
        /// <param name="fromId">Source item id</param>
        /// <param name="toId">Destination item id</param>
        /// <returns>Related item as EntryModel</returns>
        EntryModel GetItem(Guid fromId, Guid toId);

        /// <summary>
        /// Delete item by the source item and the destination item
        /// </summary>
        /// <param name="fromId">Source item id</param>
        /// <param name="toId">Destination item id</param>
        void Delete(Guid fromId, Guid toId);

        /// <summary>
        /// Delete all items related with the given item
        /// </summary>
        /// <param name="itemId">Item id to remove all related items</param>
        void Delete(Guid itemId);
    }
}