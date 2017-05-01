﻿namespace PCController
{
    partial class ControllerForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tB_SyntecIP = new System.Windows.Forms.TextBox();
            this.bt_SyntecConnect = new System.Windows.Forms.Button();
            this.bt_upload = new System.Windows.Forms.Button();
            this.tB_fileName = new System.Windows.Forms.TextBox();
            this.bt_cycStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label_state = new System.Windows.Forms.Label();
            this.label_syntecBusy = new System.Windows.Forms.Label();
            this.bt_test = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.num_gvarNo = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label_pos = new System.Windows.Forms.Label();
            this.num_gvarValue = new System.Windows.Forms.NumericUpDown();
            this.bt_writeGVar = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.bt_readGVar = new System.Windows.Forms.Button();
            this.bt_genNC = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tB_TRCIP = new System.Windows.Forms.TextBox();
            this.tB_TRCPort = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.bt_TRCConnect = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panelTestFunc = new System.Windows.Forms.Panel();
            this.bt_servoSW = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.num_obitNo = new System.Windows.Forms.NumericUpDown();
            this.rBt_obitF = new System.Windows.Forms.RadioButton();
            this.rBt_obitT = new System.Windows.Forms.RadioButton();
            this.bt_writeO = new System.Windows.Forms.Button();
            this.bt_setOrigin = new System.Windows.Forms.Button();
            this.bt_TRCCommStart = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panelJOG = new System.Windows.Forms.FlowLayoutPanel();
            this.bt_JOG1P = new System.Windows.Forms.Button();
            this.bt_JOG1N = new System.Windows.Forms.Button();
            this.bt_JOG2P = new System.Windows.Forms.Button();
            this.bt_JOG2N = new System.Windows.Forms.Button();
            this.bt_JOG3P = new System.Windows.Forms.Button();
            this.bt_JOG3N = new System.Windows.Forms.Button();
            this.bt_JOG4P = new System.Windows.Forms.Button();
            this.bt_JOG4N = new System.Windows.Forms.Button();
            this.tB_mesPrint = new System.Windows.Forms.TextBox();
            this.bt_cycleReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.num_gvarNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_gvarValue)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panelTestFunc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_obitNo)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.panelJOG.SuspendLayout();
            this.SuspendLayout();
            // 
            // tB_SyntecIP
            // 
            this.tB_SyntecIP.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tB_SyntecIP.Location = new System.Drawing.Point(115, 17);
            this.tB_SyntecIP.Name = "tB_SyntecIP";
            this.tB_SyntecIP.Size = new System.Drawing.Size(85, 22);
            this.tB_SyntecIP.TabIndex = 0;
            // 
            // bt_SyntecConnect
            // 
            this.bt_SyntecConnect.Location = new System.Drawing.Point(49, 83);
            this.bt_SyntecConnect.Name = "bt_SyntecConnect";
            this.bt_SyntecConnect.Size = new System.Drawing.Size(174, 23);
            this.bt_SyntecConnect.TabIndex = 1;
            this.bt_SyntecConnect.Text = "Connect to Syntec Controller";
            this.bt_SyntecConnect.UseVisualStyleBackColor = true;
            this.bt_SyntecConnect.Click += new System.EventHandler(this.bt_SyntecConnect_Click);
            // 
            // bt_upload
            // 
            this.bt_upload.Location = new System.Drawing.Point(109, 3);
            this.bt_upload.Name = "bt_upload";
            this.bt_upload.Size = new System.Drawing.Size(75, 23);
            this.bt_upload.TabIndex = 2;
            this.bt_upload.Text = "Upload";
            this.bt_upload.UseVisualStyleBackColor = true;
            this.bt_upload.Click += new System.EventHandler(this.bt_upload_Click);
            // 
            // tB_fileName
            // 
            this.tB_fileName.Location = new System.Drawing.Point(3, 3);
            this.tB_fileName.Name = "tB_fileName";
            this.tB_fileName.Size = new System.Drawing.Size(100, 22);
            this.tB_fileName.TabIndex = 3;
            // 
            // bt_cycStart
            // 
            this.bt_cycStart.Location = new System.Drawing.Point(97, 44);
            this.bt_cycStart.Name = "bt_cycStart";
            this.bt_cycStart.Size = new System.Drawing.Size(100, 23);
            this.bt_cycStart.TabIndex = 4;
            this.bt_cycStart.Text = "Cycle Start / Stop";
            this.bt_cycStart.UseVisualStyleBackColor = true;
            this.bt_cycStart.Click += new System.EventHandler(this.bt_cycStart_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 395);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "Local State: ";
            // 
            // label_state
            // 
            this.label_state.AutoSize = true;
            this.label_state.Location = new System.Drawing.Point(126, 395);
            this.label_state.Name = "label_state";
            this.label_state.Size = new System.Drawing.Size(31, 12);
            this.label_state.TabIndex = 6;
            this.label_state.Text = "ready";
            // 
            // label_syntecBusy
            // 
            this.label_syntecBusy.AutoSize = true;
            this.label_syntecBusy.Location = new System.Drawing.Point(128, 443);
            this.label_syntecBusy.Name = "label_syntecBusy";
            this.label_syntecBusy.Size = new System.Drawing.Size(22, 12);
            this.label_syntecBusy.TabIndex = 7;
            this.label_syntecBusy.Text = "idle";
            // 
            // bt_test
            // 
            this.bt_test.Location = new System.Drawing.Point(209, 301);
            this.bt_test.Name = "bt_test";
            this.bt_test.Size = new System.Drawing.Size(75, 23);
            this.bt_test.TabIndex = 9;
            this.bt_test.Text = "test";
            this.bt_test.UseVisualStyleBackColor = true;
            this.bt_test.Click += new System.EventHandler(this.bt_test_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 443);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "Syntec State: ";
            // 
            // num_gvarNo
            // 
            this.num_gvarNo.Location = new System.Drawing.Point(62, 93);
            this.num_gvarNo.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.num_gvarNo.Name = "num_gvarNo";
            this.num_gvarNo.Size = new System.Drawing.Size(69, 22);
            this.num_gvarNo.TabIndex = 11;
            this.num_gvarNo.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 419);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 12);
            this.label3.TabIndex = 13;
            this.label3.Text = "Current Position: ";
            // 
            // label_pos
            // 
            this.label_pos.AutoSize = true;
            this.label_pos.Location = new System.Drawing.Point(126, 419);
            this.label_pos.Name = "label_pos";
            this.label_pos.Size = new System.Drawing.Size(11, 12);
            this.label_pos.TabIndex = 14;
            this.label_pos.Text = "0";
            // 
            // num_gvarValue
            // 
            this.num_gvarValue.DecimalPlaces = 3;
            this.num_gvarValue.Location = new System.Drawing.Point(178, 93);
            this.num_gvarValue.Name = "num_gvarValue";
            this.num_gvarValue.Size = new System.Drawing.Size(100, 22);
            this.num_gvarValue.TabIndex = 15;
            this.num_gvarValue.Value = new decimal(new int[] {
            1000,
            0,
            0,
            196608});
            // 
            // bt_writeGVar
            // 
            this.bt_writeGVar.Location = new System.Drawing.Point(143, 126);
            this.bt_writeGVar.Name = "bt_writeGVar";
            this.bt_writeGVar.Size = new System.Drawing.Size(75, 23);
            this.bt_writeGVar.TabIndex = 16;
            this.bt_writeGVar.Text = "Write Global";
            this.bt_writeGVar.UseVisualStyleBackColor = true;
            this.bt_writeGVar.Click += new System.EventHandler(this.bt_writeGVar_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 17;
            this.label4.Text = "GlobalNo:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(137, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 12);
            this.label5.TabIndex = 18;
            this.label5.Text = "Value:";
            // 
            // bt_readGVar
            // 
            this.bt_readGVar.Location = new System.Drawing.Point(62, 126);
            this.bt_readGVar.Name = "bt_readGVar";
            this.bt_readGVar.Size = new System.Drawing.Size(75, 23);
            this.bt_readGVar.TabIndex = 19;
            this.bt_readGVar.Text = "Read Global";
            this.bt_readGVar.UseVisualStyleBackColor = true;
            this.bt_readGVar.Click += new System.EventHandler(this.bt_readGVar_Click);
            // 
            // bt_genNC
            // 
            this.bt_genNC.Location = new System.Drawing.Point(209, 272);
            this.bt_genNC.Name = "bt_genNC";
            this.bt_genNC.Size = new System.Drawing.Size(75, 23);
            this.bt_genNC.TabIndex = 20;
            this.bt_genNC.Text = "GenNCCode";
            this.bt_genNC.UseVisualStyleBackColor = true;
            this.bt_genNC.Click += new System.EventHandler(this.bt_genNC_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(320, 356);
            this.tabControl1.TabIndex = 23;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(312, 330);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Connection";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Controls.Add(this.bt_TRCConnect);
            this.groupBox1.Location = new System.Drawing.Point(6, 146);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(297, 170);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "TRC Server Connection";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.61538F));
            this.tableLayoutPanel1.Controls.Add(this.tB_TRCIP, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tB_TRCPort, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 18);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(291, 104);
            this.tableLayoutPanel1.TabIndex = 25;
            // 
            // tB_TRCIP
            // 
            this.tB_TRCIP.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tB_TRCIP.Location = new System.Drawing.Point(89, 15);
            this.tB_TRCIP.Name = "tB_TRCIP";
            this.tB_TRCIP.Size = new System.Drawing.Size(85, 22);
            this.tB_TRCIP.TabIndex = 2;
            // 
            // tB_TRCPort
            // 
            this.tB_TRCPort.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tB_TRCPort.Location = new System.Drawing.Point(89, 67);
            this.tB_TRCPort.Name = "tB_TRCPort";
            this.tB_TRCPort.Size = new System.Drawing.Size(55, 22);
            this.tB_TRCPort.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 12);
            this.label7.TabIndex = 26;
            this.label7.Text = "TRC Server IP: ";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(53, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 12);
            this.label8.TabIndex = 27;
            this.label8.Text = "Port: ";
            // 
            // bt_TRCConnect
            // 
            this.bt_TRCConnect.Location = new System.Drawing.Point(49, 130);
            this.bt_TRCConnect.Name = "bt_TRCConnect";
            this.bt_TRCConnect.Size = new System.Drawing.Size(174, 25);
            this.bt_TRCConnect.TabIndex = 4;
            this.bt_TRCConnect.Text = "Connect to TRC Server";
            this.bt_TRCConnect.UseVisualStyleBackColor = true;
            this.bt_TRCConnect.Click += new System.EventHandler(this.bt_TRCConnect_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bt_SyntecConnect);
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.Location = new System.Drawing.Point(6, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(300, 120);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Syntec Controller Connection";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.84615F));
            this.tableLayoutPanel2.Controls.Add(this.tB_SyntecIP, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 18);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(294, 57);
            this.tableLayoutPanel2.TabIndex = 25;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 12);
            this.label6.TabIndex = 25;
            this.label6.Text = "Syntec Controller IP: ";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panelTestFunc);
            this.tabPage2.Controls.Add(this.bt_TRCCommStart);
            this.tabPage2.Controls.Add(this.bt_test);
            this.tabPage2.Controls.Add(this.bt_genNC);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(312, 330);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Test Functions";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panelTestFunc
            // 
            this.panelTestFunc.Controls.Add(this.bt_cycleReset);
            this.panelTestFunc.Controls.Add(this.bt_servoSW);
            this.panelTestFunc.Controls.Add(this.label9);
            this.panelTestFunc.Controls.Add(this.tB_fileName);
            this.panelTestFunc.Controls.Add(this.num_obitNo);
            this.panelTestFunc.Controls.Add(this.bt_cycStart);
            this.panelTestFunc.Controls.Add(this.rBt_obitF);
            this.panelTestFunc.Controls.Add(this.bt_upload);
            this.panelTestFunc.Controls.Add(this.rBt_obitT);
            this.panelTestFunc.Controls.Add(this.label5);
            this.panelTestFunc.Controls.Add(this.bt_writeO);
            this.panelTestFunc.Controls.Add(this.bt_writeGVar);
            this.panelTestFunc.Controls.Add(this.bt_setOrigin);
            this.panelTestFunc.Controls.Add(this.bt_readGVar);
            this.panelTestFunc.Controls.Add(this.num_gvarValue);
            this.panelTestFunc.Controls.Add(this.label4);
            this.panelTestFunc.Controls.Add(this.num_gvarNo);
            this.panelTestFunc.Enabled = false;
            this.panelTestFunc.Location = new System.Drawing.Point(6, 6);
            this.panelTestFunc.Name = "panelTestFunc";
            this.panelTestFunc.Size = new System.Drawing.Size(300, 260);
            this.panelTestFunc.TabIndex = 26;
            // 
            // bt_servoSW
            // 
            this.bt_servoSW.Location = new System.Drawing.Point(203, 44);
            this.bt_servoSW.Name = "bt_servoSW";
            this.bt_servoSW.Size = new System.Drawing.Size(86, 23);
            this.bt_servoSW.TabIndex = 27;
            this.bt_servoSW.Text = "Servo On / Off";
            this.bt_servoSW.UseVisualStyleBackColor = true;
            this.bt_servoSW.Click += new System.EventHandler(this.bt_servoSW_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(39, 186);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 12);
            this.label9.TabIndex = 26;
            this.label9.Text = "Obit:";
            // 
            // num_obitNo
            // 
            this.num_obitNo.Location = new System.Drawing.Point(73, 181);
            this.num_obitNo.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.num_obitNo.Name = "num_obitNo";
            this.num_obitNo.Size = new System.Drawing.Size(69, 22);
            this.num_obitNo.TabIndex = 25;
            this.num_obitNo.Value = new decimal(new int[] {
            320,
            0,
            0,
            0});
            // 
            // rBt_obitF
            // 
            this.rBt_obitF.AutoSize = true;
            this.rBt_obitF.Checked = true;
            this.rBt_obitF.Location = new System.Drawing.Point(191, 184);
            this.rBt_obitF.Name = "rBt_obitF";
            this.rBt_obitF.Size = new System.Drawing.Size(39, 16);
            this.rBt_obitF.TabIndex = 24;
            this.rBt_obitF.TabStop = true;
            this.rBt_obitF.Text = "Off";
            this.rBt_obitF.UseVisualStyleBackColor = true;
            // 
            // rBt_obitT
            // 
            this.rBt_obitT.AutoSize = true;
            this.rBt_obitT.Location = new System.Drawing.Point(148, 184);
            this.rBt_obitT.Name = "rBt_obitT";
            this.rBt_obitT.Size = new System.Drawing.Size(37, 16);
            this.rBt_obitT.TabIndex = 24;
            this.rBt_obitT.Text = "On";
            this.rBt_obitT.UseVisualStyleBackColor = true;
            // 
            // bt_writeO
            // 
            this.bt_writeO.Location = new System.Drawing.Point(97, 214);
            this.bt_writeO.Name = "bt_writeO";
            this.bt_writeO.Size = new System.Drawing.Size(75, 23);
            this.bt_writeO.TabIndex = 23;
            this.bt_writeO.Text = "Write Obit";
            this.bt_writeO.UseVisualStyleBackColor = true;
            this.bt_writeO.Click += new System.EventHandler(this.bt_writeO_Click);
            // 
            // bt_setOrigin
            // 
            this.bt_setOrigin.Location = new System.Drawing.Point(214, 2);
            this.bt_setOrigin.Name = "bt_setOrigin";
            this.bt_setOrigin.Size = new System.Drawing.Size(75, 23);
            this.bt_setOrigin.TabIndex = 22;
            this.bt_setOrigin.Text = "Set Origin";
            this.bt_setOrigin.UseVisualStyleBackColor = true;
            this.bt_setOrigin.Click += new System.EventHandler(this.bt_setOrigin_Click);
            // 
            // bt_TRCCommStart
            // 
            this.bt_TRCCommStart.Location = new System.Drawing.Point(33, 272);
            this.bt_TRCCommStart.Name = "bt_TRCCommStart";
            this.bt_TRCCommStart.Size = new System.Drawing.Size(157, 23);
            this.bt_TRCCommStart.TabIndex = 21;
            this.bt_TRCCommStart.Text = "Start TRC Communication";
            this.bt_TRCCommStart.UseVisualStyleBackColor = true;
            this.bt_TRCCommStart.Click += new System.EventHandler(this.bt_TRCCommStart_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.panelJOG);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(312, 330);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "JOG";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // panelJOG
            // 
            this.panelJOG.Controls.Add(this.bt_JOG1P);
            this.panelJOG.Controls.Add(this.bt_JOG1N);
            this.panelJOG.Controls.Add(this.bt_JOG2P);
            this.panelJOG.Controls.Add(this.bt_JOG2N);
            this.panelJOG.Controls.Add(this.bt_JOG3P);
            this.panelJOG.Controls.Add(this.bt_JOG3N);
            this.panelJOG.Controls.Add(this.bt_JOG4P);
            this.panelJOG.Controls.Add(this.bt_JOG4N);
            this.panelJOG.Enabled = false;
            this.panelJOG.Location = new System.Drawing.Point(38, 6);
            this.panelJOG.Name = "panelJOG";
            this.panelJOG.Padding = new System.Windows.Forms.Padding(3);
            this.panelJOG.Size = new System.Drawing.Size(220, 313);
            this.panelJOG.TabIndex = 25;
            // 
            // bt_JOG1P
            // 
            this.bt_JOG1P.Location = new System.Drawing.Point(6, 6);
            this.bt_JOG1P.Name = "bt_JOG1P";
            this.bt_JOG1P.Size = new System.Drawing.Size(100, 70);
            this.bt_JOG1P.TabIndex = 10;
            this.bt_JOG1P.Text = "JOG C1 +";
            this.bt_JOG1P.UseVisualStyleBackColor = true;
            this.bt_JOG1P.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bt_JOG1P_MouseDown);
            this.bt_JOG1P.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bt_JOG1P_MouseUp);
            // 
            // bt_JOG1N
            // 
            this.bt_JOG1N.Location = new System.Drawing.Point(112, 6);
            this.bt_JOG1N.Name = "bt_JOG1N";
            this.bt_JOG1N.Size = new System.Drawing.Size(100, 70);
            this.bt_JOG1N.TabIndex = 11;
            this.bt_JOG1N.Text = "JOG C1 -";
            this.bt_JOG1N.UseVisualStyleBackColor = true;
            this.bt_JOG1N.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bt_JOG1N_MouseDown);
            this.bt_JOG1N.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bt_JOG1N_MouseUp);
            // 
            // bt_JOG2P
            // 
            this.bt_JOG2P.Location = new System.Drawing.Point(6, 82);
            this.bt_JOG2P.Name = "bt_JOG2P";
            this.bt_JOG2P.Size = new System.Drawing.Size(100, 70);
            this.bt_JOG2P.TabIndex = 12;
            this.bt_JOG2P.Text = "JOG C2 +";
            this.bt_JOG2P.UseVisualStyleBackColor = true;
            this.bt_JOG2P.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bt_JOG2P_MouseDown);
            this.bt_JOG2P.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bt_JOG2P_MouseUp);
            // 
            // bt_JOG2N
            // 
            this.bt_JOG2N.Location = new System.Drawing.Point(112, 82);
            this.bt_JOG2N.Name = "bt_JOG2N";
            this.bt_JOG2N.Size = new System.Drawing.Size(100, 70);
            this.bt_JOG2N.TabIndex = 13;
            this.bt_JOG2N.Text = "JOG C2 -";
            this.bt_JOG2N.UseVisualStyleBackColor = true;
            this.bt_JOG2N.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bt_JOG2N_MouseDown);
            this.bt_JOG2N.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bt_JOG2N_MouseUp);
            // 
            // bt_JOG3P
            // 
            this.bt_JOG3P.Location = new System.Drawing.Point(6, 158);
            this.bt_JOG3P.Name = "bt_JOG3P";
            this.bt_JOG3P.Size = new System.Drawing.Size(100, 70);
            this.bt_JOG3P.TabIndex = 14;
            this.bt_JOG3P.Text = "JOG C3 +";
            this.bt_JOG3P.UseVisualStyleBackColor = true;
            this.bt_JOG3P.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bt_JOG3P_MouseDown);
            this.bt_JOG3P.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bt_JOG3P_MouseUp);
            // 
            // bt_JOG3N
            // 
            this.bt_JOG3N.Location = new System.Drawing.Point(112, 158);
            this.bt_JOG3N.Name = "bt_JOG3N";
            this.bt_JOG3N.Size = new System.Drawing.Size(100, 70);
            this.bt_JOG3N.TabIndex = 15;
            this.bt_JOG3N.Text = "JOG C3 -";
            this.bt_JOG3N.UseVisualStyleBackColor = true;
            this.bt_JOG3N.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bt_JOG3N_MouseDown);
            this.bt_JOG3N.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bt_JOG3N_MouseUp);
            // 
            // bt_JOG4P
            // 
            this.bt_JOG4P.Location = new System.Drawing.Point(6, 234);
            this.bt_JOG4P.Name = "bt_JOG4P";
            this.bt_JOG4P.Size = new System.Drawing.Size(100, 70);
            this.bt_JOG4P.TabIndex = 16;
            this.bt_JOG4P.Text = "JOG C4 +";
            this.bt_JOG4P.UseVisualStyleBackColor = true;
            this.bt_JOG4P.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bt_JOG4P_MouseDown);
            this.bt_JOG4P.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bt_JOG4P_MouseUp);
            // 
            // bt_JOG4N
            // 
            this.bt_JOG4N.Location = new System.Drawing.Point(112, 234);
            this.bt_JOG4N.Name = "bt_JOG4N";
            this.bt_JOG4N.Size = new System.Drawing.Size(100, 70);
            this.bt_JOG4N.TabIndex = 17;
            this.bt_JOG4N.Text = "JOG C4 -";
            this.bt_JOG4N.UseVisualStyleBackColor = true;
            this.bt_JOG4N.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bt_JOG4N_MouseDown);
            this.bt_JOG4N.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bt_JOG4N_MouseUp);
            // 
            // tB_mesPrint
            // 
            this.tB_mesPrint.Location = new System.Drawing.Point(338, 32);
            this.tB_mesPrint.Multiline = true;
            this.tB_mesPrint.Name = "tB_mesPrint";
            this.tB_mesPrint.ReadOnly = true;
            this.tB_mesPrint.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tB_mesPrint.Size = new System.Drawing.Size(571, 332);
            this.tB_mesPrint.TabIndex = 24;
            this.tB_mesPrint.WordWrap = false;
            // 
            // bt_cycleReset
            // 
            this.bt_cycleReset.Location = new System.Drawing.Point(9, 44);
            this.bt_cycleReset.Name = "bt_cycleReset";
            this.bt_cycleReset.Size = new System.Drawing.Size(82, 23);
            this.bt_cycleReset.TabIndex = 28;
            this.bt_cycleReset.Text = "Cycle Reset";
            this.bt_cycleReset.UseVisualStyleBackColor = true;
            this.bt_cycleReset.Click += new System.EventHandler(this.bt_cycleReset_Click);
            // 
            // ControllerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 526);
            this.Controls.Add(this.tB_mesPrint);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label_pos);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label_syntecBusy);
            this.Controls.Add(this.label_state);
            this.Controls.Add(this.label1);
            this.Name = "ControllerForm";
            this.Text = "PCController";
            ((System.ComponentModel.ISupportInitialize)(this.num_gvarNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_gvarValue)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panelTestFunc.ResumeLayout(false);
            this.panelTestFunc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_obitNo)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.panelJOG.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tB_SyntecIP;
        private System.Windows.Forms.Button bt_SyntecConnect;
        private System.Windows.Forms.Button bt_upload;
        private System.Windows.Forms.TextBox tB_fileName;
        private System.Windows.Forms.Button bt_cycStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_state;
        private System.Windows.Forms.Label label_syntecBusy;
        private System.Windows.Forms.Button bt_test;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown num_gvarNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_pos;
        private System.Windows.Forms.NumericUpDown num_gvarValue;
        private System.Windows.Forms.Button bt_writeGVar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button bt_readGVar;
        private System.Windows.Forms.Button bt_genNC;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox tB_mesPrint;
        private System.Windows.Forms.Button bt_TRCConnect;
        private System.Windows.Forms.TextBox tB_TRCIP;
        private System.Windows.Forms.TextBox tB_TRCPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button bt_TRCCommStart;
        private System.Windows.Forms.Button bt_setOrigin;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button bt_JOG4N;
        private System.Windows.Forms.Button bt_JOG4P;
        private System.Windows.Forms.Button bt_JOG3N;
        private System.Windows.Forms.Button bt_JOG3P;
        private System.Windows.Forms.Button bt_JOG2N;
        private System.Windows.Forms.Button bt_JOG2P;
        private System.Windows.Forms.Button bt_JOG1N;
        private System.Windows.Forms.Button bt_JOG1P;
        private System.Windows.Forms.Button bt_writeO;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown num_obitNo;
        private System.Windows.Forms.RadioButton rBt_obitF;
        private System.Windows.Forms.RadioButton rBt_obitT;
        private System.Windows.Forms.Panel panelTestFunc;
        private System.Windows.Forms.FlowLayoutPanel panelJOG;
        private System.Windows.Forms.Button bt_servoSW;
        private System.Windows.Forms.Button bt_cycleReset;
    }
}

