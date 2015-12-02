using System;
using System.Collections.Generic;
using System.IO;
using Sharpex2D.Framework.Audio;
using Sharpex2D.Framework.Audio.WaveOut;
using Sharpex2D.Framework.Content;

namespace ChainReact.Core
{
    public class ResourceManager
    {
        public SoundManager SoundManager { get; set; }
        public bool SoundAvailable => SoundManager != null && SoundManager.IsSupported && DeviceCount > 0;
        public int DeviceCount => ((WaveOutSoundManager) SoundManager).GetDeviceCount();
        public string LastSoundError { get; set; }

        private static ResourceManager _instance;
        public static ResourceManager Instance => _instance ?? (_instance = new ResourceManager());

        private readonly Dictionary<string, IContent> _importedResources; 

        private ResourceManager()
        {
            _importedResources = new Dictionary<string, IContent>();
        }

        public void ImportResource<T>(string name, T resource) where T : IContent
        {
            _importedResources.Add(name, resource);
        }

        public void LoadResource<T>(Sharpex2D.Framework.Game game, string name, string path) where T : IContent
        {
            var resource = game.Content.Load<T>(path);
             _importedResources.Add(name, resource);
        }

        public T GetResource<T>(string name) where T : IContent
        {
            IContent resource;
            if (_importedResources.TryGetValue(name, out resource))
            {
                return (T)resource;
            }
            throw new ContentLoadException($"Asset {name} couldn't be found! Maybe it isn't loaded?");
        }

        [Obsolete]
        public IContent GetResource(string name)
        {
            IContent resource;
            if (_importedResources.TryGetValue(name, out resource))
            {
                return resource;
            }
            throw new ContentLoadException($"Asset {name} couldn't be found! Maybe it isn't loaded?");
        }

        public T TryGetResource<T>(string name) where T : IContent
        {
            IContent resource;
            if (_importedResources.TryGetValue(name, out resource))
            {
                return (T) resource;
            }
            return default(T);
        }

        [Obsolete]
        public IContent TryGetResource(string name)
        {
            IContent resource;
            return _importedResources.TryGetValue(name, out resource) ? resource : null;
        }

        public void RemoveResource(string name)
        {
            _importedResources.Remove(name);
        }

        public void ClearAllResources()
        {
            _importedResources.Clear();
        }
    }

    public class AssetInformations
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Type Type { get; set; }
    }
}
