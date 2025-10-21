using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ConsoleApp4.Sweets;

namespace ConsoleApp4
{
    internal class Program
    {
        private static string inputFileName = "Sweets.txt";
        private static List<Sweets.Sweets> sweets = new List<Sweets.Sweets>();
        private static List<Gift> gifts = new List<Gift>();
        static void Main()
        {   
            
            sweets = GetSweets();
            while (true)
            {
                try
                {
                    RunMenu();
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"{ex.Message}");
                    continue;
                }
            }
        }



        static List<Sweets.Sweets> GetSweets()
        {
            List<Sweets.Sweets> sweets = new List<Sweets.Sweets>();
            string[] lines;
            try
            {
                lines = File.ReadAllLines(inputFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла {inputFileName}: {ex.Message}");
                return sweets;
            }

            string currentCategory = null;
            string name = null;
            int weight = 0;
            Dictionary<string, int> compound = new();

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("=") || line.StartsWith("-"))
                    continue;

                string key = line.Contains(':') ? line.Split(':')[0].Trim() : "";
                string value = line.Contains(':') ? line.Split(':', 2)[1].Trim() : "";

                switch (key.ToLower())
                {
                    case "категория":
                        currentCategory = value;
                        break;

                    case "название":
                        name = value;
                        compound = new Dictionary<string, int>();
                        break;

                    case "вес":
                        string weightStr = Regex.Match(value, @"\d+").Value;
                        weight = int.Parse(weightStr);
                        break;

                    case "состав":
                        string[] parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries);

                        foreach (var part in parts)
                        {
                            var p = part.Trim().Split(' ');
                            string compKey = p[0];
                            int compValue = Regex.Match(part, @"\d+").Success
                                ? int.Parse(Regex.Match(part, @"\d+").Value)
                                : 0;
                            compound[compKey.ToLower()] = compValue;
                        }

                        if (currentCategory != null && name != null && weight > 0)
                        {
                            try
                            {
                                sweets.Add(CreateSweet(currentCategory, name, compound, weight));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка при чтении файла {inputFileName}: {ex.Message}");
                                Console.WriteLine($"Обыект {currentCategory} {name} не будет добавлен так-как он не валидный");
                            }
                            name = null;
                            weight = 0;
                            compound = new();
                            
                            
                        }
                        break;

                    default:
                        break;
                }
            }


            return sweets;
        }

        private static Sweets.Sweets CreateSweet(string category, string name, Dictionary<string, int> compound, int weight)
        {
            return category.ToLower() switch
            {
                "шоколад" => new Chocolate(name, compound, weight, 50),
                "карамель" => new Caramel(name, compound, weight, "Без начинки"),
                "желейные конфеты" => new JellyCandy(name, compound, weight, "Фруктовые"),
                "печенье" => new Cookie(name, compound, weight, "Классическое"),
                "ирис" => new Caramel(name, compound, weight, "Сливочный"),
                _ => throw new Exception($"Неизвестная категория: {category}")
            };
        }
        private static void RunMenu()
        {
            Console.WriteLine("====МЕНЮ====\n" +
                "1.Создать подарок\n" +
                "2.Удалить подарок\n" +
                "3.Показать список сладостей\n" +
                "4.Узнать данные по конкретному подарку\n" +
                "5.Показать список подарков\n" +
                "0.Выход из программы");
            
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.WriteLine("Введите вес для нового подарка");
                    string input = Console.ReadLine();
                    if (!(int.TryParse(input, out int weight) && weight >= 1))
                    {
                        throw new Exception("Некоретный ввод");
                    }
                    gifts.Add(new Gift(weight, sweets));
                    break;
                case "2":
                    for (int i = 0; i < gifts.Count; i++)
                    {
                        Console.WriteLine("Список подарков:\n");
                        Console.WriteLine($"{i} {gifts[i]}");
                    }
                    Console.WriteLine("Введите индекс подарка для удаления");
                    input = Console.ReadLine();
                    if (!(int.TryParse(input, out int index) && index >= 0 && index < gifts.Count))
                    {
                        throw new Exception("Некоретный ввод");
                    }
                    gifts.RemoveAt(index);
                    break;
                case "3":
                    Console.WriteLine("Список сладостей:\n");
                    foreach (var sweet in sweets)
                    {
                        Console.WriteLine($" {sweet}");
                    }

                    break;
                case "4":
                    if (gifts.Count == 0)
                    {
                        Console.WriteLine("Нет созданных подарков.");
                        break;
                    }

                    for (int i = 0; i < gifts.Count; i++)
                    {
                        Console.WriteLine($"{i}. {gifts[i]}");
                    }

                    Console.WriteLine("Введите индекс подарка:");
                    input = Console.ReadLine();
                    if (!(int.TryParse(input, out int ind) || ind < 0 || ind >= gifts.Count))
                    {
                        throw new Exception("Некорректный ввод");
                    }

                    var selectedGift = gifts[ind];

                    Console.WriteLine("\nВыберите действие:");
                    Console.WriteLine("1 - Сортировка конфет по весу");
                    Console.WriteLine("2 - Поиск по содержанию сахара");

                    string action = Console.ReadLine();
                    switch (action)
                    {
                        case "1":
                            Console.WriteLine("\n--- Сортировка конфет по весу ---");
                            var sortedSweets = selectedGift.GetSweetsSortedByWeight();
                            Console.WriteLine("Сладости в подарке отсортированные по весу:");
                            foreach (var sweet in sortedSweets)
                            {
                                int count = selectedGift.GetSweetCount(sweet);
                                Console.WriteLine($"  {sweet.GetDescription()} (в подарке: {count} шт.)");
                            }
                            break;

                        case "2":
                            Console.WriteLine("\n--- Поиск по содержанию сахара ---");
                            Console.WriteLine("Введите минимальное содержание сахара (г):");
                            if (!int.TryParse(Console.ReadLine(), out int minSugar) || minSugar < 0)
                            {
                                throw new Exception("Некорректный ввод минимального значения");
                            }

                            Console.WriteLine("Введите максимальное содержание сахара (г):");
                            if (!int.TryParse(Console.ReadLine(), out int maxSugar) || maxSugar < minSugar)
                            {
                                throw new Exception("Некорректный ввод максимального значения");
                            }

                            var sweetsInRange = selectedGift.FindSweetsBySugarRange(minSugar, maxSugar);
                            if (sweetsInRange.Count == 0)
                            {
                                Console.WriteLine($"Сладостей с содержанием сахара от {minSugar}г до {maxSugar}г не найдено.");
                            }
                            else
                            {
                                Console.WriteLine($"Найдены сладости с содержанием сахара от {minSugar}г до {maxSugar}г:");
                                foreach (var sweet in sweetsInRange)
                                {
                                    int count = selectedGift.GetSweetCount(sweet);
                                    int sugar = sweet.GetSugarContent();
                                    Console.WriteLine($"  {sweet.GetDescription()} (в подарке: {count} шт., сахар: {sugar}г)");
                                }
                            }
                            break;

                        default:
                            Console.WriteLine("Неверный выбор.");
                            break;
                    }
                    break;
                case "5":
                    foreach (var gift in gifts)
                    {
                        Console.WriteLine("Список подарков:\n");
                        Console.WriteLine($" {gift}");
                    }
                    break;  
                case "0":
                    return;
                default:
                    Console.WriteLine("Неверный выбор, попробуйте снова.");
                    break;
            }
        }
    }
}
