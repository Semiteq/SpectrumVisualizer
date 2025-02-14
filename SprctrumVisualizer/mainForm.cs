using Device.ATR.Common.Utils;
using Device.ATR.Devices;
using SpectrumVisualizer.SpectrumJobs;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpectrumVisualizer
{    /// <summary>
     /// The main form of the Spectrum Visualizer application.
     /// Handles UI interactions, device connection, spectrum acquisition, and display.
     /// </summary>
    public partial class MainForm : Form
    {
        private readonly DeviceManager _deviceManager;   // Manages device connection and disconnection.
        private readonly SpectrumManager _spectrumManager; // Manages spectrum acquisition and analysis.
        private SpectrumPainter _spectrumPainter;      // Handles spectrum visualization using OxyPlot.
        private DeviceService _deviceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent(); // Initialize UI components from the designer.
            _deviceService = new DeviceService(); // Create an instance of DeviceService to interact with the device.

            _deviceManager = new DeviceManager(_deviceService); // Initialize DeviceManager with DeviceService.
            _spectrumManager = new SpectrumManager(new SpectrumAcquirer(_deviceService), new SpectrumAnalyzer()); // Initialize SpectrumManager with Acquirer and Analyzer.

            ErrorHandler.LoggingBox = loggingBox;

            // Event handlers for numericUpDown controls to update spectrum parameters.
            numericUpDownIntegration.ValueChanged += (s, e) => UpdateSpectrumParameters();
            numericUpDownInterval.ValueChanged += (s, e) => UpdateSpectrumParameters();
            numericUpDownAverage.ValueChanged += (s, e) => UpdateSpectrumParameters();


            // Event handlers for dark spectrum acquisition buttons.
            buttonAcquireDark.Click += (s, e) => _spectrumManager.SetConsiderDark(true);
            buttonResetDark.Click += (s, e) => _spectrumManager.SetConsiderDark(false);
            buttonSaveDark.Click += (s, e) => SaveDarkToFile(_spectrumManager.dark);
            buttonLoadDark.Click += (s, e) => _spectrumManager.SetDark(LoadDarkFromFile());
            checkBoxInvertData.CheckedChanged += (s, e) => _spectrumManager.FlipInvertFlag();
            
            buttonResetScale.Click += (s, e) => _spectrumPainter.ResetPlotScale();
            buttonStickToZero.Click += (s, e) => _spectrumPainter.StickToZero();
            checkBoxLogScale.CheckedChanged += (s, e) => _spectrumPainter.ToggleLogarithmicYAxis(checkBoxLogScale.Checked);

            buttonClearLog.Click += (s, e) => loggingBox.Items.Clear();
        }

        private void SaveDarkToFile(double[] data)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.Title = "Save as CSV";
                sfd.FileName = "data.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(sfd.FileName, string.Join(";", data.Select(d => d.ToString("G17"))));
                        MessageBox.Show("File saved succesfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (ArgumentNullException) 
                    {
                        MessageBox.Show("Warning, dark spectrum data is empty", "Dark spectrum is empty" , MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Log(ex);
                        MessageBox.Show("Saving failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public static double[] LoadDarkFromFile()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv";
                ofd.Title = "Open CSV file";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var file = File.ReadAllText(ofd.FileName)
                                   .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(s => double.TryParse(s, out double value) ? value : double.NaN)
                                   .ToArray();
                        return file;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Loading failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the spectrum acquisition parameters based on the values from numeric up-down controls.
        /// </summary>
        private void UpdateSpectrumParameters()
        {
            _spectrumManager.UpdateParameters( // Update parameters in SpectrumManager.
                (int)numericUpDownIntegration.Value,
                (int)numericUpDownInterval.Value,
                (int)numericUpDownAverage.Value
            );
        }

        /// <summary>
        /// Handles the Form Load event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void Form1_Load(object sender, EventArgs e)
        {
            await RepopulateComAndTryConnect();

            numericUpDownIntegration.Value = 15;
            numericUpDownInterval.Value = 0;
            numericUpDownAverage.Value = 1;
        }

        /// <summary>
        ///  Populates COM port dropdown, selects the first port, and attempts to connect.
        ///  Initializes numeric up-down controls with default values.
        /// </summary>
        private async Task RepopulateComAndTryConnect()
        {
            var coms = DeviceHelper.GetPortNames();
            comboBoxCom.Items.AddRange([.. coms]);
            if (comboBoxCom.Items.Count > 0)
            {
                comboBoxCom.SelectedIndex = 0;
                await TryConnectAsync();
            }
        }

        /// <summary>
        /// Attempts to connect to the device asynchronously using the selected COM port.
        /// If connection is successful, starts spectrum acquisition.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task TryConnectAsync()
        {
            if (await _deviceManager.ConnectAsync(comboBoxCom.SelectedItem.ToString())) // Connect to the selected COM port.
            {
                StartSpectrumAcquisition(); // Start spectrum acquisition if connection is successful.
                labelPN.Text = $"SN: {Device.DeviceGeneralInfo.PN}";
                labelSN.Text = $"PN: {Device.DeviceGeneralInfo.SN}";
            }
            else
            {                
                await RepopulateComAndTryConnect();
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Stops the spectrum acquisition process.
        /// </summary>
        private void StopSpectrumAcquisition()
        {
            _spectrumManager.StopAcquisition();
        }

        /// <summary>
        /// Starts the spectrum acquisition process and sets up the spectrum painter for visualization.
        /// </summary>
        private void StartSpectrumAcquisition()
        {
            _spectrumPainter = new SpectrumPainter();

            _spectrumManager.StartAcquisition(spectrum => // Start spectrum acquisition in SpectrumManager and provide a callback.
            {
                Invoke(() => // Invoke UI update on the main thread.
                {
                    _spectrumPainter.UpdateData(spectrum); // Update spectrum data in the painter.
                    plotView.Model = _spectrumPainter.GetPlotModel(); // Update the plot view with the painter's model.
                    
                    labelSignalAverage.Text = $"Average: {FormatNumber(_spectrumPainter.CalculateAverage())}";
                    labelSNR.Text = $"SNR: {FormatNumber(_spectrumPainter.CalculateSNR())} db";
                    labelQFactor.Text = $"Q-factor: {FormatNumber(_spectrumPainter.CalculateQFactor())}";
                });
            }, _deviceService);
        }

        private string FormatNumber(double value)
        {
            return (value > 10 || value < 0) ? value.ToString("0.00E0") : value.ToString("0.00");
        }

        /// <summary>
        /// Handles the Form Closing event. Disconnects the device and stops spectrum acquisition before closing.
        /// </summary>
        /// <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            await _deviceManager.DisconnectAsync();
            StopSpectrumAcquisition();
            base.OnFormClosing(e);
        }
    }
}