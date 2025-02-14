using Device.ATR.Devices;

namespace SpectrumVisualizer
{
    /// <summary>
    /// Manages the device connection and disconnection, ensuring thread-safe access to device operations.
    /// </summary>
    internal class DeviceManager
    {
        private readonly DeviceService _deviceService;
        private readonly SemaphoreSlim _semaphore = new(1, 1); // Semaphore to control access to device service, allowing only one operation at a time.
        private bool _isConnected; // Flag indicating whether the device is currently connected.

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceManager"/> class.
        /// </summary>
        /// <param name="deviceService">The device service to be managed.</param>
        public DeviceManager(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        /// <summary>
        /// Asynchronously connects to a device on the specified port.
        /// </summary>
        /// <param name="portName">The name of the serial port to connect to.</param>
        /// <returns>True if the connection was successful or already established; otherwise, false.</returns>
        public async Task<bool> ConnectAsync(string portName)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_isConnected)
                    return true;

                _isConnected = await _deviceService.Open(portName);
                return _isConnected;
            }
            catch (Exception ex)
            {
                ErrorHandler.Log(ex);
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Asynchronously disconnects from the currently connected device.
        /// </summary>
        public async Task DisconnectAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_isConnected)
                    return;

                _deviceService.Close();
                _isConnected = false;
            }
            catch (Exception ex)
            {
                ErrorHandler.Log(ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
