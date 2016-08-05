namespace ChainReact
{
    partial class FrmCreateIdentity
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblUuid = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txbUsername = new System.Windows.Forms.TextBox();
            this.txbUuid = new System.Windows.Forms.TextBox();
            this.cmdConfirm = new System.Windows.Forms.Button();
            this.cmdGenerateUuid = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblUuid
            // 
            this.lblUuid.AutoSize = true;
            this.lblUuid.Location = new System.Drawing.Point(12, 17);
            this.lblUuid.Name = "lblUuid";
            this.lblUuid.Size = new System.Drawing.Size(40, 13);
            this.lblUuid.TabIndex = 0;
            this.lblUuid.Text = "UUID: ";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 43);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(61, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Username: ";
            // 
            // txbUsername
            // 
            this.txbUsername.Location = new System.Drawing.Point(79, 40);
            this.txbUsername.MaxLength = 20;
            this.txbUsername.Name = "txbUsername";
            this.txbUsername.Size = new System.Drawing.Size(284, 20);
            this.txbUsername.TabIndex = 2;
            // 
            // txbUuid
            // 
            this.txbUuid.Location = new System.Drawing.Point(79, 14);
            this.txbUuid.Name = "txbUuid";
            this.txbUuid.ReadOnly = true;
            this.txbUuid.Size = new System.Drawing.Size(203, 20);
            this.txbUuid.TabIndex = 3;
            // 
            // cmdConfirm
            // 
            this.cmdConfirm.Location = new System.Drawing.Point(15, 66);
            this.cmdConfirm.Name = "cmdConfirm";
            this.cmdConfirm.Size = new System.Drawing.Size(348, 23);
            this.cmdConfirm.TabIndex = 4;
            this.cmdConfirm.Text = "Confirm";
            this.cmdConfirm.UseVisualStyleBackColor = true;
            this.cmdConfirm.Click += new System.EventHandler(this.cmdConfirm_Click);
            // 
            // cmdGenerateUuid
            // 
            this.cmdGenerateUuid.Location = new System.Drawing.Point(288, 11);
            this.cmdGenerateUuid.Name = "cmdGenerateUuid";
            this.cmdGenerateUuid.Size = new System.Drawing.Size(75, 23);
            this.cmdGenerateUuid.TabIndex = 5;
            this.cmdGenerateUuid.Text = "Generate";
            this.cmdGenerateUuid.UseVisualStyleBackColor = true;
            this.cmdGenerateUuid.Click += new System.EventHandler(this.cmdGenerateUuid_Click);
            // 
            // FrmCreateIdentity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 96);
            this.Controls.Add(this.cmdGenerateUuid);
            this.Controls.Add(this.cmdConfirm);
            this.Controls.Add(this.txbUuid);
            this.Controls.Add(this.txbUsername);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblUuid);
            this.Name = "FrmCreateIdentity";
            this.Text = "ChainReact - Create identity";
            this.Shown += new System.EventHandler(this.FrmCreateIdentity_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUuid;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txbUsername;
        private System.Windows.Forms.TextBox txbUuid;
        private System.Windows.Forms.Button cmdConfirm;
        private System.Windows.Forms.Button cmdGenerateUuid;
    }
}