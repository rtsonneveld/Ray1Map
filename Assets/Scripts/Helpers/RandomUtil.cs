using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine {
    public static class RandomUtil {

        public static T PickWeighedItem<T>(this CrossPlatformRandom r, List<(T item, float weight)> items)
        {
            if (items.Count == 0) {
                return default(T);
            }

            items.Sort((itemA, itemB) => (itemA.weight == itemB.weight) ? 0 : (itemA.weight > itemB.weight ? 1 : -1));
            float sumWeight = items.Sum(i => i.weight);
            float randomWeight = (float)r.NextDouble() * sumWeight;
            float currentSum = 0;

            int index = 1;

            foreach ((T item, float weight) entry in items) {
                currentSum += entry.weight;
                if (currentSum > randomWeight) {

                    return entry.item;
                }

                index++;
            }

            return items[0].item;
        }
    }
}
