//PORTAL DE PROVEDORES T|SYS|
//31 MAYO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA VISUALIZACION DE REPORTE DE USUARIOS

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using Proveedores_Model;
using CrystalDecisions.Shared;

public partial class Logged_Reports_UsuariosB : System.Web.UI.Page
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    ReportDocument report_document = new ReportDocument();


    protected void Page_Init(object sender, EventArgs e)
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
        try
        {
            List<Users> list = db.Users.ToList();
            List<UsuarioDTO> list_dto = new List<UsuarioDTO>();

            if (Request.QueryString.HasKeys())
            {
                string nombre = Request["username"].ToString();
                string proveedor = Request["VendName"].ToString();
                string interno = Request["interno"].ToString();
                string estado = Request["estado"].ToString().Replace(",", string.Empty);

                list_dto = Usuarios.ObtenerUsuarios(nombre, proveedor, interno, estado, true);
            }
            else
            {
                list_dto = Usuarios.ObtenerUsuarios(true);
            }

            var company_id = HttpContext.Current.Session["IDCompany"];
            if (company_id == null)
                return;
            Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();

            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "UsuariosReport.rpt"));
            report_document.SetDataSource(list_dto);
            report_document.SetParameterValue("titulo", "Reporte de Usuarios");
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");
            report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            Reporte_Usuarios.ReportSource = report_document;
            Reporte_Usuarios.SeparatePages = false;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar el reporte de usuarios");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        //Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        //Page.Response.Cache.SetNoStore();
        //Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        //bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        //if (!isAuth)
        //{
        //    HttpContext.Current.Session.RemoveAll();
        //    Context.GetOwinContext().Authentication.SignOut();
        //    Response.Redirect("~/Account/Login.aspx");
        //}

        //if (!IsPostBack)
        //{
        //    if (HttpContext.Current.Session["IDCompany"] == null)
        //    {
        //        Context.GetOwinContext().Authentication.SignOut();
        //        Response.Redirect("~/Account/Login.aspx");
        //    }
        //    else
        //    {
        //        if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
        //        {
        //            HttpContext.Current.Session.RemoveAll();
        //            Context.GetOwinContext().Authentication.SignOut();
        //            Response.Redirect("~/Account/Login.aspx");
        //        }
        //    }
        //}
        //try
        //{
        //    List<Users> list = db.Users.ToList();
        //    List<UsuarioDTO> list_dto = new List<UsuarioDTO>();

        //    if (Request.QueryString.HasKeys())
        //    {
        //        string nombre = Request["nombre"].ToString();
        //        string proveedor = Request["provId"].ToString();
        //        string interno = Request["interno"].ToString();
        //        string estado = Request["estado"].ToString().Replace(",", string.Empty);

        //        list_dto = Usuarios.ObtenerUsuarios(nombre, proveedor, interno, estado, true);
        //    }
        //    else
        //    {
        //        list_dto = Usuarios.ObtenerUsuarios(true);
        //    }

        //    var company_id = HttpContext.Current.Session["IDCompany"];
        //    if (company_id == null)
        //        return;
        //    Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();

        //    ReportDocument report_document = new ReportDocument();
        //    report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "UsuariosReport.rpt"));
        //    report_document.SetDataSource(list_dto);
        //    report_document.SetParameterValue("titulo", "Reporte de Usuarios");
        //    report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");
        //    report_document.SetParameterValue("logo", "~/Img/TSYS.png");
        //    Reporte_Usuarios.ReportSource = report_document;
        //}
        //catch (Exception exp)
        //{
        //    Tools.LogError(this.ToString() + " Page_Load", exp.Message);
        //    Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar el reporte de usuarios");
        //}

    }

    private void Page_Unload(object sender, EventArgs e)
    {
        CloseReports(report_document);
        report_document.Dispose();
        report_document.Dispose();
        report_document = null;

    }

    private void CloseReports(ReportDocument reportDocument)
    {
        Sections sections = reportDocument.ReportDefinition.Sections;
        foreach (Section section in sections)
        {
            ReportObjects reportObjects = section.ReportObjects;
            foreach (ReportObject reportObject in reportObjects)
            {
                if (reportObject.Kind == ReportObjectKind.SubreportObject)
                {
                    SubreportObject subreportObject = (SubreportObject)reportObject;
                    ReportDocument subReportDocument = subreportObject.OpenSubreport(subreportObject.SubreportName);
                    subReportDocument.Close();
                }
            }
        }
        reportDocument.Close();
    }
}