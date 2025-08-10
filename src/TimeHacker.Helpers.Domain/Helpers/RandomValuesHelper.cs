namespace TimeHacker.Helpers.Domain.Helpers
{
    public static class RandomValuesHelper
    {
        public static IEnumerable<TEntry> GetRandomEntries<TEntry>(IEnumerable<(TEntry Entry, float Weight)> entries, int count, Random? random = null)
        {
            ArgumentNullException.ThrowIfNull(entries);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than or equal to 0");

            random ??= Random.Shared;

            var entriesList = entries.Where(e => e.Weight > 0).OrderBy(e => e.Weight).ToList();
            if (count > entriesList.Count)
                count = entriesList.Count;

            var totalWeight = entriesList.Sum(e => e.Weight);
            for (var i = 0; i < count; i++)
            {
                var randomValue = random.NextDouble() * totalWeight;

                for (var j = 0; i < entriesList.Count; j++)
                {
                    var entry = entriesList[j];
                    randomValue -= entry.Weight;
                    if (randomValue > 0) 
                        continue;

                    yield return entry.Entry;
                    totalWeight -= entry.Weight;
                    entriesList.RemoveAt(j);
                    break;
                }
            }
        }
    }
}
