//PORTAL DE PROVEDORES T|SYS|
//10 JUNIO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA DE SOLICITUD DE CHEQUES

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

using Proveedores_Model;

public partial class Logged_SolicitudCheque : System.Web.UI.Page
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    protected void Page_Load(object sender, EventArgs e)
    {
        try
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
            List<ProveedorDTO> list_dto = Proveedores.ObtenerProveedores();
            ProveedorDTO vacio = new ProveedorDTO();
            vacio.Social = "[-Seleccione proveedor-]";
            vacio.ID = "";
            list_dto.Insert(0, vacio);

            Page.Title = "Solicitud de cheque";
            comboProveedores.DataSource = list_dto;
            comboProveedores.DataTextField = "Social";
            comboProveedores.DataValueField = "ID";
            comboProveedores.DataBind();
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz de creación de solicitud de cheque");
        }
    }
    
}