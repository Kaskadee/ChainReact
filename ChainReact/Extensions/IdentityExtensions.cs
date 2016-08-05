using System;
using ChainReact.Core.Client;
using System.IO;
using System.Windows.Forms;

namespace ChainReact.Extensions
{
	public static class IdentityExtensions
	{
		public static ClientIdentity LoadIdentity(string path) {
			if (!File.Exists (path)) {
				var frmCreateIdentiy = new FrmCreateIdentity ();
				frmCreateIdentiy.ShowDialog ();

				if (!File.Exists (path)) {
					MessageBox.Show ("ERROR: No identity were created.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					Environment.Exit (-1);
				}
			}
			try {
				using(var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
					using (var sr = new StreamReader (fs)) {
						var json = sr.ReadToEnd ();
						return ClientIdentity.Deserialize<ClientIdentity> (json);
					}
				}
			} catch(UnauthorizedAccessException ex) {
				MessageBox.Show ("ERROR: Access denied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Environment.Exit (-1);
			}
			return null;
		}
	}
}

