using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApp4
{
    [Serializable]
    public class SweetDTO
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public CompoundItem[] Compound { get; set; }
        public string Extra { get; set; }

        public int Count { get; set; }

    }

    [Serializable]
    public class GiftDTO
    {
        public int Weight { get; set; }
        public List<SweetDTO> Content { get; set; }
    }


    [Serializable]
    public class CompoundItem
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }

    public static class DtoConverter
    {
        public static SweetDTO ToDTO(Sweets.Sweets sweet)
        {
            var dto = new SweetDTO
            {
                Type = sweet.GetType().Name,
                Name = sweet.Name,
                Weight = sweet.Weight,
                Compound = sweet.Compound.Select(kv => new CompoundItem
                {
                    Key = kv.Key,
                    Value = kv.Value
                }).ToArray(),
                Extra = sweet switch
                {
                    Sweets.Chocolate chocolate => chocolate.CocoaPercent.ToString(),
                    Sweets.Caramel caramel => caramel.FillingType,
                    Sweets.JellyCandy jellycandy => jellycandy.Flavor,
                    Sweets.Cookie cookie => cookie.Shape,
                    _ => ""
                },
            };
            return dto;
        }

        public static Sweets.Sweets FromDTO(SweetDTO dto)
        {
            var compound = dto.Compound.ToDictionary(c => c.Key, c => c.Value);
            return dto.Type switch
            {
                "Chocolate" => new Sweets.Chocolate(dto.Name, compound, dto.Weight, int.Parse(dto.Extra)),
                "Caramel" => new Sweets.Caramel(dto.Name, compound, dto.Weight, dto.Extra),
                "JellyCandy" => new Sweets.JellyCandy(dto.Name, compound, dto.Weight, dto.Extra),
                "Cookie" => new Sweets.Cookie(dto.Name, compound, dto.Weight, dto.Extra),
                _ => throw new Exception($"Неизвестный тип сладости: {dto.Type}")
            };
        }

        public static GiftDTO ToDTO(Gift gift)
        {
            var dto = new GiftDTO
            {
                Weight = gift.Weight,
                Content = gift.Content.Select(kv =>
                {
                    var sweetDto = ToDTO(kv.Key); 
                    sweetDto.Count = kv.Value;   
                    return sweetDto;
                }).ToList()
            };
            return dto;
        }

        public static Gift FromDTO(GiftDTO dto)
        {
            var content = new Dictionary<Sweets.Sweets, int>();
            foreach (var sweetDTO in dto.Content)
            {
                var sweet = FromDTO(sweetDTO);
                content[sweet] = sweetDTO.Count;
            }
            return new Gift(dto.Weight, content);
        }

        public static List<Gift> LoadGifts()
        {
            if (!File.Exists("gifts.xml"))
                return new List<Gift>();

            var serializer = new XmlSerializer(typeof(List<GiftDTO>));

            using (var stream = new FileStream("gifts.xml", FileMode.Open))
            {
                var dtoList = (List<GiftDTO>)serializer.Deserialize(stream);
                return dtoList.Select(dto => DtoConverter.FromDTO(dto)).ToList();
            }
        }
    }
}

