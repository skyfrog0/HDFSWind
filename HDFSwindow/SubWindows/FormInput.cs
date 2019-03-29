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
    public partial class FormInput : Form
    {
        public string Content { get; set; }

        public FormInput()
        {
            InitializeComponent();
        }

        private void FormInput_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.Content;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Content = this.textBox1.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

  
    }
}
