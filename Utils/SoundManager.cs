﻿using nightreign_auto_storm_timer.Services;
using System.Diagnostics;
using System.IO;
using System.Media;

namespace nightreign_auto_storm_timer.Utils
{
    public static class SoundManager
    {
        private static readonly SoundPlayer startPlayer = LoadSound(Asset.StartWav);
        private static readonly SoundPlayer stopPlayer = LoadSound(Asset.StopWav);

        private static SoundPlayer LoadSound(Asset asset)
        {
            var stream = AssetLoader.LoadAssetStream(asset);
            if (stream != null)
                return new SoundPlayer(stream);

            LogService.Warning($"[SoundManager] Sound asset not found {asset}");
            return new SoundPlayer();
        }

        public static void PlayStart()
        {
            startPlayer.Play();
        }

        public static void PlayStop()
        {
            stopPlayer.Play();
        }
    }

}
