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
using System.Text.RegularExpressions;

namespace mass_file_renamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       List<ReportFile> reports = new List<ReportFile>();
       public string rPrefix = "QA Rep ";
       public string saveDirectory = null;

        public MainWindow()
        {
            InitializeComponent();
        }     


        // FILE ADDING

        // Struct Creation

        // reportFile is the type, struct is the construct
        public struct ReportFile
        {
            public string fileName { get; set; }
            public string fileType { get; set; }
            public string filePath { get; set; }
            public string fileFull { get; set; }
        }

        private void FileAddButton_Click(object sender, RoutedEventArgs e)
        {
            NewFile();
        }

        public void NewFile()
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
                    ReportFile rFile = new ReportFile();
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

        private void FileRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveFile();     
        }

        public void RemoveFile()
        {

        }


        // FILE CLEAR \ RESET

        private void FileClearButton_Click(object sender, RoutedEventArgs e)
        {
            clearFile();
        }

        private void clearFile()
        {
            reports.Clear();
            this.fileDataGrid.Items.Refresh();
        }


        // SAVE DIRECTORY SELECTION

        private void SaveDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            SaveLocation();                                
        }

        private void SaveLocation()
        {
            FolderBrowserDialog browseSaveDialog = new FolderBrowserDialog();

            browseSaveDialog.ShowNewFolderButton = true;
            browseSaveDialog.ShowDialog();
            savePathField.Text = browseSaveDialog.SelectedPath.ToString();
            browseSaveDialog.SelectedPath = savePathField.Text.ToString();
            
            saveDirectory = System.IO.Path.GetFullPath(browseSaveDialog.SelectedPath);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();        
        }


        // RENAMING PROCESS

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            RenameProcess();
        }

        private void RenameProcess()
        {

            switch(supplierName.SelectedIndex)
            {
                case 0:                        
                    // Loop through each of the reportFile types in reports to process them
                    foreach (ReportFile file in reports)
                    {
                        string[] rSplit = new string[2] { null, null };

                        try
                        {
                            rSplit = S1FilenameCleaner(file.fileName);
                        }
                        catch(ArgumentException ex)
                        {
                            System.Windows.MessageBox.Show("Error processing file: " + file.fileFull + "\n" + ex.Message + "\n" + "No operation was performed.");
                            Console.WriteLine(ex.Message);

                            break;
                        }

                        // Set rName to a ToUpper conversion of fileName, to avoid case-sensitive issues
                        string rName = (rPrefix + rSplit[0] + " " + rSplit[1] + file.fileType);
                        Console.WriteLine(rName);

                        CombineStrings(saveDirectory, rName, file.fileFull);
                        
                    }
                    break;

                case 1:
                    break;

                case 2:
                    break;

                case 3:
                    break;
            }
               
        }

        private string[] S1FilenameCleaner(string inputStr)
        {
            const string PN_PATTERN = @"PN#[a-z0-9-\ ]+[^PO#\(]";
            const string PO_PATTERN = @"PO#[0-9]+";

            Match pnMatch = Regex.Match(inputStr, PN_PATTERN, RegexOptions.IgnoreCase);
            Match poMatch = Regex.Match(inputStr, PO_PATTERN, RegexOptions.IgnoreCase);

            if (!pnMatch.Success || !poMatch.Success)
                throw new ArgumentException("Filename was not in an expected format:" + "\n" + "Could not find valid PN or PO.");

            string pn = pnMatch.Value.Replace(" ", "").Substring(3);
            string po = poMatch.Value.Replace(" ", "").Substring(3);

            return new string[2] { pn, po };
        }

        private static void CombineStrings(string fSave, string fName, string fFull)
        {
            string newFile = System.IO.Path.Combine(fSave, fName);
            // check if file exists, if it does rename 
            if (File.Exists(newFile))
                return;

            File.Move(fFull, newFile);
        }


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
