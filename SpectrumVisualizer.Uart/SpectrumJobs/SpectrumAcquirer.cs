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

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var bytesToRead = _serialPort.BytesToRead;
                var receivedBytes = new byte[bytesToRead];
                _serialPort.Read(receivedBytes, 0, bytesToRead);

                lock (_bufferLock)
                {
                    // Append data to MemoryStream
                    _buffer.Write(receivedBytes, 0, bytesToRead);
                }
                ProcessBuffer();
            }
            catch (Exception ex)
            {
                EventHandler.Log(ex);
            }
        }

        protected virtual void ProcessBuffer()
        {
            // Define header length constant
            const int headerLength = 10;
            while (true)
            {
                byte[] messageBytes;
                int expectedLength = 0;
                int headerPosition = -1;

                lock (_bufferLock)
                {
                    var bufferArray = _buffer.ToArray();
                    // Requires at least headerLength bytes to search for the header
                    if (bufferArray.Length < headerLength)
                        break;

                    // Optimize the search for the header using Span<byte>
                    var bufferSpan = bufferArray.AsSpan();
                    
                    headerPosition = FindHeader(bufferSpan, MessageStruct1.SpectrumDelimiter);
                    
                    if (headerPosition >= 0)
                    {
                        expectedLength = MessageStruct1.TotalMessageLength;
                    }
                    else
                    {
                        headerPosition = FindHeader(bufferSpan, MessageStruct2.SpectrumDelimiter);
                        
                        if (headerPosition >= 0)
                        {
                            expectedLength = MessageStruct2.TotalMessageLength;
                        }
                    }

                    if (headerPosition == -1)
                    {
                        // Save the last part of the buffer as it may contain the header
                        var keepLength = Math.Min(headerLength, bufferArray.Length);
                        var newBuffer = new byte[keepLength];
                        Array.Copy(bufferArray, bufferArray.Length - keepLength, newBuffer, 0, keepLength);
                        _buffer.SetLength(0);
                        _buffer.Write(newBuffer, 0, keepLength);
                        break;
                    }

                    // Drop the bytes before the header
                    if (headerPosition > 0)
                    {
                        var remaining = bufferArray.Length - headerPosition;
                        var newBuffer = new byte[remaining];
                        Array.Copy(bufferArray, headerPosition, newBuffer, 0, remaining);
                        _buffer.SetLength(0);
                        _buffer.Write(newBuffer, 0, remaining);
                        bufferArray = newBuffer;
                        bufferSpan = bufferArray.AsSpan();
                    }

                    // Check if we have enough bytes for the expected message length
                    if (bufferArray.Length < expectedLength)
                        break;

                    messageBytes = new byte[expectedLength];
                    Array.Copy(bufferArray, 0, messageBytes, 0, expectedLength);
                    // Remove the processed message from the buffer
                    var leftoverLength = bufferArray.Length - expectedLength;
                    var leftover = new byte[leftoverLength];
                    Array.Copy(bufferArray, expectedLength, leftover, 0, leftoverLength);
                    _buffer.SetLength(0);
                    _buffer.Write(leftover, 0, leftoverLength);
                }

                EventHandler.Log($" [INFO] Header found. Message length: {messageBytes.Length}.");
                ProcessMessage(messageBytes);
            }
        }

        // Efficient header search using Span<byte>
        private int FindHeader(ReadOnlySpan<byte> buffer, byte[] header)
        {
            for (var i = 0; i <= buffer.Length - header.Length; i++)
            {
                if (buffer.Slice(i, header.Length).SequenceEqual(header))
                {
                    return i;
                }
            }
            return -1;
        }

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
