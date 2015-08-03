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
using System.IO;
using System.Windows.Forms;

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


        // FILE ADDING

        // File 


        private void fileAddButton_Click(object sender, RoutedEventArgs e)
        {
            newFile();
        }

        public struct reportFile
        {
            public string fileName;
            public string fileType;
            public string filePath;
        }

        public string newFile()
        {
            List<reportFile> reports = new List<reportFile>();
      
            Microsoft.Win32.OpenFileDialog addFileOpenDialog = new Microsoft.Win32.OpenFileDialog();
            addFileOpenDialog.Title = "Select Files:";
            addFileOpenDialog.Filter = "All Files (*.*)|*.*";
            addFileOpenDialog.FilterIndex = 1;
            addFileOpenDialog.Multiselect = true;
            // Show the FileOpen Dialog
            bool? userClickedOk = addFileOpenDialog.ShowDialog();

            if (userClickedOk == true)
            {
                foreach (String file in addFileOpenDialog.FileNames)
                {
                    reportFile rFile = new reportFile();
                    rFile.fileName = System.IO.Path.GetFileName(file);
                    rFile.filePath = System.IO.Path.GetFullPath(file);
                    // Leaving out fileType for now, maybe?
                    reports.Add(rFile);
                    Console.WriteLine();
                }

            }
            
            return null;
        }

        // FILE REMOVING

        private void fileRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            removeFile();     
        }

        public string removeFile()
        {
            return null;
        }

        // FILE CLEAR \ RESET

        private void fileClearButton_Click(object sender, RoutedEventArgs e)
        {
            clearFile();
        }

        private void clearFile()
        {
            
        }


        // SAVE DIRECTORY SELECTION

        private void saveDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            saveLocation();                                
        }

        public string saveLocation()
        {
            FolderBrowserDialog browseSaveDialog = new FolderBrowserDialog();

            browseSaveDialog.ShowNewFolderButton = true;
            browseSaveDialog.ShowDialog();
            savePathField.Text = browseSaveDialog.SelectedPath.ToString();
            browseSaveDialog.SelectedPath = savePathField.Text.ToString();
            
            return System.IO.Path.GetFullPath(browseSaveDialog.SelectedPath);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();        
        }

        private static void CombineStrings(string pF, string pS)
        {
            string newFile = System.IO.Path.Combine(pF, pS);
            // check if file exists, if it does rename 
            if (File.Exists(newFile))
            return;

            //File.Move(pathFile)
        }

        private void renameButton_Click(object sender, RoutedEventArgs e)
        {

        }
      


        // test code, ignore for now?

        /*
        public string savePath()
        {
            FolderBrowserDialog browseSaveDialog = new FolderBrowserDialog();

            browseSaveDialog.ShowNewFolderButton = true;
            browseSaveDialog.ShowDialog();
            savePathField.Text = browseSaveDialog.SelectedPath.ToString();
            browseSaveDialog.SelectedPath = savePathField.Text.ToString();

            string pathSave = browseSaveDialog.SelectedPath.ToString();

            return pathSave;
        }

        private void renameButton_Click(object sender, RoutedEventArgs e)
        {
            CombineStrings(savePath(), newFile());
            // I think I messed up this?
        }

        private static void CombineStrings(string pF, string pS)
        {
            string newFile = System.IO.Path.Combine(pF, pS);
        }
         */

    }
        

}

/////// Different suppliers that will be implemented

// ELECTECH LIMITED: " MER-A-ORDERNUMBER DC.file " " A13519-A-36692 1915.pdf "
// ALLFAVOR: " poORDERNUMBER pn#MER-A(DC).file " "po0036585 pn#12152-A(1715).xls "
// SUNTAK: " MER-A ORDERNUMBER QA Report.file " "A12607-A 0000036667 QA Report.xlsx "
