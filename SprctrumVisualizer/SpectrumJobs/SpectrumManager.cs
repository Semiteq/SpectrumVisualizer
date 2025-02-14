using Device.ATR.Devices;
using Device.ATR.Model.Spectrometer;
using SpectrumVisualizer.Device;
using SpectrumVisualizer.SpectrumJobs;

namespace SpectrumVisualizer
{
    /// <summary>
    /// Manages the spectrum acquisition and analysis process, including handling dark spectrum subtraction and parameter updates.
    /// </summary>
    internal class SpectrumManager
    {
        private readonly SpectrumAcquirer _spectrumAcquirer;
        private readonly SpectrumAnalyzer _spectrumAnalyzer;
        private AcquireParameter _parameters = new();
        private bool _considerDark = false;
        private bool _readDarkFlag = false;
        private CancellationTokenSource? _cts;
        private bool IsInverse = false;
        public double[]? dark { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpectrumManager"/> class.
        /// </summary>
        /// <param name="acquirer">The spectrum acquirer instance.</param>
        /// <param name="analyzer">The spectrum analyzer instance.</param>
        public SpectrumManager(SpectrumAcquirer acquirer, SpectrumAnalyzer analyzer)
        {
            dark = null;
            _spectrumAcquirer = acquirer;
            _spectrumAnalyzer = analyzer;
        }

        public void FlipInvertFlag()
        {
            IsInverse = !IsInverse;
        }

        public void SetDark(double[] d)
        {
            dark = d;
            _considerDark = true;
            _readDarkFlag = false;
        }

        /// <summary>
        /// Updates the spectrum acquisition parameters.
        /// </summary>
        /// <param name="integrationTime">The integration time in milliseconds.</param>
        /// <param name="interval">The interval between acquisitions.</param>
        /// <param name="average">The number of spectra to average.</param>
        public void UpdateParameters(int integrationTime, int interval, int average)
        {
            _parameters.IntegrationTime = integrationTime;
            _parameters.Interval = interval;
            _parameters.Average = average;
        }

        /// <summary>
        /// Sets whether to consider dark spectrum subtraction.
        /// </summary>
        /// <param name="considerDark">True to consider dark spectrum; otherwise, false.</param>
        public void SetConsiderDark(bool considerDark)
        {
            _considerDark = considerDark;
            _readDarkFlag = considerDark; // Set flag to read dark spectrum if enabling dark subtraction.
        }

        /// <summary>
        /// Starts continuous spectrum acquisition.
        /// </summary>
        /// <param name="updateChart">Action to update the chart with processed spectrum data.</param>
        public void StartAcquisition(Action<Dictionary<double, double>> updateChart, DeviceService ds)
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            DeviceGeneralInfo.InitializeAsync(ds); // Update general data

            Task.Run(async () => // Start a new task to run the acquisition loop.
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var spectrum = await _spectrumAcquirer.AcquireAsync(_parameters); // Acquire a spectrum.

                        // If dark spectrum is enabled and not yet read, acquire dark spectrum.
                        if (_considerDark && _readDarkFlag)
                        {
                            dark = await _spectrumAcquirer.AcquireAsync(_parameters, true); // Acquire dark spectrum.
                            _readDarkFlag = false; // Reset flag after first dark spectrum acquisition.
                        }
                        // If dark spectrum is disabled, clear the dark spectrum data.
                        else if (!_considerDark)
                        {
                            dark = null;
                        }

                        var analyzed = await Task.Run(() => _spectrumAnalyzer.Process( // Analyze the acquired spectrum.
                            new Spectrum { Data = spectrum, Dark = dark }
                        , IsInverse));

                        updateChart(analyzed);
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Handle(ex);
                    }

                    await Task.Delay(100, token); // Minimal delay for empty task
                }
            }, token);
        }

        /// <summary>
        /// Stops the spectrum acquisition process.
        /// </summary>
        public void StopAcquisition()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}