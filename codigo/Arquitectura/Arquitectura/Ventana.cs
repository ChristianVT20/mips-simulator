using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace Arquitectura
{
    public partial class Ventana : Form
    {
        private ArrayList rutas; // array con las rutas de los hilos seleccionados
        public Ventana()
        {
            rutas = new ArrayList();
            InitializeComponent();
        }

        // muestra un cuadro de dialogo para seleccionar los hilos
        private void botonSeleccionar_Click(object sender, EventArgs e){
            string hilo;
            openFileDialog1.Filter = "Archivos de texto (*.txt)|*.txt";
            openFileDialog1.Multiselect = true;
            DialogResult result = openFileDialog1.ShowDialog(); // Abre el selector de archivos
            if (result == DialogResult.OK){ // Test result.
                foreach (String archivo in openFileDialog1.FileNames){
                    hilo = archivo;// ruta del hilo
                    rutas.Add(hilo);
                    ListViewItem itemHilo = new ListViewItem(hilo);
                    listaHilos.Items.Add(itemHilo); // agrega la ruta a la ventana
                }
                
            }
            
        }  

        // borra las rutas seleccionadas
        private void botonBorrar_Click(object sender, EventArgs e)
        {
            rutas.Clear();
            listaHilos.Items.Clear();
        }

        // verifica que existan 3 archivos de hilos, luego cierra la ventana, 
        // ejecuta y muestra los resultados por consola.
        private void botonIniciar_Click(object sender, EventArgs e)
        {
            this.Visible = false; // Cierra la ventana
            Multiprocesador m = new Multiprocesador(rutas);
            m.ejecutar();
        }
    }
}
