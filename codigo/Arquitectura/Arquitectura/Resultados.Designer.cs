namespace Arquitectura
{
    partial class Resultados
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
            this.reloj1 = new System.Windows.Forms.Label();
            this.reloj2 = new System.Windows.Forms.Label();
            this.reloj3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mem1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.mem2 = new System.Windows.Forms.TextBox();
            this.mem3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.hilo3 = new System.Windows.Forms.Label();
            this.hilo2 = new System.Windows.Forms.Label();
            this.hilo1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // reloj1
            // 
            this.reloj1.AutoSize = true;
            this.reloj1.Location = new System.Drawing.Point(12, 37);
            this.reloj1.Name = "reloj1";
            this.reloj1.Size = new System.Drawing.Size(13, 13);
            this.reloj1.TabIndex = 0;
            this.reloj1.Text = "0";
            this.reloj1.Click += new System.EventHandler(this.reloj1_Click);
            // 
            // reloj2
            // 
            this.reloj2.AutoSize = true;
            this.reloj2.Location = new System.Drawing.Point(143, 37);
            this.reloj2.Name = "reloj2";
            this.reloj2.Size = new System.Drawing.Size(13, 13);
            this.reloj2.TabIndex = 1;
            this.reloj2.Text = "0";
            // 
            // reloj3
            // 
            this.reloj3.AutoSize = true;
            this.reloj3.Location = new System.Drawing.Point(268, 37);
            this.reloj3.Name = "reloj3";
            this.reloj3.Size = new System.Drawing.Size(13, 13);
            this.reloj3.TabIndex = 2;
            this.reloj3.Text = "0";
            this.reloj3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Reloj P1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(143, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Reloj P2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(268, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Reloj P3";
            // 
            // mem1
            // 
            this.mem1.Enabled = false;
            this.mem1.Location = new System.Drawing.Point(15, 128);
            this.mem1.Multiline = true;
            this.mem1.Name = "mem1";
            this.mem1.Size = new System.Drawing.Size(300, 38);
            this.mem1.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Memoria Compartida";
            // 
            // mem2
            // 
            this.mem2.Enabled = false;
            this.mem2.Location = new System.Drawing.Point(15, 172);
            this.mem2.Multiline = true;
            this.mem2.Name = "mem2";
            this.mem2.Size = new System.Drawing.Size(300, 38);
            this.mem2.TabIndex = 8;
            // 
            // mem3
            // 
            this.mem3.Enabled = false;
            this.mem3.Location = new System.Drawing.Point(15, 216);
            this.mem3.Multiline = true;
            this.mem3.Name = "mem3";
            this.mem3.Size = new System.Drawing.Size(300, 38);
            this.mem3.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(268, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Hilo en P3";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(143, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Hilo en P2";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 63);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Hilo en P1";
            // 
            // hilo3
            // 
            this.hilo3.AutoSize = true;
            this.hilo3.Location = new System.Drawing.Point(268, 79);
            this.hilo3.Name = "hilo3";
            this.hilo3.Size = new System.Drawing.Size(0, 13);
            this.hilo3.TabIndex = 12;
            // 
            // hilo2
            // 
            this.hilo2.AutoSize = true;
            this.hilo2.Location = new System.Drawing.Point(143, 79);
            this.hilo2.Name = "hilo2";
            this.hilo2.Size = new System.Drawing.Size(0, 13);
            this.hilo2.TabIndex = 11;
            // 
            // hilo1
            // 
            this.hilo1.AutoSize = true;
            this.hilo1.Location = new System.Drawing.Point(12, 79);
            this.hilo1.Name = "hilo1";
            this.hilo1.Size = new System.Drawing.Size(0, 13);
            this.hilo1.TabIndex = 10;
            // 
            // Resultados
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 262);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.hilo3);
            this.Controls.Add(this.hilo2);
            this.Controls.Add(this.hilo1);
            this.Controls.Add(this.mem3);
            this.Controls.Add(this.mem2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.mem1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.reloj3);
            this.Controls.Add(this.reloj2);
            this.Controls.Add(this.reloj1);
            this.Name = "Resultados";
            this.Text = "Resultados";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Resultados_FormClosed);
            this.Load += new System.EventHandler(this.Resultados_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label reloj1;
        private System.Windows.Forms.Label reloj2;
        private System.Windows.Forms.Label reloj3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox mem1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox mem2;
        private System.Windows.Forms.TextBox mem3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label hilo3;
        private System.Windows.Forms.Label hilo2;
        private System.Windows.Forms.Label hilo1;
    }
}