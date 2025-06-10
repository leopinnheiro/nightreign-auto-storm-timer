using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace nightreign_auto_storm_timer.Utils
{
    public enum Asset
    {
        StartWav,
        StopWav
    }

    public static class AssetLoader
    {
        private static string ResourcePrefix = "";

        static AssetLoader()
        {
            string? fullNamespace = typeof(AssetLoader).Namespace;

            string baseNamespace = fullNamespace?.Split(".")[0] ?? "nightreign_auto_storm_timer";

            ResourcePrefix = $"{baseNamespace}.Assets";
        }

        public static Stream? LoadAssetStream(Asset asset)
        {
            string fileName = asset switch
            {
                Asset.StartWav => "start.wav",
                Asset.StopWav => "stop.wav",
                _ => throw new ArgumentOutOfRangeException(nameof(asset), asset, "Asset desconhecido")
            };

            string resourcePath = $"{ResourcePrefix}.{fileName}";

            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resourcePath);

            if (stream == null)
                Console.WriteLine($"[Warning] Asset {asset} not found. Resource: {resourcePath}");

            return stream;
        }
    }

}
