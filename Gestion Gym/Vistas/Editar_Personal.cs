using Gestion_Gym.Modelos;
using Gestion_Gym.Servicios;
using Gestion_Gym.Servicios.Persistencia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gestion_Gym.Vistas
{
    public partial class Editar_Personal : Nuevo_Personal
    {
        protected PersonalDAO personalDAO = new PersonalDAO();
        protected Personal personal;
        public Editar_Personal(string CUIL, Buscar_Personal buscarPersonal)
        {
            InitializeComponent();
            txtNombre.Focus();
            dateTimePickerFNacim.MaxDate = DateTime.Now.AddMinutes(1);


            this.textBox1.Enabled = false;
            this.label11.Text = "Editar Personal";

            //Iniciar Persona existente
            personal = personalDAO.Traer(CUIL);

            if (personal == null)
            {
                MessageBox.Show($"No se encontró personal con CUIL {CUIL}");
                this.Close(); // o lanzar excepción controlada / devolver al formulario padre
                return;
            }

            //Iniciar variables de autocompletado
            txtNombre.Text = personal.Nombre;
            txtApellido.Text = personal.Apellido;
            textBox1.Text = personal.Cuil;
            txtEmail.Text = personal.Email;
            txtCalle.Text = personal.Direccion;
            txtLocalidad.Text = personal.Direccion;
            txtProvincia.Text = personal.Direccion;
            txtTelefono.Text = personal.Telefono;

            dateTimePickerFNacim.Value = DateTime.Parse(personal.FNacimiento);
            dateTimePickerFIngreso.Value = DateTime.Parse(personal.FIngreso);
        }

        private void Editar_Personal_Load(object sender, EventArgs e)
        {

        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            //iniciar Personal con los valores nuevos
            // 1) Tomar valores
            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string cuil = textBox1.Text.Trim();

            char genero = radioButtonMasculino.Checked ? 'M' : 'F';

            string fnacim = dateTimePickerFNacim.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string email = txtEmail.Text.Trim();
            string fingreso = dateTimePickerFIngreso.Text.Trim();

            string calle = txtCalle.Text.Trim();
            string localidad = txtLocalidad.Text.Trim();
            string provincia = txtProvincia.Text.Trim();
            string direccion = $"{calle}, {localidad}, {provincia}";

            // 2) Validaciones
            bool validarDatos =
                Validacion.ValidarCadenaEstandar(nombre, "Nombre");
            bool val8 =
                Validacion.ValidarCadenaEstandar(apellido, "Apellido");
            bool val2 =
                Validacion.ValidarCuil(cuil, "Cuil");
            bool val5 =
                Validacion.ValidarFecha(fnacim, "Fecha de nacimiento");
            bool val3 =
                Validacion.ValidarCelular(telefono, "Telefono");
            bool val6 =
                Validacion.ValidarEmail(email, "Email");
            bool val4 =
                Validacion.ValidarFecha(fingreso, "Fecha de ingreso");

            //MessageBox.Show(
            //    nombre + Validacion.ValidarCadenaEstandar(nombre) +
            //    apellido + Validacion.ValidarCadenaEstandar(apellido) +
            //    cuil + Validacion.ValidarCuil(cuil) +
            //    fnacim + Validacion.ValidarFecha(fnacim) +
            //    telefono + Validacion.ValidarCelular(telefono) +
            //    email + Validacion.ValidarEmail(email) +
            //    fingreso + Validacion.ValidarFecha(fingreso) +
            //    direccion + Validacion.ValidarCadenaEstandar(direccion) 
            //    );

            if (validarDatos && val2 && val3 && val4 && val5 && val6 && val8)
            {
                //GuardarPersonal();

                // 3) Construir entidad
                var nuevo = new Personal(
                    nombre,
                    apellido,
                    cuil,
                    fnacim,
                    genero,
                    telefono,
                    email,
                    direccion,
                    fingreso
                );

                // 4) Guardar con control de duplicado (PersonalDAO.Guardar devuelve 0 si CUIL existe)
                try
                {
                    int resultado = personalDAO.Editar(nuevo);

                    if (resultado > 0)
                    {
                        MessageBox.Show("Datos guardados correctamente.");
                        //LimpiarCampos();
                    }
                    else
                    {
                        MessageBox.Show("CUIL duplicado. No se guardaron los datos.");
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error al guardar: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Verifique que los datos sean correctos");
            }


        }
    }
}
