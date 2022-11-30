//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : ADRIAN QUIALA
//PANTALLA DE CONFIGURACION PARA REPORTE DE CONTRARECIBOS

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Proveedores_Model;

public partial class Logged_ReportsFilter_ContrarecibosFilter : System.Web.UI.Page
{
    string eventName = String.Empty;

    private PortalProveedoresEntities db = new PortalProveedoresEntities();

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
        }

        Page.Title = "Contrarecibos";
        try
        {
            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Empleado")
            {
                List<ProveedorDTO> list_dtos = Proveedores.ObtenerProveedoresEmpleado(true);
                if (list_dtos != null && list_dtos.Count != 0)
                {
                    ProveedorDTO vacio = new ProveedorDTO();
                    vacio.Social = "[-Seleccione proveedor-]";
                    vacio.ID = "";
                    list_dtos.Insert(0, vacio);
                    comboProveedores.DataSource = list_dtos;
                    comboProveedores.DataTextField = "Social";
                    comboProveedores.DataValueField = "ID";
                    comboProveedores.DataBind();
                }
            }
            else
            {
                List<ProveedorDTO> list_dto = Proveedores.ObtenerProveedores(true);
                if (list_dto != null && list_dto.Count != 0)
                {
                    ProveedorDTO vacio = new ProveedorDTO();
                    vacio.Social = "[-Seleccione proveedor-]";
                    vacio.ID = "";
                    list_dto.Insert(0, vacio);

                    comboProveedores.DataSource = list_dto;
                    comboProveedores.DataTextField = "Social";
                    comboProveedores.DataValueField = "ID";
                    comboProveedores.DataBind();
                }

                if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                {
                    Global.Docs();
                    int Dias = Convert.ToInt16(HttpContext.Current.Session["Docs"].ToString());
                    if (Dias == 0)
                    {
                        Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
                    }
                    else if (Dias < 0)
                    {
                        Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
                    }
                    else if (Dias == 30)
                    {
                        // Page.MasterPageFile = "~/Logged/Proveedores/MenuPreP.master";
                    }
                    else if (Dias == 25)
                    {
                        Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
                    }
                    else if (Dias == 26)
                    {
                        Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
                    }
                    else if (Dias == 27)
                    {
                        Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
                    }
                    else if (Dias == 28)
                    {
                        //Page.MasterPageFile = "~/Logged/Proveedores/MenuPreP.master";
                    }
                    else if (Dias == 22)
                    {
                        Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
                    }
                    else if (Dias <= 10 && Dias > 0)
                    {
                        //Page.MasterPageFile = "~/Logged/Proveedores/MenuPreP.master";
                    }
                    else if (Dias > 10)
                    {
                        //Page.MasterPageFile = "~/Logged/Proveedores/MenuPreP.master";
                    }
                }

                else
                    return;

            }


        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar los contrarecibos");
        }
    }

    protected override void OnPreInit(EventArgs e)
    {
        try
        {
            bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (!isAuth)
            {
                HttpContext.Current.Session.RemoveAll();
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            base.OnPreInit(e);
            eventName = "OnPreInit";

        }
        catch (Exception ex)
        {
            HttpContext.Current.Session.RemoveAll();
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }

    }


}