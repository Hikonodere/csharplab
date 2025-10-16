using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ConsoleApp3.Models
{
    public class Sentence
    {
        [XmlElement("Word", typeof(Word))]
        [XmlElement("Punctuation", typeof(Punctuation))]
        public List<Token> Parts { get; set; } = new List<Token>();

        [XmlIgnore]
        public IEnumerable<Word> Words => Parts.OfType<Word>();

        [XmlIgnore]
        public IEnumerable<Punctuation> Punctuations => Parts.OfType<Punctuation>();

        [XmlIgnore]
        public int WordCount => Words.Count();

        [XmlIgnore]
        public bool IsQuestion => Punctuations.Any(punctuation => punctuation.Symbol == "?");

        public void AddPart(Token token) => Parts.Add(token);

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < Parts.Count; i++)
            {
                if (Parts[i] is Word word)
                {
                    builder.Append(word.Value);
                    if (i + 1 < Parts.Count && Parts[i + 1] is not Punctuation)
                        builder.Append(" ");
                }
                else if (Parts[i] is Punctuation punctuation)
                {
                    builder.Append(punctuation.Symbol);
                    if (i + 1 < Parts.Count && Parts[i + 1] is Word)
                        builder.Append(" ");
                }
            }
            return builder.ToString().Trim();
        }
    }
}
