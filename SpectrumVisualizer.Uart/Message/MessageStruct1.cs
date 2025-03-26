namespace SpectrumVisualizer.Uart.Message
{
    public struct MessageStruct1
    {
        public static readonly byte[] SpectrumDelimiter = { 0x00, 0x00, 0x00, 0x01 };

        // Data presented in bytes
        public const int SpectrumDelimiterLegth = 4;
        public const int SpectrumLength = 4096; // 2048 ushort
        public const int SpectrumAverageLength = 2;
        public const int SpectrumSnrLength = 2;
        public const int SpectrumQualityLength = 2;

        public const int SpectrumDelimiterPos = 0;
        public const int SpectrumPos = SpectrumDelimiterPos + SpectrumDelimiterLegth; // 4
        public const int SpectrumAveragePos = SpectrumPos + SpectrumLength; // 4100
        public const int SpectrumSnrPos = SpectrumAveragePos + SpectrumAverageLength; // 4102
        public const int SpectrumQualityPos = SpectrumSnrPos + SpectrumSnrLength; // 4104

        public const int TotalMessageLength = SpectrumQualityPos + SpectrumQualityLength; // 4106
    }
}