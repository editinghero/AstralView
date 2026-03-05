using System.Diagnostics;

namespace AstralView.Core
{
    public class ScrcpyRunner
    {
        private Process? _process;
        private readonly string _scrcpyPath;

        public bool IsRunning => _process != null && !_process.HasExited;

        public ScrcpyRunner(string scrcpyPath)
        {
            _scrcpyPath = scrcpyPath;
        }

        public void Start(string arguments)
        {
            if (IsRunning) return;

            var startInfo = new ProcessStartInfo
            {
                FileName = _scrcpyPath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _process = Process.Start(startInfo);
        }

        public void Stop()
        {
            if (IsRunning)
            {
                _process?.Kill();
            }
        }
    }
}
