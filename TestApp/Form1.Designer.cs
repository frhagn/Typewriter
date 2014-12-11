namespace TestApp
{
    partial class Form1
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
            this.source = new System.Windows.Forms.TextBox();
            this.target = new System.Windows.Forms.TextBox();
            this.preview = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // source
            // 
            this.source.AcceptsReturn = true;
            this.source.AcceptsTab = true;
            this.source.Dock = System.Windows.Forms.DockStyle.Left;
            this.source.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.source.Location = new System.Drawing.Point(0, 0);
            this.source.Multiline = true;
            this.source.Name = "source";
            this.source.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.source.Size = new System.Drawing.Size(504, 692);
            this.source.TabIndex = 1;
            this.source.TextChanged += new System.EventHandler(this.source_TextChanged);
            // 
            // target
            // 
            this.target.Dock = System.Windows.Forms.DockStyle.Right;
            this.target.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.target.Location = new System.Drawing.Point(994, 0);
            this.target.Multiline = true;
            this.target.Name = "target";
            this.target.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.target.Size = new System.Drawing.Size(486, 692);
            this.target.TabIndex = 2;
            // 
            // preview
            // 
            this.preview.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.preview.Location = new System.Drawing.Point(510, 0);
            this.preview.Name = "preview";
            this.preview.Size = new System.Drawing.Size(478, 692);
            this.preview.TabIndex = 3;
            this.preview.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1480, 692);
            this.Controls.Add(this.preview);
            this.Controls.Add(this.target);
            this.Controls.Add(this.source);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox source;
        private System.Windows.Forms.TextBox target;
        private System.Windows.Forms.RichTextBox preview;
    }
}

