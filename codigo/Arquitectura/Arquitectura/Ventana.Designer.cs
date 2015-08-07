namespace Arquitectura
{
    partial class Ventana
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
            this.listaHilos = new System.Windows.Forms.ListView();
            this.botonSeleccionar = new System.Windows.Forms.Button();
            this.botonBorrar = new System.Windows.Forms.Button();
            this.botonIniciar = new System.Windows.Forms.Button();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // listaHilos
            // 
            this.listaHilos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listaHilos.Location = new System.Drawing.Point(12, 12);
            this.listaHilos.Name = "listaHilos";
            this.listaHilos.Size = new System.Drawing.Size(544, 256);
            this.listaHilos.TabIndex = 0;
            this.listaHilos.UseCompatibleStateImageBehavior = false;
            this.listaHilos.View = System.Windows.Forms.View.Details;
            // 
            // botonSeleccionar
            // 
            this.botonSeleccionar.Location = new System.Drawing.Point(562, 12);
            this.botonSeleccionar.Name = "botonSeleccionar";
            this.botonSeleccionar.Size = new System.Drawing.Size(127, 23);
            this.botonSeleccionar.TabIndex = 1;
            this.botonSeleccionar.Text = "Seleccionar Hilos";
            this.botonSeleccionar.UseVisualStyleBackColor = true;
            this.botonSeleccionar.Click += new System.EventHandler(this.botonSeleccionar_Click);
            // 
            // botonBorrar
            // 
            this.botonBorrar.Location = new System.Drawing.Point(562, 41);
            this.botonBorrar.Name = "botonBorrar";
            this.botonBorrar.Size = new System.Drawing.Size(127, 23);
            this.botonBorrar.TabIndex = 2;
            this.botonBorrar.Text = "Borrar Selección";
            this.botonBorrar.UseVisualStyleBackColor = true;
            this.botonBorrar.Click += new System.EventHandler(this.botonBorrar_Click);
            // 
            // botonIniciar
            // 
            this.botonIniciar.Location = new System.Drawing.Point(562, 245);
            this.botonIniciar.Name = "botonIniciar";
            this.botonIniciar.Size = new System.Drawing.Size(127, 23);
            this.botonIniciar.TabIndex = 3;
            this.botonIniciar.Text = "Iniciar Simulación";
            this.botonIniciar.UseVisualStyleBackColor = true;
            this.botonIniciar.Click += new System.EventHandler(this.botonIniciar_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Hilos";
            this.columnHeader1.Width = 544;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Ventana
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 280);
            this.Controls.Add(this.botonIniciar);
            this.Controls.Add(this.botonBorrar);
            this.Controls.Add(this.botonSeleccionar);
            this.Controls.Add(this.listaHilos);
            this.Name = "Ventana";
            this.Text = "Selección de Hilos";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listaHilos;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button botonSeleccionar;
        private System.Windows.Forms.Button botonBorrar;
        private System.Windows.Forms.Button botonIniciar;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;

    }
}