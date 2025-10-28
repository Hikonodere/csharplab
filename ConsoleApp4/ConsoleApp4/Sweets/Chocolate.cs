using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ConsoleApp4.Sweets
{
    internal class Chocolate : Sweets
    {
        public int CocoaPercent { get; private set; }

        public Chocolate(string name, Dictionary<string, int> compound, int weight, int cocoaPercent)
            : base(name, compound, weight)
        {
            if (cocoaPercent < 0 || cocoaPercent > 100)
                throw new ArgumentException("Процент какао должен быть в диапазоне 0–100.");

            CocoaPercent = cocoaPercent;
        }

        public override string GetDescription()
        {
            return $"│ {Name,-17} │ Шоколад {CocoaPercent,2}% какао {new string(' ',13)} │ {Weight,4} г │ {GetSugarContent(),4} г │";
        }
    }
}