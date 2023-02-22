using System.Collections.Generic;
using System.Linq;

namespace proxygen.Models
{
    public class SanitizedCardsToPrint
    {
        public int? CardsToPrint { get; set; }
        public string CardName { get; set; }
    }

    public class ScryfallCardModel
    {
        public string name { get; set; }
        public string oracle_text { get; set; }
        public string power { get; set; }
        public string toughness { get; set; }
        public string type_line { get; set; }
        public string mana_cost { get; set; }
    }

    public class ScryfallResultsObject
    {
        public List<ScryfallCardModel> data { get; set; }
    }

    public class ProxyPageModel
    {
        public string Name { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public string TypeLine { get; set; }
        public string ManaCost { get; set; }
        public List<string> OracleText { get; set; }
        public ProxyPageModel() : base() { }
        public ProxyPageModel(ScryfallCardModel baseModel) 
        { 
            Name = baseModel.name; 
            Power = baseModel.power;
            Toughness = baseModel.toughness;
            TypeLine = baseModel.type_line;
            ManaCost = baseModel.mana_cost.Replace("{", "").Replace("}", "");
            OracleText = baseModel.oracle_text.Split('\n').ToList();
        }
    }
}
