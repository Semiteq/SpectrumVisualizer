using SpectrumVisualizer.Uart.Message;

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
        DataStruct ProcessMessage(byte[] message);
    }

    /// <summary>
    /// Implementation of ISpectrumParser that extracts two spectrum data blocks and concatenates them.
    /// </summary>


    public class SpectrumParser : ISpectrumParser
    {
        public DataStruct ProcessMessage(byte[] message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            DataStruct data;
            int spectrumCount;

            switch (message.Length)
            {
                case MessageStruct1.TotalMessageLength:
                    spectrumCount = MessageStruct1.SpectrumLength / 2;
                    data = new DataStruct(spectrumCount);

                    for (var i = 0; i < spectrumCount; i++)
                    {
                        // Each point occupies 2 bytes
                        var offset = MessageStruct1.SpectrumPos + i * 2;
                        data.Spectrum[i] = AggregateTwoBytesFromPos(message, offset);
                    }

                    data.Average = AggregateTwoBytesFromPos(message, MessageStruct1.SpectrumAveragePos);
                    data.Snr = AggregateTwoBytesFromPos(message, MessageStruct1.SpectrumSnrPos);
                    data.Quality = AggregateTwoBytesFromPos(message, MessageStruct1.SpectrumQualityPos);
                    break;

                case MessageStruct2.TotalMessageLength:
                    spectrumCount = MessageStruct2.SpectrumLength / 2;
                    data = new DataStruct(spectrumCount);

                    for (var i = 0; i < spectrumCount; i++)
                    {
                        // Use SpectrumPos from the second message struct
                        var offset = MessageStruct2.SpectrumPos + i * 2;
                        data.Spectrum[i] = AggregateTwoBytesFromPos(message, offset);
                    }

                    data.Average = AggregateTwoBytesFromPos(message, MessageStruct2.SpectrumAveragePos);
                    data.Snr = AggregateTwoBytesFromPos(message, MessageStruct2.SpectrumSnrPos);
                    data.Quality = AggregateTwoBytesFromPos(message, MessageStruct2.SpectrumQualityPos);
                    break;

                default:
                    EventHandler.Log("Unsupported message length");
                    return new DataStruct(0);
            }
            return data;
        }
        
        private static ushort AggregateTwoBytesFromPos(byte [] message, int pos)
        {
            return (ushort)((message[pos] << 8) | message[pos + 1]);
        }
    }


}
