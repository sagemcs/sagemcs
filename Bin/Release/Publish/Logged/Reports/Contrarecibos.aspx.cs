//PORTAL DE PROVEDORES T|SYS|
//31 MAYO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA VISUALIZACION DE REPORTE  CONTRARECIBOS

//REFERENCIAS UTILIZADAS
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Proveedores_Model;
using CrystalDecisions.Shared;


public partial class Logged_Reports_ContrarecibosB : System.Web.UI.Page
{
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
        }
        try
        {

            if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
            {
                Global.Docs();
                if ((HttpContext.Current.Session["Docs"].ToString() == "0"))
                {
                    Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
                }
            }

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            List<ContrareciboDTO> contrarecibos = new List<ContrareciboDTO>();
            if (Request.QueryString.HasKeys())
            {
                string folio = Request["folio"];
                string proveedor = Request["provId"];
                string total = Request["total"];
                string rfc = Request["rfc"];
                string fechaPago = Request["fechaPago"];
                string fecha = Request["fecha"];

                fecha = Tools.ObtenerFechaEnFormatoNew(fecha);
                fechaPago = Tools.ObtenerFechaEnFormatoNew(fechaPago);

                contrarecibos = Contrarecibos.ObtenerContrarecibos(folio, proveedor, rfc, total, fecha, fechaPago, false, true);
            }
            else
                contrarecibos = Contrarecibos.ObtenerContrarecibos(false, true);

            if (contrarecibos == null)
                throw new MulticonsultingException("No hay contrarecibos que mostrar");

            foreach (var c in contrarecibos)
                c.Total = "$ " + c.Total;

            
            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "ContrarecibosReport.rpt"));
            report_document.SetDataSource(contrarecibos);

            var company_id = HttpContext.Current.Session["IDCompany"];
            if (company_id == null)
                return;
            Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();

            report_document.SetParameterValue("titulo", "Reporte de Contrarecibos");
            if (company != null)
            {
                report_document.SetParameterValue("compannia", company.CompanyName);
            }
            Reporte_Contrarecibos.ReportSource = report_document;
            Reporte_Contrarecibos.SeparatePages = false;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte de contrarecibos");
        }
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
        //}
        //try
        //{
        //    PortalProveedoresEntities db = new PortalProveedoresEntities();
        //    List<ContrareciboDTO> contrarecibos = new List<ContrareciboDTO>();
        //    if (Request.QueryString.HasKeys())
        //    {
        //        string folio = Request["folio"];
        //        string proveedor = Request["provId"];
        //        string total = Request["total"];
        //        string rfc = Request["rfc"];
        //        string fecha = Request["&fecha_r="];
        //        fecha = Tools.ObtenerFechaEnFormato(fecha);

        //        contrarecibos = Contrarecibos.ObtenerContrarecibos(folio, proveedor, rfc, total, fecha, false, true);
        //    }
        //    else
        //        contrarecibos = Contrarecibos.ObtenerContrarecibos(false, true);

        //    if (contrarecibos == null)
        //        throw new MulticonsultingException("No hay contrarecibos que mostrar");

        //    foreach (var c in contrarecibos)
        //        c.Total = "$ " + c.Total;

        //    ReportDocument report_document = new ReportDocument();
        //    report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "ContrarecibosReport.rpt"));
        //    report_document.SetDataSource(contrarecibos);

        //    var company_id = HttpContext.Current.Session["IDCompany"];
        //    if (company_id == null)
        //        return;
        //    Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();

        //    report_document.SetParameterValue("titulo", "Reporte de Contrarecibos");
        //    if (company != null)
        //    {
        //        report_document.SetParameterValue("compannia", company.CompanyName);
        //    }
        //    Reporte_Contrarecibos.ReportSource = report_document;
        //}
        //catch (Exception exp)
        //{
        //    Tools.LogError(this.ToString() + " Page_Load", exp.Message);
        //    Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte de contrarecibos");
        //}

    }
}