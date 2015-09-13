using System;
using System.Collections.Generic;
using System.Windows;
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
            public string fileOld { get; set; }
            public string fileNew { get; set; }
        }

        private void FileAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (supplierName.SelectedIndex == -1)
            {
                System.Windows.MessageBox.Show("Error:" + "\n" + "No supplier was selected." + "\n" + "Please select a supplier before continuing.");
            }
            else
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
                    rFile.fileType = System.IO.Path.GetExtension(file);
                    rFile.fileOld = System.IO.Path.GetFullPath(file);
                    rFile.fileNew = CleanupProcess(rFile.fileName, rFile.fileType);
                    if(rFile.fileNew == null)
                    {
                        System.Windows.MessageBox.Show("Filename was not in an expected format:" + "\n" + "Could not find valid PN or PO.");
                        return;
                    }
                    else
                        reports.Add(rFile);
                }
            }
            // Refresh the ItemsSource in the DataGrid.
            this.fileGridSelected.ItemsSource = reports;

            // Force the DataGrid to redraw it's elements.
            this.fileGridSelected.Items.Refresh();
        }


        // FILE REMOVING

        private void FileRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveFile();     
        }

        public void RemoveFile()
        {
            // Check if an item is actually selected, first.
            if (fileGridSelected.SelectedIndex == -1)
            {
                System.Windows.MessageBox.Show("Please select an item to remove.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
                
            // This gets the ReportFile that is bound to that item in the list, and puts it in a variable.
            // We know that it will be a ReportFile, the compiler doesn't, so we 'cast' it explicity.
            ReportFile selectedReportFile = (ReportFile)fileGridSelected.SelectedValue;

            // Now we get the index of the ReportFile bound to the item in the list. This is a search.
            int remIndex = reports.IndexOf(selectedReportFile);

            // Now we remove the report at the index we got
            reports.RemoveAt(remIndex);

            // Refresh the ItemsSource in the DataGrid.
            this.fileGridSelected.ItemsSource = reports;

            // Force the DataGrid to redraw it's elements.
            this.fileGridSelected.Items.Refresh();
            
        }


        // FILE CLEAR \ RESET

        private void FileClearButton_Click(object sender, RoutedEventArgs e)
        {
            clearFile();
        }

        private void clearFile()
        {
            reports.Clear();
            this.fileGridSelected.Items.Refresh();
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


        private string CleanupProcess(string fName, string fExt)
        {

            switch(supplierName.SelectedIndex)
            {
                // ALLFAVOUR
                case 0:                        
                    return S1FilenameCleaner(fName, fExt); 

                case 1:
                    return S2FilenameCleaner(fName, fExt); 

                case 2:
                    return S3FilenameCleaner(fName, fExt); 
            }

            return null;
        }

        private string S1FilenameCleaner(string fName, string fExt)
        {
            const string PN_PATTERN = @"PN#[a-z0-9-\ ]+[^PO#\(]";
            const string PO_PATTERN = @"PO#[0-9]+";
            Match pnMatch = Regex.Match(fName, PN_PATTERN, RegexOptions.IgnoreCase);
            Match poMatch = Regex.Match(fName, PO_PATTERN, RegexOptions.IgnoreCase);
            // If pnMatch and poMatch were not successful, perform the following lines
            if (!pnMatch.Success || !poMatch.Success)
            {
                return null;
            }
            string pn = pnMatch.Value.Replace(" ", "").Substring(3);
            string po = poMatch.Value.Replace(" ", "").Substring(3);
            string rName = (rPrefix + pn + " " + po + fExt);
            return rName;
        }

        // Suntak - A11356-A    0000037486  QA report
        private string S2FilenameCleaner(string fName, string fExt)
        {
            const string FILECHECK = @"[a-z0-9\-\ ]+QA Report";
            Match fileMatch = Regex.Match(fName, FILECHECK, RegexOptions.IgnoreCase);

            if(!fileMatch.Success)
            {
                return null;
            }

            string[] splitStr = fName.Split((char[])null, 3, StringSplitOptions.RemoveEmptyEntries);
            splitStr[1] = splitStr[1].Substring(splitStr[1].IndexOf('3'));
            string rName = (rPrefix + splitStr[0] + " " + splitStr[1] + fExt);
            return rName;
        }

        // United Electech - 7358-B-37465-2915
        private string S3FilenameCleaner(string fName, string fExt)
        {
            const string FILECHECK = @"[0-9]+\-[a-z]\-+[0-9]+\-[0-9]+";
            Match fileMatch = Regex.Match(fName, FILECHECK, RegexOptions.IgnoreCase);

            if (!fileMatch.Success)
            {
                return null;
            }

            string[] splitStr = fName.Split(new[] { '-' } , 4, StringSplitOptions.RemoveEmptyEntries);
            string rName = (rPrefix + splitStr[0] + "-" + splitStr[1] + " " + splitStr[2] + fExt);
            return rName;
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            // Loop through each of the reportFile types in reports to process them
            foreach (ReportFile file in reports)
            {
                CombineStrings(saveDirectory, file.fileNew, file.fileOld);
            }
        }

        private static void CombineStrings(string fSave, string fName, string fFull)
        {
            string newFile = System.IO.Path.Combine(fSave, fName);
            // check if file exists, if it does rename 
            if (File.Exists(newFile))
                return;

            File.Move(fFull, newFile);
        }

        private void fileGridSelected_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Console.WriteLine("SIN: {0} RIN: {1}", fileGridSelected.SelectedIndex, reports.Count);
        }
    }
}

// Suntak and Electech suppliers crash app when invalid files. - ADD CHECK SYSTEM
//
