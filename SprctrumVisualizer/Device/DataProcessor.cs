namespace SpectrumVisualizer.Device;
class DataProcessor : IDisposable
{
    private volatile bool _isProcessing = false;
    private volatile bool _hasNewData = false;
    private readonly object _lock = new();
    private readonly CancellationTokenSource _cts = new();
    private Task? _processingTask;

    public void OnDataUpdated(Func<Task> processMethod)
    {
        lock (_lock)
        {
            if (_cts.IsCancellationRequested) return;

            _hasNewData = true;
            if (!_isProcessing)
            {
                _isProcessing = true;
                _processingTask = Task.Run(async () =>
                {
                    try
                    {
                        await ProcessLoop(processMethod, _cts.Token);
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Log(ex);
                    }
                });
            }
        }
    }


    private async Task ProcessLoop(Func<Task> processMethod, CancellationToken token)
    {
        try
        {
            do
            {
                _hasNewData = false;
                await processMethod();

                if (token.IsCancellationRequested)
                    break;
            }
            while (_hasNewData);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Processing cancelled.");
        }
        finally
        {
            lock (_lock)
            {
                _isProcessing = false;
            }
        }
    }

    public void Dispose()
    {
        try 
        {
            _cts.Cancel(); 

            if (_processingTask != null)
            {
                 _processingTask.ConfigureAwait(false).GetAwaiter().GetResult();
            }

            _cts.Dispose();
        }
        catch (ObjectDisposedException) 
        {
            // Игнорируем ошибку при утилизации ресурсов
        }
    }
}