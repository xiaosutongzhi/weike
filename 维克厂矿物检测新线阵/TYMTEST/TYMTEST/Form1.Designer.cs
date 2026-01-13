namespace TYMTest
{
    partial class Form1
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.label1 = new System.Windows.Forms.Label();
            this.tbx_sdkversion = new System.Windows.Forms.TextBox();
            this.btn_init = new System.Windows.Forms.Button();
            this.btn_cleanup = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbx_status = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbx_daqip = new System.Windows.Forms.TextBox();
            this.tbx_hostip = new System.Windows.Forms.TextBox();
            this.tbx_cmdport = new System.Windows.Forms.TextBox();
            this.tbx_imgport = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.header_checkbox = new System.Windows.Forms.CheckBox();
            this.tbx_blockheight = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tbx_frameheight = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.cbx_energymode = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tbx_pixelsperdetector = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btn_save_config = new System.Windows.Forms.Button();
            this.tbx_pixelnumber = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_preset = new System.Windows.Forms.Button();
            this.panel_presetting = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_getgainlist = new System.Windows.Forms.Button();
            this.cbx_highgain = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cbx_cardno_high = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.btn_setting = new System.Windows.Forms.Button();
            this.cbx_opmode = new System.Windows.Forms.ComboBox();
            this.lbl_opmode = new System.Windows.Forms.Label();
            this.tbx_delaytime = new System.Windows.Forms.TextBox();
            this.lbl_delaytime = new System.Windows.Forms.Label();
            this.tbx_triggertime = new System.Windows.Forms.TextBox();
            this.lbl_triggertime = new System.Windows.Forms.Label();
            this.cbx_lowgain = new System.Windows.Forms.ComboBox();
            this.lbl_lowgain = new System.Windows.Forms.Label();
            this.cbx_cardno_low = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbx_integrationTime = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cbx_triggermode = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbx_testpattern = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.gropbox_img = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox_Img = new System.Windows.Forms.PictureBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btn_stopgrab = new System.Windows.Forms.Button();
            this.btn_startgrab = new System.Windows.Forms.Button();
            this.tbx_status_callback = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gropbox_img.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Img)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "SDK Version";
            // 
            // tbx_sdkversion
            // 
            this.tbx_sdkversion.Enabled = false;
            this.tbx_sdkversion.Location = new System.Drawing.Point(136, 26);
            this.tbx_sdkversion.Margin = new System.Windows.Forms.Padding(4);
            this.tbx_sdkversion.Name = "tbx_sdkversion";
            this.tbx_sdkversion.Size = new System.Drawing.Size(107, 25);
            this.tbx_sdkversion.TabIndex = 1;
            // 
            // btn_init
            // 
            this.btn_init.Location = new System.Drawing.Point(7, 188);
            this.btn_init.Margin = new System.Windows.Forms.Padding(4);
            this.btn_init.Name = "btn_init";
            this.btn_init.Size = new System.Drawing.Size(151, 29);
            this.btn_init.TabIndex = 2;
            this.btn_init.Text = "Init";
            this.btn_init.UseVisualStyleBackColor = true;
            this.btn_init.Click += new System.EventHandler(this.btn_init_Click);
            // 
            // btn_cleanup
            // 
            this.btn_cleanup.Location = new System.Drawing.Point(175, 188);
            this.btn_cleanup.Margin = new System.Windows.Forms.Padding(4);
            this.btn_cleanup.Name = "btn_cleanup";
            this.btn_cleanup.Size = new System.Drawing.Size(144, 29);
            this.btn_cleanup.TabIndex = 3;
            this.btn_cleanup.Text = "Clean Up";
            this.btn_cleanup.UseVisualStyleBackColor = true;
            this.btn_cleanup.Click += new System.EventHandler(this.btn_cleanup_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 231);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Status:";
            // 
            // tbx_status
            // 
            this.tbx_status.Enabled = false;
            this.tbx_status.Location = new System.Drawing.Point(87, 228);
            this.tbx_status.Margin = new System.Windows.Forms.Padding(4);
            this.tbx_status.Name = "tbx_status";
            this.tbx_status.Size = new System.Drawing.Size(231, 25);
            this.tbx_status.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 35);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "DAQ IP:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 74);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Host IP:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 114);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "Cmd Port:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 158);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 15);
            this.label6.TabIndex = 9;
            this.label6.Text = "Img Port:";
            // 
            // tbx_daqip
            // 
            this.tbx_daqip.Location = new System.Drawing.Point(87, 31);
            this.tbx_daqip.Margin = new System.Windows.Forms.Padding(4);
            this.tbx_daqip.Name = "tbx_daqip";
            this.tbx_daqip.Size = new System.Drawing.Size(231, 25);
            this.tbx_daqip.TabIndex = 10;
            // 
            // tbx_hostip
            // 
            this.tbx_hostip.Location = new System.Drawing.Point(87, 70);
            this.tbx_hostip.Margin = new System.Windows.Forms.Padding(4);
            this.tbx_hostip.Name = "tbx_hostip";
            this.tbx_hostip.Size = new System.Drawing.Size(231, 25);
            this.tbx_hostip.TabIndex = 11;
            // 
            // tbx_cmdport
            // 
            this.tbx_cmdport.Location = new System.Drawing.Point(87, 110);
            this.tbx_cmdport.Margin = new System.Windows.Forms.Padding(4);
            this.tbx_cmdport.Name = "tbx_cmdport";
            this.tbx_cmdport.Size = new System.Drawing.Size(231, 25);
            this.tbx_cmdport.TabIndex = 12;
            // 
            // tbx_imgport
            // 
            this.tbx_imgport.Location = new System.Drawing.Point(87, 154);
            this.tbx_imgport.Margin = new System.Windows.Forms.Padding(4);
            this.tbx_imgport.Name = "tbx_imgport";
            this.tbx_imgport.Size = new System.Drawing.Size(231, 25);
            this.tbx_imgport.TabIndex = 13;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbx_daqip);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbx_status);
            this.groupBox1.Controls.Add(this.tbx_imgport);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btn_cleanup);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btn_init);
            this.groupBox1.Controls.Add(this.tbx_cmdport);
            this.groupBox1.Controls.Add(this.tbx_hostip);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(36, 60);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(335, 269);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Init Area";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.header_checkbox);
            this.groupBox2.Controls.Add(this.tbx_blockheight);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.tbx_frameheight);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.cbx_energymode);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.tbx_pixelsperdetector);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.btn_save_config);
            this.groupBox2.Controls.Add(this.tbx_pixelnumber);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.btn_preset);
            this.groupBox2.Controls.Add(this.panel_presetting);
            this.groupBox2.Location = new System.Drawing.Point(408, 15);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(1077, 235);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "PreSetting Area";
            // 
            // header_checkbox
            // 
            this.header_checkbox.AutoSize = true;
            this.header_checkbox.Location = new System.Drawing.Point(912, 161);
            this.header_checkbox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.header_checkbox.Name = "header_checkbox";
            this.header_checkbox.Size = new System.Drawing.Size(77, 19);
            this.header_checkbox.TabIndex = 13;
            this.header_checkbox.Text = "header";
            this.header_checkbox.UseVisualStyleBackColor = true;
            this.header_checkbox.CheckedChanged += new System.EventHandler(this.header_checkbox_CheckedChanged);
            // 
            // tbx_blockheight
            // 
            this.tbx_blockheight.Location = new System.Drawing.Point(332, 140);
            this.tbx_blockheight.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbx_blockheight.Name = "tbx_blockheight";
            this.tbx_blockheight.Size = new System.Drawing.Size(76, 25);
            this.tbx_blockheight.TabIndex = 12;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(223, 142);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(103, 15);
            this.label17.TabIndex = 11;
            this.label17.Text = "BlockHeight:";
            // 
            // tbx_frameheight
            // 
            this.tbx_frameheight.Location = new System.Drawing.Point(132, 140);
            this.tbx_frameheight.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbx_frameheight.Name = "tbx_frameheight";
            this.tbx_frameheight.Size = new System.Drawing.Size(76, 25);
            this.tbx_frameheight.TabIndex = 10;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(23, 142);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(103, 15);
            this.label16.TabIndex = 9;
            this.label16.Text = "FrameHeight:";
            // 
            // cbx_energymode
            // 
            this.cbx_energymode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_energymode.FormattingEnabled = true;
            this.cbx_energymode.Items.AddRange(new object[] {
            "Single Energy",
            "Dual Energy"});
            this.cbx_energymode.Location = new System.Drawing.Point(373, 92);
            this.cbx_energymode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbx_energymode.Name = "cbx_energymode";
            this.cbx_energymode.Size = new System.Drawing.Size(144, 23);
            this.cbx_energymode.TabIndex = 8;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(272, 96);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(95, 15);
            this.label15.TabIndex = 7;
            this.label15.Text = "EnergyMode:";
            // 
            // tbx_pixelsperdetector
            // 
            this.tbx_pixelsperdetector.Location = new System.Drawing.Point(180, 92);
            this.tbx_pixelsperdetector.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbx_pixelsperdetector.Name = "tbx_pixelsperdetector";
            this.tbx_pixelsperdetector.Size = new System.Drawing.Size(76, 25);
            this.tbx_pixelsperdetector.TabIndex = 6;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(23, 96);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(151, 15);
            this.label11.TabIndex = 5;
            this.label11.Text = "PixelsPerDetector:";
            // 
            // btn_save_config
            // 
            this.btn_save_config.Location = new System.Drawing.Point(912, 195);
            this.btn_save_config.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_save_config.Name = "btn_save_config";
            this.btn_save_config.Size = new System.Drawing.Size(128, 22);
            this.btn_save_config.TabIndex = 4;
            this.btn_save_config.Text = "Save Config";
            this.btn_save_config.UseVisualStyleBackColor = true;
            this.btn_save_config.Click += new System.EventHandler(this.btn_save_config_Click);
            // 
            // tbx_pixelnumber
            // 
            this.tbx_pixelnumber.Enabled = false;
            this.tbx_pixelnumber.Location = new System.Drawing.Point(363, 200);
            this.tbx_pixelnumber.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbx_pixelnumber.Name = "tbx_pixelnumber";
            this.tbx_pixelnumber.Size = new System.Drawing.Size(100, 25);
            this.tbx_pixelnumber.TabIndex = 3;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(205, 202);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(151, 15);
            this.label12.TabIndex = 2;
            this.label12.Text = "Total PixelNumber:";
            // 
            // btn_preset
            // 
            this.btn_preset.Location = new System.Drawing.Point(23, 195);
            this.btn_preset.Margin = new System.Windows.Forms.Padding(4);
            this.btn_preset.Name = "btn_preset";
            this.btn_preset.Size = new System.Drawing.Size(175, 29);
            this.btn_preset.TabIndex = 1;
            this.btn_preset.Text = "Set";
            this.btn_preset.UseVisualStyleBackColor = true;
            this.btn_preset.Click += new System.EventHandler(this.btn_preset_Click);
            // 
            // panel_presetting
            // 
            this.panel_presetting.Location = new System.Drawing.Point(23, 45);
            this.panel_presetting.Margin = new System.Windows.Forms.Padding(4);
            this.panel_presetting.Name = "panel_presetting";
            this.panel_presetting.Size = new System.Drawing.Size(1017, 34);
            this.panel_presetting.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_getgainlist);
            this.groupBox3.Controls.Add(this.cbx_highgain);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.cbx_cardno_high);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.btn_setting);
            this.groupBox3.Controls.Add(this.cbx_opmode);
            this.groupBox3.Controls.Add(this.lbl_opmode);
            this.groupBox3.Controls.Add(this.tbx_delaytime);
            this.groupBox3.Controls.Add(this.lbl_delaytime);
            this.groupBox3.Controls.Add(this.tbx_triggertime);
            this.groupBox3.Controls.Add(this.lbl_triggertime);
            this.groupBox3.Controls.Add(this.cbx_lowgain);
            this.groupBox3.Controls.Add(this.lbl_lowgain);
            this.groupBox3.Controls.Add(this.cbx_cardno_low);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.tbx_integrationTime);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.cbx_triggermode);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.cbx_testpattern);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(36, 352);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(335, 491);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Setting Area";
            // 
            // btn_getgainlist
            // 
            this.btn_getgainlist.Location = new System.Drawing.Point(15, 378);
            this.btn_getgainlist.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_getgainlist.Name = "btn_getgainlist";
            this.btn_getgainlist.Size = new System.Drawing.Size(120, 28);
            this.btn_getgainlist.TabIndex = 21;
            this.btn_getgainlist.Text = "Get GainList";
            this.btn_getgainlist.UseVisualStyleBackColor = true;
            this.btn_getgainlist.Click += new System.EventHandler(this.btn_getgainlist_Click);
            // 
            // cbx_highgain
            // 
            this.cbx_highgain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_highgain.FormattingEnabled = true;
            this.cbx_highgain.Location = new System.Drawing.Point(235, 338);
            this.cbx_highgain.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_highgain.Name = "cbx_highgain";
            this.cbx_highgain.Size = new System.Drawing.Size(69, 23);
            this.cbx_highgain.TabIndex = 20;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(159, 341);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(79, 15);
            this.label13.TabIndex = 19;
            this.label13.Text = "HighGain:";
            // 
            // cbx_cardno_high
            // 
            this.cbx_cardno_high.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_cardno_high.FormattingEnabled = true;
            this.cbx_cardno_high.Location = new System.Drawing.Point(88, 338);
            this.cbx_cardno_high.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_cardno_high.Name = "cbx_cardno_high";
            this.cbx_cardno_high.Size = new System.Drawing.Size(69, 23);
            this.cbx_cardno_high.TabIndex = 18;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 341);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 15);
            this.label14.TabIndex = 17;
            this.label14.Text = "Card No:";
            // 
            // btn_setting
            // 
            this.btn_setting.Location = new System.Drawing.Point(161, 378);
            this.btn_setting.Margin = new System.Windows.Forms.Padding(4);
            this.btn_setting.Name = "btn_setting";
            this.btn_setting.Size = new System.Drawing.Size(143, 29);
            this.btn_setting.TabIndex = 16;
            this.btn_setting.Text = "Set";
            this.btn_setting.UseVisualStyleBackColor = true;
            this.btn_setting.Click += new System.EventHandler(this.btn_setting_Click);
            // 
            // cbx_opmode
            // 
            this.cbx_opmode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_opmode.FormattingEnabled = true;
            this.cbx_opmode.Items.AddRange(new object[] {
            "non-continuous mode",
            "constant integration mode",
            "pixeloffset mode"});
            this.cbx_opmode.Location = new System.Drawing.Point(161, 262);
            this.cbx_opmode.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_opmode.Name = "cbx_opmode";
            this.cbx_opmode.Size = new System.Drawing.Size(141, 23);
            this.cbx_opmode.TabIndex = 15;
            this.cbx_opmode.Visible = false;
            // 
            // lbl_opmode
            // 
            this.lbl_opmode.AutoSize = true;
            this.lbl_opmode.Location = new System.Drawing.Point(16, 262);
            this.lbl_opmode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_opmode.Name = "lbl_opmode";
            this.lbl_opmode.Size = new System.Drawing.Size(127, 15);
            this.lbl_opmode.TabIndex = 14;
            this.lbl_opmode.Text = "Operation Mode:";
            this.lbl_opmode.Visible = false;
            // 
            // tbx_delaytime
            // 
            this.tbx_delaytime.Location = new System.Drawing.Point(161, 224);
            this.tbx_delaytime.Margin = new System.Windows.Forms.Padding(4);
            this.tbx_delaytime.Name = "tbx_delaytime";
            this.tbx_delaytime.Size = new System.Drawing.Size(141, 25);
            this.tbx_delaytime.TabIndex = 13;
            this.tbx_delaytime.Visible = false;
            // 
            // lbl_delaytime
            // 
            this.lbl_delaytime.AutoSize = true;
            this.lbl_delaytime.Location = new System.Drawing.Point(15, 228);
            this.lbl_delaytime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_delaytime.Name = "lbl_delaytime";
            this.lbl_delaytime.Size = new System.Drawing.Size(95, 15);
            this.lbl_delaytime.TabIndex = 12;
            this.lbl_delaytime.Text = "Delay Time:";
            this.lbl_delaytime.Visible = false;
            // 
            // tbx_triggertime
            // 
            this.tbx_triggertime.Location = new System.Drawing.Point(161, 184);
            this.tbx_triggertime.Margin = new System.Windows.Forms.Padding(4);
            this.tbx_triggertime.Name = "tbx_triggertime";
            this.tbx_triggertime.Size = new System.Drawing.Size(141, 25);
            this.tbx_triggertime.TabIndex = 11;
            this.tbx_triggertime.Visible = false;
            // 
            // lbl_triggertime
            // 
            this.lbl_triggertime.AutoSize = true;
            this.lbl_triggertime.Location = new System.Drawing.Point(16, 188);
            this.lbl_triggertime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_triggertime.Name = "lbl_triggertime";
            this.lbl_triggertime.Size = new System.Drawing.Size(111, 15);
            this.lbl_triggertime.TabIndex = 10;
            this.lbl_triggertime.Text = "Trigger Time:";
            this.lbl_triggertime.Visible = false;
            // 
            // cbx_lowgain
            // 
            this.cbx_lowgain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_lowgain.FormattingEnabled = true;
            this.cbx_lowgain.Location = new System.Drawing.Point(233, 299);
            this.cbx_lowgain.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_lowgain.Name = "cbx_lowgain";
            this.cbx_lowgain.Size = new System.Drawing.Size(69, 23);
            this.cbx_lowgain.TabIndex = 9;
            // 
            // lbl_lowgain
            // 
            this.lbl_lowgain.AutoSize = true;
            this.lbl_lowgain.Location = new System.Drawing.Point(164, 302);
            this.lbl_lowgain.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_lowgain.Name = "lbl_lowgain";
            this.lbl_lowgain.Size = new System.Drawing.Size(71, 15);
            this.lbl_lowgain.TabIndex = 8;
            this.lbl_lowgain.Text = "LowGain:";
            // 
            // cbx_cardno_low
            // 
            this.cbx_cardno_low.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_cardno_low.FormattingEnabled = true;
            this.cbx_cardno_low.Location = new System.Drawing.Point(87, 299);
            this.cbx_cardno_low.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_cardno_low.Name = "cbx_cardno_low";
            this.cbx_cardno_low.Size = new System.Drawing.Size(69, 23);
            this.cbx_cardno_low.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 308);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 15);
            this.label10.TabIndex = 6;
            this.label10.Text = "Card No:";
            // 
            // tbx_integrationTime
            // 
            this.tbx_integrationTime.Location = new System.Drawing.Point(161, 141);
            this.tbx_integrationTime.Margin = new System.Windows.Forms.Padding(4);
            this.tbx_integrationTime.Name = "tbx_integrationTime";
            this.tbx_integrationTime.Size = new System.Drawing.Size(141, 25);
            this.tbx_integrationTime.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 145);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(143, 15);
            this.label9.TabIndex = 4;
            this.label9.Text = "Integration Time:";
            // 
            // cbx_triggermode
            // 
            this.cbx_triggermode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_triggermode.FormattingEnabled = true;
            this.cbx_triggermode.Items.AddRange(new object[] {
            "Internal",
            "External"});
            this.cbx_triggermode.Location = new System.Drawing.Point(133, 89);
            this.cbx_triggermode.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_triggermode.Name = "cbx_triggermode";
            this.cbx_triggermode.Size = new System.Drawing.Size(169, 23);
            this.cbx_triggermode.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 92);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(111, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "Trigger Mode:";
            // 
            // cbx_testpattern
            // 
            this.cbx_testpattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_testpattern.FormattingEnabled = true;
            this.cbx_testpattern.Location = new System.Drawing.Point(133, 38);
            this.cbx_testpattern.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_testpattern.Name = "cbx_testpattern";
            this.cbx_testpattern.Size = new System.Drawing.Size(169, 23);
            this.cbx_testpattern.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 41);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(111, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "Test Pattern:";
            // 
            // gropbox_img
            // 
            this.gropbox_img.Controls.Add(this.panel1);
            this.gropbox_img.Controls.Add(this.chart1);
            this.gropbox_img.Controls.Add(this.btn_stopgrab);
            this.gropbox_img.Controls.Add(this.btn_startgrab);
            this.gropbox_img.Location = new System.Drawing.Point(408, 269);
            this.gropbox_img.Margin = new System.Windows.Forms.Padding(4);
            this.gropbox_img.Name = "gropbox_img";
            this.gropbox_img.Padding = new System.Windows.Forms.Padding(4);
            this.gropbox_img.Size = new System.Drawing.Size(1077, 489);
            this.gropbox_img.TabIndex = 17;
            this.gropbox_img.TabStop = false;
            this.gropbox_img.Text = "Image Area";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox_Img);
            this.panel1.Location = new System.Drawing.Point(8, 61);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(660, 398);
            this.panel1.TabIndex = 5;
            // 
            // pictureBox_Img
            // 
            this.pictureBox_Img.Location = new System.Drawing.Point(4, 4);
            this.pictureBox_Img.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox_Img.Name = "pictureBox_Img";
            this.pictureBox_Img.Size = new System.Drawing.Size(651, 382);
            this.pictureBox_Img.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Img.TabIndex = 2;
            this.pictureBox_Img.TabStop = false;
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(675, 61);
            this.chart1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chart1.Name = "chart1";
            series1.BorderWidth = 2;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Average";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(396, 300);
            this.chart1.TabIndex = 4;
            this.chart1.Text = "chart1";
            // 
            // btn_stopgrab
            // 
            this.btn_stopgrab.Location = new System.Drawing.Point(116, 25);
            this.btn_stopgrab.Margin = new System.Windows.Forms.Padding(4);
            this.btn_stopgrab.Name = "btn_stopgrab";
            this.btn_stopgrab.Size = new System.Drawing.Size(100, 29);
            this.btn_stopgrab.TabIndex = 1;
            this.btn_stopgrab.Text = "StopGrab";
            this.btn_stopgrab.UseVisualStyleBackColor = true;
            this.btn_stopgrab.Click += new System.EventHandler(this.btn_stopgrab_Click);
            // 
            // btn_startgrab
            // 
            this.btn_startgrab.Location = new System.Drawing.Point(8, 25);
            this.btn_startgrab.Margin = new System.Windows.Forms.Padding(4);
            this.btn_startgrab.Name = "btn_startgrab";
            this.btn_startgrab.Size = new System.Drawing.Size(100, 29);
            this.btn_startgrab.TabIndex = 0;
            this.btn_startgrab.Text = "StartGrab";
            this.btn_startgrab.UseVisualStyleBackColor = true;
            this.btn_startgrab.Click += new System.EventHandler(this.btn_startgrab_Click);
            // 
            // tbx_status_callback
            // 
            this.tbx_status_callback.Location = new System.Drawing.Point(408, 765);
            this.tbx_status_callback.Multiline = true;
            this.tbx_status_callback.Name = "tbx_status_callback";
            this.tbx_status_callback.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbx_status_callback.Size = new System.Drawing.Size(668, 79);
            this.tbx_status_callback.TabIndex = 18;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1499, 856);
            this.Controls.Add(this.tbx_status_callback);
            this.Controls.Add(this.gropbox_img);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tbx_sdkversion);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "TestDemo_C#V1.3.0";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gropbox_img.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Img)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbx_sdkversion;
        private System.Windows.Forms.Button btn_init;
        private System.Windows.Forms.Button btn_cleanup;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbx_status;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbx_daqip;
        private System.Windows.Forms.TextBox tbx_hostip;
        private System.Windows.Forms.TextBox tbx_cmdport;
        private System.Windows.Forms.TextBox tbx_imgport;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel_presetting;
        private System.Windows.Forms.Button btn_preset;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbx_integrationTime;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbx_triggermode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbx_testpattern;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btn_setting;
        private System.Windows.Forms.ComboBox cbx_opmode;
        private System.Windows.Forms.Label lbl_opmode;
        private System.Windows.Forms.TextBox tbx_delaytime;
        private System.Windows.Forms.Label lbl_delaytime;
        private System.Windows.Forms.TextBox tbx_triggertime;
        private System.Windows.Forms.Label lbl_triggertime;
        private System.Windows.Forms.ComboBox cbx_lowgain;
        private System.Windows.Forms.Label lbl_lowgain;
        private System.Windows.Forms.ComboBox cbx_cardno_low;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox gropbox_img;
        private System.Windows.Forms.Button btn_stopgrab;
        private System.Windows.Forms.Button btn_startgrab;
        private System.Windows.Forms.PictureBox pictureBox_Img;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbx_pixelnumber;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cbx_highgain;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cbx_cardno_high;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btn_getgainlist;
        private System.Windows.Forms.Button btn_save_config;
        private System.Windows.Forms.ComboBox cbx_energymode;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tbx_pixelsperdetector;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbx_blockheight;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox tbx_frameheight;
        private System.Windows.Forms.CheckBox header_checkbox;
        private System.Windows.Forms.TextBox tbx_status_callback;
    }
}

