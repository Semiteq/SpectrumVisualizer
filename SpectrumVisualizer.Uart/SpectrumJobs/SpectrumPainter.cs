﻿using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SpectrumVisualizer.Uart.SpectrumJobs
{
    /// <summary>
    /// Manages spectrum visualization using OxyPlot.
    /// </summary>
    public class SpectrumPainter
    {
        private const string SpectrumDefaultTitle = "Spectrum";
        private const string SpectrumWavelengthAxisTitle = "Wavelength [nm]";
        private const string SpectrumIntensityAxisTitle = "Intensity";
        private const string SpectrumLogIntensityAxisTitle = "Intensity (Log)";
        
        private readonly PlotModel _plotModel;
        private readonly LineSeries _spectrumSeries;
        private bool _isResetNeeded;
        private bool _isStickToZeroNeeded;

        public SpectrumPainter()
        {
            _plotModel = CreatePlotModel();
            _spectrumSeries = CreateSpectrumSeries();
            
            _plotModel.Series.Add(_spectrumSeries);
            InitializeAxes();
        }

        /// <summary>
        /// Updates the spectrum data and redraws the plot.
        /// </summary>
        public void UpdateData(Dictionary<double, double> data)
        {
            if (data is null) return;

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
            const double xMargin = 0.05;
            const double yMargin = 0.2;

            var xMin = _spectrumSeries.Points.Min(p => p.X);
            var xMax = _spectrumSeries.Points.Max(p => p.X);
            var yMax = _spectrumSeries.Points.Max(p => p.Y);

            foreach (var axis in _plotModel.Axes)
            {
                switch (axis.Position)
                {
                    case AxisPosition.Bottom:
                        axis.Zoom(xMin * (1 - xMargin), xMax * (1 + xMargin));
                        break;
                    case AxisPosition.Left:
                        axis.Zoom(0, yMax * (1 + yMargin));
                        break;
                }
            }
        }
    }
}