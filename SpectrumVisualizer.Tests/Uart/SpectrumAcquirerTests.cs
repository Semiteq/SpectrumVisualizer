using Moq;
using SpectrumVisualizer.Uart.Message;
using SpectrumVisualizer.Uart.SpectrumJobs;

namespace SpectrumVisualizer.Tests.Uart
{
    [TestClass]
    public class SpectrumAcquirerTests
    {
        private Mock<ISpectrumParser> _mockParser;
        private TestableAcquirer _acquirer;

        [TestInitialize]
        public void Setup()
        {
            _mockParser = new Mock<ISpectrumParser>();
            _acquirer = new TestableAcquirer("COM1", 115200, _mockParser.Object);
        }

        [TestMethod]
        public void ProcessBuffer_ValidMessageType1_RaisesEvent()
        {
            // Arrange
            var message = new byte[MessageStruct1.TotalMessageLength];
            var expectedData = new DataStruct(MessageStruct1.SpectrumLength / 2);
            DataStruct? receivedData = null;

            _mockParser.Setup(p => p.ProcessMessage(It.Is<byte[]>(b => b.Length == MessageStruct1.TotalMessageLength)))
                .Returns(expectedData);
            _acquirer.SpectrumReceived += data => receivedData = data;

            // Act
            _acquirer.TestProcessBuffer(message);

            // Assert
            Assert.IsNotNull(receivedData);
            _mockParser.Verify(p => p.ProcessMessage(It.Is<byte[]>(b => b.Length == MessageStruct1.TotalMessageLength)),
                Times.Once);
        }

        [TestMethod]
        public void ProcessBuffer_ValidMessageType2_RaisesEvent()
        {
            // Arrange
            var message = new byte[MessageStruct2.TotalMessageLength];
            var expectedData = new DataStruct(MessageStruct2.SpectrumLength / 2);
            DataStruct? receivedData = null;

            _mockParser.Setup(p => p.ProcessMessage(It.Is<byte[]>(b => b.Length == MessageStruct2.TotalMessageLength)))
                .Returns(expectedData);
            _acquirer.SpectrumReceived += data => receivedData = data;

            // Act
            _acquirer.TestProcessBuffer(message);

            // Assert
            Assert.IsNotNull(receivedData);
            _mockParser.Verify(p => p.ProcessMessage(It.Is<byte[]>(b => b.Length == MessageStruct2.TotalMessageLength)),
                Times.Once);
        }

        [TestMethod]
        public void ProcessBuffer_InvalidLength_LogsErrorOnly()
        {
            // Arrange
            var message = new byte[MessageStruct1.TotalMessageLength - 1];
            DataStruct? receivedData = null;
            _acquirer.SpectrumReceived += data => receivedData = data;

            // Act
            _acquirer.TestProcessBuffer(message);

            // Assert
            Assert.IsNull(receivedData);
            _mockParser.Verify(p => p.ProcessMessage(It.IsAny<byte[]>()), Times.Never);
        }

        private class TestableAcquirer : SpectrumAcquirer
        {
            public TestableAcquirer(string portName, int baudRate, ISpectrumParser parser)
                : base(portName, baudRate, parser)
            {
            }

            public void TestProcessBuffer(byte[] data)
            {
                GetBuffer().Clear();
                GetBuffer().AddRange(data);
                ProcessBuffer();
            }

            private List<byte> GetBuffer() =>
                (List<byte>)GetType().BaseType
                    .GetField("_buffer",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .GetValue(this);

            public override void Start()
            {
            }

            public override void Stop()
            {
            }
        }
    }
}