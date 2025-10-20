using System;
using System.Collections.Generic;

namespace ConsoleApp4.Sweets
{
    internal class Caramel : Sweets
    {
        public string FillingType { get; private set; }

        public Caramel(string name, Dictionary<string, int> compound, int weight, string fillingType)
            : base(name, compound, weight)
        {
            if (string.IsNullOrWhiteSpace(fillingType))
                throw new ArgumentException("Тип начинки не может быть пустым.");

            FillingType = fillingType;
        }

        public override string GetDescription()
        {
            return $"{Name} — карамель с начинкой: {FillingType}. Вес: {Weight} г, сахар: {GetSugarContent()} г.";
        }
    }
}
