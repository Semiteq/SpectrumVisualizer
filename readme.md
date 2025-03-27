# Spectrum Visualizer Solution

This solution contains two implementations of a Spectrum Visualizer application and one test project:

1. **SpectrumVisualizer (Old Project)**
   - Uses a vendor-specific library for device communication.
   
2. **SpectrumVisualizer.Uart (New Project)**
   - Replaces the vendor-specific library with a custom UART implementation using `System.IO.Ports`.

3. **SpectrumVisualizer.Tests**
   - Contains MSTest-based unit tests for both implementations (where applicable).
   - Focuses primarily on testing the new UART-based logic in `SpectrumVisualizer.Uart`.

## Getting Started

1. **Clone and Open the Solution**  
   - Clone this repository and open it in Visual Studio or another .NET-compatible IDE.

2. **Build and Run**  
   - Each project can be built independently.  
   - `SpectrumVisualizer` (old) relies on the vendor library (if still present).  
   - `SpectrumVisualizer.Uart` (new) uses `System.IO.Ports` for UART communication.
   - For single file package use `dotnet publish -r win-x64 -c Release --self-contained true /p:PublishSingleFile=true`

3. **Testing**  
   - Open the Test Explorer in Visual Studio.
   - Build and run the tests from the `SpectrumVisualizer.Tests` project.
   - Tests cover parsing, data acquisition, and inversion logic for the new UART implementation.
