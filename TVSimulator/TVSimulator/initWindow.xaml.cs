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


        private  void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (pathTextBox.Text.Equals(""))
            {
                System.Windows.MessageBox.Show("Please select a folder");
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TimeSpan a = new TimeSpan(2,17,32);
            string str = a.ToString();
            TimeSpan b = TimeSpan.Parse(str); ;
            test.Content = b.ToString();
        }
    }
}
