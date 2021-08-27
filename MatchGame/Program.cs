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
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
        }
    }
}
