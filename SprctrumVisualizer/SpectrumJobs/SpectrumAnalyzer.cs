using Device.ATR.Devices;
using Device.ATR.Model.Spectrometer;

namespace SpectrumVisualizer.SpectrumJobs
{
    internal class SpectrumAnalyzer
    {
        private readonly DeviceService _deviceService;
        private SpectrumNormilizer _spectrumNormilizer;
        private int _dataSize;
        public SpectrumAnalyzer(DeviceService deviceService)
        {
            _deviceService = deviceService;
            _spectrumNormilizer = new(_deviceService);
        }

        public async Task<Dictionary<double, double>> ProcessAsync(Spectrum spectrum, bool considerDark)
        {
            _dataSize = _deviceService.DeviceInfo.CcdSize;

            if (considerDark)
            {
                for (int i = 0; i < _dataSize; i++)
                {
                    spectrum.Data[i] -= spectrum.Dark[i];
                }
            }
            // NSR etc goes here
            return await _spectrumNormilizer.ProcessAsync(spectrum);
        }
    }
}