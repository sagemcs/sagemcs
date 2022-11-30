//PORTAL DE PROVEDORES T|SYS|
//31 MAYO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA VISUALIZACION DE REPORTE STATUS DE FACTURAS

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using Proveedores_Model;
using CrystalDecisions.Shared;



public partial class Logged_Reports_FacturasEstadoAll : System.Web.UI.Page
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
            //List<FacturasAllDTO> list_dto = new List<FacturasAllDTO>();
            //List<Invoice> list = new List<Invoice>();
            List<Inv2020> Lista_Facts = new List<Inv2020>();

            if (Request.QueryString.HasKeys())
            {
                string folio = Request["folio"];
                string fecha = Request["fecha"];
                string fechaR = Request["fechaR"];
                string FechaPP = Request["FechaPP"];
                string FechaP = Request["FechaP"];
                string FolioP = Request["FolioP"];
                string Banco = Request["Banco"];
                string proveedor = Request["provId"];
                string contrarecibo = Request["contrarecibo"];
                string solicitud = Request["solicitud"];
                string estado = Request["estado"];

                string order_col = "";
                string order_dir = "";

                fecha = Tools.ObtenerFechaEnFormatoNew(fecha);
                fechaR = Tools.ObtenerFechaEnFormatoNew(fechaR);
                FechaPP = Tools.ObtenerFechaEnFormatoNew(FechaPP);
                FechaP = Tools.ObtenerFechaEnFormatoNew(FechaP);

                //list_dto = FacturasAll.ObtenerFacturas(proveedor, folio, fecha, fechaR, FechaPP, FechaP, FolioP, Banco, contrarecibo, solicitud, estado, true);
                Lista_Facts = FacturasAll.ObtenerFacturas20(order_col, order_dir, proveedor, folio, fecha, fechaR, FechaPP, FechaP, FolioP, Banco, contrarecibo, solicitud, estado);
            }
            else
            {
                string folio = Request["folio"];
                string fecha = Request["fecha"];
                string fechaR = Request["fechaR"];
                string FechaPP = Request["FechaPP"];
                string FechaP = Request["FechaP"];
                string FolioP = Request["FolioP"];
                string Banco = Request["Banco"];
                string proveedor = Request["provId"];
                string contrarecibo = Request["contrarecibo"];
                string solicitud = Request["solicitud"];
                string estado = Request["estado"];

                string order_col = "";
                string order_dir = "";

                fecha = Tools.ObtenerFechaEnFormatoNew(fecha);
                fechaR = Tools.ObtenerFechaEnFormatoNew(fechaR);
                FechaPP = Tools.ObtenerFechaEnFormatoNew(FechaPP);
                FechaP = Tools.ObtenerFechaEnFormatoNew(FechaP);

                Lista_Facts = FacturasAll.ObtenerFacturas20(order_col, order_dir, proveedor, folio, fecha, fechaR, FechaPP, FechaP, FolioP, Banco, contrarecibo, solicitud, estado);
            }

                //list_dto = FacturasAll.ObtenerFacturas(true);

                report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "FacturasEstadoAllReport.rpt"));
                report_document.SetDataSource(Lista_Facts);
                //report_document.SetDataSource(list_dto);
                report_document.SetParameterValue("titulo", "Reporte de Estado de Facturas");
                

            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return;
            report_document.SetParameterValue("compannia", company != null ? company.CompanyName : "Nombre de la compañia");

            string path = Server.MapPath("~/Img/estados/");
            report_document.SetParameterValue("img_base", path);
            report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            Reporte_FacturasEstado.ReportSource = report_document;
            //Reporte_FacturasEstado.SeparatePages = false;
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la interfaz del reporte estado de facturas");
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
}