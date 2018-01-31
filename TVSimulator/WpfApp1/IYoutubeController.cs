using MyToolkit.Multimedia;

using System.Windows.Input;

namespace YoutubeImporter
{
    interface IYoutubeController
    {
        YouTubeQuality CurrentQuality { get; set; }

        string VideoId { get; set; }

        int Volume { get; set; }

        bool AutoPlay { get; set; }

        ICommand StartCommand { get; }

        ICommand StopCommand { get; }

        ICommand PauseCommand { get; }
    }
}
