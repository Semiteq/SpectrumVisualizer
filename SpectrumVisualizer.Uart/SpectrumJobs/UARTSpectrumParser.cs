using System;

namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    /// <summary>
    /// Interface for parsing a complete UART message into a combined spectrum.
    /// </summary>
    public interface ISpectrumParser
    {
        /// <summary>
        /// Processes a full message and returns combined spectrum data as an array of ushort.
        /// </summary>
        /// <param name="message">Full message bytes.</param>
        /// <returns>Combined spectrum data (first spectrum followed by second spectrum).</returns>
        ushort[] ProcessMessage(byte[] message);
    }

    /// <summary>
    /// Implementation of ISpectrumParser that extracts two spectrum data blocks and concatenates them.
    /// </summary>
    public class UartSpectrumParser : ISpectrumParser
    {
        public ushort[] ProcessMessage(byte[] message)
        {
            if (message.Length != UartMessageStruct.TotalMessageLength)
                throw new ArgumentException($"Message length must be {UartMessageStruct.TotalMessageLength} bytes.", nameof(message));

            // Extract first spectrum data:
            var firstSpectrumCount = UartMessageStruct.FirstSpectrumLength / 2;
            var firstSpectrumData = new ushort[firstSpectrumCount];
            Buffer.BlockCopy(message, UartMessageStruct.FirstSpectrumDelimiterLegth, firstSpectrumData, 0, UartMessageStruct.FirstSpectrumLength);

            int secondSpectrumOffset = UartMessageStruct.SecondSpectrumOffset;

            // Extract second spectrum data:
            var secondSpectrumCount = UartMessageStruct.SecondSpectrumLength / 2;
            var secondSpectrumData = new ushort[secondSpectrumCount];
            Buffer.BlockCopy(message, secondSpectrumOffset, secondSpectrumData, 0, UartMessageStruct.SecondSpectrumLength);

            // Combine the two arrays into one.
            var combined = new ushort[firstSpectrumCount + secondSpectrumCount];
            Array.Copy(firstSpectrumData, 0, combined, 0, firstSpectrumCount);
            Array.Copy(secondSpectrumData, 0, combined, firstSpectrumCount, secondSpectrumCount);
            return combined;
        }
    }
}
