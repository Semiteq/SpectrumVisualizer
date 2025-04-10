﻿using System.IO.Ports;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ComPortPopulator
{
    // Class that handles writing to the COM port periodically with random data simalating a device
    public class ComPortWriter : IDisposable
    {
        private readonly SerialPort _serialPort;
        private readonly Timer _timer;
        //private readonly byte[] _message = GenerateRandomByteArray(1034);

        public ComPortWriter(string portName, int baudRate)
        {
            // Initialize COM port with given parameters
            _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            _serialPort.Open();

            // Initialize timer with 1-second interval (1000 milliseconds)
            _timer = new Timer(1000);
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = true;
        }

        // Start sending messages periodically
        public void Start() => _timer.Start();

        // Stop sending messages
        public void Stop() => _timer.Stop();

        // Timer callback that writes a byte array to the COM port
        private void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            // var _message = GenerateSmoothGradient(1050);
            var _message = ReadFromFile("C:/Users/admin/Desktop/plot_yat_data/yat_data.txt");
            // Write the byte array to the serial port
            _serialPort.Write(_message, 0, _message.Length);
        }

        // Clean up resources
        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
            if (_serialPort?.IsOpen == true)
            {
                _serialPort.Close();
            }
            _serialPort?.Dispose();
        }

        private static byte[] GenerateSmoothGradient(int length)
        {
            if (length < 4)
                throw new ArgumentException("Array length must be at least 4 bytes.", nameof(length));

            var data = new byte[length];

            for (var i = 0; i < length; i++)
            {
                // Линейный градиент от 0 до 255
                data[i] = (byte)(255.0 * i / (length - 1));
            }

            // Header
            data[0] = 0x00;
            data[1] = 0x02;
            data[2] = 0xFF;
            data[3] = 0xFF;
            data[4] = 0xFF;
            data[5] = 0xFF;
            data[6] = 0xFF;
            data[7] = 0xFF;
            
            // Footer
            data[1043] = 0xFF;
            data[1044] = 0xFF;
            data[1045] = 0xFF;
            data[1046] = 0xFF;
            data[1047] = 0xFF;
            data[1048] = 0xFF;
            data[1049] = 0x02;
            data[1050] = 0x00;
            
            return data;
        }

        private static byte[] ReadFromFile(string path)
        {
            var hexData = File.ReadAllText(path)
                              .Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            byte[] data = new byte[hexData.Length];

            for (int i = 0; i < hexData.Length; i++)
            {
                string cleanHex = hexData[i].TrimEnd('h'); // Удаляем 'h' в конце
                data[i] = Convert.ToByte(cleanHex, 16);
            }

            return data;
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            // Parse command-line arguments:
            // Expected format: populate_com -s 115200 -p 10
            // -s: baud rate, -p: port number
            var baudRate = 115200; // default baud rate
            var portNumber = 7;  // default port number

            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-s" when i + 1 < args.Length && int.TryParse(args[i + 1], out var br):
                        baudRate = br;
                        i++; // Skip next argument since it's consumed
                        break;
                    case "-p" when i + 1 < args.Length && int.TryParse(args[i + 1], out var pn):
                        portNumber = pn;
                        i++;
                        break;
                }
            }

            // Format port name. For COM ports above 9, use "\\.\COMx"
            var portName = $"COM{portNumber}";
            if (portNumber >= 10)
            {
                portName = $"\\\\.\\{portName}";
            }

            Console.WriteLine($"Opening {portName} with baud rate {baudRate}.");
            using var writer = new ComPortWriter(portName, baudRate);

            // Start sending messages periodically
            writer.Start();
            Console.WriteLine("Sending messages every second. Press Ctrl+C to exit.");

            // Keep application running until user terminates the process
            var exitEvent = new System.Threading.ManualResetEvent(false);
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                exitEvent.Set();
            };
            exitEvent.WaitOne();

            writer.Stop();
            Console.WriteLine("Stopped sending messages. Exiting application.");
        }
    }
}
