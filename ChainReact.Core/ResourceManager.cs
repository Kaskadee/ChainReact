using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sharpex2D.Framework.Audio;
using Sharpex2D.Framework.Audio.WaveOut;
using Sharpex2D.Framework.Content;

namespace ChainReact.Core
{
    public static class ResourceManager
    {
        public static SoundManager SoundManager { get; set; }
        public static bool SoundAvailable => SoundManager != null && SoundManager.IsSupported && DeviceCount > 0;
        public static int DeviceCount => ((WaveOutSoundManager) SoundManager).GetDeviceCount();
        public static string LastSoundError { get; set; }

        private static readonly Dictionary<string, IContent> _importedResources = new Dictionary<string, IContent>();

        public static void ImportResource<T>(string name, T resource) where T : IContent
        {
            if (_importedResources.ContainsKey(name))
                return;

            _importedResources.Add(name, resource);
        }

        public static void LoadResource<T>(Sharpex2D.Framework.Game game, string name, string path) where T : IContent
        {
            if (_importedResources.ContainsKey(name))
                return;

            var resource = game.Content.Load<T>(path);
            _importedResources.Add(name, resource);
        }

        public static T GetResource<T>(string name) where T : IContent
        {
            IContent resource;
            if (_importedResources.TryGetValue(name, out resource))
            {
                return (T)resource;
            }
            throw new ContentLoadException($"Asset {name} couldn't be found! Maybe it isn't loaded?");
        }

        public static T TryGetResource<T>(string name) where T : IContent
        {
            IContent resource;
            if (_importedResources.TryGetValue(name, out resource))
            {
                return (T) resource;
            }
            return default(T);
        }

        public static void UnloadResource(string name)
        {
            _importedResources.Remove(name);
        }

        public static void UnloadAll()
        {
            _importedResources.Clear();
        }
    }
}
