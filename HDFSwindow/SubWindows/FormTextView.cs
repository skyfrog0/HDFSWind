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
    public delegate string ReadFilePartCallback(int tailsize);

    public partial class FormTextView : Form
    {
        public event ReadFilePartCallback OnReloadContent;

        public string FileName { get; set; }
        public string Content { get; set; }
        public int TailSize { get; set; }

        public FormTextView()
        {
            InitializeComponent();
        }

        private void FormTextView_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = Content.Replace("\n", "\r\n");
            this.textBoxTailSize.Text = this.TailSize.ToString();
            this.Text = "文本浏览 - [" + FileName + "]";
        }

        private void FormTextView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Escape == e.KeyCode)
            {
                this.Close();
            }
        }

        private void textBoxTailSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Enter == e.KeyCode)
            {
                string newsize = this.textBoxTailSize.Text;
                int n = TailSize;
                int.TryParse(newsize, out n);
                if (n > 0 && n != TailSize)
                {
                    //Reload
                    if (null != this.OnReloadContent)
                    {
                        this.Content = this.OnReloadContent(n);
                        this.textBox1.Text = this.Content.Replace("\n", "\r\n");
                    }
                }
            }
        }

  
    }
}
