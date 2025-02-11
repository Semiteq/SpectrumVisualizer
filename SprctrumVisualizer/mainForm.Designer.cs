namespace SpectrumVisualizer
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            comboBoxCom = new ComboBox();
            numericUpDownIntegration = new NumericUpDown();
            numericUpDownInterval = new NumericUpDown();
            numericUpDownAverage = new NumericUpDown();
            labelIntegration = new Label();
            labelInterval = new Label();
            labelAverage = new Label();
            plotView = new OxyPlot.WindowsForms.PlotView();
            loggingBox = new ListBox();
            labelLog = new Label();
            buttonAcquireDark = new Button();
            buttonResetDark = new Button();
            labelComPort = new Label();
            ((System.ComponentModel.ISupportInitialize)numericUpDownIntegration).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownInterval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownAverage).BeginInit();
            SuspendLayout();
            // 
            // comboBoxCom
            // 
            comboBoxCom.FormattingEnabled = true;
            comboBoxCom.Location = new Point(12, 27);
            comboBoxCom.Name = "comboBoxCom";
            comboBoxCom.Size = new Size(132, 23);
            comboBoxCom.TabIndex = 0;
            comboBoxCom.SelectedIndexChanged += ComboBoxCom_SelectedIndexChanged;
            // 
            // numericUpDownIntegration
            // 
            numericUpDownIntegration.Location = new Point(12, 76);
            numericUpDownIntegration.Name = "numericUpDownIntegration";
            numericUpDownIntegration.Size = new Size(41, 23);
            numericUpDownIntegration.TabIndex = 1;
            numericUpDownIntegration.ValueChanged += numericUpDownIntegration_ValueChanged;
            // 
            // numericUpDownInterval
            // 
            numericUpDownInterval.Location = new Point(12, 105);
            numericUpDownInterval.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
            numericUpDownInterval.Name = "numericUpDownInterval";
            numericUpDownInterval.Size = new Size(41, 23);
            numericUpDownInterval.TabIndex = 2;
            numericUpDownInterval.ValueChanged += numericUpDownInterval_ValueChanged;
            // 
            // numericUpDownAverage
            // 
            numericUpDownAverage.Location = new Point(13, 134);
            numericUpDownAverage.Name = "numericUpDownAverage";
            numericUpDownAverage.Size = new Size(40, 23);
            numericUpDownAverage.TabIndex = 3;
            numericUpDownAverage.ValueChanged += numericUpDownAverage_ValueChanged;
            // 
            // labelIntegration
            // 
            labelIntegration.AutoSize = true;
            labelIntegration.Location = new Point(59, 78);
            labelIntegration.Name = "labelIntegration";
            labelIntegration.Size = new Size(92, 15);
            labelIntegration.TabIndex = 4;
            labelIntegration.Text = "Integration time";
            // 
            // labelInterval
            // 
            labelInterval.AutoSize = true;
            labelInterval.Location = new Point(59, 107);
            labelInterval.Name = "labelInterval";
            labelInterval.Size = new Size(46, 15);
            labelInterval.TabIndex = 5;
            labelInterval.Text = "Interval";
            // 
            // labelAverage
            // 
            labelAverage.AutoSize = true;
            labelAverage.Location = new Point(59, 136);
            labelAverage.Name = "labelAverage";
            labelAverage.Size = new Size(50, 15);
            labelAverage.TabIndex = 6;
            labelAverage.Text = "Average";
            // 
            // plotView
            // 
            plotView.BackColor = SystemColors.ControlLightLight;
            plotView.Location = new Point(161, 12);
            plotView.Name = "plotView";
            plotView.PanCursor = Cursors.Hand;
            plotView.Size = new Size(1177, 620);
            plotView.TabIndex = 7;
            plotView.Text = "plotView";
            plotView.ZoomHorizontalCursor = Cursors.SizeWE;
            plotView.ZoomRectangleCursor = Cursors.SizeNWSE;
            plotView.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // loggingBox
            // 
            loggingBox.FormattingEnabled = true;
            loggingBox.HorizontalScrollbar = true;
            loggingBox.Location = new Point(12, 638);
            loggingBox.Name = "loggingBox";
            loggingBox.Size = new Size(1326, 79);
            loggingBox.TabIndex = 8;
            // 
            // labelLog
            // 
            labelLog.AutoSize = true;
            labelLog.Location = new Point(15, 620);
            labelLog.Name = "labelLog";
            labelLog.Size = new Size(35, 15);
            labelLog.TabIndex = 9;
            labelLog.Text = "Logs:";
            // 
            // buttonAcquireDark
            // 
            buttonAcquireDark.Location = new Point(12, 185);
            buttonAcquireDark.Name = "buttonAcquireDark";
            buttonAcquireDark.Size = new Size(63, 39);
            buttonAcquireDark.TabIndex = 10;
            buttonAcquireDark.Text = "Acquire Dark";
            buttonAcquireDark.UseVisualStyleBackColor = true;
            buttonAcquireDark.Click += buttonAcquireDark_Click;
            // 
            // buttonResetDark
            // 
            buttonResetDark.Location = new Point(81, 185);
            buttonResetDark.Name = "buttonResetDark";
            buttonResetDark.Size = new Size(63, 39);
            buttonResetDark.TabIndex = 11;
            buttonResetDark.Text = "Reset Dark";
            buttonResetDark.UseVisualStyleBackColor = true;
            buttonResetDark.Click += buttonResetDark_Click;
            // 
            // labelComPort
            // 
            labelComPort.AutoSize = true;
            labelComPort.Location = new Point(12, 9);
            labelComPort.Name = "labelComPort";
            labelComPort.Size = new Size(65, 15);
            labelComPort.TabIndex = 12;
            labelComPort.Text = "COM-port:";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1350, 729);
            Controls.Add(labelComPort);
            Controls.Add(buttonResetDark);
            Controls.Add(buttonAcquireDark);
            Controls.Add(labelLog);
            Controls.Add(loggingBox);
            Controls.Add(plotView);
            Controls.Add(labelAverage);
            Controls.Add(labelInterval);
            Controls.Add(labelIntegration);
            Controls.Add(numericUpDownAverage);
            Controls.Add(numericUpDownInterval);
            Controls.Add(numericUpDownIntegration);
            Controls.Add(comboBoxCom);
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Визуализатор спектра";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)numericUpDownIntegration).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownInterval).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownAverage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboBoxCom;
        private NumericUpDown numericUpDownIntegration;
        private NumericUpDown numericUpDownInterval;
        private NumericUpDown numericUpDownAverage;
        private Label labelIntegration;
        private Label labelInterval;
        private Label labelAverage;
        private OxyPlot.WindowsForms.PlotView plotView;
        private ListBox loggingBox;
        private Label labelLog;
        private Button buttonAcquireDark;
        private Button buttonResetDark;
        private Label labelComPort;
    }
}
