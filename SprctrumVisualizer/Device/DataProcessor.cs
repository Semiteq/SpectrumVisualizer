namespace SpectrumVisualizer.Device
{
    /// <summary>
    /// Processes data updates asynchronously, ensuring that processing happens only when new data is available
    /// and avoiding overlapping processing tasks.
    /// </summary>
    class DataProcessor : IDisposable
    {
        private volatile bool _isProcessing = false;
        private volatile bool _hasNewData = false;  
        private readonly object _lock = new();      
        private readonly CancellationTokenSource _cts = new();
        private Task? _processingTask;

        /// <summary>
        /// Called when new data is available. Starts or resumes the processing loop if necessary.
        /// </summary>
        /// <param name="processMethod">The asynchronous method to process the data.</param>
        public void OnDataUpdated(Func<Task> processMethod)
        {
            lock (_lock) // Lock to ensure thread-safe operations on state variables.
            {
                if (_cts.IsCancellationRequested) return;

                _hasNewData = true; // Indicate that new data is available.
                if (!_isProcessing) // Start processing only if it's not already running.
                {
                    _isProcessing = true; 
                    _processingTask = Task.Run(async () => // Start a new task to run the processing loop.
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


        /// <summary>
        /// The main processing loop that executes the data processing method while new data is available.
        /// </summary>
        /// <param name="processMethod">The asynchronous method to process the data.</param>
        /// <param name="token">Cancellation token to stop the processing loop.</param>
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
                while (_hasNewData); // Continue processing as long as new data is available.
            }
            catch (OperationCanceledException)
            {
                // Ignore error
            }
            finally
            {
                lock (_lock)
                {
                    _isProcessing = false; // Reset the processing flag when loop finishes or is cancelled.
                }
            }
        }

        /// <summary>
        /// Disposes of the DataProcessor, cancelling any ongoing processing and releasing resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                _cts.Cancel(); // Request cancellation of the processing task.

                if (_processingTask != null)
                {
                    _processingTask.ConfigureAwait(false).GetAwaiter().GetResult(); // Wait for the processing task to complete.
                }

                _cts.Dispose(); // Dispose of the cancellation token source.
            }
            catch (ObjectDisposedException)
            {
                // Ignore error during disposal if resources are already disposed
            }
        }
    }
}