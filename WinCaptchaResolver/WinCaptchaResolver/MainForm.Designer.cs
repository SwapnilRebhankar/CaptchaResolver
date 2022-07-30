namespace WinCaptchaResolver
{
    partial class MainForm
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
            this.btnSelect = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.lblCaptchaText = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bckColorsPictBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bckColorsPictBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(16, 15);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(489, 28);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "Select Image";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(16, 50);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(489, 169);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // lblCaptchaText
            // 
            this.lblCaptchaText.AutoSize = true;
            this.lblCaptchaText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCaptchaText.Location = new System.Drawing.Point(184, 231);
            this.lblCaptchaText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCaptchaText.MinimumSize = new System.Drawing.Size(200, 0);
            this.lblCaptchaText.Name = "lblCaptchaText";
            this.lblCaptchaText.Size = new System.Drawing.Size(200, 20);
            this.lblCaptchaText.TabIndex = 3;
            this.lblCaptchaText.Text = "Not Recognise";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 260);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Background colors";
            // 
            // bckColorsPictBox
            // 
            this.bckColorsPictBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bckColorsPictBox.Location = new System.Drawing.Point(19, 281);
            this.bckColorsPictBox.Margin = new System.Windows.Forms.Padding(4);
            this.bckColorsPictBox.Name = "bckColorsPictBox";
            this.bckColorsPictBox.Size = new System.Drawing.Size(539, 81);
            this.bckColorsPictBox.TabIndex = 5;
            this.bckColorsPictBox.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 375);
            this.Controls.Add(this.bckColorsPictBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCaptchaText);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.btnSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Captcha Resolver Using AForge & Teseract";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bckColorsPictBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label lblCaptchaText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox bckColorsPictBox;
    }
}

