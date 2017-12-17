using MediaClasses;
using System;
using System.Collections.Generic;
using System.Windows;
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

        public void selectedHandler (Object sender,EventArgs e)
        {
            //   var temp = mListView.SelectedValue as YouTubeChannel;
            //   var res = await searcher.GetVideosFromChannelAsync(temp.Path);
            //   var uri = await searcher.GetYoutubeUri("UO-8CMdeSHA");
            //;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            channels = await searcher.customChannelSearch(SearchBox.Text, 30);
            mListView.ItemsSource = channels;
        }
    }
}
