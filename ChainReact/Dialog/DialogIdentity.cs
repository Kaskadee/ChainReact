using System;

namespace ChainReact
{
	public partial class DialogIdentity : Gtk.Dialog
	{
		private string _id;

		public DialogIdentity ()
		{
			this.Build ();
			_id = Guid.NewGuid ().ToString ();

		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}
	}
}

