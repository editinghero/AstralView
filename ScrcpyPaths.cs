namespace AstralView;

/// <summary>
/// Provides paths to bundled scrcpy and adb executables.
/// </summary>
public static class ScrcpyPaths
{
    private static readonly string _toolsDir = System.IO.Path.Combine(
        System.AppContext.BaseDirectory, "Tools");

    public static string ScrcpyPath => System.IO.Path.Combine(_toolsDir, "scrcpy.exe");
    public static string AdbPath    => System.IO.Path.Combine(_toolsDir, "adb.exe");
}
