namespace Echo_system
{
    partial class Launcher
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
            this.launchbtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // launchbtn
            // 
            this.launchbtn.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.launchbtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.launchbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.launchbtn.Location = new System.Drawing.Point(828, 520);
            this.launchbtn.Name = "launchbtn";
            this.launchbtn.Size = new System.Drawing.Size(217, 52);
            this.launchbtn.TabIndex = 0;
            this.launchbtn.Text = "Launch!";
            this.launchbtn.UseVisualStyleBackColor = false;
            this.launchbtn.Click += new System.EventHandler(this.launchbtn_Click);
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1140, 691);
            this.Controls.Add(this.launchbtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Launcher";
            this.Text = "Echo\'System";
            this.Load += new System.EventHandler(this.launcher_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button launchbtn;
    }
}

