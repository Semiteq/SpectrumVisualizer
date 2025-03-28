using SpectrumVisualizer.Uart.Message;
using SpectrumVisualizer.Uart.SpectrumJobs;

namespace SpectrumVisualizer.Tests.Uart
{
    // Fake acquirer that simulates receiving a full UART message.
    internal class FakeUartSpectrumAcquirer : IDisposable
    {
        // Virtual event using the new UartMessageData type.
        public virtual event Action<DataStruct>? SpectrumReceived;

        // Virtual methods to simulate start and stop.
        public virtual void Start()
        {
            /* Simulated start */
        }

        public virtual void Stop()
        {
            /* Simulated stop */
        }

        public void Dispose()
        {
        }

        // Simulate receiving a complete message.
        public void SimulateMessage(byte[] message)
        {
            // Use the real parser to process the message.
            var parser = new SpectrumParser();
            var result = parser.ProcessMessage(message);
            SpectrumReceived?.Invoke(result);
        }
    }

    // Wrapper to allow FakeUartSpectrumAcquirer to be used in UartSpectrumManager.
    internal class FakeAcquirerWrapper : SpectrumAcquirer
    {
        private readonly FakeUartSpectrumAcquirer _fake;

        public override event Action<DataStruct>? SpectrumReceived
        {
            add { _fake.SpectrumReceived += value; }
            remove { _fake.SpectrumReceived -= value; }
        }

        public FakeAcquirerWrapper(FakeUartSpectrumAcquirer fake)
            : base("COM_FAKE", 115200, new SpectrumParser())
        {
            _fake = fake;
        }

        public override void Start() => _fake.Start();
        public override void Stop() => _fake.Stop();
    }

    [TestClass]
    public class SpectrumManagerTests
    {
        // Generates a test message according to UartFirstMessageStruct.
        // Fills first spectrum data with sequential ushort values (1, 2, …).
        // The additional fields (average, SNR, quality) are filled with zeros.
        private byte[] GenerateTestMessage()
        {
            var totalLength = MessageStruct1.TotalMessageLength;
            var message = new byte[totalLength];
            var offset = 0;

            // Write spectrum delimiter (4 bytes); remains zeros.
            offset += MessageStruct1.SpectrumDelimiterLegth;

            // Write spectrum data (4096 bytes representing 2048 ushort values).
            var valueCount = MessageStruct1.SpectrumLength / 2;
            for (var i = 0; i < valueCount; i++)
            {
                // Each ushort value is (i+1)
                var value = (ushort)(i + 1);
                Buffer.BlockCopy(BitConverter.GetBytes(value), 0, message, offset, 2);
                offset += 2;
            }

            // Write additional fields: average, SNR, quality (2 bytes each).
            // For testing, these fields are left as zeros.
            offset += MessageStruct1.SpectrumAverageLength +
                      MessageStruct1.SpectrumSnrLength +
                      MessageStruct1.SpectrumQualityLength;

            return message;
        }

        [TestMethod]
        public void StartAcquisition_ProcessesSpectrumData_Correctly()
        {
            // Arrange
            var fakeAcquirer = new FakeUartSpectrumAcquirer();
            var acquirerWrapper = new FakeAcquirerWrapper(fakeAcquirer);
            var manager = new SpectrumManager(acquirerWrapper);

            Dictionary<double, double>? receivedData = null;
            double? receivedAverage = null;
            double? receivedSnr = null;
            double? receivedQuality = null;

            manager.StartAcquisition(
                updateUi: dict => receivedData = dict,
                updateSpectrumInfo: (avg, snr, quality) =>
                {
                    receivedAverage = avg;
                    receivedSnr = snr;
                    receivedQuality = quality;
                }
            );

            // Act
            var testMessage = GenerateTestMessage();
            fakeAcquirer.SimulateMessage(testMessage);
            Task.Delay(100).Wait();

            // Assert
            Assert.IsNotNull(receivedData);

            var expectedLength = MessageStruct1.SpectrumLength / 2;
            Assert.AreEqual(expectedLength, receivedData!.Count);

            // Check first and last wavelength-intensity pairs
            var firstPair = receivedData.First();
            var lastPair = receivedData.Last();

            Assert.AreEqual(SpectrumCalc.WaveLength(0, 1), firstPair.Key);
            Assert.AreEqual(1.0, firstPair.Value);

            Assert.AreEqual(SpectrumCalc.WaveLength(expectedLength - 1, 1), lastPair.Key);
            Assert.AreEqual(expectedLength, lastPair.Value);

            manager.StopAcquisition();
        }

        [TestMethod]
        public void FlipInvertFlag_InvertsSpectrumData()
        {
            var fakeAcquirer = new FakeUartSpectrumAcquirer();
            var acquirerWrapper = new FakeAcquirerWrapper(fakeAcquirer);
            var manager = new SpectrumManager(acquirerWrapper);

            Dictionary<double, double>? receivedDataNormal = null;
            Dictionary<double, double>? receivedDataInverted = null;

            manager.StartAcquisition(
                updateUi: dict => receivedDataNormal = dict,
                updateSpectrumInfo: (average, snr, quality) =>
                {
                    /* Ignored for this test */
                }
            );
            var testMessage = GenerateTestMessage();
            fakeAcquirer.SimulateMessage(testMessage);
            Task.Delay(100).Wait();
            manager.StopAcquisition();

            manager.FlipInvertFlag();
            manager.StartAcquisition(
                updateUi: dict => receivedDataInverted = dict,
                updateSpectrumInfo: (average, snr, quality) =>
                {
                    /* Ignored for this test */
                }
            );
            fakeAcquirer.SimulateMessage(testMessage);
            Task.Delay(100).Wait();
            manager.StopAcquisition();

            Assert.IsNotNull(receivedDataNormal);
            Assert.IsNotNull(receivedDataInverted);

            var normalFirst = receivedDataNormal!.FirstOrDefault().Value;
            var invertedFirst = receivedDataInverted!.FirstOrDefault().Value;

            var actualMax = receivedDataNormal.Values.Max();
            var expectedNormalFirst = receivedDataNormal.Values.Min();
            var expectedInvertedFirst = actualMax - expectedNormalFirst;

            Assert.AreEqual(expectedNormalFirst, normalFirst);
            Assert.AreEqual(expectedInvertedFirst, invertedFirst);
        }
    }
}