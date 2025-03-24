namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    /// <summary>
    /// Manages spectrum acquisition and basic processing.
    /// Receives combined spectrum data from UartSpectrumAcquirer,
    /// optionally applies inversion, and converts it for display.
    /// </summary>
    public class SpectrumManager
    {
        private readonly UartSpectrumAcquirer _acquirer;
        private bool _isInverse = false;

        // Store the event handler so it can be unsubscribed.
        private Action<ushort[]>? _spectrumHandler;

        public SpectrumManager(UartSpectrumAcquirer acquirer)
        {
            _acquirer = acquirer;
        }

        /// <summary>
        /// Toggles the inversion flag.
        /// </summary>
        public void FlipInvertFlag() => _isInverse = !_isInverse;

        /// <summary>
        /// Starts continuous spectrum acquisition.
        /// The updateChart callback is invoked with a dictionary mapping index (as wavelength) to intensity.
        /// </summary>
        public void StartAcquisition(Action<Dictionary<double, double>> updateChart)
        {
            // Create a new event handler instance.
            _spectrumHandler = combined =>
            {
                // Convert data to double.
                var data = combined.Select(x => (double)x).ToArray();

                // If inversion is enabled, subtract each value from the maximum.
                if (_isInverse)
                {
                    var maxValue = data.Max();
                    for (var i = 0; i < data.Length; i++)
                    {
                        data[i] = maxValue - data[i]; // Inversion
                    }
                }

                // Create dictionary for chart update.
                var dict = new Dictionary<double, double>();
                for (var i = 0; i < data.Length; i++)
                {
                    dict.Add(i, data[i]);
                }

                updateChart(dict);
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
