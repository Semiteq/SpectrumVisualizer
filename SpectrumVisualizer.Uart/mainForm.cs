using System.IO.Ports;
using SpectrumVisualizer.Uart.Device;
using SpectrumVisualizer.Uart.SpectrumJobs;

namespace SpectrumVisualizer.Uart
{
    /// <summary>
    /// The main form of the Spectrum Visualizer application.
    /// Handles UI interactions, device connection, spectrum acquisition, and display.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly DeviceManager _deviceManager;         // Manages device connection/disconnection via UART.
        private SpectrumManager _spectrumManager;                // Manages spectrum acquisition and analysis.
        private SpectrumPainter _spectrumPainter;                // Handles spectrum visualization via OxyPlot.

        public MainForm()
        {
            InitializeComponent();

            ErrorHandler.LoggingBox = loggingBox;

            // Invert flag toggling for display (inversion is applied in SpectrumManager)
            checkBoxInvertData.CheckedChanged += (s, e) => _spectrumManager.FlipInvertFlag();

            buttonResetScale.Click += (s, e) => _spectrumPainter.ResetPlotScale();
            buttonStickToZero.Click += (s, e) => _spectrumPainter.StickToZero();
            checkBoxLogScale.CheckedChanged += (s, e) => _spectrumPainter.ToggleLogarithmicYAxis(checkBoxLogScale.Checked);
            buttonClearLog.Click += (s, e) => loggingBox.Items.Clear();

            _deviceManager = new DeviceManager();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await RepopulateComAndTryConnect();
        }

        /// <summary>
        /// Populates COM port dropdown, selects the first port, and attempts to connect.
        /// </summary>
        private async Task RepopulateComAndTryConnect()
        {
            var coms = SerialPort.GetPortNames();
            comboBoxCom.Items.AddRange(coms);
            if (comboBoxCom.Items.Count > 0)
            {
                comboBoxCom.SelectedIndex = 0;
                await TryConnectAsync();
            }
        }

        /// <summary>
        /// Attempts to connect to the device asynchronously using the selected COM port.
        /// </summary>
        private async Task TryConnectAsync()
        {
            if (comboBoxCom.SelectedItem == null) return;

            var portName = comboBoxCom.SelectedItem.ToString();
            if (string.IsNullOrEmpty(portName)) return;

            if (await _deviceManager.ConnectAsync(portName))
            {
                StartSpectrumAcquisition();
                ErrorHandler.Log($"Connected to {portName}");
            }
            else
            {
                ErrorHandler.Log($"Failed to connect to {portName}");
            }
        }

        /// <summary>
        /// Starts the spectrum acquisition process and sets up visualization.
        /// </summary>
        private void StartSpectrumAcquisition()
        {
            _spectrumPainter = new SpectrumPainter();
            _spectrumManager = new SpectrumManager(_deviceManager.Acquirer!);

            _spectrumManager.StartAcquisition(
                spectrum =>
                {
                    Invoke((Action)(() =>
                    {
                        _spectrumPainter.UpdateData(spectrum);
                        plotView.Model = _spectrumPainter.GetPlotModel();
                    }));
                },
                (average, snr, quality) =>
                {
                    Invoke((Action)(() =>
                    {
                        labelSignalAverage.Text = $"Average: {average:F2}";
                        labelSNR.Text = $"SNR: {snr:F2}";
                        labelQFactor.Text = $"Quality: {quality:F2}";
                    }));
                }
            );
        }

        /// <summary>
        /// Stops the spectrum acquisition process.
        /// </summary>
        private void StopSpectrumAcquisition()
        {
            _spectrumManager?.StopAcquisition();
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            await _deviceManager.DisconnectAsync();
            StopSpectrumAcquisition();
            base.OnFormClosing(e);
        }

        private void comboBoxCom_Click(object sender, EventArgs e)
        {
            var currentSelection = comboBoxCom.SelectedItem?.ToString();
            comboBoxCom.Items.Clear();
            comboBoxCom.Items.AddRange(SerialPort.GetPortNames());

            // Don't trigger selection change if selecting the same port
            comboBoxCom.SelectedIndexChanged -= comboBoxCom_SelectedIndexChanged;
            if (currentSelection != null && comboBoxCom.Items.Contains(currentSelection))
            {
                comboBoxCom.SelectedItem = currentSelection;
            }
            else if (comboBoxCom.Items.Count > 0)
            {
                comboBoxCom.SelectedIndex = 0;
            }
            comboBoxCom.SelectedIndexChanged += comboBoxCom_SelectedIndexChanged;
        }

        private async void comboBoxCom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCom.SelectedItem == null) return;

            try
            {
                await _deviceManager.DisconnectAsync();
                StopSpectrumAcquisition();
                await TryConnectAsync();
            }
            catch (Exception ex)
            {
                ErrorHandler.Log($"Error connecting to {comboBoxCom.SelectedItem}: {ex.Message}");
            }
        }
    }
}
