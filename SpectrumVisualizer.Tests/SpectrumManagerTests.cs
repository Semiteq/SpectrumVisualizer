using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpectrumVisualizer.Uart.SpectrumJobs;

namespace SpectrumVisualizer.Tests
{
    // Fake acquirer that simulates the behavior of receiving a full UART message.
    internal class FakeUartSpectrumAcquirer : IDisposable
    {
        // Virtual event to allow overriding in the wrapper.
        public virtual event Action<ushort[]>? SpectrumReceived;

        // Virtual methods to simulate starting and stopping the acquirer.
        public virtual void Start() { /* Simulate start - do nothing */ }
        public virtual void Stop() { /* Simulate stop */ }
        public void Dispose() { }

        // Simulates receiving a full message.
        public void SimulateMessage(byte[] message)
        {
            // Use the real parser to process the full message.
            var parser = new UartSpectrumParser();
            var combined = parser.ProcessMessage(message);
            SpectrumReceived?.Invoke(combined);
        }
    }

    // Wrapper to allow FakeUartSpectrumAcquirer to be used in SpectrumManager.
    // This class overrides the virtual methods and event.
    internal class FakeAcquirerWrapper : UartSpectrumAcquirer
    {
        private readonly FakeUartSpectrumAcquirer _fake;
        public override event Action<ushort[]>? SpectrumReceived
        {
            add { _fake.SpectrumReceived += value; }
            remove { _fake.SpectrumReceived -= value; }
        }

        public FakeAcquirerWrapper(FakeUartSpectrumAcquirer fake)
            : base("COM_FAKE", 115200, new UartSpectrumParser())
        {
            _fake = fake;
        }

        public override void Start() => _fake.Start();
        public override void Stop() => _fake.Stop();
    }

    [TestClass]
    public class SpectrumManagerTests
    {
        // Generates a test message according to UartMessageStruct.
        // First part: FirstSpectrumLength bytes of data representing (FirstSpectrumLength/2) ushort values,
        // filled with sequential values from 1 to (FirstSpectrumLength/2).
        // Second part: SecondSpectrumLength bytes of data representing (SecondSpectrumLength/2) ushort values,
        // filled with sequential values from (firstCount + 1) to (firstCount + secondCount).
        private byte[] GenerateTestMessage()
        {
            int totalLength = UartMessageStruct.TotalMessageLength;
            byte[] message = new byte[totalLength];
            int offset = 0;

            // First spectrum delimiter (2 bytes) – leave as zeros.
            offset += UartMessageStruct.FirstSpectrumDelimiterLegth;

            // First spectrum data: FirstSpectrumLength bytes = 4096 bytes → 2048 ushort values.
            int firstCount = UartMessageStruct.FirstSpectrumLength / 2;
            for (int i = 0; i < firstCount; i++)
            {
                ushort value = (ushort)(i + 1);
                Buffer.BlockCopy(BitConverter.GetBytes(value), 0, message, offset, 2);
                offset += 2;
            }

            // First spectrum additional fields (average, SNR, quality): 3 fields of 2 bytes each.
            offset += UartMessageStruct.FirstSpectrumAverageLength +
                      UartMessageStruct.FirstSpectrumSnrLength +
                      UartMessageStruct.FirstSpectrumQualityLength;

            // Second spectrum delimiter (2 bytes) – leave as zeros.
            offset += UartMessageStruct.SecondSpectrumDelimiterLegth;

            // Second spectrum data: SecondSpectrumLength bytes = 1024 bytes → 512 ushort values.
            int secondCount = UartMessageStruct.SecondSpectrumLength / 2;
            for (int i = 0; i < secondCount; i++)
            {
                // Values start from firstCount + 1 and continue sequentially.
                ushort value = (ushort)(firstCount + i + 1);
                Buffer.BlockCopy(BitConverter.GetBytes(value), 0, message, offset, 2);
                offset += 2;
            }

            // Second spectrum additional fields (average, SNR, quality): 3 fields of 2 bytes each.
            offset += UartMessageStruct.SecondSpectrumAverageLength +
                      UartMessageStruct.SecondSpectrumSnrLength +
                      UartMessageStruct.SecondSpectrumQualityLength;

            return message;
        }

        [TestMethod]
        public void StartAcquisition_ProcessesSpectrumData_Correctly()
        {
            // Arrange: create a fake acquirer.
            var fakeAcquirer = new FakeUartSpectrumAcquirer();
            var acquirerWrapper = new FakeAcquirerWrapper(fakeAcquirer);
            var manager = new SpectrumManager(acquirerWrapper);

            Dictionary<double, double>? receivedData = null;
            manager.StartAcquisition(dict => receivedData = dict);

            // Simulate receiving a complete message.
            byte[] testMessage = GenerateTestMessage();
            fakeAcquirer.SimulateMessage(testMessage);

            // Allow time for the event to be processed.
            Task.Delay(100).Wait();

            // Assert:
            // Total number of ushort values = (FirstSpectrumLength/2) + (SecondSpectrumLength/2)
            int expectedCount = (UartMessageStruct.FirstSpectrumLength / 2) + (UartMessageStruct.SecondSpectrumLength / 2);
            Assert.IsNotNull(receivedData);
            Assert.AreEqual(expectedCount, receivedData.Count);

            // Verify the first element equals 1 and the last equals the total count.
            Assert.AreEqual(1.0, receivedData[0]);
            Assert.AreEqual((double)expectedCount, receivedData[expectedCount - 1]);
        }

        [TestMethod]
        public void FlipInvertFlag_InvertsSpectrumData()
        {
            // Arrange: create a fake acquirer.
            var fakeAcquirer = new FakeUartSpectrumAcquirer();
            var acquirerWrapper = new FakeAcquirerWrapper(fakeAcquirer);
            var manager = new SpectrumManager(acquirerWrapper);

            Dictionary<double, double>? receivedDataNormal = null;
            Dictionary<double, double>? receivedDataInverted = null;

            // First, receive data without inversion.
            manager.StartAcquisition(dict => receivedDataNormal = dict);
            byte[] testMessage = GenerateTestMessage();
            fakeAcquirer.SimulateMessage(testMessage);
            Task.Delay(100).Wait();
            manager.StopAcquisition();

            // Enable inversion.
            manager.FlipInvertFlag();
            manager.StartAcquisition(dict => receivedDataInverted = dict);
            fakeAcquirer.SimulateMessage(testMessage);
            Task.Delay(100).Wait();
            manager.StopAcquisition();

            // Assert: For the non-inverted spectrum, the first element should be the minimum value in data.
            // For the inverted spectrum, the first element should be (max - min).
            Assert.IsNotNull(receivedDataNormal);
            Assert.IsNotNull(receivedDataInverted);

            double normalFirst = receivedDataNormal[0];
            double invertedFirst = receivedDataInverted[0];

            double actualMax = receivedDataNormal.Values.Max(); // Find actual max intensity value
            double expectedNormalFirst = receivedDataNormal.Values.Min(); // First value before inversion
            double expectedInvertedFirst = actualMax - expectedNormalFirst; // First value after inversion

            Assert.AreEqual(expectedNormalFirst, normalFirst);
            Assert.AreEqual(expectedInvertedFirst, invertedFirst);
        }
    }
}
