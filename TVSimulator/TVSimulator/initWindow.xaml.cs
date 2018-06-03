using HelperClasses;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;


namespace TVSimulator
{
    /// <summary>
    /// Interaction logic for initWindow.xaml
    /// </summary>
    public partial class initWindow : Window
    {
        private FileImporter fileImporter;
        private Database db;
 
        public initWindow()
        {
            InitializeComponent();
            fileImporter = new FileImporter();
            //fileImporter.OnVideoLoaded += onVideoRecievedHandler;
            db = new Database();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void chooseFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
                
            {
                if (Properties.Settings.Default.Init_fd_lastPath != "")// remember the last selected path
                {
                    folderDialog.SelectedPath = Properties.Settings.Default.Init_fd_lastPath;
                }

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    folderDialog.SelectedPath = folderDialog.SelectedPath;  // save the last selected path for navigating next time
                    pathTextBox.Text = folderDialog.SelectedPath;
                }
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
     
            if (getAllTimes() == -1)
            {
                System.Windows.MessageBox.Show("Please select hours for all the days or setup time automatically");
                return;
            }
            if(!CheckForInternetConnection())
            {
                System.Windows.MessageBox.Show("Please check your internt connection");
                return;
            }   
            

            loader.IsBusy = true;
            if (db.isYoutubeChannelListEmpty())
            {
                YoutubeImporter.Search a = new YoutubeImporter.Search();
                await a.syncAll();  // change to sync partially on start
            }
            if(pathTextBox.Text != "")
            {
                int res = await fileImporter.getAllMediaFromDirectory(pathTextBox.Text, SubfoldersCheckBox.IsChecked.Value);
                if (res == -1)
                {
                    System.Windows.MessageBox.Show("path not contains any media, please enter A path with media files");
                    loader.IsBusy = false;
                    gotoStatrtupIW();
                }
                //var t = new Task(() =>
                //{//(new Action(() => youtubePlayer.AutoPlay = true));
                //    Dispatcher.Invoke(new Action(() => loader.IsBusy = true));
                //    Dispatcher.Invoke(new Action(() => fileImporter.getAllMediaFromDirectory(pathTextBox.Text, SubfoldersCheckBox.IsChecked.Value)));
                //});
                ////t.ContinueWith(a =>
                ////{
                ////    loader.IsBusy = false;
                ////}, TaskScheduler.FromCurrentSynchronizationContext());
                //t.Start();
             }
            loader.IsBusy = false;
            MainWindow mw = new MainWindow();
            this.Close();
            mw.Show();

        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void onVideoRecievedHandler(Object o, List<Media> arg)
        {
            loader.IsBusy = false;
            MainWindow mw = new MainWindow();
            this.Close();
            mw.Show();
        }

        private void youtubeBtn_Click(object sender, RoutedEventArgs e)
        {
            YoutubeImporter.MainWindow importer = new YoutubeImporter.MainWindow();
            importer.Show();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            back_btn.Visibility = Visibility.Visible;
            var ytChanels = db.getYoutubeChannelList();
            if ((pathTextBox.Text == "") && ( ytChanels == null || ytChanels.Count < 1 ))
            {
                System.Windows.MessageBox.Show("Please select your media");
                return;
            }
            foreach(UIElement c in firstWin.Children)
            {
                c.Visibility = Visibility.Hidden;
            }
            // hide controllers of the first screen
            //getFolderBtn.Visibility = Visibility.Hidden;
            //lblAddYouTube.Visibility = Visibility.Hidden;
            //youtubeBtn.Visibility = Visibility.Hidden;
            //SubfoldersCheckBox.Visibility = Visibility.Hidden;
            //pathTextBox.Visibility = Visibility.Hidden;
            //btnNext.Visibility = Visibility.Hidden;
            // show controllers of the second screen and change background
            secondBackground.Visibility = Visibility.Visible;
            btnSubmit.Visibility = Visibility.Visible;
            isSetupAuto.Visibility = Visibility.Visible;
            timeFieldsGrid.Visibility = Visibility.Visible;
            addHours();
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            gotoStatrtupIW();
        }

        private void gotoStatrtupIW()
        {
            //show controllers of the first screen
            //getFolderBtn.Visibility = Visibility.Visible;
            //lblAddYouTube.Visibility = Visibility.Visible;
            //youtubeBtn.Visibility = Visibility.Visible;
            //SubfoldersCheckBox.Visibility = Visibility.Visible;
            //pathTextBox.Visibility = Visibility.Visible;
            //btnNext.Visibility = Visibility.Visible;
            foreach (System.Windows.Controls.Control c in firstWin.Children)
            {
                c.Visibility = Visibility.Visible;
            }

            //hide controllers of the second screen and change background
            secondBackground.Visibility = Visibility.Hidden;
            btnSubmit.Visibility = Visibility.Hidden;
            isSetupAuto.Visibility = Visibility.Hidden;
            timeFieldsGrid.Visibility = Visibility.Hidden;
            back_btn.Visibility = Visibility.Hidden;
        }

        private void isSetupAuto_Checked(object sender, RoutedEventArgs e)
        {
            int length = 7;
            for(var i=1;i<=length; i++)     //  run all over the comboBoxs 
            {
                string st = "ST" + i;
                string et = "ET" + i;
                var itemStart = timeFieldsGrid.FindName(st) as System.Windows.Controls.ComboBox;
                var itemEnd = timeFieldsGrid.FindName(et) as System.Windows.Controls.ComboBox;
                itemStart.IsEnabled = !itemStart.IsEnabled;
                itemEnd.IsEnabled = !itemEnd.IsEnabled;
                itemStart.Text = "";
                itemEnd.Text = "";
                itemEnd.Items.Clear();  // delete all items in combobox ET<number>
            }
        }

        private void addHours()
        {
            int length = 7;
            int hours = 23;
            for (var i = 1; i <= length; i++)     //  run all over the comboBoxs 
            {
                string st = "ST" + i;
                var itemStart = timeFieldsGrid.FindName(st) as System.Windows.Controls.ComboBox;
                for (var j = 0; j <= hours; j++)
                {
                    if (j == 0)
                        itemStart.Items.Add("00:00");
                    else
                    {
                        var str = j + ":00";
                        itemStart.Items.Add(str);
                    }
                }
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var value = (sender as System.Windows.Controls.ComboBox).SelectedItem as string;    // get value
            if (value == null)
                return;
            int index = value.IndexOf(':');
            int numValue = Int32.Parse(value.Substring(0, index));     // parse value to int

            var name = (sender as System.Windows.Controls.ComboBox).Name; // get the name of combobox
            var numComboBox = Int32.Parse(name.Substring(2));                   
            
            string et = "ET" + numComboBox;
            var itemEnd = timeFieldsGrid.FindName(et) as System.Windows.Controls.ComboBox;
            itemEnd.Items.Clear();
            int hours = 24;
            for (var j = numValue+1; j <= hours; j++)
            {
                    var str = j + ":00";
                    itemEnd.Items.Add(str);
            }
        }

        private int getAllTimes()
        {
            int length = 7;
            int[] startTime = new int[length];
            int[] endTime = new int[length];


            for (var i = 1; i <= length; i++)     //  run all over the comboBoxs 
            {
                if (isSetupAuto.IsChecked.Value)    //automatically from 00:00 - 24:00 
                {
                    startTime[i-1] = 0;
                    endTime[i-1] = 24;
                }
                else
                {
                    //get start time
                    string st = "ST" + i;
                    var itemStart = timeFieldsGrid.FindName(st) as System.Windows.Controls.ComboBox;
                    var value1 = itemStart.SelectedItem as string;
                    if (value1 == null)
                        return -1;
                    int index1 = value1.IndexOf(':');
                    int startValue = Int32.Parse(value1.Substring(0, index1));     // parse value to int

                    //get end time
                    string et = "ET" + i;
                    var itemEnd = timeFieldsGrid.FindName(et) as System.Windows.Controls.ComboBox;
                    var value2 = itemEnd.SelectedItem as string;
                    if (value2 == null)
                        return -1;
                    int index2 = value2.IndexOf(':');
                    int endValue = Int32.Parse(value2.Substring(0, index2));     // parse value to int

                    startTime[i - 1] = startValue;
                    endTime[i - 1] = endValue;
                }
            }

            DateTime startCycle = new DateTime();
            startCycle = DateTime.Now;

            BroadcastTime bt = new BroadcastTime(startCycle,startTime,endTime);
            db.insertBroadcastTime(bt);     // check if need to be single value!!!!!!!!!!!
            return 0;
        }

       
    }
}
