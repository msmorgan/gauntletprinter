using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GauntletPrinter.Downloaders
{
    public abstract class BaseDownloader
    {
        protected abstract Regex UriStringPattern { get; }

        public bool CanDownload(string uriString)
        {
            return UriStringPattern.Match(uriString).Success;
        }

        protected abstract Task<string> GetDeckIdAsync(string uriString);

        protected string GetDeckId(string uriString)
        {
            return Task.Run(() => GetDeckIdAsync(uriString)).Result;
        }

        public DownloadResult Download(string uriString)
        {
            return Task.Run(() => DownloadAsync(uriString)).Result;
        }

        public async Task<DownloadResult> DownloadAsync(string uriString)
        {
            var deckId = await GetDeckIdAsync(uriString);
            if (deckId == null) return null;

            var requestUri = GetDownloadUri(deckId);
            var responseText = await new WebClient().DownloadStringTaskAsync(requestUri);
            return ParseResponse(responseText);
        }

        protected abstract DownloadResult ParseResponse(string responseText);
        protected abstract Uri GetDownloadUri(string deckId);
    }
}