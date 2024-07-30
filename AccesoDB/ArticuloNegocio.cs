using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dominio;

namespace AccesoDB
{
    public class ArticuloNegocio
    {
        public List<Articulo> listar()
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("select A.Id, Codigo, Nombre, A.Descripcion Descripcion, ImagenUrl, Precio, M.Descripcion Marca, C.Descripcion Categoria, A.IdMarca, A.IdCategoria from ARTICULOS A, MARCAS M, CATEGORIAS C where M.Id = IdMarca AND C.Id = IdCategoria");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                    {
                        aux.UrlImagen = (string)datos.Lector["ImagenUrl"];
                    }
                    aux.Precio = (decimal)datos.Lector["Precio"];
                    aux.Marca = new Marca();
                    aux.Marca.Id = (int)datos.Lector["IdMarca"];
                    aux.Marca.Descripcion = (string)datos.Lector["Marca"];
                    aux.Categoria = new Categoria();
                    aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];

                    lista.Add(aux);
                }
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void agregar(Articulo Nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("insert into articulos (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, ImagenUrl, Precio) values (@codigo, @nombre, @descripcion, @idMarca, @idCategoria, @ImagenUrl, @precio)");
                datos.setearParametros("@codigo", Nuevo.Codigo);
                datos.setearParametros("@nombre", Nuevo.Nombre);
                datos.setearParametros("@descripcion", Nuevo.Descripcion);
                datos.setearParametros("@idMarca", Nuevo.Marca.Id);
                datos.setearParametros("@idCategoria", Nuevo.Categoria.Id);
                datos.setearParametros("@ImagenUrl", Nuevo.UrlImagen);
                datos.setearParametros("@precio", Nuevo.Precio);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        public void modificar(Articulo art)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("update ARTICULOS set Codigo = @codigo, Nombre = @nombre, Descripcion = @descripcion, IdMarca = @idMarca, IdCategoria = @idCategoria, ImagenUrl = @ImagenUrl, Precio = @precio where Id = @Id");
                datos.setearParametros("@codigo", art.Codigo);
                datos.setearParametros("@nombre", art.Nombre);
                datos.setearParametros("@descripcion", art.Descripcion);
                datos.setearParametros("@idMarca", art.Marca.Id);
                datos.setearParametros("@idCategoria", art.Categoria.Id);
                datos.setearParametros("@ImagenUrl", art.UrlImagen);
                datos.setearParametros("@precio", art.Precio);
                datos.setearParametros("@Id", art.Id);

                datos.ejecutarAccion();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        public void eliminar(int Id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("delete from ARTICULOS where Id = @Id");
                datos.setearParametros("@Id", Id);
                datos.ejecutarAccion();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void eliminarLogico(int Id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                // Como la DB no tiene parametro "Activo" , le cambio el valor de 
                // IdCategoria para que no me lo muestre en dgvArticulos.
                datos.setearConsulta("update ARTICULOS set IdCategoria = 0 where Id = @Id");
                datos.setearParametros("@Id", Id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            
            try
            {
                string consulta = "select A.Id, Codigo, Nombre, A.Descripcion Descripcion, ImagenUrl, Precio, M.Descripcion Marca, C.Descripcion Categoria, A.IdMarca, A.IdCategoria from ARTICULOS A, MARCAS M, CATEGORIAS C where M.Id = IdMarca And C.Id = IdCategoria And ";
                switch (campo)
                {
                    case "Precio":
                        {
                            switch (criterio)
                            {
                                case "Mayor a":
                                    consulta += "Precio > " + filtro;
                                    break;
                                case "Menor a":
                                    consulta += "Precio < " + filtro;
                                    break;
                                default:
                                    consulta += "Precio = " + filtro;
                                    break;
                            }
                            break;
                        }
                    case "Marca":
                        {
                            switch (criterio)
                            {
                                case "Empieza con":
                                    consulta += "M.Descripcion like '" + filtro + "%'";
                                    break;
                                case "Termina con":
                                    consulta += "M.Descripcion like '%" + filtro + "'";
                                    break;
                                default:
                                    consulta += "M.Descripcion like '%" + filtro + "%'";
                                    break;
                            }
                            break;
                        }
                    case "Categoria":
                        {
                            switch (criterio) 
                            {
                                case "Empieza con":
                                    consulta += "C.Descripcion like '" + filtro + "%'";
                                    break;
                                case "Termina con":
                                    consulta += "C.Descripcion like '%" + filtro + "'";
                                    break;
                                default:
                                    consulta += "C.Descripcion like '%" + filtro + "%'";
                                    break;
                            }
                            break;
                        }
                }
                datos.setearConsulta(consulta);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                    {
                        aux.UrlImagen = (string)datos.Lector["ImagenUrl"];
                    }
                    aux.Precio = (decimal)datos.Lector["Precio"];
                    aux.Marca = new Marca();
                    aux.Marca.Id = (int)datos.Lector["IdMarca"];
                    aux.Marca.Descripcion = (string)datos.Lector["Marca"];
                    aux.Categoria = new Categoria();
                    aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
