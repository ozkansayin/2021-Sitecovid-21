using System;
using System.Collections.Generic;
using System.Linq;
using Feature.SmartNavigation.Configuration;
using Feature.SmartNavigation.Models;
using LiteDB;

namespace Feature.SmartNavigation.Repositories
{
    public class EntryRepository : IEntryRepository
    {
        private readonly ISuggestionEngineConfigurationProvider suggestionEngineConfigurationProvider;

        public EntryRepository(ISuggestionEngineConfigurationProvider  suggestionEngineConfigurationProvider)
        {
            this.suggestionEngineConfigurationProvider = suggestionEngineConfigurationProvider;
        }

        private const string table_entries = "entries";

        public void Insert(Guid fromId, Guid toId, int[] hitPoints, decimal calculatedHitPoint)
        {
            EntryModel model = new EntryModel
            {
                FromId = fromId,
                ToId = toId,
                HitPoint_Level1 = hitPoints[0],
                HitPoint_Level2 = hitPoints[1],
                HitPoint_Level3 = hitPoints[2],
                CalculatedHitPoint = calculatedHitPoint,
            };

            var config = suggestionEngineConfigurationProvider.Configuration;
            using (var db = new LiteDatabase(config.DatabaseFilePath))
            {
                var col = db.GetCollection<EntryModel>(table_entries);
                col.Insert(model);
            }
        }

        public IEnumerable<EntryModel> GetAll()
        {
            IEnumerable<EntryModel> items = null;
            using (var db = GetDatabase())
            {
                var col = db.GetCollection<EntryModel>(table_entries);
                items = col.FindAll()?.ToList();
            }

            return items;
        }

        public IEnumerable<EntryModel> GetTopItemsByFromPath(Guid fromId, int? count)
        {
            IEnumerable<EntryModel> items = null;
            using (var db = GetDatabase())
            {
                var col = db.GetCollection<EntryModel>(table_entries);
                items = count == null ? col.Query().Where(p=> p.FromId == fromId)?.ToList() : col.Query().Where(p => p.FromId == fromId)?.OrderByDescending(p=> p.CalculatedHitPoint).Limit(count.Value)?.ToList();
            }

            return items;
        }

        public EntryModel GetItem(Guid fromId, Guid toId)
        {
            EntryModel item = null;
            using (var db = GetDatabase())
            {
                var col = db.GetCollection<EntryModel>(table_entries);
                item = col.Query().Where(p => p.FromId == fromId && p.ToId == toId).FirstOrDefault();
            }

            return item;
        }

        public void Delete(Guid fromId, Guid toId)
        {
            using (var db = GetDatabase())
            {
                var col = db.GetCollection<EntryModel>(table_entries);
                col.DeleteMany(p => p.FromId == fromId && p.ToId == toId);
            }
        }

        public void Delete(Guid itemId)
        {
            using (var db = GetDatabase())
            {
                var col = db.GetCollection<EntryModel>(table_entries);
                col.DeleteMany(p => p.FromId == itemId || p.ToId == itemId);
            }
        }

        private LiteDatabase GetDatabase()
        {
            var config = suggestionEngineConfigurationProvider.Configuration;
            return new LiteDatabase(config.DatabaseFilePath);
        }
    }
}
