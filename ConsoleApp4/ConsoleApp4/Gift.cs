using ConsoleApp4.Sweets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ConsoleApp4
{
    public class Gift
    {   
        public int Weight { get; private set; }
        public Dictionary<Sweets.Sweets, int > Content { get; private set;}

        public Gift(int weight, List<Sweets.Sweets> sweets)
        {
            Weight = weight;
            Content = CreateContent(weight, sweets);
        }
        public Gift(int weight, Dictionary<Sweets.Sweets, int> content)
        {
            Weight = weight;
            Content = content;
        }

        private Dictionary<Sweets.Sweets, int> CreateContent(int giftWeight, List<Sweets.Sweets> sweets)
        {
            var content = new Dictionary<Sweets.Sweets, int>();
            int remainingWeight = giftWeight;
            var random = new Random();

            while (true)
            {
                Console.WriteLine("Сгенерировать подарок(y/n)?");
                string answer = Console.ReadLine().Trim().ToLower();
                switch (answer)
                {
                    case "y":
                        var shuffled = sweets.OrderBy(x => random.Next()).ToList();
                        while (remainingWeight >= GetSweetMinWeight(sweets))
                        {
                            foreach (var sweet in shuffled)
                            {
                                if (remainingWeight - sweet.Weight >= 0)
                                {
                                    if (content.ContainsKey(sweet))
                                        content[sweet] += 1;
                                    else
                                        content[sweet] = 1;

                                    remainingWeight -= sweet.Weight;
                                }
                            }
                        }
                        Console.WriteLine($"Подарок созданан"); 

                        break;

                    case "n":
                        while (true)
                        {
                            Console.WriteLine("\nСписок сладостей:");
                            PrintTableHeader();
                            for (int i = 0; i < sweets.Count; i++)
                            {
                                Console.WriteLine($"{i + 1,2}. {sweets[i].GetDescription()}");
                            }
                            PrintTableFooter();

                            Console.WriteLine($"Оставшийся вес подарка: {remainingWeight} г");
                            Console.WriteLine("Введите номер сладости для добавления (0 для завершения):");

                            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > sweets.Count)
                            {
                                Console.WriteLine("Неверный ввод, попробуйте снова.");
                                continue;
                            }

                            if (choice == 0)
                            {

                                int currentWeight = giftWeight - remainingWeight;
                                if (remainingWeight >= GetSweetMinWeight(sweets))
                                {
                                    Console.WriteLine($"Подарок слишком лёгкий ({currentWeight} г). Добавьте ещё сладостей.");
                                    continue;
                                }
                                break;
                            }

                            var selectedSweet = sweets[choice - 1];
                            int maxCount = remainingWeight / selectedSweet.Weight;

                            if (maxCount == 0)
                            {
                                Console.WriteLine("Эта сладость слишком тяжёлая для оставшегося веса. Выберите другую.");
                                continue;
                            }

                            Console.WriteLine($"Сколько штук {selectedSweet.Name} добавить? (не больше {maxCount}):");
                            if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0 || count > maxCount)
                            {
                                Console.WriteLine("Неверное количество, попробуйте снова.");
                                continue;
                            }

                            if (content.ContainsKey(selectedSweet))
                                content[selectedSweet] += count;
                            else
                                content[selectedSweet] = count;

                            remainingWeight -= selectedSweet.Weight * count;
                        }
                        Console.WriteLine($"Подарок созданан");
                        break;

                    default:
                        Console.WriteLine("Введено некоректное значение!!!");
                        continue;
                }
                break ;
            }
            return content;
        }
        public List<Sweets.Sweets> GetSweetsSortedByWeight()
        {
            return Content.Keys.OrderBy(sweet => sweet.Weight).ToList();
        }

        public List<Sweets.Sweets> FindSweetsBySugarRange(int minSugar, int maxSugar)
        {
            return Content.Keys
                .Where(sweet => sweet.GetSugarContent() >= minSugar && sweet.GetSugarContent() <= maxSugar)
                .ToList();
        }

        public int GetSweetCount(Sweets.Sweets sweet)
        {
            return Content.ContainsKey(sweet) ? Content[sweet] : 0;
        }

        public override string ToString()
        {
            int totalWeight = Content.Sum(item => item.Key.Weight * item.Value);
            string contentInfo = string.Join("\n", Content.Select(item => $"{item.Key.Name} x{item.Value}"));
            return $"Подарок {Weight}г (факт: {totalWeight}г): \n{contentInfo}";
        }

        public int GetSweetMinWeight(List<Sweets.Sweets> sweets)
        {
            return (sweets.Min(sweet => sweet.Weight));
        }

        public static void PrintTableHeader()
        {
            Console.WriteLine("    ┌───────────────────┬──────────────────────-──────────┬────────┬────────┐");
            Console.WriteLine("    │ Название          │ Тип                             │ Вес    │ Сахар  │");
            Console.WriteLine("    ├───────────────────┼─────────────────────────────────┼────────┼────────┤");
        }

        public static void PrintTableFooter()
        {
            Console.WriteLine("    └───────────────────┴──────────────────────-──────────┴────────┴────────┘");
        }

    }
}
