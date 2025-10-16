using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ConsoleApp3.Models
{
    [XmlRoot("Text")]
    public class Text
    {
        [XmlElement("Sentence")]
        public List<Sentence> Sentences { get; set; } = new List<Sentence>();

        public void AddSentence(Sentence sentence) => Sentences.Add(sentence);

        public override string ToString() =>
            string.Join(" ", Sentences.Select(sentence => sentence.ToString()));

        public Text Clone()
        {
            var copy = new Text();
            foreach (var sentence in this.Sentences)
            {
                var newSentence = new Sentence();
                foreach (var token in sentence.Parts)
                {
                    if (token is Word w)
                        newSentence.AddPart(new Word(w.Value));
                    else if (token is Punctuation p)
                        newSentence.AddPart(new Punctuation(p.Symbol));
                }
                copy.AddSentence(newSentence);
            }
            return copy;
        }

        public List<Sentence> SortByWordCount()
        {
            var sortedSentences = new List<Sentence>(Sentences);
            sortedSentences.Sort((first, second) =>
                first.WordCount.CompareTo(second.WordCount));
            return sortedSentences;
        }

        public List<Sentence> SortByLength()
        {
            var sortedSentences = new List<Sentence>(Sentences);
            sortedSentences.Sort((first, second) =>
                first.ToString().Length.CompareTo(second.ToString().Length));
            return sortedSentences;
        }

        public IEnumerable<string> FindWordsInQuestions(int length) =>
            Sentences.Where(sentence => sentence.IsQuestion)
                     .SelectMany(sentence => sentence.Words)
                     .Where(word => word.Value.Length == length)
                     .Select(word => word.Value.ToLower())
                     .Distinct();

        public void RemoveWords(int length)
        {
            const string consonants = "бвгджзйклмнпрстфхцчшщёbcdfghjklmnpqrstvwxyz";
            foreach (var sentence in Sentences)
            {
                sentence.Parts = sentence.Parts.Where(token =>
                    !(token is Word word &&
                      word.Value.Length == length &&
                      consonants.Contains(char.ToLower(word.Value[0]))))
                    .ToList();
            }
        }

        public void ReplaceWords(int sentenceIndex, int wordLength, string replacement)
        {
            if (sentenceIndex < 0 || sentenceIndex >= Sentences.Count)
                return;

            var sentence = Sentences[sentenceIndex];
            for (int i = 0; i < sentence.Parts.Count; i++)
            {
                if (sentence.Parts[i] is Word word && word.Value.Length == wordLength)
                    sentence.Parts[i] = new Word(replacement);
            }
        }

        public void ReplaceWordsInteractive()
        {
            for (int i = 0; i < Sentences.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Sentences[i]}");
            }

            int sentenceIndex;
            while (true)
            {
                Console.Write("Введите номер предложения для замены слов: ");
                if (int.TryParse(Console.ReadLine(), out sentenceIndex) &&
                    sentenceIndex > 0 && sentenceIndex <= Sentences.Count)
                {
                    sentenceIndex--;
                    break;
                }
                Console.WriteLine("Неверный номер. Попробуйте снова.");
            }

            var selectedSentence = Sentences[sentenceIndex];
            var uniqueWords = selectedSentence.Words.Select(word => word.Value).Distinct().ToList();

            if (uniqueWords.Count == 0)
            {
                Console.WriteLine("В выбранном предложении нет слов для замены.");
                return;
            }
            
            Console.WriteLine("Введите длинну слов которые надо заменить на подстроку:");
            if (int.TryParse(Console.ReadLine(), out int wordCount) &&
                wordCount > 0 )
            {
                List<string> oldWords = new List<string>();

                foreach (string oldWord in uniqueWords)
                {
                    if(oldWord.Length == wordCount)
                    {
                        oldWords.Add(oldWord);
                    }
                }
                Console.WriteLine($"Введите подстроку для замены слов: {string.Join(", ", oldWords)}");
                string newWord = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newWord))
                {
                    Console.WriteLine("Пустая подстрока недопустима. Отмена операции.");
                    return;
                }
                for (int k = 0; k < selectedSentence.Parts.Count; k++)
                {
                    if (selectedSentence.Parts[k] is Word word && word.Value.Length == wordCount)
                        selectedSentence.Parts[k] = new Word(newWord);
                }
                Console.WriteLine($"Слова {string.Join(", ", oldWords)} заменены на '{newWord}'.");
                Console.WriteLine(selectedSentence);
            }
        }

        public void RemoveStopWords(HashSet<string> stopWords)
        {
            foreach (var sentence in Sentences)
            {
                sentence.Parts = sentence.Parts.Where(token =>
                    !(token is Word word && stopWords.Contains(word.Value.ToLower())))
                    .ToList();
            }
        }

        public void ExportToXml(string fileName)
        {
            var serializer = new XmlSerializer(typeof(Text));
            using var fileStream = new FileStream(fileName, FileMode.Create);
            serializer.Serialize(fileStream, this);
        }

        public static Text ImportFromXml(string fileName)
        {
            var serializer = new XmlSerializer(typeof(Text));
            using var fileStream = new FileStream(fileName, FileMode.Open);
            return (Text)serializer.Deserialize(fileStream);
        }

        public List<string> CreateConcordanceDictionary()
        {
            var concordance = new SortedDictionary<string, (int Count, HashSet<int> Lines)>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < Sentences.Count; i++)
            {
                int lineNumber = i + 1;
                foreach (var word in Sentences[i].Words)
                {
                    string key = word.Value.ToLower();
                    if (!concordance.ContainsKey(key))
                    {
                        concordance[key] = (0, new HashSet<int>());
                    }
                    var entry = concordance[key];
                    entry.Count++;
                    entry.Lines.Add(lineNumber);
                    concordance[key] = entry;
                }
            }
            var result = new List<string>();
            int maxWordLength = concordance.Keys.Max(w => w.Length);
            int totalWidth = maxWordLength + 20;
            foreach (var keyValuePair in concordance)
            {
                string word = keyValuePair.Key;
                int count = keyValuePair.Value.Count;
                string lines = string.Join(" ", keyValuePair.Value.Lines.OrderBy(n => n));
                int dotCount = Math.Max(1, totalWidth - word.Length - count.ToString().Length - 2);
                string dots = new string('.', dotCount);

                result.Add($"{word}{dots}{count}: {lines}");
            }

            return result;
        }
    }
}
