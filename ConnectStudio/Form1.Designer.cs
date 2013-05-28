namespace ConnectStudio
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Sgry.Azuki.FontInfo fontInfo3 = new Sgry.Azuki.FontInfo();
            Sgry.Azuki.FontInfo fontInfo4 = new Sgry.Azuki.FontInfo();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.processComboBox = new System.Windows.Forms.ComboBox();
            this.canvas = new System.Windows.Forms.PictureBox();
            this.appListView = new System.Windows.Forms.ListView();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.srcIconTitle = new System.Windows.Forms.Label();
            this.radioContextMenu = new System.Windows.Forms.RadioButton();
            this.textBoxContextMenu = new System.Windows.Forms.TextBox();
            this.radioHotKey = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel7 = new System.Windows.Forms.FlowLayoutPanel();
            this.CtrlCheckBoxHotKey = new System.Windows.Forms.CheckBox();
            this.AltCheckBoxHotKey = new System.Windows.Forms.CheckBox();
            this.WinCheckBoxHotKey = new System.Windows.Forms.CheckBox();
            this.ShiftCheckBoxHotKey = new System.Windows.Forms.CheckBox();
            this.FnCheckBoxHotKey = new System.Windows.Forms.CheckBox();
            this.keyComboBoxHotKey = new System.Windows.Forms.ComboBox();
            this.BlockKeyStrokeCheckBox = new System.Windows.Forms.CheckBox();
            this.srcScript = new Sgry.Azuki.WinForms.AzukiControl();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.dstIconTitle = new System.Windows.Forms.Label();
            this.dstScript = new Sgry.Azuki.WinForms.AzukiControl();
            this.applyActionHandlerButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.virtualContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.newNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel7.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer1.Panel2.Controls.Add(this.applyActionHandlerButton);
            this.splitContainer1.Size = new System.Drawing.Size(1105, 565);
            this.splitContainer1.SplitterDistance = 628;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.processComboBox);
            this.splitContainer2.Panel1.Controls.Add(this.canvas);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.appListView);
            this.splitContainer2.Size = new System.Drawing.Size(628, 565);
            this.splitContainer2.SplitterDistance = 306;
            this.splitContainer2.TabIndex = 1;
            // 
            // processComboBox
            // 
            this.processComboBox.FormattingEnabled = true;
            this.processComboBox.Location = new System.Drawing.Point(12, 3);
            this.processComboBox.Name = "processComboBox";
            this.processComboBox.Size = new System.Drawing.Size(151, 20);
            this.processComboBox.TabIndex = 1;
            this.processComboBox.DropDown += new System.EventHandler(this.processComboBox_DropDown);
            this.processComboBox.SelectedIndexChanged += new System.EventHandler(this.processComboBox_SelectedIndexChanged);
            // 
            // canvas
            // 
            this.canvas.BackColor = System.Drawing.Color.White;
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.Location = new System.Drawing.Point(0, 0);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(628, 306);
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.canvas_Paint);
            this.canvas.MouseClick += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseClick);
            this.canvas.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseDoubleClick);
            this.canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseMove);
            // 
            // appListView
            // 
            this.appListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appListView.Location = new System.Drawing.Point(0, 0);
            this.appListView.Name = "appListView";
            this.appListView.Size = new System.Drawing.Size(628, 255);
            this.appListView.TabIndex = 0;
            this.appListView.UseCompatibleStateImageBehavior = false;
            this.appListView.Click += new System.EventHandler(this.appListView_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.srcIconTitle);
            this.flowLayoutPanel1.Controls.Add(this.radioContextMenu);
            this.flowLayoutPanel1.Controls.Add(this.textBoxContextMenu);
            this.flowLayoutPanel1.Controls.Add(this.radioHotKey);
            this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel7);
            this.flowLayoutPanel1.Controls.Add(this.BlockKeyStrokeCheckBox);
            this.flowLayoutPanel1.Controls.Add(this.srcScript);
            this.flowLayoutPanel1.Controls.Add(this.splitter1);
            this.flowLayoutPanel1.Controls.Add(this.dstIconTitle);
            this.flowLayoutPanel1.Controls.Add(this.dstScript);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(463, 505);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // srcIconTitle
            // 
            this.srcIconTitle.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.srcIconTitle.Location = new System.Drawing.Point(3, 0);
            this.srcIconTitle.Name = "srcIconTitle";
            this.srcIconTitle.Size = new System.Drawing.Size(454, 64);
            this.srcIconTitle.TabIndex = 0;
            this.srcIconTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // radioContextMenu
            // 
            this.radioContextMenu.AutoSize = true;
            this.radioContextMenu.Checked = true;
            this.radioContextMenu.Location = new System.Drawing.Point(3, 67);
            this.radioContextMenu.Name = "radioContextMenu";
            this.radioContextMenu.Size = new System.Drawing.Size(114, 20);
            this.radioContextMenu.TabIndex = 14;
            this.radioContextMenu.TabStop = true;
            this.radioContextMenu.Text = "ContextMenu";
            this.radioContextMenu.UseVisualStyleBackColor = true;
            this.radioContextMenu.CheckedChanged += new System.EventHandler(this.EdgeDetailChanged);
            // 
            // textBoxContextMenu
            // 
            this.textBoxContextMenu.Location = new System.Drawing.Point(3, 93);
            this.textBoxContextMenu.Name = "textBoxContextMenu";
            this.textBoxContextMenu.Size = new System.Drawing.Size(436, 23);
            this.textBoxContextMenu.TabIndex = 1;
            this.textBoxContextMenu.TextChanged += new System.EventHandler(this.EdgeDetailChanged);
            // 
            // radioHotKey
            // 
            this.radioHotKey.AutoSize = true;
            this.radioHotKey.Location = new System.Drawing.Point(3, 122);
            this.radioHotKey.Name = "radioHotKey";
            this.radioHotKey.Size = new System.Drawing.Size(74, 20);
            this.radioHotKey.TabIndex = 13;
            this.radioHotKey.Text = "HotKey";
            this.radioHotKey.UseVisualStyleBackColor = true;
            this.radioHotKey.CheckedChanged += new System.EventHandler(this.EdgeDetailChanged);
            // 
            // flowLayoutPanel7
            // 
            this.flowLayoutPanel7.Controls.Add(this.CtrlCheckBoxHotKey);
            this.flowLayoutPanel7.Controls.Add(this.AltCheckBoxHotKey);
            this.flowLayoutPanel7.Controls.Add(this.WinCheckBoxHotKey);
            this.flowLayoutPanel7.Controls.Add(this.ShiftCheckBoxHotKey);
            this.flowLayoutPanel7.Controls.Add(this.FnCheckBoxHotKey);
            this.flowLayoutPanel7.Controls.Add(this.keyComboBoxHotKey);
            this.flowLayoutPanel7.Location = new System.Drawing.Point(0, 145);
            this.flowLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel7.Name = "flowLayoutPanel7";
            this.flowLayoutPanel7.Size = new System.Drawing.Size(457, 28);
            this.flowLayoutPanel7.TabIndex = 10;
            // 
            // CtrlCheckBoxHotKey
            // 
            this.CtrlCheckBoxHotKey.AutoSize = true;
            this.CtrlCheckBoxHotKey.Location = new System.Drawing.Point(3, 3);
            this.CtrlCheckBoxHotKey.Name = "CtrlCheckBoxHotKey";
            this.CtrlCheckBoxHotKey.Size = new System.Drawing.Size(59, 20);
            this.CtrlCheckBoxHotKey.TabIndex = 5;
            this.CtrlCheckBoxHotKey.Text = "Ctrl";
            this.CtrlCheckBoxHotKey.UseVisualStyleBackColor = true;
            this.CtrlCheckBoxHotKey.CheckedChanged += new System.EventHandler(this.EdgeDetailChanged);
            // 
            // AltCheckBoxHotKey
            // 
            this.AltCheckBoxHotKey.AutoSize = true;
            this.AltCheckBoxHotKey.Location = new System.Drawing.Point(67, 2);
            this.AltCheckBoxHotKey.Margin = new System.Windows.Forms.Padding(2);
            this.AltCheckBoxHotKey.Name = "AltCheckBoxHotKey";
            this.AltCheckBoxHotKey.Size = new System.Drawing.Size(51, 20);
            this.AltCheckBoxHotKey.TabIndex = 6;
            this.AltCheckBoxHotKey.Text = "Alt";
            this.AltCheckBoxHotKey.UseVisualStyleBackColor = true;
            this.AltCheckBoxHotKey.CheckedChanged += new System.EventHandler(this.EdgeDetailChanged);
            // 
            // WinCheckBoxHotKey
            // 
            this.WinCheckBoxHotKey.AutoSize = true;
            this.WinCheckBoxHotKey.Location = new System.Drawing.Point(122, 2);
            this.WinCheckBoxHotKey.Margin = new System.Windows.Forms.Padding(2);
            this.WinCheckBoxHotKey.Name = "WinCheckBoxHotKey";
            this.WinCheckBoxHotKey.Size = new System.Drawing.Size(51, 20);
            this.WinCheckBoxHotKey.TabIndex = 7;
            this.WinCheckBoxHotKey.Text = "Win";
            this.WinCheckBoxHotKey.UseVisualStyleBackColor = true;
            this.WinCheckBoxHotKey.CheckedChanged += new System.EventHandler(this.EdgeDetailChanged);
            // 
            // ShiftCheckBoxHotKey
            // 
            this.ShiftCheckBoxHotKey.AutoSize = true;
            this.ShiftCheckBoxHotKey.Location = new System.Drawing.Point(177, 2);
            this.ShiftCheckBoxHotKey.Margin = new System.Windows.Forms.Padding(2);
            this.ShiftCheckBoxHotKey.Name = "ShiftCheckBoxHotKey";
            this.ShiftCheckBoxHotKey.Size = new System.Drawing.Size(67, 20);
            this.ShiftCheckBoxHotKey.TabIndex = 8;
            this.ShiftCheckBoxHotKey.Text = "Shift";
            this.ShiftCheckBoxHotKey.UseVisualStyleBackColor = true;
            this.ShiftCheckBoxHotKey.CheckedChanged += new System.EventHandler(this.EdgeDetailChanged);
            // 
            // FnCheckBoxHotKey
            // 
            this.FnCheckBoxHotKey.AutoSize = true;
            this.FnCheckBoxHotKey.Location = new System.Drawing.Point(248, 2);
            this.FnCheckBoxHotKey.Margin = new System.Windows.Forms.Padding(2);
            this.FnCheckBoxHotKey.Name = "FnCheckBoxHotKey";
            this.FnCheckBoxHotKey.Size = new System.Drawing.Size(43, 20);
            this.FnCheckBoxHotKey.TabIndex = 10;
            this.FnCheckBoxHotKey.Text = "Fn";
            this.FnCheckBoxHotKey.UseVisualStyleBackColor = true;
            this.FnCheckBoxHotKey.CheckedChanged += new System.EventHandler(this.EdgeDetailChanged);
            // 
            // keyComboBoxHotKey
            // 
            this.keyComboBoxHotKey.FormattingEnabled = true;
            this.keyComboBoxHotKey.Location = new System.Drawing.Point(295, 2);
            this.keyComboBoxHotKey.Margin = new System.Windows.Forms.Padding(2);
            this.keyComboBoxHotKey.Name = "keyComboBoxHotKey";
            this.keyComboBoxHotKey.Size = new System.Drawing.Size(144, 24);
            this.keyComboBoxHotKey.TabIndex = 9;
            this.keyComboBoxHotKey.TextChanged += new System.EventHandler(this.EdgeDetailChanged);
            // 
            // BlockKeyStrokeCheckBox
            // 
            this.BlockKeyStrokeCheckBox.AutoSize = true;
            this.BlockKeyStrokeCheckBox.Location = new System.Drawing.Point(3, 176);
            this.BlockKeyStrokeCheckBox.Name = "BlockKeyStrokeCheckBox";
            this.BlockKeyStrokeCheckBox.Size = new System.Drawing.Size(203, 20);
            this.BlockKeyStrokeCheckBox.TabIndex = 12;
            this.BlockKeyStrokeCheckBox.Text = "Ignore in this process";
            this.BlockKeyStrokeCheckBox.UseVisualStyleBackColor = true;
            this.BlockKeyStrokeCheckBox.CheckedChanged += new System.EventHandler(this.EdgeDetailChanged);
            // 
            // srcScript
            // 
            this.srcScript.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(250)))), ((int)(((byte)(240)))));
            this.srcScript.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.srcScript.DrawingOption = ((Sgry.Azuki.DrawingOption)(((((((Sgry.Azuki.DrawingOption.DrawsFullWidthSpace | Sgry.Azuki.DrawingOption.DrawsTab) 
            | Sgry.Azuki.DrawingOption.DrawsEol) 
            | Sgry.Azuki.DrawingOption.HighlightCurrentLine) 
            | Sgry.Azuki.DrawingOption.ShowsLineNumber) 
            | Sgry.Azuki.DrawingOption.ShowsDirtBar) 
            | Sgry.Azuki.DrawingOption.HighlightsMatchedBracket)));
            this.srcScript.FirstVisibleLine = 0;
            this.srcScript.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            fontInfo3.Name = "MS UI Gothic";
            fontInfo3.Size = 9;
            fontInfo3.Style = System.Drawing.FontStyle.Regular;
            this.srcScript.FontInfo = fontInfo3;
            this.srcScript.ForeColor = System.Drawing.Color.Black;
            this.srcScript.Location = new System.Drawing.Point(3, 202);
            this.srcScript.Name = "srcScript";
            this.srcScript.Size = new System.Drawing.Size(457, 99);
            this.srcScript.TabIndex = 7;
            this.srcScript.ViewWidth = 4129;
            this.srcScript.TextChanged += new System.EventHandler(this.EdgeDetailChanged);
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.Control;
            this.splitter1.Location = new System.Drawing.Point(3, 307);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(454, 5);
            this.splitter1.TabIndex = 8;
            this.splitter1.TabStop = false;
            // 
            // dstIconTitle
            // 
            this.dstIconTitle.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.dstIconTitle.Location = new System.Drawing.Point(3, 315);
            this.dstIconTitle.Name = "dstIconTitle";
            this.dstIconTitle.Size = new System.Drawing.Size(454, 64);
            this.dstIconTitle.TabIndex = 6;
            this.dstIconTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // dstScript
            // 
            this.dstScript.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(250)))), ((int)(((byte)(240)))));
            this.dstScript.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.dstScript.DrawingOption = ((Sgry.Azuki.DrawingOption)(((((((Sgry.Azuki.DrawingOption.DrawsFullWidthSpace | Sgry.Azuki.DrawingOption.DrawsTab) 
            | Sgry.Azuki.DrawingOption.DrawsEol) 
            | Sgry.Azuki.DrawingOption.HighlightCurrentLine) 
            | Sgry.Azuki.DrawingOption.ShowsLineNumber) 
            | Sgry.Azuki.DrawingOption.ShowsDirtBar) 
            | Sgry.Azuki.DrawingOption.HighlightsMatchedBracket)));
            this.dstScript.FirstVisibleLine = 0;
            this.dstScript.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            fontInfo4.Name = "MS UI Gothic";
            fontInfo4.Size = 9;
            fontInfo4.Style = System.Drawing.FontStyle.Regular;
            this.dstScript.FontInfo = fontInfo4;
            this.dstScript.ForeColor = System.Drawing.Color.Black;
            this.dstScript.Location = new System.Drawing.Point(3, 382);
            this.dstScript.Name = "dstScript";
            this.dstScript.Size = new System.Drawing.Size(457, 120);
            this.dstScript.TabIndex = 9;
            this.dstScript.ViewWidth = 4129;
            this.dstScript.TextChanged += new System.EventHandler(this.EdgeDetailChanged);
            // 
            // applyActionHandlerButton
            // 
            this.applyActionHandlerButton.Location = new System.Drawing.Point(391, 526);
            this.applyActionHandlerButton.Name = "applyActionHandlerButton";
            this.applyActionHandlerButton.Size = new System.Drawing.Size(75, 23);
            this.applyActionHandlerButton.TabIndex = 15;
            this.applyActionHandlerButton.Text = "Delete";
            this.applyActionHandlerButton.UseVisualStyleBackColor = true;
            this.applyActionHandlerButton.Click += new System.EventHandler(this.applyActionHandlerButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileFToolStripMenuItem,
            this.runRToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1105, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileFToolStripMenuItem
            // 
            this.fileFToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newNToolStripMenuItem,
            this.toolStripSeparator2,
            this.openOToolStripMenuItem,
            this.saveSToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitQToolStripMenuItem});
            this.fileFToolStripMenuItem.Name = "fileFToolStripMenuItem";
            this.fileFToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.fileFToolStripMenuItem.Text = "File(&F)";
            // 
            // openOToolStripMenuItem
            // 
            this.openOToolStripMenuItem.Name = "openOToolStripMenuItem";
            this.openOToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openOToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.openOToolStripMenuItem.Text = "Open(&O)";
            this.openOToolStripMenuItem.Click += new System.EventHandler(this.openOToolStripMenuItem_Click);
            // 
            // saveSToolStripMenuItem
            // 
            this.saveSToolStripMenuItem.Name = "saveSToolStripMenuItem";
            this.saveSToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveSToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.saveSToolStripMenuItem.Text = "Save(&S)";
            this.saveSToolStripMenuItem.Click += new System.EventHandler(this.saveSToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(167, 6);
            // 
            // exitQToolStripMenuItem
            // 
            this.exitQToolStripMenuItem.Name = "exitQToolStripMenuItem";
            this.exitQToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.exitQToolStripMenuItem.Text = "Exit(&Q)";
            this.exitQToolStripMenuItem.Click += new System.EventHandler(this.exitQToolStripMenuItem_Click);
            // 
            // runRToolStripMenuItem
            // 
            this.runRToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startAllToolStripMenuItem,
            this.stopAllToolStripMenuItem});
            this.runRToolStripMenuItem.Name = "runRToolStripMenuItem";
            this.runRToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.runRToolStripMenuItem.Text = "Run(&R)";
            // 
            // startAllToolStripMenuItem
            // 
            this.startAllToolStripMenuItem.Name = "startAllToolStripMenuItem";
            this.startAllToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.startAllToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.startAllToolStripMenuItem.Text = "Start All";
            this.startAllToolStripMenuItem.Click += new System.EventHandler(this.startAllToolStripMenuItem_Click);
            // 
            // stopAllToolStripMenuItem
            // 
            this.stopAllToolStripMenuItem.Name = "stopAllToolStripMenuItem";
            this.stopAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.stopAllToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.stopAllToolStripMenuItem.Text = "Stop All";
            this.stopAllToolStripMenuItem.Click += new System.EventHandler(this.stopAllToolStripMenuItem_Click);
            // 
            // virtualContextMenu
            // 
            this.virtualContextMenu.Name = "virtualContextMenu";
            this.virtualContextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // newNToolStripMenuItem
            // 
            this.newNToolStripMenuItem.Name = "newNToolStripMenuItem";
            this.newNToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newNToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.newNToolStripMenuItem.Text = "New(&N)";
            this.newNToolStripMenuItem.Click += new System.EventHandler(this.newNToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(167, 6);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1105, 589);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel7.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox canvas;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label srcIconTitle;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel7;
        private System.Windows.Forms.CheckBox CtrlCheckBoxHotKey;
        private System.Windows.Forms.CheckBox AltCheckBoxHotKey;
        private System.Windows.Forms.CheckBox WinCheckBoxHotKey;
        private System.Windows.Forms.CheckBox ShiftCheckBoxHotKey;
        private System.Windows.Forms.ComboBox keyComboBoxHotKey;
        private System.Windows.Forms.CheckBox BlockKeyStrokeCheckBox;
        private Sgry.Azuki.WinForms.AzukiControl srcScript;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Label dstIconTitle;
        private Sgry.Azuki.WinForms.AzukiControl dstScript;
        private System.Windows.Forms.RadioButton radioHotKey;
        private System.Windows.Forms.RadioButton radioContextMenu;
        private System.Windows.Forms.TextBox textBoxContextMenu;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopAllToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip virtualContextMenu;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView appListView;
        private System.Windows.Forms.CheckBox FnCheckBoxHotKey;
        private System.Windows.Forms.Button applyActionHandlerButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ComboBox processComboBox;
        private System.Windows.Forms.ToolStripMenuItem newNToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}

