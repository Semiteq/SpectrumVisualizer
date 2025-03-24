using SpectrumVisualizer.Uart.SpectrumJobs;

namespace SpectrumVisualizer.Tests
{
    [TestClass]
    public class UartSpectrumParserTests
    {
        private ISpectrumParser _parser;

        [TestInitialize]
        public void Setup() => _parser = new UartSpectrumParser();

        // Generates a valid test message based on UartMessageStruct constants.
        private byte[] GenerateValidTestMessage()
        {
            int totalLength = UartMessageStruct.TotalMessageLength;
            byte[] message = new byte[totalLength];
            int offset = 0;

            // First spectrum delimiter (2 bytes) - leave as zeros.
            offset += UartMessageStruct.FirstSpectrumDelimiterLegth;

            // First spectrum data (4096 bytes = 2048 ushort values).
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

            // Second spectrum delimiter (2 bytes) - leave as zeros.
            offset += UartMessageStruct.SecondSpectrumDelimiterLegth;

            // Second spectrum data (1024 bytes = 512 ushort values).
            int secondCount = UartMessageStruct.SecondSpectrumLength / 2;
            for (int i = 0; i < secondCount; i++)
            {
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
        public void ProcessMessage_ValidMessage_ReturnsCombinedSpectrum()
        {
            // Arrange: Create a valid UART message.
            var message = GenerateValidTestMessage();

            // Act: Process the message.
            var combined = _parser.ProcessMessage(message);

            // Assert: Verify the combined length and values.
            int expectedLength = (UartMessageStruct.FirstSpectrumLength / 2) + (UartMessageStruct.SecondSpectrumLength / 2);
            Assert.AreEqual(expectedLength, combined.Length);

            // Verify first and last values.
            Assert.AreEqual(1, combined[0]);
            Assert.AreEqual(expectedLength, combined[expectedLength - 1]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessMessage_InvalidLength_ThrowsArgumentException()
        {
            // Arrange: Create an invalid message with incorrect length.
            byte[] invalidMessage = new byte[UartMessageStruct.TotalMessageLength - 1];

            // Act & Assert: Expect an ArgumentException.
            _parser.ProcessMessage(invalidMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessMessage_EmptyMessage_ThrowsArgumentException()
        {
            // Arrange: Create an empty message.
            byte[] emptyMessage = new byte[0];

            // Act & Assert: Expect an ArgumentException.
            _parser.ProcessMessage(emptyMessage);
        }
    }
}
