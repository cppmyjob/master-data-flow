namespace MasterDataFlow.Trading.Ui
{
    partial class MainForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.ofdOpenTestFile = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tradingChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.lvDateRange = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnStart = new System.Windows.Forms.Button();
            this.nudPopulationFactor = new System.Windows.Forms.NumericUpDown();
            this.label23 = new System.Windows.Forms.Label();
            this.cmbProcessors = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tbIndicators = new System.Windows.Forms.TextBox();
            this.tbPredictionMinusCount = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbPredictionPlusCount = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tbStopLoss = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbIteration = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbTrainingMinusCount = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbTrainingPlusCount = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbPredictionOrderCount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbPredictionDiff = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbPredictionProfit = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbTrainingOrderCount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbTrainingDiff = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbTrainingProfit = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSpeed = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbFitness = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.testChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnTesterStart = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmbStocks = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tuiTesting = new MasterDataFlow.Trading.Ui.TradingUserInfo();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tradingChart)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPopulationFactor)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testChart)).BeginInit();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ofdOpenTestFile
            // 
            this.ofdOpenTestFile.DefaultExt = "save";
            this.ofdOpenTestFile.Filter = "Saved Data|*.save";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1186, 655);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 53);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1180, 599);
            this.panel1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1180, 599);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1172, 573);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Learning";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tradingChart);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1166, 313);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Chart";
            // 
            // tradingChart
            // 
            chartArea1.Name = "ChartArea1";
            this.tradingChart.ChartAreas.Add(chartArea1);
            this.tradingChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.tradingChart.Legends.Add(legend1);
            this.tradingChart.Location = new System.Drawing.Point(3, 16);
            this.tradingChart.Name = "tradingChart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.tradingChart.Series.Add(series1);
            this.tradingChart.Size = new System.Drawing.Size(1160, 294);
            this.tradingChart.TabIndex = 0;
            this.tradingChart.Text = "chart1";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.tabControl2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(3, 316);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1166, 254);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(3, 16);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1160, 235);
            this.tabControl2.TabIndex = 46;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.lvDateRange);
            this.tabPage3.Controls.Add(this.btnStart);
            this.tabPage3.Controls.Add(this.nudPopulationFactor);
            this.tabPage3.Controls.Add(this.label23);
            this.tabPage3.Controls.Add(this.cmbProcessors);
            this.tabPage3.Controls.Add(this.label15);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1152, 209);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Input";
            // 
            // lvDateRange
            // 
            this.lvDateRange.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvDateRange.HideSelection = false;
            this.lvDateRange.Location = new System.Drawing.Point(286, 6);
            this.lvDateRange.Name = "lvDateRange";
            this.lvDateRange.Size = new System.Drawing.Size(447, 197);
            this.lvDateRange.TabIndex = 57;
            this.lvDateRange.UseCompatibleStateImageBehavior = false;
            this.lvDateRange.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Type";
            this.columnHeader1.Width = 173;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Date";
            this.columnHeader2.Width = 229;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(1013, 27);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(112, 34);
            this.btnStart.TabIndex = 56;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // nudPopulationFactor
            // 
            this.nudPopulationFactor.Location = new System.Drawing.Point(164, 68);
            this.nudPopulationFactor.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudPopulationFactor.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPopulationFactor.Name = "nudPopulationFactor";
            this.nudPopulationFactor.Size = new System.Drawing.Size(50, 20);
            this.nudPopulationFactor.TabIndex = 55;
            this.nudPopulationFactor.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(31, 72);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(127, 18);
            this.label23.TabIndex = 54;
            this.label23.Text = "Population Count 100x :";
            // 
            // cmbProcessors
            // 
            this.cmbProcessors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProcessors.FormattingEnabled = true;
            this.cmbProcessors.Location = new System.Drawing.Point(137, 27);
            this.cmbProcessors.Name = "cmbProcessors";
            this.cmbProcessors.Size = new System.Drawing.Size(77, 21);
            this.cmbProcessors.TabIndex = 47;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(51, 30);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(80, 16);
            this.label15.TabIndex = 46;
            this.label15.Text = "Processors :";
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage4.Controls.Add(this.tbIndicators);
            this.tabPage4.Controls.Add(this.tbPredictionMinusCount);
            this.tabPage4.Controls.Add(this.label13);
            this.tabPage4.Controls.Add(this.tbPredictionPlusCount);
            this.tabPage4.Controls.Add(this.label14);
            this.tabPage4.Controls.Add(this.tbStopLoss);
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Controls.Add(this.tbIteration);
            this.tabPage4.Controls.Add(this.label10);
            this.tabPage4.Controls.Add(this.tbTrainingMinusCount);
            this.tabPage4.Controls.Add(this.label9);
            this.tabPage4.Controls.Add(this.tbTrainingPlusCount);
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Controls.Add(this.tbPredictionOrderCount);
            this.tabPage4.Controls.Add(this.label5);
            this.tabPage4.Controls.Add(this.tbPredictionDiff);
            this.tabPage4.Controls.Add(this.label6);
            this.tabPage4.Controls.Add(this.tbPredictionProfit);
            this.tabPage4.Controls.Add(this.label7);
            this.tabPage4.Controls.Add(this.tbTrainingOrderCount);
            this.tabPage4.Controls.Add(this.label4);
            this.tabPage4.Controls.Add(this.tbTrainingDiff);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.tbTrainingProfit);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.tbSpeed);
            this.tabPage4.Controls.Add(this.label1);
            this.tabPage4.Controls.Add(this.tbFitness);
            this.tabPage4.Controls.Add(this.label12);
            this.tabPage4.Controls.Add(this.label22);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1152, 209);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Output";
            // 
            // tbIndicators
            // 
            this.tbIndicators.Location = new System.Drawing.Point(300, 157);
            this.tbIndicators.Name = "tbIndicators";
            this.tbIndicators.ReadOnly = true;
            this.tbIndicators.Size = new System.Drawing.Size(501, 20);
            this.tbIndicators.TabIndex = 107;
            // 
            // tbPredictionMinusCount
            // 
            this.tbPredictionMinusCount.Location = new System.Drawing.Point(807, 128);
            this.tbPredictionMinusCount.Name = "tbPredictionMinusCount";
            this.tbPredictionMinusCount.ReadOnly = true;
            this.tbPredictionMinusCount.Size = new System.Drawing.Size(124, 20);
            this.tbPredictionMinusCount.TabIndex = 106;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(698, 130);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(103, 16);
            this.label13.TabIndex = 105;
            this.label13.Text = "PredictionMinusCount :";
            // 
            // tbPredictionPlusCount
            // 
            this.tbPredictionPlusCount.Location = new System.Drawing.Point(807, 102);
            this.tbPredictionPlusCount.Name = "tbPredictionPlusCount";
            this.tbPredictionPlusCount.ReadOnly = true;
            this.tbPredictionPlusCount.Size = new System.Drawing.Size(124, 20);
            this.tbPredictionPlusCount.TabIndex = 104;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(698, 104);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(103, 16);
            this.label14.TabIndex = 103;
            this.label14.Text = "PredictionPlusCount :";
            // 
            // tbStopLoss
            // 
            this.tbStopLoss.Location = new System.Drawing.Point(300, 62);
            this.tbStopLoss.Name = "tbStopLoss";
            this.tbStopLoss.ReadOnly = true;
            this.tbStopLoss.Size = new System.Drawing.Size(124, 20);
            this.tbStopLoss.TabIndex = 102;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(227, 62);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 16);
            this.label11.TabIndex = 101;
            this.label11.Text = "StopLoss :";
            // 
            // tbIteration
            // 
            this.tbIteration.Location = new System.Drawing.Point(300, 131);
            this.tbIteration.Name = "tbIteration";
            this.tbIteration.ReadOnly = true;
            this.tbIteration.Size = new System.Drawing.Size(124, 20);
            this.tbIteration.TabIndex = 100;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(227, 133);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 16);
            this.label10.TabIndex = 99;
            this.label10.Text = "Iteration :";
            // 
            // tbTrainingMinusCount
            // 
            this.tbTrainingMinusCount.Location = new System.Drawing.Point(557, 133);
            this.tbTrainingMinusCount.Name = "tbTrainingMinusCount";
            this.tbTrainingMinusCount.ReadOnly = true;
            this.tbTrainingMinusCount.Size = new System.Drawing.Size(124, 20);
            this.tbTrainingMinusCount.TabIndex = 98;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(448, 135);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(103, 16);
            this.label9.TabIndex = 97;
            this.label9.Text = "TrainingMinusCount :";
            // 
            // tbTrainingPlusCount
            // 
            this.tbTrainingPlusCount.Location = new System.Drawing.Point(557, 107);
            this.tbTrainingPlusCount.Name = "tbTrainingPlusCount";
            this.tbTrainingPlusCount.ReadOnly = true;
            this.tbTrainingPlusCount.Size = new System.Drawing.Size(124, 20);
            this.tbTrainingPlusCount.TabIndex = 96;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(448, 109);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(103, 16);
            this.label8.TabIndex = 95;
            this.label8.Text = "TrainingPlusCount :";
            // 
            // tbPredictionOrderCount
            // 
            this.tbPredictionOrderCount.Location = new System.Drawing.Point(807, 81);
            this.tbPredictionOrderCount.Name = "tbPredictionOrderCount";
            this.tbPredictionOrderCount.ReadOnly = true;
            this.tbPredictionOrderCount.Size = new System.Drawing.Size(124, 20);
            this.tbPredictionOrderCount.TabIndex = 94;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(687, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 16);
            this.label5.TabIndex = 93;
            this.label5.Text = "PredictionOrderCount :";
            // 
            // tbPredictionDiff
            // 
            this.tbPredictionDiff.Location = new System.Drawing.Point(807, 55);
            this.tbPredictionDiff.Name = "tbPredictionDiff";
            this.tbPredictionDiff.ReadOnly = true;
            this.tbPredictionDiff.Size = new System.Drawing.Size(124, 20);
            this.tbPredictionDiff.TabIndex = 92;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(719, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 16);
            this.label6.TabIndex = 91;
            this.label6.Text = "PredictionDiff :";
            // 
            // tbPredictionProfit
            // 
            this.tbPredictionProfit.Location = new System.Drawing.Point(807, 31);
            this.tbPredictionProfit.Name = "tbPredictionProfit";
            this.tbPredictionProfit.ReadOnly = true;
            this.tbPredictionProfit.Size = new System.Drawing.Size(124, 20);
            this.tbPredictionProfit.TabIndex = 90;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(709, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 16);
            this.label7.TabIndex = 89;
            this.label7.Text = "PredictionProfit :";
            // 
            // tbTrainingOrderCount
            // 
            this.tbTrainingOrderCount.Location = new System.Drawing.Point(557, 81);
            this.tbTrainingOrderCount.Name = "tbTrainingOrderCount";
            this.tbTrainingOrderCount.ReadOnly = true;
            this.tbTrainingOrderCount.Size = new System.Drawing.Size(124, 20);
            this.tbTrainingOrderCount.TabIndex = 88;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(448, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 16);
            this.label4.TabIndex = 87;
            this.label4.Text = "TrainingOrderCount :";
            // 
            // tbTrainingDiff
            // 
            this.tbTrainingDiff.Location = new System.Drawing.Point(557, 55);
            this.tbTrainingDiff.Name = "tbTrainingDiff";
            this.tbTrainingDiff.ReadOnly = true;
            this.tbTrainingDiff.Size = new System.Drawing.Size(124, 20);
            this.tbTrainingDiff.TabIndex = 86;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(480, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 85;
            this.label3.Text = "TrainingDiff :";
            // 
            // tbTrainingProfit
            // 
            this.tbTrainingProfit.Location = new System.Drawing.Point(557, 31);
            this.tbTrainingProfit.Name = "tbTrainingProfit";
            this.tbTrainingProfit.ReadOnly = true;
            this.tbTrainingProfit.Size = new System.Drawing.Size(124, 20);
            this.tbTrainingProfit.TabIndex = 84;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(471, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 16);
            this.label2.TabIndex = 83;
            this.label2.Text = "TrainingProfit :";
            // 
            // tbSpeed
            // 
            this.tbSpeed.Location = new System.Drawing.Point(300, 100);
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.ReadOnly = true;
            this.tbSpeed.Size = new System.Drawing.Size(124, 20);
            this.tbSpeed.TabIndex = 82;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(227, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 81;
            this.label1.Text = "Speed :";
            // 
            // tbFitness
            // 
            this.tbFitness.Location = new System.Drawing.Point(300, 35);
            this.tbFitness.Name = "tbFitness";
            this.tbFitness.ReadOnly = true;
            this.tbFitness.Size = new System.Drawing.Size(124, 20);
            this.tbFitness.TabIndex = 80;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(227, 35);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 16);
            this.label12.TabIndex = 79;
            this.label12.Text = "Fitness :";
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(227, 161);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(80, 16);
            this.label22.TabIndex = 108;
            this.label22.Text = "Indicators :";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tabControl3);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1172, 573);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Test";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tabPage5);
            this.tabControl3.Controls.Add(this.tabPage6);
            this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl3.Location = new System.Drawing.Point(3, 3);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(1166, 386);
            this.tabControl3.TabIndex = 1;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.testChart);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1158, 360);
            this.tabPage5.TabIndex = 0;
            this.tabPage5.Text = "Chart";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // testChart
            // 
            chartArea2.Name = "ChartArea1";
            this.testChart.ChartAreas.Add(chartArea2);
            this.testChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Name = "Legend1";
            this.testChart.Legends.Add(legend2);
            this.testChart.Location = new System.Drawing.Point(3, 3);
            this.testChart.Name = "testChart";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.testChart.Series.Add(series2);
            this.testChart.Size = new System.Drawing.Size(1152, 354);
            this.testChart.TabIndex = 1;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.dataGridView1);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(1158, 360);
            this.tabPage6.TabIndex = 1;
            this.tabPage6.Text = "Grid";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1152, 354);
            this.dataGridView1.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnTesterStart);
            this.groupBox3.Controls.Add(this.tuiTesting);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox3.Location = new System.Drawing.Point(3, 389);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1166, 181);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Parameters";
            // 
            // btnTesterStart
            // 
            this.btnTesterStart.Location = new System.Drawing.Point(971, 36);
            this.btnTesterStart.Name = "btnTesterStart";
            this.btnTesterStart.Size = new System.Drawing.Size(112, 34);
            this.btnTesterStart.TabIndex = 59;
            this.btnTesterStart.Text = "Open";
            this.btnTesterStart.UseVisualStyleBackColor = true;
            this.btnTesterStart.Click += new System.EventHandler(this.btnTesterStart_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cmbStocks);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1180, 44);
            this.panel2.TabIndex = 1;
            // 
            // cmbStocks
            // 
            this.cmbStocks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStocks.FormattingEnabled = true;
            this.cmbStocks.Location = new System.Drawing.Point(57, 12);
            this.cmbStocks.Name = "cmbStocks";
            this.cmbStocks.Size = new System.Drawing.Size(171, 21);
            this.cmbStocks.TabIndex = 1;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(11, 15);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(40, 13);
            this.label16.TabIndex = 0;
            this.label16.Text = "Stocks";
            // 
            // tuiTesting
            // 
            this.tuiTesting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tuiTesting.Location = new System.Drawing.Point(3, 16);
            this.tuiTesting.Name = "tuiTesting";
            this.tuiTesting.Size = new System.Drawing.Size(1160, 162);
            this.tuiTesting.TabIndex = 58;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1186, 655);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MainForm";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tradingChart)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudPopulationFactor)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.testChart)).EndInit();
            this.tabPage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog ofdOpenTestFile;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataVisualization.Charting.Chart tradingChart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListView lvDateRange;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.NumericUpDown nudPopulationFactor;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.ComboBox cmbProcessors;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox tbIndicators;
        private System.Windows.Forms.TextBox tbPredictionMinusCount;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbPredictionPlusCount;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tbStopLoss;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbIteration;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbTrainingMinusCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbTrainingPlusCount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbPredictionOrderCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbPredictionDiff;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbPredictionProfit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbTrainingOrderCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbTrainingDiff;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbTrainingProfit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbSpeed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbFitness;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.DataVisualization.Charting.Chart testChart;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cmbStocks;
        private System.Windows.Forms.Label label16;
        private TradingUserInfo tuiTesting;
        private System.Windows.Forms.Button btnTesterStart;
    }
}