using System.Collections.Generic;

namespace proxygen.Models
{
    public class CardsModel
    {
        public int? CardsToPrint { get; set; }
        public string CardName { get; set; }
    }

    public class CardModel
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
        public List<CardModel> data { get; set; }
    }


    public class ProxyViewModel
    {
        public List<CardProxies> Cards { get; set; }
    }
    public class CardProxies
    {
        public CardModel Card { get; set; }
        public int NumToPrint { get; set; }
    }
}
