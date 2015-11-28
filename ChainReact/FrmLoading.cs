﻿using System;
using System.Diagnostics;
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
            Process.GetCurrentProcess().Refresh();
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
