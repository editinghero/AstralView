using System.Diagnostics;

namespace AstralView.Core
{
    public class ScrcpyRunner
    {
        private Process? _process;
        private readonly string _scrcpyPath;

        public event EventHandler? Exited;

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
            if (_process != null)
            {
                _process.EnableRaisingEvents = true;
                _process.Exited += (_, _) =>
                {
                    try
                    {
                        _process?.Dispose();
                    }
                    catch { }
                    finally
                    {
                        _process = null;
                    }

                    Exited?.Invoke(this, EventArgs.Empty);
                };
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                _process?.Kill();
            }
        }

        public void ForceStop()
        {
            if (_process != null)
            {
                try
                {
                    _process.Kill(true); // Kill entire process tree
                    _process.WaitForExit(500);
                }
                catch { }
                finally
                {
                    _process?.Dispose();
                    _process = null;
                }
            }
            
            // Also kill any orphaned scrcpy processes
            try
            {
                foreach (var proc in Process.GetProcessesByName("scrcpy"))
                {
                    proc.Kill(true);
                    proc.Dispose();
                }
            }
            catch { }
        }
    }
}
