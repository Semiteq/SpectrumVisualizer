namespace SpectrumVisualizer
{
    /// <summary>
    /// The main entry point for the Spectrum Visualizer application.
    /// Sets up application settings and runs the main form.
    /// </summary>
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread] // Specifies that the COM threading model for this application is single-threaded apartment (STA). Required for Windows Forms.
        static void Main()
        {
            Application.EnableVisualStyles();          // Enables visual styles for the application to use the current Windows theme.
            Application.SetCompatibleTextRenderingDefault(false); // Sets whether to use compatible text rendering engine for controls that support text rendering using GDI. False is generally recommended for better performance and visual appearance.
            Application.Run(new MainForm());           // Creates and runs the main application form, starting the application's message loop.
        }
    }
}