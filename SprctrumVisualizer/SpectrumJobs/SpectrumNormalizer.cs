using Device.ATR.Devices;
using Device.ATR.Model.Spectrometer;

namespace SpectrumVisualizer.SpectrumJobs
{
    internal class SpectrumNormalizer(DeviceService deviceService) : WavelengthCalibrationCoeff
    {
        private readonly DeviceService _deviceService = deviceService;

        public async Task<Dictionary<double, double>> ProcessAsync(Spectrum spectrum)
        {
            var coeffResult = await _deviceService.GetWavelengthCalibrationCoeff();
            Coeff = coeffResult.Coeff;
            int dataSize = _deviceService.DeviceInfo.CcdSize;

            // Calibration of the spectrum by the polinoimial coefficients
            double[] wavelengths = CalcWavelength(dataSize);

            // Normalized spectrum absciss data
            double[] intensivities = spectrum.Data;

            var result = new Dictionary<double, double>(dataSize);

            for (int i = 0; i < dataSize; i++)
            {
                result[wavelengths[i]] = intensivities[i];
            }

            return result;
        }
    }
}