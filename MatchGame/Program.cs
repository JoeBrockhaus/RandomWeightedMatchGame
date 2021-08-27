using System;
using System.Collections.Generic;
using System.Linq;

namespace MatchGame
{
    public enum ItemType { Common, Rare, Epic }
    public class Item
    {
        public int Id;
        public int Weight;
        public ItemType Type;

        public override string ToString() => $"{Id} - {Type} - {Weight}";
    }

    public static class Helper
    {
        private static Random _rand = new Random();

        public static List<Item> GetItems(int count, int start, ItemType type)
            => Enumerable
                .Range(start, count)
                .Select(id => new Item { Id = id, Type = type, Weight = _rand.Next(1, 100) })
                .ToList();
    }

    public class Program
    {
        private static Random rand = new Random();

        /// <summary>
        /// 
        /// Generates a List of Tuples of ascending-ordered 
        /// Items with their associated Weighted Ranges
        /// 
        /// given:
        ///    id  | weight
        ///    ------------
        ///     1  | 30
        ///     2  | 45
        ///     3  | 55
        ///     4  | 35
        ///     
        /// result: 
        ///     (1, (1, 30))
        ///     (2, (31, 66))
        ///     (3, (67, 112))
        ///     (4, (113, 168))
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static List<(Item Item, (int L, int U) Range)> GetWeightedItems(IEnumerable<Item> items)
        {
            var offset = 0;
            return items
                .OrderBy(x => x.Weight)
                .Select(item => (Item: item, Range: (offset + 1, offset += item.Weight)))
                .ToList();
        }

        /// <summary>
        /// Given the pool of Items + number of cards to draw, generate a list of 
        /// Item Tuples with their associated Range distributions, iterating n-times 
        /// to return the desired number of draws from the Pool.
        /// 
        /// Removes the drawn card(s) from the pool and recalculates Item Weights
        /// on each subsequent iteration.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="cardsToDraw"></param>
        /// <returns></returns>
        private static IEnumerable<Item> DrawRandomWeightedItemsFromPool(IEnumerable<Item> pool, int cardsToDraw)
        {
            var drawn = new List<Item>();
            var poolCopy = new List<Item>(pool);
            // get n items
            for (var i = 0; i < cardsToDraw; i++)
            {
                var weightedItems = GetWeightedItems(poolCopy);
                var totalWeight = poolCopy.Sum(x => x.Weight);
                // choose a random value between 1 -> totalWeight
                var rando = rand.Next(1, totalWeight + 1);
                // select the item with a matching range
                var chosenItem = weightedItems.First(wi => rando >= wi.Range.L && rando <= wi.Range.U).Item;
                // remove the item from the pool so we don't select it 2x
                var rC = poolCopy.RemoveAll(x => x.Id == chosenItem.Id);
                drawn.Add(chosenItem);
            }
            return drawn;
        }

        /// <summary>
        /// Implementation of the Fisher-Yates Shuffle pseudo-code algorithm.
        /// http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        private static IEnumerable<T> FisherYatesShuffle<T>(IList<T> items)
        {
            var copy = new List<T>(items);
            int pos = copy.Count();
            while (pos > 1)
            {
                pos--;
                int left = rand.Next(pos + 1);
                T right = copy[left];
                copy[left] = copy[pos];
                copy[pos] = right;
            }
            return copy;
        }

        static void Main(string[] args)
        {
            // 1. generate some items
            var comms = Helper.GetItems(100, 100, ItemType.Common);
            var rares = Helper.GetItems(10, 200, ItemType.Rare);
            var epics = Helper.GetItems(1, 300, ItemType.Epic);

            for (var testRuns = 0; testRuns < 5; testRuns++)
            {
                // 2. draw cards based on the desired distribution 
                var drawnComms = DrawRandomWeightedItemsFromPool(comms, 5).ToArray();
                var drawnRares = DrawRandomWeightedItemsFromPool(rares, 2).ToArray();
                var drawnEpics = DrawRandomWeightedItemsFromPool(epics, 1).ToArray();

                // 3. concat + duplicate each of the items
                var unShuffledItems = Enumerable.Empty<Item>()
                    .Concat(drawnComms).Concat(drawnComms)
                    .Concat(drawnRares).Concat(drawnRares)
                    .Concat(drawnEpics).Concat(drawnEpics)
                    .ToList();

                // 4. shuffle all the items
                var shuffledDeck = FisherYatesShuffle(unShuffledItems);

                // 4.a. confirm items are only added 2x
                if (shuffledDeck.GroupBy(x => x.Id).Count() != 8)
                    throw new Exception("Same card selected twice!");

                // 5. place items on the 4x4 board
                var board = new Item[4, 4];
                var counter = 0;
                for (var row = 0; row < 4; row++)
                {
                    Console.Write("| ");
                    for (var col = 0; col < 4; col++)
                    {
                        board[row, col] = shuffledDeck.ElementAt(counter);
                        counter++;
                        Console.Write($"{(board[row, col].Id).ToString().PadRight(3)} | ");
                    }
                    Console.WriteLine();
                }

                Console.ReadLine();
            }

            /*
             * Example Output: 
             * 
             * | 204 | 204 | 184 | 202 |
             * | 121 | 157 | 196 | 202 |
             * | 196 | 184 | 300 | 195 |
             * | 195 | 157 | 121 | 300 |
             * 
             * | 300 | 191 | 170 | 194 |
             * | 202 | 170 | 202 | 109 |
             * | 194 | 207 | 207 | 191 |
             * | 129 | 300 | 129 | 109 |
             * 
             * | 204 | 300 | 125 | 300 |
             * | 195 | 149 | 125 | 204 |
             * | 176 | 195 | 207 | 134 |
             * | 176 | 134 | 149 | 207 |
             * 
             * | 206 | 169 | 109 | 109 |
             * | 204 | 181 | 169 | 181 |
             * | 206 | 124 | 204 | 157 |
             * | 300 | 300 | 124 | 157 |
             * 
             * | 152 | 152 | 300 | 102 |
             * | 166 | 202 | 207 | 207 |
             * | 102 | 142 | 109 | 142 |
             * | 166 | 202 | 109 | 300 |
             * 
             */

            Console.ReadLine();
        }
    }

}
