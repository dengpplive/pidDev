namespace DataCacheService
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.button_RUN = new System.Windows.Forms.Button();
            this.listBox_status = new System.Windows.Forms.ListBox();
            this.button_Close = new System.Windows.Forms.Button();
            this.button_Clear = new System.Windows.Forms.Button();
            this.label_Mem = new System.Windows.Forms.Label();
            this.button_Load = new System.Windows.Forms.Button();
            this.label_time = new System.Windows.Forms.Label();
            this.label_counter = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_RUN
            // 
            this.button_RUN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_RUN.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_RUN.Image = ((System.Drawing.Image)(resources.GetObject("button_RUN.Image")));
            this.button_RUN.Location = new System.Drawing.Point(9, 385);
            this.button_RUN.Name = "button_RUN";
            this.button_RUN.Size = new System.Drawing.Size(75, 23);
            this.button_RUN.TabIndex = 0;
            this.button_RUN.Text = "启动服务";
            this.button_RUN.UseVisualStyleBackColor = true;
            this.button_RUN.Click += new System.EventHandler(this.button_RUN_Click);
            // 
            // listBox_status
            // 
            this.listBox_status.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox_status.BackColor = System.Drawing.Color.Black;
            this.listBox_status.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox_status.ForeColor = System.Drawing.Color.Lime;
            this.listBox_status.FormattingEnabled = true;
            this.listBox_status.ItemHeight = 14;
            this.listBox_status.Location = new System.Drawing.Point(9, 26);
            this.listBox_status.Name = "listBox_status";
            this.listBox_status.Size = new System.Drawing.Size(767, 354);
            this.listBox_status.TabIndex = 1;
            // 
            // button_Close
            // 
            this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Close.Location = new System.Drawing.Point(310, 385);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(75, 23);
            this.button_Close.TabIndex = 4;
            this.button_Close.Text = "关闭";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // button_Clear
            // 
            this.button_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Clear.Location = new System.Drawing.Point(90, 385);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(133, 23);
            this.button_Clear.TabIndex = 7;
            this.button_Clear.Text = "清空缓存并刷新";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // label_Mem
            // 
            this.label_Mem.AutoSize = true;
            this.label_Mem.Location = new System.Drawing.Point(7, 9);
            this.label_Mem.Name = "label_Mem";
            this.label_Mem.Size = new System.Drawing.Size(137, 12);
            this.label_Mem.TabIndex = 8;
            this.label_Mem.Text = "缓存系统物理内存使用：";
            // 
            // button_Load
            // 
            this.button_Load.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Load.Location = new System.Drawing.Point(229, 385);
            this.button_Load.Name = "button_Load";
            this.button_Load.Size = new System.Drawing.Size(75, 23);
            this.button_Load.TabIndex = 9;
            this.button_Load.Text = "立即刷新";
            this.button_Load.UseVisualStyleBackColor = true;
            this.button_Load.Click += new System.EventHandler(this.button_Load_Click);
            // 
            // label_time
            // 
            this.label_time.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_time.AutoSize = true;
            this.label_time.Location = new System.Drawing.Point(411, 390);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(41, 12);
            this.label_time.TabIndex = 10;
            this.label_time.Text = "label1";
            // 
            // label_counter
            // 
            this.label_counter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_counter.AutoSize = true;
            this.label_counter.Location = new System.Drawing.Point(590, 390);
            this.label_counter.Name = "label_counter";
            this.label_counter.Size = new System.Drawing.Size(41, 12);
            this.label_counter.TabIndex = 10;
            this.label_counter.Text = "label1";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 412);
            this.Controls.Add(this.label_counter);
            this.Controls.Add(this.label_time);
            this.Controls.Add(this.button_Load);
            this.Controls.Add(this.label_Mem);
            this.Controls.Add(this.button_Clear);
            this.Controls.Add(this.button_Close);
            this.Controls.Add(this.listBox_status);
            this.Controls.Add(this.button_RUN);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "测试版_票宝数据缓存服务端";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_RUN;
        private System.Windows.Forms.ListBox listBox_status;
        private System.Windows.Forms.Button button_Close;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Label label_Mem;
        private System.Windows.Forms.Button button_Load;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Label label_counter;
    }
}

