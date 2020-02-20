using AGI.Ui.Plugins;
using AGI.STKObjects;

namespace ValispacePlugin
{
    partial class CustomUserControl
    {

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.mainWindow1 = new ValispacePlugin.MainWindow();
            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(0, 3);
            this.elementHost1.Name = "elementHost1";
            //this.elementHost1.Size = new System.Drawing.Size(1271, 746);
            this.elementHost1.AutoSize = true;
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.ChildChanged += new System.EventHandler<System.Windows.Forms.Integration.ChildChangedEventArgs>(this.elementHost1_ChildChanged_1);
            this.elementHost1.Child = this.mainWindow1;
            // 
            // CustomUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.elementHost1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CustomUserControl";
            this.Size = new System.Drawing.Size(1395, 1009);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private MainWindow mainWindow1;
    }
}
