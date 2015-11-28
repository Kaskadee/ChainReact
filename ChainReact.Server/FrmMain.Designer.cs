namespace ChainReact.Server
{
    partial class FrmMain
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdStart = new System.Windows.Forms.Button();
            this.lblConnectedPlayersInfo = new System.Windows.Forms.Label();
            this.lblPlayersCount = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // cmdStart
            // 
            this.cmdStart.Location = new System.Drawing.Point(12, 12);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(450, 23);
            this.cmdStart.TabIndex = 0;
            this.cmdStart.Text = "Start Server";
            this.cmdStart.UseVisualStyleBackColor = true;
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // lblConnectedPlayersInfo
            // 
            this.lblConnectedPlayersInfo.AutoSize = true;
            this.lblConnectedPlayersInfo.Location = new System.Drawing.Point(12, 52);
            this.lblConnectedPlayersInfo.Name = "lblConnectedPlayersInfo";
            this.lblConnectedPlayersInfo.Size = new System.Drawing.Size(102, 13);
            this.lblConnectedPlayersInfo.TabIndex = 1;
            this.lblConnectedPlayersInfo.Text = "Connected Players: ";
            // 
            // lblPlayersCount
            // 
            this.lblPlayersCount.AutoSize = true;
            this.lblPlayersCount.Location = new System.Drawing.Point(120, 52);
            this.lblPlayersCount.Name = "lblPlayersCount";
            this.lblPlayersCount.Size = new System.Drawing.Size(13, 13);
            this.lblPlayersCount.TabIndex = 2;
            this.lblPlayersCount.Text = "0";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(15, 68);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(447, 141);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 221);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.lblPlayersCount);
            this.Controls.Add(this.lblConnectedPlayersInfo);
            this.Controls.Add(this.cmdStart);
            this.Name = "FrmMain";
            this.Text = "ChainReact - Development Servertest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.Label lblConnectedPlayersInfo;
        private System.Windows.Forms.Label lblPlayersCount;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

