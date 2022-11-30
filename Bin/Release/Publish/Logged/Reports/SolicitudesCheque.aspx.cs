//PORTAL DE PROVEDORES T|SYS|
//31 MAYO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA VISUALIZACION DE REPORTE DE SOLICITUD DE CHEQUES

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
using SAGE_Model;
using CrystalDecisions.Shared;

public partial class Logged_Reports_SolicitudesChequeB : System.Web.UI.Page
{
    PortalProveedoresEntities db = new PortalProveedoresEntities();
    sage500_appEntities db_sage = new sage500_appEntities();
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
            List<CheckRequest> list = db.CheckRequest.ToList();
            List<ChequeSolicitudesDTO> list_dto = new List<ChequeSolicitudesDTO>();

            if (Request.QueryString.HasKeys())
            {
                string Serie = Request["serie"];
                string Proveedor = Request["provId"];
                string Solicitante = Request["userId"];
                string Total = Request["total"];
                string fecha = Request["fecha"];
                string fechaP = Request["fechaP"];
                fecha = Tools.ObtenerFechaEnFormatoNew(fecha);
                fechaP = Tools.ObtenerFechaEnFormatoNew(fechaP);

                list_dto = ChequeSolicitudess.ObtenerSolicitudesCheque(Serie, Proveedor, Solicitante, Total, fecha,fechaP, true);
            }
            else
            {
                list_dto = ChequeSolicitudess.ObtenerSolicitudesCheque(true);
            }

            if (list_dto == null)
                throw new MulticonsultingException("No hay solicitudes de cheque que mostrar");

            foreach (var c in list_dto)
                c.Total = "$ " + c.Total;

            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "SolicitudesCheque.rpt"));
            report_document.SetDataSource(list_dto);
            report_document.SetParameterValue("titulo", "Solicitud de Cheque");

            var company_id = HttpContext.Current.Session["IDCompany"];
            if (company_id == null)
                return;
            Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");
            report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            Reporte_Solicitudes_Cheque.ReportSource = report_document;
            //Reporte_Solicitudes_Cheque.SeparatePages = false;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar el reporte de solicitudes de cheque");
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
        //    List<CheckRequest> list = db.CheckRequest.ToList();
        //    List<ChequeSolicitudDTO> list_dto = new List<ChequeSolicitudDTO>();

        //    if (Request.QueryString.HasKeys())
        //    {
        //        string Serie = Request["serie"];
        //        string Proveedor = Request["provId"];
        //        string Solicitante = Request["userId"];
        //        string Total = Request["total"];
        //        string fecha = Request["fecha"];
        //        fecha = Tools.ObtenerFechaEnFormato(fecha);

        //        list_dto = ChequeSolicitudes.ObtenerSolicitudesCheque(Serie, Proveedor, Solicitante, Total, fecha, true);
        //    }
        //    else
        //    {
        //        list_dto = ChequeSolicitudes.ObtenerSolicitudesCheque(true);
        //    }

        //    if (list_dto == null)
        //        throw new MulticonsultingException("No hay solicitudes de cheque que mostrar");

        //    foreach (var c in list_dto)
        //        c.Total = "$ " + c.Total;

        //    ReportDocument report_document = new ReportDocument();
        //    report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "SolicitudesCheque.rpt"));
        //    report_document.SetDataSource(list_dto);
        //    report_document.SetParameterValue("titulo", "Solicitud de Cheque");

        //    var company_id = HttpContext.Current.Session["IDCompany"];
        //    if (company_id == null)
        //        return;
        //    Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();
        //    report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");
        //    report_document.SetParameterValue("logo", "~/Img/TSYS.png");
        //    Reporte_Solicitudes_Cheque.ReportSource = report_document;
        //}
        //catch (Exception exp)
        //{
        //    Tools.LogError(this.ToString() + " Page_Load", exp.Message);
        //    Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar el reporte de solicitudes de cheque");
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