using Device.ATR.Devices;

namespace SpectrumVisualizer.Device
{
    /// <summary>
    /// Stores general info about device.
    /// </summary>
    internal struct DeviceGeneralInfo
    {
        /// <summary>
        /// Returns pixel quantity.
        /// </summary>
        
        public static int DataSize { get; private set; }
        
        /// <summary>
        /// Returns coefficients for polinomial normalization.
        /// </summary>
        public static float[]? Coeff { get; private set; }  
        
        /// <summary>
        /// Returns partnumber.
        /// </summary>
        public static string? PN { get; private set; }
        
        /// <summary>
        /// Returns serial number.
        /// </summary>
        public static string? SN { get; private set; }

        public static void InitializeAsync(DeviceService deviceService)
        {
            DataSize = deviceService.DeviceInfo.CcdSize;
            Coeff = Task.Run(() => deviceService.GetWavelengthCalibrationCoeff()).Result.Coeff;
            PN = Task.Run(() => deviceService.DeviceInfo.PN).Result;
            SN = Task.Run(() => deviceService.DeviceInfo.SN).Result;
        }
    }
}
