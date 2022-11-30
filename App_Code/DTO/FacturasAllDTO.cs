//PORTAL DE PROVEDORES T|SYS|
//08 ABRIL DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA P.

//REFERENCIAS UTILIZADAS
using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

public class FacturasAllDTO
{
    public int Key { get; set; }
    public string Compania { get; set; }
    public string Folio { get; set; }
    public string Serie { get; set; }
    public string Fecha { get; set; }
    public DateTime DateFF { get; set; }
    public string Fecha_Recepcion { get; set; }
    public DateTime DateRc { get; set; }
    public string Fecha_Aprobacion { get; set; }
    public DateTime DateAr { get; set; }
    public string Fecha_Programada_Pago { get; set; }
    public DateTime DatePr { get; set; }
    public string Proveedor { get; set; }
    public string Proveedor_Nombre { get; set; }
    public string Subtotal { get; set; }
    public double Subtotal_In_Doouble { get; set; }
    public string Retenciones { get; set; }
    public string Traslados { get; set; }
    public double Traslados_In_Double { get; set; }

    public string Fecha_Pago { get; set; }
    public DateTime DatePag { get; set; }
    public string Banco_Pago { get; set; }
    public string Cuenta_Pago { get; set; }
    public string Fecha_Notificacion_Pago { get; set; }
    public DateTime DateNP { get; set; }
    public string Folio_Pago { get; set; }
    public string Fecha_FechaRecepcion_Pago { get; set; }
    public DateTime DateRcP { get; set; }
    public string Fecha_FechaAprobacion_Pago { get; set; }
    public DateTime DateArP { get; set; }


    public string Descuento { get; set; }
    public string Total { get; set; }
    public double Total_In_Double { get; set; }
    public string UUID { get; set; }
    public string Usuario { get; set; }
    public string Estado { get; set; }
    public string Estado_Id { get; set; }
    public string Estado_Img { get; set; }

    public string Moneda { get; set; }

    public string Tipo { get; set; }
    public string Orden_de_Compra { get; set; }

    public List<NotasDTO> Notas { get; set; }

    public int VendKey { get; set; }
    public string Contrarecibo_Folio { get; set; }

    public string Solicitud_Folio { get; set; }

    public FacturasAllDTO()
    { }

    public string[] REvisaPago(int Vend, string company, int Vk)
    {
        string[] Pago = new string[4];
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("DTOPago", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@VendKey", Value = Vend });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@IDC", Value = company });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Fol", Value = Vk });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable Registros = new DataTable();
                Registros.Load(rdr);
                int Filas = Registros.Rows.Count;

                if (Filas >= 1)
                {
                    DataRow rows;
                    rows = Registros.Rows[0];
                    Pago[0] = (rows["FechaPago"].ToString());
                    Pago[1] = (rows["Banco"].ToString());
                    Pago[2] = (rows["Cuenta"].ToString());
                    Pago[3] = (rows["FechaNot"].ToString());
                }
            }
        }
        catch (Exception ex)
        {
            string ms = ex.Message;
        }

        return Pago;
    }

    public FacturasAllDTO(int Key)
    {
        PortalProveedoresEntities db = new PortalProveedoresEntities();
        Invoice factura = db.Invoice.Where(f => f.InvoiceKey == Key).FirstOrDefault();
        PaymentAppl Pago = db.PaymentAppl.Where(a => a.ApplInvoiceKey == Key).FirstOrDefault();

        if (factura != null)
        {
            //this.Key = Key;
            //this.Compania = factura.CompanyID;
            this.Folio = factura.Folio;
            this.Serie = factura.Serie;
            this.Fecha = factura.FechaTransaccion != null ? factura.FechaTransaccion.Value.ToString("dd/MM/yyyy") : "";
            this.DateFF = factura.FechaTransaccion == null ? DateTime.MinValue : Convert.ToDateTime(factura.FechaTransaccion);
            this.Fecha_Recepcion = factura.UpdateDate != null ? factura.UpdateDate.Value.ToString("dd/MM/yyyy") : "";
            this.DateRc = factura.UpdateDate == null ? DateTime.MinValue : Convert.ToDateTime(factura.UpdateDate);
            this.Fecha_Aprobacion = factura.AprovDate != null ? factura.AprovDate.Value.ToString("dd/MM/yyyy") : "";
            this.DateAr = factura.AprovDate == null ? DateTime.MinValue : Convert.ToDateTime(factura.AprovDate);
            if (factura.InvcRcptDetails.Count == 0)
            {
                this.Fecha_Programada_Pago = "-";
                this.DatePr = DateTime.MinValue;
            }
            else
            {
                InvoiceReceipt invoiceReceipt = null;
                InvcRcptDetails invcRcptDetails = factura.InvcRcptDetails.FirstOrDefault();
                if (invcRcptDetails != null)
                {
                    invoiceReceipt = invcRcptDetails.InvoiceReceipt;
                }

                //this.Fecha_Programada_Pago = factura.InvcRcptDetails.First().InvoiceReceipt.ChkReqDetail.First().CheckRequest.ChkReqPmtDate.Value.ToString("dd/MM/yyyy") != null ? factura.InvcRcptDetails.First().InvoiceReceipt.ChkReqDetail.First().CheckRequest.ChkReqPmtDate.Value.ToString("dd/MM/yyyy") : "-";

                if (invoiceReceipt != null)
                {
                    this.Fecha_Programada_Pago = invoiceReceipt.PaymentDate != null ? invoiceReceipt.PaymentDate.ToString("dd/MM/yyyy") : "-";
                    this.DatePr = invoiceReceipt.PaymentDate;
                }

            }
            this.Proveedor = factura.Vendors != null ? factura.Vendors.VendName : "";
            //this.Proveedor_Nombre = factura.Vendors.VendName;
            //this.VendKey = factura.Vendors.VendorKey;
            this.Subtotal = Math.Round(Convert.ToDecimal(factura.Subtotal), 2).ToString();
            this.Subtotal_In_Doouble = Convert.ToDouble(factura.Subtotal);
            this.Traslados = factura.ImpuestoImporteTrs != null ? Math.Round(Convert.ToDecimal(factura.ImpuestoImporteTrs), 2).ToString() : "0";
            this.Traslados_In_Double = Convert.ToDouble(factura.ImpuestoImporteTrs);
            this.Descuento = factura.Descuento != null ? Math.Round(Convert.ToDecimal(factura.Descuento), 2).ToString() : "0";
            this.Total = Math.Round(Convert.ToDecimal(factura.Total), 2).ToString();
            this.Total_In_Double = Convert.ToDouble(factura.Total);
            this.UUID = factura.UUID;

            //Int32 AprovUserKey = Convert.ToInt32(factura.AprovUserKey);
            //Users user = db.Users.Where(u => u.UserKey == AprovUserKey).FirstOrDefault();
            //this.Usuario = user != null ? user.UserName : string.Empty;

            StatusDocs status = null;
            if (factura.Status != null)
                status = db.StatusDocs.Where(s => s.Status == factura.Status).FirstOrDefault();
            this.Estado_Id = status != null ? status.Status.ToString() : "0";
            this.Estado = status != null ? status.Descripcion : "No definido";
            this.Moneda = factura.Moneda;
            //this.Tipo = factura.TranType;

            string[] DSage = REvisaPago(factura.VendorKey, factura.CompanyID.ToString(), Key);

            if (DSage[0] != null)
            {
                this.Fecha_Pago = DSage[0].ToString();
                if (Fecha_Pago == "-")
                {
                    this.DatePag = DateTime.MinValue;
                }
                else
                {
                    this.DatePag = DateTime.ParseExact(DSage[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                this.Banco_Pago = DSage[1].ToString();
                this.Cuenta_Pago = DSage[2].ToString();
                this.Fecha_Notificacion_Pago = DSage[3].ToString();
                if (Fecha_Notificacion_Pago == "-")
                {
                    this.DateNP = DateTime.MinValue;
                }
                else
                {
                    this.DateNP = DateTime.ParseExact(DSage[3], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                if (Pago != null)
                {
                    if (Pago.Payment != null)
                    {
                        this.Folio_Pago = Pago.Payment.Folio;
                        this.Fecha_FechaRecepcion_Pago = Pago.Payment.CreateDate != null ? Pago.Payment.CreateDate.ToString("dd/MM/yyyy") : "";
                        this.DateRcP = Pago.Payment.CreateDate;
                        if (Pago.Payment.AprovDate != null)
                        {
                            this.Fecha_FechaAprobacion_Pago = Pago.Payment.AprovDate.Value.ToString("dd/MM/yyyy");
                            this.DateArP = Pago.Payment.AprovDate.Value;
                        }
                        else
                        {
                            this.Fecha_FechaAprobacion_Pago = "-";
                            this.DateArP = DateTime.MinValue;
                        }
                    }
                    else
                    {
                        this.Fecha_FechaAprobacion_Pago = "-";
                        this.DateArP = DateTime.MinValue;
                    }


                }
                else
                {
                    this.Folio_Pago = "-";
                    this.Fecha_FechaRecepcion_Pago = "-";
                    this.Fecha_FechaAprobacion_Pago = "-";
                    this.DateArP = DateTime.MinValue;
                    this.DateRcP = DateTime.MinValue;
                }
            }
            else
            {
                this.Fecha_Pago = "-";
                this.Banco_Pago = "-";
                this.Cuenta_Pago = "-";
                this.Fecha_Notificacion_Pago = "-";
                this.Folio_Pago = "-";
                this.Fecha_FechaRecepcion_Pago = "-";
                this.Fecha_FechaAprobacion_Pago = "-";
                this.DateNP = DateTime.MinValue;
                this.DateArP = DateTime.MinValue;
                this.DateRcP = DateTime.MinValue;
                this.DatePag = DateTime.MinValue;
            }


            if (factura.InvcRcptDetails != null && factura.InvcRcptDetails.Count > 0)
            {
                InvoiceReceipt invoiceReceipt = null;
                InvcRcptDetails invcRcptDetails = factura.InvcRcptDetails.FirstOrDefault();
                if (invcRcptDetails != null)
                {
                    invoiceReceipt = invcRcptDetails.InvoiceReceipt;
                }
                this.Contrarecibo_Folio = invoiceReceipt != null ? invoiceReceipt.Folio : "";
                if (invoiceReceipt != null && invoiceReceipt.ChkReqDetail != null && invoiceReceipt.ChkReqDetail.Count > 0)
                {
                    ChkReqDetail chkReqDetail = invoiceReceipt.ChkReqDetail.FirstOrDefault();
                    CheckRequest checkRequest = null;
                    if (chkReqDetail != null)
                    {
                        checkRequest = chkReqDetail.CheckRequest;
                    }
                    this.Solicitud_Folio = checkRequest != null ? checkRequest.Serie : "";
                }

                else
                    this.Solicitud_Folio = "-";
            }
            else
            {
                this.Contrarecibo_Folio = "-";
                this.Solicitud_Folio = "-";
            }

            //this.Notas = new List<NotasDTO>();
            //if (this.Tipo == "IN")
            //{
            //    foreach (Invoice nota in factura.Invoice1.Where(i => i.InvoiceKey != factura.InvoiceKey)) // Recorrido por notas de débito y crédito
            //        this.Notas.Add(new NotasDTO(nota.InvoiceKey, factura.InvoiceKey));
            //}
            //else
            //    this.Notas = null;

        }
    }
}

public class FacturasAllDTOs
{
    public int Key { get; set; }
    public string Compania { get; set; }
    public string Folio { get; set; }
    public string Serie { get; set; }
    public string Fecha { get; set; }
    public DateTime DateFF { get; set; }
    public string Fecha_Recepcion { get; set; }
    public DateTime DateRc { get; set; }
    public string Fecha_Aprobacion { get; set; }
    public DateTime DateAr { get; set; }
    public string Fecha_Programada_Pago { get; set; }
    public DateTime DatePr { get; set; }
    public string Proveedor { get; set; }
    public string Proveedor_Nombre { get; set; }
    public string Subtotal { get; set; }
    public double Subtotal_In_Doouble { get; set; }
    public string Retenciones { get; set; }
    public string Traslados { get; set; }
    public double Traslados_In_Double { get; set; }
    public string Fecha_Pago { get; set; }
    public DateTime DatePag { get; set; }
    public string Banco_Pago { get; set; }
    public string Cuenta_Pago { get; set; }
    public string Fecha_Notificacion_Pago { get; set; }
    public DateTime DateNP { get; set; }
    public string Folio_Pago { get; set; }
    public string Fecha_FechaRecepcion_Pago { get; set; }
    public DateTime DateRcP { get; set; }
    public string Fecha_FechaAprobacion_Pago { get; set; }
    public DateTime DateArP { get; set; }
    public string Descuento { get; set; }
    public string Total { get; set; }
    public double Total_In_Double { get; set; }
    public string UUID { get; set; }
    public string Usuario { get; set; }
    public string Estado { get; set; }
    public string Estado_Id { get; set; }
    public string Estado_Img { get; set; }

    public string Moneda { get; set; }

    public string Tipo { get; set; }
    public string Orden_de_Compra { get; set; }

    public List<NotasDTO> Notas { get; set; }

    public int VendKey { get; set; }
    public string Contrarecibo_Folio { get; set; }

    public string Solicitud_Folio { get; set; }

    public FacturasAllDTOs()
    { }

    public string[] REvisaPago(int Vend, string company, int Vk)
    {
        string[] Pago = new string[4];
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("DTOPago", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@VendKey", Value = Vend });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@IDC", Value = company });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Fol", Value = Vk });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable Registros = new DataTable();
                Registros.Load(rdr);
                int Filas = Registros.Rows.Count;

                if (Filas >= 1)
                {
                    DataRow rows;
                    rows = Registros.Rows[0];
                    Pago[0] = (rows["FechaPago"].ToString());
                    Pago[1] = (rows["Banco"].ToString());
                    Pago[2] = (rows["Cuenta"].ToString());
                    Pago[3] = (rows["FechaNot"].ToString());
                }
            }
        }
        catch (Exception ex)
        {
            string ms = ex.Message;
        }

        return Pago;
    }

    public FacturasAllDTOs(int Key)
    {
        PortalProveedoresEntities db = new PortalProveedoresEntities();
        Invoice factura = db.Invoice.Where(f => f.InvoiceKey == Key).FirstOrDefault();
        PaymentAppl Pago = db.PaymentAppl.Where(a => a.ApplInvoiceKey == Key).FirstOrDefault();

        if (factura != null)
        {
            //this.Key = Key;
            //this.Compania = factura.CompanyID;
            this.Folio = factura.Folio;
            this.Serie = factura.Serie;
            this.Fecha = factura.FechaTransaccion != null ? factura.FechaTransaccion.Value.ToString("dd/MM/yyyy") : "";
            this.DateFF = factura.FechaTransaccion == null ? DateTime.MinValue : Convert.ToDateTime(factura.FechaTransaccion);
            this.Fecha_Recepcion = factura.UpdateDate != null ? factura.UpdateDate.Value.ToString("dd/MM/yyyy") : "";
            this.DateRc = factura.UpdateDate == null ? DateTime.MinValue : Convert.ToDateTime(factura.UpdateDate);
            this.Fecha_Aprobacion = factura.AprovDate != null ? factura.AprovDate.Value.ToString("dd/MM/yyyy") : "";
            this.DateAr = factura.AprovDate == null ? DateTime.MinValue : Convert.ToDateTime(factura.AprovDate);
            if (factura.InvcRcptDetails.Count == 0)
            {
                this.Fecha_Programada_Pago = "-";
                this.DatePr = DateTime.MinValue;
            }
            else
            {
                InvoiceReceipt invoiceReceipt = null;
                InvcRcptDetails invcRcptDetails = factura.InvcRcptDetails.FirstOrDefault();
                if (invcRcptDetails != null)
                {
                    invoiceReceipt = invcRcptDetails.InvoiceReceipt;
                }

                if (invoiceReceipt != null)
                {
                    this.Fecha_Programada_Pago = invoiceReceipt.PaymentDate != null ? invoiceReceipt.PaymentDate.ToString("dd/MM/yyyy") : "-";
                    this.DatePr = invoiceReceipt.PaymentDate;
                }

            }
            this.Proveedor = factura.Vendors != null ? factura.Vendors.VendName : "";
            this.Subtotal = Math.Round(Convert.ToDecimal(factura.Subtotal), 2).ToString();
            this.Subtotal_In_Doouble = Convert.ToDouble(factura.Subtotal);
            this.Traslados = factura.ImpuestoImporteTrs != null ? Math.Round(Convert.ToDecimal(factura.ImpuestoImporteTrs), 2).ToString() : "0";
            this.Traslados_In_Double = Convert.ToDouble(factura.ImpuestoImporteTrs);
            this.Descuento = factura.Descuento != null ? Math.Round(Convert.ToDecimal(factura.Descuento), 2).ToString() : "0";
            this.Total = Math.Round(Convert.ToDecimal(factura.Total), 2).ToString();
            this.Total_In_Double = Convert.ToDouble(factura.Total);
            this.UUID = factura.UUID;

            StatusDocs status = null;
            if (factura.Status != null)
                status = db.StatusDocs.Where(s => s.Status == factura.Status).FirstOrDefault();
            this.Estado_Id = status != null ? status.Status.ToString() : "0";
            this.Estado = status != null ? status.Descripcion : "No definido";
            this.Moneda = factura.Moneda;

            string[] DSage = REvisaPago(factura.VendorKey, factura.CompanyID.ToString(), Key);

            if (DSage[0] != null)
            {
                this.Fecha_Pago = DSage[0].ToString();
                if (Fecha_Pago == "-")
                {
                    this.DatePag = DateTime.MinValue;
                }
                else
                {
                    this.DatePag = DateTime.ParseExact(DSage[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                this.Banco_Pago = DSage[1].ToString();
                this.Cuenta_Pago = DSage[2].ToString();
                this.Fecha_Notificacion_Pago = DSage[3].ToString();
                if (Fecha_Notificacion_Pago == "-")
                {
                    this.DateNP = DateTime.MinValue;
                }
                else
                {
                    this.DateNP = DateTime.ParseExact(DSage[3], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                if (Pago != null)
                {
                    if (Pago.Payment != null)
                    {
                        this.Folio_Pago = Pago.Payment.Folio;
                        this.Fecha_FechaRecepcion_Pago = Pago.Payment.CreateDate != null ? Pago.Payment.CreateDate.ToString("dd/MM/yyyy") : "";
                        this.DateRcP = Pago.Payment.CreateDate;
                        if (Pago.Payment.AprovDate != null)
                        {
                            this.Fecha_FechaAprobacion_Pago = Pago.Payment.AprovDate.Value.ToString("dd/MM/yyyy");
                            this.DateArP = Pago.Payment.AprovDate.Value;
                        }
                        else
                        {
                            this.Fecha_FechaAprobacion_Pago = "-";
                            this.DateArP = DateTime.MinValue;
                        }
                    }
                    else
                    {
                        this.Fecha_FechaAprobacion_Pago = "-";
                        this.DateArP = DateTime.MinValue;
                    }


                }
                else
                {
                    this.Folio_Pago = "-";
                    this.Fecha_FechaRecepcion_Pago = "-";
                    this.Fecha_FechaAprobacion_Pago = "-";
                    this.DateArP = DateTime.MinValue;
                    this.DateRcP = DateTime.MinValue;
                }
            }
            else
            {
                this.Fecha_Pago = "-";
                this.Banco_Pago = "-";
                this.Cuenta_Pago = "-";
                this.Fecha_Notificacion_Pago = "-";
                this.Folio_Pago = "-";
                this.Fecha_FechaRecepcion_Pago = "-";
                this.Fecha_FechaAprobacion_Pago = "-";
                this.DateNP = DateTime.MinValue;
                this.DateArP = DateTime.MinValue;
                this.DateRcP = DateTime.MinValue;
                this.DatePag = DateTime.MinValue;
            }


            if (factura.InvcRcptDetails != null && factura.InvcRcptDetails.Count > 0)
            {
                InvoiceReceipt invoiceReceipt = null;
                InvcRcptDetails invcRcptDetails = factura.InvcRcptDetails.FirstOrDefault();
                if (invcRcptDetails != null)
                {
                    invoiceReceipt = invcRcptDetails.InvoiceReceipt;
                }
                this.Contrarecibo_Folio = invoiceReceipt != null ? invoiceReceipt.Folio : "";
                if (invoiceReceipt != null && invoiceReceipt.ChkReqDetail != null && invoiceReceipt.ChkReqDetail.Count > 0)
                {
                    ChkReqDetail chkReqDetail = invoiceReceipt.ChkReqDetail.FirstOrDefault();
                    CheckRequest checkRequest = null;
                    if (chkReqDetail != null)
                    {
                        checkRequest = chkReqDetail.CheckRequest;
                    }
                    this.Solicitud_Folio = checkRequest != null ? checkRequest.Serie : "";
                }

                else
                    this.Solicitud_Folio = "-";
            }
            else
            {
                this.Contrarecibo_Folio = "-";
                this.Solicitud_Folio = "-";
            }

        }
    }
}

public class Inv2020
{
    public int Key { get; set; }
    public string Serie { get; set; }
    public string Folio { get; set; }
    public string Proveedor { get; set; }
    public string Fecha { get; set; }
    public string Fecha_Recepcion { get; set; }
    public string Fecha_Aprobacion { get; set; }
    public string Subtotal { get; set; }
    public string Impuestos { get; set; }
    public string Total { get; set; }
    public string Contrarecibo_Folio { get; set; }
    public string Solicitud_Folio { get; set; }
    public string Fecha_Programada_Pago { get; set; }
    public string Fecha_Pago { get; set; }
    public string Banco_Pago { get; set; }
    public string Cuenta_Pago { get; set; }
    public string Fecha_Notificacion_Pago { get; set; }
    public string Folio_Pago { get; set; }
    public string Fecha_FechaRecepcion_Pago { get; set; }
    public string Fecha_FechaAprobacion_Pago { get; set; }
    public string Estado { get; set; }
    public string Estado_Img { get; set; }
    public string Moneda { get; set; }
    public string Estado_Id { get; set; }
    public decimal Subtotal_in_Doouble { get; set; }
    public decimal Traslados_in_Double { get; set; }
    public decimal Total_in_Double { get; set; }

    public Inv2020()
    {

    }
}

public class FacturasAll
{

    private static List<Inv2020> ObtenerDeTablaInvoice20(string order_col, string order_dir, string VendorId, string Folio, string Fecha, string FechaR, string FechaPP, string FechaP, string FolioP, string Banco, string contrarecibo, string solicitud, string Estado, string document_type, Expression<Func<Invoice, bool>> predicate)
    {
        try
        {
            //List<FacturasAllDTOs> facturas = new List<FacturasAllDTOs>();
            List<Inv2020> Lista_Facts = new List<Inv2020>();

            try
            {

                SqlConnection sqlConnection1 = new SqlConnection();
                sqlConnection1 = SqlConnectionDB("PortalConnection");
                sqlConnection1.Open();

                if (VendorId == "[-Seleccione proveedor-]") { VendorId = ""; }
                if (Banco == "[-Seleccione Banco-]") { Banco = ""; }
                if (Estado == "0") { Estado = ""; }



                string sSQL = "spSelectAllInv2020";
                List<SqlParameter> parsT = new List<SqlParameter>();
                parsT.Add(new SqlParameter("@Vendor", VendorId));
                parsT.Add(new SqlParameter("@Folio", Folio));
                parsT.Add(new SqlParameter("@Fecha", Fecha));
                parsT.Add(new SqlParameter("@FechaR", FechaR));
                parsT.Add(new SqlParameter("@FechaPP", FechaPP));
                parsT.Add(new SqlParameter("@FechaP", FechaP));
                parsT.Add(new SqlParameter("@FolioP", FolioP));
                parsT.Add(new SqlParameter("@Banco", Banco));
                parsT.Add(new SqlParameter("@contrarecibo", contrarecibo));
                parsT.Add(new SqlParameter("@solicitud", solicitud));
                parsT.Add(new SqlParameter("@Estatus", Estado));
                parsT.Add(new SqlParameter("@Orden", order_col));
                parsT.Add(new SqlParameter("@Dir", order_dir));

                using (SqlCommand Cmd = new SqlCommand(sSQL, sqlConnection1))
                {

                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandText = sSQL;

                    foreach (SqlParameter par in parsT)
                    {
                        Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);
                    }

                    SqlDataReader rdr = null;
                    rdr = Cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        if (rdr.GetInt32(0) != 0)
                        {
                            Inv2020 Fac = new Inv2020();
                            DateTime Fechad;

                            if (string.IsNullOrEmpty(rdr["Serie"].ToString())) { Fac.Serie = "-"; } else { Fac.Serie = rdr["Serie"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["Folio"].ToString())) { Fac.Folio = "-"; } else { Fac.Folio = rdr["Folio"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["Vendor"].ToString())) { Fac.Proveedor = "-"; } else { Fac.Proveedor = rdr["Vendor"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["FechaFa"].ToString())) { Fac.Fecha = "-"; } else { Fechad = Convert.ToDateTime(rdr["FechaFa"].ToString()); Fac.Fecha = Fechad.ToString("dd/MM/yyyy"); };
                            if (string.IsNullOrEmpty(rdr["FechaRc"].ToString())) { Fac.Fecha_Recepcion = "-"; } else { Fechad = Convert.ToDateTime(rdr["FechaRc"].ToString()); Fac.Fecha_Recepcion = Fechad.ToString("dd/MM/yyyy"); };
                            if (string.IsNullOrEmpty(rdr["FechaAp"].ToString())) { Fac.Fecha_Aprobacion = "-"; } else { Fechad = Convert.ToDateTime(rdr["FechaAp"].ToString()); Fac.Fecha_Aprobacion = Fechad.ToString("dd/MM/yyyy"); };
                            if (string.IsNullOrEmpty(rdr["Subtotal"].ToString())) { Fac.Subtotal = "-"; } else { Fac.Subtotal = rdr["Subtotal"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["Impuesto"].ToString())) { Fac.Impuestos = "-"; } else { Fac.Impuestos = rdr["Impuesto"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["Total"].ToString())) { Fac.Total = "-"; } else { Fac.Total = rdr["Total"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["Subtotal_"].ToString())) { Fac.Subtotal_in_Doouble = 0; } else { Fac.Subtotal_in_Doouble = Convert.ToDecimal(rdr["Subtotal_"].ToString()); };
                            if (string.IsNullOrEmpty(rdr["Impuesto_"].ToString())) { Fac.Traslados_in_Double = 0; } else { Fac.Traslados_in_Double = Convert.ToDecimal(rdr["Impuesto_"].ToString()); };
                            if (string.IsNullOrEmpty(rdr["Total_"].ToString())) { Fac.Total_in_Double = 0; } else { Fac.Total_in_Double = Convert.ToDecimal(rdr["Total_"].ToString()); };
                            if (string.IsNullOrEmpty(rdr["FolioC"].ToString())) { Fac.Contrarecibo_Folio = "-"; } else { Fac.Contrarecibo_Folio = rdr["FolioC"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["FolioSC"].ToString())) { Fac.Solicitud_Folio = "-"; } else { Fac.Solicitud_Folio = rdr["FolioSC"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["FechaPp"].ToString())) { Fac.Fecha_Programada_Pago = "-"; } else { Fechad = Convert.ToDateTime(rdr["FechaPp"].ToString()); Fac.Fecha_Programada_Pago = Fechad.ToString("dd/MM/yyyy"); };
                            if (string.IsNullOrEmpty(rdr["FechaP"].ToString())) { Fac.Fecha_Pago = "-"; } else { Fechad = Convert.ToDateTime(rdr["FechaP"].ToString()); Fac.Fecha_Pago = Fechad.ToString("dd/MM/yyyy"); };
                            if (string.IsNullOrEmpty(rdr["Banco"].ToString())) { Fac.Banco_Pago = "-"; } else { Fac.Banco_Pago = rdr["Banco"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["Cuenta"].ToString())) { Fac.Cuenta_Pago = "-"; } else { Fac.Cuenta_Pago = rdr["Cuenta"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["FechaNp"].ToString())) { Fac.Fecha_Notificacion_Pago = "-"; } else { Fac.Fecha_Notificacion_Pago = rdr["FechaNp"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["FolioCp"].ToString())) { Fac.Folio_Pago = "-"; } else { Fac.Folio_Pago = rdr["FolioCp"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["FechaRCP"].ToString())) { Fac.Fecha_FechaRecepcion_Pago = "-"; } else { Fechad = Convert.ToDateTime(rdr["FechaRCP"].ToString()); Fac.Fecha_FechaRecepcion_Pago = Fechad.ToString("dd/MM/yyyy"); };
                            if (string.IsNullOrEmpty(rdr["FechaACP"].ToString())) { Fac.Fecha_FechaAprobacion_Pago = "-"; } else { Fechad = Convert.ToDateTime(rdr["FechaACP"].ToString()); Fac.Fecha_FechaAprobacion_Pago = Fechad.ToString("dd/MM/yyyy"); };
                            if (string.IsNullOrEmpty(rdr["Status"].ToString())) { Fac.Estado = "-"; } else { Fac.Estado = rdr["Status"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["Imagen"].ToString())) { Fac.Estado_Img = "-"; } else { Fac.Estado_Img = rdr["Imagen"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["Moneda"].ToString())) { Fac.Moneda = "-"; } else { Fac.Moneda = rdr["Moneda"].ToString(); };
                            if (string.IsNullOrEmpty(rdr["Id"].ToString())) { Fac.Estado_Id = "-"; } else { Fac.Estado_Id = rdr["Id"].ToString(); };

                            Lista_Facts.Add(Fac);
                        }
                    }

                    sqlConnection1.Close();

                }


            }
            catch (Exception ex)
            {
                string err;
                err = ex.Message;
                HttpContext.Current.Session["Error"] = err;
                string Msj = string.Empty;
                StackTrace st = new StackTrace(ex, true);
                StackFrame frame = st.GetFrame(st.FrameCount - 1);
                int LogKey2, Userk;
                string Company = string.Empty;
                if (HttpContext.Current.Session["UserKey"] == null) { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
                if (HttpContext.Current.Session["IDCompany"] == null) { Msj = Msj + "," + "Variable IDCompany null"; Company = "MGS"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
                if (HttpContext.Current.Session["LogKey"] == null) { Msj = Msj + "," + "Variable LogKey null"; LogKey2 = 0; } else { LogKey2 = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
                Msj = Msj + ex.Message;
                string nombreMetodo = frame.GetMethod().Name.ToString();
                int linea = frame.GetFileLineNumber();
                Msj = Msj + " || Metodo : FacturaDTOs.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
                throw new MulticonsultingException(Msj);
            }

            return Lista_Facts;
        }
        catch
        {
            return new List<Inv2020>();
        }
    }

    private static List<Inv2020> Obtener20(string order_col, string order_dir, string VendorId, string Folio, string Fecha, string FechaR, string FechaPP, string FechaP, string FolioP, string Banco, string contrarecibo, string solicitud, string Estado, string document_type, bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            Expression<Func<Invoice, bool>> predicate;
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.TranType == document_type);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.TranType == document_type && VendorsIds.Contains(a.VendorKey));
            }

            return ObtenerDeTablaInvoice20(order_col, order_dir, VendorId, Folio, Fecha, FechaR, FechaPP, FechaP, FolioP, Banco, contrarecibo, solicitud, Estado, "IN", predicate);
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<Inv2020>();
        }
    }

    public static List<Inv2020> ObtenerFacturas20(string order_col, string order_dir, string VendorId, string Folio, string Fecha, string FechaR, string FechaPP, string FechaP, string FolioP, string Banco, string contrarecibo, string solicitud, string Estado)
    {
        return Obtener20(order_col, order_dir, VendorId, Folio, Fecha, FechaR, FechaPP, FechaP, FolioP, Banco, contrarecibo, solicitud, Estado, "IN");
    }

    public static List<Inv2020> ObtenerFacturas20(string VendorId, string Folio, string Serie, string Fecha, string FechaR, string Total, string UUID, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId.Contains("[-Seleccione proveedor-]"))
                VendorId = "";

            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";


            List<Inv2020> facturas = ObtenerFacturas20(VendorId, Folio, Serie, Fecha, FechaR, Total, UUID, Estado);

            return facturas;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<Inv2020>();
        }
    }

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
            return null;
            throw new MulticonsultingException(ex.Message);

        }
    }

    public FacturasAll()
    {

    }
    private static List<FacturasAllDTO> ObtenerDeTablaInvoice(Expression<Func<Invoice, bool>> predicate)
    {
        try
        {
            List<FacturasAllDTO> facturas = new List<FacturasAllDTO>();
            PortalProveedoresEntities db = new PortalProveedoresEntities();

            List<Invoice> list = db.Invoice.Where(predicate).ToList();

            foreach (Invoice invoice in list)
            {
                FacturasAllDTO factura = new FacturasAllDTO(invoice.InvoiceKey);
                facturas.Add(factura);
            }
            return facturas;
        }
        catch (Exception ex)
        {
            return new List<FacturasAllDTO>();
        }
    }
    private static List<FacturasAllDTO> Obtener(string document_type, bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            Expression<Func<Invoice, bool>> predicate;
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.TranType == document_type);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.TranType == document_type && VendorsIds.Contains(a.VendorKey));
            }

            return ObtenerDeTablaInvoice(predicate);
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<FacturasAllDTO>();
        }
    }
    public static List<FacturasAllDTO> ObtenerFacturas(bool directo_en_vista = false)
    {
        return Obtener("IN", directo_en_vista);
    }
    public static List<FacturasAllDTO> ObtenerFacturas(string VendorId, string Folio, string Fecha, string FechaR, string FechaPP, string FechaP, string FolioP, string Banco, string contrarecibo, string solicitud, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId.Contains("[-Seleccione proveedor-]"))
                VendorId = "";

            if (!string.IsNullOrWhiteSpace(Banco) && Banco.Contains("[-Seleccione Banco-]"))
                Banco = "";

            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";


            List<FacturasAllDTO> facturas = ObtenerFacturas();

            if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
                facturas = facturas.Where(f => f.Folio.ToUpper().Contains(Folio.ToUpper())).ToList(); //Folio Factura
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId != "null")
                facturas = facturas.Where(f => f.Proveedor.ToUpper().Contains(VendorId.ToUpper())).ToList();  //Proveedor
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                facturas = facturas.Where(f => f.Fecha == Fecha).ToList();  //Fecha de Factura
            if (!string.IsNullOrWhiteSpace(FechaR) && FechaR != "null")
                facturas = facturas.Where(f => f.Fecha_Recepcion == FechaR).ToList();  //Fecha Recepcion de Factura
            if (!string.IsNullOrWhiteSpace(contrarecibo) && contrarecibo != "null")
                facturas = facturas.Where(f => f.Contrarecibo_Folio.ToUpper().Contains(contrarecibo.ToUpper())).ToList(); // Folio ContraRecibo
            if (!string.IsNullOrWhiteSpace(solicitud) && solicitud != "null")
                facturas = facturas.Where(f => f.Solicitud_Folio.ToUpper().Contains(solicitud.ToUpper())).ToList();   // Folio de solicitud
            if (!string.IsNullOrWhiteSpace(FechaPP) && FechaPP != "null")
                facturas = facturas.Where(f => f.Fecha_Programada_Pago == FechaPP).ToList();  //Fecha Programada de Pago
            if (!string.IsNullOrWhiteSpace(FolioP) && FolioP != "null")
                facturas = facturas.Where(f => f.Folio_Pago.ToUpper().Contains(FolioP.ToUpper())).ToList();  //Folio de Pago
            if (!string.IsNullOrWhiteSpace(FechaP) && FechaP != "null")
                facturas = facturas.Where(f => f.Fecha_Pago == FechaP).ToList();  //Fecha de Pago
            if (!string.IsNullOrWhiteSpace(Banco) && Banco != "null")
                facturas = facturas.Where(f => f.Banco_Pago.ToUpper().Contains(Banco.ToUpper())).ToList();  //Banco Emisor
            if (!string.IsNullOrWhiteSpace(Estado) && Estado != "null")
            {
                Estado = Tools.GetDocumentStatusDescription(Estado);
                facturas = facturas.Where(p => p.Estado == Estado).ToList(); //Estado 
            }

            return facturas;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<FacturasAllDTO>();
        }
    }

}


