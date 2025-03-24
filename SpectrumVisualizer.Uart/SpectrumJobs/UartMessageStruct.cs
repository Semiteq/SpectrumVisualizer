namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    public struct UartMessageStruct
    {
        public const int FirstSpectrumDelimiterLegth = 2;
        public const int FirstSpectrumLength = 4096; // 2048 ushort
        public const int FirstSpectrumAverageLength = 2;
        public const int FirstSpectrumSnrLength = 2;
        public const int FirstSpectrumQualityLength = 2;

        public const int SecondSpectrumDelimiterLegth = 2;
        public const int SecondSpectrumLength = 1024; // 512 ushort
        public const int SecondSpectrumAverageLength = 2;
        public const int SecondSpectrumSnrLength = 2;
        public const int SecondSpectrumQualityLength = 2;

        public const int SecondSpectrumOffset = FirstSpectrumDelimiterLegth +
                                                   FirstSpectrumLength +
                                                   FirstSpectrumAverageLength +
                                                   FirstSpectrumSnrLength +
                                                   FirstSpectrumQualityLength +
                                                   SecondSpectrumDelimiterLegth;

        public const int TotalMessageLength = FirstSpectrumDelimiterLegth +
                                                 FirstSpectrumLength +
                                                 FirstSpectrumAverageLength +
                                                 FirstSpectrumSnrLength +
                                                 FirstSpectrumQualityLength +
                                                 SecondSpectrumDelimiterLegth +
                                                 SecondSpectrumLength +
                                                 SecondSpectrumAverageLength +
                                                 SecondSpectrumSnrLength +
                                                 SecondSpectrumQualityLength;
    }
}
