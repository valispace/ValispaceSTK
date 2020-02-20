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
using Newtonsoft.Json;

namespace ValispacePlugin
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
   
    public partial class Login : Window
    {
        public static ValispaceAPI valispace;
        private bindingsFile projInfoFile;
        public bool loggedIn;
        private string username;
        private string password;
        private string url;
        
        public Login(bindingsFile bindingFile)
        {
            InitializeComponent();
            projInfoFile = bindingFile;
            
            if (projInfoFile.Exists())
            {
                projInfoFile.readInfofile();

                var dict = (Dictionary<string,object>) projInfoFile.getInfo("Cred") ;

                url = (string)dict["URL"];
                username = (string)dict["User"];
                password = (string)dict["Password"];
                //string fileName = @"C:\Temp\temp_valicred.txt";
                //if (File.Exists(fileName))
                //{
                //    // Open the stream and read it back.    
                //    using (StreamReader sr = File.OpenText(fileName))
                //    {
                //        string s = "";
                //        while ((s = sr.ReadLine()) != null)
                //        {
                //            string[] lines = s.Split(':');
                //            url = lines[0];
                //            username = lines[1];
                //            password = lines[2];
                //        }
                //    }
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
            this.Show();
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
            if (url0 == "" || user == "" ||passwordBox.Password == "")
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
                LoginInfo.Add("URL",url0);
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
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid Credentials", "Warning");
                this.Show();
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
    }
}
