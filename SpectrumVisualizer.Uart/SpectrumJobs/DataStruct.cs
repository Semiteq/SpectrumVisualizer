namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    public class DataStruct(int spectrumLength)
    {
        public readonly ushort[] Spectrum = new ushort[spectrumLength];

        public double Average;

        public double Snr;

        public double Quality;
    }
}