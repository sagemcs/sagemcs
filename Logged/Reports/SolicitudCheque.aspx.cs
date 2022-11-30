//PORTAL DE PROVEDORES T|SYS|
//31 MAYO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : Alexis Atrizco 
//Impresion del reporte

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
using System.Drawing.Printing;
using System.Data.SqlClient;
using System.Configuration;
using CrystalDecisions.Shared;

public partial class Logged_Reports_SolicitudChequeB : System.Web.UI.Page
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
            if (Request.QueryString.HasKeys())
            {
                try
                {
                    Users usuario = Tools.UsuarioAutenticado();
                    if (usuario == null)
                        return;
                    Company company = Tools.EmpresaAutenticada();
                    if (company == null)
                        return;

                    int id = Convert.ToInt32(Request["id"]);

                    sage500_appEntities db_sage = new sage500_appEntities();
                    report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "SolicitudChequeReport.rpt"));
                    CheckRequest solicitud_de_cheque = db.CheckRequest.Where(c => c.ChkReqKey == id).FirstOrDefault();
                    List<FacturaDTO> facturas = new List<FacturaDTO>();

                    if (solicitud_de_cheque == null)
                        return;
                    ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(solicitud_de_cheque.Vendors.VendorID);

                    decimal total = Math.Round(Convert.ToDecimal(solicitud_de_cheque.Total), 2);

                    NumLetra nl = new NumLetra();

                    string total_en_letras = nl.Convertir(total.ToString(), true,solicitud_de_cheque.Moneda.ToString());

                    //for(int i = 0; i < 20; i++)// Solo activar esta línea cuando se quiera ver comportamiento del documento con muchos datos cargados
                    foreach (ChkReqDetail detail in solicitud_de_cheque.ChkReqDetail)
                    {
                        foreach (InvcRcptDetails fac in detail.InvoiceReceipt.InvcRcptDetails)
                        {
                            FacturaDTO factura = new FacturaDTO()
                            {
                                Folio = fac.Invoice.Folio,
                                Compania = fac.Invoice.CompanyID,
                                Orden_de_Compra = fac.Invoice.NodeOc,
                                Tipo = fac.Invoice.TipoComprobante,
                                Total = Math.Round(fac.Invoice.Total, 2).ToString()
                            };
                            facturas.Add(factura);
                        }
                    }
                    report_document.SetDataSource(facturas);
                    report_document.SetParameterValue("razon_social_empresa", company.CompanyName.ToUpper());
                    report_document.SetParameterValue("direccion_gerencia", "Dirección de Finanzas");
                    report_document.SetParameterValue("condiciones_pago", "Pago a " + proveedor.Condiciones_Descripcion.ToString());
                    report_document.SetParameterValue("proveedor", proveedor.Social.ToUpper());
                    report_document.SetParameterValue("importe", total.ToString());
                    report_document.SetParameterValue("folio", solicitud_de_cheque.Serie);
                    report_document.SetParameterValue("solicitante", usuario.UserName);
                    report_document.SetParameterValue("concepto", solicitud_de_cheque.Concept);
                    report_document.SetParameterValue("comentarios", solicitud_de_cheque.Comment);
                    report_document.SetParameterValue("importe_letras", "(" + total_en_letras.ToUpper() + ")");
                    //report_document.SetParameterValue("fecha_programada_pago", solicitud_de_cheque.ChkReqPmtDate.Value.ToShortDateString());
                    report_document.SetParameterValue("fecha_programada_pago", Tools.FechaEnEspañol(solicitud_de_cheque.ChkReqPmtDate.Value));
                    report_document.SetParameterValue("fecha_solicitud", Tools.FechaCortaEsp(solicitud_de_cheque.ChkReqDate));
                    report_document.SetParameterValue("logo", "~/Img/TSYS.png");
                    report_document.SetParameterValue("title", "Solicitud de Cheque");
                    Reporte_Solicitud_Cheque.ReportSource = report_document;
                    //HttpContext.Current.Session["rpt_solicitud"] = report_document;
                    //report_document.PrintToPrinter(1, false, 0, 0);
                    //Reporte_Solicitud_Cheque.SeparatePages = false;
                    //report_document.PrintToPrinter(1, false, 0, 0);

                    //string Impresora = GetDefaultPrinter();
                    //if (Impresora =="") { Impresora = "No funciono"; }
                    //impreso.Text = Impresora;

                }
                catch(Exception ex)
                {
                    string Error = ex.Message;
                }

            }
            else
            {
            }
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la información de una solicitud de cheque");
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

    protected void SelPrint(object sender, EventArgs e)
    {
        string Impresora = GetDefaultPrinter();
    }

    private string GetDefaultPrinter()
    {
        PrinterSettings settings = new PrinterSettings();
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            settings.PrinterName = printer;
            if (settings.IsDefaultPrinter)
            {
                return printer;
            }
        }
        return string.Empty;
    }

    protected void PrintDirect(object sender, EventArgs e)
    {
        //int i;
        //i = 0;
        try
        {
            Users usuario = Tools.UsuarioAutenticado();
            if (usuario == null)
                return;
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return;

            int id = Convert.ToInt32(Request["id"]);

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            sage500_appEntities db_sage = new sage500_appEntities();
            ReportDocument report_document = new ReportDocument();
            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "SolicitudChequeReport.rpt"));
            CheckRequest solicitud_de_cheque = db.CheckRequest.Where(c => c.ChkReqKey == id).FirstOrDefault();
            List<FacturaDTO> facturas = new List<FacturaDTO>();

            if (solicitud_de_cheque == null)
                return;
            ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(solicitud_de_cheque.Vendors.VendorID);

            decimal total = Math.Round(Convert.ToDecimal(solicitud_de_cheque.Total), 2);

            NumLetra nl = new NumLetra();

            string total_en_letras = nl.Convertir(total.ToString(), true,solicitud_de_cheque.Moneda);

            //for(int i = 0; i < 20; i++)// Solo activar esta línea cuando se quiera ver comportamiento del documento con muchos datos cargados
            foreach (ChkReqDetail detail in solicitud_de_cheque.ChkReqDetail)
            {
                foreach (InvcRcptDetails fac in detail.InvoiceReceipt.InvcRcptDetails)
                {
                    FacturaDTO factura = new FacturaDTO()
                    {
                        Folio = fac.Invoice.Folio,
                        Compania = fac.Invoice.CompanyID,
                        Orden_de_Compra = fac.Invoice.NodeOc,
                        Tipo = fac.Invoice.TipoComprobante,
                        Total = Math.Round(fac.Invoice.Total, 2).ToString()
                    };
                    facturas.Add(factura);
                }
            }
            report_document.SetDataSource(facturas);
            report_document.SetParameterValue("razon_social_empresa", company.CompanyName.ToUpper());
            report_document.SetParameterValue("direccion_gerencia", "Dirección de Finanzas");
            report_document.SetParameterValue("condiciones_pago", "Pago a " + proveedor.Condiciones_Descripcion.ToString());
            report_document.SetParameterValue("proveedor", proveedor.Social.ToUpper());
            report_document.SetParameterValue("importe", total.ToString());
            report_document.SetParameterValue("folio", solicitud_de_cheque.Serie);
            report_document.SetParameterValue("solicitante", usuario.UserName);
            report_document.SetParameterValue("concepto", solicitud_de_cheque.Concept);
            report_document.SetParameterValue("comentarios", solicitud_de_cheque.Comment);
            report_document.SetParameterValue("importe_letras", "(" + total_en_letras.ToUpper() + ")");
            //report_document.SetParameterValue("fecha_programada_pago", solicitud_de_cheque.ChkReqPmtDate.Value.ToShortDateString());
            report_document.SetParameterValue("fecha_programada_pago", Tools.FechaEnEspañol(solicitud_de_cheque.ChkReqPmtDate.Value));
            report_document.SetParameterValue("fecha_solicitud", Tools.FechaCortaEsp(solicitud_de_cheque.ChkReqDate));
            report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            report_document.SetParameterValue("title", "Solicitud de Cheque");
            //Reporte_Solicitud_Cheque.ReportSource = report_document;
            //report_document.PrintToPrinter(1, false, 0, 0);
            //Reporte_Solicitud_Cheque.SeparatePages = false;
            //report_document = Reporte_Solicitud_Cheque.repor
            //report_document.PrintToPrinter(1, false, 0, 0);

        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la información de una solicitud de cheque");
        }
    }

    protected void NewPage(object sender, EventArgs e)
    {
        try
        {
            if (Request.QueryString.HasKeys())
            {
                string id = Convert.ToString(Request["id"]);

                string archivo = string.Empty;
                MemoryStream memoryStream = new MemoryStream();
                memoryStream = databaseFileRead(id); //PDF
                archivo += " Solicitud De Cheque " + id;
                archivo += ".pdf";
                HttpContext.Current.Response.ContentType = "application/pdf";
                //string filename = Request.QueryString["filename"].ToString();

                //HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", filename));
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "inline; filename=\"" + archivo + "\"");
                HttpContext.Current.Response.AppendHeader("Content-Length", memoryStream.Length.ToString());
                HttpContext.Current.Response.BinaryWrite(memoryStream.ToArray());
                HttpContext.Current.Response.End();

            }


        }
        catch(Exception ex)
        {

        }
    }
    //Rutina de Conexión
    public static SqlConnection SqlConnectionDB(string cnx)
    {
        try
        {
            SqlConnection SqlConnectionDB = new SqlConnection();
            ConnectionStringSettings connSettings = ConfigurationManager.ConnectionStrings[cnx];
            if ((connSettings != null) && (connSettings.ConnectionString != null))
            {
                SqlConnectionDB.ConnectionString = ConfigurationManager.ConnectionStrings[cnx].ConnectionString;
            }

            return SqlConnectionDB;
        }
        catch (Exception ex)
        {
            string Msj = string.Empty;
            return null;
        }
    }
    private MemoryStream databaseFileRead(string InvoiceKey)
    {
        try
        {
            string sql = "";
            MemoryStream memoryStream = new MemoryStream();
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            sql = @"SELECT FileBinary FROM ChkReqFile where CheckRequestKey =  @varID";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.Parameters.AddWithValue("@varID", InvoiceKey);
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        sqlQueryResult.Read();
                        var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                        sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                        memoryStream.Write(blob, 0, blob.Length);
                    }
            }

            sqlConnection1.Close();


            return memoryStream;
        }
        catch (Exception ex)
        {
            //RutinaError(ex);
            return null;
        }
    }
}