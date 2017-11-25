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
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        private bool isSubfolders;
        private localFileImporter fileImporter;

        public SettingWindow()
        {
            InitializeComponent();
            fileImporter = new localFileImporter();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isSubfolders = !isSubfolders;
        }

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
    }
}
