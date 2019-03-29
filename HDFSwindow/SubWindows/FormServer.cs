using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HDFSwindow.SubWindows
{
    public partial class FormServer : Form
    {
        public String Address { get; set; }


        public FormServer()
        {
            InitializeComponent();
        }
        
        private void FormServer_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.Address))
                this.textBoxAddres.Text = this.Address;
        }

        // OK
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Address = this.textBoxAddres.Text.Trim();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void FormServer_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Enter == e.KeyCode)
            {
                btnOk_Click(null, null);
            }
        }

       
    }
}
