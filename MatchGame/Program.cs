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

    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
        }
    }
}
