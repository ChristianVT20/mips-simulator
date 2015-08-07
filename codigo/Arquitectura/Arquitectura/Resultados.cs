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
    public partial class Resultados : Form
    {
        public Resultados()
        {
            InitializeComponent();
        }

        private void Resultados_Load(object sender, EventArgs e)
        {

        }

        public void mostrarMemorias(String m1, String m2, String m3)
        {
            mem1.Text = m1;
            mem2.Text = m2;
            mem3.Text = m3;
        }
        public void mostrarHiloActual(String proc, String hilo) {
            switch (proc){
                case "Procesador1":
                    hilo1.Text = hilo;
                    break;
                case "Procesador2":
                    hilo2.Text = hilo;
                    break;
                case "Procesador3":
                    hilo3.Text = hilo;
                    break;
            }
        }

        public void ponerReloj(int proc, int r)
        {
            switch (proc) { 
                case 1:
                    reloj1.Text = "" + r;
                    break;
                case 2:
                    reloj2.Text = "" + r;
                    break;
                case 3:
                    reloj3.Text = "" + r;
                    break;
            }
            
        }

        private void Resultados_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void reloj1_Click(object sender, EventArgs e)
        {

        }
    }
}
