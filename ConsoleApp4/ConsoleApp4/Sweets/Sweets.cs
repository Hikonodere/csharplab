using System.Data;
namespace ConsoleApp4.Sweets
{
    public abstract class Sweets : IComparable<Sweets>
    {
        public String Name { get; private set; }

        public void nameSet(string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentException("Некоректное имя.");
            }
            Name = newName;
        }

        public Dictionary<string, int> Compound { get; private set; }
        public int Weight { get; private set; }
        public void weightSet(int weight)
        {
            if (weight <= 0)
            {
                throw new ArgumentException("Некоректный вес.");
            }
            Weight = weight;
        }

        public Sweets(string name, Dictionary<string, int> compound, int weight)
        {
            nameSet(name);
            Compound = compound;
            weightSet(weight);
        }

        public virtual int GetSugarContent()
        {
            return Compound.ContainsKey("sugar") ? Compound["sugar"] : 0;
        }

        public static Sweets CreateSweetFromUserInput()
        {
            Console.WriteLine("Выберите тип сладости:");
            Console.WriteLine("1 - Шоколад");
            Console.WriteLine("2 - Карамель");
            Console.WriteLine("3 - Желейная конфета");
            Console.WriteLine("4 - Печенье");

            var choice = Console.ReadLine();

            Console.Write("Введите название: ");
            var name = Console.ReadLine();

            Console.Write("Введите вес (г): ");
            var weight = int.Parse(Console.ReadLine());

            Console.Write("Введите содержание сахара (г): ");
            var sugar = int.Parse(Console.ReadLine());
            if (sugar < 0)
            {
                throw new ArgumentException("Некоректное содержание сахара");
            }

            var compound = new Dictionary<string, int> { ["sugar"] = sugar };

            return choice switch
            {
                "1" => Chocolate.CreateChocolateFromInput(name, compound, weight),
                "2" => Caramel.CreateCaramelFromInput(name, compound, weight),
                "3" => JellyCandy.CreateJellyFromInput(name, compound, weight),
                "4" => Cookie.CreateCookieFromInput(name, compound, weight),
                _ => throw new ArgumentException("Неверный выбор")
            };

        }
        public override string ToString() => $"{GetType().Name}: {Name}";

        public int CompareTo(Sweets other)
        {
            if (other == null) return 1;
            return Weight.CompareTo(other.Weight);
        }


        public abstract string GetDescription();

    }
}
