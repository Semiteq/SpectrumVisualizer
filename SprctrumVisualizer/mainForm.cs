using Device.ATR.Common.Utils;
using Device.ATR.Devices;
using Device.ATR.Model.Spectrometer;
using SpectrumVisualizer.SpectrumJobs;

namespace SpectrumVisualizer
{
    public partial class MainForm : Form
    {
        private readonly DeviceManager _deviceManager;
        private readonly SpectrumManager _spectrumManager;
        
        public MainForm()
        {
            InitializeComponent();
            var deviceService = new DeviceService();

            _deviceManager = new DeviceManager(deviceService);
            _spectrumManager = new SpectrumManager(new SpectrumAcquirer(deviceService), new SpectrumAnalyzer(deviceService));

            // �������� �� ������� ��������� ����������
            numericUpDownIntegration.ValueChanged += (s, e) => UpdateSpectrumParameters();
            numericUpDownInterval.ValueChanged += (s, e) => UpdateSpectrumParameters();
            numericUpDownAverage.ValueChanged += (s, e) => UpdateSpectrumParameters();

            // �������� �� ������
            buttonAcquireDark.Click += (s, e) => _spectrumManager.SetConsiderDark(true);
            buttonResetDark.Click += (s, e) => _spectrumManager.SetConsiderDark(false);
        }

        private void UpdateSpectrumParameters()
        {
            _spectrumManager.UpdateParameters(
                (int)numericUpDownIntegration.Value,
                (int)numericUpDownInterval.Value,
                (int)numericUpDownAverage.Value
            );
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var coms = DeviceHelper.GetPortNames();
            comboBoxCom.Items.AddRange([.. coms]);
            if (comboBoxCom.Items.Count > 0)
            {
                comboBoxCom.SelectedIndex = 0;
                await TryConnectAsync();
            }

            // ��������� ��������
            numericUpDownIntegration.Value = 15;
            numericUpDownInterval.Value = 0;
            numericUpDownAverage.Value = 1;
        }

        private async Task TryConnectAsync()
        {
            if (await _deviceManager.ConnectAsync(comboBoxCom.SelectedItem.ToString()))
            {
                StartSpectrumAcquisition();
            }
        }

        private void StartSpectrumAcquisition()
        {
            SpectrumPainter _spectrumPainter;
            _spectrumPainter = new();
            _spectrumManager.StartAcquisition(spectrum =>
            {
                Invoke(() =>
                {

                    _spectrumPainter.UpdateData(spectrum);

                    plotView.Model = _spectrumPainter.GetPlotModel();
                    labelSignalAverage.Text = $"Average: {Math.Round(_spectrumPainter.CalculateAverage(),2).ToString()}";
                });
            });
        }

        private void StopSpectrumAcquisition()
        {
            _spectrumManager.StopAcquisition();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _deviceManager.Disconnect();
            StopSpectrumAcquisition();
            base.OnFormClosing(e);
        }
    }
}