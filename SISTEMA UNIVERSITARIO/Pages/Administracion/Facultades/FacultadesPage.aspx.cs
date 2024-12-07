using SISTEMA_UNIVERSITARIO.InfraestructuraSQL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using DevExpress.Web;
using System.Web.UI.WebControls;

namespace SISTEMA_UNIVERSITARIO.Pages.Administracion.Facultades
{
    public partial class FacultadesPage : System.Web.UI.Page
    {
        Infraestructura infraestructura = new Infraestructura();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGuardarFacu_Click(object sender, EventArgs e)
        {
            var sp = "SP_INSERTAFACULTADES";
            var nombreFacultad = txtNombreFacultad.Text;
           

            try
            {
                //Contruyo los parametros que enviare a mi SP, mapearlos igual a como estan en el SP.
                var param = new SqlParameter[]
                  {
                        new SqlParameter("@COD_FACULTAD",0),
                        new SqlParameter("@NOMBRE",nombreFacultad),
                        new SqlParameter("@COD_SUCURSAL", 0),
                         new SqlParameter("@TIPO",1) //ES UNO POR LOGICA IMPLEMENTADA EN SP, SIGINIFICA QUE SOLO INSERTA LA FACULTAD.
                  };
                /*En esta linea de abajo lo que hago es ejecutar de mi clase infraestructura... Ejecutar, quien esta esperando el nombre del SP y los Parametros de tipo SQLParameters,
                  y en mi metodo tambien le estoy indicando que debe recibir un generico de tipo T, es decir cualquier cosa, en este caso como en mi sp estoy retornando un entero para la validacion
                 entonces le digo que estoy esperando recibir en ese metodo un int por eso el <int>*/
                var resultado = infraestructura.EjecutarSQL<int>(sp, param);
                if (resultado == 1)
                {
                     gvFacultades.DataBind();
                   
                }
                else
                {
                    //Aqui deberian brincar una ventana de tipo advertencia indicando que ya existe la facultad con ese nombre, es el resultado que estoy enviando desde mi sp.
                    //Se podria utilizar la libreria SweetAlert 1 o SweetAlert 2 para los mensajes, haciendo uso de JS.
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        protected void btnInactivar_Click(object sender, EventArgs e)
        {
            var btn = (LinkButton)sender;
            GridViewDataItemTemplateContainer c = (GridViewDataItemTemplateContainer)btn.NamingContainer;
            var codFacultad = gvFacultades.GetRowValues(c.VisibleIndex, "COD_FACULTAD");
            var sp = "SP_CAMBIARESTADOS";
            try
            {
                //Contruyo los parametros que enviare a mi SP, mapearlos igual a como estan en el SP.
                var param = new SqlParameter[]
                {
                    new SqlParameter("@COD_FACULTAD",Convert.ToInt32(codFacultad))
                };

                //Ejecuto mi SP con el metodo EjecutarSQL y vuelvo a actualizar mi GridView Bindeando mi nueva Data.
                infraestructura.EjecutarSQL<int>(sp, param);
                gvFacultades.DataBind();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            //PONER BREAKPOINT PARA PODER DEBUGUEAR Y COMPRENDER MEJOR LO QUE SE HACE PASO A PASO
            var btn = (LinkButton)sender;
            GridViewDataItemTemplateContainer c = (GridViewDataItemTemplateContainer)btn.NamingContainer;
            //Recupero la informacion traida desde el GridView. la variables es de tipo LinkButton y el sender lo que hace es traerme toda la informacion, y lo parseo al tipo Link Button porque Sender es de tipo Object.
            var codFacultad = gvFacultades.GetRowValues(c.VisibleIndex, "COD_FACULTAD");
            var nombrefacu = gvFacultades.GetRowValues(c.VisibleIndex, "NOMBREFACULTAD");
           

            //ASIGNO VARIABLES A CAMPOS DE MODAL

            txtNombreEdit.Text = nombrefacu.ToString();
           
            
            /*establezco un bloque de script en el lado del cliente al ejecutarse para mandar a llamar mi script llamado AbrirModal, y le mando el ID del modal que es lo que esta esperando mi Function
             Se debe establecer el control ScriptManager en la Master para que esto funcione, yo ya lo estableci, pero es necesario saberlo.*/
            var script = "abrirModal('modalFacultadesEditar')";
            ClientScript.RegisterStartupScript(GetType(),"edicionFacultad",script,true);

        }

        protected void btnEditarFacultades_Click(object sender, EventArgs e)
        {
            //PROBARSE HACIENDOLO CON LAS HERRAMIENTAS Y EJEMPLOS QUE YA DI.
        }
    }
}