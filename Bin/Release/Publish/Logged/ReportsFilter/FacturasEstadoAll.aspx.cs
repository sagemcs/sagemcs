//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : ADRIAN QUIALA
//PANTALLA DE CONFIGURACION PARA REPORTE DE STATUS DE FACTURAS

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

public partial class Logged_ReportsFilter_FacturasEstadoFilter : System.Web.UI.Page
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
        Page.Title = "Estado de Facturas Completo";
        try
        {
            //Facturas.ActualizarEstadoFacturas(true);
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

            List<EstadoDocumentoDTO> list_dto_estados = EstadosDocumento.Obtener();
            if (list_dto_estados != null && list_dto_estados.Count != 0)
            {
                EstadoDocumentoDTO vacio = new EstadoDocumentoDTO();
                vacio.Id = "0";
                vacio.Descripcion = "[-Seleccione estado-]";
                list_dto_estados.Insert(0, vacio);
                comboEstado.DataSource = list_dto_estados;
                comboEstado.DataTextField = "Descripcion";
                comboEstado.DataValueField = "Id";
                comboEstado.DataBind();
            }

            List<BancoSAGEDTO> list_dto_Bancos = BancoSAGE.Obtener();
            if (list_dto_Bancos != null && list_dto_Bancos.Count != 0)
            {
                BancoSAGEDTO vaciox = new BancoSAGEDTO();
                vaciox.Id = "[-Seleccione Banco-]";
                vaciox.Descripcion = "[-Seleccione Banco-]";
                list_dto_Bancos.Insert(0, vaciox);
                comboBanco.DataSource = list_dto_Bancos;
                comboBanco.DataTextField = "Descripcion";
                comboBanco.DataValueField = "Id";
                comboBanco.DataBind();
            }
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar los estados de facturas");
        }
    }
}