using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GauntletPrinter
{
    public class Deck
    {
        private readonly List<Card> _mainboard = new List<Card>();
        public IEnumerable<Card> Mainboard => _mainboard;

        private readonly List<Card> _sideboard = new List<Card>();
        public IEnumerable<Card> Sideboard => _sideboard;

        public Deck(IEnumerable<Card> mainCards, IEnumerable<Card> sideCards)
        {
            _mainboard.AddRange(mainCards);
            _sideboard.AddRange(sideCards);
        }

        public Deck(string mainList, string sideList)
            : this(ParseList(mainList), ParseList(sideList))
        {
        }

        private static IEnumerable<Card> ParseList(string list)
        {
            var reg = new Regex(@"^\s*(\d*)\s*(.*?)\s*$");
            var lines = list.Split(new [] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var match = reg.Match(line);
                int count;
                if (!int.TryParse(match.Groups[0].Value, out count)) count = 1;
                while (count-- > 0)
                {
                    yield return GetCard(match.Groups[1].Value);
                }
            }
        } 

        private static Card GetCard(string name)
        {
            var cip = CardInformationProvider.Instance;
            return cip.AllCards[name];
        }
    }
}
