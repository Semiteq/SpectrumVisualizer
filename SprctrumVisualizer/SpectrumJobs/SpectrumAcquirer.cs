using Device.ATR.Devices;
using Device.ATR.Model.Spectrometer;
using SpectrumVisualizer.Device;

namespace SpectrumVisualizer.SpectrumJobs
{
    /// <summary>
    /// Acquires spectrum data from the device using the DeviceService.
    /// Handles acquisition of both regular and dark spectra.
    /// Ensures thread-safe access using a semaphore.
    /// </summary>
    public class SpectrumAcquirer
    {
        private readonly DeviceService _deviceService; // Device service to communicate with the spectrometer.
        private readonly SemaphoreSlim _semaphore = new(1, 1); // Semaphore to allow only one acquisition at a time.

        /// <summary>
        /// Initializes a new instance of the <see cref="SpectrumAcquirer"/> class.
        /// </summary>
        /// <param name="deviceService">The device service instance.</param>
        public SpectrumAcquirer(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        /// <summary>
        /// Asynchronously acquires spectrum data from the device, ensuring thread-safe access.
        /// </summary>
        /// <param name="parameter">Acquisition parameters.</param>
        /// <param name="isDark">True if acquiring dark spectrum; otherwise, false.</param>
        /// <returns>An array of doubles representing the acquired spectrum data.</returns>
        public async Task<double[]> AcquireAsync(AcquireParameter parameter, bool isDark = false)
        {
            await _semaphore.WaitAsync(); // Acquire semaphore to ensure exclusive access.
            try
            {
                return isDark
                    ? (await _deviceService.AcquireDark(parameter)).Data // Acquire dark spectrum if isDark is true.
                    : (await _deviceService.Acquire(parameter)).Data;    // Acquire regular spectrum otherwise.
            }
            catch (Exception ex)
            {
                ErrorHandler.Log(ex);
                return new double[DeviceGeneralInfo.DataSize]; // Return an empty data array in case of error.
            }
            finally
            {
                _semaphore.Release(); // Release semaphore, making it available for other operations.
            }
        }
    }
}
