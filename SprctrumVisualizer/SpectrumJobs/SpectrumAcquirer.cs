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

        public async Task<double[]> AcquireAsync(AcquireParameter parameter)
        {
            try
            {
                return (await _deviceService.Acquire(parameter)).Data;
            }
            catch (Exception ex)
            {
                _dataSize = _deviceService.DeviceInfo.CcdSize;
                ErrorHandler.Log(ex);
                return new double[_dataSize];
            }
        }

        public async Task<double[]> AcquireDarkAsync(AcquireParameter parameter)
        {
            try
            {
                return (await _deviceService.AcquireDark(parameter)).Data;
            }
            catch (Exception ex)
            {
                _dataSize = _deviceService.DeviceInfo.CcdSize;
                ErrorHandler.Log(ex);
                return new double[_dataSize];
            }
        }
    }
}
