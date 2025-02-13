using Device.ATR.Model.Spectrometer;
using SpectrumVisualizer.SpectrumJobs;
using SpectrumVisualizer;

internal class SpectrumManager
{
    private readonly SpectrumAcquirer _spectrumAcquirer;
    private readonly SpectrumAnalyzer _spectrumAnalyzer;
    private AcquireParameter _parameters = new();
    private bool _considerDark = false;
    private CancellationTokenSource? _cts;

    public SpectrumManager(SpectrumAcquirer acquirer, SpectrumAnalyzer analyzer)
    {
        _spectrumAcquirer = acquirer;
        _spectrumAnalyzer = analyzer;
    }

    public void UpdateParameters(int integrationTime, int interval, int average)
    {
        _parameters.IntegrationTime = integrationTime;
        _parameters.Interval = interval;
        _parameters.Average = average;
    }

    public void SetConsiderDark(bool considerDark)
    {
        _considerDark = considerDark;
    }

    public void StartAcquisition(Action<Dictionary<double, double>> updateChart)
    {
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var spectrum = await _spectrumAcquirer.AcquireAsync(_parameters);
                    var dark = await _spectrumAcquirer.AcquireAsync(_parameters, true);

                    var analyzed = await _spectrumAnalyzer.ProcessAsync(
                        new Spectrum { Data = spectrum, Dark = dark },
                        _considerDark
                    );

                    updateChart(analyzed);
                }
                catch (Exception ex)
                {
                    ErrorHandler.Handle(ex);
                }

                await Task.Delay(100, token);
            }
        }, token);
    }

    public void StopAcquisition()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }
}
