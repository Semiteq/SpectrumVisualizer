using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            // Fill the message with the delimiter at the start.
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

        [TestMethod]
        public void ProcessBuffer_NoiseBeforeValidMessage_RaisesEvent()
        {
            // Arrange
            // Simulate noise bytes followed by a valid type1 message.
            var noise = new byte[] { 0x00, 0xFF, 0x10, 0x20 };
            var message = new byte[MessageStruct1.TotalMessageLength];
            Array.Copy(MessageStruct1.SpectrumDelimiter, message, MessageStruct1.SpectrumDelimiter.Length);
            for (var i = MessageStruct1.SpectrumDelimiter.Length; i < MessageStruct1.TotalMessageLength; i++)
            {
                message[i] = 0x55;
            }
            var combined = noise.Concat(message).ToArray();

            var expectedData = new DataStruct(MessageStruct1.SpectrumLength / 2);
            DataStruct? receivedData = null;
            _mockParser.Setup(p => p.ProcessMessage(It.IsAny<byte[]>()))
                       .Returns(expectedData);
            _acquirer.SpectrumReceived += data => receivedData = data;

            // Act: Append combined data without clearing previous content.
            _acquirer.TestClearBuffer();
            _acquirer.AppendToBuffer(combined);
            _acquirer.InvokeProcessBuffer();

            // Assert
            Assert.IsNotNull(receivedData, "Event should be raised for valid message after noise.");
            Assert.AreEqual(0, _acquirer.TestBufferCount, "Buffer should be empty after processing complete message.");
        }

        [TestMethod]
        public void ProcessBuffer_MultipleMessages_RaisesEventForEach()
        {
            // Arrange
            // Create two valid type1 messages concatenated.
            var message = new byte[MessageStruct1.TotalMessageLength];
            Array.Copy(MessageStruct1.SpectrumDelimiter, message, MessageStruct1.SpectrumDelimiter.Length);
            for (var i = MessageStruct1.SpectrumDelimiter.Length; i < MessageStruct1.TotalMessageLength; i++)
            {
                message[i] = 0x55;
            }
            var combined = message.Concat(message).ToArray();

            var callCount = 0;
            _mockParser.Setup(p => p.ProcessMessage(It.IsAny<byte[]>()))
                       .Returns(new DataStruct(MessageStruct1.SpectrumLength / 2))
                       .Callback(() => callCount++);
            _acquirer.SpectrumReceived += data => { };

            // Act
            _acquirer.TestClearBuffer();
            _acquirer.AppendToBuffer(combined);
            _acquirer.InvokeProcessBuffer();

            // Assert
            Assert.AreEqual(2, callCount, "Two messages should have been processed.");
            Assert.AreEqual(0, _acquirer.TestBufferCount, "Buffer should be empty after processing all messages.");
        }

        [TestMethod]
        public void ProcessBuffer_PartialThenCompleteMessage_RaisesEventAfterCompletion()
        {
            // Arrange
            // Create a valid type1 message and split it into two parts.
            var message = new byte[MessageStruct1.TotalMessageLength];
            Array.Copy(MessageStruct1.SpectrumDelimiter, message, MessageStruct1.SpectrumDelimiter.Length);
            for (var i = MessageStruct1.SpectrumDelimiter.Length; i < MessageStruct1.TotalMessageLength; i++)
            {
                message[i] = 0x55;
            }
            var part1 = message.Take(MessageStruct1.TotalMessageLength - 2).ToArray();
            var part2 = message.Skip(MessageStruct1.TotalMessageLength - 2).ToArray();

            var expectedData = new DataStruct(MessageStruct1.SpectrumLength / 2);
            DataStruct? receivedData = null;
            _mockParser.Setup(p => p.ProcessMessage(It.IsAny<byte[]>()))
                       .Returns(expectedData);
            _acquirer.SpectrumReceived += data => receivedData = data;

            // Act: Append first part and process (should not trigger event).
            _acquirer.TestClearBuffer();
            _acquirer.AppendToBuffer(part1);
            _acquirer.InvokeProcessBuffer();
            Assert.IsNull(receivedData, "Incomplete message should not trigger event.");
            // Now append the remaining part.
            _acquirer.AppendToBuffer(part2);
            _acquirer.InvokeProcessBuffer();

            // Assert: Event should now be raised.
            Assert.IsNotNull(receivedData, "Event should be raised after complete message is available.");
            Assert.AreEqual(0, _acquirer.TestBufferCount, "Buffer should be empty after complete message processed.");
        }

        // Testable subclass that exposes buffer manipulation and ProcessBuffer for testing.
        private class TestableAcquirer : SpectrumAcquirer
        {
            public TestableAcquirer(string portName, int baudRate, ISpectrumParser parser)
                : base(portName, baudRate, parser)
            {
            }

            // Clears the internal buffer.
            public void TestClearBuffer()
            {
                lock (GetBufferLock())
                {
                    GetBuffer().Clear();
                }
            }

            // Appends data to the internal buffer.
            public void AppendToBuffer(byte[] data)
            {
                lock (GetBufferLock())
                {
                    GetBuffer().AddRange(data);
                }
            }

            // Invokes the ProcessBuffer method.
            public void InvokeProcessBuffer()
            {
                ProcessBuffer();
            }

            // Exposes current count of buffer.
            public int TestBufferCount
            {
                get
                {
                    lock (GetBufferLock())
                    {
                        return GetBuffer().Count;
                    }
                }
            }

            // Expose the protected ProcessBuffer method for testing.
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
