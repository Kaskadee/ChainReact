#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;


#endregion

namespace ChainReact
{
	public static class Program
    {
		private static MainGame _game;

		internal static void RunGame ()
		{
			_game = new MainGame ();
			_game.Run ();
			_game.Dispose ();
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
        [STAThread]
        private static void Main (string[] args)
		{
			if (args.Contains ("--debug"))
				Debugger.Launch ();
			RunGame ();
		}
	}
}

