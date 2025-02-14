using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Annotations;
using SpectrumVisualizer.SpectrumJobs;

namespace SpectrumVisualizer
{
    /// <summary>
    /// Manages spectrum visualization using OxyPlot, including data updates, marker handling, scaling, and averaging.
    /// </summary>
    internal class SpectrumPainter
    {
        private readonly PlotModel _plotModel;
        private LineSeries _spectrumSeries;
        private double[] _xData;
        private double[] _yData;
        private LineAnnotation _leftMarker;
        private LineAnnotation _rightMarker;
        private bool _isDraggingLeft;
        private bool _isDraggingRight;
        private bool _isResetNeeded;
        private bool _isStickToZeroNeeded;

        /// <summary>
        /// Initializes the spectrum plot with default axes, series, and event handlers.
        /// </summary>
        public SpectrumPainter()
        {
            _plotModel = new PlotModel { Title = "Spectrum" };
            _spectrumSeries = new LineSeries { MarkerType = MarkerType.None };
            _plotModel.Series.Add(_spectrumSeries);
            _plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Wavelength [nm]" });
            _plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Intensity" });
            _plotModel.MouseDown += OnMouseDown;
            _plotModel.MouseMove += OnMouseMove;
            _plotModel.MouseUp += OnMouseUp;
        }

        /// <summary>
        /// Updates the spectrum data and redraws the plot.
        /// </summary>
        public void UpdateData(Dictionary<double, double> data)
        {
            var sorted = data.OrderBy(pair => pair.Key).ToArray();
            _xData = sorted.Select(pair => pair.Key).ToArray();
            _yData = sorted.Select(pair => pair.Value).ToArray();

            _spectrumSeries.Points.Clear();
            foreach (var pair in sorted)
                _spectrumSeries.Points.Add(new DataPoint(pair.Key, pair.Value));

            // Initialize or update markers for data range selection
            if (_xData.Length > 0)
            {
                if (_leftMarker == null || _rightMarker == null)
                {
                    _leftMarker = new LineAnnotation { X = _xData[0], Color = OxyColors.Red, Type = LineAnnotationType.Vertical };
                    _rightMarker = new LineAnnotation { X = _xData[^1], Color = OxyColors.Red, Type = LineAnnotationType.Vertical };
                    _plotModel.Annotations.Add(_leftMarker);
                    _plotModel.Annotations.Add(_rightMarker);
                }
                else
                {
                    if (_leftMarker.X < _xData[0]) _leftMarker.X = _xData[0];
                    if (_rightMarker.X > _xData[^1]) _rightMarker.X = _xData[^1];
                }
            }

            // Reset axes if needed
            if (_isResetNeeded)
            {
                _plotModel.ResetAllAxes();
                _isResetNeeded = false;
            }

            // Adjust axes to stick to zero if required
            if (_isStickToZeroNeeded)
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

        /// <summary>
        /// Flags the plot for scale reset on the next update.
        /// </summary>
        public void ResetPlotScale() => _isResetNeeded = true;

        /// <summary>
        /// Flags the plot to adjust its Y-axis to start from zero.
        /// </summary>
        public void StickToZero() => _isStickToZeroNeeded = true;

        /// <summary>
        /// Toggles between logarithmic and linear scale for the Y-axis.
        /// </summary>
        public void ToggleLogarithmicYAxis(bool useLogScale)
        {
            _plotModel.Axes.Remove(_plotModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Left));
            _plotModel.Axes.Add(useLogScale
                ? new LogarithmicAxis { Position = AxisPosition.Left, Title = "Intensity (Log)", Base = 10 }
                : new LinearAxis { Position = AxisPosition.Left, Title = "Intensity" });
            _plotModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Handles mouse down events for moving markers.
        /// </summary>
        private void OnMouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (_plotModel.Axes.Count == 0) return;
            var xValue = _plotModel.Axes[0].InverseTransform(e.Position.X);
            if (_leftMarker != null && System.Math.Abs(xValue - _leftMarker.X) < 5)
                _isDraggingLeft = true;
            else if (_rightMarker != null && System.Math.Abs(xValue - _rightMarker.X) < 5)
                _isDraggingRight = true;
        }

        /// <summary>
        /// Handles mouse move events for dragging markers.
        /// </summary>
        private void OnMouseMove(object sender, OxyMouseEventArgs e)
        {
            if (!_isDraggingLeft && !_isDraggingRight) return;
            if (_plotModel.Axes.Count == 0) return;
            var xValue = _plotModel.Axes[0].InverseTransform(e.Position.X);
            if (_isDraggingLeft && _rightMarker != null && xValue < _rightMarker.X)
                _leftMarker.X = xValue;
            else if (_isDraggingRight && _leftMarker != null && xValue > _leftMarker.X)
                _rightMarker.X = xValue;
            _plotModel.InvalidatePlot(false);
        }

        /// <summary>
        /// Handles mouse up events to stop dragging markers.
        /// </summary>
        private void OnMouseUp(object sender, OxyMouseEventArgs e)
        {
            _isDraggingLeft = false;
            _isDraggingRight = false;
        }

        /// <summary>
        /// Calculates the average intensity in the selected marker range.
        /// </summary>
        public double CalculateAverage()
            => SpectrumCalculator.CalculateAverage(_xData, _yData, _leftMarker, _rightMarker);

        /// <summary>
        /// Calculates the signal-to-noise ratio (SNR) in the selected marker range.
        /// </summary>
        public double CalculateSNR()
            => SpectrumCalculator.CalculateSNR(_xData, _yData, _leftMarker, _rightMarker);

        /// <summary>
        /// Calculates the quality factor (Q-factor) in the selected marker range.
        /// </summary>
        public double CalculateQFactor()
            => SpectrumCalculator.CalculateQFactor(_xData, _yData, _leftMarker, _rightMarker);

        /// <summary>
        /// Returns the current PlotModel for rendering.
        /// </summary>
        public PlotModel GetPlotModel() => _plotModel;
    }
}
