using System;
using System.IO;
using System.Windows.Forms;

namespace ChainReact
{
    public partial class FrmLoading : Form
    {
        public FrmLoading()
        {
            InitializeComponent();
        }

        private void FrmLoading_Shown(object sender, EventArgs e)
        {
            if (!File.Exists("identity.dat"))
            {
                var frmIdentity = new FrmCreateIdentity();
                frmIdentity.ShowDialog(this);
                if (!File.Exists("identity.dat"))
                {
                    Environment.Exit(-1);
                }
            }
            var game = new MainGame();
            game.Run();
        }

        public void DestroyControls()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(DestroyControls));
            }
            else
            {
                for (var i = Controls.Count - 1; i < 0; i--)
                {
                    var ctl = Controls[i];
                    ctl.Dispose();
                }
                Controls.Clear();
            }
        }
    }
}
