using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;

namespace ValispacePlugin.Installer
{
    public partial class InstallForm : Form
    {
        string m_programData32;
        string m_programData64;
        public InstallForm()
        {
            InitializeComponent();
            string assemblyName = Properties.Settings.Default.AssemblyName;
            m_programData32 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            m_programData64 = Path.Combine(m_programData32, @"AGI\STK 11 (x64)\Plugins");
            m_programData32 = Path.Combine(m_programData32, @"AGI\STK 11\Plugins");
            txtBinaryLocation.Text = Path.Combine(m_programData32, assemblyName);
            if (Directory.Exists(m_programData32))
            {
                cb32.Checked = true;
            }
            else
            {
                cb32.Enabled = false;
            }

            if (Directory.Exists(m_programData64))
            {
                cb64.Checked = true;
                txtBinaryLocation.Text = Path.Combine(m_programData64, assemblyName);
            }
            else
            {
                cb64.Enabled = false;
            }
        }

        private void WriteXMLFile(string installDirectory, string pluginDirectory)
        {
            string displayName = ValispacePlugin.Installer.Properties.Settings.Default.DisplayName;
            string typeName = ValispacePlugin.Installer.Properties.Settings.Default.TypeName;
            string assemblyName = ValispacePlugin.Installer.Properties.Settings.Default.AssemblyName;

            using (StreamWriter writer = new StreamWriter(Path.Combine(pluginDirectory, typeName + ".xml")))
            {
                writer.Write(CreateXMLFile(displayName, typeName, assemblyName, installDirectory));
                writer.Close();
            }
        }


        private string CreateXMLFile(string displayName, string typeName, string assemblyName, string installDirectory)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<?xml version = \"1.0\"?>");
            stringBuilder.AppendLine("<AGIRegistry version = \"1.0\">");
            stringBuilder.AppendLine("<CategoryRegistry>");
            stringBuilder.AppendLine("<Category Name = \"UiPlugins\">");
            stringBuilder.AppendLine("<NETUiPlugin");
            stringBuilder.AppendLine("DisplayName=\"" + displayName + "\"");
            stringBuilder.AppendLine("TypeName=\"" + typeName + "\"");
            stringBuilder.AppendLine("AssemblyName=\"" + assemblyName + "\"");
            stringBuilder.AppendLine("CodeBase=\"" + installDirectory + "\">");
            stringBuilder.AppendLine("</NETUiPlugin>");
            stringBuilder.AppendLine("</Category>");
            stringBuilder.AppendLine("</CategoryRegistry>");
            stringBuilder.AppendLine("</AGIRegistry>");
            return stringBuilder.ToString();
        }

        private void UnZIPBinnaries(string installDirectory)
        {
            string zipLocation = Path.Combine(installDirectory, "bin.zip");
            File.WriteAllBytes(zipLocation, Properties.Resources.Bin);

            Zip.UnzipFiles(zipLocation, installDirectory);
            //FileInfo zipFile = new FileInfo(zipLocation);
            //Zip.Decompress(zipFile);

        }

        private void InstallPlugin()
        {
            if ((cb32.Checked) || (cb64.Checked))
            {
                string installDirectory = txtBinaryLocation.Text;
                if (installDirectory != "")
                {
                    if (!Directory.Exists(installDirectory))
                    {
                        Directory.CreateDirectory(installDirectory);
                    }
                    UnZIPBinnaries(installDirectory);
                    if (cb32.Checked)
                    {
                        WriteXMLFile(installDirectory, m_programData32);
                    }
                    if (cb64.Checked)
                    {
                        WriteXMLFile(installDirectory, m_programData64);
                    }
                    MessageBox.Show("Done!");
                    
                }
            }
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            InstallPlugin();
        }

        private void btnBinaryLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtBinaryLocation.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
