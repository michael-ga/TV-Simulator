using HelperClasses;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace TVSimulator
{
    /// <summary>
    /// Interaction logic for initWindow.xaml
    /// </summary>
    public partial class initWindow : Window
    {
        FileImporter fileImporter;
        public Database db;
 
        public initWindow()
        {
            InitializeComponent();
            fileImporter = new FileImporter();
            fileImporter.OnVideoLoaded += onVideoRecievedHandler;
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
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    pathTextBox.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            /*if (pathTextBox.Text.Equals(""))
            {
                System.Windows.MessageBox.Show("Please select a folder");
                return;
            }*/

            if (getAllTimes() == -1)
            {
                System.Windows.MessageBox.Show("Please select hours for all the days or setup time automatically");
                return;
            }
            

            loader.IsBusy = true;
            fileImporter.getAllMediaFromDirectory(pathTextBox.Text, SubfoldersCheckBox.IsChecked.Value);
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
            if (pathTextBox.Text == "" || pathTextBox.Text == null)
            {
                System.Windows.MessageBox.Show("Please select folder");
                return;
            }

            //hide controllers of the first screen
            getFolderBtn.Visibility = Visibility.Hidden;
            lblAddYouTube.Visibility = Visibility.Hidden;
            youtubeBtn.Visibility = Visibility.Hidden;
            SubfoldersCheckBox.Visibility = Visibility.Hidden;
            pathTextBox.Visibility = Visibility.Hidden;
            btnNext.Visibility = Visibility.Hidden;

            //show controllers of the second screen and change background
            secondBackground.Visibility = Visibility.Visible;
            btnSubmit.Visibility = Visibility.Visible;
            isSetupAuto.Visibility = Visibility.Visible;
            timeFieldsGrid.Visibility = Visibility.Visible;
            addHours();
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
            db = new Database();
            db.insertBroadcastTime(bt);
            return 0;
        }

       
    }
}
