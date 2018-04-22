using HelperClasses;
using MediaClasses;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace TVSimulator
{
    /// <summary>
    /// Interaction logic for boardWindow.xaml
    /// </summary>
    public partial class boardWindow : Window
    {

        public Database db;
        private List<Channel> chanList;
        private Channel curChar;
        private int channelNum;
        private DateTime specDay;
        private MainWindow m;

        private bool isEmpty = false;

        public boardWindow(MainWindow main)
        {
            InitializeComponent();
            db = new Database();
            m = main;
            chanList = db.getChannelList();
            curChar = m.CurrentChannel;
            channelNum = m.curChannelNum;

            specific_day.Content = DateTime.Now.ToLongDateString();
            channelName.Content = channelNum + " - " + curChar.Genre;

            specDay = DateTime.Now;         //only here specDay = today
            specDay = specDay.AddHours(-specDay.Hour);
            specDay = specDay.AddMinutes(-specDay.Minute);
            specDay = specDay.AddSeconds(-specDay.Second);
            specDay = specDay.AddMilliseconds(-specDay.Millisecond);

            board.RowBackground = Brushes.LightGray;
            buildBoardByDay(m, curChar, specDay);

        }

        private void buildBoardByDay(MainWindow m, Channel c, DateTime day)
        {
            board.Items.Clear();

            var i = 1;
            var j = m.getIndexes(m.parseChanneltoIndex(channelNum), day);
            var tommorow = day.AddDays(1);

            var temp = c.BoardSchedule.ElementAt(j).Key;

            while (DateTime.Compare(temp, tommorow) < 0) //if temp is earlier than tommorow
            {
                var t = new table();

                t.no = "" + i;
                t.startTime = c.BoardSchedule.ElementAt(j).Key.ToShortTimeString();
                Movie mov;
                TvSeries tvs;
                if (c.TypeOfMedia.Equals(Constants.MOVIE))
                {
                    mov = c.BoardSchedule.ElementAt(j).Value as Movie;
                    t.name = mov.Name;
                    t.description = mov.Description;
                }
                if (c.TypeOfMedia.Equals(Constants.TVSERIES))
                {
                    tvs = c.BoardSchedule.ElementAt(j).Value as TvSeries;
                    t.name = tvs.Name + " Season " + tvs.Season + " Episode " + tvs.Episode;
                    t.description = tvs.Description;
                }
                i++;
                j++;
                if (j >= c.BoardSchedule.Count())        //end of board
                {
                    dayUp.Visibility = Visibility.Hidden;
                    break;
                }
                temp = c.BoardSchedule.ElementAt(j).Key;
                t.endTime = c.BoardSchedule.ElementAt(j).Key.ToShortTimeString();

                board.Items.Add(t);
            }
            if (board.Items.Count == 0)
            {
                dayDown.Visibility = Visibility.Hidden;
                return;
            }
        }

            private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Return == e.Key &&
                0 < (ModifierKeys.Shift & e.KeyboardDevice.Modifiers))
            {
                var tb = (TextBox)sender;
                var caret = tb.CaretIndex;
                tb.Text = tb.Text.Insert(caret, Environment.NewLine);
                tb.CaretIndex = caret + 1;
                e.Handled = true;
            }
        }

        public class table
        {
            public string no { get; set; }
            public string startTime { get; set; }
            public string endTime { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }

        private void dayDown_Click(object sender, RoutedEventArgs e)
        {
            dayUp.Visibility = Visibility.Visible;
            specDay = specDay.AddDays(-1);
            specific_day.Content = specDay.ToLongDateString();
            buildBoardByDay(m, curChar, specDay);
        }

        private void dayUp_Click(object sender, RoutedEventArgs e)
        {
            dayDown.Visibility = Visibility.Visible;
            specDay = specDay.AddDays(1);
            specific_day.Content = specDay.ToLongDateString();
            buildBoardByDay(m, curChar, specDay);
        }

        private void channelUp_Click(object sender, RoutedEventArgs e)
        {
            channelNum = m.switchChannel(channelNum, 1);         // second paramter 1 for increament
            int index = m.parseChanneltoIndex(channelNum);
            curChar = chanList[index];
            channelName.Content = channelNum + " - " + curChar.Genre;
            buildBoardByDay(m, curChar, specDay);
        }

        private void channelDown_Click(object sender, RoutedEventArgs e)
        {
            channelNum = m.switchChannel(channelNum, -1);         // second paramter -1 for decreament
            int index = m.parseChanneltoIndex(channelNum);
            curChar = chanList[index];
            channelName.Content = channelNum + " - " + curChar.Genre;
            buildBoardByDay(m, curChar, specDay);
        }
    }
}
