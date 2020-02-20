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
using System.Security.Cryptography;
using System.Reflection;
using Valispace;
using System.IO;
using AGI.Ui.Application;
using AGI.Ui.Plugins;
using AGI.STKObjects;
using AGI.STKUtil;
using Newtonsoft.Json;
using stdole;

namespace ValispacePlugin
{
    /// <summary>
    /// Interaction logic for WPFLogin.xaml
    /// </summary>
    public partial class WPFLogin : UserControl
    {
        
        public static ValispaceAPI valispace;
        private bindingsFile projInfoFile;
        public bool loggedIn;
        private string username;
        private string password;
        private string url;

        private IAgUiPluginEmbeddedControlSite m_pEmbeddedControlSite;
        private MySampleUIPlugin m_uiplugin;
        private AgStkObjectRoot m_root;
        public event EventHandler HideWindow;

        public WPFLogin()
        {
            InitializeComponent();
            
        }
        public void InitProjnFile(IAgUiPluginEmbeddedControlSite Site)
        {
            SetSite(Site);
            try
            {

                IAgStkObject Scenario = m_root.CurrentScenario;
                IAgExecCmdResult result = m_root.ExecuteCommand("GetDirectory / Scenario");
                string m_scenarioPath = string.Empty;
                if (result.IsSucceeded)
                {
                    m_scenarioPath = result[0];
                }
                // ADD SCENARIO PATH TO BINDING FILE CLASS
                projInfoFile = new bindingsFile(m_scenarioPath);
            }
            catch
            {
                MessageBox.Show("STK Instance does not exist or is lost! \n Make sure STK is open and the scenario is loaded ");
                //Environment.Exit(0);
            }

            try
            {
                
                projInfoFile.readInfofile();
                m_root.UnitPreferences.ResetUnits();
                WPFLoginLoadSavedInfo(projInfoFile);
            }

            catch
            { }
        }
        public void WPFLoginLoadSavedInfo(bindingsFile bindingFile)
        {
            
            projInfoFile = bindingFile;

            if (projInfoFile.Exists())
            {
                projInfoFile.readInfofile();

                var dict = (Dictionary<string, object>)projInfoFile.getInfo("Cred");

                url = (string)dict["URL"];
                username = (string)dict["User"];
                password = (string)dict["Password"];
                url_textbox.Text = url;
                username_textbox.Text = username;
                try
                {
                    passwordBox.Password = Unprotect(password);
                }
                catch { }
                //Unprotect(password);
            }
            //}
            else
            {
                projInfoFile.Create();
            }
            
        }


        public static string Protect(string str)
        {
            byte[] entropy = Encoding.ASCII.GetBytes(Assembly.GetExecutingAssembly().FullName);
            byte[] data = Encoding.ASCII.GetBytes(str);
            string protectedData = Convert.ToBase64String(ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser));
            return protectedData;
        }
        public static string Unprotect(string str)
        {
            byte[] protectedData = Convert.FromBase64String(str);
            byte[] entropy = Encoding.ASCII.GetBytes(Assembly.GetExecutingAssembly().FullName);
            string data = Encoding.ASCII.GetString(ProtectedData.Unprotect(protectedData, entropy, DataProtectionScope.CurrentUser));
            return data;
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var url0 = url_textbox.Text;
            var user = username_textbox.Text;
            if (url0 == "" || user == "" || passwordBox.Password == "")
            {
                MessageBox.Show("A Login field cannot be empty. Please Retry!");
                return;
            }
            var url1 = baseUrl(url0);
            var passHash = Protect(passwordBox.Password);



            valispace = ValispaceAPI.Connect(url1, user, Unprotect(passHash));
            if (valispace.GetAuthenticationResult() != null)
            {
                loggedIn = true;
                Dictionary<string, object> LoginInfo = new Dictionary<string, object>();
                LoginInfo.Add("URL", url0);
                LoginInfo.Add("User", user);
                LoginInfo.Add("Password", passHash);

                projInfoFile.writeInfo("Cred", LoginInfo);
                //string fileName = @"C:\Temp\temp_valicred.txt";
                //// Check if file already exists. If yes, delete it.     
                //if (File.Exists(fileName))
                //{
                //    File.Delete(fileName);
                //}

                //// Create a new file     
                //using (FileStream fs = File.Create(fileName))
                //{
                //    Byte[] title = new UTF8Encoding(true).GetBytes(url0 + ":" + user + ":" + passHash);
                //    fs.Write(title, 0, title.Length);


                /// CHANGE ALL THIS WHEN DOING WINDOW INTERACTION
                //this.Hide();
                //m_parent.Show();

                //}
                EventArgs eventArgs = new EventArgs();
                if (HideWindow != null)
                {
                    HideWindow(this, eventArgs);
                }

                StatusTextBox.Background = Brushes.Green;
                StatusTextBox.Text = " Logged In ";
                passwordBox.IsEnabled = false;
                username_textbox.IsEnabled = false;
                url_textbox.IsEnabled = false;
                
            }
            else
            {
                MessageBox.Show("Invalid Credentials", "Warning");
                
            }


        }

        private void login_Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //System.Windows.Application.Current.Shutdown();
            Environment.Exit(0);
        }

        public static string baseUrl(string domainValue)
        {

            if (!string.IsNullOrWhiteSpace(domainValue) && !domainValue.StartsWith("http"))
                return "https://" + domainValue;

            return domainValue;

        }

        public void SetSite(IAgUiPluginEmbeddedControlSite Site)
        {
            m_pEmbeddedControlSite = Site;
            m_uiplugin = m_pEmbeddedControlSite.Plugin as MySampleUIPlugin;
            m_root = m_uiplugin.STKRoot;
        }

        //public IPictureDisp GetIcon()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
