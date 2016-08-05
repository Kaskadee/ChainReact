using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ChainReact.Core.Client;

namespace ChainReact
{
    public partial class FrmCreateIdentity : Form
    {
        private Guid _uuid;

        public FrmCreateIdentity()
        {
            InitializeComponent();
        }

        private void FrmCreateIdentity_Shown(object sender, EventArgs e)
        {
            _uuid = Guid.NewGuid();
            txbUuid.Text = _uuid.ToString();
        }

        private void cmdGenerateUuid_Click(object sender, EventArgs e)
        {
            _uuid = Guid.NewGuid();
            txbUuid.Text = _uuid.ToString();
        }

        private void cmdConfirm_Click(object sender, EventArgs e)
        {
            if (_uuid != Guid.Empty && !string.IsNullOrEmpty(txbUsername.Text))
            {
				var identity = new ClientIdentity (_uuid.ToString (), txbUsername.Text);
                var json = identity.Serialize();
                using (var fs = new FileStream("identity.dat", FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.Write(json);
                    }
                }
                Guid.Parse(_uuid.ToString());
                
                Close();
                return;
            }
            MessageBox.Show(@"UUID or Username is empty", @"Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }
}
