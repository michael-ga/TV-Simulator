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
using System.Windows.Shapes;

namespace YoutubeImporter
{
    /// <summary>
    /// Interaction logic for YoutubeEmbeddedPlayer.xaml
    /// </summary>
    public partial class YoutubeEmbeddedPlayer : Window
    {
        public YoutubeEmbeddedPlayer(string id)
        {
            InitializeComponent();
            TxtVideoId.Text = id;
            Player.GetBindingExpression(YoutubeImporter.Cef.CefYoutubeController.VideoIdProperty).UpdateSource();
           
        }
        public void bindID(string id)
        {
        }

        private void TxtVideoId_KeyDown(object sender, KeyEventArgs e)
        {

        }

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
