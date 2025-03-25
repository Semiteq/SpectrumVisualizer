using SpectrumVisualizer.Uart.Message;
using SpectrumVisualizer.Uart.SpectrumJobs;

namespace SpectrumVisualizer.Tests.Uart
{
    [TestClass]
    public class SpectrumParserTests
    {
        private ISpectrumParser _parser;

        [TestInitialize]
        public void Setup() => _parser = new SpectrumParser();

        private byte[] GenerateTestMessage(int totalLength)
        {
            var message = new byte[totalLength];
            var offset = 0;

            if (totalLength == MessageStruct1.TotalMessageLength)
            {
                offset += MessageStruct1.SpectrumDelimiterLegth;

                var valueCount = MessageStruct1.SpectrumLength / 2;
                for (var i = 0; i < valueCount; i++)
                {
                    var value = (ushort)(i + 1);
                    Buffer.BlockCopy(BitConverter.GetBytes(value), 0, message, offset, 2);
                    offset += 2;
                }

                // Set test values for additional fields
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)100), 0, message, MessageStruct1.SpectrumAveragePos, 2);
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)200), 0, message, MessageStruct1.SpectrumSnrPos, 2);
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)300), 0, message, MessageStruct1.SpectrumQualityPos, 2);
            }
            else if (totalLength == MessageStruct2.TotalMessageLength)
            {
                offset += MessageStruct2.SpectrumDelimiterLegth;

                var valueCount = MessageStruct2.SpectrumLength / 2;
                for (var i = 0; i < valueCount; i++)
                {
                    var value = (ushort)(i + 1);
                    Buffer.BlockCopy(BitConverter.GetBytes(value), 0, message, offset, 2);
                    offset += 2;
                }

                // Set test values for additional fields
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)100), 0, message, MessageStruct2.SpectrumAveragePos, 2);
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)200), 0, message, MessageStruct2.SpectrumSnrPos, 2);
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)300), 0, message, MessageStruct2.SpectrumQualityPos, 2);
            }

            return message;
        }

        [TestMethod]
        public void ProcessMessage_ValidMessageType1_ReturnsCorrectSpectrum()
        {
            var message = GenerateTestMessage(MessageStruct1.TotalMessageLength);
            var result = _parser.ProcessMessage(message);

            int expectedLength = MessageStruct1.SpectrumLength / 2;
            Assert.AreEqual(expectedLength, result.Spectrum.Length);
            Assert.AreEqual(1, result.Spectrum[0]);
            Assert.AreEqual(expectedLength, result.Spectrum[expectedLength - 1]);
            Assert.AreEqual(100, result.Average);
            Assert.AreEqual(200, result.Snr);
            Assert.AreEqual(300, result.Quality);
        }

        [TestMethod]
        public void ProcessMessage_ValidMessageType2_ReturnsCorrectSpectrum()
        {
            var message = GenerateTestMessage(MessageStruct2.TotalMessageLength);
            var result = _parser.ProcessMessage(message);

            int expectedLength = MessageStruct2.SpectrumLength / 2;
            Assert.AreEqual(expectedLength, result.Spectrum.Length);
            Assert.AreEqual(1, result.Spectrum[0]);
            Assert.AreEqual(expectedLength, result.Spectrum[expectedLength - 1]);
            Assert.AreEqual(100, result.Average);
            Assert.AreEqual(200, result.Snr);
            Assert.AreEqual(300, result.Quality);
        }

        [TestMethod]
        public void ProcessMessage_InvalidLength_ReturnsEmptyDataStruct()
        {
            // Test too short message
            var tooShortMessage = new byte[MessageStruct1.TotalMessageLength - 1];
            var tooShortResult = _parser.ProcessMessage(tooShortMessage);
            Assert.AreEqual(0, tooShortResult.Spectrum.Length);

            // Test message length between valid lengths
            var invalidLengthMessage = new byte[MessageStruct1.TotalMessageLength + 1];
            var invalidLengthResult = _parser.ProcessMessage(invalidLengthMessage);
            Assert.AreEqual(0, invalidLengthResult.Spectrum.Length);

            // Test too long message
            var tooLongMessage = new byte[MessageStruct2.TotalMessageLength + 1];
            var tooLongResult = _parser.ProcessMessage(tooLongMessage);
            Assert.AreEqual(0, tooLongResult.Spectrum.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessMessage_NullMessage_ThrowsArgumentNullException()
        {
            _parser.ProcessMessage(null);
        }

        [TestMethod]
        public void ProcessMessage_InvalidLength_ReturnsEmptyDataStructAndLogs()
        {
            // Test various invalid message lengths
            int[] invalidLengths = new[]
            {
                0,  // Empty message
                MessageStruct1.TotalMessageLength - 1,  // Too short for Type 1
                MessageStruct1.TotalMessageLength + 1,  // Between Type 1 and 2
                MessageStruct2.TotalMessageLength + 1   // Too long
            };

            foreach (var length in invalidLengths)
            {
                // Arrange
                var invalidMessage = new byte[length];

                // Act
                var result = _parser.ProcessMessage(invalidMessage);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(0, result.Spectrum.Length);
                Assert.AreEqual(0, result.Average);
                Assert.AreEqual(0, result.Snr);
                Assert.AreEqual(0, result.Quality);
            }
        }
    }
}