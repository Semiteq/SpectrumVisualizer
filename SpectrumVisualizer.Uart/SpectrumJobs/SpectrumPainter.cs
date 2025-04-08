using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using HorizontalAlignment = OxyPlot.HorizontalAlignment;

namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    /// <summary>
    /// Manages spectrum visualization using OxyPlot.
    /// </summary>
    public class SpectrumPainter
    {
        // Magic numbers
        private const double AnnotationMargin = 20.0;     // Margin in pixels for annotation from the PlotArea top-left corner.
        private const double XAxisMarginRatio = 0.05;       // Margin ratio for X axis.
        private const double YAxisMarginRatio = 0.2;        // Margin ratio for Y axis.

        private const string SpectrumDefaultTitle = "Спектр";
        private const string SpectrumWavelengthAxisTitle = "Длина волны [нм]";
        private const string SpectrumIntensityAxisTitle = "Интенсивность";
        private const string SpectrumLogIntensityAxisTitle = "Интенсивность (Log)";
        private const string SpectrumLastUpdateLabelFormat = "Последнее обновление: {0:HH:mm:ss.fff}";

        private readonly PlotModel _plotModel;
        private readonly LineSeries _spectrumSeries;
        private readonly TextAnnotation _lastUpdateAnnotation;
        private DateTime _lastUpdateTime;
        private bool _isResetNeeded;
        private bool _isStickToZeroNeeded;

        public SpectrumPainter()
        {
            _plotModel = CreatePlotModel();

            // Subscribe to plot updates to recalc annotation position relative to PlotArea.
            _plotModel.Updated += (sender, e) => UpdateAnnotationScreenPosition();

            _spectrumSeries = CreateSpectrumSeries();
            _lastUpdateAnnotation = new TextAnnotation
            {
                TextHorizontalAlignment = HorizontalAlignment.Left,
                TextVerticalAlignment = VerticalAlignment.Top,
                // Initial position will be updated in the Updated event.
                TextPosition = new DataPoint(0, 0),
                Text = string.Format(SpectrumLastUpdateLabelFormat, DateTime.Now)
            };

            _plotModel.Series.Add(_spectrumSeries);
            _plotModel.Annotations.Add(_lastUpdateAnnotation);
            InitializeAxes();
        }

        /// <summary>
        /// Updates the spectrum data and redraws the plot.
        /// </summary>
        public void UpdateData(Dictionary<double, double> data)
        {
            if (data is null) return;

            _lastUpdateTime = DateTime.Now;
            _lastUpdateAnnotation.Text = string.Format(SpectrumLastUpdateLabelFormat, _lastUpdateTime);

            UpdateSpectrumPoints(data);

            if (_isResetNeeded)
            {
                _plotModel.ResetAllAxes();
                _isResetNeeded = false;
            }

            if (_isStickToZeroNeeded && _spectrumSeries.Points.Count != 0)
            {
                AdjustAxesRange();
                _isStickToZeroNeeded = false;
            }

            // Invalidate plot will trigger the Updated event and recalc annotation position.
            _plotModel.InvalidatePlot(true);
        }

        public void ResetPlotScale() => _isResetNeeded = true;
        public void StickToZero() => _isStickToZeroNeeded = true;

        public void ToggleLogarithmicYAxis(bool useLogScale)
        {
            var currentAxis = _plotModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Left);
            _plotModel.Axes.Remove(currentAxis);
            _plotModel.Axes.Add(CreateYAxis(useLogScale));
            _plotModel.InvalidatePlot(true);
        }

        public PlotModel GetPlotModel() => _plotModel;

        private static PlotModel CreatePlotModel() =>
            new() { Title = SpectrumDefaultTitle };

        private static LineSeries CreateSpectrumSeries() =>
            new() { MarkerType = MarkerType.None };

        private void InitializeAxes()
        {
            _plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = SpectrumWavelengthAxisTitle
            });

            _plotModel.Axes.Add(CreateYAxis(false));
        }

        private static Axis CreateYAxis(bool isLogarithmic) =>
            isLogarithmic
                ? new LogarithmicAxis
                {
                    Position = AxisPosition.Left,
                    Title = SpectrumLogIntensityAxisTitle,
                    Base = 10
                }
                : new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = SpectrumIntensityAxisTitle
                };

        private void UpdateSpectrumPoints(Dictionary<double, double> data)
        {
            var sorted = data.OrderBy(pair => pair.Key).ToArray();
            _spectrumSeries.Points.Clear();

            foreach (var pair in sorted)
            {
                _spectrumSeries.Points.Add(new DataPoint(pair.Key, pair.Value));
            }
        }

        private void AdjustAxesRange()
        {
            // Using magic numbers extracted to constants
            var xMin = _spectrumSeries.Points.Min(p => p.X);
            var xMax = _spectrumSeries.Points.Max(p => p.X);
            var yMax = _spectrumSeries.Points.Max(p => p.Y);

            foreach (var axis in _plotModel.Axes)
            {
                switch (axis.Position)
                {
                    case AxisPosition.Bottom:
                        axis.Zoom(xMin * (1 - XAxisMarginRatio), xMax * (1 + XAxisMarginRatio));
                        break;
                    case AxisPosition.Left:
                        axis.Zoom(0, yMax * (1 + YAxisMarginRatio));
                        break;
                }
            }
        }

        /// <summary>
        /// Updates annotation position to be fixed relative to the PlotArea.
        /// </summary>
        private void UpdateAnnotationScreenPosition()
        {
            // Get current PlotArea (in screen coordinates)
            var plotArea = _plotModel.PlotArea;
            if (plotArea.Width <= 0 || plotArea.Height <= 0)
            {
                // PlotArea not set yet, skip update.
                return;
            }

            // Fixed margin from the top-left corner (in pixels)
            var desiredScreenPoint = new ScreenPoint(plotArea.Left + AnnotationMargin, plotArea.Top + AnnotationMargin);

            var xAxis = _plotModel.Axes.First(a => a.Position == AxisPosition.Bottom);
            var yAxis = _plotModel.Axes.First(a => a.Position == AxisPosition.Left);

            // Transform screen coordinates to data coordinates using respective axis transformations.
            var dataX = xAxis.InverseTransform(desiredScreenPoint.X);
            var dataY = yAxis.InverseTransform(desiredScreenPoint.Y);

            _lastUpdateAnnotation.TextPosition = new DataPoint(dataX, dataY);
        }
    }
}
