namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    public static class SpectrumCalc
    {
        // Data obtained directly from the spectrometers via vendor dll
        private static readonly double[] FirstSpectrometerCoefficients = [185.89856, 0.390762, 1.126E-5, -8.969E-10];
        private static readonly double[] SecondSpectrometerCoefficients = [895.373169, 3.00962138, 9.568E-5, 4.519E-8];
        public static double WaveLength(int pixel, int messageNumber)
        {
            double wavelength = 0;
            switch (messageNumber)
            {
                case 1:
                    wavelength += FirstSpectrometerCoefficients.Select((t, i) => t * Math.Pow(pixel, i)).Sum();
                    return wavelength;

                case 2:
                    wavelength += SecondSpectrometerCoefficients.Select((t, i) => t * Math.Pow(pixel, i)).Sum();
                    return wavelength;

                default:
                    throw new ArgumentOutOfRangeException(nameof(messageNumber));
            }

        }
    }
}
