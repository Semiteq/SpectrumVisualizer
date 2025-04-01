using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpectrumVisualizer.Uart.Message;
using SpectrumVisualizer.Uart.SpectrumJobs;

namespace SpectrumVisualizer.Tests.Uart
{
    // TestableSpectrumAcquirer extends SpectrumAcquirer and provides a constructor for testing.
    public class TestableSpectrumAcquirer : SpectrumAcquirer
    {
        // Constructor that passes parameters to the base class.
        public TestableSpectrumAcquirer(string portName, int baudRate, ISpectrumParser parser)
            : base(portName, baudRate, parser)
        {
        }

        // Expose the protected ProcessBuffer() via a public method for testing.
        public void ProcessBufferPublic() =>
            GetType().GetMethod("ProcessBuffer", BindingFlags.NonPublic | BindingFlags.Instance)!
                   .Invoke(this, null);

        // Inject data into the private _buffer field via reflection.
        public void InjectBuffer(byte[] data)
        {
            var field = typeof(SpectrumAcquirer).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance);
            var ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Position = 0;
            field!.SetValue(this, ms);
        }
    }

    [TestClass]
    public class SpectrumAcquirerTests
    {
        // Helper: Create valid Type1 message.
        // Перенесён в данный класс, чтобы не зависеть от SpectrumParserTests.
        private byte[] CreateValidType1Message(ushort avg = 100, ushort snr = 200, ushort quality = 300)
        {
            // Total length: 4122 bytes.
            var message = new byte[4122];
            // Header for Type1: [0x01,0xFF,...,0x1E] (10 bytes)
            var header = new byte[] { 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x1E };
            Array.Copy(header, 0, message, 0, 10);
            // Spectrum data: 4096 bytes. Fill with incremental values.
            for (int i = 0; i < 4096; i += 2)
            {
                var value = (ushort)(i / 2);
                message[10 + i] = (byte)(value >> 8);
                message[10 + i + 1] = (byte)(value & 0xFF);
            }
            // Average at position 4106
            message[4106] = (byte)(avg >> 8);
            message[4107] = (byte)(avg & 0xFF);
            // SNR at position 4108
            message[4108] = (byte)(snr >> 8);
            message[4109] = (byte)(snr & 0xFF);
            // Quality at position 4110
            message[4110] = (byte)(quality >> 8);
            message[4111] = (byte)(quality & 0xFF);
            // Footer for Type1: [0x1E,0xFF,...,0x01] (10 bytes)
            var footer = new byte[] { 0x1E, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 };
            Array.Copy(footer, 0, message, 4122 - 10, 10);
            return message;
        }

        private SpectrumParser CreateParser() => new();

        [TestMethod]
        public void ProcessBuffer_CompleteType1Message_FiresSpectrumReceivedEvent()
        {
            // Arrange
            var parser = CreateParser();
            using var acquirer = new TestableSpectrumAcquirer("COM1", 9600, parser);
            DataStruct receivedData = null;
            acquirer.SpectrumReceived += data => receivedData = data;
            var message = CreateValidType1Message();
            // Inject a complete message into the buffer.
            acquirer.InjectBuffer(message);

            // Act
            acquirer.ProcessBufferPublic();

            // Assert: SpectrumReceived должен сработать.
            Assert.IsNotNull(receivedData);
            Assert.AreEqual(2048, receivedData.Spectrum.Length);
        }

        [TestMethod]
        public void ProcessBuffer_MultipleMessages_FiresSpectrumReceivedMultipleTimes()
        {
            // Arrange
            var parser = CreateParser();
            using var acquirer = new TestableSpectrumAcquirer("COM1", 9600, parser);
            int eventCount = 0;
            acquirer.SpectrumReceived += _ => eventCount++;
            var message1 = CreateValidType1Message();
            var message2 = CreateValidType1Message();
            var combined = message1.Concat(message2).ToArray();
            acquirer.InjectBuffer(combined);

            // Act
            acquirer.ProcessBufferPublic();

            // Assert: Expect 2 events.
            Assert.AreEqual(2, eventCount);
        }

        [TestMethod]
        public void ProcessBuffer_IncompleteMessage_DoesNotFireEvent()
        {
            // Arrange
            var parser = CreateParser();
            using var acquirer = new TestableSpectrumAcquirer("COM1", 9600, parser);
            bool eventFired = false;
            acquirer.SpectrumReceived += _ => eventFired = true;
            var incomplete = new byte[15];
            acquirer.InjectBuffer(incomplete);

            // Act
            acquirer.ProcessBufferPublic();

            // Assert: Event не должен срабатывать.
            Assert.IsFalse(eventFired);
        }

        [TestMethod]
        public void ProcessBuffer_MessageWithInvalidHeader_DoesNotFireEvent()
        {
            // Arrange
            var parser = CreateParser();
            using var acquirer = new TestableSpectrumAcquirer("COM1", 9600, parser);
            bool eventFired = false;
            acquirer.SpectrumReceived += _ => eventFired = true;
            var message = CreateValidType1Message();
            message[0] = 0xFF; // invalid header
            acquirer.InjectBuffer(message);

            // Act
            acquirer.ProcessBufferPublic();

            // Assert: Event не должен срабатывать.
            Assert.IsFalse(eventFired);
        }

        [TestMethod]
        public void ProcessBuffer_MessageWithInvalidFooter_DoesNotFireEvent()
        {
            // Arrange
            var parser = CreateParser();
            using var acquirer = new TestableSpectrumAcquirer("COM1", 9600, parser);
            bool eventFired = false;
            acquirer.SpectrumReceived += _ => eventFired = true;
            var message = CreateValidType1Message();
            message[^1] = 0x00; // invalid footer
            acquirer.InjectBuffer(message);

            // Act
            acquirer.ProcessBufferPublic();

            // Assert: Event не должен срабатывать.
            Assert.IsFalse(eventFired);
        }

        [TestMethod]
        public void ProcessBuffer_Buffering_PartialThenCompleteMessage_FiresEventOnce()
        {
            // Arrange
            var parser = CreateParser();
            using var acquirer = new TestableSpectrumAcquirer("COM1", 9600, parser);
            int eventCount = 0;
            acquirer.SpectrumReceived += _ => eventCount++;

            var message = CreateValidType1Message();
            int splitIndex = message.Length / 2;
            var part1 = message.Take(splitIndex).ToArray();
            var part2 = message.Skip(splitIndex).ToArray();

            // Inject first part.
            acquirer.InjectBuffer(part1);
            acquirer.ProcessBufferPublic();
            Assert.AreEqual(0, eventCount);

            // Append second part.
            var currentBuffer = part1.Concat(part2).ToArray();
            acquirer.InjectBuffer(currentBuffer);
            acquirer.ProcessBufferPublic();

            // Assert: Событие должно сработать один раз.
            Assert.AreEqual(1, eventCount);
        }
    }
}
