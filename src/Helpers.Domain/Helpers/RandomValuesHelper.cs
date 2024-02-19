using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Domain.Helpers
{
    public static class RandomValuesHelper
    {
        public static IEnumerable<TEntry> GetRandomEntries<TEntry>(IEnumerable<(TEntry Entry, float Weight)> entries, int count, Random? random = null)
        {
            ArgumentNullException.ThrowIfNull(entries);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than or equal to 0");

            random ??= Random.Shared;

            var entriesList = entries.OrderBy(e => e.Weight).ToList();
            if (count > entriesList.Count)
                count = entriesList.Count;

            var totalWeight = entriesList.Sum(e => e.Weight);
            for (int i = 0; i < count; i++)
            {
                var randomValue = random.NextDouble() * totalWeight;

                foreach (var entry in entriesList)
                {
                    randomValue -= entry.Weight;
                    if (randomValue <= 0)
                    {
                        yield return entry.Entry;

                        totalWeight -= entry.Weight;
                        entriesList.Remove(entry);
                        break;
                    }
                }
            }
        }
    }
}
