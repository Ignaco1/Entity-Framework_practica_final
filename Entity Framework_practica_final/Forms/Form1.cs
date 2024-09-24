using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entity_Framework_practica_final.Models;

namespace Entity_Framework_practica_final
{
    public partial class Form1 : Form
    {
        string vari;
        public Form1()
        {
            InitializeComponent();

            ModoVista();
            txt_edad.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CargaDatos();
        }

        private void ModoCarga()
        {
            groupBox1.Enabled = false;
            groupBox2.Enabled = true;
        }

        private void Limpiar()
        {
            txt_nombre.Text = "";
            txt_edad.Text = "";
            fecha_picker.Text = "";
        }

        private int GetId()
        {
            try
            {
                return Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString());
            }
            catch(Exception ex)
            {
                throw new Exception("Error a la hora de obtener el id: " + ex.Message);
            }
        }

        private Personas GetPersona()
        {
            using(DB_PRACTICAEntities db = new DB_PRACTICAEntities())
            {
                int id = GetId();

                Personas persona = db.Personas.Find(id);

                return persona;
            }
        }

        private void ModoVista()
        {
            groupBox1.Enabled = true;
            groupBox2.Enabled = false;
        }

        private void CargaDatos()
        {
            using(DB_PRACTICAEntities db = new DB_PRACTICAEntities())
            {
                var lista = from d in db.Personas select d;

                dataGridView1.DataSource = lista.ToList();
            }
        }

        private int GetEdad()
        {
            int edad = DateTime.Now.Year - fecha_picker.Value.Year;

            if (DateTime.Now.Month < fecha_picker.Value.Month)
            {
                edad -= 1;
            }
            else if(DateTime.Now.Month == fecha_picker.Value.Month  &&  DateTime.Now.Day < fecha_picker.Value.Day)
            {
                edad -= 1;
            }

            return edad;
        }

        private void btn_cerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_agregar_Click(object sender, EventArgs e)
        {
            vari = "A";

            ModoCarga();
        }

        private void btn_mod_Click(object sender, EventArgs e)
        {
            vari = "M";

            try
            {
                Personas people = GetPersona();

                txt_nombre.Text = people.Nombre;
                fecha_picker.Value = people.Fecha_de_nacimiento;
                txt_edad.Text = Convert.ToString(people.Edad);

                ModoCarga();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error al intentar pasar los datos: " + ex.Message);
            }
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            int id = GetId();

            using (DB_PRACTICAEntities db = new DB_PRACTICAEntities())
            {
                Personas tabla = db.Personas.Find(id);

                string fecha = tabla.Fecha_de_nacimiento.ToString("dd/MM/yyyy");

                DialogResult respuesta = MessageBox.Show($"Esta seguro que dese eliminar al usuario:\n\nNombre: {tabla.Nombre}\n\nFecha de nacimiento: {fecha}\n\nEdad: {tabla.Edad}", "AVISO", MessageBoxButtons.YesNo);

                if (respuesta == DialogResult.Yes)
                {
                    db.Personas.Remove(tabla);
                    db.SaveChanges();
                    CargaDatos();
                }

            }
        }

        private void btn_guardar_Click(object sender, EventArgs e)
        {
            try
            {
                using (DB_PRACTICAEntities db = new DB_PRACTICAEntities())
                {
                    if (vari == "A")
                    {
                        int edad = GetEdad();

                        Personas tabla = new Personas();

                        tabla.Nombre = txt_nombre.Text;
                        tabla.Fecha_de_nacimiento = fecha_picker.Value.Date;
                        tabla.Edad = edad;

                        db.Personas.Add(tabla);

                    }
                    else if (vari == "M")
                    {
                        int edad = GetEdad();

                        Personas tabla = GetPersona();

                        tabla.Nombre = txt_nombre.Text;
                        tabla.Fecha_de_nacimiento = fecha_picker.Value.Date;
                        tabla.Edad = edad;

                        db.Entry(tabla).State = System.Data.Entity.EntityState.Modified;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar: " + ex.Message);
            }

            CargaDatos();
            Limpiar();
            ModoVista();
        }

        private void btn_cancelar_Click(object sender, EventArgs e)
        {
            ModoVista();
            Limpiar();
        }
    }
}
