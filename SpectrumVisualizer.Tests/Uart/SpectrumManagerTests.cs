using Moq;
using SpectrumVisualizer.Uart.Message;
using SpectrumVisualizer.Uart.SpectrumJobs;

namespace SpectrumVisualizer.Tests.Uart
{
    [TestClass]
    public class SpectrumManagerTests
    {
        [TestMethod]
        public void StartAcquisition_SpectrumReceived_EventTriggersUpdateCallbacks()
        {
            // Arrange
            var mockAcquirer = new Mock<SpectrumAcquirer>("COM1", 9600, new SpectrumParser());
            // Use TestableSpectrumManager by subscribing to the event.
            var manager = new SpectrumManager(mockAcquirer.Object);

            Dictionary<double, double> uiSpectrum = null;
            double avg = 0, snr = 0, quality = 0;
            void UpdateUi(Dictionary<double, double> dict) => uiSpectrum = dict;
            void UpdateSpectrumInfo(double a, double s, double q)
            {
                avg = a;
                snr = s;
                quality = q;
            }

            // Prepare a dummy DataStruct with known spectrum data.
            var spectrumLength = 2048;
            var dataStruct = new DataStruct(spectrumLength)
            {
                Average = 123,
                Snr = 234,
                Quality = 345
            };
            // Fill spectrum with a known pattern.
            for (var i = 0; i < spectrumLength; i++)
                dataStruct.Spectrum[i] = (ushort)i;

            // Capture the SpectrumReceived delegate.
            Action<DataStruct> capturedHandler = null;
            mockAcquirer.SetupAdd(a => a.SpectrumReceived += It.IsAny<Action<DataStruct>>())
                        .Callback<Action<DataStruct>>(handler => capturedHandler = handler);

            // Act
            manager.StartAcquisition(UpdateUi, UpdateSpectrumInfo);
            // Simulate event firing.
            capturedHandler?.Invoke(dataStruct);

            // Assert: Verify callbacks were called and processed data.
            Assert.IsNotNull(uiSpectrum);
            // Check one sample from UI dictionary (wavelength calculated via SpectrumCalc.WaveLength)
            // Так как вычисление длины волны зависит от спектрометра, проверим, что количество элементов совпадает.
            Assert.AreEqual(spectrumLength, uiSpectrum.Count);
            Assert.AreEqual(123, avg);
            Assert.AreEqual(234, snr);
            Assert.AreEqual(345, quality);
        }

        [TestMethod]
        public void FlipInvertFlag_InvertsSpectrumValues()
        {
            // Arrange
            var mockAcquirer = new Mock<SpectrumAcquirer>("COM1", 9600, new SpectrumParser());
            var manager = new SpectrumManager(mockAcquirer.Object);

            Dictionary<double, double> uiSpectrum = null;
            void UpdateUi(Dictionary<double, double> dict) => uiSpectrum = dict;
            void UpdateSpectrumInfo(double a, double s, double q) { /* no-op */ }

            var spectrumLength = 4;
            var dataStruct = new DataStruct(spectrumLength)
            {
                Average = 0,
                Snr = 0,
                Quality = 0
            };
            // Set spectrum values.
            dataStruct.Spectrum[0] = 10;
            dataStruct.Spectrum[1] = 20;
            dataStruct.Spectrum[2] = 30;
            dataStruct.Spectrum[3] = 40;

            // Subscribe to event.
            Action<DataStruct> capturedHandler = null;
            mockAcquirer.SetupAdd(a => a.SpectrumReceived += It.IsAny<Action<DataStruct>>())
                        .Callback<Action<DataStruct>>(handler => capturedHandler = handler);

            // Act: Start acquisition without inversion.
            manager.StartAcquisition(UpdateUi, UpdateSpectrumInfo);
            capturedHandler?.Invoke(dataStruct);

            // Capture original values.
            var original = uiSpectrum.Values.ToArray();

            // Act: Enable inversion.
            manager.FlipInvertFlag();
            uiSpectrum = null;
            // For inversion, maximum value is 40 so inverted: 30,20,10,0.
            capturedHandler?.Invoke(dataStruct);
            var inverted = uiSpectrum.Values.ToArray();

            // Assert: Check that inversion was applied.
            Assert.AreEqual(4, original.Length);
            Assert.AreEqual(4, inverted.Length);
            Assert.AreEqual(40 - 10, inverted[0]);
            Assert.AreEqual(40 - 20, inverted[1]);
            Assert.AreEqual(40 - 30, inverted[2]);
            Assert.AreEqual(40 - 40, inverted[3]);
        }
    }
}