using Device.ATR.Model.Spectrometer;
using SpectrumVisualizer.Device;

namespace SpectrumVisualizer.SpectrumJobs
{
    /// <summary>
    /// Uses polinom to adjust pixel data to wavelength range.
    /// </summary>
    internal class SpectrumNormalizer: WavelengthCalibrationCoeff
    {
        public Dictionary<double, double> Process(Spectrum spectrum)
        {
            Coeff = DeviceGeneralInfo.Coeff;

            // Calibration of the spectrum by the polinoimial coefficients
            double[] wavelengths = CalcWavelength(DeviceGeneralInfo.DataSize);

            // Normalized spectrum absciss data
            double[] intensivities = spectrum.Data;

            var result = new Dictionary<double, double>(DeviceGeneralInfo.DataSize);

            for (int i = 0; i < DeviceGeneralInfo.DataSize; i++)
            {
                result[wavelengths[i]] = intensivities[i];
            }

            return result;
        }
    }
}