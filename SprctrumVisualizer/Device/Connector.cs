using Device.ATR.Devices;

namespace SpectrumVisualizer.Device
{
    public class Connector(DeviceService deviceService)
    {
        private readonly DeviceService _deviceService = deviceService;

        public async Task<bool> ConnectAsync(string portName)
        {
            try
            {
                return await _deviceService.Open(portName);
            }
            catch (Exception ex)
            {
                ErrorHandler.Log(ex);
                return false;
            }
        }

        public bool Connect(string portName)
        {
            try
            {
                return _deviceService.Open(portName).GetAwaiter().GetResult();   
            }
            catch (Exception ex)
            {
                ErrorHandler.Log(ex);
                return false;
            }
        }

        public void Disconnect() => _deviceService.Close();
    }
}
