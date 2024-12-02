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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.opcionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iNVITARJUGADORESStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ButtonInvite = new System.Windows.Forms.Button();
            this.textChat = new System.Windows.Forms.TextBox();
            this.Escribir = new System.Windows.Forms.Button();
            this.Chat = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SalaLabel1 = new System.Windows.Forms.Label();
            this.SalaLabel2 = new System.Windows.Forms.Label();
            this.SalaLabel3 = new System.Windows.Forms.Label();
            this.SalaLabel4 = new System.Windows.Forms.Label();
            this.BtnSala1 = new System.Windows.Forms.Button();
            this.BtnSala2 = new System.Windows.Forms.Button();
            this.BtnSala3 = new System.Windows.Forms.Button();
            this.BtnSala4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConectados)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonRegister
            // 
            this.buttonRegister.Location = new System.Drawing.Point(137, 246);
            this.buttonRegister.Name = "buttonRegister";
            this.buttonRegister.Size = new System.Drawing.Size(113, 37);
            this.buttonRegister.TabIndex = 5;
            this.buttonRegister.Text = "Register";
            this.buttonRegister.UseVisualStyleBackColor = true;
            this.buttonRegister.Click += new System.EventHandler(this.buttonRegister_Click_1);
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(137, 191);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(112, 37);
            this.buttonLogin.TabIndex = 6;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click_1);
            // 
            // nombre
            // 
            this.nombre.Location = new System.Drawing.Point(93, 56);
            this.nombre.Name = "nombre";
            this.nombre.Size = new System.Drawing.Size(254, 22);
            this.nombre.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Nombre";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Cuenta";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 16);
            this.label4.TabIndex = 10;
            this.label4.Text = "Contraseña";
            // 
            // cuenta
            // 
            this.cuenta.Location = new System.Drawing.Point(93, 108);
            this.cuenta.Name = "cuenta";
            this.cuenta.Size = new System.Drawing.Size(254, 22);
            this.cuenta.TabIndex = 11;
            // 
            // contraseña
            // 
            this.contraseña.Location = new System.Drawing.Point(93, 147);
            this.contraseña.Name = "contraseña";
            this.contraseña.Size = new System.Drawing.Size(254, 22);
            this.contraseña.TabIndex = 12;
            this.contraseña.TextChanged += new System.EventHandler(this.contraseña_TextChanged);
            // 
            // dataGridViewConectados
            // 
            this.dataGridViewConectados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewConectados.Location = new System.Drawing.Point(391, 56);
            this.dataGridViewConectados.Name = "dataGridViewConectados";
            this.dataGridViewConectados.RowHeadersWidth = 51;
            this.dataGridViewConectados.RowTemplate.Height = 24;
            this.dataGridViewConectados.Size = new System.Drawing.Size(231, 268);
            this.dataGridViewConectados.TabIndex = 23;
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
            this.ButtonInvite.Location = new System.Drawing.Point(391, 360);
            this.ButtonInvite.Name = "ButtonInvite";
            this.ButtonInvite.Size = new System.Drawing.Size(77, 26);
            this.ButtonInvite.TabIndex = 26;
            this.ButtonInvite.Text = "Invitar";
            this.ButtonInvite.UseVisualStyleBackColor = true;
            this.ButtonInvite.Click += new System.EventHandler(this.buttonInvite_Click);
            // 
            // textChat
            // 
            this.textChat.Location = new System.Drawing.Point(1010, 289);
            this.textChat.Name = "textChat";
            this.textChat.Size = new System.Drawing.Size(159, 22);
            this.textChat.TabIndex = 32;
            // 
            // Escribir
            // 
            this.Escribir.Location = new System.Drawing.Point(1175, 289);
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
            this.Chat.Location = new System.Drawing.Point(1010, 28);
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
            // SalaLabel1
            // 
            this.SalaLabel1.AutoSize = true;
            this.SalaLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SalaLabel1.Location = new System.Drawing.Point(643, 81);
            this.SalaLabel1.Name = "SalaLabel1";
            this.SalaLabel1.Size = new System.Drawing.Size(90, 31);
            this.SalaLabel1.TabIndex = 34;
            this.SalaLabel1.Text = "Sala 1";
            // 
            // SalaLabel2
            // 
            this.SalaLabel2.AutoSize = true;
            this.SalaLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SalaLabel2.Location = new System.Drawing.Point(643, 138);
            this.SalaLabel2.Name = "SalaLabel2";
            this.SalaLabel2.Size = new System.Drawing.Size(97, 31);
            this.SalaLabel2.TabIndex = 35;
            this.SalaLabel2.Text = "Sala 2 ";
            // 
            // SalaLabel3
            // 
            this.SalaLabel3.AutoSize = true;
            this.SalaLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SalaLabel3.Location = new System.Drawing.Point(643, 197);
            this.SalaLabel3.Name = "SalaLabel3";
            this.SalaLabel3.Size = new System.Drawing.Size(97, 31);
            this.SalaLabel3.TabIndex = 36;
            this.SalaLabel3.Text = "Sala 3 ";
            // 
            // SalaLabel4
            // 
            this.SalaLabel4.AutoSize = true;
            this.SalaLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SalaLabel4.Location = new System.Drawing.Point(643, 252);
            this.SalaLabel4.Name = "SalaLabel4";
            this.SalaLabel4.Size = new System.Drawing.Size(97, 31);
            this.SalaLabel4.TabIndex = 37;
            this.SalaLabel4.Text = "Sala 4 ";
            // 
            // BtnSala1
            // 
            this.BtnSala1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSala1.Location = new System.Drawing.Point(803, 77);
            this.BtnSala1.Name = "BtnSala1";
            this.BtnSala1.Size = new System.Drawing.Size(82, 35);
            this.BtnSala1.TabIndex = 38;
            this.BtnSala1.Text = "Unirse";
            this.BtnSala1.UseVisualStyleBackColor = true;
            this.BtnSala1.Click += new System.EventHandler(this.BtnSala1_Click);
            // 
            // BtnSala2
            // 
            this.BtnSala2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSala2.Location = new System.Drawing.Point(803, 137);
            this.BtnSala2.Name = "BtnSala2";
            this.BtnSala2.Size = new System.Drawing.Size(82, 35);
            this.BtnSala2.TabIndex = 39;
            this.BtnSala2.Text = "Unirse";
            this.BtnSala2.UseVisualStyleBackColor = true;
            this.BtnSala2.Click += new System.EventHandler(this.BtnSala2_Click);
            // 
            // BtnSala3
            // 
            this.BtnSala3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSala3.Location = new System.Drawing.Point(803, 191);
            this.BtnSala3.Name = "BtnSala3";
            this.BtnSala3.Size = new System.Drawing.Size(82, 35);
            this.BtnSala3.TabIndex = 40;
            this.BtnSala3.Text = "Unirse";
            this.BtnSala3.UseVisualStyleBackColor = true;
            this.BtnSala3.Click += new System.EventHandler(this.BtnSala3_Click);
            // 
            // BtnSala4
            // 
            this.BtnSala4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSala4.Location = new System.Drawing.Point(803, 255);
            this.BtnSala4.Name = "BtnSala4";
            this.BtnSala4.Size = new System.Drawing.Size(82, 35);
            this.BtnSala4.TabIndex = 41;
            this.BtnSala4.Text = "Unirse";
            this.BtnSala4.UseVisualStyleBackColor = true;
            this.BtnSala4.Click += new System.EventHandler(this.BtnSala4_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(745, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 31);
            this.label1.TabIndex = 42;
            this.label1.Text = "0/4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(745, 137);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 31);
            this.label5.TabIndex = 43;
            this.label5.Text = "0/4";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(745, 197);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 31);
            this.label6.TabIndex = 44;
            this.label6.Text = "0/4";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(745, 259);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 31);
            this.label7.TabIndex = 45;
            this.label7.Text = "0/4";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1291, 692);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnSala4);
            this.Controls.Add(this.BtnSala3);
            this.Controls.Add(this.BtnSala2);
            this.Controls.Add(this.BtnSala1);
            this.Controls.Add(this.SalaLabel4);
            this.Controls.Add(this.SalaLabel3);
            this.Controls.Add(this.SalaLabel2);
            this.Controls.Add(this.SalaLabel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textChat);
            this.Controls.Add(this.Escribir);
            this.Controls.Add(this.Chat);
            this.Controls.Add(this.ButtonInvite);
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
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem opcionesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iNVITARJUGADORESStripMenuItem;
        private System.Windows.Forms.Button ButtonInvite;
        private System.Windows.Forms.TextBox textChat;
        private System.Windows.Forms.Button Escribir;
        private System.Windows.Forms.ListBox Chat;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label SalaLabel1;
        private System.Windows.Forms.Label SalaLabel2;
        private System.Windows.Forms.Label SalaLabel3;
        private System.Windows.Forms.Label SalaLabel4;
        private System.Windows.Forms.Button BtnSala1;
        private System.Windows.Forms.Button BtnSala2;
        private System.Windows.Forms.Button BtnSala3;
        private System.Windows.Forms.Button BtnSala4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}

