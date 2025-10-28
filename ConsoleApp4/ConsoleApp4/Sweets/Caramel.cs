using System;
using System.Collections.Generic;

namespace ConsoleApp4.Sweets
{
    internal class Caramel : Sweets
    {
        public string FillingType { get; private set; }
        public static Caramel CreateCaramelFromInput(string name, Dictionary<string, int> compound, int weight)
        {
            Console.Write("Введите тип начинки: ");
            var filling = Console.ReadLine();
            return new Caramel(name, compound, weight, filling);
        }

        public Caramel(string name, Dictionary<string, int> compound, int weight, string fillingType)
            : base(name, compound, weight)
        {
            if (string.IsNullOrWhiteSpace(fillingType))
                throw new ArgumentException("Тип начинки не может быть пустым.");

            FillingType = fillingType;
        }

        public override string GetDescription()
        {
            return $"│ {Name,-17} │ Карамель {FillingType,-22} │ {Weight,4} г │ {GetSugarContent(),4} г │";
        }
    }
}
