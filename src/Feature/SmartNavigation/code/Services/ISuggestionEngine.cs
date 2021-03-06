using System;
using System.Collections.Generic;
using Feature.SmartNavigation.Models;

namespace Feature.SmartNavigation.Services
{
    public interface ISuggestionEngine
    {
        void AddEntry(Guid fromId, Guid fromParentId, Guid toPath, Guid toParentId);
        IEnumerable<Guid> Predict(Guid fromId, Guid fromParentId, int limit);
        void Clear(Guid itemId);
        void Recalculate();
        IEnumerable<EntryModel> GetAll();
    }
}