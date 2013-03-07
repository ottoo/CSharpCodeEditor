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
using System.Windows.Shapes;
using System.IO;

namespace CodeEditor
{
    /// <summary>
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences : Window
    {
        // Property for filePath
        public string filePath { get; set; }
        private Prefs prefs;
        private XMLHandler xmlhandler;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Preferences(Prefs prefs)
        {
            InitializeComponent();
            this.prefs = prefs;
            xmlhandler = new XMLHandler();

            if (File.Exists(prefs.filePath))
            {
                TextBox1.Text = prefs.filePath;
            }
        }

        /// <summary>
        /// Handles a button event. 
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event contents</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        /// <summary>
        /// Opens a new openfiledialog.
        /// </summary>
        public void OpenFile()
        {            
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            
            openFile.Filter = ".exe - files (*.exe)|*.exe|All Files|*.*";
           
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                prefs.filePath = openFile.FileName;                 
                xmlhandler.SerializeToXML(prefs);
            }               
            if (File.Exists(prefs.filePath))
            {
                TextBox1.Text = prefs.filePath;
            }
        }

        /// <summary>
        /// Handles the Ok-Button event.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event contents</param>
        private void Ok_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }        
    }
}
