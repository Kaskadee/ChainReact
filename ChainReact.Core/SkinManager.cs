using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChainReact.Core
{
    public class SkinManager
    {
        public List<Skin> Skins { get; }

        private readonly Dictionary<string, string> _defaultResources = new Dictionary<string, string>(); 

        private static SkinManager _instance;
        public static SkinManager Instance => _instance ?? (_instance = new SkinManager());

        private SkinManager()
        {
            Skins = new List<Skin>();
            LoadAvailableSkins();
        }

        public void AddDefaultResource(string name, string file)
        {
            _defaultResources.Add(file, name);
        }

        private void LoadAvailableSkins()
        {
            if (!Directory.Exists("skins") || !Directory.EnumerateDirectories("skins").Any()) throw new DirectoryNotFoundException("No available skin was found!");
            foreach (var skin in Directory.EnumerateDirectories("skins").Select(skinDir => Directory.EnumerateFiles(skinDir).Where(fileName => _defaultResources.ContainsKey(Path.GetFileName(fileName))).ToDictionary(fileName => fileName, fileName => _defaultResources[Path.GetFileName(fileName)])).Select(skinFiles => new Skin
            {
                Files = skinFiles
            }))
            {
                Skins.Add(skin);
            }
        }
    }

    public class Skin
    {
        public Dictionary<string, string> Files { get; set; }

        public void Load()
        {
            ResourceManager.Instance.UnloadAll();
            
        }
    }
}
