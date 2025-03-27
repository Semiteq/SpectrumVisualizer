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
        private readonly object _bufferLock = new();

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
                EventHandler.Log(new Exception("Access to the COM port is denied.", ex));
            }
            catch (IOException ex)
            {
                EventHandler.Log(new Exception("Error accessing the COM port.", ex));
            }
            catch (InvalidOperationException ex)
            {
                EventHandler.Log(new Exception("Attempted to open an already open COM port.", ex));
            }
            catch (Exception ex)
            {
                EventHandler.Log(ex);
            }
        }

        /// <summary>
        /// Stops acquisition by closing the serial port.
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

                // Synchronize access to the buffer.
                lock (_bufferLock)
                {
                    _buffer.AddRange(receivedBytes);
                }
                ProcessBuffer();
            }
            catch (Exception ex)
            {
                EventHandler.Log(ex);
            }
        }

        /// <summary>
        /// Processes the accumulated buffer to extract complete messages.
        /// It scans for valid headers and removes extraneous bytes before the header.
        /// </summary>
        protected virtual void ProcessBuffer()
        {
            while (true)
            {
                byte[] messageBytes = null;
                int expectedLength = 0;
                bool headerFound = false;
                int headerIndex = -1;

                lock (_bufferLock)
                {
                    // Need at least 4 bytes to identify a header.
                    if (_buffer.Count < 4)
                        break;

                    // Scan for a valid header in the buffer.
                    for (var i = 0; i <= _buffer.Count - 4; i++)
                    {
                        var potentialHeader = _buffer.GetRange(i, 4).ToArray();
                        if (AreArraysEqual(potentialHeader, MessageStruct1.SpectrumDelimiter))
                        {
                            expectedLength = MessageStruct1.TotalMessageLength;
                            headerFound = true;
                            headerIndex = i;
                            break;
                        }
                        else if (AreArraysEqual(potentialHeader, MessageStruct2.SpectrumDelimiter))
                        {
                            expectedLength = MessageStruct2.TotalMessageLength;
                            headerFound = true;
                            headerIndex = i;
                            break;
                        }
                    }

                    if (!headerFound)
                    {
                        // No valid header found.
                        // Remove all bytes except the last 3 (which may be the start of a header).
                        var removeCount = _buffer.Count - 3;
                        _buffer.RemoveRange(0, removeCount);
                        break;
                    }

                    // Discard any extraneous bytes before the header.
                    if (headerIndex > 0)
                    {
                        _buffer.RemoveRange(0, headerIndex);
                    }

                    // Check if we have a complete message.
                    if (_buffer.Count < expectedLength)
                        break;

                    // Extract complete message.
                    messageBytes = _buffer.GetRange(0, expectedLength).ToArray();
                    _buffer.RemoveRange(0, expectedLength);
                }

                // Log message details (optional).
                //ErrorHandler.Log($"Message length: {messageBytes.Length}. Start 4 bytes: {BitConverter.ToString(messageBytes.Take(4).ToArray())}, End 4 bytes: {BitConverter.ToString(messageBytes.Skip(messageBytes.Length - 4).ToArray())}");
                ProcessMessage(messageBytes);
            }
        }

        /// <summary>
        /// Processes a complete message by parsing it and invoking the SpectrumReceived event.
        /// </summary>
        /// <param name="messageBytes">Complete message bytes</param>
        private void ProcessMessage(byte[] messageBytes)
        {
            try
            {
                var dataStruct = _parser.ProcessMessage(messageBytes);
                SpectrumReceived?.Invoke(dataStruct);
            }
            catch (Exception ex)
            {
                EventHandler.Log(ex);
            }
        }

        /// <summary>
        /// Compares two byte arrays for equality.
        /// </summary>
        /// <param name="a1">First array</param>
        /// <param name="a2">Second array</param>
        /// <returns>True if arrays are equal, false otherwise</returns>
        private bool AreArraysEqual(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;
            for (var i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                    return false;
            }
            return true;
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
