using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ChainReact.Core
{
	public static class ResourceManager
	{
		public static bool SoundAvailable => true;

		private static Dictionary<string, object> _importedContent = new Dictionary<string, object>();

		public static void ImportResource(string name, object resource) 
		{
			if (_importedContent.ContainsKey (name))
				return;
			_importedContent.Add (name, resource);
		}

		public static void LoadResource<T>(ContentManager content, string name, string path) {
			if (_importedContent.ContainsKey (name))
				return;
			var resource = content.Load<T> (path);
			_importedContent.Add (name, resource);
		}

		public static void LoadTexture(GraphicsDevice device, string name, string path) {
			if (_importedContent.ContainsKey (name))
				return;
			using(var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				var tex = Texture2D.FromStream (device, fs);
				_importedContent.Add (name, tex);
			}

		}

		public static T GetResource<T>(string name) {
			object resource;
			if (_importedContent.TryGetValue (name, out resource)) {
				if (resource is T) {
					return (T)resource;
				}
				throw new ContentLoadException ($"Couldn't get resource {name}: Wrong type definied");
			}
			throw new ContentLoadException ($"Couldn't get resource {name}. Maybe it isnt't loaded?");
		}

		public static T TryGetResource<T>(string name) {
			object resource;
			if (_importedContent.TryGetValue (name, out resource)) {
				if (resource is T) {
					return (T)resource;
				}
			}
			return default(T);
		}

		public static void UnloadResource(string name) {
			if (!_importedContent.ContainsKey (name))
				return;
			var resourceObject = _importedContent [name];
			var resource = resourceObject as IDisposable;
			if (resource != null)
				resource.Dispose ();
			_importedContent.Remove (name);
		}

		public static void UnloadAll() {
			foreach (var obj in _importedContent.Values) {
				var resource = obj as IDisposable;
				if (resource != null)
					resource.Dispose ();
			}
			_importedContent.Clear ();
		}
	}
}
