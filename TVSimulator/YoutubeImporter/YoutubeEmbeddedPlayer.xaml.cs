using MediaClasses;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;


namespace YoutubeImporter
{
    /// <summary>
    /// Interaction logic for YoutubeEmbeddedPlayer.xaml
    /// </summary>
    public partial class YoutubeEmbeddedPlayer : Window
    {
        private List<YoutubeVideo> videos;
        private string currentVideoId;
        public string CurrentVideoId { get => currentVideoId; set => currentVideoId = value; }

        public YoutubeEmbeddedPlayer()
        {
            InitializeComponent();
            Player.GetBindingExpression(YoutubeImporter.Cef.CefYoutubeController.VideoIdProperty).UpdateSource();
        }
        public YoutubeEmbeddedPlayer(List<YoutubeVideo> videoList)
        {
            InitializeComponent();
            videos = videoList;
            currentVideoId = videos[0].Path;
            TxtVideoId.Text = videos[0].Path;
            Player.GetBindingExpression(YoutubeImporter.Cef.CefYoutubeController.VideoIdProperty).UpdateSource();
        }

        public YoutubeEmbeddedPlayer(string id)
        {
            InitializeComponent();
            TxtVideoId.Text = id;
            Player.GetBindingExpression(YoutubeImporter.Cef.CefYoutubeController.VideoIdProperty).UpdateSource();
        }


        private void playNext()
        {

        }
        private void TxtVideoId_KeyDown(object sender, KeyEventArgs e)
        {

        }
        //
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Escape)
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.ToolWindow;
            }
        }

       
    }
}
