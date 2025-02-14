using OxyPlot.Annotations;

namespace SpectrumVisualizer.SpectrumJobs
{
    /// <summary>
    /// Provides methods for calculating various spectrum parameters such as average intensity, SNR, and quality factor.
    /// </summary>
    internal static class SpectrumCalculator
    {
        /// <summary>
        /// Calculates the average Y-value within the range defined by the left and right markers.
        /// </summary>
        /// <param name="_xData">Array of X-axis values.</param>
        /// <param name="_yData">Array of Y-axis values.</param>
        /// <param name="_leftMarker">Left boundary marker.</param>
        /// <param name="_rightMarker">Right boundary marker.</param>
        /// <returns>Average Y-value within the specified range, or 0 if input data is invalid.</returns>
        public static double CalculateAverage(double[] _xData, double[] _yData, LineAnnotation _leftMarker, LineAnnotation _rightMarker)
        {
            if (_xData == null || _yData == null || _leftMarker == null || _rightMarker == null) return 0;

            int start = System.Array.FindIndex(_xData, x => x >= _leftMarker.X);
            int end = System.Array.FindIndex(_xData, x => x >= _rightMarker.X);

            if (start < 0 || end < 0 || start >= end) return 0;

            return _yData.Skip(start).Take(end - start).Average();
        }

        /// <summary>
        /// Calculates the Signal-to-Noise Ratio (SNR) within the specified range.
        /// </summary>
        /// <param name="_xData">Array of X-axis values.</param>
        /// <param name="_yData">Array of Y-axis values.</param>
        /// <param name="_leftMarker">Left boundary marker.</param>
        /// <param name="_rightMarker">Right boundary marker.</param>
        /// <returns>SNR value, or NaN if input data is invalid.</returns>
        public static double CalculateSNR(double[] _xData, double[] _yData, LineAnnotation _leftMarker, LineAnnotation _rightMarker)
        {
            if (_xData == null || _yData == null || _leftMarker == null || _rightMarker == null)
                return double.NaN;

            int start = Array.FindIndex(_xData, x => x >= _leftMarker.X);
            int end = Array.FindIndex(_xData, x => x >= _rightMarker.X);

            if (start < 0 || end < 0 || start >= end)
                return double.NaN;

            // Calculate signal as the average Y-value within the selected range
            double signal = _yData.Skip(start).Take(end - start).Average();

            // Extract noise data (values outside the selected range)
            var noiseData = _yData.Take(start).Concat(_yData.Skip(end)).ToArray();
            if (noiseData.Length == 0)
                return double.NaN;

            // Compute noise RMS value
            double noiseRMS = Math.Sqrt(noiseData.Average(y => y * y));

            return signal / noiseRMS;
        }

        /// <summary>
        /// Calculates the quality factor (Q-factor) of the signal.
        /// </summary>
        /// <param name="_xData">Array of X-axis values.</param>
        /// <param name="_yData">Array of Y-axis values.</param>
        /// <param name="_leftMarker">Left boundary marker.</param>
        /// <param name="_rightMarker">Right boundary marker.</param>
        /// <returns>Q-factor value, or NaN if input data is invalid.</returns>
        public static double CalculateQFactor(double[] _xData, double[] _yData, LineAnnotation _leftMarker, LineAnnotation _rightMarker)
        {
            if (_xData == null || _yData == null || _leftMarker == null || _rightMarker == null)
                return double.NaN;

            double left = _leftMarker.X;
            double right = _rightMarker.X;

            if (right <= left)
                return double.NaN;

            // Compute center frequency and bandwidth
            double centerFrequency = (left + right) / 2.0;
            double bandwidth = right - left;

            return centerFrequency / bandwidth;
        }
    }
}
