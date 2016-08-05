using System;
using Newtonsoft.Json;

namespace ChainReact.Core.Utilities
{
	public abstract class JsonClassSerializer
	{
		public string Serialize() {
			return JsonConvert.SerializeObject (this, Formatting.Indented);
		}

		public static T Deserialize<T>(string json) {
			return JsonConvert.DeserializeObject<T> (json);
		}
	}
}

