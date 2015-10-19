using System;
using System.Windows.Forms;

namespace ChainReact
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;
            var game = new MainGame();
            game.Run();
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (e.Exception.Message.Contains("OpenGL") && e.Exception.GetType() == typeof(MissingMethodException))
            {
                MessageBox.Show(@"Ein Fehler beim erstellen des OpenGL 3.3 Contexts ist aufgetreten!" +
                                Environment.NewLine +
                                @"Sollte der Fehler nach einen Neustart des Programmes weiterhin bestehen versuchen sie den Grafikkartentreiber zu aktualisieren!" 
                                + Environment.NewLine +
                                @"Weitere Informationen: " +
                                Environment.NewLine +
                                @"Fehlernachricht: " +
                                e.Exception.Message +
                                Environment.NewLine +
                                @"Auslöser: " +
                                e.Exception.Source +
                                @"Fehlerstapel: " +
                                e.Exception.StackTrace)
                ;
            }
        }
    }
}
