using System.Diagnostics;

namespace AstralView.Core
{
    /// <summary>
    /// Launches scrcpy.exe with the provided CLI arguments.
    /// </summary>
    public class ScrcpyRunner
    {
        private readonly string _scrcpyPath;
        private Process? _currentProcess;

        public ScrcpyRunner(string scrcpyPath = "scrcpy")
        {
            _scrcpyPath = scrcpyPath;
        }

        public bool IsRunning => _currentProcess != null && !_currentProcess.HasExited;

        /// <summary>
        /// Starts scrcpy with the specified arguments.
        /// </summary>
        public void Start(string arguments)
        {
            if (IsRunning) return;

            var psi = new ProcessStartInfo(_scrcpyPath, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = false
            };

            _currentProcess = Process.Start(psi);
        }

        /// <summary>
        /// Stops the currently running scrcpy process.
        /// </summary>
        public void Stop()
        {
            if (_currentProcess != null && !_currentProcess.HasExited)
            {
                _currentProcess.Kill();
                _currentProcess = null;
            }
        }
    }
}
