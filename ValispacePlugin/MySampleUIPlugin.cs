using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGI.Ui.Application;
using AGI.Ui.Core;
using AGI.Ui.Plugins;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.Compatibility;
using System.Drawing;
using System.Reflection;
using AGI.STKObjects;

namespace ValispacePlugin
{
    [Guid("338772FA-F62E-4AE6-9224-624C5B2BA833")]
    [ProgId("Valispace.MySampleUIPlugin")]
    [ClassInterface(ClassInterfaceType.None)]

    public class MySampleUIPlugin: IAgUiPlugin, IAgUiPluginCommandTarget
    {
        private IAgUiPluginSite m_psite;
        private AgStkObjectRoot m_root;
        

        public void OnStartup(IAgUiPluginSite PluginSite)
        {
            m_psite = PluginSite;
            IAgUiApplication AgUiApp = m_psite.Application;
            m_root = AgUiApp.Personality2 as AgStkObjectRoot;
            
        }


        public AgStkObjectRoot STKRoot
        {
            get { return m_root; }
        }

        public void OnShutdown()
        {
            m_psite = null;
        }

        public void OnDisplayConfigurationPage(IAgUiPluginConfigurationPageBuilder ConfigPageBuilder)
        {
            
        }

        //public void OnDisplayContextMenu(IAgUiPluginMenuBuilder MenuBuilder)
        //{
        //    MenuBuilder.AddMenuItem("MyCompany.MySampleUIPlugin.MySecondCommand", "Valispace", " Display a message box.", null);
        //}

        public void OnInitializeToolbar(IAgUiPluginToolbarBuilder ToolbarBuilder)
        {
            ToolbarBuilder.AddButton("Valispace.MySampleUIPlugin.LoginCommand", "Valispace Login  |", "Valispace Plugin", AgEToolBarButtonOptions.eToolBarButtonOptionAlwaysOn, null);
            ToolbarBuilder.AddButton("Valispace.MySampleUIPlugin.OpenUICommand", "Ui Plugin ", "Valispace Plugin", AgEToolBarButtonOptions.eToolBarButtonOptionAlwaysOn, null);
        }


        public void Exec(string CommandName, IAgProgressTrackCancel TrackCancel, IAgUiPluginCommandParameters Parameters)
        {
            if (string.Compare(CommandName, "Valispace.MySampleUIPlugin.OpenUICommand", true) == 0)
            {
                if (WPFLogin.valispace != null)
                { OpenUserInterface(); }
                else
                { MessageBox.Show("Please Login to Valispace first using the 'Valispace Login' button"); }
                
            }
            else if (string.Compare(CommandName, "Valispace.MySampleUIPlugin.LoginCommand", true) == 0)
            {
                 OpenLoginUI(); 
            }
        }

        private void OpenLoginUI()
        {
            IAgUiPluginWindowSite windows = m_psite as IAgUiPluginWindowSite;

            if (windows == null)
            {
                MessageBox.Show("Host application is unable to open windows.");
            }
            else
            {
                IAgUiPluginWindowCreateParameters winParams = windows.CreateParameters();

                winParams.AllowMultiple = false;
                winParams.AssemblyPath = this.GetType().Assembly.Location;
                winParams.UserControlFullName = typeof(LoginUserControl).FullName;
                winParams.Caption = "Valispace Integration Plugin";
                winParams.DockStyle = AgEDockStyle.eDockStyleFloating;
                winParams.Height = 360;
                winParams.Width = 360;
                object obj = windows.CreateNetToolWindowParam(this, winParams);
               
            }
        }

        public AgEUiPluginCommandState QueryState(string CommandName)
        {
            if (string.Compare(CommandName, "Valispace.MySampleUIPlugin.LoginCommand", true) == 0)
            {
                return AgEUiPluginCommandState.eUiPluginCommandStateEnabled | AgEUiPluginCommandState.eUiPluginCommandStateSupported;
            }
            else if (string.Compare(CommandName, "Valispace.MySampleUIPlugin.OpenUICommand", true) == 0)
            {
                return AgEUiPluginCommandState.eUiPluginCommandStateEnabled | AgEUiPluginCommandState.eUiPluginCommandStateSupported;
            }
            return AgEUiPluginCommandState.eUiPluginCommandStateNone;
        }

        public void OpenUserInterface()
        {
            IAgUiPluginWindowSite windows = m_psite as IAgUiPluginWindowSite;
            // 1536x864 //1920x1080
            Console.WriteLine(System.Windows.SystemParameters.PrimaryScreenWidth.ToString() + "," + System.Windows.SystemParameters.PrimaryScreenHeight.ToString());
            if (windows == null)
            {
                MessageBox.Show("Host application is unable to open windows.");
            }
            else
            {
                IAgUiPluginWindowCreateParameters winParams = windows.CreateParameters();

                winParams.AllowMultiple = false;
                winParams.AssemblyPath = this.GetType().Assembly.Location;
                winParams.UserControlFullName = typeof(CustomUserControl).FullName;
                winParams.Caption = "Valispace Integration Plugin";
                winParams.DockStyle = AgEDockStyle.eDockStyleFloating;
                winParams.Height = 672;
                winParams.Width = 1113;
                object obj = windows.CreateNetToolWindowParam(this, winParams);
            }
        }

        public void OnDisplayContextMenu(IAgUiPluginMenuBuilder MenuBuilder)
        {
            throw new NotImplementedException();
        }
    }
}

