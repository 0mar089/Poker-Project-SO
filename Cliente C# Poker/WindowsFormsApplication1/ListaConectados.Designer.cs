namespace WindowsFormsApplication1 {
    partial class ListaConectados {
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
            if (disposing && (components != null)) {
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
            this.dataGridViewConectados = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConectados)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewConectados
            // 
            this.dataGridViewConectados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewConectados.Location = new System.Drawing.Point(94, 92);
            this.dataGridViewConectados.Name = "dataGridViewConectados";
            this.dataGridViewConectados.RowHeadersWidth = 51;
            this.dataGridViewConectados.RowTemplate.Height = 24;
            this.dataGridViewConectados.Size = new System.Drawing.Size(240, 260);
            this.dataGridViewConectados.TabIndex = 0;
            // 
            // ListaConectados
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dataGridViewConectados);
            this.Name = "ListaConectados";
            this.Text = "ListaConectadoscs";
            this.Load += new System.EventHandler(this.ListaConectados_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConectados)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewConectados;
    }
}