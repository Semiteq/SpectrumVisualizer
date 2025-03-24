# Spectrum Visualizer - UART Project Tests

This test suite verifies the functionality of the UART spectrum acquisition and processing modules for the Spectrum Visualizer project. The tests are written using MSTest.

## Test Coverage

- **UartSpectrumParserTests:**
  - Validate that a correctly formatted UART message is parsed into the expected combined spectrum.
  - Verify that messages with incorrect length throw an exception.

- **SpectrumManagerTests:**
  - Ensure that spectrum acquisition processes the received message correctly.
  - Verify the inversion functionality via the `FlipInvertFlag()` method.
  - Utilize a fake UART acquirer to simulate message reception for controlled testing.

## Prerequisites

- .NET 9
- MSTest framework
- Ensure the project and test solution are configured in Visual Studio (or your preferred IDE) to run MSTest.

## Running the Tests

1. **Using Visual Studio:**
   - Open the Test Explorer.
   - Build the solution.
   - Run all tests; the Test Explorer will display the results.

2. **Using Command Line:**
   - Navigate to the test project directory.
   - Run:  
     ```
     dotnet test
     ```

## Design for Testability

- **Fake Implementations:**
  - A `FakeUartSpectrumAcquirer` and `FakeAcquirerWrapper` are provided to simulate message reception.
- **Dependency Injection:**
  - The SpectrumManager accepts a UART acquirer instance, allowing tests to inject fake implementations.
- **Constants:**
  - All hard-coded values are centralized in `UartMessageStruct` for consistency between production code and tests.

## Additional Notes

- The tests simulate full UART messages based on the defined structure.
- Inversion tests verify that the maximum value is correctly calculated from the actual data set.
- Any modifications in message format should be reflected in both the production code and tests.

