namespace PBPid
{
    partial class FormAddModifyUser
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
            this.label_Id = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_UserPass = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_DisableCmd = new System.Windows.Forms.TextBox();
            this.button_AddModify = new System.Windows.Forms.Button();
            this.button_close = new System.Windows.Forms.Button();
            this.dateTimePicker_BeginDate = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_EndDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_Desc = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_Office = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.rad_Enable = new System.Windows.Forms.RadioButton();
            this.rad_Unable = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label_Id
            // 
            this.label_Id.AutoSize = true;
            this.label_Id.Location = new System.Drawing.Point(388, 373);
            this.label_Id.Name = "label_Id";
            this.label_Id.Size = new System.Drawing.Size(41, 12);
            this.label_Id.TabIndex = 1;
            this.label_Id.Text = "label2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(229, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "绑定Office号：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "登录用户名：";
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.Location = new System.Drawing.Point(94, 19);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.Size = new System.Drawing.Size(118, 21);
            this.textBox_UserName.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "登录密码：";
            // 
            // textBox_UserPass
            // 
            this.textBox_UserPass.Location = new System.Drawing.Point(94, 52);
            this.textBox_UserPass.Name = "textBox_UserPass";
            this.textBox_UserPass.PasswordChar = '*';
            this.textBox_UserPass.Size = new System.Drawing.Size(118, 21);
            this.textBox_UserPass.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(47, 214);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "备注：";
            // 
            // textBox_DisableCmd
            // 
            this.textBox_DisableCmd.Location = new System.Drawing.Point(94, 149);
            this.textBox_DisableCmd.Multiline = true;
            this.textBox_DisableCmd.Name = "textBox_DisableCmd";
            this.textBox_DisableCmd.Size = new System.Drawing.Size(353, 40);
            this.textBox_DisableCmd.TabIndex = 5;
            // 
            // button_AddModify
            // 
            this.button_AddModify.Location = new System.Drawing.Point(137, 263);
            this.button_AddModify.Name = "button_AddModify";
            this.button_AddModify.Size = new System.Drawing.Size(75, 23);
            this.button_AddModify.TabIndex = 8;
            this.button_AddModify.Text = "确定添加";
            this.button_AddModify.UseVisualStyleBackColor = true;
            this.button_AddModify.Click += new System.EventHandler(this.button_AddModify_Click);
            // 
            // button_close
            // 
            this.button_close.Location = new System.Drawing.Point(236, 263);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(75, 23);
            this.button_close.TabIndex = 9;
            this.button_close.Text = "关闭";
            this.button_close.UseVisualStyleBackColor = true;
            this.button_close.Click += new System.EventHandler(this.button_close_Click);
            // 
            // dateTimePicker_BeginDate
            // 
            this.dateTimePicker_BeginDate.Location = new System.Drawing.Point(94, 118);
            this.dateTimePicker_BeginDate.Name = "dateTimePicker_BeginDate";
            this.dateTimePicker_BeginDate.Size = new System.Drawing.Size(118, 21);
            this.dateTimePicker_BeginDate.TabIndex = 3;
            // 
            // dateTimePicker_EndDate
            // 
            this.dateTimePicker_EndDate.Location = new System.Drawing.Point(324, 118);
            this.dateTimePicker_EndDate.Name = "dateTimePicker_EndDate";
            this.dateTimePicker_EndDate.Size = new System.Drawing.Size(120, 21);
            this.dateTimePicker_EndDate.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "开始日期：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(253, 124);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "到期日期：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(23, 152);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "禁用指令：";
            // 
            // textBox_Desc
            // 
            this.textBox_Desc.Location = new System.Drawing.Point(94, 211);
            this.textBox_Desc.Multiline = true;
            this.textBox_Desc.Name = "textBox_Desc";
            this.textBox_Desc.Size = new System.Drawing.Size(353, 40);
            this.textBox_Desc.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(92, 192);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(359, 12);
            this.label8.TabIndex = 10;
            this.label8.Text = "不同的指令请用逗号隔开（如：SD,AVH,RT），逗号请用小写逗号。";
            // 
            // textBox_Office
            // 
            this.textBox_Office.Location = new System.Drawing.Point(324, 19);
            this.textBox_Office.Name = "textBox_Office";
            this.textBox_Office.Size = new System.Drawing.Size(120, 21);
            this.textBox_Office.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(218, 52);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "查看";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(47, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 13;
            this.label9.Text = "状态：";
            // 
            // rad_Enable
            // 
            this.rad_Enable.AutoSize = true;
            this.rad_Enable.Checked = true;
            this.rad_Enable.Location = new System.Drawing.Point(95, 86);
            this.rad_Enable.Name = "rad_Enable";
            this.rad_Enable.Size = new System.Drawing.Size(47, 16);
            this.rad_Enable.TabIndex = 14;
            this.rad_Enable.TabStop = true;
            this.rad_Enable.Text = "启用";
            this.rad_Enable.UseVisualStyleBackColor = true;
            // 
            // rad_Unable
            // 
            this.rad_Unable.AutoSize = true;
            this.rad_Unable.Location = new System.Drawing.Point(165, 86);
            this.rad_Unable.Name = "rad_Unable";
            this.rad_Unable.Size = new System.Drawing.Size(47, 16);
            this.rad_Unable.TabIndex = 15;
            this.rad_Unable.Text = "禁用";
            this.rad_Unable.UseVisualStyleBackColor = true;
            // 
            // FormAddModifyUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 299);
            this.Controls.Add(this.rad_Unable);
            this.Controls.Add(this.rad_Enable);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox_Office);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker_EndDate);
            this.Controls.Add(this.dateTimePicker_BeginDate);
            this.Controls.Add(this.button_close);
            this.Controls.Add(this.button_AddModify);
            this.Controls.Add(this.textBox_UserPass);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_Desc);
            this.Controls.Add(this.textBox_DisableCmd);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_UserName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label_Id);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAddModifyUser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "放大用户设置";
            this.Load += new System.EventHandler(this.FormAddUser_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Id;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_UserPass;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_DisableCmd;
        private System.Windows.Forms.Button button_AddModify;
        private System.Windows.Forms.Button button_close;
        private System.Windows.Forms.DateTimePicker dateTimePicker_BeginDate;
        private System.Windows.Forms.DateTimePicker dateTimePicker_EndDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_Desc;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_Office;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton rad_Enable;
        private System.Windows.Forms.RadioButton rad_Unable;
    }
}