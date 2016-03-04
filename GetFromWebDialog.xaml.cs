using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GauntletPrinter.Downloaders;

namespace GauntletPrinter
{
    /// <summary>
    /// Interaction logic for GetFromWebDialog.xaml
    /// </summary>
    public partial class GetFromWebDialog
    {
        public string DeckString { get; set; }
        public string SideboardString { get; set; }

        private bool _isClosed;

        public GetFromWebDialog()
        {
            InitializeComponent();
        }

        private readonly IList<BaseDownloader> _downloaders = new List<BaseDownloader>
        {
            new MtgGoldfishDownloader(),
            new MtgTop8Downloader(),
            new TappedOutDownloader(),

        }; 

        private async void Download_OnClick(object sender, RoutedEventArgs e)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1));

            var uriString = deckUrl.Text;
            foreach (var downloader in _downloaders)
            {
                if (!downloader.CanDownload(uriString)) continue;

                var downloadResult = await downloader.DownloadAsync(uriString);
                if (downloadResult != null)
                {
                    DeckString = downloadResult.MainboardText;
                    SideboardString = downloadResult.SideboardText;

                    DialogResult = true;
                    Close();
                    return;
                }
            }

            MessageBox.Show("Unsupported website.");

            download.IsEnabled = true;
        }

        private void GetFromWebDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            deckUrl.Focus();
        }

        private void DeckUrl_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Download_OnClick(sender, null);
            }
        }

        private void GetFromWebDialog_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void GetFromWebDialog_OnClosed(object sender, EventArgs e)
        {
            _isClosed = true;
        }
    }
}
