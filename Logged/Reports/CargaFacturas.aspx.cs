//PORTAL DE PROVEDORES T|SYS|
//31 MAYO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA VISUALIZACION DE REPORTE CARGA DE FACTURAS

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

public partial class Logged_Reports_CargaFacturasB : System.Web.UI.Page
{
    PortalProveedoresEntities db = new PortalProveedoresEntities();
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
            List<CargaFacturaDTO> list_dto = new List<CargaFacturaDTO>();
            if (Request.QueryString.HasKeys())
            {
                string NumVoucher = Request["NumVoucher"];
                string Vendkey = Request["Vendkey"];
                string RFC = Request["RFC"];
                string POTranID = Request["POTranID"];
                string Total = Request["Total"];
                string Status = Request["Status"];

                list_dto = CargaFactura.ObtenerCargaFacturas(NumVoucher, Vendkey, RFC, POTranID, Total, Status, true);
            }
            else

                list_dto = CargaFactura.ObtenerCargaFacturas(true);

            if (list_dto != null && list_dto.Count > 0)
                for (int i = 0; i < list_dto.Count; i++)
                {
                    list_dto[i].Impuestos = "$ " + list_dto[i].Impuestos;
                    list_dto[i].Subtotal = "$ " + list_dto[i].Subtotal;
                    list_dto[i].Total = "$ " + list_dto[i].Total;
                }

            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "CargaFacturaReport.rpt"));
            report_document.SetDataSource(list_dto);
            report_document.SetParameterValue("titulo", "Reporte de Carga Factura");

            var company_id = HttpContext.Current.Session["IDCompany"];
            if (company_id == null)
                return;
            Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");

            report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            Reporte_CargaFacturas.ReportSource = report_document;
            Reporte_CargaFacturas.SeparatePages = false;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte de carga de facturas");
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
        //    List<CargaFacturaDTO> list_dto = new List<CargaFacturaDTO>();
        //    if (Request.QueryString.HasKeys())
        //    {
        //        string NumVoucher = Request["NumVoucher"];
        //        string Vendkey = Request["Vendkey"];
        //        string RFC = Request["RFC"];
        //        string POTranID = Request["POTranID"];
        //        string Total = Request["Total"];
        //        string Status = Request["Status"];

        //        list_dto = CargaFactura.ObtenerCargaFacturas(NumVoucher, Vendkey, RFC, POTranID, Total, Status, true);
        //    }
        //    else

        //        list_dto = CargaFactura.ObtenerCargaFacturas(true);

        //    if (list_dto != null && list_dto.Count > 0)
        //        for (int i = 0; i < list_dto.Count; i++)
        //        {
        //            list_dto[i].Impuestos = "$ " + list_dto[i].Impuestos;
        //            list_dto[i].Subtotal = "$ " + list_dto[i].Subtotal;
        //            list_dto[i].Total = "$ " + list_dto[i].Total;
        //        }
        //    ReportDocument report_document = new ReportDocument();
        //    report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "CargaFacturaReport.rpt"));
        //    report_document.SetDataSource(list_dto);
        //    report_document.SetParameterValue("titulo", "Reporte de Carga Factura");

        //    var company_id = HttpContext.Current.Session["IDCompany"];
        //    if (company_id == null)
        //        return;
        //    Company company = db.Company.Where(c => c.CompanyID == company_id.ToString()).FirstOrDefault();
        //    report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");

        //    report_document.SetParameterValue("logo", "~/Img/TSYS.png");
        //    Reporte_CargaFacturas.ReportSource = report_document;
        //}
        //catch (Exception exp)
        //{
        //    Tools.LogError(this.ToString() + " Page_Load", exp.Message);
        //    Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte de carga de facturas");
        //}
    }
}