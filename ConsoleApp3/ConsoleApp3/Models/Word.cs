using System.Xml.Serialization;

namespace ConsoleApp3.Models
{
    public class Word : Token
    {
        public Word() { }
        public Word(string value) { Value = value; }

        [XmlText]
        public string Value { get; set; }

        public override string ToString() => Value;
    }
}
