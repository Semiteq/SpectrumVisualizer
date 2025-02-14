# Spectrum Visualizer

A simple C# application for visualizing spectrum data acquired from a spectrometer device.

## Description

This application is designed to connect to a spectrometer device, acquire spectrum data in real-time, and display it graphically. It provides basic functionalities for spectrum analysis and visualization, including:

* **Device Connection:** Connects to a spectrometer via a serial COM port.
* **Real-time Spectrum Acquisition:** Continuously acquires and displays spectrum data.
* **Spectrum Visualization:** Uses OxyPlot library to render interactive spectrum charts.
* **Dark Spectrum Subtraction:** Allows for subtracting a dark spectrum to improve data quality.
* **Adjustable Acquisition Parameters:** Users can control integration time, averaging, and acquisition interval.
* **Spectrum Analysis Tools:** Includes draggable markers for calculating average signal, SNR and Q-factor within a selected wavelength range.
* **Error Logging:** Provides basic error logging and display within the application.

## Features

* **COM Port Selection:**  Easily select the COM port for device connection from a dropdown list.
* **Interactive Spectrum Plot:** Zoomable and pannable spectrum plot using OxyPlot.
* **Adjustable Markers:**  Draggable vertical markers on the spectrum plot to define regions of interest for analysis.
* **Average Signal Calculation:** Calculates and displays the average signal intensity between the markers.
* **SNR Calculation:** Calcualtes SNR and displays the result in dB.
* **Q-factor Calcupation:** Calculates and displays Q-factor in real time.
* **Dark Spectrum Management:**  Acquire and apply dark spectrum correction.
* **Parameter Control:** Adjust integration time, averaging, and interval through numeric inputs.
* **Basic Error Handling and Logging:**  Displays error messages within the application's UI.

## Dependencies

* **Device.ATR Library:**  This project relies on a `Device.ATR` library (likely a custom SDK or library for the specific spectrometer hardware). You'll need to ensure this library is available and properly referenced in your project.
* **OxyPlot:**  Uses the OxyPlot library for creating the spectrum charts. This should be included as a NuGet package dependency.
* **Windows Forms:** Built as a Windows Forms application.

## How to Use

1. **Build the Project:** Open the project in Visual Studio and build the solution.
2. **Run the Application:** Run the compiled executable (`.exe` file usually located in the `bin\Debug` or `bin\Release` folder).
3. **Select COM Port:** In the application window, select the appropriate COM port from the dropdown menu that corresponds to your spectrometer device.
4. **Connect to Device:** The application attempts to connect automatically upon startup and COM port selection.
5. **Adjust Parameters:** Modify the integration time, interval, and averaging settings using the numeric input controls.
6. **Acquire Dark Spectrum (Optional):** If necessary, click the "Acquire Dark" button to capture a dark spectrum for background correction. Click "Reset Dark" to disable dark spectrum subtraction.
7. **Interact with Spectrum:** Observe the real-time spectrum display. Drag the red vertical markers on the plot to select a wavelength range. The average signal within the selected range will be displayed.
8. **Disconnect (On Closing):** When you close the application, it will automatically disconnect from the device.

## Project Structure

The project is structured with the following key components:

* **`DeviceManager.cs`:** Handles device connection and disconnection, manages access to the `DeviceService`.
* **`DeviceGeneralInfo.cs`:**  Struct storing general device parameters, such as pixel range or SN and PN.
* **`DataProcessor.cs`:**  Asynchronously processes data updates to avoid blocking the UI thread.
* **`ErrorHandler.cs`:**  Provides centralized error logging and display.
* **`mainForm.cs`:**  The main application form, handles UI events and interactions.
* **`Program.cs`:**  Application entry point.
* **`SpectrumManager.cs`:**  Manages spectrum acquisition, analysis, and dark spectrum handling.
* **`SpectrumNormalizer.cs`:**  Performs wavelength calibration and spectrum normalization.
* **`SpectrumPainter.cs`:**  Handles the visualization of the spectrum using OxyPlot.
* **`SpectrumAcquirer.cs`:**  Acquires raw spectrum data from the device.
* **`SpectrumAnalyzer.cs`:**  Analyzes spectrum data, including dark subtraction and normalization.
* **`SpectrumCalculator.cs`:**  Static class with spectrum analize functions.

## Notes

* Ensure that the `Device.ATR` library is correctly configured and that your spectrometer device is properly connected and powered on.
* This is a basic spectrum visualizer and may require further development for more advanced features and specific spectrometer models.

---