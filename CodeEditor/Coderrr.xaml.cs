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
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CodeEditor
{
    /// <summary>
    /// Interaction logic for Coderrr.xaml
    /// </summary>
    public partial class Coderrr : Window
    {
        // Current file
        private string currentFile;
        // File-path
        private string filePath;
        // Safefilepath
        private string safeFilePath;
        // Autosave
        private bool running;
        // Reference to preferences
        private Preferences pref;
        //
        private Prefs prefs;
        private XMLHandler xmlhandler;
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Coderrr()
        {
            InitializeComponent();
            xmlhandler = new XMLHandler();
            // XML
            prefs = new Prefs();

        }

        /// <summary>
        /// Exits the application from the File-menu "Exit" item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event contents</param>
        private void ExitApp(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }        
       
        /// <summary>
        /// Determine whether the command can be executed on the command target.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event contents</param>
        private void OpenCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the command associated with this CommandBinding executes.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event contents</param>
        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFile();
            e.Handled = true;
        }
        
        /// <summary>
        /// Determine whether the command can be executed on the command target.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event contents</param>
        private void SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the command associated with this CommandBinding executes.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event contents</param>
        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFile();
            e.Handled = true;
        }

        /// <summary>
        /// Opens a .cs file to the code-editors richtextbox.
        /// </summary>
        public void OpenFile()
        {
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            // Set the filter to look for *.cs (C#) files
            openFile.Filter = "C# - files (*.cs)|*.cs|All Files|*.*";
            // If OK is pressed, set the filePath variable to the file-path
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = openFile.FileName;
                currentFile = openFile.FileName;
                safeFilePath = openFile.SafeFileName;
            }
            // If file exists..            
            if (File.Exists(filePath))
            {                                
                // TextRange takes the start and the end positions of the richtextbox
                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                // FileStream opens the file from the path and makes it readable and writable
                FileStream fStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                // Loads the data in text format
                range.Load(fStream, DataFormats.Text);
                fStream.Close();
            }            
        }

        /// <summary>
        /// Saves a .cs file depending on application status. If application doesn't have content yet,
        /// try to save the contents to a new file, else do the quick-save.
        /// </summary>
        public void SaveFile()
        {
            if (String.IsNullOrEmpty(filePath))
            {
                SaveAsFile();
            }
            else
            {
                QuickSave();
            }
        }

        /// <summary>
        /// Saves a new .cs file.
        /// </summary>
        public void SaveAsFile()
        {
            try
            {
                System.Windows.Forms.SaveFileDialog saveFile = new System.Windows.Forms.SaveFileDialog();
                saveFile.FileName = ".cs";
                saveFile.DefaultExt = ".cs";
                saveFile.Filter = "C# - files (.cs)|*.cs";

                if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    filePath = saveFile.FileName;
                    safeFilePath = System.IO.Path.GetFileName(saveFile.FileName);
                    currentFile = saveFile.FileName;
                    
                }

                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                FileStream fStream = new FileStream(filePath, FileMode.Create);
                range.Save(fStream, DataFormats.Text);
                fStream.Close();
            }
            catch (Exception ex)
            {
                SaveAsFile();
            }
        }

        /// <summary>
        /// Saves the contents of the file to the existing file.
        /// </summary>
        public void QuickSave()
        {
            TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            FileStream fStream = new FileStream(filePath, FileMode.Create);
            range.Save(fStream, DataFormats.Text);
            fStream.Close();
        }

        /// <summary>
        /// Parses the file name for running the .exe application.
        /// </summary>
        /// <param name="input">The file name to be parsed</param>
        /// <returns></returns>
        public string ParseFileName(string input)
        {
            string result = input.Substring(0, input.Length - 3);
            return result;
        }       

        /// <summary>
        /// Handles the event for the Save As option.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event contents</param>
        private void SaveAs_Clicked(object sender, RoutedEventArgs e)
        {
            SaveAsFile();
        }

        /// <summary>
        /// Opens the preferences menu.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event contents</param>
        private void Preferences_Clicked(object sender, RoutedEventArgs e)
        {
            
            // Window
            pref = new Preferences(prefs);
            pref.Owner = this;
            pref.ShowDialog();            
        }

        /// <summary>
        /// Compiles the current .cs file
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event contents</param>
        private void Compile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process compiler = new Process();
                compiler.StartInfo.FileName = loadXML();               
                compiler.StartInfo.Arguments = currentFile;
                compiler.StartInfo.UseShellExecute = false;
                compiler.StartInfo.RedirectStandardOutput = true;
                compiler.Start();

                Console.WriteLine(compiler.StandardOutput.ReadToEnd());

                compiler.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Go to Preferences to set your csc.exe filepath.");
            }
                        
            
                
           // prefs.filePath = pref.filePath;
            
                           
        }

        /// <summary>
        /// Shows author information.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event contents</param>
        private void Information_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Author Information\n\nAuthor: Otto Kivikärki\nVersion: 0.1\nRelease date: 2013-2-17");
        }

        /// <summary>
        /// Runs the compiled .cs file.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event contents</param>
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = @"cmd";
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Arguments = "/C " + ParseFileName(safeFilePath) + ".exe";
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Compile your file first");
            }
        }

        /// <summary>
        /// Handles the autosave option from the file-menu.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event contents</param>
        private void AutoSave_Clicked(object sender, RoutedEventArgs e)
        {
            if (AutoSave.IsChecked)
            {
                running = true;
                AutoSaveStart();
            }
            else if (!AutoSave.IsChecked)
            {
                running = false;
            }
        }

        /// <summary>
        /// Starts the autosave feature and keeps it running as long as the checkbox is checked
        /// in the file-menu. Saves a file every 1 minute.
        /// </summary>
        private void AutoSaveStart()
        {
            
            Thread thread = new Thread(delegate()
            {

                while (running)
                {
                    SaveFile();
                    Thread.Sleep(60000);
                }
            });

            thread.Start();
        }

        /// <summary>
        /// Loads the xml-file from disk with preferences.
        /// </summary>
        /// <returns></returns>
        public string loadXML()
        {
            string result = "";

            if (File.Exists(@"C:\prefs.xml"))
            {
                
                    result = xmlhandler.DeserializeFromXML();
                
            }

            return result;
        }
    }
}
