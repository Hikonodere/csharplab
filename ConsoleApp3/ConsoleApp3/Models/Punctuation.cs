using System.Xml.Serialization;

namespace ConsoleApp3.Models
{
    public class Punctuation : Token
    {
        public Punctuation() { }
        public Punctuation(string symbol) { Symbol = symbol; }

        [XmlText]
        public string Symbol { get; set; }

        public override string ToString() => Symbol;
    }
}
