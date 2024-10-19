using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1 {
    public partial class ListaConectados : Form {
        public ListaConectados()
        {
            InitializeComponent();
        }

        public List<string> listaConectados = new List<string>();

        public void SetConectados(List<string> conectados)
        {
            this.listaConectados = conectados;
        }

        private void ListaConectados_Load(object sender, EventArgs e)
        {
            // Llenar el DataGridView con los nombres de los conectados
            dataGridViewConectados.Columns.Clear(); // Limpiar columnas previas si las hay
            dataGridViewConectados.Columns.Add("Nombre", "Nombre");

            foreach (var nombre in listaConectados) {
                // Agregar una nueva fila con el nombre de cada jugador conectado
                dataGridViewConectados.Rows.Add(nombre);
            }
        }
    }
}
