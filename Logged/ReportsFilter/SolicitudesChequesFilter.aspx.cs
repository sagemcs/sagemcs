//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : ADRIAN QUIALA
//PANTALLA DE CONFIGURACION PARA REPORTE DE SOLICITUD DE CHEQUES

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logged_ReportsFilter_SolicitudesChequesFilter : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        if (!isAuth)
        {
            HttpContext.Current.Session.RemoveAll();
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }

        if (!IsPostBack)
        {
            if (HttpContext.Current.Session["IDCompany"] == null)
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                {
                    HttpContext.Current.Session.RemoveAll();
                    Context.GetOwinContext().Authentication.SignOut();
                    Response.Redirect("~/Account/Login.aspx");
                }
            }
        }
        Page.Title = "Solicitudes cheques";
        try
        {
            List<ProveedorDTO> list_dto_proveedores = Proveedores.ObtenerProveedores(true);
            List<UsuarioDTO> list_dto_solicitantes = Usuarios.ObtenerUsuarios(true);
            List<UsuarioDTO> list_dto_solicitantes_Tsys = Usuarios.ObtenerUsuarios(true);

            if (list_dto_proveedores != null && list_dto_proveedores.Count != 0)
            {
                ProveedorDTO vacio_p = new ProveedorDTO();
                vacio_p.Social = "[-Seleccione proveedor-]";
                vacio_p.ID = "";
                list_dto_proveedores.Insert(0, vacio_p);
                comboProveedores.DataSource = list_dto_proveedores;
                comboProveedores.DataTextField = "Social";
                comboProveedores.DataValueField = "ID";
                comboProveedores.DataBind();
            }
            if (list_dto_solicitantes != null && list_dto_solicitantes.Count != 0)
            {
                list_dto_solicitantes_Tsys.Clear();             
                for (int i=0;i< list_dto_solicitantes.Count();i++)
                {
                    if (list_dto_solicitantes[i].Interno == "SI")
                    {
                        UsuarioDTO valor_s = new UsuarioDTO();
                        valor_s.Actualizacion = list_dto_solicitantes[i].Actualizacion;
                        valor_s.Compania = list_dto_solicitantes[i].Compania;
                        valor_s.Correo = list_dto_solicitantes[i].Nombre;
                        valor_s.Creacion= list_dto_solicitantes[i].Creacion;
                        valor_s.Estado = list_dto_solicitantes[i].Estado;
                        valor_s.Interno = list_dto_solicitantes[i].Interno;
                        valor_s.Nombre = list_dto_solicitantes[i].Nombre;
                        //valor_s.Proveedor = list_dto_solicitantes[i].Proveedor;
                        //valor_s.ProveedorId = list_dto_solicitantes[i].ProveedorId;
                        list_dto_solicitantes_Tsys.Insert(0, valor_s);
                    }
                }

                UsuarioDTO vacia_s = new UsuarioDTO();
                vacia_s.Nombre = "[-Seleccione solicitante-]";
                vacia_s.Correo = "";
                list_dto_solicitantes_Tsys.Insert(0, vacia_s);
                comboSolicitantes.DataSource = list_dto_solicitantes_Tsys;
                comboSolicitantes.DataTextField = "Nombre";
                comboSolicitantes.DataValueField = "Correo";
                comboSolicitantes.DataBind();
            }
            else
                return;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar las solicitudes de cheque");
        }
    }
}