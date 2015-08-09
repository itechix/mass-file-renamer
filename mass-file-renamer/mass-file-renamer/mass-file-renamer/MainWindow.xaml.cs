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
       List<reportFile> reports = new List<reportFile>();
       public string rPrefix = "QA Rep ";

        public MainWindow()
        {
            InitializeComponent();  

        }     


        // FILE ADDING

        // Struct Creation
        // reportFile is the type, struct is the construct
        public struct reportFile
        {
            public string fileName { get; set; }
            public string fileType { get; set; }
            public string filePath { get; set; }
            public string fileFull { get; set; }
        }

        private void fileAddButton_Click(object sender, RoutedEventArgs e)
        {
            newFile();
        }

        public void newFile()
        {
            
      
            Microsoft.Win32.OpenFileDialog addFileOpenDialog = new Microsoft.Win32.OpenFileDialog();
            addFileOpenDialog.Title = "Select Files:";
            addFileOpenDialog.Filter = "All Files (*.*)|*.*";
            addFileOpenDialog.FilterIndex = 1;
            addFileOpenDialog.Multiselect = true;
            // Display the FileOpen Dialog
            bool? userClickedOk = addFileOpenDialog.ShowDialog();

            if (userClickedOk == true)
            {
                foreach (String file in addFileOpenDialog.FileNames)
                {
                    reportFile rFile = new reportFile();
                    rFile.fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                    rFile.filePath = System.IO.Path.GetDirectoryName(file);
                    // Leaving out fileType for now, maybe?
                    rFile.fileType = System.IO.Path.GetExtension(file);
                    rFile.fileFull = System.IO.Path.GetFullPath(file);
                    reports.Add(rFile);
                    Console.WriteLine();
                }
            }
            // Refresh the ItemsSource in the DataGrid.
            this.fileDataGrid.ItemsSource = reports;

            // Force the DataGrid to redraw it's elements.
            this.fileDataGrid.Items.Refresh();
        }


        // FILE REMOVING

        private void fileRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            removeFile();     
        }

        public void removeFile()
        {

        }


        // FILE CLEAR \ RESET

        private void fileClearButton_Click(object sender, RoutedEventArgs e)
        {
            clearFile();
        }

        private void clearFile()
        {
            reports.Clear();
            this.fileDataGrid.Items.Refresh();
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


        // RENAMING PROCESS

        private void renameButton_Click(object sender, RoutedEventArgs e)
        {
            renameProcess();
        }

        private void renameProcess()
        {
           
            // Index number = supplier name (0 = Allfavor, 1 = Electech etc)
            if (supplierName.SelectedIndex == 0)
            {
                
                // Loop through each of the reportFile types in reports to process them
                foreach (reportFile file in reports)
                {
                    // Set rName to a ToUpper conversion of fileName, to avoid case-sensitive issues
                    string rName = file.fileName.ToUpper();

                    // Messy Replace, insert StringBuilder instead?
                    rName = rName.Replace("PO#", "");
                    rName = rName.Replace("PN#", "");
                    Console.WriteLine(rName);
                    
                    // Get the index value of the first "(" in rName and assign the value to rEnd
                    int rEnd = rName.IndexOf("(");
                    // Check whether the index value is the first character or not
                    if (rEnd > 0)
                        // Take the characters from the first index value up until the bracket and replace the string with them
                        rName = rName.Substring(0, rEnd);
                    Console.WriteLine(rName);

                    // Split each section of rName (separated by " ") and put it into an array
                    string[] rSplit = rName.Split(' ');
                    Array.Reverse(rSplit);

                    rName = (rPrefix + rSplit[0] + " " + rSplit[1] + file.fileType);
                    Console.WriteLine(rName);

                    CombineStrings(saveLocation(), rName, file.fileFull);
                }
                
            }

            else
                Console.WriteLine(supplierName.SelectedIndex);

            // repeat for different formats / suppliers below
             /*if(supplierName.SelectedIndex == 2)
             {

             }*/
        }

        private static void CombineStrings(string fSave, string fName, string fFull)
        {
            string newFile = System.IO.Path.Combine(fSave, fName);
            // check if file exists, if it does rename 
            if (File.Exists(newFile))
                return;

            File.Move(fFull, newFile);
        }

        // adding more files overwrites original files in datagrid


        // test code, ignore for now?

        /*

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
