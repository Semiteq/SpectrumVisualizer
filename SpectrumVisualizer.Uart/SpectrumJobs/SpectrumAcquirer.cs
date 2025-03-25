using System.IO.Ports;
using SpectrumVisualizer.Uart.Message;

namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    /// <summary>
    /// Acquires UART data from the COM port.
    /// Accumulates received bytes until a complete message (of fixed length) is available,
    /// then uses ISpectrumParser to extract and combine the spectrum data.
    /// </summary>
    public class SpectrumAcquirer : IDisposable
    {
        private readonly SerialPort _serialPort;
        private readonly ISpectrumParser _parser;
        private readonly List<byte> _buffer = new();

        // Event raised when a complete spectrum is available.
        public virtual event Action<DataStruct>? SpectrumReceived;

        public SpectrumAcquirer(string portName, int baudRate, ISpectrumParser parser)
        {
            _parser = parser;
            _serialPort = new SerialPort(portName, baudRate)
            {
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            _serialPort.DataReceived += SerialPort_DataReceived;
        }

        /// <summary>
        /// Opens the serial port and starts acquisition.
        /// Ensures the port is not already open.
        /// </summary>
        public virtual void Start()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                ErrorHandler.Log(new Exception("Access to the COM port is denied.", ex));
            }
            catch (IOException ex)
            {
                ErrorHandler.Log(new Exception("Error accessing the COM port.", ex));
            }
            catch (InvalidOperationException ex)
            {
                ErrorHandler.Log(new Exception("Attempted to open an already open COM port.", ex));
            }
            catch (Exception ex)
            {
                ErrorHandler.Log(ex);
            }
        }


        /// <summary>
        /// Stops acquisition by closing the serial port.
        /// Ensures the port is open before closing.
        /// </summary>
        public virtual void Stop()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var bytesToRead = _serialPort.BytesToRead;
                var receivedBytes = new byte[bytesToRead];
                _serialPort.Read(receivedBytes, 0, bytesToRead);
                _buffer.AddRange(receivedBytes);

                ProcessBuffer();
            }
            catch (Exception ex)
            {
                ErrorHandler.Log(ex);
            }
        }

        /// <summary>
        /// Processes the accumulated buffer to extract complete messages.
        /// </summary>
        private void ProcessBuffer()
        {
            byte[] messageBytes;
            DataStruct dataStruct;

            switch (_buffer.Count)
            {
                case MessageStruct1.TotalMessageLength:
                    messageBytes = _buffer.GetRange(0, MessageStruct1.TotalMessageLength).ToArray();
                    _buffer.RemoveRange(0, MessageStruct1.TotalMessageLength);

                    dataStruct = _parser.ProcessMessage(messageBytes);
                    SpectrumReceived?.Invoke(dataStruct);
                    break;
                case MessageStruct2.TotalMessageLength:
                    messageBytes = _buffer.GetRange(0, MessageStruct1.TotalMessageLength).ToArray();
                    _buffer.RemoveRange(0, MessageStruct1.TotalMessageLength);

                    dataStruct = _parser.ProcessMessage(messageBytes);
                    SpectrumReceived?.Invoke(dataStruct);
                    break;
            }
        }

        public void Dispose()
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();

            _serialPort.DataReceived -= SerialPort_DataReceived;
            _serialPort.Dispose();
        }
    }
}