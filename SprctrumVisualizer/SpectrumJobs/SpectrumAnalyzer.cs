using Device.ATR.Model.Spectrometer;
using SpectrumVisualizer.Device;

namespace SpectrumVisualizer.SpectrumJobs
{
    /// <summary>
    /// Analyzes spectrum data, including dark spectrum subtraction and normalization.
    /// </summary>
    internal class SpectrumAnalyzer
    {
        private SpectrumNormalizer _spectrumNormilizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpectrumAnalyzer"/> class.
        /// </summary>
        /// <param name="deviceService">The device service instance.</param>
        public SpectrumAnalyzer()
        {
            _spectrumNormilizer = new SpectrumNormalizer();
        }

        /// <summary>
        /// Processes the raw spectrum data, applying dark spectrum subtraction and normalization.
        /// </summary>
        /// <param name="spectrum">The spectrum data to process, may include dark spectrum data.</param>
        /// <returns>A dictionary of wavelength-intensity pairs representing the analyzed spectrum.</returns>
        public Dictionary<double, double> Process(Spectrum spectrum, bool isInverse)
        {
            if (spectrum is not { Data: { } data })
            {
                ErrorHandler.Log(new ArgumentNullException(nameof(spectrum)));
                return new Dictionary<double, double>();
            }

            double[]? dark = spectrum.Dark;
            double factor = isInverse ? -1.0 : 1.0;

            for (int i = 0; i < DeviceGeneralInfo.DataSize; i++)
            {
                double value = data[i] - (dark?[i] ?? 0); // Substract dark if needed
                data[i] = Math.Max(value, 0) * factor; // Appling inversion and removing negative numbers
            }

            return _spectrumNormilizer.Process(spectrum);
        }
    }
}