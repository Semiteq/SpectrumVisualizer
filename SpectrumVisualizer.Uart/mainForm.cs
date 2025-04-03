// MainForm.cs
using System.IO.Ports;
using SpectrumVisualizer.Uart.Device;
using SpectrumVisualizer.Uart.SpectrumJobs;
using Timer = System.Threading.Timer;

namespace SpectrumVisualizer.Uart
{
    public partial class MainForm : Form
    {
        private readonly DeviceManager _deviceManager; // Manages device connection/disconnection via SerialPortStream.
        private SpectrumManager _spectrumManager;       // Manages spectrum acquisition and analysis.
        private SpectrumPainter _spectrumPainter;       // Handles spectrum visualization via OxyPlot.
        private readonly Timer _statusTimer;            // Timer to check connection and update COM port list.

        private bool _isConnecting;
        public MainForm()
        {
            InitializeComponent();

            // Set logging UI element (если требуется, иначе можно удалить)
            EventHandler.LoggingBox = loggingBox;

            // Setup UI controls event handlers
            checkBoxInvertData.CheckedChanged += (s, e) => _spectrumManager?.FlipInvertFlag();
            buttonResetScale.Click += (s, e) => _spectrumPainter?.ResetPlotScale();
            buttonStickToZero.Click += (s, e) => _spectrumPainter?.StickToZero();
            checkBoxLogScale.CheckedChanged += (s, e) => _spectrumPainter?.ToggleLogarithmicYAxis(checkBoxLogScale.Checked);
            buttonClearLog.Click += (s, e) => loggingBox.Items.Clear();

            _deviceManager = new DeviceManager();

            // Timer fires every 1 second; callback posts work to UI thread.
            _statusTimer = new Timer(CheckDeviceStatus, null, 0, 1000);

            btnDisconnect.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RepopulateCom();
        }

        private void RepopulateCom()
        {
            var coms = SerialPort.GetPortNames();
            comboBoxCom.Items.Clear();
            comboBoxCom.Items.AddRange(coms);
            if (comboBoxCom.Items.Count > 0)
                comboBoxCom.SelectedIndex = 0;
        }

        /// <summary>
        /// Attempts to connect to the device using the selected COM port.
        /// If a previous device is connected, it will be disconnected first.
        /// </summary>
        private async Task TryConnectAsync()
        {
            if (comboBoxCom.SelectedItem == null)
                return;

            var portName = comboBoxCom.SelectedItem.ToString();
            if (string.IsNullOrEmpty(portName))
                return;

            if (_deviceManager.IsConnected)
            {
                StopSpectrumAcquisition();
                await _deviceManager.DisconnectAsync();
            }

            if (await _deviceManager.ConnectAsync(portName))
            {
                StartSpectrumAcquisition();
                groupBoxDeviceInfo.Text = $"Current Device: {portName}";
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
            }
            else
            {
                groupBoxDeviceInfo.Text = "Current Device:";
            }
        }

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

        private void StopSpectrumAcquisition()
        {
            _spectrumManager?.StopAcquisition();
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            await _statusTimer.DisposeAsync();
            await _deviceManager.DisconnectAsync();
            StopSpectrumAcquisition();
            base.OnFormClosing(e);
        }

        private void comboBoxCom_Click(object sender, EventArgs e)
        {
            // Refresh COM port list while preserving current selection if available.
            var currentSelection = comboBoxCom.SelectedItem?.ToString();
            var ports = SerialPort.GetPortNames();
            comboBoxCom.Items.Clear();
            comboBoxCom.Items.AddRange(ports);
            if (currentSelection != null && comboBoxCom.Items.Contains(currentSelection))
                comboBoxCom.SelectedItem = currentSelection;
            else if (comboBoxCom.Items.Count > 0)
                comboBoxCom.SelectedIndex = 0;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (_isConnecting || comboBoxCom.SelectedItem == null)
                return;

            try
            {
                _isConnecting = true;
                btnConnect.Enabled = false;
                await TryConnectAsync();
            }
            catch (Exception ex)
            {
                EventHandler.Log($"Error connecting: {ex.Message}");
            }
            finally
            {
                _isConnecting = false;
                if (!_deviceManager.IsConnected)
                {
                    btnConnect.Enabled = true;
                }
            }
        }

        private async void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                await _deviceManager.DisconnectAsync();
                StopSpectrumAcquisition();
                groupBoxDeviceInfo.Text = "Current Device:";
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
            }
            catch (Exception ex)
            {
                EventHandler.Log($"Error disconnecting: {ex.Message}");
            }
        }

        /// <summary>
        /// Timer callback that posts work to the UI thread.
        /// </summary>
        private void CheckDeviceStatus(object? state)
        {
            if (!IsHandleCreated)
                return;

            BeginInvoke(new Action(CheckDeviceStatusUI));
        }

        /// <summary>
        /// Checks the device status and updates the COM port dropdown.
        /// Removes COM ports that are no longer available.
        /// </summary>
        private async void CheckDeviceStatusUI()
        {
            try
            {
                var availablePorts = SerialPort.GetPortNames();
                var currentPort = _deviceManager.Acquirer?.SerialPort?.PortName;

                // If the device is connected and the current port is not in the available ports,
                if (_deviceManager.IsConnected &&
                   (currentPort == null || !availablePorts.Contains(currentPort)))
                {
                    StopSpectrumAcquisition();
                    await _deviceManager.DisconnectAsync();
                    groupBoxDeviceInfo.Text = "Current Device:";
                    btnConnect.Enabled = true;
                    btnDisconnect.Enabled = false;
                }

                // Update the COM port list in the dropdown.
                var currentItems = comboBoxCom.Items.Cast<string>().ToList();

                // If the current port is not in the available ports, remove it from the list.
                if (!currentItems.SequenceEqual(availablePorts))
                {
                    comboBoxCom.Text = "";
                    comboBoxCom.Items.Clear();
                    comboBoxCom.Items.AddRange(availablePorts);

                    // If the current port is still available, select it; otherwise, select the first available port.
                    if (comboBoxCom.Items.Contains(currentPort))
                    {
                        comboBoxCom.SelectedItem = currentPort;
                    }
                    else if (comboBoxCom.Items.Count > 0)
                    {
                        comboBoxCom.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                EventHandler.Log($"Error in status check: {ex.Message}");
            }
        }
    }
}
