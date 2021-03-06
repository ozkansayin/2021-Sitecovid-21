using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public IEnumerable<EntryModel> GetTopItemsByFromId(Guid fromId, Guid fromParentId, int? count) =>
            GetTopItemsById(fromId, fromParentId, count, true);

        public IEnumerable<EntryModel> GetTopItemsByToId(Guid toId, Guid toParentId, int? count) =>
        GetTopItemsById(toId, toParentId, count, false);

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

        private IEnumerable<EntryModel> GetTopItemsById(Guid itemId, Guid itemParentId, int? count, bool isFromQuery)
        {
            using (var db = GetDatabase())
            {
                var collection = db.GetCollection<EntryModel>(table_entries);
                var query = (isFromQuery
                    ? GetQueryByFromId(collection, itemId, itemParentId)
                    : GetQueryByToId(collection, itemId, itemParentId));
                query = query.OrderByDescending(p => p.CalculatedHitPoint);
                Func<EntryModel, Guid> groupByExpression = null;
                if (isFromQuery)
                {
                    groupByExpression = x => x.ToId;
                }
                else
                {
                    groupByExpression = x => x.FromId;
                }

                if (count.HasValue)
                {
                    return query.Limit(count.Value * 2).ToList().GroupBy(groupByExpression).Select(x => x.First()).Take(count.Value);
                }

                return query.ToList().GroupBy(groupByExpression).Select(x => x.First());
            }
        }

        private ILiteQueryable<EntryModel> GetQueryByFromId(ILiteCollection<EntryModel> collection, Guid fromId, Guid fromParentId) =>
            collection.Query().Where(p => (p.FromId == fromId || p.FromId == fromParentId) && p.ToId != fromId);

        private ILiteQueryable<EntryModel> GetQueryByToId(ILiteCollection<EntryModel> collection, Guid toId, Guid toParentId) =>
            collection.Query().Where(p => (p.ToId == toId || p.ToId == toParentId) && p.FromId != toId);
    }
}
