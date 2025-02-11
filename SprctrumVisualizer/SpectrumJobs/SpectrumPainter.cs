using OxyPlot.Series;
using OxyPlot.WindowsForms;
using OxyPlot;

namespace SpectrumVisualizer.SpectrumJobs
{
    internal static class SpectrumPainter
    {
        public static void Process(Dictionary<double, double> spectrum, PlotView plotView)
        {
            var model = new PlotModel { Title = "Спектр" };
            var series = new LineSeries();

            foreach (var key in spectrum.Keys)
            {
                series.Points.Add(new DataPoint(key, spectrum[key]));
            }

            model.Series.Add(series);
            plotView.Model = model;
        }
    }
}
