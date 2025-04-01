namespace SpectrumVisualizer.Uart.Message
{
    public struct MessageStruct2
    {
        public static readonly byte[] SpectrumDelimiter = { 0x02, 0xFF, 0xFF, 0xFF, 0xFF, 
                                                            0xFF, 0xFF, 0xFF, 0xFF, 0x1E};

        // Data presented in bytes
        public const int SpectrumDelimiterLegth = 10;
        public const int SpectrumLength = 1024; // 512 ushort
        public const int SpectrumAverageLength = 2;
        public const int SpectrumSnrLength = 2;
        public const int SpectrumQualityLength = 2;

        public const int SpectrumDelimiterPos = 0;
        public const int SpectrumPos = SpectrumDelimiterPos + SpectrumDelimiterLegth; // 10
        public const int SpectrumAveragePos = SpectrumPos + SpectrumLength; // 1034
        public const int SpectrumSnrPos = SpectrumAveragePos + SpectrumAverageLength; // 1036
        public const int SpectrumQualityPos = SpectrumSnrPos + SpectrumSnrLength; // 1038

        public const int TotalMessageLength = SpectrumQualityPos + SpectrumQualityLength; // 1040
    }
}
