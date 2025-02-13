using Device.ATR.Devices;
using Device.ATR.Model.Spectrometer;

namespace SpectrumVisualizer.SpectrumJobs
{
    public class SpectrumAcquirer
    {
        private readonly DeviceService _deviceService;
        private int _dataSize;

        public SpectrumAcquirer(DeviceService deviceService) 
        {
            _deviceService = deviceService;
        }

        public async Task<double[]> AcquireAsync(AcquireParameter parameter, bool isDark = false)
        {
            try
            {
                // Writing first in case of communication error
                _dataSize = _deviceService.DeviceInfo.CcdSize;

                return isDark
                    ? (await _deviceService.AcquireDark(parameter)).Data
                    : (await _deviceService.Acquire(parameter)).Data;
            }
            catch (Exception ex)
            {
                ErrorHandler.Log(ex);
                return new double[_dataSize];
            }
        }

    }
}
