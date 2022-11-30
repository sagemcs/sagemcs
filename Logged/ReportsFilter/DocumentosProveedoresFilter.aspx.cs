//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : ADRIAN QUIALA
//PANTALLA DE CONFIGURACION PARA REPORTE DE DOCUMENTOS DE PROVEEDOR

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logged_ReportsFilter_DocumentosProveedoresFilter : System.Web.UI.Page
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
        Page.Title = "Documentos proveedores";
        try
        {
            List<EstadoDocumentosDTO> list_dto_estados = EstadosDocumentos.Obtener();
            if (list_dto_estados != null && list_dto_estados.Count != 0)
            {
                EstadoDocumentosDTO vacio = new EstadoDocumentosDTO();
                vacio.Id = "0";
                vacio.Descripcion = "[-Seleccione estado-]";
                list_dto_estados.Insert(0, vacio);
                comboEstado.DataSource = list_dto_estados;
                comboEstado.DataTextField = "Descripcion";
                comboEstado.DataValueField = "Id";
                comboEstado.DataBind();
            }
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar los documentos de proveedores");
        }
    }
}