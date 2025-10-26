using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ConsoleApp4.Sweets;
using System.Xml.Serialization;


namespace ConsoleApp4
{
    internal class Program
    {

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
            string xmlFileName = "sweets.xml";


            if (!File.Exists(xmlFileName))
            {
                throw new Exception($"Файл {xmlFileName} не найден.");
            }

            var serializer = new XmlSerializer(typeof(List<SweetDTO>));

            using (var stream = new FileStream(xmlFileName, FileMode.Open))
            {
                var sweetDTOs = (List<SweetDTO>)serializer.Deserialize(stream);

                foreach (var sweetDTO in sweetDTOs)
                {
                    try
                    {
                        var sweet = DtoConverter.FromDTO(sweetDTO);
                        sweets.Add(sweet);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при создании сладости '{sweetDTO.Name}': {ex.Message}");
                        continue;
                    }
                }
            }
            Console.WriteLine($"Успешно загружено {sweets.Count} сладостей из XML");
            return sweets;
        }

        private static void RunMenu()
        {
            Console.WriteLine("====МЕНЮ====\n" +
                "1.Создать подарок\n" +
                "2.Удалить подарок\n" +
                "3.Показать список сладостей\n" +
                "4.Узнать данные по конкретному подарку\n" +
                "5.Показать список подарков\n" +
                "6.Сохранить подарки в XML\n" +
                "7.Подгрузить ранее созданные подарки\n" +
                "0.Выход из программы");
            
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.WriteLine("Введите вес для нового подарка");
                    string input = Console.ReadLine();
                    if (!int.TryParse(input, out int weight) || weight < 1)
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

                case "6":
                    var dtoList = gifts.Select(g => DtoConverter.ToDTO(g)).ToList();
                    var serializer = new XmlSerializer(typeof(List<GiftDTO>));
                    using (var stream = new FileStream("gifts.xml", FileMode.Create))
                    {
                        serializer.Serialize(stream, dtoList);
                    }
                    break;
                case "7":
                      gifts.AddRange(DtoConverter.LoadGifts()); 
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
