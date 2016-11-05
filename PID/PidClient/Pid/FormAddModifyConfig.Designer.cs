namespace PBPid
{
    partial class FormAddModifyConfig
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Ip = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_Name = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_pass = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_SI = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox_type = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_office = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_corporationName = new System.Windows.Forms.TextBox();
            this.button_Add = new System.Windows.Forms.Button();
            this.button_close = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox_Desc = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label_ID = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_LocalIp = new System.Windows.Forms.TextBox();
            this.rad_Enable = new System.Windows.Forms.RadioButton();
            this.rad_Unable = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(271, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "认证服务器IP：";
            // 
            // textBox_Ip
            // 
            this.textBox_Ip.Location = new System.Drawing.Point(366, 36);
            this.textBox_Ip.Name = "textBox_Ip";
            this.textBox_Ip.Size = new System.Drawing.Size(164, 21);
            this.textBox_Ip.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(295, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "认证端口：";
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(366, 73);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(62, 21);
            this.textBox_port.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "原始配置帐号：";
            // 
            // textBox_Name
            // 
            this.textBox_Name.Location = new System.Drawing.Point(96, 110);
            this.textBox_Name.Name = "textBox_Name";
            this.textBox_Name.Size = new System.Drawing.Size(164, 21);
            this.textBox_Name.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(271, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "原始配置密码：";
            // 
            // textBox_pass
            // 
            this.textBox_pass.Location = new System.Drawing.Point(366, 110);
            this.textBox_pass.Name = "textBox_pass";
            this.textBox_pass.Size = new System.Drawing.Size(164, 21);
            this.textBox_pass.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(61, 155);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Si：";
            // 
            // textBox_SI
            // 
            this.textBox_SI.Location = new System.Drawing.Point(96, 152);
            this.textBox_SI.Name = "textBox_SI";
            this.textBox_SI.Size = new System.Drawing.Size(164, 21);
            this.textBox_SI.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(295, 155);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "认证方式：";
            // 
            // comboBox_type
            // 
            this.comboBox_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_type.FormattingEnabled = true;
            this.comboBox_type.Items.AddRange(new object[] {
            "密码认证",
            "地址认证",
            "信天游"});
            this.comboBox_type.Location = new System.Drawing.Point(366, 152);
            this.comboBox_type.Name = "comboBox_type";
            this.comboBox_type.Size = new System.Drawing.Size(163, 20);
            this.comboBox_type.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 194);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "OFFICE号：";
            // 
            // textBox_office
            // 
            this.textBox_office.Location = new System.Drawing.Point(96, 191);
            this.textBox_office.Name = "textBox_office";
            this.textBox_office.Size = new System.Drawing.Size(164, 21);
            this.textBox_office.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(295, 194);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "公司名称：";
            // 
            // textBox_corporationName
            // 
            this.textBox_corporationName.Location = new System.Drawing.Point(366, 191);
            this.textBox_corporationName.Name = "textBox_corporationName";
            this.textBox_corporationName.Size = new System.Drawing.Size(164, 21);
            this.textBox_corporationName.TabIndex = 8;
            // 
            // button_Add
            // 
            this.button_Add.Location = new System.Drawing.Point(152, 292);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(75, 23);
            this.button_Add.TabIndex = 10;
            this.button_Add.Text = "确定添加";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // button_close
            // 
            this.button_close.Location = new System.Drawing.Point(309, 292);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(75, 23);
            this.button_close.TabIndex = 11;
            this.button_close.Text = "关闭";
            this.button_close.UseVisualStyleBackColor = true;
            this.button_close.Click += new System.EventHandler(this.button_close_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(49, 223);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 4;
            this.label9.Text = "备注：";
            // 
            // textBox_Desc
            // 
            this.textBox_Desc.Location = new System.Drawing.Point(96, 223);
            this.textBox_Desc.Multiline = true;
            this.textBox_Desc.Name = "textBox_Desc";
            this.textBox_Desc.Size = new System.Drawing.Size(434, 66);
            this.textBox_Desc.TabIndex = 9;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(49, 385);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 6;
            this.label10.Text = "主键：";
            // 
            // label_ID
            // 
            this.label_ID.AutoSize = true;
            this.label_ID.Location = new System.Drawing.Point(97, 385);
            this.label_ID.Name = "label_ID";
            this.label_ID.Size = new System.Drawing.Size(11, 12);
            this.label_ID.TabIndex = 7;
            this.label_ID.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(49, 73);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 12);
            this.label11.TabIndex = 12;
            this.label11.Text = "状态：";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 39);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(77, 12);
            this.label12.TabIndex = 13;
            this.label12.Text = "本地IP地址：";
            // 
            // textBox_LocalIp
            // 
            this.textBox_LocalIp.Location = new System.Drawing.Point(96, 36);
            this.textBox_LocalIp.Name = "textBox_LocalIp";
            this.textBox_LocalIp.Size = new System.Drawing.Size(164, 21);
            this.textBox_LocalIp.TabIndex = 0;
            // 
            // rad_Enable
            // 
            this.rad_Enable.AutoSize = true;
            this.rad_Enable.Checked = true;
            this.rad_Enable.Location = new System.Drawing.Point(99, 73);
            this.rad_Enable.Name = "rad_Enable";
            this.rad_Enable.Size = new System.Drawing.Size(47, 16);
            this.rad_Enable.TabIndex = 2;
            this.rad_Enable.TabStop = true;
            this.rad_Enable.Text = "启用";
            this.rad_Enable.UseVisualStyleBackColor = true;
            // 
            // rad_Unable
            // 
            this.rad_Unable.AutoSize = true;
            this.rad_Unable.Location = new System.Drawing.Point(165, 73);
            this.rad_Unable.Name = "rad_Unable";
            this.rad_Unable.Size = new System.Drawing.Size(47, 16);
            this.rad_Unable.TabIndex = 3;
            this.rad_Unable.Text = "禁用";
            this.rad_Unable.UseVisualStyleBackColor = true;
            // 
            // FormAddModifyConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 357);
            this.Controls.Add(this.rad_Unable);
            this.Controls.Add(this.rad_Enable);
            this.Controls.Add(this.textBox_LocalIp);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label_ID);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.textBox_Desc);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.button_close);
            this.Controls.Add(this.button_Add);
            this.Controls.Add(this.comboBox_type);
            this.Controls.Add(this.textBox_pass);
            this.Controls.Add(this.textBox_corporationName);
            this.Controls.Add(this.textBox_office);
            this.Controls.Add(this.textBox_SI);
            this.Controls.Add(this.textBox_Name);
            this.Controls.Add(this.textBox_port);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_Ip);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAddModifyConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "原始配置设置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Ip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_Name;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_pass;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_SI;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox_type;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_office;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_corporationName;
        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.Button button_close;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox_Desc;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label_ID;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox_LocalIp;
        private System.Windows.Forms.RadioButton rad_Enable;
        private System.Windows.Forms.RadioButton rad_Unable;
    }
}