using System;
using System.Collections.Generic;

namespace ConsoleApp4.Sweets
{
    internal class JellyCandy : Sweets
    {
        public string Flavor { get; private set; }

        public static JellyCandy CreateJellyFromInput(string name, Dictionary<string, int> compound, int weight)
        {
            Console.Write("Введите вкус: ");
            var flavor = Console.ReadLine();
            return new JellyCandy(name, compound, weight, flavor);
        }

        public JellyCandy(string name, Dictionary<string, int> compound, int weight, string flavor)
            : base(name, compound, weight)
        {
            if (string.IsNullOrWhiteSpace(flavor))
                throw new ArgumentException("Вкус должен быть указан.");

            Flavor = flavor;
        }

        public override string GetDescription()
        {
            return $"│ {Name,-17} │ Желейная {Flavor,-22} │ {Weight,4} г │ {GetSugarContent(),4} г │";
        }
    }
}
