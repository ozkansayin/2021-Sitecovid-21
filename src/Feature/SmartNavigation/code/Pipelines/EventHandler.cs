using System;
using Feature.SmartNavigation.Services;
using Microsoft.Extensions.Logging;
using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.Publishing;

namespace Feature.SmartNavigation.Pipelines
{
    public class EventHandler
    {
        private readonly ILogger<EventHandler> logger;
        private readonly ISmartNavigationService smartNavigationService;

        public EventHandler(ILogger<EventHandler> logger, 
            ISmartNavigationService smartNavigationService)
        {
            this.logger = logger;
            this.smartNavigationService = smartNavigationService;
        }

        public void OnItemDeleted(object sender, EventArgs args)
        {
            if (!(Event.ExtractParameter(args, 0) is Item deletedItem))
            {
                logger.LogWarning("Item is null on OnItemDeleted");
                return;
            }

            if (deletedItem.Database.Name.Equals("master", StringComparison.OrdinalIgnoreCase))
            {
                smartNavigationService.HandleItemRemoved(deletedItem.ID.Guid);
            }
        }

        public void OnItemSaved(object sender, EventArgs args)
        {
            var savedItem = Event.ExtractParameter(args, 0) as Item;

            if (savedItem == null)
            {
                logger.LogWarning("Item is null on OnItemSaved");
                return;
            }

            if (savedItem.Database.Name.Equals("master", StringComparison.OrdinalIgnoreCase))
            {
                smartNavigationService.HandleItemEvent(savedItem);
            }
        }

        public void OnItemPublished(object sender, EventArgs args)
        {
            if (!(args is SitecoreEventArgs sitecoreArgs))
            {
                logger.LogWarning("Could not map EventArgs to SitecoreEventArgs in OnItemPublished");
                return;
            }

            var publisher = sitecoreArgs.Parameters[0] as Publisher;
            if (publisher == null)
            {
                logger.LogWarning("Could not get Publisher in OnItemPublished");
                return;
            }

            smartNavigationService.HandleItemEvent(publisher.Options.RootItem);
        }
    }
}