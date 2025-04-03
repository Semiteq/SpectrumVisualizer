﻿namespace SpectrumVisualizer.Uart
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
            btnDisconnect = new Button();
            btnConnect = new Button();
            groupBoxSignalAnalize.SuspendLayout();
            groupBoxPlotSettings.SuspendLayout();
            groupBoxDeviceInfo.SuspendLayout();
            SuspendLayout();
            // 
            // comboBoxCom
            // 
            comboBoxCom.FormattingEnabled = true;
            comboBoxCom.Location = new Point(74, 17);
            comboBoxCom.Name = "comboBoxCom";
            comboBoxCom.Size = new Size(69, 23);
            comboBoxCom.TabIndex = 0;
            comboBoxCom.Click += comboBoxCom_Click;
            // 
            // plotView
            // 
            plotView.BackColor = SystemColors.ControlLightLight;
            plotView.Location = new Point(161, 12);
            plotView.Name = "plotView";
            plotView.PanCursor = Cursors.Hand;
            plotView.Size = new Size(1050, 600);
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
            loggingBox.Location = new Point(6, 618);
            loggingBox.Name = "loggingBox";
            loggingBox.Size = new Size(1205, 64);
            loggingBox.TabIndex = 8;
            // 
            // labelLog
            // 
            labelLog.AutoSize = true;
            labelLog.Location = new Point(6, 597);
            labelLog.Name = "labelLog";
            labelLog.Size = new Size(35, 15);
            labelLog.TabIndex = 9;
            labelLog.Text = "Logs:";
            // 
            // labelComPort
            // 
            labelComPort.AutoSize = true;
            labelComPort.Location = new Point(6, 20);
            labelComPort.Name = "labelComPort";
            labelComPort.Size = new Size(65, 15);
            labelComPort.TabIndex = 12;
            labelComPort.Text = "COM-port:";
            // 
            // labelSignalAverage
            // 
            labelSignalAverage.AutoSize = true;
            labelSignalAverage.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            labelSignalAverage.Location = new Point(6, 20);
            labelSignalAverage.Name = "labelSignalAverage";
            labelSignalAverage.Size = new Size(90, 15);
            labelSignalAverage.TabIndex = 13;
            labelSignalAverage.Text = "Average Signal";
            // 
            // buttonClearLog
            // 
            buttonClearLog.Location = new Point(80, 590);
            buttonClearLog.Name = "buttonClearLog";
            buttonClearLog.Size = new Size(75, 27);
            buttonClearLog.TabIndex = 14;
            buttonClearLog.Text = "Clear log";
            buttonClearLog.UseVisualStyleBackColor = true;
            // 
            // groupBoxSignalAnalize
            // 
            groupBoxSignalAnalize.Controls.Add(labelSNR);
            groupBoxSignalAnalize.Controls.Add(labelQFactor);
            groupBoxSignalAnalize.Controls.Add(labelSignalAverage);
            groupBoxSignalAnalize.Location = new Point(6, 91);
            groupBoxSignalAnalize.Name = "groupBoxSignalAnalize";
            groupBoxSignalAnalize.Size = new Size(149, 92);
            groupBoxSignalAnalize.TabIndex = 17;
            groupBoxSignalAnalize.TabStop = false;
            groupBoxSignalAnalize.Text = "Signal Analize";
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
            groupBoxPlotSettings.Location = new Point(6, 187);
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
            buttonStickToZero.Size = new Size(91, 22);
            buttonStickToZero.TabIndex = 3;
            buttonStickToZero.Text = "Stick to zero";
            buttonStickToZero.UseVisualStyleBackColor = true;
            // 
            // buttonResetScale
            // 
            buttonResetScale.Location = new Point(6, 70);
            buttonResetScale.Name = "buttonResetScale";
            buttonResetScale.Size = new Size(91, 22);
            buttonResetScale.TabIndex = 2;
            buttonResetScale.Text = "Reset scale";
            buttonResetScale.UseVisualStyleBackColor = true;
            // 
            // checkBoxLogScale
            // 
            checkBoxLogScale.AutoSize = true;
            checkBoxLogScale.Location = new Point(6, 46);
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
            groupBoxDeviceInfo.Controls.Add(btnDisconnect);
            groupBoxDeviceInfo.Controls.Add(btnConnect);
            groupBoxDeviceInfo.Controls.Add(comboBoxCom);
            groupBoxDeviceInfo.Controls.Add(labelComPort);
            groupBoxDeviceInfo.Location = new Point(6, 12);
            groupBoxDeviceInfo.Name = "groupBoxDeviceInfo";
            groupBoxDeviceInfo.Size = new Size(149, 73);
            groupBoxDeviceInfo.TabIndex = 19;
            groupBoxDeviceInfo.TabStop = false;
            groupBoxDeviceInfo.Text = "Current Device:";
            // 
            // btnDisconnect
            // 
            btnDisconnect.Font = new Font("Segoe UI", 9F);
            btnDisconnect.Location = new Point(74, 44);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(69, 23);
            btnDisconnect.TabIndex = 14;
            btnDisconnect.Text = "Drop";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(6, 44);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(65, 23);
            btnConnect.TabIndex = 13;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1218, 711);
            Controls.Add(groupBoxDeviceInfo);
            Controls.Add(groupBoxPlotSettings);
            Controls.Add(groupBoxSignalAnalize);
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
        private Button btnDisconnect;
        private Button btnConnect;
    }
}
