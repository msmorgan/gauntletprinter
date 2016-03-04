using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GauntletPrinter
{
    public static class CardTextShortener
    {
        private static readonly IList<Rule> Rules = new List<Rule>();

        static CardTextShortener()
        {
            foreach (var line in File.ReadAllLines("rules.txt"))
            {
                var lineParts = line.Split(new[] { "///" }, StringSplitOptions.None);
                if (lineParts.Length == 2)
                {
                    Rules.Add(new Rule(lineParts[0], lineParts[1]));
                }
            }
        }

        private static string ReplaceSymbols(string str, bool isGrayscale)
        {
            return ReplaceSymbols(str, isGrayscale ? GrayscaleSymbols : ColoredSymbols);
        }

        private static string ReplaceSymbols(string str, IDictionary<string, string> symbols)
        {
            foreach (var entry in symbols)
            {
                str = str.Replace(entry.Key, ImageTag(entry.Value));
            }
            return str;
        }


        private static string ImageTag(string src, int width = 10, int height = 10)
        {
            return $"<img src=\"{src}\" width=\"{width}\" height=\"{height}\" />";
        }


        public static void ProcessCard(Card card, bool isGrayscale, bool omitTypeLineForBasics)
        {
            if (card.Processed) return;
            card.Processed = true;

            card.Type = card.Type.Replace("—", "-");

            if (card.ManaCost != null)
                card.ManaCost = ReplaceSymbols(card.ManaCost, isGrayscale);

            if (card.Text == null) return;

            card.Text = card.Text.Replace(card.Name, "~");

            // The card can refer to itself with partial name (eg. Purphoros instead of Purphoros, God of the Forge)
            if (card.Name.Contains(','))
            {
                var shortName = card.Name.Substring(0, card.Name.IndexOf(','));
                card.Text = card.Text.Replace(shortName, "~");
            }

            foreach (var rule in Rules)
            {
                card.Text = rule.Regex.Replace(card.Text, rule.Replacement);
            }

            card.Type = card.Type.Replace('—', '-');

            card.TextLength = (int)Math.Ceiling(card.Text.Length / 40.0);

            if (omitTypeLineForBasics && card.Types.Contains("Basic"))
            {
                card.TextLength = 0;
            }

            card.Text = ReplaceSymbols(card.Text, isGrayscale);
        }

        private class Rule
        {
            public Rule(string find, string replace)
            {
                Regex = new Regex(find, RegexOptions.ECMAScript | RegexOptions.IgnoreCase | RegexOptions.Compiled);
                Replacement = replace;
            }

            public Regex Regex { get; set; }
            public string Replacement { get; set; }
        }

        private static readonly IDictionary<string, string> ColoredSymbols = new Dictionary<string, string>
        {
            {"{0}", "symbols/mana-0.svg"},
            {"{1}", "symbols/mana-1.svg"},
            {"{2}", "symbols/mana-2.svg"},
            {"{3}", "symbols/mana-3.svg"},
            {"{4}", "symbols/mana-4.svg"},
            {"{5}", "symbols/mana-5.svg"},
            {"{6}", "symbols/mana-6.svg"},
            {"{7}", "symbols/mana-7.svg"},
            {"{8}", "symbols/mana-8.svg"},
            {"{9}", "symbols/mana-9.svg"},
            {"{10}", "symbols/mana-10.svg"},
            {"{11}", "symbols/mana-11.svg"},
            {"{12}", "symbols/mana-12.svg"},
            {"{13}", "symbols/mana-13.svg"},
            {"{14}", "symbols/mana-14.svg"},
            {"{15}", "symbols/mana-15.svg"},
            {"{16}", "symbols/mana-16.svg"},
            {"{17}", "symbols/mana-17.svg"},
            {"{18}", "symbols/mana-18.svg"},
            {"{19}", "symbols/mana-19.svg"},
            {"{20}", "symbols/mana-20.svg"},
            {"{X}", "symbols/mana-x.svg"},
            {"{Y}", "symbols/mana-y.svg"},
            {"{Z}", "symbols/mana-z.svg"},
            {"{W}", "symbols/mana-w.svg"},
            {"{U}", "symbols/mana-u.svg"},
            {"{B}", "symbols/mana-b.svg"},
            {"{R}", "symbols/mana-r.svg"},
            {"{G}", "symbols/mana-g.svg"},
            {"{2/W}", "symbols/mana-2w.svg"},
            {"{2/U}", "symbols/mana-2u.svg"},
            {"{2/B}", "symbols/mana-2b.svg"},
            {"{2/R}", "symbols/mana-2r.svg"},
            {"{2/G}", "symbols/mana-2g.svg"},
            {"{W/P}", "symbols/mana-pw.svg"},
            {"{U/P}", "symbols/mana-pu.svg"},
            {"{B/P}", "symbols/mana-pb.svg"},
            {"{R/P}", "symbols/mana-pr.svg"},
            {"{G/P}", "symbols/mana-pg.svg"},
            {"{W/U}", "symbols/mana-wu.svg"},
            {"{W/B}", "symbols/mana-wb.svg"},
            {"{U/B}", "symbols/mana-ub.svg"},
            {"{U/R}", "symbols/mana-ur.svg"},
            {"{B/R}", "symbols/mana-br.svg"},
            {"{B/G}", "symbols/mana-bg.svg"},
            {"{R/W}", "symbols/mana-rw.svg"},
            {"{R/G}", "symbols/mana-rg.svg"},
            {"{G/W}", "symbols/mana-gw.svg"},
            {"{G/U}", "symbols/mana-gu.svg"},
            {"{T}", "symbols/tap.svg"},
            {"{Q}", "symbols/untap.svg"},
            {"{+1/+1}", "symbols/counter-plus.svg"},
            {"{-1/-1}", "symbols/counter-minus.svg"},
            {"{charge}", "symbols/counter-charge.svg"},
            {"{any}", "symbols/mana-any.svg"},
        };

        private static readonly IDictionary<string, string> GrayscaleSymbols = new Dictionary<string, string>
        {
            {"{0}", "symbols/mana-0.svg"},
            {"{1}", "symbols/mana-1.svg"},
            {"{2}", "symbols/mana-2.svg"},
            {"{3}", "symbols/mana-3.svg"},
            {"{4}", "symbols/mana-4.svg"},
            {"{5}", "symbols/mana-5.svg"},
            {"{6}", "symbols/mana-6.svg"},
            {"{7}", "symbols/mana-7.svg"},
            {"{8}", "symbols/mana-8.svg"},
            {"{9}", "symbols/mana-9.svg"},
            {"{10}", "symbols/mana-10.svg"},
            {"{11}", "symbols/mana-11.svg"},
            {"{12}", "symbols/mana-12.svg"},
            {"{13}", "symbols/mana-13.svg"},
            {"{14}", "symbols/mana-14.svg"},
            {"{15}", "symbols/mana-15.svg"},
            {"{16}", "symbols/mana-16.svg"},
            {"{17}", "symbols/mana-17.svg"},
            {"{18}", "symbols/mana-18.svg"},
            {"{19}", "symbols/mana-19.svg"},
            {"{20}", "symbols/mana-20.svg"},
            {"{X}", "symbols/mana-x.svg"},
            {"{Y}", "symbols/mana-y.svg"},
            {"{Z}", "symbols/mana-z.svg"},
            {"{W}", "symbols/mana-w-gray.svg"},
            {"{U}", "symbols/mana-u-gray.svg"},
            {"{B}", "symbols/mana-b-gray.svg"},
            {"{R}", "symbols/mana-r-gray.svg"},
            {"{G}", "symbols/mana-g-gray.svg"},
            {"{2/W}", "symbols/mana-2w-gray.svg"},
            {"{2/U}", "symbols/mana-2u-gray.svg"},
            {"{2/B}", "symbols/mana-2b-gray.svg"},
            {"{2/R}", "symbols/mana-2r-gray.svg"},
            {"{2/G}", "symbols/mana-2g-gray.svg"},
            {"{W/P}", "symbols/mana-pw-gray.svg"},
            {"{U/P}", "symbols/mana-pu-gray.svg"},
            {"{B/P}", "symbols/mana-pb-gray.svg"},
            {"{R/P}", "symbols/mana-pr-gray.svg"},
            {"{G/P}", "symbols/mana-pg-gray.svg"},
            {"{W/U}", "symbols/mana-wu-gray.svg"},
            {"{W/B}", "symbols/mana-wb-gray.svg"},
            {"{U/B}", "symbols/mana-ub-gray.svg"},
            {"{U/R}", "symbols/mana-ur-gray.svg"},
            {"{B/R}", "symbols/mana-br-gray.svg"},
            {"{B/G}", "symbols/mana-bg-gray.svg"},
            {"{R/W}", "symbols/mana-rw-gray.svg"},
            {"{R/G}", "symbols/mana-rg-gray.svg"},
            {"{G/W}", "symbols/mana-gw-gray.svg"},
            {"{G/U}", "symbols/mana-gu-gray.svg"},
            {"{T}", "symbols/tap.svg"},
            {"{Q}", "symbols/untap.svg"},
            {"{+1/+1}", "symbols/counter-plus.svg"},
            {"{-1/-1}", "symbols/counter-minus.svg"},
            {"{charge}", "symbols/counter-charge-gray.svg"},
            {"{any}", "symbols/mana-any.svg"},
        };
    }
}
