namespace SpectrumVisualizer.Uart
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
            plotView = new OxyPlot.WindowsForms.PlotView();
            loggingBox = new ListBox();
            labelLog = new Label();
            labelComPort = new Label();
            labelSignalAverage = new Label();
            buttonClearLog = new Button();
            groupBoxSignalAnalize = new GroupBox();
            labelSNR = new Label();
            labelQFactor = new Label();
            groupBoxPlotSettings = new GroupBox();
            buttonStickToZero = new Button();
            buttonResetScale = new Button();
            checkBoxLogScale = new CheckBox();
            checkBoxInvertData = new CheckBox();
            groupBoxDeviceInfo = new GroupBox();
            groupBoxSignalAnalize.SuspendLayout();
            groupBoxPlotSettings.SuspendLayout();
            groupBoxDeviceInfo.SuspendLayout();
            SuspendLayout();
            // 
            // comboBoxCom
            // 
            comboBoxCom.FormattingEnabled = true;
            comboBoxCom.Location = new Point(9, 62);
            comboBoxCom.Margin = new Padding(4, 5, 4, 5);
            comboBoxCom.Name = "comboBoxCom";
            comboBoxCom.Size = new Size(187, 33);
            comboBoxCom.TabIndex = 0;
            // 
            // plotView
            // 
            plotView.BackColor = SystemColors.ControlLightLight;
            plotView.Location = new Point(230, 20);
            plotView.Margin = new Padding(4, 5, 4, 5);
            plotView.Name = "plotView";
            plotView.PanCursor = Cursors.Hand;
            plotView.Size = new Size(1681, 1033);
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
            loggingBox.Location = new Point(9, 1063);
            loggingBox.Margin = new Padding(4, 5, 4, 5);
            loggingBox.Name = "loggingBox";
            loggingBox.Size = new Size(1901, 129);
            loggingBox.TabIndex = 8;
            // 
            // labelLog
            // 
            labelLog.AutoSize = true;
            labelLog.Location = new Point(9, 1028);
            labelLog.Margin = new Padding(4, 0, 4, 0);
            labelLog.Name = "labelLog";
            labelLog.Size = new Size(54, 25);
            labelLog.TabIndex = 9;
            labelLog.Text = "Logs:";
            // 
            // labelComPort
            // 
            labelComPort.AutoSize = true;
            labelComPort.Location = new Point(9, 32);
            labelComPort.Margin = new Padding(4, 0, 4, 0);
            labelComPort.Name = "labelComPort";
            labelComPort.Size = new Size(98, 25);
            labelComPort.TabIndex = 12;
            labelComPort.Text = "COM-port:";
            // 
            // labelSignalAverage
            // 
            labelSignalAverage.AutoSize = true;
            labelSignalAverage.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            labelSignalAverage.Location = new Point(9, 32);
            labelSignalAverage.Margin = new Padding(4, 0, 4, 0);
            labelSignalAverage.Name = "labelSignalAverage";
            labelSignalAverage.Size = new Size(140, 25);
            labelSignalAverage.TabIndex = 13;
            labelSignalAverage.Text = "Average Signal";
            // 
            // buttonClearLog
            // 
            buttonClearLog.Location = new Point(114, 1022);
            buttonClearLog.Margin = new Padding(4, 5, 4, 5);
            buttonClearLog.Name = "buttonClearLog";
            buttonClearLog.Size = new Size(107, 38);
            buttonClearLog.TabIndex = 14;
            buttonClearLog.Text = "Clear log";
            buttonClearLog.UseVisualStyleBackColor = true;
            // 
            // groupBoxSignalAnalize
            // 
            groupBoxSignalAnalize.Controls.Add(labelSNR);
            groupBoxSignalAnalize.Controls.Add(labelQFactor);
            groupBoxSignalAnalize.Controls.Add(labelSignalAverage);
            groupBoxSignalAnalize.Location = new Point(9, 137);
            groupBoxSignalAnalize.Margin = new Padding(4, 5, 4, 5);
            groupBoxSignalAnalize.Name = "groupBoxSignalAnalize";
            groupBoxSignalAnalize.Padding = new Padding(4, 5, 4, 5);
            groupBoxSignalAnalize.Size = new Size(213, 152);
            groupBoxSignalAnalize.TabIndex = 17;
            groupBoxSignalAnalize.TabStop = false;
            groupBoxSignalAnalize.Text = "Signal Analize";
            // 
            // labelSNR
            // 
            labelSNR.AutoSize = true;
            labelSNR.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            labelSNR.Location = new Point(9, 75);
            labelSNR.Margin = new Padding(4, 0, 4, 0);
            labelSNR.Name = "labelSNR";
            labelSNR.Size = new Size(48, 25);
            labelSNR.TabIndex = 17;
            labelSNR.Text = "SNR";
            // 
            // labelQFactor
            // 
            labelQFactor.AutoSize = true;
            labelQFactor.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            labelQFactor.Location = new Point(9, 117);
            labelQFactor.Margin = new Padding(4, 0, 4, 0);
            labelQFactor.Name = "labelQFactor";
            labelQFactor.Size = new Size(84, 25);
            labelQFactor.TabIndex = 16;
            labelQFactor.Text = "Q-factor";
            // 
            // groupBoxPlotSettings
            // 
            groupBoxPlotSettings.Controls.Add(buttonStickToZero);
            groupBoxPlotSettings.Controls.Add(buttonResetScale);
            groupBoxPlotSettings.Controls.Add(checkBoxLogScale);
            groupBoxPlotSettings.Controls.Add(checkBoxInvertData);
            groupBoxPlotSettings.Location = new Point(9, 298);
            groupBoxPlotSettings.Margin = new Padding(4, 5, 4, 5);
            groupBoxPlotSettings.Name = "groupBoxPlotSettings";
            groupBoxPlotSettings.Padding = new Padding(4, 5, 4, 5);
            groupBoxPlotSettings.Size = new Size(213, 207);
            groupBoxPlotSettings.TabIndex = 18;
            groupBoxPlotSettings.TabStop = false;
            groupBoxPlotSettings.Text = "Plot Settings";
            // 
            // buttonStickToZero
            // 
            buttonStickToZero.Location = new Point(9, 163);
            buttonStickToZero.Margin = new Padding(4, 5, 4, 5);
            buttonStickToZero.Name = "buttonStickToZero";
            buttonStickToZero.Size = new Size(119, 38);
            buttonStickToZero.TabIndex = 3;
            buttonStickToZero.Text = "Stick to zero";
            buttonStickToZero.UseVisualStyleBackColor = true;
            // 
            // buttonResetScale
            // 
            buttonResetScale.Location = new Point(9, 117);
            buttonResetScale.Margin = new Padding(4, 5, 4, 5);
            buttonResetScale.Name = "buttonResetScale";
            buttonResetScale.Size = new Size(119, 38);
            buttonResetScale.TabIndex = 2;
            buttonResetScale.Text = "Reset scale";
            buttonResetScale.UseVisualStyleBackColor = true;
            // 
            // checkBoxLogScale
            // 
            checkBoxLogScale.AutoSize = true;
            checkBoxLogScale.Location = new Point(9, 78);
            checkBoxLogScale.Margin = new Padding(4, 5, 4, 5);
            checkBoxLogScale.Name = "checkBoxLogScale";
            checkBoxLogScale.Size = new Size(113, 29);
            checkBoxLogScale.TabIndex = 1;
            checkBoxLogScale.Text = "Log Scale";
            checkBoxLogScale.UseVisualStyleBackColor = true;
            // 
            // checkBoxInvertData
            // 
            checkBoxInvertData.AutoSize = true;
            checkBoxInvertData.Location = new Point(9, 37);
            checkBoxInvertData.Margin = new Padding(4, 5, 4, 5);
            checkBoxInvertData.Name = "checkBoxInvertData";
            checkBoxInvertData.Size = new Size(125, 29);
            checkBoxInvertData.TabIndex = 0;
            checkBoxInvertData.Text = "Invert Data";
            checkBoxInvertData.UseVisualStyleBackColor = true;
            // 
            // groupBoxDeviceInfo
            // 
            groupBoxDeviceInfo.Controls.Add(comboBoxCom);
            groupBoxDeviceInfo.Controls.Add(labelComPort);
            groupBoxDeviceInfo.Location = new Point(9, 20);
            groupBoxDeviceInfo.Margin = new Padding(4, 5, 4, 5);
            groupBoxDeviceInfo.Name = "groupBoxDeviceInfo";
            groupBoxDeviceInfo.Padding = new Padding(4, 5, 4, 5);
            groupBoxDeviceInfo.Size = new Size(213, 107);
            groupBoxDeviceInfo.TabIndex = 19;
            groupBoxDeviceInfo.TabStop = false;
            groupBoxDeviceInfo.Text = "Current Device Info";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1929, 1215);
            Controls.Add(groupBoxDeviceInfo);
            Controls.Add(groupBoxPlotSettings);
            Controls.Add(groupBoxSignalAnalize);
            Controls.Add(buttonClearLog);
            Controls.Add(labelLog);
            Controls.Add(loggingBox);
            Controls.Add(plotView);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Spectrum Visualizer";
            Load += Form1_Load;
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
        private OxyPlot.WindowsForms.PlotView plotView;
        private ListBox loggingBox;
        private Label labelLog;
        private Label labelComPort;
        private Label labelSignalAverage;
        private Button buttonClearLog;
        private GroupBox groupBoxSignalAnalize;
        private Label labelQFactor;
        private GroupBox groupBoxPlotSettings;
        private CheckBox checkBoxLogScale;
        private CheckBox checkBoxInvertData;
        private Button buttonResetScale;
        private GroupBox groupBoxDeviceInfo;
        private Button buttonStickToZero;
        private Label labelSNR;
    }
}
