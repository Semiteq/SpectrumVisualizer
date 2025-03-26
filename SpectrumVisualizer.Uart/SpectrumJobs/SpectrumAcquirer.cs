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

                // Synchronize access to the buffer.
                lock (_bufferLock)
                {
                    _buffer.AddRange(receivedBytes);
                }
                ProcessBuffer();
            }
            catch (Exception ex)
            {
                ErrorHandler.Log(ex);
            }
        }

        /// <summary>
        /// Processes the accumulated buffer to extract complete messages.
        /// Uses the first 4 bytes as a header (delimiter) to determine the expected message length.
        /// The header is included in the total message length.
        /// If the header is not expected, the message is discarded.
        /// See the MessageStruct classes for expected headers and message lengths.
        /// </summary>
        protected virtual void ProcessBuffer()
        {
            while (true)
            {
                byte[] messageBytes = null;
                int expectedLength = 0;

                // Lock the buffer for thread-safe access.
                lock (_bufferLock)
                {
                    // Ensure we have at least 4 bytes to read the header.
                    if (_buffer.Count < 4)
                        break;

                    // Extract the first 4 bytes as header.
                    var header = _buffer.GetRange(0, 4).ToArray();

                    // Determine message type and expected total length
                    if (AreArraysEqual(header, MessageStruct1.SpectrumDelimiter))
                    {
                        expectedLength = MessageStruct1.TotalMessageLength;
                    }
                    else if (AreArraysEqual(header, MessageStruct2.SpectrumDelimiter))
                    {
                        expectedLength = MessageStruct2.TotalMessageLength;
                    }
                    else
                    {
                        // Unexpected header, dropping message.
                        ErrorHandler.Log($"Unknown message header: {BitConverter.ToString(header)}");
                        _buffer.Clear();
                        break;
                    }

                    // If the buffer does not contain a complete message, wait for more data.
                    if (_buffer.Count < expectedLength)
                        break;

                    // Extract the complete message.
                    messageBytes = [.. _buffer.GetRange(0, expectedLength)];
                    _buffer.RemoveRange(0, expectedLength);
                }

                // Process the complete message outside the lock.
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
                ErrorHandler.Log(ex);
            }
        }

        /// <summary>
        /// Helper method to compare two byte arrays for equality.
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
