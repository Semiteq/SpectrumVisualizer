using System.Diagnostics;

namespace SpectrumVisualizer
{
    /// <summary>
    /// Handles and logs errors within the application. Provides a centralized point for error reporting,
    /// logging to debug output and optionally to a UI ListBox.
    /// </summary>
    internal static class ErrorHandler
    {
        public static ListBox? LoggingBox { get; set; }
        private const int MaxLogCount = 100;

        /// <summary>
        /// Logs an exception message. Writes to Debug output and optionally to a UI ListBox.
        /// </summary>
        /// <param name="message">The exception to log.</param>
        public static void Log(Exception message)
        {
            Debug.WriteLine(message);
            if (LoggingBox != null)
            {
                LoggingBox.Invoke((MethodInvoker)(() => // Invoke action on UI thread to update ListBox safely.
                {
                    if (LoggingBox.Items.Count >= MaxLogCount)
                    {
                        LoggingBox.Items.RemoveAt(0);
                    }
                    LoggingBox.Items.Add(message);
                    LoggingBox.TopIndex = LoggingBox.Items.Count - 1; // Scroll to the bottom to show the latest message.
                }));
            }
        }

        /// <summary>
        /// Handles an exception, primarily by writing to debug output.
        /// </summary>
        /// <param name="ex">The exception to handle.</param>
        public static void Handle(Exception ex)
        {
            Debug.WriteLine($"Error: {ex}");
        }
    }
}