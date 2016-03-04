using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace GauntletPrinter
{
    /// <summary>
    ///     Interaction logic for DeckInput.xaml
    /// </summary>
    public partial class DeckInput
    {
        public static readonly DependencyProperty DeckNumberProperty = DependencyProperty.Register(
            nameof(DeckNumber), typeof (int), typeof (DeckInput), new PropertyMetadata(default(int), DeckNumberChanged));

        public DeckInput()
        {
            InitializeComponent();
        }

        public Deck Deck => new Deck(MainboardInput.Text, SideboardInput.Text);

        public int DeckNumber
        {
            get { return (int) GetValue(DeckNumberProperty); }
            set { SetValue(DeckNumberProperty, value); }
        }

        public static void DeckNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as DeckInput;
            var val = e.NewValue as int?;

            if (me == null || !val.HasValue) return;

            me.MainboardLabel.Content = $"Mainboard {val}";
            me.SideboardLabel.Content = $"Sideboard {val}";
        }

        //private void LoadFromFileWithDialog()
        //{
        //    var dialog = new OpenFileDialog();
        //    dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
        //    if (dialog.ShowDialog() == DialogResult.OK)
        //    {
        //        LoadFromFile(dialog.FileName);
        //    }
        //}

        //private void LoadFromFile(string path)
        //{
        //    if (File.Exists(path))
        //    {
        //        var content = File.ReadAllText(path);
        //        var decks = content.Split(new string[] { "\n==========\n", "\n\r==========\n\r", "\r\n==========\r\n" }, StringSplitOptions.None);
        //        for (int i = 0; i < decks.Length; i++)
        //        {
        //            if (deckNumber + i < deckInputs.Count)
        //            {
        //                var deckString = decks[i].Trim();

        //                var sideboardSeparatorPosition = Math.Max(
        //                    deckString.LastIndexOf("\n\r", System.StringComparison.Ordinal),
        //                    deckString.LastIndexOf("\n\n", System.StringComparison.Ordinal));

        //                if (sideboardSeparatorPosition != -1)
        //                {
        //                    this.deckInputs[deckNumber + i].Text =
        //                        deckString.Substring(0, sideboardSeparatorPosition).Trim();
        //                    this.sideboardInputs[deckNumber + i].Text =
        //                        deckString.Substring(sideboardSeparatorPosition + 1).Trim();
        //                }
        //                else
        //                {
        //                    this.deckInputs[deckNumber + i].Text = deckString;
        //                    this.sideboardInputs[deckNumber + i].Text = "";
        //                }
        //            }
        //        }

        //        this.generate.Focus();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Input file \"" + path + "\" not found.");
        //    }
        //}

        private void GetFromWeb_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new GetFromWebDialog();
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            MainboardInput.Text = dialog.DeckString;
            SideboardInput.Text = dialog.SideboardString;
        }
    }
}