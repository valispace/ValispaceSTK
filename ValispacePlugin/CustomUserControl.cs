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
using AGI.STKObjects;
using stdole;

namespace ValispacePlugin
{
    public partial class CustomUserControl : UserControl, IAgUiPluginEmbeddedControl
    {
        private IAgUiPluginEmbeddedControlSite m_pEmbeddedControlSite;
        private UIPlugin m_uiplugin;
        private AgStkObjectRoot m_root;


        public CustomUserControl()
        {
            InitializeComponent();
            
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (m_root.CurrentScenario == null)
            {
                MessageBox.Show("I know that no scenario is open.");
            }
            else
            {
                MessageBox.Show("I know your scenario's name is " + m_root.CurrentScenario.InstanceName);
            }
        }

        public void SetSite(IAgUiPluginEmbeddedControlSite Site)
        {
            m_pEmbeddedControlSite = Site;
            m_uiplugin = m_pEmbeddedControlSite.Plugin as UIPlugin;
            m_root = m_uiplugin.STKRoot;
            this.mainWindow1.InitProjnFile(Site);
            //this.mainWindow1.loadAll(Site);
        }

        public void OnClosing()
        {
            
        }

        public void OnSaveModified()
        {
            throw new NotImplementedException();
        }

        public IPictureDisp GetIcon()
        {
            return null;
        }

        private void ElementHost1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void elementHost1_ChildChanged_1(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }
    }
}
