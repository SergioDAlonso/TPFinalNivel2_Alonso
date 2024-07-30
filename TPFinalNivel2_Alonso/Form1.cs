using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AccesoDB;
using Dominio;

namespace TPFinalNivel2_Alonso
{
    public partial class Form1 : Form
    {
        private List<Articulo> listaArticulo;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cargar();
            cbxCampo.Items.Add("Precio");
            cbxCampo.Items.Add("Marca");
            cbxCampo.Items.Add("Categoria");  
        }
        private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                listaArticulo = negocio.listar();
                dgvArticulos.DataSource = listaArticulo;
                ocultarColumnas();
                cargarImagen(listaArticulo[0].UrlImagen);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
    
        }
        private void ocultarColumnas()
        {
            dgvArticulos.Columns["Id"].Visible = false;
            dgvArticulos.Columns["UrlImagen"].Visible = false;
        }
        private void cargarImagen(string imagen)
        {
            try
            {
                pbxImagen.Load(imagen);
            }
            catch (Exception)
            {
                pbxImagen.Load("https://t3.ftcdn.net/jpg/02/48/42/64/360_F_248426448_NVKLywWqArG2ADUxDq6QprtIzsF82dMF.jpg");
            }
        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                Articulo selecionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                cargarImagen(selecionado.UrlImagen);
            }
        }

        private void bttSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Contacte a Atencion a Clientes. Disculpe las molestias.");
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bttAgregar_Click(object sender, EventArgs e)
        {
            Alta alta = new Alta();
            alta.ShowDialog();
            cargar();
        }

        private void bttModificar_Click(object sender, EventArgs e)
        {
            if (validarBoton())
                return;
            Articulo seleccionado;
            seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
            Alta Modificar = new Alta(seleccionado);
            Modificar.ShowDialog();
            cargar();
        }

        private void bttEliFis_Click(object sender, EventArgs e)
        {
            if (validarBoton())
                return;
            eliminar();
        }

        private void bttEliLog_Click(object sender, EventArgs e)
        {
            if (validarBoton())
                return;
            eliminar(true);
        }
        private void eliminar (bool logico = false)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            Articulo seleccionado;
            try
            {
                DialogResult resultado = MessageBox.Show("Desea eliminarlo?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Hand);
                if (resultado == DialogResult.Yes)
                {
                    seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                    if (logico)
                        negocio.eliminarLogico(seleccionado.Id);
                    else
                        negocio.eliminar(seleccionado.Id);
                    MessageBox.Show("Eliminado.");
                    cargar();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void txtFiltroSimple_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("Filtro Simple para Codigo o Nombre del articulo, para mayor precision utilice el Filtro Avanzado");
        }

        private void txtFiltroSimple_TextChanged(object sender, EventArgs e)
        {
            List<Articulo> filtrada;
            string FiltroSimple = txtFiltroSimple.Text;
            if (FiltroSimple.Length >= 3)
                filtrada = listaArticulo.FindAll(x => x.Nombre.ToUpper().Contains(FiltroSimple.ToUpper()) || x.Codigo.ToUpper().Contains(FiltroSimple.ToUpper()));
            else
                filtrada = listaArticulo;

            dgvArticulos.DataSource = null;
            dgvArticulos.DataSource = filtrada;
            ocultarColumnas();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cbxCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cbxCampo.SelectedItem.ToString();
            if(opcion == "Precio")
            {
                cbxCriterio.Items.Clear();
                cbxCriterio.Items.Add("Mayor a");
                cbxCriterio.Items.Add("Menor a");
                cbxCriterio.Items.Add("Igual a");

            }
            else
            {
                cbxCriterio.Items.Clear();
                cbxCriterio.Items.Add("Empieza con");
                cbxCriterio.Items.Add("Termina con");
                cbxCriterio.Items.Add("Contiene");
            }
        }
        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
            return true;
        }
        private bool validarFiltro()
        {
            if (cbxCampo.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un campo");
                return true;
            }
            if (cbxCriterio.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un criterio");
                return true;
            }
            if (cbxCampo.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanz.Text))
                {
                    MessageBox.Show("Coloque en la caja 'FiltroAvanzado' el valor numerico buscado.");
                        return true;
                }
                if (!(soloNumeros(txtFiltroAvanz.Text)))
                {
                    MessageBox.Show("Solo Numeros");
                    return true;
                }
            }
            return false;
        }
        private void bttBuscar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                if (validarFiltro())
                    return;
                string campo = cbxCampo.SelectedItem.ToString();
                string criterio = cbxCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanz.Text;
                dgvArticulos.DataSource = negocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {
                //throw ex;
                MessageBox.Show(ex.ToString());
            }
        }
        private bool validarBoton()
        {
            if (dgvArticulos.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un elemento");
                return true;
            }
            return false;
        }
    }
}
