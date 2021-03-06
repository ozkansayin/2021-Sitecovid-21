using System;
using System.Collections.Generic;
using Feature.SmartNavigation.Models;

namespace Feature.SmartNavigation.Repositories
{
    public interface IEntryRepository
    {
        void Insert(Guid fromId, Guid toId, int[] hitPoints, decimal calculatedHitPoint);
        IEnumerable<EntryModel> GetAll();
        IEnumerable<EntryModel> GetTopItemsByFromPath(Guid fromId, int? count);
        EntryModel GetItem(Guid fromId, Guid toId);
        void Delete(Guid fromId, Guid toId);
        void Delete(Guid itemId);
    }
}