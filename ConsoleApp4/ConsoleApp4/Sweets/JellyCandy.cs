using System;
using System.Collections.Generic;

namespace ConsoleApp4.Sweets
{
    internal class JellyCandy : Sweets
    {
        public string Flavor { get; private set; }

        public JellyCandy(string name, Dictionary<string, int> compound, int weight, string flavor)
            : base(name, compound, weight)
        {
            if (string.IsNullOrWhiteSpace(flavor))
                throw new ArgumentException("Вкус должен быть указан.");

            Flavor = flavor;
        }

        public override string GetDescription()
        {
            return $"{Name} — желейная конфета со вкусом {Flavor}. Вес: {Weight} г, сахар: {GetSugarContent()} г.";
        }
    }
}
