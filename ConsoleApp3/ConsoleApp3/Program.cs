using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp3
{
    class Program
    {
        private static string inputFileName = "";

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Выберите язык текста:");
            Console.WriteLine("1. Русский");
            Console.WriteLine("2. Английский");

            string stopWordsFilePath = "";
            while (true)
            {
                string languageChoice = Console.ReadLine();
                switch (languageChoice)
                {
                    case "1":
                        stopWordsFilePath = "stopwords_ru.txt";
                        break;
                    case "2":
                        stopWordsFilePath = "stopwords_en.txt";
                        break;
                    default:
                        Console.WriteLine("Неверный выбор, попробуйте снова:");
                        continue;
                }
                break;
            }

            string inputText = "";
            while (true)
            {
                Console.WriteLine("Введите номер файла (например, 1 для 1.input):");
                string fileNumber = Console.ReadLine();
                inputFileName = $"{fileNumber}.input.txt";

                if (!File.Exists(inputFileName))
                {
                    Console.WriteLine($"Файл {inputFileName} не найден, попробуйте снова.");
                    continue;
                }

                inputText = File.ReadAllText(inputFileName, Encoding.UTF8);
                Console.WriteLine((int)inputText[0]);
                break;
            }

            var stopWordLines = File.ReadAllLines(stopWordsFilePath, Encoding.UTF8);
            var stopWords = new HashSet<string>(
                stopWordLines.Select(line => line.Trim().ToLower()).Where(line => line != ""),
                StringComparer.OrdinalIgnoreCase);

            var parsedText = TextParser.Parse(inputText);

            while (true)
            {
                Console.WriteLine("\n=== Меню действий ===");
                Console.WriteLine("1. Показать текст");
                Console.WriteLine("2. Сортировка по количеству слов");
                Console.WriteLine("3. Сортировка по длине предложения");
                Console.WriteLine("4. Найти слова заданной длины в вопросительных предложениях");
                Console.WriteLine("5. Удалить слова заданной длины, начинающиеся с согласной");
                Console.WriteLine("6. Заменить слова");
                Console.WriteLine("7. Удалить стоп-слова");
                Console.WriteLine("8. Экспорт в XML");
                Console.WriteLine("9. Создать словарь согласования");
                Console.WriteLine("0. Выход");

                Console.Write("Выберите действие: ");
                string userChoice = Console.ReadLine();

                switch (userChoice)
                {
                    case "1":
                        Console.WriteLine("\nТекст:");
                        Console.WriteLine(parsedText);
                        SaveToFile("ShowText", parsedText.ToString());
                        break;
                    case "2":
                        Console.WriteLine("\nСортировка по количеству слов:");
                        var sortedByWordCount = parsedText.SortByWordCount();
                        foreach (var sentence in sortedByWordCount)
                            Console.WriteLine(sentence);
                        SaveToFile("SortByWordCount", string.Join("\n", sortedByWordCount));
                        break;
                    case "3":
                        Console.WriteLine("\nСортировка по длине:");
                        var sortedByLength = parsedText.SortByLength();
                        foreach (var sentence in sortedByLength)
                            Console.WriteLine(sentence);
                        SaveToFile("SortByLength", string.Join("\n", sortedByLength));
                        break;
                    case "4":
                        Console.Write("Введите длину слова: ");
                        if (int.TryParse(Console.ReadLine(), out int targetLength))
                        {
                            var foundWords = parsedText.FindWordsInQuestions(targetLength);
                            Console.WriteLine("Найденные слова:");
                            foreach (var word in foundWords)
                                Console.WriteLine(word);
                            SaveToFile("FindWordsInQuestions", string.Join("\n", foundWords));
                        }
                        break;
                    case "5":
                        Console.Write("Введите длину слов для удаления: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteLength))
                        {
                            var copy = parsedText.Clone();
                            copy.RemoveWords(deleteLength);
                            Console.WriteLine("Слова удалены (работа с копией).");
                            SaveToFile("RemoveWords", copy.ToString());
                        }
                        break;

                    case "6":
                        var copy2 = parsedText.Clone();
                        copy2.ReplaceWordsInteractive();
                        SaveToFile("ReplaceWords", copy2.ToString());
                        break;

                    case "7":
                        var copy3 = parsedText.Clone();
                        copy3.RemoveStopWords(stopWords);
                        Console.WriteLine("Стоп-слова удалены (работа с копией).");
                        SaveToFile("RemoveStopWords", copy3.ToString());
                        break;
                    case "8":
                        parsedText.ExportToXml("text.xml");
                        Console.WriteLine("Текст экспортирован в text.xml");
                        SaveToFile("ExportToXml", "XML экспорт выполнен. Файл: text.xml");
                        break;
                    case "9":
                        var concordanceDictionary = parsedText.CreateConcordanceDictionary();
                        Console.WriteLine($"{string.Join("\n", concordanceDictionary)}");
                        SaveToFile("CreateConcordanceDictionary", string.Join("\n", concordanceDictionary));
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор, попробуйте снова.");
                        break;
                }
            }
        }

        private static void SaveToFile(string methodName, string content)
        {
            string baseFileName = Path.GetFileNameWithoutExtension(inputFileName).Split('.')[0];
            string outputFileName = $"{baseFileName}_{methodName}.txt";

            try
            {
                File.WriteAllText(outputFileName, content, Encoding.UTF8);
                Console.WriteLine($"Результат сохранен в файл: {outputFileName}");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Ошибка при сохранении файла: {exception.Message}");
            }
        }
    }
}
