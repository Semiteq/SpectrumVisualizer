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
    [TestClass]
    public class SpectrumParserTests
    {
        // Helper: Create valid Type1 message.
        private byte[] CreateValidType1Message(ushort avg = 100, ushort snr = 200, ushort quality = 300)
        {
            // Total length: 4122 bytes.
            var message = new byte[4122];
            // Header for Type1: [0x01,0xFF,...,0x1E] (10 bytes)
            var header = new byte[] { 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x1E };
            Array.Copy(header, 0, message, 0, 10);
            // Spectrum data: 4096 bytes. Fill with incremental values for test.
            for (int i = 0; i < 4096; i += 2)
            {
                // For example, set each ushort to i/2.
                var value = (ushort)(i / 2);
                message[10 + i] = (byte)(value >> 8);       // high byte
                message[10 + i + 1] = (byte)(value & 0xFF);   // low byte
            }
            // Average at position 4106 (bytes at 4106 and 4107)
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

        // Helper: Create valid Type2 message.
        private byte[] CreateValidType2Message(ushort avg = 110, ushort snr = 210, ushort quality = 310)
        {
            // Total length: 1050 bytes.
            var message = new byte[1050];
            // Header for Type2: [0x02,0xFF,...,0x1E] (10 bytes)
            var header = new byte[] { 0x02, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x1E };
            Array.Copy(header, 0, message, 0, 10);
            // Spectrum data: 1024 bytes. Fill with incremental values.
            for (int i = 0; i < 1024; i += 2)
            {
                var value = (ushort)(i / 2);
                message[10 + i] = (byte)(value >> 8);
                message[10 + i + 1] = (byte)(value & 0xFF);
            }
            // Average at position 1034
            message[1034] = (byte)(avg >> 8);
            message[1035] = (byte)(avg & 0xFF);
            // SNR at position 1036
            message[1036] = (byte)(snr >> 8);
            message[1037] = (byte)(snr & 0xFF);
            // Quality at position 1038
            message[1038] = (byte)(quality >> 8);
            message[1039] = (byte)(quality & 0xFF);
            // Footer for Type2: [0x1E,0xFF,...,0x02] (10 bytes)
            var footer = new byte[] { 0x1E, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x02 };
            Array.Copy(footer, 0, message, 1050 - 10, 10);
            return message;
        }

        [TestMethod]
        public void ProcessMessage_ValidType1_ReturnsCorrectData()
        {
            // Arrange
            var parser = new SpectrumParser();
            var message = CreateValidType1Message(avg: 500, snr: 600, quality: 700);
            // Act
            var data = parser.ProcessMessage(message);
            // Assert
            // Spectrum length should be 2048 for type1
            Assert.AreEqual(2048, data.Spectrum.Length);
            // Verify first spectrum value equals 0 (since incremental, first value is 0)
            Assert.AreEqual(0, data.Spectrum[0]);
            Assert.AreEqual(500, data.Average);
            Assert.AreEqual(600, data.Snr);
            Assert.AreEqual(700, data.Quality);
        }

        [TestMethod]
        public void ProcessMessage_ValidType2_ReturnsCorrectData()
        {
            // Arrange
            var parser = new SpectrumParser();
            var message = CreateValidType2Message(avg: 550, snr: 650, quality: 750);
            // Act
            var data = parser.ProcessMessage(message);
            // Assert
            // Spectrum length should be 512 for type2
            Assert.AreEqual(512, data.Spectrum.Length);
            Assert.AreEqual(0, data.Spectrum[0]);
            Assert.AreEqual(550, data.Average);
            Assert.AreEqual(650, data.Snr);
            Assert.AreEqual(750, data.Quality);
        }

        [TestMethod]
        public void ProcessMessage_InvalidLength_ReturnsEmptyDataStruct()
        {
            // Arrange
            var parser = new SpectrumParser();
            var invalidMessage = new byte[100]; // Incorrect length
            // Act
            var data = parser.ProcessMessage(invalidMessage);
            // Assert: Expecting DataStruct with zero spectrum length
            Assert.AreEqual(0, data.Spectrum.Length);
        }
    }
}