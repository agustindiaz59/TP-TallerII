using Gestion_Gym.Servicios.Persistencia;
using Gestion_Gym.Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;

namespace Gestion_Gym.Vistas
{
    public partial class DashboardModerno : Form
    {
        private MiembroDAO miembroDAO = new MiembroDAO();
        private PersonalDAO personalDAO = new PersonalDAO();

        public DashboardModerno()
        {
            InitializeComponent();
            this.Load += new EventHandler(DashboardModerno_Load);
        }

        private void DashboardModerno_Load(object sender, EventArgs e)
        {
            RefrescarTodo();
        }

        public void RefrescarTodo()
        {
            try
            {
                // 1. Cargar listas desde DAOs
                List<Miembro> listaMiembros = miembroDAO.TraerTodos();
                List<Personal> listaPersonal = personalDAO.TraerTodos();
                DateTime hoy = DateTime.Today;

                int contActivos = 0;
                int contVencidos = 0;
                int contNuevos = 0;

                // Lista temporal para filtrar los nuevos del mes y mostrar en la grilla
                List<Miembro> miembrosNuevosLista = new List<Miembro>();

                foreach (var m in listaMiembros)
                {
                    // Lógica de Vencimiento (Tarjetas)
                    if (DateTime.TryParse(m.FVencimiento, out DateTime fechaVenc))
                    {
                        if (hoy > fechaVenc) contVencidos++;
                        else contActivos++;
                    }

                    // Lógica para Nuevos del Mes (inscritos hace menos de 30 días)
                    if (DateTime.TryParse(m.FIngreso, out DateTime fechaAlta))
                    {
                        if ((hoy - fechaAlta).Days <= 30)
                        {
                            contNuevos++;
                            miembrosNuevosLista.Add(m); // Lo agregamos a la lista para la grilla
                        }
                    }
                }

                // 2. Actualizar Tarjetas (Labels)
                lblValor1.Text = listaPersonal.Count.ToString();
                lblValor3.Text = listaMiembros.Count.ToString();
                lblValor4.Text = contNuevos.ToString();

                // 3. Cargar dgvNuevosMES (La tabla de arriba)
                dgvNuevosMES.DataSource = null;
                dgvNuevosMES.DataSource = miembrosNuevosLista.Select(m => new {
                    m.DNI,
                    m.Nombre,
                    m.Apellido,
                    FechaIngreso = m.FIngreso,
                    Estado = "Nuevo"
                }).ToList();

                // 4. Cargar dgvInactivosPersonal (La tabla de abajo)
                dgvInactivosPersonal.DataSource = null;
                dgvInactivosPersonal.DataSource = listaPersonal.Select(p => new {
                    p.Cuil,
                    p.Nombre,
                    p.Apellido,
                    p.Telefono
                }).ToList();

                // 5. Aplicar Estilos a ambas grillas
                EstilizarGrilla(dgvNuevosMES);
                EstilizarGrilla(dgvInactivosPersonal);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }
        }

        private void EstilizarGrilla(DataGridView dgv)
        {
            if (dgv != null)
            {
                dgv.AutoGenerateColumns = true;
                dgv.ReadOnly = true;
                dgv.RowHeadersVisible = false;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.BackgroundColor = Color.White;
                dgv.AllowUserToAddRows = false;
                dgv.BorderStyle = BorderStyle.None;

                dgv.EnableHeadersVisualStyles = false;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgv.ColumnHeadersHeight = 30;

                dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9);
                dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            }
        }

        // Eventos para evitar errores en el Designer
        private void lblTitulo1_Click(object sender, EventArgs e) { }
        private void lblTitulo4_Click(object sender, EventArgs e) { }

        private void lblActivosTitulo_Click(object sender, EventArgs e)
        {

        }
    }
}