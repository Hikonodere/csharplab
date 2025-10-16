using System.Text.RegularExpressions;
using ConsoleApp3.Models;

namespace ConsoleApp3
{
    public static class TextParser
    {
        public static Text Parse(string inputText)
        {
            var parsedText = new Text();
            string[] sentenceTexts = Regex.Split(inputText.Trim(), @"(?<=[.!?])\s+");

            foreach (string sentenceText in sentenceTexts)
            {
                if (string.IsNullOrWhiteSpace(sentenceText))
                    continue;

                var sentence = new Sentence();
                var tokenMatches = Regex.Matches(sentenceText, @"\w+|[^\w\s]");

                foreach (Match tokenMatch in tokenMatches)
                {
                    if (Regex.IsMatch(tokenMatch.Value, @"\w+"))
                        sentence.AddPart(new Word(tokenMatch.Value));
                    else
                        sentence.AddPart(new Punctuation(tokenMatch.Value));
                }
                parsedText.AddSentence(sentence);
            }
            return parsedText;
        }
    }
}
