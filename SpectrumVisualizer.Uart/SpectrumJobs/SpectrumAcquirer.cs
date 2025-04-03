using RJCP.IO.Ports;
using SpectrumVisualizer.Uart.Message;

namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    public class SpectrumAcquirer : IDisposable
    {
        private readonly SerialPortStream _serialPort;
        private readonly ISpectrumParser _parser;
        private readonly MemoryStream _buffer = new();
        private readonly object _bufferLock = new();
        private bool _isDisposed;

        public virtual event Action<DataStruct>? SpectrumReceived;

        public bool IsOpen => _serialPort?.IsOpen ?? false;
        public SerialPortStream SerialPort => _serialPort;

        public SpectrumAcquirer(string portName, int baudRate, ISpectrumParser parser)
        {
            _parser = parser;
            _serialPort = new SerialPortStream(portName, baudRate)
            {
                ReadTimeout = 500,
                WriteTimeout = 500,
                RtsEnable = true,
                DtrEnable = true,
                DiscardNull = false,
                ReadBufferSize = 4096,
                WriteBufferSize = 4096
            };
            _serialPort.ErrorReceived += (_, e) =>
            {
                EventHandler.Log($"Serial port error: {e.EventType}");
                if (e.EventType == SerialError.Frame || 
                    e.EventType == SerialError.RXOver)
                {
                    HandleSerialError();
                }
            };
        
            _serialPort.DataReceived += SerialPort_DataReceived;
        }

        private void HandleSerialError()
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                    // Wait a bit before reopening
                    Task.Delay(100).Wait();
                    _serialPort.Open();
                }
            }
            catch (Exception ex)
            {
                EventHandler.Log($"Error handling serial error: {ex.Message}");
            }
        }
        
        public virtual void Start()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(SpectrumAcquirer));

            try
            {
                if (!_serialPort.IsOpen)
                {
                    // Check if the port exists
                    if (!SerialPortStream.GetPortNames().Contains(_serialPort.PortName))
                    {
                        throw new IOException($"Port {_serialPort.PortName} does not exist");
                    }

                    // Try to open the port with retries
                    var retryCount = 3;
                    while (retryCount > 0)
                    {
                        try
                        {
                            _serialPort.Open();
                            break;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            retryCount--;
                            if (retryCount == 0) throw;
                            Task.Delay(100).Wait();
                        }
                    }

                    if (!_serialPort.IsOpen)
                    {
                        throw new IOException($"Failed to open port {_serialPort.PortName}");
                    }

                    // Reset the buffers
                    _serialPort.DiscardInBuffer();
                    _serialPort.DiscardOutBuffer();
                }
            }
            catch (Exception ex)
            {
                EventHandler.Log($"Error opening port: {ex.Message}");
                throw;
            }
        }

        public virtual void Stop()
        {
            if (_isDisposed)
                return;

            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.DiscardInBuffer();
                    _serialPort.DiscardOutBuffer();
                    _serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                EventHandler.Log($"Error closing port: {ex.Message}");
                throw;
            }
        }

        private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_isDisposed) return;

            try
            {
                var buffer = new byte[_serialPort.BytesToRead];
                int bytesRead = await _serialPort.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                if (bytesRead > 0)
                {
                    lock (_bufferLock)
                    {
                        _buffer.Write(buffer.AsSpan(0, bytesRead));
                    }
                    ProcessBuffer();
                }
            }
            catch (Exception ex)
            {
                EventHandler.Log($"Error reading data: {ex.Message}");
                if (!_serialPort.IsOpen)
                {
                    EventHandler.Log("Serial port was closed unexpectedly");
                }
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

                if (!ValidateFooter(messageBytes, expectedFooter))
                {
                    EventHandler.Log("Invalid message footer detected");
                    continue;
                }

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
            messageBytes = data.AsSpan(0, expectedLength).ToArray();

            // Update the buffer with leftover bytes.
            var leftover = data.AsSpan(expectedLength).ToArray();
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
        private static int FindHeader(ReadOnlySpan<byte> buffer, byte[] header)
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
        private static bool ValidateFooter(byte[] messageBytes, byte[] expectedFooter)
        {
            if (messageBytes.Length < expectedFooter.Length)
                return false;

            return messageBytes.AsSpan(messageBytes.Length - expectedFooter.Length)
                             .SequenceEqual(expectedFooter);
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
                EventHandler.Log($"Error processing message: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            
            _isDisposed = true;
            if (_serialPort.IsOpen)
            {
                Stop();
            }
            _serialPort.Dispose();
            _buffer.Dispose();
        }
    }
}