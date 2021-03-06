using System;
using System.Collections.Generic;
using Feature.SmartNavigation.Models;

namespace Feature.SmartNavigation.Services
{
    public interface ISuggestionEngine
    {
        void AddEntry(Guid fromId, Guid fromParentId, Guid toId, Guid toParentId);
        void Clear(Guid itemId);
        void Recalculate();
        IEnumerable<EntryModel> GetAll();
        IEnumerable<Guid> PredictFromItem(Guid fromId, Guid fromParentId, int? limit);
        IEnumerable<Guid> PredictToItem(Guid toId, Guid toParentId, int? limit);
    }
}