namespace PBPid.Server
{
    partial class FormMain
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
            this.button_Run = new System.Windows.Forms.Button();
            this.button_ClearCache = new System.Windows.Forms.Button();
            this.button_Stop = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_ClearStatus = new System.Windows.Forms.Button();
            this.label_RunTime = new System.Windows.Forms.Label();
            this.label_Mem = new System.Windows.Forms.Label();
            this.ckbautoStart = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.listBox_Online = new PBPid.Server.myControl.myListBox();
            this.listBox_Status = new PBPid.Server.myControl.myListBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Run
            // 
            this.button_Run.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Run.Location = new System.Drawing.Point(150, 433);
            this.button_Run.Name = "button_Run";
            this.button_Run.Size = new System.Drawing.Size(111, 23);
            this.button_Run.TabIndex = 1;
            this.button_Run.Text = "开始服务";
            this.button_Run.UseVisualStyleBackColor = true;
            this.button_Run.Click += new System.EventHandler(this.button_Run_Click);
            // 
            // button_ClearCache
            // 
            this.button_ClearCache.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_ClearCache.Enabled = false;
            this.button_ClearCache.Location = new System.Drawing.Point(384, 433);
            this.button_ClearCache.Name = "button_ClearCache";
            this.button_ClearCache.Size = new System.Drawing.Size(111, 23);
            this.button_ClearCache.TabIndex = 1;
            this.button_ClearCache.Text = "清空缓存重新加载";
            this.button_ClearCache.UseVisualStyleBackColor = true;
            this.button_ClearCache.Click += new System.EventHandler(this.button_ClearCache_Click);
            // 
            // button_Stop
            // 
            this.button_Stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Stop.Enabled = false;
            this.button_Stop.Location = new System.Drawing.Point(267, 433);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(111, 23);
            this.button_Stop.TabIndex = 1;
            this.button_Stop.Text = "停止服务";
            this.button_Stop.UseVisualStyleBackColor = true;
            this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.listBox_Online);
            this.groupBox1.Controls.Add(this.listBox_Status);
            this.groupBox1.Location = new System.Drawing.Point(12, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(920, 391);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PBPid.客户端调用状态";
            // 
            // button_ClearStatus
            // 
            this.button_ClearStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_ClearStatus.Location = new System.Drawing.Point(501, 433);
            this.button_ClearStatus.Name = "button_ClearStatus";
            this.button_ClearStatus.Size = new System.Drawing.Size(111, 23);
            this.button_ClearStatus.TabIndex = 3;
            this.button_ClearStatus.Text = "清空显示状态";
            this.button_ClearStatus.UseVisualStyleBackColor = true;
            this.button_ClearStatus.Click += new System.EventHandler(this.button_ClearStatus_Click);
            // 
            // label_RunTime
            // 
            this.label_RunTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_RunTime.AutoSize = true;
            this.label_RunTime.Location = new System.Drawing.Point(16, 398);
            this.label_RunTime.Name = "label_RunTime";
            this.label_RunTime.Size = new System.Drawing.Size(53, 12);
            this.label_RunTime.TabIndex = 4;
            this.label_RunTime.Text = "系统运行";
            // 
            // label_Mem
            // 
            this.label_Mem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Mem.AutoSize = true;
            this.label_Mem.Location = new System.Drawing.Point(16, 416);
            this.label_Mem.Name = "label_Mem";
            this.label_Mem.Size = new System.Drawing.Size(95, 12);
            this.label_Mem.TabIndex = 5;
            this.label_Mem.Text = "PBPid.物理内存状态";
            // 
            // ckbautoStart
            // 
            this.ckbautoStart.AutoSize = true;
            this.ckbautoStart.Checked = true;
            this.ckbautoStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbautoStart.Location = new System.Drawing.Point(18, 437);
            this.ckbautoStart.Name = "ckbautoStart";
            this.ckbautoStart.Size = new System.Drawing.Size(126, 16);
            this.ckbautoStart.TabIndex = 6;
            this.ckbautoStart.Text = "5秒后自动启动服务";
            this.ckbautoStart.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // listBox_Online
            // 
            this.listBox_Online.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox_Online.BackColor = System.Drawing.Color.Black;
            this.listBox_Online.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox_Online.ForeColor = System.Drawing.Color.Lime;
            this.listBox_Online.ItemHeight = 12;
            this.listBox_Online.Location = new System.Drawing.Point(730, 13);
            this.listBox_Online.Name = "listBox_Online";
            this.listBox_Online.Size = new System.Drawing.Size(184, 364);
            this.listBox_Online.TabIndex = 5;
            // 
            // listBox_Status
            // 
            this.listBox_Status.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox_Status.BackColor = System.Drawing.Color.Black;
            this.listBox_Status.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox_Status.ForeColor = System.Drawing.Color.Lime;
            this.listBox_Status.ItemHeight = 12;
            this.listBox_Status.Location = new System.Drawing.Point(6, 13);
            this.listBox_Status.Name = "listBox_Status";
            this.listBox_Status.Size = new System.Drawing.Size(718, 364);
            this.listBox_Status.TabIndex = 4;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(938, 457);
            this.Controls.Add(this.ckbautoStart);
            this.Controls.Add(this.label_Mem);
            this.Controls.Add(this.label_RunTime);
            this.Controls.Add(this.button_ClearStatus);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Stop);
            this.Controls.Add(this.button_ClearCache);
            this.Controls.Add(this.button_Run);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PBPid.云端";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing_1);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Run;
        private System.Windows.Forms.Button button_ClearCache;
        private System.Windows.Forms.Button button_Stop;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_ClearStatus;
        private System.Windows.Forms.Label label_RunTime;
        private System.Windows.Forms.Label label_Mem;
        private myControl.myListBox listBox_Status;
        private myControl.myListBox listBox_Online;
        private System.Windows.Forms.CheckBox ckbautoStart;
        private System.Windows.Forms.Timer timer1;
    }
}

