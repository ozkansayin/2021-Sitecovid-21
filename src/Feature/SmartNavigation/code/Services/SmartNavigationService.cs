using System;
using System.Collections.Generic;
using Feature.SmartNavigation.Models;
using Microsoft.Extensions.Logging;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Feature.SmartNavigation.Services
{
    public class SmartNavigationService : ISmartNavigationService
    {
        private const string LastItemRegistryKey = "SmartNavigation.LastItem";
        private const string LastParentItemRegistryKey = "SmartNavigation.LastParentItem";

        private readonly IRegistryService registryService;
        private readonly ISuggestionEngine suggestionEngine;
        private readonly ILogger<SmartNavigationService> logger;

        public SmartNavigationService(IRegistryService registryService, 
            ISuggestionEngine suggestionEngine,
            ILogger<SmartNavigationService> logger)
        {
            this.registryService = registryService;
            this.suggestionEngine = suggestionEngine;
            this.logger = logger;
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
        }

        public IEnumerable<NavigationItem> GetSuggestions(Item item, int count)
        {
            var itemId = item.ID.Guid;
            var parentId = GetParentItemId(item);
            var itemIds = suggestionEngine.Predict(itemId, parentId, count);

            foreach (var id in itemIds)
            {
                var targetItem = item.Database.GetItem(new ID(id));
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

        private static Guid GetParentItemId(Item item)
        {
            using (new SecurityDisabler())
            {
                return item.ParentID?.Guid ?? Guid.Empty;
            }
        }
    }
}