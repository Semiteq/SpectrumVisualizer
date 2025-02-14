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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
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
            labelSignalAverage = new Label();
            buttonClearLog = new Button();
            groupBoxDarkControl = new GroupBox();
            buttonLoadDark = new Button();
            buttonSaveDark = new Button();
            groupBoxSpectrometerSettings = new GroupBox();
            groupBoxSignalAnalize = new GroupBox();
            labelQFactor = new Label();
            groupBoxPlotSettings = new GroupBox();
            buttonStickToZero = new Button();
            buttonResetScale = new Button();
            checkBoxLogScale = new CheckBox();
            checkBoxInvertData = new CheckBox();
            groupBoxDeviceInfo = new GroupBox();
            labelPN = new Label();
            labelSN = new Label();
            labelSNR = new Label();
            ((System.ComponentModel.ISupportInitialize)numericUpDownIntegration).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownInterval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownAverage).BeginInit();
            groupBoxDarkControl.SuspendLayout();
            groupBoxSpectrometerSettings.SuspendLayout();
            groupBoxSignalAnalize.SuspendLayout();
            groupBoxPlotSettings.SuspendLayout();
            groupBoxDeviceInfo.SuspendLayout();
            SuspendLayout();
            // 
            // comboBoxCom
            // 
            comboBoxCom.FormattingEnabled = true;
            comboBoxCom.Location = new Point(6, 37);
            comboBoxCom.Name = "comboBoxCom";
            comboBoxCom.Size = new Size(132, 23);
            comboBoxCom.TabIndex = 0;
            // 
            // numericUpDownIntegration
            // 
            numericUpDownIntegration.Location = new Point(7, 22);
            numericUpDownIntegration.Name = "numericUpDownIntegration";
            numericUpDownIntegration.Size = new Size(41, 23);
            numericUpDownIntegration.TabIndex = 1;
            // 
            // numericUpDownInterval
            // 
            numericUpDownInterval.Location = new Point(7, 51);
            numericUpDownInterval.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
            numericUpDownInterval.Name = "numericUpDownInterval";
            numericUpDownInterval.Size = new Size(41, 23);
            numericUpDownInterval.TabIndex = 2;
            // 
            // numericUpDownAverage
            // 
            numericUpDownAverage.Location = new Point(8, 80);
            numericUpDownAverage.Name = "numericUpDownAverage";
            numericUpDownAverage.Size = new Size(40, 23);
            numericUpDownAverage.TabIndex = 3;
            // 
            // labelIntegration
            // 
            labelIntegration.AutoSize = true;
            labelIntegration.Location = new Point(54, 24);
            labelIntegration.Name = "labelIntegration";
            labelIntegration.Size = new Size(92, 15);
            labelIntegration.TabIndex = 4;
            labelIntegration.Text = "Integration time";
            // 
            // labelInterval
            // 
            labelInterval.AutoSize = true;
            labelInterval.Location = new Point(54, 53);
            labelInterval.Name = "labelInterval";
            labelInterval.Size = new Size(46, 15);
            labelInterval.TabIndex = 5;
            labelInterval.Text = "Interval";
            // 
            // labelAverage
            // 
            labelAverage.AutoSize = true;
            labelAverage.Location = new Point(54, 82);
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
            loggingBox.Location = new Point(6, 638);
            loggingBox.Name = "loggingBox";
            loggingBox.Size = new Size(1332, 79);
            loggingBox.TabIndex = 8;
            // 
            // labelLog
            // 
            labelLog.AutoSize = true;
            labelLog.Location = new Point(6, 617);
            labelLog.Name = "labelLog";
            labelLog.Size = new Size(35, 15);
            labelLog.TabIndex = 9;
            labelLog.Text = "Logs:";
            // 
            // buttonAcquireDark
            // 
            buttonAcquireDark.Location = new Point(6, 22);
            buttonAcquireDark.Name = "buttonAcquireDark";
            buttonAcquireDark.Size = new Size(65, 23);
            buttonAcquireDark.TabIndex = 10;
            buttonAcquireDark.Text = "Acquire";
            buttonAcquireDark.UseVisualStyleBackColor = true;
            // 
            // buttonResetDark
            // 
            buttonResetDark.Location = new Point(77, 22);
            buttonResetDark.Name = "buttonResetDark";
            buttonResetDark.Size = new Size(64, 23);
            buttonResetDark.TabIndex = 11;
            buttonResetDark.Text = "Reset";
            buttonResetDark.UseVisualStyleBackColor = true;
            // 
            // labelComPort
            // 
            labelComPort.AutoSize = true;
            labelComPort.Location = new Point(6, 19);
            labelComPort.Name = "labelComPort";
            labelComPort.Size = new Size(65, 15);
            labelComPort.TabIndex = 12;
            labelComPort.Text = "COM-port:";
            // 
            // labelSignalAverage
            // 
            labelSignalAverage.AutoSize = true;
            labelSignalAverage.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            labelSignalAverage.Location = new Point(6, 19);
            labelSignalAverage.Name = "labelSignalAverage";
            labelSignalAverage.Size = new Size(90, 15);
            labelSignalAverage.TabIndex = 13;
            labelSignalAverage.Text = "Average Signal";
            // 
            // buttonClearLog
            // 
            buttonClearLog.Location = new Point(80, 613);
            buttonClearLog.Name = "buttonClearLog";
            buttonClearLog.Size = new Size(75, 23);
            buttonClearLog.TabIndex = 14;
            buttonClearLog.Text = "Clear log";
            buttonClearLog.UseVisualStyleBackColor = true;
            // 
            // groupBoxDarkControl
            // 
            groupBoxDarkControl.Controls.Add(buttonLoadDark);
            groupBoxDarkControl.Controls.Add(buttonSaveDark);
            groupBoxDarkControl.Controls.Add(buttonAcquireDark);
            groupBoxDarkControl.Controls.Add(buttonResetDark);
            groupBoxDarkControl.Location = new Point(6, 237);
            groupBoxDarkControl.Name = "groupBoxDarkControl";
            groupBoxDarkControl.Size = new Size(149, 82);
            groupBoxDarkControl.TabIndex = 15;
            groupBoxDarkControl.TabStop = false;
            groupBoxDarkControl.Text = "Dark Control";
            // 
            // buttonLoadDark
            // 
            buttonLoadDark.Location = new Point(77, 51);
            buttonLoadDark.Name = "buttonLoadDark";
            buttonLoadDark.Size = new Size(64, 23);
            buttonLoadDark.TabIndex = 13;
            buttonLoadDark.Text = "Load";
            buttonLoadDark.UseVisualStyleBackColor = true;
            // 
            // buttonSaveDark
            // 
            buttonSaveDark.Location = new Point(7, 51);
            buttonSaveDark.Name = "buttonSaveDark";
            buttonSaveDark.Size = new Size(64, 23);
            buttonSaveDark.TabIndex = 12;
            buttonSaveDark.Text = "Save";
            buttonSaveDark.UseVisualStyleBackColor = true;
            // 
            // groupBoxSpectrometerSettings
            // 
            groupBoxSpectrometerSettings.Controls.Add(numericUpDownIntegration);
            groupBoxSpectrometerSettings.Controls.Add(numericUpDownInterval);
            groupBoxSpectrometerSettings.Controls.Add(numericUpDownAverage);
            groupBoxSpectrometerSettings.Controls.Add(labelIntegration);
            groupBoxSpectrometerSettings.Controls.Add(labelInterval);
            groupBoxSpectrometerSettings.Controls.Add(labelAverage);
            groupBoxSpectrometerSettings.Location = new Point(6, 118);
            groupBoxSpectrometerSettings.Name = "groupBoxSpectrometerSettings";
            groupBoxSpectrometerSettings.Size = new Size(149, 113);
            groupBoxSpectrometerSettings.TabIndex = 16;
            groupBoxSpectrometerSettings.TabStop = false;
            groupBoxSpectrometerSettings.Text = "Spectrometer Settings";
            // 
            // groupBoxSignalAnalize
            // 
            groupBoxSignalAnalize.Controls.Add(labelSNR);
            groupBoxSignalAnalize.Controls.Add(labelQFactor);
            groupBoxSignalAnalize.Controls.Add(labelSignalAverage);
            groupBoxSignalAnalize.Location = new Point(6, 325);
            groupBoxSignalAnalize.Name = "groupBoxSignalAnalize";
            groupBoxSignalAnalize.Size = new Size(149, 91);
            groupBoxSignalAnalize.TabIndex = 17;
            groupBoxSignalAnalize.TabStop = false;
            groupBoxSignalAnalize.Text = "Signal Analize";
            // 
            // labelQFactor
            // 
            labelQFactor.AutoSize = true;
            labelQFactor.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            labelQFactor.Location = new Point(6, 70);
            labelQFactor.Name = "labelQFactor";
            labelQFactor.Size = new Size(55, 15);
            labelQFactor.TabIndex = 16;
            labelQFactor.Text = "Q-factor";
            // 
            // groupBoxPlotSettings
            // 
            groupBoxPlotSettings.Controls.Add(buttonStickToZero);
            groupBoxPlotSettings.Controls.Add(buttonResetScale);
            groupBoxPlotSettings.Controls.Add(checkBoxLogScale);
            groupBoxPlotSettings.Controls.Add(checkBoxInvertData);
            groupBoxPlotSettings.Location = new Point(6, 422);
            groupBoxPlotSettings.Name = "groupBoxPlotSettings";
            groupBoxPlotSettings.Size = new Size(149, 124);
            groupBoxPlotSettings.TabIndex = 18;
            groupBoxPlotSettings.TabStop = false;
            groupBoxPlotSettings.Text = "Plot Settings";
            // 
            // buttonStickToZero
            // 
            buttonStickToZero.Location = new Point(6, 98);
            buttonStickToZero.Name = "buttonStickToZero";
            buttonStickToZero.Size = new Size(83, 23);
            buttonStickToZero.TabIndex = 3;
            buttonStickToZero.Text = "Stick to zero";
            buttonStickToZero.UseVisualStyleBackColor = true;
            // 
            // buttonResetScale
            // 
            buttonResetScale.Location = new Point(6, 70);
            buttonResetScale.Name = "buttonResetScale";
            buttonResetScale.Size = new Size(83, 23);
            buttonResetScale.TabIndex = 2;
            buttonResetScale.Text = "Reset scale";
            buttonResetScale.UseVisualStyleBackColor = true;
            // 
            // checkBoxLogScale
            // 
            checkBoxLogScale.AutoSize = true;
            checkBoxLogScale.Location = new Point(6, 47);
            checkBoxLogScale.Name = "checkBoxLogScale";
            checkBoxLogScale.Size = new Size(76, 19);
            checkBoxLogScale.TabIndex = 1;
            checkBoxLogScale.Text = "Log Scale";
            checkBoxLogScale.UseVisualStyleBackColor = true;
            // 
            // checkBoxInvertData
            // 
            checkBoxInvertData.AutoSize = true;
            checkBoxInvertData.Location = new Point(6, 22);
            checkBoxInvertData.Name = "checkBoxInvertData";
            checkBoxInvertData.Size = new Size(83, 19);
            checkBoxInvertData.TabIndex = 0;
            checkBoxInvertData.Text = "Invert Data";
            checkBoxInvertData.UseVisualStyleBackColor = true;
            // 
            // groupBoxDeviceInfo
            // 
            groupBoxDeviceInfo.Controls.Add(labelPN);
            groupBoxDeviceInfo.Controls.Add(labelSN);
            groupBoxDeviceInfo.Controls.Add(comboBoxCom);
            groupBoxDeviceInfo.Controls.Add(labelComPort);
            groupBoxDeviceInfo.Location = new Point(6, 12);
            groupBoxDeviceInfo.Name = "groupBoxDeviceInfo";
            groupBoxDeviceInfo.Size = new Size(149, 100);
            groupBoxDeviceInfo.TabIndex = 19;
            groupBoxDeviceInfo.TabStop = false;
            groupBoxDeviceInfo.Text = "Current Device Info";
            // 
            // labelPN
            // 
            labelPN.AutoSize = true;
            labelPN.Location = new Point(6, 82);
            labelPN.Name = "labelPN";
            labelPN.Size = new Size(23, 15);
            labelPN.TabIndex = 14;
            labelPN.Text = "PN";
            // 
            // labelSN
            // 
            labelSN.AutoSize = true;
            labelSN.Location = new Point(6, 63);
            labelSN.Name = "labelSN";
            labelSN.Size = new Size(22, 15);
            labelSN.TabIndex = 13;
            labelSN.Text = "SN";
            // 
            // labelSNR
            // 
            labelSNR.AutoSize = true;
            labelSNR.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            labelSNR.Location = new Point(6, 45);
            labelSNR.Name = "labelSNR";
            labelSNR.Size = new Size(31, 15);
            labelSNR.TabIndex = 17;
            labelSNR.Text = "SNR";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1350, 729);
            Controls.Add(groupBoxDeviceInfo);
            Controls.Add(groupBoxPlotSettings);
            Controls.Add(groupBoxSignalAnalize);
            Controls.Add(groupBoxSpectrometerSettings);
            Controls.Add(groupBoxDarkControl);
            Controls.Add(buttonClearLog);
            Controls.Add(labelLog);
            Controls.Add(loggingBox);
            Controls.Add(plotView);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Spectrum Visualizer";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)numericUpDownIntegration).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownInterval).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownAverage).EndInit();
            groupBoxDarkControl.ResumeLayout(false);
            groupBoxSpectrometerSettings.ResumeLayout(false);
            groupBoxSpectrometerSettings.PerformLayout();
            groupBoxSignalAnalize.ResumeLayout(false);
            groupBoxSignalAnalize.PerformLayout();
            groupBoxPlotSettings.ResumeLayout(false);
            groupBoxPlotSettings.PerformLayout();
            groupBoxDeviceInfo.ResumeLayout(false);
            groupBoxDeviceInfo.PerformLayout();
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
        private Label labelSignalAverage;
        private Button buttonClearLog;
        private GroupBox groupBoxDarkControl;
        private Button buttonLoadDark;
        private Button buttonSaveDark;
        private GroupBox groupBoxSpectrometerSettings;
        private GroupBox groupBoxSignalAnalize;
        private Label labelQFactor;
        private GroupBox groupBoxPlotSettings;
        private CheckBox checkBoxLogScale;
        private CheckBox checkBoxInvertData;
        private Button buttonResetScale;
        private GroupBox groupBoxDeviceInfo;
        private Label labelPN;
        private Label labelSN;
        private Button buttonStickToZero;
        private Label labelSNR;
    }
}
