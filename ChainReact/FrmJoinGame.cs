using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ChainReact.Core;

namespace ChainReact
{
    public partial class FrmJoinGame : Form
    {
        public FrmJoinGame()
        {
            InitializeComponent();
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            IPAddress address;
            if (IPAddress.TryParse(txbAddress.Text, out address))
            {
                GameSettings.Instance.Address = address;
                Close();
            }
        }
    }
}
