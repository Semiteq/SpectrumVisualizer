namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    /// <summary>
    /// Manages spectrum acquisition and basic processing.
    /// Receives complete spectrum data from UartSpectrumAcquirer in form of UartMessageData,
    /// optionally applies inversion, and converts it for display.
    /// </summary>
    public class SpectrumManager
    {
        private readonly SpectrumAcquirer _acquirer;
        private bool _isInverse = false;

        // Event handler with UartMessageData parameter.
        private Action<DataStruct>? _spectrumHandler;

        public SpectrumManager(SpectrumAcquirer acquirer)
        {
            _acquirer = acquirer;
        }

        /// <summary>
        /// Toggles the inversion flag.
        /// </summary>
        public void FlipInvertFlag() => _isInverse = !_isInverse;

        /// <summary>
        /// Starts continuous spectrum acquisition.
        /// The updateUI callback is invoked with full UartMessageData including spectrum, average, SNR and quality.
        /// </summary>
        public void StartAcquisition(Action<Dictionary<double, double>> updateUI, Action<double, double, double> updateSpectrumInfo)
        {
            // Create a new event handler instance.
            _spectrumHandler = data =>
            {
                // If inversion is enabled, subtract each value from the maximum.
                if (_isInverse)
                {
                    var maxValue = data.Spectrum.Max();
                    for (var i = 0; i < data.Spectrum.Length; i++)
                    {
                        data.Spectrum[i] = maxValue - data.Spectrum[i];
                    }
                }

                var dict = new Dictionary<double, double>();
                for (var i = 0; i < data.Spectrum.Length; i++)
                {
                    dict.Add(i, data.Spectrum[i]);
                }

                updateUI(dict);
                updateSpectrumInfo(data.Average, data.Snr, data.Quality);
            };

            _acquirer.SpectrumReceived += _spectrumHandler;
            _acquirer.Start();
        }

        /// <summary>
        /// Stops the spectrum acquisition process.
        /// </summary>
        public void StopAcquisition()
        {
            _acquirer.Stop();
            if (_spectrumHandler is not null)
            {
                _acquirer.SpectrumReceived -= _spectrumHandler;
                _spectrumHandler = null;
            }
        }
    }
}
