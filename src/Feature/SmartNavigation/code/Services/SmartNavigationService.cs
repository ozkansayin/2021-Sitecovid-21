using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Feature.SmartNavigation.Models;
using Microsoft.Extensions.Logging;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Feature.SmartNavigation.Services
{
    public class SmartNavigationService : ISmartNavigationService
    {
        private const string CacheKey = "SmartNavigation.ItemCache";
        private const string LastItemRegistryKey = "SmartNavigation.LastItem";
        private const string LastParentItemRegistryKey = "SmartNavigation.LastParentItem";
        private const int LockWaitMilliseconds = 600;

        private readonly IRegistryService registryService;
        private readonly ISuggestionEngine suggestionEngine;
        private readonly ILogger<SmartNavigationService> logger;
        private readonly int cacheSeconds;

        private static readonly object LockObject = new object();
        
        public SmartNavigationService(IRegistryService registryService, 
            ISuggestionEngine suggestionEngine,
            ILogger<SmartNavigationService> logger)
        {
            this.registryService = registryService;
            this.suggestionEngine = suggestionEngine;
            this.logger = logger;
            cacheSeconds = Settings.GetIntSetting("SmartNavigation.CacheSeconds", 10);
        }

        public void HandleItemEvent(Item item)
        {
            if (item == null)
            {
                logger.LogWarning("Smart navigation item event handler called with null item");
                return;
            }

            var itemId = item.ID.Guid;
            var parentId = item.ParentID?.Guid ?? Guid.Empty;
            var lastItemId = GetLastItem();

            if (itemId == lastItemId)
            {
                return;
            }

            SaveLastItem(itemId, parentId);

            if (lastItemId == null)
            {
                logger.LogInformation($"Ignoring relation to item {itemId} as there is no last item");
                return;
            }

            if (lastItemId.Value == itemId)
            {
                logger.LogDebug($"Ignoring relation from item to itself with id {itemId}");
                return;
            }

            var lastParentId = GetLastParentItem();
            if (lastParentId == null)
            {
                logger.LogInformation($"Ignoring relation to item {itemId} as there is no last parent item");
                return;
            }

            suggestionEngine.AddEntry(lastItemId.Value, lastParentId.Value, itemId, parentId);
        }

        public void HandleItemRemoved(Guid removedItemId)
        {
            suggestionEngine.Clear(removedItemId);
            ClearLastItem();
        }

        public SuggestionSet GetSuggestions(Item item, int count)
        {
            if (item == null)
            {
                return null;
            }

            var cacheKey = GetCacheKey(item.ID.Guid);
            var cached = HttpContext.Current?.Cache.Get(cacheKey) as SuggestionSet;

            if (cached != null)
            {
                return cached;
            }

            if (Monitor.TryEnter(LockObject, LockWaitMilliseconds))
            {
                try
                {
                    cached = HttpContext.Current?.Cache.Get(cacheKey) as SuggestionSet;

                    if (cached != null)
                    {
                        return cached;
                    }

                    cached = GenerateSuggestionSet(item, count);
                    HttpContext.Current?.Cache.Insert(cacheKey, cached, null, DateTime.Now.AddSeconds(cacheSeconds), Cache.NoSlidingExpiration);
                    return cached;
                }
                finally
                {
                    Monitor.Exit(LockObject);
                }
            }

            return null;
        }

        private SuggestionSet GenerateSuggestionSet(Item item, int count)
        {
            var itemId = item.ID.Guid;
            var parentId = GetParentItemId(item);
            var fromItemIds = suggestionEngine.PredictFromItem(itemId, parentId, count);
            var suggestionsFromItem = GetNavigationItems(item.Database, fromItemIds).ToList();

            var toItemIds = suggestionEngine.PredictToItem(itemId, parentId, count);
            var suggestionsToItem = GetNavigationItems(item.Database, toItemIds).ToList();

            var suggestions = new SuggestionSet
            {
                NavigationsFromItem = suggestionsFromItem,
                NavigationsToItem = suggestionsToItem
            };

            var lastItemId = GetLastItem();
            if (lastItemId.HasValue)
            {
                var lastItem = item.Database.GetItem(new ID(lastItemId.Value));
                suggestions.LastItem = lastItem == null ? null : MapItem(lastItem);
            }

            return suggestions;
        }

        private static IEnumerable<NavigationItem> GetNavigationItems(Database database, IEnumerable<Guid> itemIds)
        {
            foreach (var id in itemIds)
            {
                var targetItem = database.GetItem(new ID(id));
                if (targetItem == null)
                {
                    continue;
                }

                yield return MapItem(targetItem);
            }
        }

        private static NavigationItem MapItem(Item item) => new NavigationItem
        {
            ItemId = item.ID.Guid,
            Name = string.IsNullOrEmpty(item.DisplayName) ? item.Name : item.DisplayName,
            Path = item.Paths.Path
        };

        private Guid? GetLastItem() => registryService.TryGetGuid(LastItemRegistryKey);
        private Guid? GetLastParentItem() => registryService.TryGetGuid(LastParentItemRegistryKey);

        private void SaveLastItem(Guid itemId, Guid parentItemId)
        {
            registryService.SaveValue(LastItemRegistryKey, itemId.ToString("N"));
            registryService.SaveValue(LastParentItemRegistryKey, parentItemId.ToString("N"));
        }

        private void ClearLastItem()
        {
            registryService.SaveValue(LastItemRegistryKey, string.Empty);
            registryService.SaveValue(LastParentItemRegistryKey, string.Empty);
        }

        private static Guid GetParentItemId(Item item)
        {
            using (new SecurityDisabler())
            {
                return item.ParentID?.Guid ?? Guid.Empty;
            }
        }

        private static string GetCacheKey(Guid itemId) => $"{CacheKey}.{itemId.ToString("N")}";
    }
}