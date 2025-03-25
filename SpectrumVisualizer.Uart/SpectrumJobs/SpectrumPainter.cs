using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    /// <summary>
    /// Manages spectrum visualization using OxyPlot.
    /// </summary>
    public class SpectrumPainter
    {
        private readonly PlotModel _plotModel;
        private readonly LineSeries _spectrumSeries;
        private bool _isResetNeeded;
        private bool _isStickToZeroNeeded;

        public SpectrumPainter()
        {
            _plotModel = new PlotModel { Title = "Spectrum" };
            _spectrumSeries = new LineSeries { MarkerType = MarkerType.None };
            _plotModel.Series.Add(_spectrumSeries);
            _plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Wavelength [nm]" });
            _plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Intensity" });
        }

        /// <summary>
        /// Updates the spectrum data and redraws the plot.
        /// </summary>
        public void UpdateData(Dictionary<double, double> data)
        {
            var sorted = data.OrderBy(pair => pair.Key).ToArray();
            _spectrumSeries.Points.Clear();
            foreach (var pair in sorted)
            {
                _spectrumSeries.Points.Add(new DataPoint(pair.Key, pair.Value));
            }

            if (_isResetNeeded)
            {
                _plotModel.ResetAllAxes();
                _isResetNeeded = false;
            }

            if (_isStickToZeroNeeded && _spectrumSeries.Points.Any())
            {
                double xMin = _spectrumSeries.Points.Min(p => p.X);
                double xMax = _spectrumSeries.Points.Max(p => p.X);
                double yMax = _spectrumSeries.Points.Max(p => p.Y);

                foreach (var axis in _plotModel.Axes)
                {
                    if (axis.Position == AxisPosition.Bottom)
                        axis.Zoom(xMin * 0.95, xMax * 1.05);
                    else if (axis.Position == AxisPosition.Left)
                        axis.Zoom(0, yMax * 1.2);
                }
                _isStickToZeroNeeded = false;
            }

            _plotModel.InvalidatePlot(true);
        }

        public void ResetPlotScale() => _isResetNeeded = true;
        public void StickToZero() => _isStickToZeroNeeded = true;

        public void ToggleLogarithmicYAxis(bool useLogScale)
        {
            _plotModel.Axes.Remove(_plotModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Left));
            _plotModel.Axes.Add(useLogScale
                ? new LogarithmicAxis { Position = AxisPosition.Left, Title = "Intensity (Log)", Base = 10 }
                : new LinearAxis { Position = AxisPosition.Left, Title = "Intensity" });
            _plotModel.InvalidatePlot(true);
        }

        public PlotModel GetPlotModel() => _plotModel;
    }
}
