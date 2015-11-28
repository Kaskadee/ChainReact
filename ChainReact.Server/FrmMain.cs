using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ChainReact.Core.Game;
using ChainReact.Core.Game.Objects;
using ChainReact.Core.Server;
using Sharpex2D.Framework.Network;
using Color = Sharpex2D.Framework.Rendering.Color;

namespace ChainReact.Server
{
    public partial class FrmMain : Form
    {
        private Core.Server.Server _internalServer;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            _internalServer = new Core.Server.Server(ServerMode.Internal, NetworkPeer.Protocol.Tcp);
        }
    }
}
