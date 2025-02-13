using Device.ATR.Common.Utils;
using Device.ATR.Devices;
using Device.ATR.Model.Spectrometer;
using SpectrumVisualizer.SpectrumJobs;

namespace SpectrumVisualizer
{
    public partial class MainForm : Form
    {
        private readonly SpectrumJobs.SpectrumAcquirer _spectrumAcquirer;
        private readonly SpectrumJobs.SpectrumAnalyzer _spectrumAnalyzer;
        private readonly Device.Connector _deviceConnector;
        private readonly Device.DataProcessor _dataProcessor;
        private Spectrum _spectrum;

        private bool considerDark = false;

        private readonly AcquireParameter _spectrumParameter;

        private readonly DeviceService _deviceService;

        private bool _isConnected = false;

        // Добавляем семафор для синхронизации доступа к DeviceService
        private readonly SemaphoreSlim _deviceServiceSemaphore = new(1, 1);

        public MainForm()
        {
            InitializeComponent();

            _deviceService = new DeviceService();

            _spectrumAcquirer = new(_deviceService);
            _spectrumAnalyzer = new(_deviceService);
            _deviceConnector = new(_deviceService);
            _dataProcessor = new();
            _spectrum = new();

            _spectrumParameter = new();

            ErrorHandler.LoggingBox = loggingBox;
        }

        private void EnableControls(bool flag)
        {
            buttonAcquireDark.Enabled = flag;
            buttonResetDark.Enabled = flag;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Set default values
            numericUpDownIntegration.Value = 15;
            numericUpDownInterval.Value = 0;
            numericUpDownAverage.Value = 1;

            EnableControls(false);

            var coms = DeviceHelper.GetPortNames();
            comboBoxCom.Items.AddRange([.. coms]);

            if (comboBoxCom.Items.Count > 0)
            {
                EnableControls(true);
                comboBoxCom.SelectedIndex = 0;
                await TryConnectAsync();
            }
        }

        private async void ComboBoxCom_SelectedIndexChanged(object sender, EventArgs e)
        {
            await TryConnectAsync();
        }

        private async Task TryConnectAsync()
        {
            try
            {
                if (_isConnected)
                {
                    Disconnect();
                }

                var com = comboBoxCom.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(com))
                    return;

                await _deviceServiceSemaphore.WaitAsync();
                bool success = false;
                try
                {
                    success = await _deviceService.Open(com);
                }
                finally
                {
                    _deviceServiceSemaphore.Release();
                }


                _isConnected = success;

                if (success)
                {
                    StartSpectrumAcquisition();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }
        private void Disconnect()
        {
            StopSpectrumAcquisition();
            _deviceConnector.Disconnect();
            _isConnected = false;
            _dataProcessor.Dispose();
            EnableControls(false);
        }

        private CancellationTokenSource? _acquisitionCts;

        private void StartSpectrumAcquisition()
        {
            if (_acquisitionCts != null)
                return; // Если процесс уже запущен, не запускаем повторно

            _acquisitionCts = new CancellationTokenSource();
            var token = _acquisitionCts.Token;

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        if (!_isConnected) break;

                        // Безопасное ожидание семафора
                        await _deviceServiceSemaphore.WaitAsync(token);
                        try
                        {
                            _spectrum.Data = await _spectrumAcquirer.AcquireAsync(_spectrumParameter);
                            _spectrum.Dark = await _spectrumAcquirer.AcquireAsync(_spectrumParameter, considerDark);
                        }
                        finally
                        {
                            _deviceServiceSemaphore.Release();
                        }

                        if (_spectrum != null)
                        {
                            await UpdateChartAsync(_spectrum);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break; // Корректное завершение по запросу
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Handle(ex);
                    }

                    await Task.Delay(100, token); // Небольшая задержка для снижения нагрузки
                }
            }, token);
        }

        private void StopSpectrumAcquisition()
        {
            _dataProcessor.Dispose();
            _acquisitionCts?.Cancel();
            _acquisitionCts?.Dispose();
            _acquisitionCts = null;
        }


        private void ParameterUpdate()
        {
            _spectrumParameter.IntegrationTime = (int)numericUpDownIntegration.Value;
            _spectrumParameter.Interval = (int)numericUpDownInterval.Value;
            _spectrumParameter.Average = (int)numericUpDownAverage.Value;
        }

        private void numericUpDownIntegration_ValueChanged(object sender, EventArgs e) => ParameterUpdate();
        private void numericUpDownInterval_ValueChanged(object sender, EventArgs e) => ParameterUpdate();
        private void numericUpDownAverage_ValueChanged(object sender, EventArgs e) => ParameterUpdate();

        private async Task UpdateChartAsync(Spectrum spectrumToAnalyze)
        {
            if (spectrumToAnalyze == null)
                return;

            Dictionary<double, double>? spectrumAnalized;

            spectrumAnalized = await _spectrumAnalyzer.ProcessAsync(spectrumToAnalyze, considerDark);

            if (spectrumAnalized is { Count: > 0 })
            {
                Invoke(() => SpectrumPainter.Process(spectrumAnalized, plotView));
            }
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Disconnect();
            _dataProcessor.Dispose();
            base.OnFormClosing(e);
        }

        private void buttonAcquireDark_Click(object sender, EventArgs e)
        {
            considerDark = true;
        }

        private void buttonResetDark_Click(object sender, EventArgs e)
        {
            considerDark = false;
        }
    }
}