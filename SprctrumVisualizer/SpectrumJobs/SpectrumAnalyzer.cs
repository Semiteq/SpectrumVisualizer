using Device.ATR.Devices;
using Device.ATR.Model.Spectrometer;

namespace SpectrumVisualizer.SpectrumJobs
{
    internal class SpectrumAnalyzer
    {
        private readonly DeviceService _deviceService;
        private SpectrumNormalizer _spectrumNormilizer;
        private int _dataSize;
        public SpectrumAnalyzer(DeviceService deviceService)
        {
            _deviceService = deviceService;
            _spectrumNormilizer = new(_deviceService);
        }

        public async Task<Dictionary<double, double>> ProcessAsync(Spectrum spectrum, bool considerDark)
        {
            if (spectrum?.Data == null || spectrum.Dark == null)
            {
                ErrorHandler.Log(new ArgumentNullException(nameof(spectrum)));
                return new Dictionary<double, double>();
            }

            _dataSize = _deviceService.DeviceInfo.CcdSize;

            if (considerDark)
            {
                for (int i = 0; i < _dataSize; i++)
                {
                    spectrum.Data[i] -= spectrum.Dark[i];
                }
            }

            return await _spectrumNormilizer.ProcessAsync(spectrum);
        }

    }
}