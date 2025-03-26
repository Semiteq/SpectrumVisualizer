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
            var delimiter = MessageStruct1.SpectrumDelimiter;

            // Fill the entire message with the delimiter at the start
            Array.Copy(delimiter, message, delimiter.Length);

            var expectedData = new DataStruct(MessageStruct1.SpectrumLength / 2);
            DataStruct? receivedData = null;

            _mockParser.Setup(p => p.ProcessMessage(It.IsAny<byte[]>()))
                .Returns(expectedData);
            _acquirer.SpectrumReceived += data => receivedData = data;

            // Act
            _acquirer.TestProcessBuffer(message);

            // Assert
            Assert.IsNotNull(receivedData);
            _mockParser.Verify(p => p.ProcessMessage(It.IsAny<byte[]>()), Times.Once);
        }

        [TestMethod]
        public void ProcessBuffer_ValidMessageType2_RaisesEvent()
        {
            // Arrange
            var message = new byte[MessageStruct2.TotalMessageLength];
            MessageStruct2.SpectrumDelimiter.CopyTo(message, 0);
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
                lock (GetBufferLock())
                {
                    GetBuffer().Clear();
                    GetBuffer().AddRange(data);
                }
                ProcessBuffer();
            }

            private List<byte> GetBuffer()
            {
                var field = GetType().BaseType?
                    .GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?? throw new InvalidOperationException("Cannot access buffer field");
                return (List<byte>)field.GetValue(this);
            }

            private object GetBufferLock()
            {
                var field = GetType().BaseType?
                    .GetField("_bufferLock", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?? throw new InvalidOperationException("Cannot access buffer lock field");
                return field.GetValue(this);
            }

            public override void Start() { }
            public override void Stop() { }
        }
    }
}