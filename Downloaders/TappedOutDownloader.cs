using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GauntletPrinter.Downloaders
{
    public class TappedOutDownloader : BaseDownloader
    {
        protected override Regex UriStringPattern { get; } = new Regex(
            @"(?:www\.)?tappedout\.net/mtg-decks/([a-z0-9-]+)/$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override Task<string> GetDeckIdAsync(string uriString)
        {
            var match = UriStringPattern.Match(uriString);
            if (match.Success && match.Groups[1].Success)
                return Task.Run(() => match.Groups[1].Value);

            return null;
        }

        protected override DownloadResult ParseResponse(string responseText)
        {
            var halves = responseText.Split(new[] {"Sideboard:"}, StringSplitOptions.None);

            return new DownloadResult
            {
                MainboardText = halves[0].Trim().Replace("/", "//"),
                SideboardText = halves.Length > 0 ? halves[1].Trim().Replace("/", "//") : ""
            };
        }

        protected override Uri GetDownloadUri(string deckId)
        {
            return new Uri($"http://www.tappedout.net/mtg-decks/{deckId}/?fmt=txt");
        }
    }
}