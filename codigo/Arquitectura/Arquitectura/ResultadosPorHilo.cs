using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arquitectura
{
    public partial class ResultadosPorHilo : Form
    {
        public ResultadosPorHilo()
        {
            InitializeComponent();
        }

        public void escribirProcesador(string numProc){
            etiquetaProcesador.Text = "Procesador " + numProc;
        }
        public void escribirFinal(){
            etiquetaProcesador.Text += " (FINAL)";
        }
        public void escribirRegistros(string reg){
            registros.Text = reg;
        }

        public void mostrarCaches(string c1, string c2, string c3)
        {
            cache1.Text = c1;
            cache2.Text = c2;
            cache3.Text = c3;
        }

        public void mostrarDirectorios(string d1, string d2, string d3){
            Dir1.Text = d1;
            Dir2.Text = d2;
            Dir3.Text = d3; 
        }
        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
