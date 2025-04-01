using System.IO.Ports;
using SpectrumVisualizer.Uart.Message;

namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    public class SpectrumAcquirer : IDisposable
    {
        private readonly SerialPort _serialPort;
        private readonly ISpectrumParser _parser;
        private readonly MemoryStream _buffer = new();
        private readonly Lock _bufferLock = new();

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

        public virtual void Start()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }
            }
            catch (Exception ex)
            {
                EventHandler.Log(ex);
            }
        }

        public virtual void Stop()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        protected void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var bytesToRead = _serialPort.BytesToRead;
                var receivedBytes = new byte[bytesToRead];
                _serialPort.Read(receivedBytes, 0, bytesToRead);

                // Append data from serial port to buffer
                lock (_bufferLock)
                {
                    _buffer.Write(receivedBytes, 0, bytesToRead);
                }

                ProcessBuffer();
            }
            catch (Exception ex)
            {
                EventHandler.Log(ex);
            }
        }

        /// <summary>
        /// Main loop for extracting and processing complete messages from the internal buffer.
        /// </summary>
        protected virtual void ProcessBuffer()
        {
            while (true)
            {
                byte[] messageBytes;
                byte[] expectedFooter;

                // Try to extract a complete message from the buffer inside lock for thread-safety.
                lock (_bufferLock)
                {
                    if (!TryExtractMessage(out messageBytes, out expectedFooter))
                        break;
                }

                // Validate message footer outside lock.
                if (!ValidateFooter(messageBytes, expectedFooter))
                {
                    EventHandler.Log(" [ERROR] Invalid footer detected. Dropping message.");
                    continue;
                }

                EventHandler.Log($" [INFO] Valid message received. Length: {messageBytes.Length}.");
                ProcessMessage(messageBytes);
            }
        }

        /// <summary>
        /// Attempts to extract the next complete message from the internal buffer.
        /// If no valid header is found, trims the buffer to preserve potential header fragments.
        /// </summary>
        private bool TryExtractMessage(out byte[] messageBytes, out byte[] expectedFooter)
        {
            const int headerSize = 10;
            messageBytes = null;
            expectedFooter = null;

            var data = _buffer.ToArray();
            if (data.Length < headerSize)
                return false;

            // Check for MessageStruct1 header
            int headerPos = FindHeader(data.AsSpan(), MessageStruct1.SpectrumHeader);
            if (headerPos >= 0)
            {
                expectedFooter = MessageStruct1.SpectrumFooter;
                return ExtractMessage(data, headerPos, MessageStruct1.TotalMessageLength, out messageBytes);
            }

            // Check for MessageStruct2 header
            headerPos = FindHeader(data.AsSpan(), MessageStruct2.SpectrumHeader);
            if (headerPos >= 0)
            {
                expectedFooter = MessageStruct2.SpectrumFooter;
                return ExtractMessage(data, headerPos, MessageStruct2.TotalMessageLength, out messageBytes);
            }

            // No valid header found; keep only the last few bytes that may contain a partial header.
            int keep = Math.Min(headerSize, data.Length);
            UpdateBuffer(data.AsSpan(data.Length - keep, keep).ToArray());
            return false;
        }

        /// <summary>
        /// Extracts a complete message from the provided data starting at the header position.
        /// Updates the internal buffer with any leftover data.
        /// </summary>
        private bool ExtractMessage(byte[] data, int headerPos, int expectedLength, out byte[] messageBytes)
        {
            messageBytes = null;

            // Remove any bytes before the header.
            if (headerPos > 0)
            {
                data = data.AsSpan(headerPos).ToArray();
                UpdateBuffer(data);
            }

            // Wait for more data if complete message is not yet received.
            if (data.Length < expectedLength)
                return false;

            // Extract complete message.
            messageBytes = new byte[expectedLength];
            Array.Copy(data, 0, messageBytes, 0, expectedLength);

            // Update the buffer with leftover bytes.
            int leftoverLength = data.Length - expectedLength;
            var leftover = new byte[leftoverLength];
            Array.Copy(data, expectedLength, leftover, 0, leftoverLength);
            UpdateBuffer(leftover);

            return true;
        }

        /// <summary>
        /// Replaces the internal buffer with the specified data.
        /// </summary>
        private void UpdateBuffer(byte[] data)
        {
            _buffer.SetLength(0);
            _buffer.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Searches for the specified header in the given buffer using Span for efficiency.
        /// </summary>
        private int FindHeader(ReadOnlySpan<byte> buffer, byte[] header)
        {
            for (var i = 0; i <= buffer.Length - header.Length; i++)
            {
                if (buffer.Slice(i, header.Length).SequenceEqual(header))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Validates that the message ends with the expected footer bytes.
        /// </summary>
        private bool ValidateFooter(byte[] messageBytes, byte[] expectedFooter)
        {
            if (messageBytes.Length < expectedFooter.Length)
                return false;

            int footerStart = messageBytes.Length - expectedFooter.Length;
            return messageBytes.AsSpan(footerStart, expectedFooter.Length).SequenceEqual(expectedFooter);
        }

        /// <summary>
        /// Processes a valid message by parsing it and invoking the SpectrumReceived event.
        /// </summary>
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

        public void Dispose()
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();

            _serialPort.DataReceived -= SerialPort_DataReceived;
            _serialPort.Dispose();
        }
    }
}
