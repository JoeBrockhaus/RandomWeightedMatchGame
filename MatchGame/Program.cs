using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static List<Item> GetItems(int numCommons, int numRares, int numEpics)
            => Enumerable
                .Range(1, numCommons + numRares + numEpics)
                .Select(id =>
                    new Item
                    {
                        Id = id,
                        Type = GetItemType(numCommons, numRares, id),
                        Weight = GetItemType(numCommons, numRares, id) == ItemType.Epic ? 100 : _rand.Next(1, 100)
                    })
                .ToList();

        private static ItemType GetItemType(int numCommons, int numRares, int id)
            =>
                id <= numCommons
                    ? ItemType.Common
                    : id > numCommons && id <= numCommons + numRares
                        ? ItemType.Rare
                        : ItemType.Epic;

    }

    public class GameConfig
    {
        public (int Total, int ToSelect) Common { get; set; } = (20, 5);
        public (int Total, int ToSelect) Rare { get; set; } = (10, 2);
        public (int Total, int ToSelect) Epic { get; set; } = (1, 1);
    }
    public class WeightedItemGame
    {
        private readonly Random Rand = new Random();
        private readonly List<Item> Items;
        private readonly GameConfig Config;

        public WeightedItemGame() : this(new GameConfig()) { }
        public WeightedItemGame(GameConfig config)
        {
            Config = config;
            Items = Helper.GetItems(Config.Common.Total, Config.Rare.Total, Config.Epic.Total);
            AllItems = new List<Item>(Items);
        }

        public List<Item> AllItems { get; set; }

        private Item DrawWeightedItem()
        {
            // sort by weight
            var sortedItems = Items.OrderBy(x => x.Weight);
            // total weight of all items 
            var totalWeight = sortedItems.Sum(x => x.Weight);
            while (true)
            {
                var choice = Rand.Next(totalWeight);
                foreach (var item in Items)
                {
                    // if the choice is less than the current item's weight, select it
                    if (choice < item.Weight)
                        return item;
                    // decrement the choice by the current-item's weight
                    choice -= item.Weight;
                }
            }
        }

        public IEnumerable<Item> GetWeightedItems()
        {
            // keep track of the number of items we've drawn
            var commons = 0;
            var rares = 0;
            var epics = 0;

            while (true)
            {
                // should the draw be across all items or within each category of type?
                var draw = DrawWeightedItem();
                if (draw.Type == ItemType.Common && commons < Config.Common.ToSelect)
                {
                    commons++;
                    yield return draw;
                    yield return draw;
                    Items.Remove(draw);
                }
                else if (draw.Type == ItemType.Rare && rares < Config.Rare.ToSelect)
                {
                    rares++;
                    yield return draw;
                    yield return draw;
                    Items.Remove(draw);
                }
                else if (draw.Type == ItemType.Epic && epics < Config.Epic.ToSelect)
                {
                    epics++;
                    yield return draw;
                    yield return draw;
                    Items.Remove(draw);
                }

                if (commons == (int)ItemType.Common
                    && rares == (int)ItemType.Rare
                    && epics == (int)ItemType.Epic)
                { yield break; }
            }
        }

    }

    public class Program
    {
        static void Main(string[] args)
        {
            var game = new WeightedItemGame();
            var randomItems = game.GetWeightedItems().ToList().OrderBy(x => new Guid()).ToList();

        }
    }
}
