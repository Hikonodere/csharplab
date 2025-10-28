using System;
using System.Collections.Generic;

namespace ConsoleApp4.Sweets
{
    internal class Cookie : Sweets
    {
        public string Shape { get; private set; }

        public static Cookie CreateCookieFromInput(string name, Dictionary<string, int> compound, int weight)
        {
            Console.Write("Введите форму: ");
            var shape = Console.ReadLine();
            return new Cookie(name, compound, weight, shape);
        }
        public Cookie(string name, Dictionary<string, int> compound, int weight, string shape)
            : base(name, compound, weight)
        {
            if (string.IsNullOrWhiteSpace(shape))
                throw new ArgumentException("Форма печенья должна быть указана.");

            Shape = shape;
        }

        public override string GetDescription()
        {
            return $"│ {Name,-17} │ Печенье {Shape,-23} │ {Weight,4} г │ {GetSugarContent(),4} г │";
        }
    }
}
