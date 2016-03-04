using System;
using System.Collections.Generic;

namespace GauntletPrinter
{
    public class Card
    {
        //public string Id { get; set; }
        public string Layout { get; set; }
        public string Name { get; set; }
        public IList<string> Names { get; set; }
        public string ManaCost { get; set; }
        public int Cmc { get; set; }
        public IList<string> Colors { get; set; }
        public IList<string> ColorIdentity { get; set; }
        public string Type { get; set; }
        public IList<string> Supertypes { get; set; }
        public IList<string> Types { get; set; }
        public IList<string> Subtypes { get; set; }
        //public string Rarity { get; set; }
        public string Text { get; set; }
        //public string Flavor { get; set; }
        //public string Artist { get; set; }
        //public string Number { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public int? Loyalty { get; set; }
        //public int MultiverseId { get; set; }
        //public IList<int> Variations { get; set; }
        //public string Watermark { get; set; }
        //public string Border { get; set; }
        //public bool? Timeshifted { get; set; }
        //public int? Hand { get; set; }
        //public int? Life { get; set; }
        //public bool Reserved { get; set; }
        //public DateTime? ReleaseDate { get; set; }
        //public bool Starter { get; set; }


        public bool Processed { get; set; } = false;


        public int TextLength { get; set; }


        public override string ToString() => Name;
    }
}