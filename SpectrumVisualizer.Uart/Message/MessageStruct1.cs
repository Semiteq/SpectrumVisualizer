namespace SpectrumVisualizer.Uart.Message
{
    public struct MessageStruct1
    {
        public static readonly byte[] SpectrumHeader = { 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 
                                                            0xFF, 0xFF, 0xFF, 0xFF, 0x1E};
        
        public static readonly byte[] SpectrumFooter = { 0x1E, 0xFF, 0xFF, 0xFF, 0xFF, 
                                                            0xFF, 0xFF, 0xFF, 0xFF, 0x01};

        // Data presented in bytes
        public const int SpectrumDelimiterLegth = 10;
        public const int SpectrumLength = 4096; // 2048 ushort
        public const int SpectrumAverageLength = 2;
        public const int SpectrumSnrLength = 2;
        public const int SpectrumQualityLength = 2;
        public const int SpectrumFooterLength = 10;
        
        
        public const int SpectrumDelimiterPos = 0;
        public const int SpectrumPos = SpectrumDelimiterPos + SpectrumDelimiterLegth; // 10
        public const int SpectrumAveragePos = SpectrumPos + SpectrumLength; // 4106
        public const int SpectrumSnrPos = SpectrumAveragePos + SpectrumAverageLength; // 4108
        public const int SpectrumQualityPos = SpectrumSnrPos + SpectrumSnrLength; // 4110
        public const int SpectrumFooterPos = SpectrumQualityPos + SpectrumQualityLength; // 4112

        public const int TotalMessageLength = SpectrumFooterPos + SpectrumFooterLength; // 4122
        
        
    }
}