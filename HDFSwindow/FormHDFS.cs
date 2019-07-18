using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HDFSwindow.HDFSCore;
using HDFSwindow.SubWindows;

namespace HDFSwindow
{
    public partial class FormHDFS : Form
    {
        public delegate void LoadDirsCallback(List<HdfsFileInfo> files, TreeNode parentNode);
        public delegate void LoadDocsCallback(List<HdfsFileInfo> files);

        #region 字段

        private string _server = "192.98.19.11";
        private CancellationTokenSource _taskCancelTS = new CancellationTokenSource();
        private int _fileViewKB = 100;    //文件浏览时，显示尾部的KB字节数
        private int _fileNameColIndex = 1;
        private bool _popInGrid = false;
        private long _fileTransBlockSize = 64 * 1024 * 1024;    //文件传输分块大小

        private ConcurrentDictionary<String, List<HdfsFileInfo>> _fileinfoCache = new ConcurrentDictionary<string, List<HdfsFileInfo>>();
        private AtomicInt _dirLoadingLatch = new AtomicInt();

        #endregion

        public FormHDFS()
        {
            InitializeComponent();
        }

        // 窗体开启
        private void FormHDFS_Load(object sender, EventArgs e)
        {
            //ConnectWebHDFS();
        }

        private void ConnectWebHDFS()
        {
            DebugHelper.OutLog("Hello HDFS Window");
            
            // 加载顶级目录
            Task.Run(() =>
            {
                ShowDirLoading(true);
                try
                {
                    HdfsHelper.SetHDFSNameNode(_server);

                    List<HdfsFileInfo> files = LoadFileInfos("/");
                    List<HdfsFileInfo> dirs = files.Where((fi) => fi.IsDirectory).ToList();
                    List<HdfsFileInfo> docs = files.Where((fi) => !fi.IsDirectory).ToList();
                    /*files.ForEach(fi =>
                    {
                        DebugHelper.OutLog(fi.ToString());
                    });*/
                    LoadDirs(dirs);
                    LoadDocs(docs);
                }
                catch (System.Exception ex)
                {
                    DebugHelper.OutLog(ex.Message);
                }
                ShowDirLoading(false);
            });
        }

        // 查询文件属性
        public List<HdfsFileInfo> LoadFileInfos(string dfsPath)
        {
            if (string.IsNullOrWhiteSpace(dfsPath))
                dfsPath = "/";
            List<HdfsFileInfo> files = null;

            if (_fileinfoCache.ContainsKey(dfsPath))
                _fileinfoCache.TryGetValue(dfsPath, out files);
            else
            {
                files = HdfsHelper.LsDir(ConstructDfsFullPath(dfsPath));
                if (null != files && files.Count > 0)
                {
                    _fileinfoCache.TryAdd(dfsPath, files);
                }
            }

            return files;
        }

        // 读取文件最后一页
        public string ReadFileTail(string filepath, long pageSize)
        {
            try
            {
                string hdfsPath = ConstructDfsFullPath(filepath);
                HdfsFileInfo fi = HdfsHelper.GetStatus(hdfsPath);
                long offset = 0;
                long size = fi.Size;
                if (size > pageSize)
                {
                    offset = size - pageSize;
                    size = pageSize;
                }
                var result = HdfsHelper.ReadFilePart(hdfsPath, offset, size);
                if (-1 != result.Code)
                    return Encoding.UTF8.GetString(result.Content);
                else
                    return result.ErrorLog;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 文件上传，多线程并行
        /// </summary>
        /// <param name="localFile">本地文件名（绝对路径）</param>
        /// <param name="hdfsDir">目标HDFS目录</param>
        public void UploadFileMT(string localFile, string hdfsDir)
        {
            FileInfo fi = new FileInfo(localFile);
            string fileName = fi.Name;
            if (fi.Length < _fileTransBlockSize)
            {
                UploadFile(localFile, hdfsDir);
                return;
            }

            string msg = string.Format("Uploading {0} to {1} ", localFile, hdfsDir);
            ShowFileTransmission(true, msg);
            Stopwatch allwt = Stopwatch.StartNew();
            // 分块
            int n = (int)(fi.Length / _fileTransBlockSize);
            if (fi.Length % _fileTransBlockSize > 0) n++;
            AtomicInt clatch = new AtomicInt();
            int parallelNum = 8;
            SemaphoreSlim sema = new SemaphoreSlim(parallelNum, parallelNum);
            Task[] subtasks = new Task[n];
            for (int i = 0; i < n; i++)
            {
                int pid = i;
                subtasks[i] = Task.Run(() =>
                {
                    sema.Wait();
                    clatch.Increment();
                    string partName = string.Format("{0}_p_{1:D5}", fileName, pid);
                    ShowFileTransmission(true, "正在传输\"" + partName + "\"");
                    Stopwatch wt = Stopwatch.StartNew();
                    bool bok = false;
                    using (FileStream fs = new FileStream(localFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        long offset = pid * _fileTransBlockSize;
                        bok = HdfsHelper.UploadStream(fs, offset, _fileTransBlockSize, hdfsDir, partName);
                        fs.Close();
                    }
                    wt.Stop();
                    msg = string.Format("上传\"{0}\" part-{1} {2}，耗时 {3}",
                        localFile, pid, bok ? "成功" : "失败", wt.Elapsed.ToString());
                    DebugHelper.OutLog(msg);
                    ShowFileTransmission(true, msg);
                    clatch.Decrement();
                    sema.Release();
                    ReloadDocsInDir(hdfsDir);
                });
            }
            //Task.WaitAll(subtasks);
            Task.Run(() =>
            {
                while (!clatch.IsEmpty)
                    Thread.Sleep(500);
                allwt.Stop();
                string msgtotal = string.Format("文件\"{0}\"已上传完成，共{1}个分区, 耗时：{2}",
                    localFile, n, allwt.Elapsed.ToString());
                ShowFileTransmission(true, msgtotal);
                DebugHelper.OutLog(msgtotal);

                // 拼接文件名
                string wholeFile = hdfsDir + (hdfsDir.EndsWith("/") ? "" : "/") + fileName;
                string[] partFiles = new string[n];
                for (int i = 0; i < n; i++)
                {
                    partFiles[i] = string.Format("{0}_p_{1:D5}", wholeFile, i);
                }
                HdfsHelper.NewFile(wholeFile);
                HdfsHelper.CombineFiles(wholeFile, partFiles);
                ShowFileTransmission(true, "文件已合并完整");
                ReloadDocsInDir(hdfsDir);
                Thread.Sleep(500);
                ShowFileTransmission(false, "");
            });
        }

        // 上传文件
        public void UploadFile(string localFile, string hdfsDir)
        {
            string hdfsPath = ConstructDfsFullPath(hdfsDir);
            string msg = string.Format("Uploading {0} to {1} ", localFile, hdfsPath);
            DebugHelper.OutLog(msg);
            string errlog = "";

            ShowFileTransmission(true, msg);
            Task.Run(() =>
            {
                Stopwatch wt = Stopwatch.StartNew();
                bool bok = HdfsHelper.UploadFile(localFile, hdfsPath, out errlog);
                msg = bok ? "上传成功 " : "上传失败 " + localFile;
                DebugHelper.OutLog(string.Format("上传\"{0}\"{1}，耗时 {2}",
                    localFile, bok ? "成功" : "失败", wt.Elapsed.ToString()));
                ShowFileTransmission(true, msg);
                ReloadDocsInDir(hdfsDir);
                Thread.Sleep(500);
                ShowFileTransmission(false, "");
            });
        }

        // 下载文件
        public void DownloadFile(string hdfsFile, string localFilename)
        {
            string hdfsPath = ConstructDfsFullPath(hdfsFile);
            string msg = string.Format("Downloading {0} to {1} ", hdfsPath, localFilename);
            DebugHelper.OutLog(msg);
            string errlog = "";

            HdfsFileInfo fi = HdfsHelper.GetStatus(hdfsPath);  //TODO: 大文件多线程下载
            ShowFileTransmission(true, msg);
            Task.Run(() =>
            {
                Stopwatch wt = Stopwatch.StartNew();
                byte[] bytes = HdfsHelper.GetFile(hdfsPath);
                bool isOk = bytes.Length == fi.Size;
                if (isOk)
                {
                    using (FileStream fs = new FileStream(localFilename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();
                    }
                }
                wt.Stop();
                msg = isOk ? "下载成功 " : "下载失败 " + hdfsPath;
                DebugHelper.OutLog(string.Format("下载\"{0}\"{1}，耗时 {2}",
                    hdfsPath, isOk ? "成功" : "失败", wt.Elapsed.ToString()));
                ShowFileTransmission(true, msg);
                Thread.Sleep(1000);
                ShowFileTransmission(false, "");
            });

        }

        // 线程安全的目录挂载
        private void LoadDirs(List<HdfsFileInfo> dirs, TreeNode parent = null)
        {
            if (this.InvokeRequired)
                this.Invoke(new LoadDirsCallback(LoadDirs), dirs, parent);
            else
            {
                AddTreeNodes(dirs.ConvertAll<string>(dir => dir.Name).ToList(), parent);
            }
        }

        // 线程安全的文件列表展示
        private void LoadDocs(List<HdfsFileInfo> docs)
        {
            if (this.InvokeRequired)
                this.Invoke(new LoadDocsCallback(LoadDocs), docs);
            else
            {
                if (this.dataGridView1.Rows.Count > 0)
                    ClearGrid();
                AddToGrid(docs);
            }
        }

        // 刷新目录信息
        private void ReloadDocsInDir(string hdfsDir)
        {
            // 清理缓存
            List<HdfsFileInfo> temp = null;
            _fileinfoCache.TryRemove(hdfsDir, out temp);
            _taskCancelTS.Cancel();
            _taskCancelTS = new CancellationTokenSource();
            ShowFileLoading(true);
            Task.Run(() =>
            {
                List<HdfsFileInfo> files = LoadFileInfos(hdfsDir);
                List<HdfsFileInfo> docs = files.Where((fi) => !fi.IsDirectory).ToList();
                LoadDocs(docs);
                ShowFileLoading(false);
            }, _taskCancelTS.Token);
        }

        #region 目录与文件展示

        // 目录树 添加
        private void AddTreeNodes(List<String> names, TreeNode parent = null)
        {
            if (null == parent)
                parent = this.treeView1.TopNode;

            if (null != parent)
            {
                parent.Nodes.Clear();

                string parentName = parent.Name.Length > 1 ? parent.Name : "";
                names.ForEach(str =>
                {
                    TreeNode nod = parent.Nodes.Add(str, str, 0);
                    nod.Name = parentName + "/" + str;
                    AddFakeChildNode(nod);
                });
            }

        }

        private void AddFakeChildNode(TreeNode treeNode)
        {
            if (null == treeNode)
                return;
            treeNode.Nodes.Add("...", "...", 0);
        }

        // 目录树 节点展开时，加载孙子节点
        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeNode currNode = e.Node;

            ShowDirLoading(true);
            /*foreach (TreeNode nod in currNode.Nodes)
            {
                string subdir = nod.Name;
                Task.Run(() =>
                {
                    _dirLoadingLatch.Increment();
                    List<HdfsFileInfo> files = LoadFileInfos(subdir);
                    List<HdfsFileInfo> subsubDirs = files.Where((fi) => fi.IsDirectory).ToList();
                    LoadDirs(subsubDirs, nod);
                    _dirLoadingLatch.Decrement();
                });
            }*/
            string fsDir = currNode.Name;
            Task.Run(() =>
            {
                _dirLoadingLatch.Increment();
                List<HdfsFileInfo> files = LoadFileInfos(fsDir);
                List<HdfsFileInfo> subDirs = files.Where((fi) => fi.IsDirectory).ToList();
                LoadDirs(subDirs, currNode);
                _dirLoadingLatch.Decrement();
            });

            // loading progress
            Task.Run(() =>
            {
                while (!_dirLoadingLatch.IsEmpty)
                {
                    ShowDirLoading(true);
                    Thread.Sleep(100);
                }
                ShowDirLoading(false);
            });
        }

        // 目录树 节点选择时，加载文件列表
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string dirname = e.Node.Name;
            _taskCancelTS.Cancel();
            _taskCancelTS = new CancellationTokenSource();
            this.toolStripStatusLabelDir.Text = dirname;
            //ShowFileLoading(true);
            Task.Run(() =>
            {
                List<HdfsFileInfo> files = LoadFileInfos(dirname);
                List<HdfsFileInfo> docs = files.Where((fi) => !fi.IsDirectory).ToList();
                LoadDocs(docs);
                //ShowFileLoading(false);
            }, _taskCancelTS.Token);
        }

        private void ClearTree()
        {
            this.treeView1.Nodes.Clear();
            var root = this.treeView1.Nodes.Add("/", "/", 0);
            root.Name = "/";
        }

        // 文件列表 添加
        private void AddToGrid(List<HdfsFileInfo> docs)
        {
            if (null == docs || docs.Count == 0)
                return;
            int i = 0;
            docs.ForEach(doc => this.dataGridView1.Rows.Add(i++, doc.Name, doc.ModificationTime, doc.Replication, doc.Size.ToString("N0")));
            this.dataGridView1.Refresh();
        }

        // 文件列表 清空
        private void ClearGrid()
        {
            this.dataGridView1.Rows.Clear();
            this.dataGridView1.Refresh();
        }

        // 文件列表 双击打开文件
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            var row = this.dataGridView1.CurrentRow;
            string fileName = row.Cells[_fileNameColIndex].Value.ToString();
            string path = this.treeView1.SelectedNode.Name;
            string fullName = path + "/" + fileName;
            int pageSize = _fileViewKB * 1024;

            FormTextView fm = new FormTextView();
            fm.FileName = fullName;
            fm.Content = ReadFileTail(fullName, pageSize);
            fm.TailSize = _fileViewKB;
            fm.OnReloadContent += (tailsize) => { return ReadFileTail(fullName, tailsize * 1024); };
            fm.Show();
        }

        #endregion

        #region 右键菜单

        // 目录树 PopupMenu 右键菜单
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (System.Windows.Forms.MouseButtons.Right == e.Button)
            {
                _popInGrid = false;
                if (!e.Node.IsSelected)
                    treeView1.SelectedNode = e.Node;

                var loc = treeView1.PointToScreen(e.Location);
                menuDownload.Visible = false;
                menuUpload.Visible = true;
                menuMkdir.Visible = true;
                contextMenuStrip1.Show(loc);
            }
        }

        // 文件列表 PopupMenu 右键菜单
        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (System.Windows.Forms.MouseButtons.Right == e.Button &&
                e.RowIndex >= 0)
            {
                _popInGrid = true;
                //若行已是选中状态就不再进行设置
                if (!dataGridView1.Rows[e.RowIndex].Selected)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[e.RowIndex].Selected = true;
                }
                //只选中一行时设置活动单元格
                if (dataGridView1.SelectedRows.Count == 1)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                }
                //弹出操作菜单
                menuDownload.Visible = true;
                menuUpload.Visible = false;
                menuMkdir.Visible = false;
                contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
            }
        }

        // 上传文件
        private void menuUpload_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    string localFile = dlg.FileName;
                    TreeNode currNod = this.treeView1.SelectedNode;
                    string hdfsDir = currNod.Name;
                    UploadFileMT(localFile, hdfsDir);
                }
            }
            catch (System.Exception ex)
            {
                DebugHelper.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        // 下载文件
        private void menuDownload_Click(object sender, EventArgs e)
        {
            try
            {
                string destDir = GetDownloadDir();
                var row = this.dataGridView1.CurrentRow;
                string fileName = row.Cells[_fileNameColIndex].Value.ToString();
                string dfspath = this.treeView1.SelectedNode.Name;
                string hdfsFile = dfspath + "/" + fileName;
                DownloadFile(hdfsFile, Path.Combine(destDir, fileName).ToString());
            }
            catch (System.Exception ex)
            {
                DebugHelper.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        // 删除文件或目录
        private void menuDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_popInGrid)     // 删除文件
                {
                    if (this.dataGridView1.SelectedRows.Count > 1)
                    {
                        int n = this.dataGridView1.SelectedRows.Count;
                        if (DialogResult.OK == MessageBox.Show("确定要删除选中的 " + n + " 个文件 ？",
                            "确认提示", MessageBoxButtons.OKCancel))
                        {
                            string dfspath = this.treeView1.SelectedNode.Name;
                            foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
                            {
                                string fileName = row.Cells[_fileNameColIndex].Value.ToString();
                                string hdfsFile = dfspath + "/" + fileName;
                                bool bok = HdfsHelper.Rm(hdfsFile);
                            }    
                            ReloadDocsInDir(dfspath);
                        }
                    }
                    else
                    {
                        var row = this.dataGridView1.CurrentRow;
                        string fileName = row.Cells[_fileNameColIndex].Value.ToString();
                        string dfspath = this.treeView1.SelectedNode.Name;
                        string hdfsFile = dfspath + "/" + fileName;
                        if (DialogResult.OK == MessageBox.Show("确定要删除文件：\"" + hdfsFile + "\" ？",
                            "确认提示", MessageBoxButtons.OKCancel))
                        {
                            bool bok = HdfsHelper.Rm(hdfsFile);
                            if (bok)
                                ReloadDocsInDir(dfspath);
                        }
                    }
                }
                else    //删除目录
                {
                    TreeNode currNod = this.treeView1.SelectedNode;
                    string hdfsDir = currNod.Name;
                    if ("/".Equals(hdfsDir))
                    {
                        MessageBox.Show("根目录不能删除");
                        return;
                    }
                    if (DialogResult.OK == MessageBox.Show("确定要删除目录：\"" + currNod.Text + "\" ？",
                       "确认提示", MessageBoxButtons.OKCancel))
                    {
                        bool bok = HdfsHelper.Rm(hdfsDir);
                        if (bok)
                        {
                            // update cache
                            List<HdfsFileInfo> fis;
                            _fileinfoCache.TryRemove(hdfsDir, out fis);
                            string parentDir = hdfsDir.Substring(0, hdfsDir.LastIndexOf('/'));
                            if (string.IsNullOrWhiteSpace(parentDir))
                                parentDir = "/";
                            if (_fileinfoCache.ContainsKey(parentDir))
                            {
                                fis = _fileinfoCache[parentDir];
                                var currFi = fis.FirstOrDefault(fi => fi.IsDirectory && fi.Name == currNod.Text);
                                if (null != currFi)
                                    fis.Remove(currFi);
                            }
                            // refresh TreeView
                            this.treeView1.Nodes.Remove(currNod);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                DebugHelper.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        // 新建目录
        private void menuMkdir_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode currNod = this.treeView1.SelectedNode;
                if (null == currNod) return;

                FormInput fm = new FormInput();
                fm.Content = "newdirectory";
                if (DialogResult.OK == fm.ShowDialog())
                {
                    string name = fm.Content;
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        MessageBox.Show("目录名称不能为空！");
                        return;
                    }
                    string dirname = name.Replace(":", "").Replace("/", "")
                        .Replace(" ", "").Replace("*", "");
                    if (string.IsNullOrWhiteSpace(dirname))
                        return;

                    string parentDir = currNod.Name;
                    string hdfsDir = (parentDir.Length > 1) ? parentDir + "/" : parentDir;
                    hdfsDir = hdfsDir + dirname;

                    bool bok = HdfsHelper.MkDir(hdfsDir);
                    if (bok)
                    {
                        // update cache
                        if (_fileinfoCache.ContainsKey(parentDir))
                        {
                            List<HdfsFileInfo> fis = _fileinfoCache[parentDir];
                            if (null != fis)
                                fis.Add(new HdfsFileInfo()
                                {
                                    Name = dirname,
                                    PathName = hdfsDir,
                                    IsDirectory = true,
                                });
                        }
                        // Refresh TreeNodes
                        TreeNode nod = currNod.Nodes.Add(dirname, dirname, 0);
                        nod.Name = hdfsDir;
                    }
                    else
                        MessageBox.Show("目录创建失败");
                }
            }
            catch (System.Exception ex)
            {
                DebugHelper.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        // 快捷键 F5 刷新目录
        private void FormHDFS_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.F5 == e.KeyCode)
            {
                ClearGrid();
                ClearTree();
                _fileinfoCache.Clear();
                ConnectWebHDFS();  //重新加载
            }
            else if (Keys.F2 == e.KeyCode)
            {
                // copy current dir
                Clipboard.SetText(this.treeView1.SelectedNode.Name);
            }
        }

        // 工具条 设置服务器地址
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            FormServer fm = new FormServer();
            fm.Address = this._server;
            if (DialogResult.OK == fm.ShowDialog() &&
                !string.IsNullOrWhiteSpace(fm.Address))
            {
                this._server = fm.Address;
                ClearGrid();
                ClearTree();
                _fileinfoCache.Clear();
                ConnectWebHDFS();  //重新加载
            }

        }

        // 工具条 打开下载路径
        private void btnOpenDownloadDir_Click(object sender, EventArgs e)
        {
            string downDir = GetDownloadDir();

            Process p = Process.Start(downDir);
        }

        // 工具条 上传文件
        private void toolStripBtnUpload_Click(object sender, EventArgs e)
        {
            menuUpload_Click(null, null);
        }

        // 工具条 下载文件
        private void toolStripBtnDownload_Click(object sender, EventArgs e)
        {
            if (null == this.dataGridView1.CurrentRow)
            {
                MessageBox.Show("请先选择文件", "提示");
                return;
            }
            menuDownload_Click(null, null);
        }

        // 显示Loading动画
        private void ShowDirLoading(bool bVisible)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action<bool>(ShowDirLoading), bVisible);
            else
            {
                this.panelDirLoading.Visible = bVisible;
            }
        }

        private void ShowFileLoading(bool bVisible)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action<bool>(ShowFileLoading), bVisible);
            else
            {
                this.panelFileLoading.Visible = bVisible;
            }
        }

        private void ShowFileTransmission(bool bVisible, string msg)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action<bool, string>(ShowFileTransmission), bVisible, msg);
            else
            {
                this.panelFileTrans.Visible = bVisible;
                this.labelLoading.Text = msg;
            }
        }

        // 路径前面加上 hdfs://serverIP:9000/ 
        private string ConstructDfsFullPath(string dir)
        {
            return "hdfs://" + _server + ":9000" + dir;
        }

        private string GetDownloadDir()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string destDir = Path.Combine(path, "Download");
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);
            return destDir;
        }


    }
}
