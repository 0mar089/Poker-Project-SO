namespace WindowsFormsApplication1 {
    partial class Form1 {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonRegister = new System.Windows.Forms.Button();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.nombre = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cuenta = new System.Windows.Forms.TextBox();
            this.contraseña = new System.Windows.Forms.TextBox();
            this.dataGridViewConectados = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.opcionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iNVITARJUGADORESStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ButtonInvite = new System.Windows.Forms.Button();
            this.textChat = new System.Windows.Forms.TextBox();
            this.Escribir = new System.Windows.Forms.Button();
            this.Chat = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConectados)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonRegister
            // 
            this.buttonRegister.Location = new System.Drawing.Point(227, 321);
            this.buttonRegister.Name = "buttonRegister";
            this.buttonRegister.Size = new System.Drawing.Size(133, 49);
            this.buttonRegister.TabIndex = 5;
            this.buttonRegister.Text = "Register";
            this.buttonRegister.UseVisualStyleBackColor = true;
            this.buttonRegister.Click += new System.EventHandler(this.buttonRegister_Click_1);
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(227, 266);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(132, 49);
            this.buttonLogin.TabIndex = 6;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click_1);
            // 
            // nombre
            // 
            this.nombre.Location = new System.Drawing.Point(200, 111);
            this.nombre.Name = "nombre";
            this.nombre.Size = new System.Drawing.Size(254, 22);
            this.nombre.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(138, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Nombre";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(145, 163);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Cuenta";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(118, 202);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 16);
            this.label4.TabIndex = 10;
            this.label4.Text = "Contraseña";
            // 
            // cuenta
            // 
            this.cuenta.Location = new System.Drawing.Point(200, 163);
            this.cuenta.Name = "cuenta";
            this.cuenta.Size = new System.Drawing.Size(254, 22);
            this.cuenta.TabIndex = 11;
            // 
            // contraseña
            // 
            this.contraseña.Location = new System.Drawing.Point(200, 202);
            this.contraseña.Name = "contraseña";
            this.contraseña.Size = new System.Drawing.Size(254, 22);
            this.contraseña.TabIndex = 12;
            // 
            // dataGridViewConectados
            // 
            this.dataGridViewConectados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewConectados.Location = new System.Drawing.Point(531, 142);
            this.dataGridViewConectados.Name = "dataGridViewConectados";
            this.dataGridViewConectados.RowHeadersWidth = 51;
            this.dataGridViewConectados.RowTemplate.Height = 24;
            this.dataGridViewConectados.Size = new System.Drawing.Size(231, 268);
            this.dataGridViewConectados.TabIndex = 23;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Location = new System.Drawing.Point(0, 28);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1291, 25);
            this.toolStrip1.TabIndex = 24;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.opcionesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1291, 28);
            this.menuStrip1.TabIndex = 25;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // opcionesToolStripMenuItem
            // 
            this.opcionesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iNVITARJUGADORESStripMenuItem});
            this.opcionesToolStripMenuItem.Name = "opcionesToolStripMenuItem";
            this.opcionesToolStripMenuItem.Size = new System.Drawing.Size(85, 24);
            this.opcionesToolStripMenuItem.Text = "Opciones";
            // 
            // iNVITARJUGADORESStripMenuItem
            // 
            this.iNVITARJUGADORESStripMenuItem.Name = "iNVITARJUGADORESStripMenuItem";
            this.iNVITARJUGADORESStripMenuItem.Size = new System.Drawing.Size(83, 26);
            // 
            // ButtonInvite
            // 
            this.ButtonInvite.Location = new System.Drawing.Point(531, 446);
            this.ButtonInvite.Name = "ButtonInvite";
            this.ButtonInvite.Size = new System.Drawing.Size(77, 26);
            this.ButtonInvite.TabIndex = 26;
            this.ButtonInvite.Text = "Invitar";
            this.ButtonInvite.UseVisualStyleBackColor = true;
            this.ButtonInvite.Click += new System.EventHandler(this.buttonInvite_Click);
            // 
            // textChat
            // 
            this.textChat.Location = new System.Drawing.Point(950, 403);
            this.textChat.Name = "textChat";
            this.textChat.Size = new System.Drawing.Size(159, 22);
            this.textChat.TabIndex = 32;
            // 
            // Escribir
            // 
            this.Escribir.Location = new System.Drawing.Point(1115, 403);
            this.Escribir.Name = "Escribir";
            this.Escribir.Size = new System.Drawing.Size(86, 34);
            this.Escribir.TabIndex = 31;
            this.Escribir.Text = "Enviar";
            this.Escribir.UseVisualStyleBackColor = true;
            this.Escribir.Click += new System.EventHandler(this.Escribir_Click);
            // 
            // Chat
            // 
            this.Chat.FormattingEnabled = true;
            this.Chat.ItemHeight = 16;
            this.Chat.Location = new System.Drawing.Point(950, 142);
            this.Chat.Name = "Chat";
            this.Chat.Size = new System.Drawing.Size(251, 244);
            this.Chat.TabIndex = 30;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(43, 634);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(105, 32);
            this.button1.TabIndex = 33;
            this.button1.Text = "Salir";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1291, 692);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textChat);
            this.Controls.Add(this.Escribir);
            this.Controls.Add(this.Chat);
            this.Controls.Add(this.ButtonInvite);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dataGridViewConectados);
            this.Controls.Add(this.contraseña);
            this.Controls.Add(this.cuenta);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nombre);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.buttonRegister);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConectados)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonRegister;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.TextBox nombre;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox cuenta;
        private System.Windows.Forms.TextBox contraseña;
        private System.Windows.Forms.DataGridView dataGridViewConectados;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem opcionesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iNVITARJUGADORESStripMenuItem;
        private System.Windows.Forms.Button ButtonInvite;
        private System.Windows.Forms.TextBox textChat;
        private System.Windows.Forms.Button Escribir;
        private System.Windows.Forms.ListBox Chat;
        private System.Windows.Forms.Button button1;
    }
}

