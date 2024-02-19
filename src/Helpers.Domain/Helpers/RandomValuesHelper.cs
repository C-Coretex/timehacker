using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Domain.Helpers
{
    public static class RandomValuesHelper
    {
        public static IEnumerable<TEntry> GetRandomEntries<TEntry>(IEnumerable<(TEntry Entry, float Weight)> entries, int count)
        {
            ArgumentNullException.ThrowIfNull(entries);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than or equal to 0");

            var entriesList = entries.ToList();
            if (count > entriesList.Count)
                count = entriesList.Count;

            for (int i = 0; i < count; i++)
            {
                var totalWeight = entriesList.Sum(e => e.Weight);
                var randomValue = Random.Shared.NextDouble() * totalWeight;

                foreach (var entry in entriesList)
                {
                    randomValue -= entry.Weight;
                    if (randomValue <= 0)
                    {
                        yield return entry.Entry;
                        entriesList.Remove(entry);
                        break;
                    }
                }
            }
        }
    }
}
