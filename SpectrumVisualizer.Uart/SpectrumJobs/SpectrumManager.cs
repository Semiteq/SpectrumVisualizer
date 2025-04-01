using SpectrumVisualizer.Uart.Message;

namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    /// <summary>
    /// Manages spectrum acquisition and basic processing.
    /// Receives complete spectrum data from UartSpectrumAcquirer in form of UartMessageData,
    /// optionally applies inversion, and converts it for display.
    /// </summary>
    public class SpectrumManager(SpectrumAcquirer acquirer)
    {
        private bool _isInverse = false;

        // Event handler with UartMessageData parameter.
        private Action<DataStruct>? _spectrumHandler;

        /// <summary>
        /// Toggles the inversion flag.
        /// </summary>
        public void FlipInvertFlag() => _isInverse = !_isInverse;

        /// <summary>
        /// Starts continuous spectrum acquisition.
        /// The updateUI callback is invoked with full UartMessageData including spectrum, average, SNR and quality.
        /// </summary>
        public void StartAcquisition(Action<Dictionary<double, double>> updateUi, Action<double, double, double> updateSpectrumInfo)
        {
            // Create a new event handler instance.
            _spectrumHandler = data =>
            {
                // If inversion is enabled, subtract each value from the maximum.
                if (_isInverse)
                {
                    ushort maxValue = data.Spectrum.Max();
                    for (var i = 0; i < data.Spectrum.Length; i++)
                    {
                        data.Spectrum[i] = (ushort)(maxValue - data.Spectrum[i]);
                    }
                }

                var dict = new Dictionary<double, double>();

                // Clarifying spectrometer type depending on spectrum length 1 for 2048, 2 for 512
                var spectrometerType = data.Spectrum.Length == MessageStruct1.SpectrumLength / 2 ? 1 : 2;

                for (var i = 0; i < data.Spectrum.Length; i++)
                {
                    dict.Add(SpectrumCalc.WaveLength(i, spectrometerType), data.Spectrum[i]);
                }

                updateUi(dict);
                updateSpectrumInfo(data.Average, data.Snr, data.Quality);
            };

            acquirer.SpectrumReceived += _spectrumHandler;
            acquirer.Start();
        }

        /// <summary>
        /// Stops the spectrum acquisition process.
        /// </summary>
        public void StopAcquisition()
        {
            acquirer.Stop();
            if (_spectrumHandler is not null)
            {
                acquirer.SpectrumReceived -= _spectrumHandler;
                _spectrumHandler = null;
            }
        }
    }
}
