using System.Diagnostics;

namespace SpectrumVisualizer
{
    internal static class ErrorHandler
    {
        public static ListBox? LoggingBox { get; set; }
        private const int MaxLogCount = 100; // Максимальное количество записей

        public static void Log(Exception message)
        {
            Debug.WriteLine(message);
            if (LoggingBox != null)
            {
                LoggingBox.Invoke((MethodInvoker)(() =>
                {
                    if (LoggingBox.Items.Count >= MaxLogCount)
                    {
                        LoggingBox.Items.RemoveAt(0); // Удаляем самую старую запись
                    }
                    LoggingBox.Items.Add(message);                    
                    LoggingBox.TopIndex = LoggingBox.Items.Count - 1; // Автопрокрутка вниз
                }));
            }
        }

        public static void Handle (Exception ex)
        {
            Debug.WriteLine($"Ошибка: {ex}");
        }
    }
}
