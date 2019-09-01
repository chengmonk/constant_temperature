namespace 恒温测试机.UI
{
    partial class FormConnectValueSetting
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.autoFind_A = new System.Windows.Forms.Button();
            this.angleBtn_A = new System.Windows.Forms.Button();
            this.angleTb_A = new System.Windows.Forms.TextBox();
            this.radioBtn_A = new System.Windows.Forms.Button();
            this.radioTb_A = new System.Windows.Forms.TextBox();
            this.shutdownBtn_A = new System.Windows.Forms.Button();
            this.autoRunBtn_A = new System.Windows.Forms.Button();
            this.backOrignBtn_A = new System.Windows.Forms.Button();
            this.orignBtn_A = new System.Windows.Forms.Button();
            this.angelLb_A = new System.Windows.Forms.Label();
            this.radioLb_A = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.noForwardBtn_A = new System.Windows.Forms.Button();
            this.forwardBtn_A = new System.Windows.Forms.Button();
            this.powerBtn_A = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.angleBtn_L = new System.Windows.Forms.Button();
            this.angleTb_L = new System.Windows.Forms.TextBox();
            this.radioBtn_L = new System.Windows.Forms.Button();
            this.radioTb_L = new System.Windows.Forms.TextBox();
            this.shutdownBtn_L = new System.Windows.Forms.Button();
            this.autoRunBtn_L = new System.Windows.Forms.Button();
            this.backOrignBtn_L = new System.Windows.Forms.Button();
            this.orignBtn_L = new System.Windows.Forms.Button();
            this.angelLb_L = new System.Windows.Forms.Label();
            this.radioLb_L = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.noForwardBtn_L = new System.Windows.Forms.Button();
            this.forwardBtn_L = new System.Windows.Forms.Button();
            this.powerBtn_L = new System.Windows.Forms.Button();
            this.outInfoTb = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 108);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "变频器通讯";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.comboBox1.Location = new System.Drawing.Point(58, 52);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(73, 20);
            this.comboBox1.TabIndex = 35;
            this.comboBox1.Text = "1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(13, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 17);
            this.label2.TabIndex = 34;
            this.label2.Text = "值";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(58, 78);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(73, 21);
            this.textBox3.TabIndex = 33;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(13, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 32;
            this.label1.Text = "站号";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(164, 28);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(74, 29);
            this.button2.TabIndex = 31;
            this.button2.Text = "读取";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.ReadBtn_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(13, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 17);
            this.label5.TabIndex = 30;
            this.label5.Text = "地址";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(58, 24);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(73, 21);
            this.textBox2.TabIndex = 29;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(164, 70);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 29);
            this.button1.TabIndex = 28;
            this.button1.Text = "写入";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.WriteBtn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.autoFind_A);
            this.groupBox2.Controls.Add(this.angleBtn_A);
            this.groupBox2.Controls.Add(this.angleTb_A);
            this.groupBox2.Controls.Add(this.radioBtn_A);
            this.groupBox2.Controls.Add(this.radioTb_A);
            this.groupBox2.Controls.Add(this.shutdownBtn_A);
            this.groupBox2.Controls.Add(this.autoRunBtn_A);
            this.groupBox2.Controls.Add(this.backOrignBtn_A);
            this.groupBox2.Controls.Add(this.orignBtn_A);
            this.groupBox2.Controls.Add(this.angelLb_A);
            this.groupBox2.Controls.Add(this.radioLb_A);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.noForwardBtn_A);
            this.groupBox2.Controls.Add(this.forwardBtn_A);
            this.groupBox2.Controls.Add(this.powerBtn_A);
            this.groupBox2.Location = new System.Drawing.Point(12, 147);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(327, 187);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "伺服电机A通讯";
            // 
            // autoFind_A
            // 
            this.autoFind_A.Location = new System.Drawing.Point(246, 130);
            this.autoFind_A.Name = "autoFind_A";
            this.autoFind_A.Size = new System.Drawing.Size(74, 29);
            this.autoFind_A.TabIndex = 46;
            this.autoFind_A.Text = "自动找点";
            this.autoFind_A.UseVisualStyleBackColor = true;
            this.autoFind_A.Click += new System.EventHandler(this.AutoFind_A_Click);
            // 
            // angleBtn_A
            // 
            this.angleBtn_A.Location = new System.Drawing.Point(247, 50);
            this.angleBtn_A.Name = "angleBtn_A";
            this.angleBtn_A.Size = new System.Drawing.Size(74, 29);
            this.angleBtn_A.TabIndex = 45;
            this.angleBtn_A.Text = "写入范围";
            this.angleBtn_A.UseVisualStyleBackColor = true;
            this.angleBtn_A.Click += new System.EventHandler(this.AngleBtn_A_Click);
            // 
            // angleTb_A
            // 
            this.angleTb_A.Location = new System.Drawing.Point(168, 53);
            this.angleTb_A.Name = "angleTb_A";
            this.angleTb_A.Size = new System.Drawing.Size(73, 21);
            this.angleTb_A.TabIndex = 44;
            // 
            // radioBtn_A
            // 
            this.radioBtn_A.Location = new System.Drawing.Point(247, 18);
            this.radioBtn_A.Name = "radioBtn_A";
            this.radioBtn_A.Size = new System.Drawing.Size(74, 29);
            this.radioBtn_A.TabIndex = 43;
            this.radioBtn_A.Text = "写入";
            this.radioBtn_A.UseVisualStyleBackColor = true;
            this.radioBtn_A.Click += new System.EventHandler(this.RadioBtn_A_Click);
            // 
            // radioTb_A
            // 
            this.radioTb_A.Location = new System.Drawing.Point(168, 22);
            this.radioTb_A.Name = "radioTb_A";
            this.radioTb_A.Size = new System.Drawing.Size(73, 21);
            this.radioTb_A.TabIndex = 42;
            // 
            // shutdownBtn_A
            // 
            this.shutdownBtn_A.Location = new System.Drawing.Point(6, 130);
            this.shutdownBtn_A.Name = "shutdownBtn_A";
            this.shutdownBtn_A.Size = new System.Drawing.Size(74, 29);
            this.shutdownBtn_A.TabIndex = 41;
            this.shutdownBtn_A.Text = "紧急停止";
            this.shutdownBtn_A.UseVisualStyleBackColor = true;
            this.shutdownBtn_A.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ShutdownBtn_A_MouseDown);
            this.shutdownBtn_A.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ShutdownBtn_A_MouseUp);
            // 
            // autoRunBtn_A
            // 
            this.autoRunBtn_A.Location = new System.Drawing.Point(246, 95);
            this.autoRunBtn_A.Name = "autoRunBtn_A";
            this.autoRunBtn_A.Size = new System.Drawing.Size(74, 29);
            this.autoRunBtn_A.TabIndex = 40;
            this.autoRunBtn_A.Text = "自动运行";
            this.autoRunBtn_A.UseVisualStyleBackColor = true;
            this.autoRunBtn_A.Click += new System.EventHandler(this.AutoRunBtn_Click);
            // 
            // backOrignBtn_A
            // 
            this.backOrignBtn_A.Location = new System.Drawing.Point(166, 130);
            this.backOrignBtn_A.Name = "backOrignBtn_A";
            this.backOrignBtn_A.Size = new System.Drawing.Size(74, 29);
            this.backOrignBtn_A.TabIndex = 39;
            this.backOrignBtn_A.Text = "回原点";
            this.backOrignBtn_A.UseVisualStyleBackColor = true;
            this.backOrignBtn_A.Click += new System.EventHandler(this.BackOrignBtn_Click);
            // 
            // orignBtn_A
            // 
            this.orignBtn_A.Location = new System.Drawing.Point(86, 130);
            this.orignBtn_A.Name = "orignBtn_A";
            this.orignBtn_A.Size = new System.Drawing.Size(74, 29);
            this.orignBtn_A.TabIndex = 38;
            this.orignBtn_A.Text = "原点";
            this.orignBtn_A.UseVisualStyleBackColor = true;
            this.orignBtn_A.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OrignBtn_A_MouseClick);
            // 
            // angelLb_A
            // 
            this.angelLb_A.AutoSize = true;
            this.angelLb_A.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.angelLb_A.Location = new System.Drawing.Point(81, 55);
            this.angelLb_A.Name = "angelLb_A";
            this.angelLb_A.Size = new System.Drawing.Size(15, 17);
            this.angelLb_A.TabIndex = 37;
            this.angelLb_A.Text = "0";
            // 
            // radioLb_A
            // 
            this.radioLb_A.AutoSize = true;
            this.radioLb_A.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioLb_A.Location = new System.Drawing.Point(81, 26);
            this.radioLb_A.Name = "radioLb_A";
            this.radioLb_A.Size = new System.Drawing.Size(15, 17);
            this.radioLb_A.TabIndex = 36;
            this.radioLb_A.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(6, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 17);
            this.label4.TabIndex = 35;
            this.label4.Text = "角度(度)：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(6, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 17);
            this.label3.TabIndex = 34;
            this.label3.Text = "转速(度/秒)：";
            // 
            // noForwardBtn_A
            // 
            this.noForwardBtn_A.Location = new System.Drawing.Point(166, 95);
            this.noForwardBtn_A.Name = "noForwardBtn_A";
            this.noForwardBtn_A.Size = new System.Drawing.Size(74, 29);
            this.noForwardBtn_A.TabIndex = 33;
            this.noForwardBtn_A.Text = "反传";
            this.noForwardBtn_A.UseVisualStyleBackColor = true;
            this.noForwardBtn_A.Click += new System.EventHandler(this.NoForwardBtn_A_Click);
            // 
            // forwardBtn_A
            // 
            this.forwardBtn_A.Location = new System.Drawing.Point(86, 95);
            this.forwardBtn_A.Name = "forwardBtn_A";
            this.forwardBtn_A.Size = new System.Drawing.Size(74, 29);
            this.forwardBtn_A.TabIndex = 32;
            this.forwardBtn_A.Text = "正传";
            this.forwardBtn_A.UseVisualStyleBackColor = true;
            this.forwardBtn_A.Click += new System.EventHandler(this.ForwardBtn_A_Click);
            // 
            // powerBtn_A
            // 
            this.powerBtn_A.BackColor = System.Drawing.SystemColors.Control;
            this.powerBtn_A.Location = new System.Drawing.Point(6, 95);
            this.powerBtn_A.Name = "powerBtn_A";
            this.powerBtn_A.Size = new System.Drawing.Size(74, 29);
            this.powerBtn_A.TabIndex = 29;
            this.powerBtn_A.Text = "伺服开关";
            this.powerBtn_A.UseVisualStyleBackColor = false;
            this.powerBtn_A.Click += new System.EventHandler(this.PowerSwitchBtn_A_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.angleBtn_L);
            this.groupBox3.Controls.Add(this.angleTb_L);
            this.groupBox3.Controls.Add(this.radioBtn_L);
            this.groupBox3.Controls.Add(this.radioTb_L);
            this.groupBox3.Controls.Add(this.shutdownBtn_L);
            this.groupBox3.Controls.Add(this.autoRunBtn_L);
            this.groupBox3.Controls.Add(this.backOrignBtn_L);
            this.groupBox3.Controls.Add(this.orignBtn_L);
            this.groupBox3.Controls.Add(this.angelLb_L);
            this.groupBox3.Controls.Add(this.radioLb_L);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.noForwardBtn_L);
            this.groupBox3.Controls.Add(this.forwardBtn_L);
            this.groupBox3.Controls.Add(this.powerBtn_L);
            this.groupBox3.Location = new System.Drawing.Point(12, 352);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(327, 185);
            this.groupBox3.TabIndex = 29;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "伺服电机L通讯";
            // 
            // angleBtn_L
            // 
            this.angleBtn_L.Location = new System.Drawing.Point(246, 52);
            this.angleBtn_L.Name = "angleBtn_L";
            this.angleBtn_L.Size = new System.Drawing.Size(74, 29);
            this.angleBtn_L.TabIndex = 49;
            this.angleBtn_L.Text = "写入范围";
            this.angleBtn_L.UseVisualStyleBackColor = true;
            this.angleBtn_L.Click += new System.EventHandler(this.AngleBtn_L_Click);
            // 
            // angleTb_L
            // 
            this.angleTb_L.Location = new System.Drawing.Point(167, 55);
            this.angleTb_L.Name = "angleTb_L";
            this.angleTb_L.Size = new System.Drawing.Size(73, 21);
            this.angleTb_L.TabIndex = 48;
            // 
            // radioBtn_L
            // 
            this.radioBtn_L.Location = new System.Drawing.Point(246, 20);
            this.radioBtn_L.Name = "radioBtn_L";
            this.radioBtn_L.Size = new System.Drawing.Size(74, 29);
            this.radioBtn_L.TabIndex = 47;
            this.radioBtn_L.Text = "写入";
            this.radioBtn_L.UseVisualStyleBackColor = true;
            this.radioBtn_L.Click += new System.EventHandler(this.RadioBtn_L_Click);
            // 
            // radioTb_L
            // 
            this.radioTb_L.Location = new System.Drawing.Point(167, 24);
            this.radioTb_L.Name = "radioTb_L";
            this.radioTb_L.Size = new System.Drawing.Size(73, 21);
            this.radioTb_L.TabIndex = 46;
            // 
            // shutdownBtn_L
            // 
            this.shutdownBtn_L.Location = new System.Drawing.Point(6, 133);
            this.shutdownBtn_L.Name = "shutdownBtn_L";
            this.shutdownBtn_L.Size = new System.Drawing.Size(74, 29);
            this.shutdownBtn_L.TabIndex = 41;
            this.shutdownBtn_L.Text = "紧急停止";
            this.shutdownBtn_L.UseVisualStyleBackColor = true;
            this.shutdownBtn_L.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ShutdownBtn_L_MouseDown);
            this.shutdownBtn_L.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ShutdownBtn_L_MouseUp);
            // 
            // autoRunBtn_L
            // 
            this.autoRunBtn_L.Location = new System.Drawing.Point(246, 98);
            this.autoRunBtn_L.Name = "autoRunBtn_L";
            this.autoRunBtn_L.Size = new System.Drawing.Size(74, 29);
            this.autoRunBtn_L.TabIndex = 40;
            this.autoRunBtn_L.Text = "自动运行";
            this.autoRunBtn_L.UseVisualStyleBackColor = true;
            this.autoRunBtn_L.Click += new System.EventHandler(this.AutoRunBtn_L_Click);
            // 
            // backOrignBtn_L
            // 
            this.backOrignBtn_L.Location = new System.Drawing.Point(166, 133);
            this.backOrignBtn_L.Name = "backOrignBtn_L";
            this.backOrignBtn_L.Size = new System.Drawing.Size(74, 29);
            this.backOrignBtn_L.TabIndex = 39;
            this.backOrignBtn_L.Text = "回原点";
            this.backOrignBtn_L.UseVisualStyleBackColor = true;
            this.backOrignBtn_L.Click += new System.EventHandler(this.BackOrignBtn_L_Click);
            // 
            // orignBtn_L
            // 
            this.orignBtn_L.Location = new System.Drawing.Point(86, 133);
            this.orignBtn_L.Name = "orignBtn_L";
            this.orignBtn_L.Size = new System.Drawing.Size(74, 29);
            this.orignBtn_L.TabIndex = 38;
            this.orignBtn_L.Text = "原点";
            this.orignBtn_L.UseVisualStyleBackColor = true;
            this.orignBtn_L.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OrignBtn_L_MouseClick);
            // 
            // angelLb_L
            // 
            this.angelLb_L.AutoSize = true;
            this.angelLb_L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.angelLb_L.Location = new System.Drawing.Point(81, 56);
            this.angelLb_L.Name = "angelLb_L";
            this.angelLb_L.Size = new System.Drawing.Size(15, 17);
            this.angelLb_L.TabIndex = 37;
            this.angelLb_L.Text = "0";
            // 
            // radioLb_L
            // 
            this.radioLb_L.AutoSize = true;
            this.radioLb_L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioLb_L.Location = new System.Drawing.Point(81, 27);
            this.radioLb_L.Name = "radioLb_L";
            this.radioLb_L.Size = new System.Drawing.Size(15, 17);
            this.radioLb_L.TabIndex = 36;
            this.radioLb_L.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(6, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 17);
            this.label8.TabIndex = 35;
            this.label8.Text = "位移(mm)：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(6, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 17);
            this.label9.TabIndex = 34;
            this.label9.Text = "转速(mm/s)：";
            // 
            // noForwardBtn_L
            // 
            this.noForwardBtn_L.Location = new System.Drawing.Point(166, 98);
            this.noForwardBtn_L.Name = "noForwardBtn_L";
            this.noForwardBtn_L.Size = new System.Drawing.Size(74, 29);
            this.noForwardBtn_L.TabIndex = 33;
            this.noForwardBtn_L.Text = "后退";
            this.noForwardBtn_L.UseVisualStyleBackColor = true;
            this.noForwardBtn_L.Click += new System.EventHandler(this.NoForwardBtn_L_Click);
            // 
            // forwardBtn_L
            // 
            this.forwardBtn_L.Location = new System.Drawing.Point(86, 98);
            this.forwardBtn_L.Name = "forwardBtn_L";
            this.forwardBtn_L.Size = new System.Drawing.Size(74, 29);
            this.forwardBtn_L.TabIndex = 32;
            this.forwardBtn_L.Text = "前进";
            this.forwardBtn_L.UseVisualStyleBackColor = true;
            this.forwardBtn_L.Click += new System.EventHandler(this.ForwardBtn_L_Click);
            this.forwardBtn_L.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ForwardBtn_L_MouseClick);
            // 
            // powerBtn_L
            // 
            this.powerBtn_L.BackColor = System.Drawing.SystemColors.Control;
            this.powerBtn_L.Location = new System.Drawing.Point(6, 98);
            this.powerBtn_L.Name = "powerBtn_L";
            this.powerBtn_L.Size = new System.Drawing.Size(74, 29);
            this.powerBtn_L.TabIndex = 29;
            this.powerBtn_L.Text = "伺服开关";
            this.powerBtn_L.UseVisualStyleBackColor = false;
            this.powerBtn_L.Click += new System.EventHandler(this.PowerSwitchBtn_L_Click);
            // 
            // outInfoTb
            // 
            this.outInfoTb.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.outInfoTb.Location = new System.Drawing.Point(357, 12);
            this.outInfoTb.Multiline = true;
            this.outInfoTb.Name = "outInfoTb";
            this.outInfoTb.ReadOnly = true;
            this.outInfoTb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outInfoTb.Size = new System.Drawing.Size(234, 525);
            this.outInfoTb.TabIndex = 30;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(18, 556);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(164, 29);
            this.button3.TabIndex = 41;
            this.button3.Text = "导出角度与时间数据";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(188, 556);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(164, 29);
            this.button4.TabIndex = 42;
            this.button4.Text = "导出温度与时间数据";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.Button4_Click);
            // 
            // FormConnectValueSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 597);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.outInfoTb);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormConnectValueSetting";
            this.ShowIcon = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConnectValueSetting_FormClosing);
            this.Load += new System.EventHandler(this.FormConnectValueSetting_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button powerBtn_A;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button noForwardBtn_A;
        private System.Windows.Forms.Button forwardBtn_A;
        private System.Windows.Forms.Label angelLb_A;
        private System.Windows.Forms.Label radioLb_A;
        private System.Windows.Forms.Button orignBtn_A;
        private System.Windows.Forms.Button backOrignBtn_A;
        private System.Windows.Forms.Button shutdownBtn_A;
        private System.Windows.Forms.Button autoRunBtn_A;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button shutdownBtn_L;
        private System.Windows.Forms.Button autoRunBtn_L;
        private System.Windows.Forms.Button backOrignBtn_L;
        private System.Windows.Forms.Button orignBtn_L;
        private System.Windows.Forms.Label angelLb_L;
        private System.Windows.Forms.Label radioLb_L;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button noForwardBtn_L;
        private System.Windows.Forms.Button forwardBtn_L;
        private System.Windows.Forms.Button powerBtn_L;
        private System.Windows.Forms.TextBox outInfoTb;
        private System.Windows.Forms.Button autoFind_A;
        private System.Windows.Forms.Button angleBtn_A;
        private System.Windows.Forms.TextBox angleTb_A;
        private System.Windows.Forms.Button radioBtn_A;
        private System.Windows.Forms.TextBox radioTb_A;
        private System.Windows.Forms.Button angleBtn_L;
        private System.Windows.Forms.TextBox angleTb_L;
        private System.Windows.Forms.Button radioBtn_L;
        private System.Windows.Forms.TextBox radioTb_L;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}