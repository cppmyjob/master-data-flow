namespace MasterDataFlow.Trading.Ui
{
    partial class Form1
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tradingChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.nudPopulationFactor = new System.Windows.Forms.NumericUpDown();
            this.label23 = new System.Windows.Forms.Label();
            this.dtpStartTestDate = new System.Windows.Forms.DateTimePicker();
            this.dtpStartValidationDate = new System.Windows.Forms.DateTimePicker();
            this.dtpStartTrainingDate = new System.Windows.Forms.DateTimePicker();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.tbNeuronsLayers = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.tbAdditionalsParams = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tbAvgMaxPeriod = new System.Windows.Forms.TextBox();
            this.label143 = new System.Windows.Forms.Label();
            this.tbBarCount = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.cmbProcessors = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
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
            this.tbIndicators = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tradingChart)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPopulationFactor)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(978, 622);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tradingChart);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1051, 335);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SBER";
            // 
            // tradingChart
            // 
            chartArea2.Name = "ChartArea1";
            this.tradingChart.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.tradingChart.Legends.Add(legend2);
            this.tradingChart.Location = new System.Drawing.Point(6, 19);
            this.tradingChart.Name = "tradingChart";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.tradingChart.Series.Add(series2);
            this.tradingChart.Size = new System.Drawing.Size(1014, 300);
            this.tradingChart.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.nudPopulationFactor);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.dtpStartTestDate);
            this.groupBox2.Controls.Add(this.dtpStartValidationDate);
            this.groupBox2.Controls.Add(this.dtpStartTrainingDate);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.tbNeuronsLayers);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.tbAdditionalsParams);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.tbAvgMaxPeriod);
            this.groupBox2.Controls.Add(this.label143);
            this.groupBox2.Controls.Add(this.tbBarCount);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.cmbProcessors);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Location = new System.Drawing.Point(12, 353);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1051, 104);
            this.groupBox2.TabIndex = 48;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // nudPopulationFactor
            // 
            this.nudPopulationFactor.Location = new System.Drawing.Point(135, 50);
            this.nudPopulationFactor.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPopulationFactor.Name = "nudPopulationFactor";
            this.nudPopulationFactor.Size = new System.Drawing.Size(50, 20);
            this.nudPopulationFactor.TabIndex = 35;
            this.nudPopulationFactor.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(14, 52);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(127, 18);
            this.label23.TabIndex = 34;
            this.label23.Text = "Population Count 100x :";
            // 
            // dtpStartTestDate
            // 
            this.dtpStartTestDate.CustomFormat = "dd/MM/yyyy HH:MM";
            this.dtpStartTestDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartTestDate.Location = new System.Drawing.Point(320, 55);
            this.dtpStartTestDate.Name = "dtpStartTestDate";
            this.dtpStartTestDate.Size = new System.Drawing.Size(153, 20);
            this.dtpStartTestDate.TabIndex = 32;
            this.dtpStartTestDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // dtpStartValidationDate
            // 
            this.dtpStartValidationDate.CustomFormat = "dd/MM/yyyy HH:MM";
            this.dtpStartValidationDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartValidationDate.Location = new System.Drawing.Point(320, 34);
            this.dtpStartValidationDate.Name = "dtpStartValidationDate";
            this.dtpStartValidationDate.Size = new System.Drawing.Size(153, 20);
            this.dtpStartValidationDate.TabIndex = 31;
            this.dtpStartValidationDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // dtpStartTrainingDate
            // 
            this.dtpStartTrainingDate.CustomFormat = "dd/MM/yyyy HH:MM";
            this.dtpStartTrainingDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartTrainingDate.Location = new System.Drawing.Point(320, 16);
            this.dtpStartTrainingDate.Name = "dtpStartTrainingDate";
            this.dtpStartTrainingDate.Size = new System.Drawing.Size(153, 20);
            this.dtpStartTrainingDate.TabIndex = 30;
            this.dtpStartTrainingDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(206, 52);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(119, 18);
            this.label21.TabIndex = 28;
            this.label21.Text = "End Predication Date :";
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(206, 34);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(113, 18);
            this.label20.TabIndex = 27;
            this.label20.Text = "Start Validation Date :";
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(206, 16);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(113, 18);
            this.label19.TabIndex = 26;
            this.label19.Text = "Start Training Date :";
            // 
            // tbNeuronsLayers
            // 
            this.tbNeuronsLayers.Location = new System.Drawing.Point(799, 42);
            this.tbNeuronsLayers.Name = "tbNeuronsLayers";
            this.tbNeuronsLayers.ReadOnly = true;
            this.tbNeuronsLayers.Size = new System.Drawing.Size(79, 20);
            this.tbNeuronsLayers.TabIndex = 25;
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(702, 42);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(91, 20);
            this.label18.TabIndex = 24;
            this.label18.Text = "Neurons Layers :";
            // 
            // tbAdditionalsParams
            // 
            this.tbAdditionalsParams.Location = new System.Drawing.Point(799, 16);
            this.tbAdditionalsParams.Name = "tbAdditionalsParams";
            this.tbAdditionalsParams.ReadOnly = true;
            this.tbAdditionalsParams.Size = new System.Drawing.Size(79, 20);
            this.tbAdditionalsParams.TabIndex = 23;
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(702, 16);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(91, 20);
            this.label17.TabIndex = 22;
            this.label17.Text = "Additional params :";
            // 
            // tbAvgMaxPeriod
            // 
            this.tbAvgMaxPeriod.Location = new System.Drawing.Point(605, 42);
            this.tbAvgMaxPeriod.Name = "tbAvgMaxPeriod";
            this.tbAvgMaxPeriod.ReadOnly = true;
            this.tbAvgMaxPeriod.Size = new System.Drawing.Size(79, 20);
            this.tbAvgMaxPeriod.TabIndex = 21;
            // 
            // label143
            // 
            this.label143.Location = new System.Drawing.Point(508, 42);
            this.label143.Name = "label143";
            this.label143.Size = new System.Drawing.Size(91, 20);
            this.label143.TabIndex = 20;
            this.label143.Text = "Avg Max Period :";
            // 
            // tbBarCount
            // 
            this.tbBarCount.Location = new System.Drawing.Point(605, 16);
            this.tbBarCount.Name = "tbBarCount";
            this.tbBarCount.ReadOnly = true;
            this.tbBarCount.Size = new System.Drawing.Size(79, 20);
            this.tbBarCount.TabIndex = 19;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(508, 16);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(80, 16);
            this.label16.TabIndex = 18;
            this.label16.Text = "Bar Count :";
            // 
            // cmbProcessors
            // 
            this.cmbProcessors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProcessors.FormattingEnabled = true;
            this.cmbProcessors.Location = new System.Drawing.Point(108, 13);
            this.cmbProcessors.Name = "cmbProcessors";
            this.cmbProcessors.Size = new System.Drawing.Size(77, 21);
            this.cmbProcessors.TabIndex = 17;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(14, 16);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(80, 16);
            this.label15.TabIndex = 16;
            this.label15.Text = "Processors :";
            // 
            // tbPredictionMinusCount
            // 
            this.tbPredictionMinusCount.Location = new System.Drawing.Point(929, 564);
            this.tbPredictionMinusCount.Name = "tbPredictionMinusCount";
            this.tbPredictionMinusCount.ReadOnly = true;
            this.tbPredictionMinusCount.Size = new System.Drawing.Size(124, 20);
            this.tbPredictionMinusCount.TabIndex = 76;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(820, 566);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(103, 16);
            this.label13.TabIndex = 75;
            this.label13.Text = "PredictionMinusCount :";
            // 
            // tbPredictionPlusCount
            // 
            this.tbPredictionPlusCount.Location = new System.Drawing.Point(929, 538);
            this.tbPredictionPlusCount.Name = "tbPredictionPlusCount";
            this.tbPredictionPlusCount.ReadOnly = true;
            this.tbPredictionPlusCount.Size = new System.Drawing.Size(124, 20);
            this.tbPredictionPlusCount.TabIndex = 74;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(820, 540);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(103, 16);
            this.label14.TabIndex = 73;
            this.label14.Text = "PredictionPlusCount :";
            // 
            // tbStopLoss
            // 
            this.tbStopLoss.Location = new System.Drawing.Point(422, 498);
            this.tbStopLoss.Name = "tbStopLoss";
            this.tbStopLoss.ReadOnly = true;
            this.tbStopLoss.Size = new System.Drawing.Size(124, 20);
            this.tbStopLoss.TabIndex = 72;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(349, 498);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 16);
            this.label11.TabIndex = 71;
            this.label11.Text = "StopLoss :";
            // 
            // tbIteration
            // 
            this.tbIteration.Location = new System.Drawing.Point(422, 567);
            this.tbIteration.Name = "tbIteration";
            this.tbIteration.ReadOnly = true;
            this.tbIteration.Size = new System.Drawing.Size(124, 20);
            this.tbIteration.TabIndex = 70;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(349, 569);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 16);
            this.label10.TabIndex = 69;
            this.label10.Text = "Iteration :";
            // 
            // tbTrainingMinusCount
            // 
            this.tbTrainingMinusCount.Location = new System.Drawing.Point(679, 569);
            this.tbTrainingMinusCount.Name = "tbTrainingMinusCount";
            this.tbTrainingMinusCount.ReadOnly = true;
            this.tbTrainingMinusCount.Size = new System.Drawing.Size(124, 20);
            this.tbTrainingMinusCount.TabIndex = 68;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(570, 571);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(103, 16);
            this.label9.TabIndex = 67;
            this.label9.Text = "TrainingMinusCount :";
            // 
            // tbTrainingPlusCount
            // 
            this.tbTrainingPlusCount.Location = new System.Drawing.Point(679, 543);
            this.tbTrainingPlusCount.Name = "tbTrainingPlusCount";
            this.tbTrainingPlusCount.ReadOnly = true;
            this.tbTrainingPlusCount.Size = new System.Drawing.Size(124, 20);
            this.tbTrainingPlusCount.TabIndex = 66;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(570, 545);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(103, 16);
            this.label8.TabIndex = 65;
            this.label8.Text = "TrainingPlusCount :";
            // 
            // tbPredictionOrderCount
            // 
            this.tbPredictionOrderCount.Location = new System.Drawing.Point(929, 517);
            this.tbPredictionOrderCount.Name = "tbPredictionOrderCount";
            this.tbPredictionOrderCount.ReadOnly = true;
            this.tbPredictionOrderCount.Size = new System.Drawing.Size(124, 20);
            this.tbPredictionOrderCount.TabIndex = 64;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(809, 519);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 16);
            this.label5.TabIndex = 63;
            this.label5.Text = "PredictionOrderCount :";
            // 
            // tbPredictionDiff
            // 
            this.tbPredictionDiff.Location = new System.Drawing.Point(929, 491);
            this.tbPredictionDiff.Name = "tbPredictionDiff";
            this.tbPredictionDiff.ReadOnly = true;
            this.tbPredictionDiff.Size = new System.Drawing.Size(124, 20);
            this.tbPredictionDiff.TabIndex = 62;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(841, 495);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 16);
            this.label6.TabIndex = 61;
            this.label6.Text = "PredictionDiff :";
            // 
            // tbPredictionProfit
            // 
            this.tbPredictionProfit.Location = new System.Drawing.Point(929, 467);
            this.tbPredictionProfit.Name = "tbPredictionProfit";
            this.tbPredictionProfit.ReadOnly = true;
            this.tbPredictionProfit.Size = new System.Drawing.Size(124, 20);
            this.tbPredictionProfit.TabIndex = 60;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(831, 467);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 16);
            this.label7.TabIndex = 59;
            this.label7.Text = "PredictionProfit :";
            // 
            // tbTrainingOrderCount
            // 
            this.tbTrainingOrderCount.Location = new System.Drawing.Point(679, 517);
            this.tbTrainingOrderCount.Name = "tbTrainingOrderCount";
            this.tbTrainingOrderCount.ReadOnly = true;
            this.tbTrainingOrderCount.Size = new System.Drawing.Size(124, 20);
            this.tbTrainingOrderCount.TabIndex = 58;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(570, 519);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 16);
            this.label4.TabIndex = 57;
            this.label4.Text = "TrainingOrderCount :";
            // 
            // tbTrainingDiff
            // 
            this.tbTrainingDiff.Location = new System.Drawing.Point(679, 491);
            this.tbTrainingDiff.Name = "tbTrainingDiff";
            this.tbTrainingDiff.ReadOnly = true;
            this.tbTrainingDiff.Size = new System.Drawing.Size(124, 20);
            this.tbTrainingDiff.TabIndex = 56;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(602, 495);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 55;
            this.label3.Text = "TrainingDiff :";
            // 
            // tbTrainingProfit
            // 
            this.tbTrainingProfit.Location = new System.Drawing.Point(679, 467);
            this.tbTrainingProfit.Name = "tbTrainingProfit";
            this.tbTrainingProfit.ReadOnly = true;
            this.tbTrainingProfit.Size = new System.Drawing.Size(124, 20);
            this.tbTrainingProfit.TabIndex = 54;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(593, 467);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 16);
            this.label2.TabIndex = 53;
            this.label2.Text = "TrainingProfit :";
            // 
            // tbSpeed
            // 
            this.tbSpeed.Location = new System.Drawing.Point(422, 536);
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.ReadOnly = true;
            this.tbSpeed.Size = new System.Drawing.Size(124, 20);
            this.tbSpeed.TabIndex = 52;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(349, 538);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 51;
            this.label1.Text = "Speed :";
            // 
            // tbFitness
            // 
            this.tbFitness.Location = new System.Drawing.Point(422, 471);
            this.tbFitness.Name = "tbFitness";
            this.tbFitness.ReadOnly = true;
            this.tbFitness.Size = new System.Drawing.Size(124, 20);
            this.tbFitness.TabIndex = 50;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(349, 471);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 16);
            this.label12.TabIndex = 49;
            this.label12.Text = "Fitness :";
            // 
            // tbIndicators
            // 
            this.tbIndicators.Location = new System.Drawing.Point(422, 593);
            this.tbIndicators.Name = "tbIndicators";
            this.tbIndicators.ReadOnly = true;
            this.tbIndicators.Size = new System.Drawing.Size(501, 20);
            this.tbIndicators.TabIndex = 77;
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(349, 597);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(80, 16);
            this.label22.TabIndex = 78;
            this.label22.Text = "Indicators :";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(106, 571);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 79;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(207, 571);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 80;
            this.button3.Text = "ZigZag";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1075, 647);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.tbIndicators);
            this.Controls.Add(this.tbPredictionMinusCount);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.tbPredictionPlusCount);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.tbStopLoss);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.tbIteration);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tbTrainingMinusCount);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tbTrainingPlusCount);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tbPredictionOrderCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbPredictionDiff);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbPredictionProfit);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbTrainingOrderCount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbTrainingDiff);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbTrainingProfit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbSpeed);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbFitness);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label22);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tradingChart)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPopulationFactor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown nudPopulationFactor;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.DateTimePicker dtpStartTestDate;
        private System.Windows.Forms.DateTimePicker dtpStartValidationDate;
        private System.Windows.Forms.DateTimePicker dtpStartTrainingDate;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox tbNeuronsLayers;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox tbAdditionalsParams;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tbAvgMaxPeriod;
        private System.Windows.Forms.Label label143;
        private System.Windows.Forms.TextBox tbBarCount;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cmbProcessors;
        private System.Windows.Forms.Label label15;
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
        private System.Windows.Forms.TextBox tbIndicators;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataVisualization.Charting.Chart tradingChart;
        private System.Windows.Forms.Button button3;
    }
}

