using HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using YoutubeImporter;

namespace TVSimulator
{
        #region Fields and Constructor
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {

        private bool isSubfolders;
        private FileImporter fileImporter;
        private Search ytbSearcher;
        private List<costumPath> pathes;
        private Database db;
        public Window UI_caller;
        private TimesWindow tw = null;

        YoutubeImporter.MainWindow youtubeBrowser;
        private bool isYoutubeChannelSynced;

        public SettingWindow(Window mw)
        {
            InitializeComponent();
            db = new Database();
            UI_caller = mw;
            fileImporter = new FileImporter();
            ytbSearcher = new Search();
            pathes = new List<costumPath>();
            isYoutubeChannelSynced = false;
            streamtime_checkbox.IsChecked = isStreaming247();
        }
        #endregion


        #region Local Tab
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        // checkBox bool value
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isSubfolders = !isSubfolders;
        }

        #region reload local media pathes
        // browse folder path as media source
        private void getFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileImporter.getAllMediaFromDirectory(folderDialog.SelectedPath, isSubfolders);
                }
            }
        }

        private void change_path_click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (Properties.Settings.Default.Setting_fd_lastPath != "")
                fd.SelectedPath = Properties.Settings.Default.Setting_fd_lastPath.ToString();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.Setting_fd_lastPath = fd.SelectedPath;
                path_textBox.Text = fd.SelectedPath;
            }

        }

        private void addPathBtn_Click(object sender, RoutedEventArgs e)
        {
            if (path_textBox.Text == "")
            {
                System.Windows.MessageBox.Show("Please choose enter a folder path");
                return;
            }
            var match = pathes.Find(x => x.Path.Equals(path_textBox.Text));
            if (match != null)
            {
                System.Windows.MessageBox.Show("Folder path is already added");
                return;
            }
            costumPath tmp = new costumPath(path_textBox.Text, isIncludeSubfolders.IsChecked.Value);
            pathes.Add(tmp);
            System.Windows.MessageBox.Show("path added!");
        }

        private async void update_Click(object sender, RoutedEventArgs e)
        {
            if (pathes.Count() == 0)
            {
                System.Windows.MessageBox.Show("Please add at least one folder path");
                return;
            }
            System.Windows.MessageBox.Show("are you shure?\n all channels schedule will be lost");
            // change to yes no box..

            // close main window
            UI_caller.Close();

            // remove old channel data
            db = new Database();
            db.removeLocalMediaCollection();

            // reimport folder list and pathes and create 
            var res = await fileImporter.addDirectoryList(pathes);
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }
        #endregion


        // reset the times to create board.
        private void set_times_btn_click(object sender, RoutedEventArgs e)
        {
            // set times of watching'
            if (tw == null)
            {
                tw = new TimesWindow();
                tw.Show();
            }
            else
            {
                tw.BringIntoView();
                tw.WindowState = WindowState.Normal;
            }


        }
        

        #endregion


        #region Youtube Tab


        // start async Task in order to sync Youtbe channel
        private async void sync_btn_Click(object sender, RoutedEventArgs e)
        {
            //switch(((System.Windows.Controls.Button)sender).Name)
            //{
            //    case "sync_all_btn":
            //        ytbSearcher.syncYoutubeChannels();
            //        ytbSearcher.syncYoutubePlaylistChannels();
            //        break;
            //    case "sync_reg_btn":
            //        ytbSearcher.syncYoutubeChannels();
            //        break;
            //    case "sync_plylst_btn":
            //        ytbSearcher.syncYoutubePlaylistChannels();
            //        break;

            //    default:
            //        break;
            //}
            sync_all_btn.Click -= sync_btn_Click;
            var progressIndicator = new Progress<MyTaskProgressReport>(ReportProgress);

            await ytbSearcher.syncAllAsyncReportProgress(500, progressIndicator);
            isYoutubeChannelSynced = true;
            sync_all_btn.Click += sync_btn_Click;
        }
        private void ReportProgress(MyTaskProgressReport progress)
        {
            progress_syncLBL.Content = progress.CurrentProgressMessage;
        }

        private void launch_youtube_browser_click(object sender, RoutedEventArgs e)
        {
            if (youtubeBrowser == null)
            {
                youtubeBrowser = new YoutubeImporter.MainWindow();
                youtubeBrowser.Show();
            }
            else
                youtubeBrowser.WindowState = WindowState.Normal;
        }
        #endregion














        #region Window Closing
        // set back the focus to ui window after closing settings
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isYoutubeChannelSynced)
            {
                MainWindow s = new MainWindow();
                s.Show();
                UI_caller.Close();
            }
            else
                UI_caller.Focus();
        }

        // exit from setting window by clicking close button
        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        } 
        #endregion

        private bool isStreaming247()
        {
            var times = db.getTimes();
            foreach (int val in times.StartTime)
            {
                if (val != 0)
                    return false;
            }
            return true;
        }

        private void sync_local_btn_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
