namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    public static class SpectrumCalc
    {
        // Data optained directly from the spectrometers via vendor dll
        private static readonly double[] firstSpectrometer = [-8.969E-10, 1.126E-5, 0.390762, 185.89856];
        private static readonly double[] secondSpectrometer = [4.519E-8, 9.568E-5, 3.00962138, 895.373169];
        public static double WaveLength(int pixel, int messageNumber)
        {
            double wavelength = 0;
            switch (messageNumber)
            {
                case 1:

                    for (int i = 0; i < firstSpectrometer.Length; i++)
                    {
                        wavelength += firstSpectrometer[i] * Math.Pow(pixel, i);
                    }

                    return wavelength;

                case 2:

                    for (int i = 0; i < secondSpectrometer.Length; i++)
                    {
                        wavelength += secondSpectrometer[i] * Math.Pow(pixel, i);
                    }

                    return wavelength;

                default:
                    throw new ArgumentOutOfRangeException(nameof(messageNumber));
            }

        }
    }
}
