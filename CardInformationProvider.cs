using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GauntletPrinter
{
    public class CardInformationProvider
    {
        private static CardInformationProvider _instance;
        public static CardInformationProvider Instance => _instance ?? (_instance = new CardInformationProvider());

        private Dictionary<string, Card> _allCards;
        public IDictionary<string, Card> AllCards => _allCards; 
          
        private CardInformationProvider()
        {
            LoadCardData();
        }

        private void LoadCardData()
        {
            var allCardsJson = File.ReadAllText("AllCards.json", Encoding.UTF8);
            _allCards = JsonConvert.DeserializeObject<Dictionary<string, Card>>(allCardsJson);
        }
    }
}
