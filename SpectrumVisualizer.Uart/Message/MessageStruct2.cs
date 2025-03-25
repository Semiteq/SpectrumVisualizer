namespace SpectrumVisualizer.Uart.Message
{
    public struct MessageStruct2
    {
        // Data presented in bytes
        public const int SpectrumDelimiterLegth = 4;
        public const int SpectrumLength = 1024; // 512 ushort
        public const int SpectrumAverageLength = 2;
        public const int SpectrumSnrLength = 2;
        public const int SpectrumQualityLength = 2;

        public const int SpectrumDelimiterPos = 0;
        public const int SpectrumPos = SpectrumDelimiterPos + SpectrumDelimiterLegth; // 4
        public const int SpectrumAveragePos = SpectrumPos + SpectrumLength; // 1024
        public const int SpectrumSnrPos = SpectrumAveragePos + SpectrumAverageLength; // 1028
        public const int SpectrumQualityPos = SpectrumSnrPos + SpectrumSnrLength; // 1032

        public const int TotalMessageLength = SpectrumQualityPos + SpectrumQualityLength; // 1036
    }
}
