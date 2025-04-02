namespace Client_navale
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LabelRisultati = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.listBoxMessaggi = new System.Windows.Forms.ListBox();
            this.textBoxMessaggi = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(50, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "Campo Avversario:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(600, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 22);
            this.label2.TabIndex = 1;
            this.label2.Text = "Il tuo campo:";
            // 
            // LabelRisultati
            // 
            this.LabelRisultati.AutoSize = true;
            this.LabelRisultati.BackColor = System.Drawing.Color.Transparent;
            this.LabelRisultati.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelRisultati.Location = new System.Drawing.Point(488, 550);
            this.LabelRisultati.Name = "LabelRisultati";
            this.LabelRisultati.Size = new System.Drawing.Size(0, 22);
            this.LabelRisultati.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(345, 550);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 22);
            this.label3.TabIndex = 3;
            this.label3.Text = "Risultato Colpo:";
            // 
            // listBoxMessaggi
            // 
            this.listBoxMessaggi.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxMessaggi.FormattingEnabled = true;
            this.listBoxMessaggi.HorizontalScrollbar = true;
            this.listBoxMessaggi.ItemHeight = 22;
            this.listBoxMessaggi.Location = new System.Drawing.Point(600, 401);
            this.listBoxMessaggi.Name = "listBoxMessaggi";
            this.listBoxMessaggi.Size = new System.Drawing.Size(350, 136);
            this.listBoxMessaggi.TabIndex = 4;
            // 
            // textBoxMessaggi
            // 
            this.textBoxMessaggi.Font = new System.Drawing.Font("Arial", 14.25F);
            this.textBoxMessaggi.ForeColor = System.Drawing.Color.LightGray;
            this.textBoxMessaggi.Location = new System.Drawing.Point(600, 543);
            this.textBoxMessaggi.Name = "textBoxMessaggi";
            this.textBoxMessaggi.Size = new System.Drawing.Size(350, 29);
            this.textBoxMessaggi.TabIndex = 5;
            this.textBoxMessaggi.Text = "Scrivi un messaggio...";
            this.textBoxMessaggi.Enter += new System.EventHandler(this.textBoxMessaggi_Enter);
            this.textBoxMessaggi.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxMessaggi_KeyDown);
            this.textBoxMessaggi.Leave += new System.EventHandler(this.textBoxMessaggi_Leave);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSeaGreen;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(984, 621);
            this.Controls.Add(this.textBoxMessaggi);
            this.Controls.Add(this.listBoxMessaggi);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.LabelRisultati);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Battaglia Navale";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_Closing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LabelRisultati;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listBoxMessaggi;
        private System.Windows.Forms.TextBox textBoxMessaggi;
    }
}

