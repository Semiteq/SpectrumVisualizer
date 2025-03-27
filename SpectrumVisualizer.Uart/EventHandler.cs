using System.Diagnostics;

namespace SpectrumVisualizer.Uart
{
    /// <summary>
    /// Handles and logs errors within the application. Provides a centralized point for error reporting,
    /// logging to debug output and optionally to a UI ListBox.
    /// </summary>
    internal static class EventHandler
    {
        public static ListBox? LoggingBox { get; set; }
        private const int MaxLogCount = 100;

        /// <summary>
        /// Logs an exception message. Writes to Debug output and optionally to a UI ListBox.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="onlyDebug">If true, logs only to Debug output.</param>
        public static void Log(Exception ex, bool onlyDebug = false) => Log($"Error: {ex}", onlyDebug);

        /// <summary>
        /// Logs a string message. Writes to Debug output and optionally to a UI ListBox.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="onlyDebug">If true, logs only to Debug output.</param>
        public static void Log(string message, bool onlyDebug = false)
        {
            var timestampedMessage = $"{DateTime.Now:T} - {message}";
            Debug.WriteLine(timestampedMessage);

            if (onlyDebug || LoggingBox == null) return;

            LoggingBox.Invoke((MethodInvoker)(() =>
            {
                if (LoggingBox!.Items.Count >= MaxLogCount)
                {
                    LoggingBox.Items.RemoveAt(0);
                }
                LoggingBox.Items.Add(timestampedMessage);
                LoggingBox.TopIndex = LoggingBox.Items.Count - 1; // Scroll to latest message
            }));
        }
    }
}
