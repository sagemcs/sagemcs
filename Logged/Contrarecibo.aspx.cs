//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA DE CONTRARECIBO

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
using System.Globalization;

public partial class Logged_Contrarecibo : System.Web.UI.Page
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    protected void Page_Load(object sender, EventArgs e)
    {
        //Response.Redirect("~/Logged/Error?error=Este es el texto del mensaje de error");
        //return;
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



            Page.Title = "Generar contrarecibo";

            List<ProveedorDTO> list_dto = Proveedores.ObtenerProveedores();
            if (list_dto != null && list_dto.Count != 0)
            {
                ProveedorDTO vacio = new ProveedorDTO();
                vacio.Social = "[-Seleccione proveedor-]";
                vacio.ID = "[-Seleccione proveedor-]";
                list_dto.Insert(0, vacio);

                comboProveedores.DataSource = list_dto;
                comboProveedores.DataTextField = "ID";
                comboProveedores.DataValueField = "ID";
                comboProveedores.DataBind();
            }
            else
                return;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz de creación de contrarecibos");
        }
    }

    protected void Page_PreInit(object sender, EventArgs e)
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

            if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
            {
                Global.Docs();
                if ((HttpContext.Current.Session["Docs"].ToString() == "0"))
                {
                    //Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
                }
            }
        }
        catch
        {
            HttpContext.Current.Session.RemoveAll();
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }

    }
    string eventName = String.Empty;
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        eventName = "OnPreInit";
    }

}