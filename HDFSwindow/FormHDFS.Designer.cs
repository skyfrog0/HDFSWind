namespace HDFSwindow
{
    partial class FormHDFS
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHDFS));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("/");
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnUpload = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnDownload = new System.Windows.Forms.ToolStripButton();
            this.btnOpenDownloadDir = new System.Windows.Forms.ToolStripButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMkdir = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelDir = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelDirLoading = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panelFileTrans = new System.Windows.Forms.Panel();
            this.labelLoading = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panelFileLoading = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReplication = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelDirLoading.SuspendLayout();
            this.panelFileTrans.SuspendLayout();
            this.panelFileLoading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripBtnUpload,
            this.toolStripBtnDownload,
            this.btnOpenDownloadDir});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(874, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(95, 22);
            this.toolStripButton1.Text = "HDFS服务器";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripBtnUpload
            // 
            this.toolStripBtnUpload.Image = global::HDFSwindow.Properties.Resources.upload_16px_1225436_easyicon_net;
            this.toolStripBtnUpload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnUpload.Name = "toolStripBtnUpload";
            this.toolStripBtnUpload.Size = new System.Drawing.Size(76, 22);
            this.toolStripBtnUpload.Text = "上传文件";
            this.toolStripBtnUpload.Click += new System.EventHandler(this.toolStripBtnUpload_Click);
            // 
            // toolStripBtnDownload
            // 
            this.toolStripBtnDownload.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnDownload.Image")));
            this.toolStripBtnDownload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnDownload.Name = "toolStripBtnDownload";
            this.toolStripBtnDownload.Size = new System.Drawing.Size(76, 22);
            this.toolStripBtnDownload.Text = "下载文件";
            this.toolStripBtnDownload.Click += new System.EventHandler(this.toolStripBtnDownload_Click);
            // 
            // btnOpenDownloadDir
            // 
            this.btnOpenDownloadDir.Image = global::HDFSwindow.Properties.Resources.folder_17_406593406593px_1201766_easyicon_net;
            this.btnOpenDownloadDir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenDownloadDir.Name = "btnOpenDownloadDir";
            this.btnOpenDownloadDir.Size = new System.Drawing.Size(100, 22);
            this.btnOpenDownloadDir.Text = "打开下载目录";
            this.btnOpenDownloadDir.Click += new System.EventHandler(this.btnOpenDownloadDir_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder_16px_1229441_easyicon.net.png");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDownload,
            this.menuUpload,
            this.menuMkdir,
            this.menuDelete});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 92);
            // 
            // menuDownload
            // 
            this.menuDownload.Name = "menuDownload";
            this.menuDownload.Size = new System.Drawing.Size(100, 22);
            this.menuDownload.Text = "下载";
            this.menuDownload.Click += new System.EventHandler(this.menuDownload_Click);
            // 
            // menuUpload
            // 
            this.menuUpload.Name = "menuUpload";
            this.menuUpload.Size = new System.Drawing.Size(100, 22);
            this.menuUpload.Text = "上传";
            this.menuUpload.Click += new System.EventHandler(this.menuUpload_Click);
            // 
            // menuMkdir
            // 
            this.menuMkdir.Name = "menuMkdir";
            this.menuMkdir.Size = new System.Drawing.Size(100, 22);
            this.menuMkdir.Text = "新建";
            this.menuMkdir.Click += new System.EventHandler(this.menuMkdir_Click);
            // 
            // menuDelete
            // 
            this.menuDelete.Name = "menuDelete";
            this.menuDelete.Size = new System.Drawing.Size(100, 22);
            this.menuDelete.Text = "删除";
            this.menuDelete.Click += new System.EventHandler(this.menuDelete_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabelDir});
            this.statusStrip1.Location = new System.Drawing.Point(0, 547);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(874, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabel1.Text = "当前路径：";
            // 
            // toolStripStatusLabelDir
            // 
            this.toolStripStatusLabelDir.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabelDir.Name = "toolStripStatusLabelDir";
            this.toolStripStatusLabelDir.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripStatusLabelDir.Size = new System.Drawing.Size(72, 17);
            this.toolStripStatusLabelDir.Text = "/";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelDirLoading);
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelFileTrans);
            this.splitContainer1.Panel2.Controls.Add(this.panelFileLoading);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(874, 522);
            this.splitContainer1.SplitterDistance = 235;
            this.splitContainer1.TabIndex = 3;
            // 
            // panelDirLoading
            // 
            this.panelDirLoading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDirLoading.BackColor = System.Drawing.Color.White;
            this.panelDirLoading.Controls.Add(this.label2);
            this.panelDirLoading.Controls.Add(this.label1);
            this.panelDirLoading.Location = new System.Drawing.Point(57, 473);
            this.panelDirLoading.Name = "panelDirLoading";
            this.panelDirLoading.Size = new System.Drawing.Size(117, 36);
            this.panelDirLoading.TabIndex = 1;
            this.panelDirLoading.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(39, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "loading ..";
            // 
            // label1
            // 
            this.label1.Image = global::HDFSwindow.Properties.Resources._5_121204194112;
            this.label1.Location = new System.Drawing.Point(1, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 32);
            this.label1.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "/";
            treeNode1.Text = "/";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(235, 522);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterExpand);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // panelFileTrans
            // 
            this.panelFileTrans.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelFileTrans.BackColor = System.Drawing.Color.White;
            this.panelFileTrans.Controls.Add(this.labelLoading);
            this.panelFileTrans.Controls.Add(this.label6);
            this.panelFileTrans.Location = new System.Drawing.Point(53, 452);
            this.panelFileTrans.Name = "panelFileTrans";
            this.panelFileTrans.Size = new System.Drawing.Size(538, 54);
            this.panelFileTrans.TabIndex = 2;
            this.panelFileTrans.Visible = false;
            // 
            // labelLoading
            // 
            this.labelLoading.AutoSize = true;
            this.labelLoading.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelLoading.Location = new System.Drawing.Point(100, 19);
            this.labelLoading.Name = "labelLoading";
            this.labelLoading.Size = new System.Drawing.Size(77, 17);
            this.labelLoading.TabIndex = 1;
            this.labelLoading.Text = "uploading ..";
            // 
            // label6
            // 
            this.label6.Image = global::HDFSwindow.Properties.Resources.transmitting;
            this.label6.Location = new System.Drawing.Point(13, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 22);
            this.label6.TabIndex = 0;
            // 
            // panelFileLoading
            // 
            this.panelFileLoading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelFileLoading.BackColor = System.Drawing.Color.White;
            this.panelFileLoading.Controls.Add(this.label3);
            this.panelFileLoading.Controls.Add(this.label4);
            this.panelFileLoading.Location = new System.Drawing.Point(250, 391);
            this.panelFileLoading.Name = "panelFileLoading";
            this.panelFileLoading.Size = new System.Drawing.Size(145, 36);
            this.panelFileLoading.TabIndex = 2;
            this.panelFileLoading.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(39, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "loading docs ..";
            // 
            // label4
            // 
            this.label4.Image = global::HDFSwindow.Properties.Resources._5_121204194112;
            this.label4.Location = new System.Drawing.Point(1, 1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 32);
            this.label4.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colID,
            this.colName,
            this.colTime,
            this.colReplication,
            this.colSize});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 20;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(635, 522);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDown);
            this.dataGridView1.DoubleClick += new System.EventHandler(this.dataGridView1_DoubleClick);
            // 
            // colID
            // 
            this.colID.FillWeight = 10F;
            this.colID.HeaderText = "序号";
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            // 
            // colName
            // 
            this.colName.FillWeight = 30F;
            this.colName.HeaderText = "名称";
            this.colName.Name = "colName";
            // 
            // colTime
            // 
            this.colTime.FillWeight = 30F;
            this.colTime.HeaderText = "修改日期";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            // 
            // colReplication
            // 
            this.colReplication.FillWeight = 15F;
            this.colReplication.HeaderText = "备份数";
            this.colReplication.Name = "colReplication";
            this.colReplication.ReadOnly = true;
            // 
            // colSize
            // 
            this.colSize.FillWeight = 15F;
            this.colSize.HeaderText = "文件大小";
            this.colSize.Name = "colSize";
            this.colSize.ReadOnly = true;
            // 
            // FormHDFS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 569);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormHDFS";
            this.Text = "HDFS Window";
            this.Load += new System.EventHandler(this.FormHDFS_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormHDFS_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelDirLoading.ResumeLayout(false);
            this.panelDirLoading.PerformLayout();
            this.panelFileTrans.ResumeLayout(false);
            this.panelFileTrans.PerformLayout();
            this.panelFileLoading.ResumeLayout(false);
            this.panelFileLoading.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuDownload;
        private System.Windows.Forms.ToolStripMenuItem menuUpload;
        private System.Windows.Forms.ToolStripButton btnOpenDownloadDir;
        private System.Windows.Forms.ToolStripMenuItem menuDelete;
        private System.Windows.Forms.ToolStripButton toolStripBtnUpload;
        private System.Windows.Forms.ToolStripMenuItem menuMkdir;
        private System.Windows.Forms.ToolStripButton toolStripBtnDownload;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDir;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panelDirLoading;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panelFileTrans;
        private System.Windows.Forms.Label labelLoading;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panelFileLoading;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReplication;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSize;
    }
}

