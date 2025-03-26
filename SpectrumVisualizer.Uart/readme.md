# Spectrum Visualizer - UART Version

This project is a spectrum visualization application that acquires data via UART (COM port). It replaces the vendor’s library with a custom implementation using `System.IO.Ports`.

## Features

- **UART Data Acquisition:** 
  - Receives two messages over a COM port.
  - Messages structure is defined in the `Message` (includes two spectrum blocks and additional fields).
- **Data Processing:** 
  - Uses `SpectrumParser` to extract spectrum data blocks into a `DataStruct`.
  - Assigning wavelengths via `SpectrumCalc`.
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
   - Set the correct COM port and baud rate in `DeviceManager.cs`.
   - Adjust `MessageStruct1` and `MessageStruct2` constants if the incoming message structure changes.

3. **Running the Application:**
   - Build the project in Visual Studio via `dotnet publish -r win-x64 -c Release --self-contained true /p:PublishSingleFile=true`.
   - Launch the Spectrum Visualizer application.
   - The application will auto-populate available COM ports; select one to start acquiring and visualizing spectrum data.

4. **Error Handling:**
   - Errors during UART communication are logged using `ErrorHandler`.

## Architecture

- **Abstraction:** 
  - UART message parsing is encapsulated in `SpectrumParser` via the `ISpectrumParser` interface.
  - `SpectrumAcquirer` handles low-level data reception, buffering, and message extraction.
- **Modular Design:** 
  - Spectrum acquisition, processing, and visualization are separated into distinct classes for improved testability and maintainability.
