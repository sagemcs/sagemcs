//PORTAL DE PROVEDORES T|SYS|
//31 MAYO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA VISUALIZACION DE REPORTE CONTRARECIBO

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using Proveedores_Model;
using SAGE_Model;
using CrystalDecisions.Shared;

public partial class Logged_Reports_ContrareciboB : System.Web.UI.Page
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
        }
        try
        {
            //if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
            //{
            //    Global.Docs();
            //    if ((HttpContext.Current.Session["Docs"].ToString() == "0"))
            //    {
            //        Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            //    }
            //}

            if (Request.QueryString.HasKeys())
            {
                try
                {
                    int id = Convert.ToInt32(Request["id"]);

                    
                    report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "ContrareciboReport.rpt"));
                    InvoiceReceipt receipt = db.InvoiceReceipt.Where(c => c.InvcRcptKey == id).FirstOrDefault();
                    List<FacturaDTO> facturas = new List<FacturaDTO>();
                    List<NotaDTO> Notas = new List<NotaDTO>();
                    decimal Total = 0;
                    foreach (var detail in receipt.InvcRcptDetails)
                    {
                        Invoice invoice = detail.Invoice;
                        Int32 AprovUserKey = Convert.ToInt32(invoice.AprovUserKey);
                        Users user = db.Users.Where(u => u.UserKey == AprovUserKey).FirstOrDefault();
                        Total += invoice.Total;

                        //List<NotaDTO> Notas = new List<NotaDTO>();

                        bool sin_notas = true;

                        foreach (Invoice nota in invoice.Invoice1.Where(i => i.TranType == "CM" || i.TranType == "DM"))
                        {
                            sin_notas = false;
                            Total += nota.TranType == "CM" ? -(nota.Total) : nota.Total; // La nota afectando el valor del contrarecibo

                            NotaDTO Nota = new NotaDTO(nota.InvoiceKey, invoice.InvoiceKey);
                            Nota.Total = "$ " + Nota.Total;
                            Nota.Traslados = "$ " + Nota.Traslados;
                            Nota.Subtotal = "$ " + Nota.Subtotal;
                            Nota.Tipo = "Nota de " + (nota.TranType == "CM" ? "Crédito" : "Débito");
                            Notas.Add(Nota);
                        }
                        if (sin_notas)
                        {
                            NotaDTO Nota_Vacia = new NotaDTO();
                            Nota_Vacia.ApplyToInvcKey = invoice.InvoiceKey;
                            Notas.Add(Nota_Vacia);
                        }

                        FacturaDTO factura = new FacturaDTO(invoice.InvoiceKey);
                        factura.Total = "$ " + factura.Total;
                        factura.Traslados = "$ " + factura.Traslados;
                        factura.Subtotal = "$ " + factura.Subtotal;

                        facturas.Add(factura);
                    }
                    report_document.Database.Tables[0].SetDataSource(facturas);
                    report_document.Database.Tables[1].SetDataSource(Notas);

                    report_document.SetParameterValue("contrarecibo_no", receipt.Folio.ToString());
                    report_document.SetParameterValue("razon_social_compannia", receipt.Company.CompanyName);

                    ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(receipt.Vendors.VendorID);
                    string DBA = "Información de Proveedor no encontrada";
                    if (proveedor != null && !string.IsNullOrWhiteSpace(proveedor.RFC))
                        DBA = proveedor.RFC;

                    report_document.SetParameterValue("rfc", DBA);

                    //report_document.SetParameterValue("fecha_pago", receipt.PaymentDate.ToShortDateString());
                    report_document.SetParameterValue("fecha_pago", Tools.FechaEnEspañol(receipt.PaymentDate));
                    report_document.SetParameterValue("fecha_datos", Tools.FechaEnEspañol(receipt.CreateDate));
                    report_document.SetParameterValue("proveedor", receipt.Vendors.VendName);
                    report_document.SetParameterValue("total", Total);

                    Reporte_Contrarecibo.ReportSource = report_document;
                    //Reporte_Contrarecibo.SeparatePages = false;

                }
                catch (Exception ex)
                {
                    string eeeee = ex.ToString();
                }

            }
            else
            {
            }
        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la información de un contrarecibo");
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
            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "ContrareciboReport"));
            CheckRequest solicitud_de_cheque = db.CheckRequest.Where(c => c.ChkReqKey == id).FirstOrDefault();
            List<FacturaDTO> facturas = new List<FacturaDTO>();

            if (solicitud_de_cheque == null)
                return;
            ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(solicitud_de_cheque.Vendors.VendorID);

            decimal total = Math.Round(Convert.ToDecimal(solicitud_de_cheque.Total), 2);

            NumLetra nl = new NumLetra();
            string total_en_letras = nl.Convertir(total.ToString(), true, solicitud_de_cheque.Moneda);

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
            report_document.SetParameterValue("title", "Contrarecibo");
            //Reporte_Solicitud_Cheque.ReportSource = report_document;
            //report_document.PrintToPrinter(1, false, 0, 0);
            //Reporte_Solicitud_Cheque.SeparatePages = false;
            //report_document = Reporte_Solicitud_Cheque.repor
            report_document.PrintToPrinter(1, false, 0, 0);

        }
        catch (Exception exp)
        {
            Tools.LogError(this.ToString() + " Page_Load", exp.Message);
            Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la información de una solicitud de cheque");
        }


        //ReportDocument crystalReport = new ReportDocument();
        //crystalReport = (ReportDocument)HttpContext.Current.Session["rpt_solicitud"];
        //crystalReport.Refresh();
        // Set Paper Orientation.
        //crystalReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
        // Set Paper Size.
        //crystalReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
        // CrystalDecisions.Shared.ExportFormatType to change the format i.e. Excel, Word, PDF
        //crystalReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Response, true, "CustomerDetails");
        //crystalReport.PrintOptions.PrinterName = GetDefaultPrinter();
        //crystalReport.PrintToPrinter(1, true, 0, 0);
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
        //    if (Request.QueryString.HasKeys())
        //    {
        //        try
        //        {
        //            int id = Convert.ToInt32(Request["id"]);

        //            ReportDocument report_document = new ReportDocument();
        //            report_document.Load(Path.Combine(Server.MapPath("~/Reports"), "ContrareciboReport.rpt"));
        //            InvoiceReceipt receipt = db.InvoiceReceipt.Where(c => c.InvcRcptKey == id).FirstOrDefault();
        //            List<FacturaDTO> facturas = new List<FacturaDTO>();
        //            List<NotaDTO> Notas = new List<NotaDTO>();
        //            decimal Total = 0;
        //            foreach (var detail in receipt.InvcRcptDetails)
        //            {
        //                Invoice invoice = detail.Invoice;
        //                Int32 AprovUserKey = Convert.ToInt32(invoice.AprovUserKey);
        //                Users user = db.Users.Where(u => u.UserKey == AprovUserKey).FirstOrDefault();
        //                Total += invoice.Total;

        //                //List<NotaDTO> Notas = new List<NotaDTO>();

        //                bool sin_notas = true;

        //                foreach (Invoice nota in invoice.Invoice1.Where(i => i.TranType == "CM" || i.TranType == "DM"))
        //                {
        //                    sin_notas = false;
        //                    Total += nota.TranType == "CM" ? -(nota.Total) : nota.Total; // La nota afectando el valor del contrarecibo

        //                    NotaDTO Nota = new NotaDTO(nota.InvoiceKey, invoice.InvoiceKey);
        //                    Nota.Total = "$ " + Nota.Total;
        //                    Nota.Traslados = "$ " + Nota.Traslados;
        //                    Nota.Subtotal = "$ " + Nota.Subtotal;
        //                    Nota.Tipo = "Nota de " + (nota.TranType == "CM" ? "Crédito" : "Débito");
        //                    Notas.Add(Nota);
        //                }
        //                if (sin_notas)
        //                {
        //                    NotaDTO Nota_Vacia = new NotaDTO();
        //                    Nota_Vacia.ApplyToInvcKey = invoice.InvoiceKey;
        //                    Notas.Add(Nota_Vacia);
        //                }

        //                FacturaDTO factura = new FacturaDTO(invoice.InvoiceKey);
        //                factura.Total = "$ " + factura.Total;
        //                factura.Traslados = "$ " + factura.Traslados;
        //                factura.Subtotal = "$ " + factura.Subtotal;

        //                facturas.Add(factura);
        //            }
        //            report_document.Database.Tables[0].SetDataSource(facturas);
        //            report_document.Database.Tables[1].SetDataSource(Notas);

        //            report_document.SetParameterValue("contrarecibo_no", receipt.Folio.ToString());
        //            report_document.SetParameterValue("razon_social_compannia", receipt.Company.CompanyName);

        //            ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(receipt.Vendors.VendorID);
        //            string DBA = "Información de Proveedor no encontrada";
        //            if (proveedor != null && !string.IsNullOrWhiteSpace(proveedor.RFC))
        //                DBA = proveedor.RFC;

        //            report_document.SetParameterValue("rfc", DBA);

        //            report_document.SetParameterValue("fecha_pago", receipt.PaymentDate.ToShortDateString());
        //            report_document.SetParameterValue("fecha_datos", Tools.FechaEnEspañol(receipt.CreateDate));
        //            report_document.SetParameterValue("proveedor", receipt.Vendors.VendName);
        //            report_document.SetParameterValue("total", Total);

        //            Reporte_Contrarecibo.ReportSource = report_document;

        //        }
        //        catch (Exception ex)
        //        {
        //            string eeeee = ex.ToString();
        //        }

        //    }
        //    else
        //    {
        //    }
        //}
        //catch (Exception exp)
        //{
        //    Tools.LogError(this.ToString() + " Page_Load", exp.Message);
        //    Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar la información de un contrarecibo");
        //}
    }
}