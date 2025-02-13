using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Annotations;
using System.Collections.Generic;
using System.Linq;

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

    // Конструктор без данных – данные и маркеры будут установлены при UpdateData
    public SpectrumPainter()
    {
        _plotModel = new PlotModel { Title = "Spectrum" };
        _spectrumSeries = new LineSeries { MarkerType = MarkerType.None };
        _plotModel.Series.Add(_spectrumSeries);

        // Маркеры и данные будут созданы при поступлении первых данных
        _leftMarker = null;
        _rightMarker = null;

        _plotModel.MouseDown += OnMouseDown;
        _plotModel.MouseMove += OnMouseMove;
        _plotModel.MouseUp += OnMouseUp;
    }

    // Метод обновления данных принимает Dictionary<double,double>
    public void UpdateData(Dictionary<double, double> data)
    {
        // Сортируем данные по ключу (X)
        var sorted = data.OrderBy(pair => pair.Key).ToArray();
        _xData = sorted.Select(pair => pair.Key).ToArray();
        _yData = sorted.Select(pair => pair.Value).ToArray();

        _spectrumSeries.Points.Clear();
        foreach (var pair in sorted)
        {
            _spectrumSeries.Points.Add(new DataPoint(pair.Key, pair.Value));
        }

        // Отложенная инициализация маркеров при поступлении первых данных
        if (_xData.Length > 0)
        {
            if (_leftMarker == null || _rightMarker == null)
            {
                _leftMarker = new LineAnnotation
                {
                    X = _xData[0],
                    Color = OxyColors.Red,
                    Type = LineAnnotationType.Vertical
                };
                _rightMarker = new LineAnnotation
                {
                    X = _xData[^1],
                    Color = OxyColors.Red,
                    Type = LineAnnotationType.Vertical
                };
                _plotModel.Annotations.Add(_leftMarker);
                _plotModel.Annotations.Add(_rightMarker);
            }
            else
            {
                // Если маркеры уже созданы, корректируем их положение, если они вышли за границы новых данных
                if (_leftMarker.X < _xData[0])
                    _leftMarker.X = _xData[0];
                if (_rightMarker.X > _xData[^1])
                    _rightMarker.X = _xData[^1];
            }
        }

        _plotModel.InvalidatePlot(true);
    }

    // Позволяет вручную задать маркеры
    public void AddMarkers(double left, double right)
    {
        if (_leftMarker != null && _rightMarker != null)
        {
            _leftMarker.X = left;
            _rightMarker.X = right;
            _plotModel.InvalidatePlot(true);
        }
    }

    private void OnMouseDown(object sender, OxyMouseDownEventArgs e)
    {
        if (_plotModel.Axes.Count == 0)
            return;

        var xValue = _plotModel.Axes[0].InverseTransform(e.Position.X);
        if (_leftMarker != null && System.Math.Abs(xValue - _leftMarker.X) < 5)
            _isDraggingLeft = true;
        else if (_rightMarker != null && System.Math.Abs(xValue - _rightMarker.X) < 5)
            _isDraggingRight = true;
    }

    private void OnMouseMove(object sender, OxyMouseEventArgs e)
    {
        if (!_isDraggingLeft && !_isDraggingRight)
            return;
        if (_plotModel.Axes.Count == 0)
            return;

        var xValue = _plotModel.Axes[0].InverseTransform(e.Position.X);
        if (_isDraggingLeft && _rightMarker != null && xValue < _rightMarker.X)
        {
            _leftMarker.X = xValue;
        }
        else if (_isDraggingRight && _leftMarker != null && xValue > _leftMarker.X)
        {
            _rightMarker.X = xValue;
        }
        _plotModel.InvalidatePlot(false);
    }

    private void OnMouseUp(object sender, OxyMouseEventArgs e)
    {
        _isDraggingLeft = false;
        _isDraggingRight = false;
    }

    // Вычисляет среднее значение Y между маркерами
    public double CalculateAverage()
    {
        if (_xData == null || _yData == null || _leftMarker == null || _rightMarker == null)
            return 0;

        int start = System.Array.FindIndex(_xData, x => x >= _leftMarker.X);
        int end = System.Array.FindIndex(_xData, x => x >= _rightMarker.X);
        if (start < 0 || end < 0 || start >= end)
            return 0;

        double sum = 0;
        for (int i = start; i < end; i++)
            sum += _yData[i];
        return sum / (end - start);
    }

    public PlotModel GetPlotModel() => _plotModel;
}
