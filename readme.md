# Spectrum Visualizer Solution

This solution contains two implementations of a Spectrum Visualizer application and one test project:

1. **SpectrumVisualizer (Old Project)**
   - Uses a vendor-specific library for device communication.
   - Maintains dark spectrum functionality and integration time settings.
   - **Directory Structure**:
     ```
     SpectrumVisualizer/
     ├── Device/
     ├── SpectrumJobs/
     ├── ErrorHandler.cs
     ├── mainForm.cs
     ├── Program.cs
     ├── readme.md
     └── index.ico
     ```
   
2. **SpectrumVisualizer.Uart (New Project)**
   - Replaces the vendor-specific library with a custom UART implementation using `System.IO.Ports`.
   - Removes dark spectrum and integration time settings, focusing on receiving normalized data and displaying it.
   - **Directory Structure**:
     ```
     SpectrumVisualizer.Uart/
     ├── Device/
     ├── SpectrumJobs/
     ├── mainForm.cs
     ├── Program.cs
     ├── readme.md
     └── index.ico
     ```

3. **SpectrumVisualizer.Tests**
   - Contains MSTest-based unit tests for both implementations (where applicable).
   - Focuses primarily on testing the new UART-based logic in `SpectrumVisualizer.Uart`.
   - **Directory Structure**:
     ```
     SpectrumVisualizer.Tests/
     ├── MSTestSettings.cs
     ├── SpectrumPainterTests.cs
     ├── UartSpectrumParserTests.cs
     └── readme.md
     ```

## Getting Started

1. **Clone and Open the Solution**  
   - Clone this repository and open it in Visual Studio or another .NET-compatible IDE.

2. **Build and Run**  
   - Each project can be built independently.  
   - `SpectrumVisualizer` (old) relies on the vendor library (if still present).  
   - `SpectrumVisualizer.Uart` (new) uses `System.IO.Ports` for UART communication.

3. **Testing**  
   - Open the Test Explorer in Visual Studio.
   - Build and run the tests from the `SpectrumVisualizer.Tests` project.
   - Tests cover parsing, data acquisition, and inversion logic for the new UART implementation.
