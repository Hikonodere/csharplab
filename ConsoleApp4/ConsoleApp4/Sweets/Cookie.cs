using System;
using System.Collections.Generic;

namespace ConsoleApp4.Sweets
{
    internal class Cookie : Sweets
    {
        public string Shape { get; private set; }

        public Cookie(string name, Dictionary<string, int> compound, int weight, string shape)
            : base(name, compound, weight)
        {
            if (string.IsNullOrWhiteSpace(shape))
                throw new ArgumentException("Форма печенья должна быть указана.");

            Shape = shape;
        }

        public override string GetDescription()
        {
            return $"{Name} — печенье в форме {Shape}. Вес: {Weight} г, сахар: {GetSugarContent()} г.";
        }
    }
}
