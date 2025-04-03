using SpectrumVisualizer.Uart.SpectrumJobs;

namespace SpectrumVisualizer.Uart.Device
{
    /// <summary>
    /// Manages the device connection using SpectrumAcquirer (на основе SerialPortStream).
    /// </summary>
    internal class DeviceManager
    {
        private SpectrumAcquirer? _acquirer;
        public bool IsConnected { get; private set; } = false;
        public SpectrumAcquirer? Acquirer => _acquirer;

        /// <summary>
        /// Connects to the device using the specified COM port.
        /// </summary>
        public async Task<bool> ConnectAsync(string portName)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _acquirer = new SpectrumAcquirer(portName, 115200, new SpectrumParser());
                    _acquirer.Start();
                    IsConnected = true;
                    EventHandler.Log($"Connected successfully on {portName}");
                    return true;
                }
                catch (Exception ex)
                {
                    EventHandler.Log($"Error connecting on {portName}: {ex.Message}");
                    IsConnected = false;
                    return false;
                }
            });
        }

        /// <summary>
        /// Disconnects from the device.
        /// </summary>
        public async Task DisconnectAsync()
        {
            await Task.Run(() =>
            {
                if (_acquirer != null)
                {
                    try
                    {
                        _acquirer.Stop();
                    }
                    catch (Exception ex)
                    {
                        EventHandler.Log($"Error during stop: {ex.Message}");
                    }
                    finally
                    {
                        try
                        {
                            _acquirer.Dispose();
                        }
                        catch (Exception ex)
                        {
                            EventHandler.Log($"Error during dispose: {ex.Message}");
                        }
                        _acquirer = null;
                    }
                }
                IsConnected = false;
                EventHandler.Log("Device was disconnected.");
            });
        }
    }
}
