﻿using Dominio;
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
using System.IO;
using System.Configuration;

namespace TPFinalNivel2_Alonso
{
    public partial class Alta : Form
    {
        private Articulo articulo = null;
        private OpenFileDialog archivo = null;
        public Alta()
        {
            InitializeComponent();
        }
        public Alta(Articulo articulo)
        {
            InitializeComponent();
            this.articulo = articulo;
            Text = "Modificar Articulo";
        }
        private void bttCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bttAceptar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                if (!(datosRequeridos()))
                {
                    MessageBox.Show("Complete los campos requeridos.");
                    return;
                }
                if (articulo == null)
                    articulo = new Articulo();
                articulo.Codigo = txtCodigo.Text;
                articulo.Nombre = txtNombre.Text;
                articulo.Descripcion = txtDescripcion.Text;
                articulo.Marca = (Marca)cbxMarca.SelectedItem;
                articulo.Categoria = (Categoria)cbxCategoria.SelectedItem;
                articulo.UrlImagen = txtImagen.Text;
                articulo.Precio = numPrecio.Value;
                
                if(articulo.Id != 0)
                {
                    negocio.modificar(articulo);
                    MessageBox.Show("Articulo modificado.");
                }
                else
                {
                    negocio.agregar(articulo);
                    MessageBox.Show("Articulo agregado.");
                }
                if (archivo != null && !(txtImagen.Text.ToUpper().Contains("HTTP")));
                File.Copy(archivo.FileName, ConfigurationManager.AppSettings["imagen-folder"] + archivo.SafeFileName);

                Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void Alta_Load(object sender, EventArgs e)
        {
            MarcaNegocio marcaNegocio = new MarcaNegocio();
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();
            pbxAlta.Load("https://t3.ftcdn.net/jpg/02/48/42/64/360_F_248426448_NVKLywWqArG2ADUxDq6QprtIzsF82dMF.jpg");
            try
            {
                cbxMarca.DataSource = marcaNegocio.listar();
                cbxMarca.ValueMember = "Id";
                cbxMarca.DisplayMember = "Descripcion";
                cbxCategoria.DataSource = categoriaNegocio.listar();
                cbxCategoria.ValueMember = "Id";
                cbxCategoria.DisplayMember = "Descripcion";

                if (articulo != null)
                {
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text = articulo.Nombre;
                    txtDescripcion.Text = articulo.Descripcion;
                    txtImagen.Text = articulo.UrlImagen;
                    cargarImagen(articulo.UrlImagen);
                    cbxMarca.SelectedValue = articulo.Marca.Id;
                    cbxCategoria.SelectedValue = articulo.Categoria.Id;
                    numPrecio.Value = articulo.Precio;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void txtImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtImagen.Text);
        }
        private void cargarImagen(string imagen)
        {
            try
            {
                pbxAlta.Load(imagen);
            }
            catch (Exception)
            {
                pbxAlta.Load("https://t3.ftcdn.net/jpg/02/48/42/64/360_F_248426448_NVKLywWqArG2ADUxDq6QprtIzsF82dMF.jpg");
            }
        }

        private void bttImagenLocal_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg|png|*.png";
            if(archivo.ShowDialog() == DialogResult.OK)
            {
                txtImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);
            }
        }
        private bool datosRequeridos()
        {
            int requeridos = 0;

            if (txtCodigo.Text == "")
                txtCodigo.BackColor = Color.Red;
            else
            {
                txtCodigo.BackColor = System.Drawing.SystemColors.Control;
                requeridos++;
            }
            if (txtNombre.Text == "")
                txtNombre.BackColor = Color.Red;
            else
            {
                txtNombre.BackColor = System.Drawing.SystemColors.Control;
                requeridos++;
            }
            if (numPrecio.Value == 0)
                numPrecio.BackColor = Color.Red;
            else
            {
                numPrecio.BackColor = System.Drawing.SystemColors.Control;
                requeridos++;
            }
            if (requeridos == 3)
                return true;
            else
            {
                return false;
            }
                            
        }
    }
}
