using HelperClasses;
using System;
using System.Windows;
using System.Windows.Controls;

namespace TVSimulator
{
    /// <summary>
    /// Interaction logic for TimesWindow.xaml
    /// </summary>
    public partial class TimesWindow : Window
    {
        private Database db;
        private SettingWindow settings;
        public TimesWindow(SettingWindow _settings)
        {
            InitializeComponent();
            addHours();
            settings = _settings;
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

        private void setTimes(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you shure you want to reschedule all channels?", "Warning", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.No)
            {
                return;
            }
            db = new Database();

            int length = 7;
            int[] startTime = new int[length];
            int[] endTime = new int[length];


            for (var i = 1; i <= length; i++)     //  run all over the comboBoxs 
            {
                if (((Button)sender).Name.Equals("unlimited_time_btn"))    //automatically from 00:00 - 24:00 
                {
                    startTime[i - 1] = 0;
                    endTime[i - 1] = 24;
                }
                else
                {
                    //get start time
                    string st = "ST" + i;
                    var itemStart = timeFieldsGrid.FindName(st) as System.Windows.Controls.ComboBox;
                    var value1 = itemStart.SelectedItem as string;
                    if (value1 == null)
                    {
                        MessageBox.Show("Error: please select all start and end time fields");
                        return;
                    }
                    int index1 = value1.IndexOf(':');
                    int startValue = Int32.Parse(value1.Substring(0, index1));     // parse value to int

                    //get end time
                    string et = "ET" + i;
                    var itemEnd = timeFieldsGrid.FindName(et) as System.Windows.Controls.ComboBox;
                    var value2 = itemEnd.SelectedItem as string;
                    if (value2 == null)
                    {
                        MessageBox.Show("Error: please select all start and end time fields");
                        return;
                    }
                    int index2 = value2.IndexOf(':');
                    int endValue = Int32.Parse(value2.Substring(0, index2));     // parse value to int

                    startTime[i - 1] = startValue;
                    endTime[i - 1] = endValue;
                }
            }

            DateTime startCycle = new DateTime();
            startCycle = DateTime.Now;

            BroadcastTime bt = new BroadcastTime(startCycle, startTime, endTime);
            db.insertBroadcastTime(bt);     // check if need to be single value!!!!!!!!!!!
            MessageBox.Show("Times has been set successfully");
            settings.IsLocalChannelsSynced = true;
            this.Close();
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
            for (var j = numValue + 1; j <= hours; j++)
            {
                var str = j + ":00";
                itemEnd.Items.Add(str);
            }
        }

        
    }
}
