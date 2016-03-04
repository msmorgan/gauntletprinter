using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GauntletPrinter.Downloaders
{
    public class MtgGoldfishDownloader : BaseDownloader
    {
        protected override Regex UriStringPattern { get; } = new Regex(
            @"(?:www\.)?mtggoldfish\.com/(?:(?:deck|download|deck/download)/(\d+)|archetype/([a-z\d-]+))",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override async Task<string> GetDeckIdAsync(string uriString)
        {
            var match = UriStringPattern.Match(uriString);

            if (match.Groups[1].Success)
            {
                return match.Groups[1].Value;
            }

            if (match.Groups[2].Success)
            {
                var archetypeId = match.Groups[2].Value;
                var archetypeUri = GetArchetypeUri(archetypeId);
                var archetypeHtml = await new WebClient().DownloadStringTaskAsync(archetypeUri);
                var hrefMatch = Regex.Match(archetypeHtml, @"href=""/deck/download/(\d+)""");

                if (hrefMatch.Groups[1].Success)
                    return hrefMatch.Groups[1].Value;
            }

            return null;
        }

        protected override DownloadResult ParseResponse(string responseText)
        {
            responseText = Regex.Replace(responseText, "^//.*$", "", RegexOptions.Multiline);

            var sideboardSeparatorPosition = Math.Max(
                responseText.LastIndexOf("\n\r", StringComparison.Ordinal),
                responseText.LastIndexOf("\n\n", StringComparison.Ordinal));

            return new DownloadResult
            {
                MainboardText = responseText.Substring(0, sideboardSeparatorPosition).Trim(),
                SideboardText = sideboardSeparatorPosition >= 0 ? responseText.Substring(sideboardSeparatorPosition + 1).Trim() : "",
            };
        }

        protected override Uri GetDownloadUri(string deckId)
        {
            return new Uri($"http://www.mtggoldfish.com/deck/download/{deckId}");
        }

        protected Uri GetArchetypeUri(string archetypeId)
        {
            return new Uri($"http://www.mtggoldfish.com/archetype/{archetypeId}");
        }
    }
}
