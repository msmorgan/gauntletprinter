using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace GauntletPrinter.Downloaders
{
    public class MtgTop8Downloader : BaseDownloader
    {
        protected override Regex UriStringPattern { get; } = new Regex(
            @"(?:www\.)?mtgtop8\.com/(?:event.*d=(\d+)|export_files/deck(\d+).mwdeck)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override Task<string> GetDeckIdAsync(string uriString)
        {
            var match = UriStringPattern.Match(uriString);
            if (!match.Success) return null;

            string deckId = null;
            if (match.Groups[1].Success)
                deckId = match.Groups[1].Value;
            else if (match.Groups[2].Success)
                deckId = match.Groups[2].Value;

            return Task.Run(() => deckId);
        }

        protected override DownloadResult ParseResponse(string result)
        {
            var deckString = "";
            var sideboardString = "";

            var deckMatches = Regex.Matches(result, @"^\s+(?<count>\d+) \[[A-Z0-9]{3}\] (?<card>.+)$", RegexOptions.Multiline);

            for (int i = 0; i < deckMatches.Count; i++)
            {
                var name = deckMatches[i].Groups["card"].Captures[0].ToString().Trim();
                var count = deckMatches[i].Groups["count"].Captures[0].ToString().Trim();

                deckString += count + " " + name + Environment.NewLine;
            }

            var sideboardMatches = Regex.Matches(result, @"^SB:\s+(?<count>\d+) \[[A-Z0-9]{3}\] (?<card>.+)$", RegexOptions.Multiline);

            for (int i = 0; i < sideboardMatches.Count; i++)
            {
                var name = sideboardMatches[i].Groups["card"].Captures[0].ToString().Trim();
                var count = sideboardMatches[i].Groups["count"].Captures[0].ToString().Trim();

                sideboardString += count + " " + name + Environment.NewLine;
            }

            return new DownloadResult
            {
                MainboardText = deckString,
                SideboardText = sideboardString
            };
        }

        protected override Uri GetDownloadUri(string deckId)
        {
            return new Uri($"http://mtgtop8.com/export_files/deck{deckId}.mwDeck");
        }
    }
}
