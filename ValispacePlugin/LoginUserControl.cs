using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AGI.Ui.Plugins;
using stdole;
using AGI.STKObjects;

namespace ValispacePlugin
{
    public partial class LoginUserControl : UserControl, IAgUiPluginEmbeddedControl
    {
        private IAgUiPluginEmbeddedControlSite m_pEmbeddedControlSite;
        private UIPlugin m_uiplugin;
        private AgStkObjectRoot m_root;

        public LoginUserControl()
        {
            InitializeComponent();
            wpfLogin1.HideWindow += new EventHandler(inputBoxControl1_HideWindow);
        }

        void inputBoxControl1_HideWindow(object sender, EventArgs e)
        {
            
        }


        public void SetSite(IAgUiPluginEmbeddedControlSite Site)
        {
            m_pEmbeddedControlSite = Site;
            m_uiplugin = m_pEmbeddedControlSite.Plugin as UIPlugin;
            m_root = m_uiplugin.STKRoot;
            wpfLogin1.InitProjnFile(Site);
            
        }

        public void OnClosing()
        {
            this.Visible = false;
        }

        public void OnSaveModified()
        {
            throw new NotImplementedException();
        }

        public IPictureDisp GetIcon()
        {
            return null;
        }
    }
}
