namespace PBPid
{
    partial class PidLogin
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.textBox_Pwd = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_ServerList = new System.Windows.Forms.ComboBox();
            this.checkBox_SavePwd = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoLogin = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(47, 157);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "登录";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(142, 157);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(56, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "用户名：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(68, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "密码：";
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.Location = new System.Drawing.Point(113, 17);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.Size = new System.Drawing.Size(124, 21);
            this.textBox_UserName.TabIndex = 0;
            // 
            // textBox_Pwd
            // 
            this.textBox_Pwd.Location = new System.Drawing.Point(113, 48);
            this.textBox_Pwd.Name = "textBox_Pwd";
            this.textBox_Pwd.PasswordChar = '*';
            this.textBox_Pwd.Size = new System.Drawing.Size(124, 21);
            this.textBox_Pwd.TabIndex = 1;
            this.textBox_Pwd.TextChanged += new System.EventHandler(this.textBox_Pwd_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "认证服务器：";
            // 
            // comboBox_ServerList
            // 
            this.comboBox_ServerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ServerList.FormattingEnabled = true;
            this.comboBox_ServerList.Location = new System.Drawing.Point(113, 79);
            this.comboBox_ServerList.Name = "comboBox_ServerList";
            this.comboBox_ServerList.Size = new System.Drawing.Size(124, 20);
            this.comboBox_ServerList.TabIndex = 7;
            // 
            // checkBox_SavePwd
            // 
            this.checkBox_SavePwd.AutoSize = true;
            this.checkBox_SavePwd.Location = new System.Drawing.Point(47, 119);
            this.checkBox_SavePwd.Name = "checkBox_SavePwd";
            this.checkBox_SavePwd.Size = new System.Drawing.Size(72, 16);
            this.checkBox_SavePwd.TabIndex = 8;
            this.checkBox_SavePwd.Text = "记住密码";
            this.checkBox_SavePwd.UseVisualStyleBackColor = true;
            // 
            // checkBox_AutoLogin
            // 
            this.checkBox_AutoLogin.AutoSize = true;
            this.checkBox_AutoLogin.Location = new System.Drawing.Point(154, 119);
            this.checkBox_AutoLogin.Name = "checkBox_AutoLogin";
            this.checkBox_AutoLogin.Size = new System.Drawing.Size(72, 16);
            this.checkBox_AutoLogin.TabIndex = 9;
            this.checkBox_AutoLogin.Text = "自动登录";
            this.checkBox_AutoLogin.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 3000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 192);
            this.Controls.Add(this.checkBox_AutoLogin);
            this.Controls.Add(this.checkBox_SavePwd);
            this.Controls.Add(this.comboBox_ServerList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_Pwd);
            this.Controls.Add(this.textBox_UserName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "FormLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PID登录验证";
            this.Load += new System.EventHandler(this.FormLog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.TextBox textBox_Pwd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_ServerList;
        private System.Windows.Forms.CheckBox checkBox_SavePwd;
        private System.Windows.Forms.CheckBox checkBox_AutoLogin;
        private System.Windows.Forms.Timer timer1;
    }
}