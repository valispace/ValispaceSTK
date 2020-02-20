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

namespace ValispacePlugin
{
    /// <summary>
    /// Interaction logic for ProjWindow.xaml
    /// </summary>
    public partial class ProjWindow : Window
    {
        public static string ProjName;
        public ProjWindow()
        {
            InitializeComponent();
            //string fileName = @"C:\Temp\temp_valiProject.txt";

            try
            {
                var info = (string)MainWindow.projInfoFile.getInfo("Project Name");
                ProjectName.Text = info;
                // Check if file already exists. If yes, delete it.     
                //if (File.Exists(fileName))
                //{
                //    // Open the stream and read it back.    
                //    using (StreamReader sr = File.OpenText(fileName))
                //    {
                //        string s = "";
                //        while ((s = sr.ReadLine()) != null)
                //        {
                //            ProjectName.Text = s.ToString();
                //        }
                //    }
                //}
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

        private void Click_LoadProj(object sender, RoutedEventArgs e)
        {
            ProjName = ProjectName.Text;
            this.Hide();
            //MainWindow.ValiProjectName = ProjName;
            var isLoaded = MainWindow.updateTreeNow(FLAG:ProjectFLAG.NEW);
            
            if (isLoaded)
            {
                MessageBox.Show("Project Loaded");
                MainWindow.thisProjMsg.Visibility = Visibility.Hidden;
                MainWindow.projInfoFile.writeInfo("Project Name", ProjName);
                //string fileName = @"C:\Temp\temp_valiProject.txt";
                //// Check if file already exists. If yes, delete it.     
                //if (File.Exists(fileName))
                //{
                //    File.Delete(fileName);
                //}

                //// Create a new file     
                //using (FileStream fs = File.Create(fileName))
                //{

                //    Byte[] title = new UTF8Encoding(true).GetBytes(ProjName);
                //    fs.Write(title, 0, title.Length);
                //}
            }
        }
    }
}
