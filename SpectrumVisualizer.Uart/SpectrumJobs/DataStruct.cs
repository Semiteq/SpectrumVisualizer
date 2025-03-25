namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    public class DataStruct(int spectrumLength)
    {
        public readonly double[] Spectrum = new double[spectrumLength];

        public double Average;

        public double Snr;

        public double Quality;
    }
}