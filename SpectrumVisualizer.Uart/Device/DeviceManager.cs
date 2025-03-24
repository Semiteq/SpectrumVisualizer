using System;
using System.Threading.Tasks;
using SpectrumVisualizer.Uart.SpectrumJobs;

namespace SpectrumVisualizer.Uart.Device
{
    /// <summary>
    /// Manages the device connection via UART.
    /// Instantiates UartSpectrumAcquirer with fixed baud rate and parser.
    /// </summary>
    internal class DeviceManager
    {
        private UartSpectrumAcquirer? _acquirer;
        public bool IsConnected { get; private set; } = false;
        // Expose the acquirer for use in SpectrumManager.
        public UartSpectrumAcquirer? Acquirer => _acquirer;

        /// <summary>
        /// Connects to the device via the specified COM port.
        /// </summary>
        public async Task<bool> ConnectAsync(string portName)
        {
            try
            {
                // Fixed baud rate 115200, using UartSpectrumParser
                _acquirer = new UartSpectrumAcquirer(portName, 115200, new UartSpectrumParser());
                _acquirer.Start();
                IsConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.Log(ex);
                return false;
            }
        }

        /// <summary>
        /// Disconnects from the device.
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (_acquirer != null)
            {
                _acquirer.Stop();
                _acquirer.Dispose();
                IsConnected = false;
            }
        }
    }
}