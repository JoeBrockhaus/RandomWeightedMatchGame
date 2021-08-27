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

        private static List<(Item Item, (int L, int U) Range)> GetWeightedItems(IEnumerable<Item> items)
        {
            /*
             * generate a range for each item
             * given:
             *  id | weight
             *  1  | 30
             *  2  | 45
             *  ...
             *  9  | 55
             *  10 | 35
             *  
             * result: 
             *  (1, (1,30))
             *  (2, (31,66))
             *  (3, (67,112))
             *  (4, (113,168))
             */
            var offset = 0;
            return items
                .OrderBy(x => x.Weight)
                .Select(item => (Item: item, Range: (offset + 1, offset += item.Weight)))
                .ToList();
        }

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

        private static IEnumerable<T> FisherYatesShuffle<T>(IList<T> items)
        {
            // http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
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
            // generate some items
            var comms = Helper.GetItems(100, 100, ItemType.Common);
            var rares = Helper.GetItems(10, 200, ItemType.Rare);
            var epics = Helper.GetItems(1, 300, ItemType.Epic);

            for (var testRuns = 0; testRuns < 5; testRuns++)
            {
                // draw cards based on the desired distribution 
                var drawnComms = DrawRandomWeightedItemsFromPool(comms, 5).ToArray();
                var drawnRares = DrawRandomWeightedItemsFromPool(rares, 2).ToArray();
                var drawnEpics = DrawRandomWeightedItemsFromPool(epics, 1).ToArray();

                // concat + duplicate each of the items
                var unShuffledItems = Enumerable.Empty<Item>()
                    .Concat(drawnComms).Concat(drawnComms)
                    .Concat(drawnRares).Concat(drawnRares)
                    .Concat(drawnEpics).Concat(drawnEpics)
                    .ToList();

                // shuffle all the items
                var shuffledDeck = FisherYatesShuffle(unShuffledItems);

                // confirm items are only added 2x
                if (shuffledDeck.GroupBy(x => x.Id).Count() != 8)
                    throw new Exception("Same card selected twice!");

                // place items on the 4x4 board
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

            Console.ReadLine();
        }
    }

}
