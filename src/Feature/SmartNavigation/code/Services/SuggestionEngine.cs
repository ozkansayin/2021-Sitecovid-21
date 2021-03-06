using System;
using System.Collections.Generic;
using System.Linq;
using Feature.SmartNavigation.Configuration;
using Feature.SmartNavigation.Models;
using Feature.SmartNavigation.Repositories;

namespace Feature.SmartNavigation.Services
{
    public class SuggestionEngine : ISuggestionEngine
    {
        private readonly IEntryRepository entryRepository;

        private readonly decimal[] levelPowers;

        public SuggestionEngine(IEntryRepository entryRepository,
            ISuggestionEngineConfigurationProvider suggestionEngineConfigurationProvider)
        {
            this.entryRepository = entryRepository;
            var configuration = suggestionEngineConfigurationProvider.Configuration;
            levelPowers = new[] { configuration.ItemToItemWeight, configuration.ParentToItemWeight, configuration.ParentToParentWeight };
        }

        public void AddEntry(Guid fromId, Guid fromParentId, Guid toId, Guid toParentId)
        {
            AddSubEntry(fromId, toId, 1);

            if (fromParentId != toId)
            {
                AddSubEntry(fromParentId, toId, 2);
            }

            if (fromParentId != toParentId)
            {
                AddSubEntry(fromParentId, toParentId, 3);
            }

        }

        public void Clear(Guid itemId)
        {
            entryRepository.Delete(itemId);
        }

        public void Recalculate()
        {
            var items = entryRepository.GetAll();

            foreach (var item in items)
            {
                int[] hitPoints = { item.HitPoint_Level1, item.HitPoint_Level2, item.HitPoint_Level3 };
                decimal calculatedHitPoint = 0;
                for (int i = 0; i < levelPowers.Length; i++)
                {
                    calculatedHitPoint += hitPoints[i] * levelPowers[i];
                }

                entryRepository.Delete(item.FromId, item.ToId);
                entryRepository.Insert(item.FromId, item.ToId, hitPoints, calculatedHitPoint);
            }
        }

        public IEnumerable<EntryModel> GetAll()
        {
            return entryRepository.GetAll();
        }

        public IEnumerable<Guid> Predict(Guid fromId, Guid fromParentId, int limit)
        {
            var items = entryRepository.GetTopItemsByFromPath(fromId, limit);

            if (items != null && items.Count() >= limit)
            {
                return items.OrderByDescending(p => p.CalculatedHitPoint).Select(p => p.ToId);
            }

            var itemsList = items.ToList();
            int updatedLimit = limit - itemsList.Count();

            var parentResults = entryRepository.GetTopItemsByFromPath(fromParentId, updatedLimit);

            itemsList.AddRange(parentResults);

            return itemsList?.OrderByDescending(p => p.CalculatedHitPoint).Select(p => p.ToId);
        }


        private void AddSubEntry(Guid fromId, Guid toId, int level)
        {
            var entry = entryRepository.GetItem(fromId, toId);
            int[] hitPoints = new int[3];

            hitPoints[level - 1] = 1;

            if (entry != null)
            {
                hitPoints[0] += entry.HitPoint_Level1;
                hitPoints[1] += entry.HitPoint_Level2;
                hitPoints[2] += entry.HitPoint_Level3;

                decimal calculatedHitPoint = 0;

                for (int i = 0; i < levelPowers.Length; i++)
                {
                    calculatedHitPoint += hitPoints[i] * levelPowers[i];
                }

                entryRepository.Delete(fromId, toId);
                entryRepository.Insert(fromId, toId, hitPoints, calculatedHitPoint);
            }
            else
            {
                decimal calculatedHitPoint = 0;
                for (int i = 0; i < levelPowers.Length; i++)
                {
                    calculatedHitPoint += hitPoints[i] * levelPowers[i];
                }
                entryRepository.Insert(fromId, toId, hitPoints, calculatedHitPoint);
            }
        }
    }
}
