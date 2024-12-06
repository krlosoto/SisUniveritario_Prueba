using DevExpress.Web;
using DevExpress.Web.Bootstrap;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;

namespace SISTEMA_UNIVERSITARIO.InfraestructuraSQL
{
    public class Infraestructura
    {
        private readonly string connectionString;
        public Infraestructura()
        {
            //Para pasarle la cadena de conexion.
            connectionString = ConfigurationManager.ConnectionStrings["SISTEMAUNIVERSITARIOConnectionString"].ConnectionString;
        }


        // Metodo para recuperar el resultado de SQL como lista, por ejemplo muchos datos para una tabla.
        public List<T> ObtenerListaSQL<T>(string nombreSP) where T : new()
        {
            var registros = new List<T>();
            try
            {
                using (var conexion = new SqlConnection(connectionString))
                {
                    using (var comando = new SqlCommand(nombreSP, conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        using (var adapter = new SqlDataAdapter(comando))
                        {
                            var tabla = new DataTable();
                            adapter.Fill(tabla);

                            foreach (DataRow fila in tabla.Rows)
                            {
                                var registro = new T();

                                foreach (var propiedad in typeof(T).GetProperties())
                                {
                                    if (tabla.Columns.Contains(propiedad.Name) && fila[propiedad.Name] != DBNull.Value)
                                    {
                                        propiedad.SetValue(registro, fila[propiedad.Name]);
                                    }
                                }

                                registros.Add(registro);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }


            return registros;
        }//Fin metodo ObtenerListaSQL

        /*Metodo que me retorna una lista luego de nviar un parametro*/
        public List<T> ListaObtenerSQL<T>(string nombreSP, params SqlParameter[] parametros) where T : new()
        {
            var registros = new List<T>();
            try
            {
                using (var conexion = new SqlConnection(connectionString))
                {
                    using (var comando = new SqlCommand(nombreSP, conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;

                        if (parametros != null && parametros.Length > 0)
                        {
                            comando.Parameters.AddRange(parametros);
                        }

                        using (var adapter = new SqlDataAdapter(comando))
                        {
                            var tabla = new DataTable();
                            adapter.Fill(tabla);

                            foreach (DataRow fila in tabla.Rows)
                            {
                                var registro = new T();

                                foreach (var propiedad in typeof(T).GetProperties())
                                {
                                    if (tabla.Columns.Contains(propiedad.Name) && fila[propiedad.Name] != DBNull.Value)
                                    {
                                        propiedad.SetValue(registro, fila[propiedad.Name]);
                                    }
                                }

                                registros.Add(registro);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return registros;
        }



        //Metodo para obtener datos despues de enviar parametros, puede ser un ID o Varios parametros y retorna una fila con todos los datos deseados.
        public T ObtenerSQL<T>(string nombreSP, Dictionary<string, object> parametros)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(nombreSP, conexion);
                command.CommandType = CommandType.StoredProcedure;

                foreach (var parameter in parametros)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }

                conexion.Open();

                SqlDataReader reader = command.ExecuteReader();

                T resultado = default(T);

                if (reader.Read())
                {
                    resultado = Activator.CreateInstance<T>();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                        {
                            property.SetValue(resultado, reader.GetValue(reader.GetOrdinal(property.Name)), null);
                        }
                    }
                }

                reader.Close();

                return resultado;
            }
        }

        //Obtiene cualquier tipo despues de enviarle parametros, sirve para un Update, Insert o Delete, Y ESTE METODO ACEPTA PARAMETROS NULOS PERO SE CONSTRUYEN DE MANERA DIFERENTE.
        public T EjecutarSQL<T>(string nombreSP, SqlParameter[] parametros)
        {
            T resultado = default(T);

            using (var conexion = new SqlConnection(connectionString))
            {
                using (var comando = new SqlCommand(nombreSP, conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;

                    if (parametros != null)
                    {
                        foreach (var parametro in parametros)
                        {
                            // Verifica si el valor del parámetro es null y cambia a DBNull.Value
                            if (parametro.Value == null)
                            {
                                parametro.Value = DBNull.Value;
                            }
                        }
                        comando.Parameters.AddRange(parametros);
                    }

                    conexion.Open();
                    var valor = comando.ExecuteScalar();

                    if (valor != DBNull.Value && valor != null)
                    {
                        resultado = (T)Convert.ChangeType(valor, typeof(T));
                    }
                }
            }

            return resultado;
        }


        //public T EjecutarSQL<T>(string nombreSP, SqlParameter[] parametros)
        //{
        //    T resultado = default(T);

        //    using (var conexion = new SqlConnection(connectionString))
        //    {
        //        using (var comando = new SqlCommand(nombreSP, conexion))
        //        {
        //            comando.CommandType = CommandType.StoredProcedure;

        //            if (parametros != null)
        //            {
        //                comando.Parameters.AddRange(parametros);
        //            }

        //            conexion.Open();
        //            var valor = comando.ExecuteScalar();

        //            if (valor != DBNull.Value)
        //            {
        //                resultado = (T)Convert.ChangeType(valor, typeof(T));
        //            }
        //        }
        //    }

        //    return resultado;
        //}


        //METODO PARA ENVIAR ID Y RETORNAR DATOS DE ESE ID O ENVIAR VARIOS PARAMETROS E IGUALMENTE RETORNAR TODO LOS DATOS DE LO REQUERIDO

        //Metodo para recuperar el valor retornado pero debe ser un entero unicamente
        public int EjecutarSQL2(string nombreSP, SqlParameter[] parametros)
        {
            int resultado = 0;

            using (var conexion = new SqlConnection(connectionString))
            {
                using (var comando = new SqlCommand(nombreSP, conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;

                    if (parametros != null)
                    {
                        comando.Parameters.AddRange(parametros);
                    }

                    conexion.Open();
                    resultado = (int)comando.ExecuteScalar();
                }
            }

            return resultado;
        }

        //METODO GUARDAR ARCHIVO IMAGENES O LOGOS (TEMPORAL POR NECESIDAD)
        public string NombreAdjuntoLogos(ASPxUploadControl adjunto, string IdInstitucion, string nombreArchivo)
        {
            string Obtenido = "";
            string fileName;

            try
            {
                string filepath = "C:/ArchivosMICI/Logos de Instituciones/Logos-" + IdInstitucion + "/";
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                if (adjunto.ID == "uploadLogoInst")
                {
                    fileName = "LogoInstitucional-" + nombreArchivo;
                }
                else
                {
                    fileName = "Logo-" + nombreArchivo;
                }
                adjunto.UploadedFiles[0].SaveAs(filepath + fileName);

                Obtenido = fileName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("error: " + ex);
            }

            return Obtenido;

        }

        public int EliminarArchivo(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return 1;
                }
                else
                {
                    Debug.WriteLine("El archivo que se quiere eliminar no existe.");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Se produjo un error, contacte con soporte técnico." + ex.Message);
                return 406;
            }
        }

        public string GetImagenUrl(object nombreImagen, string rutaAccesoCarpeta)
        {
            // Ruta de la carpeta de imágenes en tu PC
            string carpetaImagenes = rutaAccesoCarpeta; /*"C:/Ruta/De/La/Carpeta/De/Imagenes";*/

            // Obtener el nombre de la imagen como una cadena
            string nombreImagenStr = nombreImagen as string;

            // Verificar si el nombre de la imagen no es nulo ni vacío
            if (!string.IsNullOrEmpty(nombreImagenStr))
            {
                // Crear la ruta completa de la imagen
                string rutaImagenCompleta = Path.Combine(carpetaImagenes, nombreImagenStr);

                // Reemplazar la ruta física de la carpeta de la aplicación por "/"
                string rutaFisicaApp = HostingEnvironment.MapPath("~/");
                string urlImagen = rutaImagenCompleta.Replace(rutaFisicaApp, "/");

                //Si este metodo se utiliza con ASP.NET MVC o ASP.NET CORE entonces utilizar la siguiente linea y quitar HostingEnviroment:
                //string urlImagen = rutaImagenCompleta.Replace(Server.MapPath("~/"), "/");

                // Devolver la URL de la imagen generada
                return urlImagen;
            }

            // Si el nombre de la imagen es nulo o vacío, devolver una cadena vacía
            return string.Empty;
        }


        public string NombreAdjuntoDocs(BootstrapUploadControl adjunto, string tipo, int? idRuta = null, int? idOficio = null)
        {
            string Obtenido = "";

            try
            {
                if (tipo == "unidades")
                {
                    string filepath = "C:/ArchivosSIGECOR/Oficios/" + "Oficio-" + idOficio + "/";
                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);

                    }
                    string fileName = string.Format("{1}-{0}", "Oficio-" + idOficio + ".pdf", Guid.NewGuid().ToString().Split('-')[0]);

                    adjunto.UploadedFiles[0].SaveAs(filepath + fileName);

                    Obtenido = fileName;
                }
                else
                {
                    string filepath = "C:/ArchivosSIGECOR/DocumentosDeRespuestas/" + "adjuntoDeRespuesta-" + idRuta + "/";
                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);

                    }
                    string fileName = string.Format("{1}-{0}", "adjunto-" + idRuta + ".docx", Guid.NewGuid().ToString().Split('-')[0]);

                    adjunto.UploadedFiles[0].SaveAs(filepath + fileName);

                    Obtenido = fileName;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("error: " + ex);
            }

            return Obtenido;

        }

        public void DescargarArchivos(string idRuta, string nombreDocumento, HttpResponse response)
        {
            string filepath = $"path o directorio-{idRuta}/{nombreDocumento}";

            if (File.Exists(filepath))
            {
                response.Clear();
                response.ContentType = "application/octet-stream";
                response.AppendHeader("Content-Disposition", $"attachment; filename={Path.GetFileName(filepath)}");
                response.TransmitFile(filepath);
                response.End();
            }
            else
            {
                // Manejo de errores si el archivo no existe
                response.Write("<script>alert('El archivo no existe.');</script>");
            }
        }

        public string nombreAdjuntosAnexos(int idMesa, string tipoMesa, ASPxUploadControl adjunto)
        {
            string nombreArchivo = "";
            try
            {
                string filePath = "C:/ArchivosMICI/Anexos/" + tipoMesa + "/" + "Anexo mesa" + "-" + idMesa + "/";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string fileName = string.Format("{1}-{0}", "Anexo Mesas-" + idMesa + ".jpg", Guid.NewGuid().ToString().Split('-')[0]);
                adjunto.UploadedFiles[0].SaveAs(filePath + fileName);
                nombreArchivo = fileName;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("error: " + ex);
            }

            return nombreArchivo;

        }

        //METODO PARA EDITAR LAS FOTOS DE ANEXOS
        public int fotoAnexoEdicion(string tipoMesa, int idMesa, string nombreArchivo, ASPxUploadControl adjunto)
        {
            int resultado = 0;
            try
            {
                string filePath = "C:/ArchivosMICI/Anexos/" + tipoMesa + "/" + "Anexo mesa" + "-" + idMesa + "/";
                if (!Directory.Exists(filePath))
                {
                    return resultado;
                }
                else
                {
                    var archivoSubido = adjunto.UploadedFiles[0].FileName.ToString();
                    archivoSubido = string.Format(nombreArchivo);
                    adjunto.UploadedFiles[0].SaveAs(filePath + archivoSubido);
                    return 1;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("error: " + ex);
                return 0;
            }
        }

        //Metodo para setear el valor que viene desde base de datos y que pertenece a un combobox
        public void SeleccionarItemPorValor(BootstrapComboBox comboBox, string valor)
        {
            foreach (BootstrapListEditItem item in comboBox.Items)
            {
                if (item.Value.ToString() == valor)
                {
                    item.Selected = true;
                    break;
                }
            }
        }

        public T ObtenerValorSP<T>(string nombreSP)
        {
            T resultado = default(T);

            using (var conexion = new SqlConnection(connectionString))
            {
                using (var comando = new SqlCommand(nombreSP, conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;

                    conexion.Open();
                    var valor = comando.ExecuteScalar();

                    if (valor != DBNull.Value)
                    {
                        resultado = (T)Convert.ChangeType(valor, typeof(T));
                    }
                }
            }

            return resultado;
        }

        public void EnviarCorreoLocal(string Para, string Asunto, string Cuerpo)
        {
            //UTILIZAR EL SOFTWARE PAPERCUT PARA PRUEBAS EN LOCAL
            string usuario = "laksjdflkajl@lkjalksdf.cv";
            string password = "gggg";
            var loginInfo = new System.Net.NetworkCredential(usuario, password);
            var msg = new MailMessage();
            //var smtpClient = new SmtpClient("smtp.office365.com", 587);
            var smtpClient = new SmtpClient("localhost", 25);

            //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            msg.From = new MailAddress(usuario);
            msg.To.Add(new MailAddress(Para));
            msg.Subject = Asunto;
            msg.Body = Cuerpo;
            msg.IsBodyHtml = true;
            //smtpClient.EnableSsl = true;
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            smtpClient.Credentials = loginInfo;

            try
            {
                smtpClient.Send(msg);
                // El correo se ha enviado correctamente, puedes agregar cualquier acción adicional aquí si es necesario.
            }
            catch (Exception ex)
            {
                // Ocurrió un error al enviar el correo, puedes registrar o manejar el error aquí.
                // Si no deseas hacer nada en caso de error, simplemente puedes no incluir ningún código dentro del bloque catch.
                // El flujo del programa continuará sin interrupciones.
                Debug.WriteLine("Error al envia correo:" + ex.ToString());
                //Page.ClientScript.RegisterStartupScript(typeof(Page), "script", "alertError(' .')", true);
            }
        }

        //===========CODIGO PARA CONFIGURAR EN MODO DE SERVIDOR==============

        //var msg = new MailMessage();
        //var smtpClient = new SmtpClient("smtp.office365.com", 587);
        ////var smtpClient = new SmtpClient("localhost", 25);

        //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    msg.From = new MailAddress(usuario);
        //msg.To.Add(new MailAddress(Para));
        //    msg.Subject = Asunto;
        //    msg.Body = Cuerpo;
        //    msg.IsBodyHtml = true;
        //    smtpClient.EnableSsl = true;
        //    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //    smtpClient.Credentials = loginInfo;

        //    try
        //    {
        //        smtpClient.Send(msg);
        //        // El correo se ha enviado correctamente, puedes agregar cualquier acción adicional aquí si es necesario.
        //    }
        //    catch (Exception ex)
        //    {
        //        // Ocurrió un error al enviar el correo, puedes registrar o manejar el error aquí.
        //        // Si no deseas hacer nada en caso de error, simplemente puedes no incluir ningún código dentro del bloque catch.
        //        // El flujo del programa continuará sin interrupciones.
        //        Debug.WriteLine("Error al envia correo:" + ex.ToString());
        //        //Page.ClientScript.RegisterStartupScript(typeof(Page), "script", "alertError(' .')", true);
        //    }


        //METODOS GENERICOS ENVIANDO QUERY EN STRING Y NO CON SP

        /*Metodo que me retorna una lista luego de enviar un parametro*/
        public List<T> ObtenerListaSQLNoSP<T>(string consultaSQL, SqlParameter[] parametros) where T : new()
        {
            var registros = new List<T>();
            try
            {
                using (var conexion = new SqlConnection(connectionString))
                {
                    using (var comando = new SqlCommand(consultaSQL, conexion))
                    {
                        // Indicar que es una consulta de texto
                        comando.CommandType = CommandType.Text;

                        // Agregar los parámetros a la consulta
                        if (parametros != null)
                        {
                            comando.Parameters.AddRange(parametros);
                        }

                        using (var adapter = new SqlDataAdapter(comando))
                        {
                            var tabla = new DataTable();
                            adapter.Fill(tabla);

                            foreach (DataRow fila in tabla.Rows)
                            {
                                var registro = new T();

                                foreach (var propiedad in typeof(T).GetProperties())
                                {
                                    if (tabla.Columns.Contains(propiedad.Name) && fila[propiedad.Name] != DBNull.Value)
                                    {
                                        propiedad.SetValue(registro, fila[propiedad.Name]);
                                    }
                                }

                                registros.Add(registro);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return registros;
        }

        public T ObtenerSQLNoSP<T>(string consultaSQL, SqlParameter[] parametros)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(consultaSQL, conexion);
                command.CommandType = CommandType.Text;

                if (parametros != null)
                {
                    command.Parameters.AddRange(parametros);
                }

                conexion.Open();

                SqlDataReader reader = command.ExecuteReader();

                T resultado = default(T);

                if (reader.Read())
                {
                    resultado = Activator.CreateInstance<T>();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                        {
                            property.SetValue(resultado, reader.GetValue(reader.GetOrdinal(property.Name)), null);
                        }
                    }
                }

                reader.Close();

                return resultado;
            }
        }

        public T EjecutarSQLNoSP<T>(string consulta, SqlParameter[] parametros)
        {
            T resultado = default(T);

            using (var conexion = new SqlConnection(connectionString))
            {
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    if (parametros != null)
                    {
                        foreach (var parametro in parametros)
                        {
                            // Verifica si el valor del parámetro es null y cambia a DBNull.Value
                            if (parametro.Value == null)
                            {
                                parametro.Value = DBNull.Value;
                            }
                        }
                        comando.Parameters.AddRange(parametros);
                    }

                    conexion.Open();
                    var valor = comando.ExecuteScalar();

                    if (valor != DBNull.Value && valor != null)
                    {
                        resultado = (T)Convert.ChangeType(valor, typeof(T));
                    }
                }
            }

            return resultado;
        }


        /*=====FIN METODOS IMPLEMENTACIONES POR CARLOS SOTO*/

    }
}