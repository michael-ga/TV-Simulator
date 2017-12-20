using MediaClasses;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace YoutubeImporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Search searcher = new Search();

        public List<YouTubeChannel> channels { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            channels = new List<YouTubeChannel>();
            mListView.ItemsSource = channels;
            mListView.SelectionChanged += selectedHandler;
        }

        public void selectedHandler (Object sender, SelectionChangedEventArgs e)
        {
            var selected = ((ListView)e.Source).SelectedItem;
            string id = ((YouTubeChannel)selected).Path;
            var list = searcher.GetVideosFromChannelAsync(id);
            YoutubeEmbeddedPlayer yte = new YoutubeEmbeddedPlayer();
            yte.Show();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            channels = await searcher.customChannelSearch(SearchBox.Text, 30);
            mListView.ItemsSource = channels;
        }
    }
}
