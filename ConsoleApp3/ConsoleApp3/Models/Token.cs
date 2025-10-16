using System.Xml.Serialization;

namespace ConsoleApp3.Models
{
    [XmlInclude(typeof(Word))]
    [XmlInclude(typeof(Punctuation))]
    public abstract class Token
    {
        public override string ToString() => base.ToString();
    }
}
