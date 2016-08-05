using System;
using ChainReact.Core.Utilities;
using Newtonsoft.Json;
using System.IO;

namespace ChainReact.Core.Client
{
	public class ClientIdentity : JsonClassSerializer
	{
		[JsonProperty("id")]
		public string Id {get; set;}
		[JsonProperty("username")]
		public string Name {get; set;}

		public ClientIdentity(string id, string name) {
			Id = id;
			Name = name;
		}

		public static ClientIdentity GetDefaultIdentity() {
			var username = Environment.UserName;
			var id = Guid.NewGuid ().ToString ();
			return new ClientIdentity (id, username);
		}
	}
}

