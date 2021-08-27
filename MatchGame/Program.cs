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

        public static List<Item> GetItems(int count, int start, ItemType type)
            => Enumerable
                .Range(start, count)
                .Select(id => new Item { Id = id, Type = type, Weight = _rand.Next(1, 100) })
                .ToList();
    }

    public class Program
    {
        static void Main(string[] args)
        {
            // generate some items
            var comms = Helper.GetItems(100, 100, ItemType.Common);
            var rares = Helper.GetItems(10, 200, ItemType.Rare);
            var epics = Helper.GetItems(1, 300, ItemType.Epic);
            

            Console.ReadLine();
        }
    }

}
