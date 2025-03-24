# Spectrum Visualizer - UART Version

This project is a spectrum visualization application that acquires data via UART (COM port). It replaces the vendor’s library with a custom implementation using `System.IO.Ports`.

## Features

- **UART Data Acquisition:** 
  - Receives full messages over a COM port.
  - Message structure is defined by the `UartMessageStruct` (includes two spectrum blocks and additional fields).
- **Data Processing:** 
  - Uses `UartSpectrumParser` to extract and combine two spectrum data blocks into a single `ushort[]` array.
  - No dark spectrum or integration settings are applied – data is already normalized.
- **Visualization:** 
  - Uses OxyPlot for rendering the spectrum.
  - Basic visualization is provided without marker handling.
- **Inversion Support:** 
  - Ability to flip/invert spectrum data based on the maximum intensity in the dataset.

## Getting Started

1. **Dependencies:**
   - .NET 9
   - OxyPlot for plotting
   - System.IO.Ports for UART communication

2. **Configuration:**
   - Set the correct COM port and baud rate (default: `COM3`, `115200`) in `DeviceManager.cs`.
   - Adjust `UartMessageStruct` constants if the incoming message structure changes.

3. **Running the Application:**
   - Build the project in Visual Studio.
   - Launch the Spectrum Visualizer application.
   - The application will auto-populate available COM ports; select one to start acquiring and visualizing spectrum data.

4. **Error Handling:**
   - Errors during UART communication are logged using `ErrorHandler`.

## Architecture

- **Abstraction:** 
  - UART message parsing is encapsulated in `UartSpectrumParser` via the `ISpectrumParser` interface.
  - `UartSpectrumAcquirer` handles low-level data reception, buffering, and message extraction.
- **Modular Design:** 
  - Spectrum acquisition, processing, and visualization are separated into distinct classes for improved testability and maintainability.
