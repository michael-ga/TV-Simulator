using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoutubeImporter;

using MediaClasses;
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
        }
     


        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            channels = await searcher.customChannelSearch(SearchBox.Text, 30);
            mListView.ItemsSource = channels;
        }
    }
}
