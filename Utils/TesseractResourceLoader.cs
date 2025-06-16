using System.IO;
using Tesseract;

namespace nightreign_auto_storm_timer.Utils;

public static class TesseractResourceLoader
{
    private static readonly HashSet<string> _loadedLanguages = [];

    public static TesseractEngine LoadEngineFromStream(Stream stream, string languageCode)
    {
        string trainedDataPath = Path.Combine(AppPaths.TessdataDirectory, $"{languageCode}.traineddata");

        if (!_loadedLanguages.Contains(languageCode))
        {
            if (!File.Exists(trainedDataPath))
            {
                using var fileStream = new FileStream(trainedDataPath, FileMode.Create, FileAccess.Write);
                stream.CopyTo(fileStream);
            }

            _loadedLanguages.Add(languageCode);
        }

        return new TesseractEngine(AppPaths.TessdataDirectory, languageCode, EngineMode.Default);
    }
}