using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SpectrumVisualizer.Uart.SpectrumJobs;

namespace SpectrumVisualizer.Tests.Uart
{
    [TestClass]
    public class SpectrumPainterTests
    {
        [TestMethod]
        public void UpdateData_PopulatesPlotModelWithDataPoints()
        {
            
            var painter = new SpectrumPainter();
            var data = new Dictionary<double, double>
            {
                { 0, 1 },
                { 1, 2 },
                { 2, 3 }
            };

            
            painter.UpdateData(data);
            PlotModel model = painter.GetPlotModel();

            
            var series = model.Series[0] as LineSeries;
            Assert.IsNotNull(series);
            Assert.AreEqual(3, series.Points.Count);
            
            Assert.IsTrue(series.Points.SequenceEqual(series.Points.OrderBy(p => p.X)));
        }

        [TestMethod]
        public void ToggleLogarithmicYAxis_ChangesLeftAxisType()
        {
            // Arrange
            var painter = new SpectrumPainter();
            var modelBefore = painter.GetPlotModel();
    
            painter.ToggleLogarithmicYAxis(true);
            var modelAfter = painter.GetPlotModel();
    
            var leftAxis = modelAfter.Axes.FirstOrDefault(a => a.Position == AxisPosition.Left);
            Assert.IsNotNull(leftAxis);
            Assert.IsInstanceOfType(leftAxis, typeof(OxyPlot.Axes.LogarithmicAxis));
        }

    }
}