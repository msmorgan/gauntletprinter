using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GauntletPrinter
{
    public static class CardTextShortener
    {
        private static readonly Dictionary<string, MatchEvaluator> _advancedRules;
        
        // This is in string form so that it can be easily copied from MTG rules. Make sure to remove any " and ".
        private static readonly IList<string> Subtypes =
            "Contraption,Equipment,Fortification,Aura,Curse,Shrine,Desert,Forest,Island,Lair,Locus,Mine,Mountain,Plains,Power-Plant,Swamp,Tower,Urza's,Ajani,Ashiok,Bolas,Chandra,Dack,Domri,Elspeth,Garruk,Gideon,Jace,Karn,Kiora,Koth,Liliana,Nissa,Ral,Sarkhan,Sorin,Tamiyo,Tezzeret,Tibalt,Venser,Vraska,Xenagos,Arcane,Trap,Advisor,Ally,Angel,Anteater,Antelope,Ape,Archer,Archon,Artificer,Assassin,Assembly-Worker,Atog,Aurochs,Avatar,Badger,Barbarian,Basilisk,Bat,Bear,Beast,Beeble,Berserker,Bird,Blinkmoth,Boar,Bringer,Brushwagg,Camarid,Camel,Caribou,Carrier,Cat,Centaur,Cephalid,Chimera,Citizen,Cleric,Cockatrice,Construct,Coward,Crab,Crocodile,Cyclops,Dauthi,Demon,Deserter,Devil,Djinn,Dragon,Drake,Dreadnought,Drone,Druid,Dryad,Dwarf,Efreet,Elder,Eldrazi,Elemental,Elephant,Elf,Elk,Eye,Faerie,Ferret,Fish,Flagbearer,Fox,Frog,Fungus,Gargoyle,Germ,Giant,Gnome,Goat,Goblin,God,Golem,Gorgon,Graveborn,Gremlin,Griffin,Hag,Harpy,Hellion,Hippo,Hippogriff,Homarid,Homunculus,Horror,Horse,Hound,Human,Hydra,Hyena,Illusion,Imp,Incarnation,Insect,Jellyfish,Juggernaut,Kavu,Kirin,Kithkin,Knight,Kobold,Kor,Kraken,Lamia,Lammasu,Leech,Leviathan,Lhurgoyf,Licid,Lizard,Manticore,Masticore,Mercenary,Merfolk,Metathran,Minion,Minotaur,Monger,Mongoose,Monk,Moonfolk,Mutant,Myr,Mystic,Naga,Nautilus,Nephilim,Nightmare,Nightstalker,Ninja,Noggle,Nomad,Nymph,Octopus,Ogre,Ooze,Orb,Orc,Orgg,Ouphe,Ox,Oyster,Pegasus,Pentavite,Pest,Phelddagrif,Phoenix,Pincher,Pirate,Plant,Praetor,Prism,Rabbit,Rat,Rebel,Reflection,Rhino,Rigger,Rogue,Sable,Salamander,Samurai,Sand,Saproling,Satyr,Scarecrow,Scorpion,Scout,Serf,Serpent,Shade,Shaman,Shapeshifter,Sheep,Siren,Skeleton,Slith,Sliver,Slug,Snake,Soldier,Soltari,Spawn,Specter,Spellshaper,Sphinx,Spider,Spike,Spirit,Splinter,Sponge,Squid,Squirrel,Starfish,Surrakar,Survivor,Tetravite,Thalakos,Thopter,Thrull,Treefolk,Triskelavite,Troll,Turtle,Unicorn,Vampire,Vedalken,Viashino,Volver,Wall,Warrior,Weird,Werewolf,Whale,Wizard,Wolf,Wolverine,Wombat,Worm,Wraith,Wurm,Yeti,Zombie,Zubera"
                .Split(',');

        static CardTextShortener()
        {            
            _advancedRules = new Dictionary<string, MatchEvaluator>
            {
                { "([A-Z]\\w+) cards", m => Subtypes.Contains(m.Groups[1].ToString()) ? m.Groups[1].ToString() + "s" : m.Groups[1].ToString() + " cards"}, // These two rules need to be careful, because incorrect string could be matched (eg. "Draw cards equal to...")
                { "([A-Z]\\w+) card", m => Subtypes.Contains(m.Groups[1].ToString()) ? m.Groups[1].ToString() : m.Groups[1].ToString() + " card" }
            };
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
            if (card.Text != null && !card.Processed)
            {
                var rules = new List<Tuple<string, string>>();
                foreach (var line in File.ReadAllLines(@"E:\GitHub\gauntletprinter\rules.txt"))
                {
                    var lineParts = line.Split(new[] {"///"}, StringSplitOptions.None);
                    if (lineParts.Length == 2)
                        rules.Add(Tuple.Create(lineParts[0], lineParts[1]));
                }

                card.Text = card.Text.Replace(card.Name, "~");

                // The card can refer to itself with partial name (eg. Purphoros instead of Purphoros, God of the Forge)
                if (card.Name.Contains(','))
                {
                    var shortName = card.Name.Substring(0, card.Name.IndexOf(','));
                    card.Text = card.Text.Replace(shortName, "~");
                }

                foreach (var rule in rules)
                {
                    card.Text = Regex.Replace(card.Text, rule.Item1, rule.Item2, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
                }


                foreach (var rule in _advancedRules)
                {
                    card.Text = Regex.Replace(card.Text, rule.Key, rule.Value);
                }

                card.Type = card.Type.Replace('—', '-');

                card.TextLength = (int)Math.Ceiling(card.Text.Length / 40.0);

                if (omitTypeLineForBasics && card.Types.Contains("Basic"))
                {
                    card.TextLength = 0;
                }

                card.Processed = true;

                card.Text = ReplaceSymbols(card.Text, isGrayscale);
            }

            card.Type = card.Type.Replace("—", "-");  

            if (card.ManaCost != null)
            {
                card.ManaCost = ReplaceSymbols(card.ManaCost, isGrayscale);
            }
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
