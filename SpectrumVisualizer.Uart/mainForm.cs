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
            if (await _deviceManager.ConnectAsync(comboBoxCom.SelectedItem.ToString()))
            {
                StartSpectrumAcquisition();
            }
            else
            {
                await Task.Delay(1000);
                await RepopulateComAndTryConnect();
            }
        }

        /// <summary>
        /// Starts the spectrum acquisition process and sets up visualization.
        /// </summary>
        private void StartSpectrumAcquisition()
        {
            _spectrumPainter = new SpectrumPainter();

            _spectrumManager = new SpectrumManager(_deviceManager.Acquirer!);

            _spectrumManager.StartAcquisition(spectrum =>
            {
                Invoke(() =>
                {
                    _spectrumPainter.UpdateData(spectrum);
                    plotView.Model = _spectrumPainter.GetPlotModel();
                });
            });
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
    }
}
