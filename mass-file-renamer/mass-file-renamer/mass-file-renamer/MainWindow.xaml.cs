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
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace mass_file_renamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void addFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog addFileOpenDialog = new Microsoft.Win32.OpenFileDialog();

            addFileOpenDialog.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            addFileOpenDialog.FilterIndex = 1;

            addFileOpenDialog.Multiselect = true;

            bool? userClickedOk = addFileOpenDialog.ShowDialog();

            if (userClickedOk == true)
            {
                //  tbResults.Text = addFileOpenDialog.FileName.ToString();
            }
        }

        private void browseSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog browseSaveOpenDialog = new Microsoft.Win32.OpenFileDialog();

            browseSaveOpenDialog.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            browseSaveOpenDialog.FilterIndex = 1;

            bool? userClickedOk = browseSaveOpenDialog.ShowDialog();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }

}
