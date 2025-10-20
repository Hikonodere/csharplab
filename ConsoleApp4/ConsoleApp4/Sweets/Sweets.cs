using System.Data;
namespace ConsoleApp4.Sweets
{
    public abstract class Sweets
    {
        public String Name { get; private set; }

        public void nameSet(string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentException("Некоректное имя.");
            }
            Name = newName;
        }

        public Dictionary<string, int> Compound { get; private set; }
        public int Weight { get; private set; }
        public void weightSet(int weight)
        {
            if (weight <= 0)
            {
                throw new ArgumentException("Некоректный вес.");
            }
            Weight = weight;
        }

        public Sweets(string name, Dictionary<string, int> compound, int weight)
        {
            nameSet(name);
            Compound = compound;
            weightSet(weight);
        }

        public virtual int GetSugarContent()
        {
            return Compound.ContainsKey("sugar") ? Compound["sugar"] : 0;
        }
        public override string ToString() => $"{GetType().Name}: {Name}";

        public abstract string GetDescription();
    }
}
