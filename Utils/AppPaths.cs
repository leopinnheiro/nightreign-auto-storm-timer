using System.IO;

namespace nightreign_auto_storm_timer.Utils;

public static class AppPaths
{
    public static string TempDirectory { get; } = Path.Combine(Path.GetTempPath(), "NRAST");
    public static string TessdataDirectory { get; } = Path.Combine(TempDirectory, "tessdata");

    public static void EnsureDirectoriesExist()
    {
        Directory.CreateDirectory(TempDirectory);
        Directory.CreateDirectory(TessdataDirectory);
    }
}
