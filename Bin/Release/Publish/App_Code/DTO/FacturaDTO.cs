//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS
using Proveedores_Model;
using SAGE_Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

public class FacturaDTO
{
    public int Key { get; set; }
    public string Compania { get; set; }
    public string Folio { get; set; }
    public string Serie { get; set; }
    public string Fecha { get; set; }
    public DateTime Date { get; set; }
    public string Fecha_Recepcion { get; set; }
    public string Fecha_Aprobacion { get; set; }
    public DateTime DateR { get; set; }
    public DateTime DateA { get; set; }
    public string Proveedor { get; set; }
    public string Proveedor_Nombre { get; set; }
    public string Subtotal { get; set; }
    public double Subtotal_In_Double { get; set; }
    public string Retenciones { get; set; }
    public double Retenciones_In_Double { get; set; }
    public string Traslados { get; set; }
    public double Traslados_In_Double { get; set; }
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

    public List<NotaDTO> Notas { get; set; }

    public int VendKey { get; set; }
    public string Contrarecibo_Folio { get; set; }

    public string Solicitud_Folio { get; set; }

    public FacturaDTO()
    { }

    public FacturaDTO(int Key)
    {
        PortalProveedoresEntities db = new PortalProveedoresEntities();
        Invoice factura = db.Invoice.Where(f => f.InvoiceKey == Key).FirstOrDefault();
        if (factura != null)
        {
            this.Key = Key;
            this.Compania = factura.CompanyID;
            this.Folio = factura.Folio;
            this.Serie = factura.Serie;
            this.Fecha = factura.FechaTransaccion != null ? factura.FechaTransaccion.Value.ToString("dd/MM/yyyy") : "";
            this.Date = factura.FechaTransaccion == null ? DateTime.MinValue : Convert.ToDateTime(factura.FechaTransaccion);
            this.Fecha_Recepcion = factura.UpdateDate != null ? factura.UpdateDate.Value.ToString("dd/MM/yyyy") : "";
            this.Fecha_Aprobacion = factura.AprovDate != null ? factura.AprovDate.Value.ToString("dd/MM/yyyy") : "";
            this.DateR = factura.UpdateDate == null ? DateTime.MinValue : Convert.ToDateTime(factura.UpdateDate);
            this.DateA = factura.AprovDate == null ? DateTime.MinValue : Convert.ToDateTime(factura.AprovDate);
            //this.Date = factura.UpdateDate == null ? DateTime.MinValue : Convert.ToDateTime(Fecha_Recepcion);
            //this.DateRx = string.IsNullOrWhiteSpace(this.Fecha_Recepcion) ? DateTime.MinValue : Convert.ToDateTime(this.Fecha_Recepcion);
            this.Proveedor = factura.Vendors != null ? factura.Vendors.VendName : "";
            this.Proveedor_Nombre = factura.Vendors != null ? factura.Vendors.VendName : "";
            this.VendKey = factura.Vendors != null ? factura.Vendors.VendorKey : -1;
            this.Subtotal = Math.Round(Convert.ToDecimal(factura.Subtotal), 2).ToString();
            this.Retenciones = factura.ImpuestoImporteRtn != null ? Math.Round(Convert.ToDecimal(factura.ImpuestoImporteRtn), 2).ToString() : "0";
            this.Traslados = factura.ImpuestoImporteTrs != null ? Math.Round(Convert.ToDecimal(factura.ImpuestoImporteTrs), 2).ToString() : "0";
            this.Descuento = factura.Descuento != null ? Math.Round(Convert.ToDecimal(factura.Descuento), 2).ToString() : "0";
            this.Total = Math.Round(Convert.ToDecimal(factura.Total), 2).ToString();
            this.Subtotal_In_Double = Convert.ToDouble(factura.Subtotal);
            this.Traslados_In_Double = Convert.ToDouble(factura.ImpuestoImporteTrs);
            this.Retenciones_In_Double = Convert.ToDouble(factura.ImpuestoImporteRtn);
            this.Total_In_Double = Convert.ToDouble(factura.Total);
            this.UUID = factura.UUID;

            Int32 AprovUserKey = Convert.ToInt32(factura.AprovUserKey);
            Users user = db.Users.Where(u => u.UserKey == AprovUserKey).FirstOrDefault();
            //this.Usuario = user != null ? user.UserID : string.Empty;
            this.Usuario = user != null ? user.UserName : string.Empty;

            StatusDocs status = null;
            if (factura.Status != null)
                status = db.StatusDocs.Where(s => s.Status == factura.Status).FirstOrDefault();
            this.Estado_Id = status != null ? status.Status.ToString() : "0";
            //this.Estado_Img = status != null ? "<img src='/Img/" + status.Status.ToString() + ".png'>" : "<img src='/Img/0.png'>";
            this.Estado = status != null ? status.Descripcion : "No definido";
            this.Moneda = factura.Moneda;
            this.Tipo = factura.TranType;


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

            this.Notas = new List<NotaDTO>();
            if (this.Tipo == "IN")
            {
                foreach (Invoice nota in factura.Invoice1.Where(i => i.InvoiceKey != factura.InvoiceKey)) // Recorrido por notas de débito y crédito
                    this.Notas.Add(new NotaDTO(nota.InvoiceKey, factura.InvoiceKey));
            }
            else
                this.Notas = null;
        }
    }
}

public class FacturaDTOs
{
    public int Key { get; set; }
    public string Compania { get; set; }
    public string Folio { get; set; }
    public string Serie { get; set; }
    public string Fecha { get; set; }
    public DateTime Date { get; set; }
    public string Fecha_Recepcion { get; set; }
    public string Fecha_Aprobacion { get; set; }
    public DateTime DateR { get; set; }
    public DateTime DateA { get; set; }
    public string Proveedor { get; set; }
    public string Proveedor_Nombre { get; set; }
    public string Subtotal { get; set; }
    public double Subtotal_In_Double { get; set; }
    public string Retenciones { get; set; }
    public double Retenciones_In_Double { get; set; }
    public string Traslados { get; set; }
    public double Traslados_In_Double { get; set; }
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

    public List<NotaDTO> Notas { get; set; }

    public int VendKey { get; set; }
    public string Contrarecibo_Folio { get; set; }

    public string Solicitud_Folio { get; set; }

    public FacturaDTOs()
    { }

    public FacturaDTOs(int Key)
    {
        PortalProveedoresEntities db = new PortalProveedoresEntities();
        Invoice factura = db.Invoice.Where(f => f.InvoiceKey == Key).FirstOrDefault();

        if (factura != null)
        {
            this.Key = Key;
            this.Compania = factura.CompanyID;
            this.Folio = factura.Folio;
            this.Serie = factura.Serie;
            this.Fecha = factura.FechaTransaccion != null ? factura.FechaTransaccion.Value.ToString("dd/MM/yyyy") : "";
            this.Date = factura.FechaTransaccion == null ? DateTime.MinValue : Convert.ToDateTime(factura.FechaTransaccion);
            this.Fecha_Recepcion = factura.UpdateDate != null ? factura.UpdateDate.Value.ToString("dd/MM/yyyy") : "";
            this.Fecha_Aprobacion = factura.AprovDate != null ? factura.AprovDate.Value.ToString("dd/MM/yyyy") : "";
            this.DateR = factura.UpdateDate == null ? DateTime.MinValue : Convert.ToDateTime(factura.UpdateDate);
            this.DateA = factura.AprovDate == null ? DateTime.MinValue : Convert.ToDateTime(factura.AprovDate);
            this.Proveedor = factura.Vendors != null ? factura.Vendors.VendName : "";
            this.Proveedor_Nombre = factura.Vendors != null ? factura.Vendors.VendName : "";
            this.VendKey = factura.Vendors != null ? factura.Vendors.VendorKey : -1;
            this.Subtotal = Math.Round(Convert.ToDecimal(factura.Subtotal), 2).ToString();
            this.Retenciones = factura.ImpuestoImporteRtn != null ? Math.Round(Convert.ToDecimal(factura.ImpuestoImporteRtn), 2).ToString() : "0";
            this.Traslados = factura.ImpuestoImporteTrs != null ? Math.Round(Convert.ToDecimal(factura.ImpuestoImporteTrs), 2).ToString() : "0";
            this.Descuento = factura.Descuento != null ? Math.Round(Convert.ToDecimal(factura.Descuento), 2).ToString() : "0";
            this.Total = Math.Round(Convert.ToDecimal(factura.Total), 2).ToString();
            this.Subtotal_In_Double = Convert.ToDouble(factura.Subtotal);
            this.Traslados_In_Double = Convert.ToDouble(factura.ImpuestoImporteTrs);
            this.Retenciones_In_Double = Convert.ToDouble(factura.ImpuestoImporteRtn);
            this.Total_In_Double = Convert.ToDouble(factura.Total);
            this.UUID = factura.UUID;

            Int32 AprovUserKey = Convert.ToInt32(factura.AprovUserKey);
            Users user = db.Users.Where(u => u.UserKey == AprovUserKey).FirstOrDefault();
            this.Usuario = user != null ? user.UserName : string.Empty;

            StatusDocs status = null;
            if (factura.Status != null)
                status = db.StatusDocs.Where(s => s.Status == factura.Status).FirstOrDefault();
            this.Estado_Id = status != null ? status.Status.ToString() : "0";
            this.Estado = status != null ? status.Descripcion : "No definido";
            this.Moneda = factura.Moneda;
            this.Tipo = factura.TranType;


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

            this.Notas = new List<NotaDTO>();
            if (this.Tipo == "IN")
            {
                foreach (Invoice nota in factura.Invoice1.Where(i => i.InvoiceKey != factura.InvoiceKey)) // Recorrido por notas de débito y crédito
                    this.Notas.Add(new NotaDTO(nota.InvoiceKey, factura.InvoiceKey));
            }
            else
                this.Notas = null;
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
            return null;
            throw new MulticonsultingException(ex.Message);

        }
    }
}

public class NotaDTO : FacturaDTO
{

    public int ApplyToInvcKey { get; set; }

    public NotaDTO()
    { }

    public NotaDTO(int Key, int ApplyToInvcKey) : base(Key)
    {
        this.ApplyToInvcKey = ApplyToInvcKey;
    }

}


public class Facturas
{

    public Facturas()
    {


    }

    private static List<FacturaDTO> ObtenerDeTablaInvoice(Expression<Func<Invoice, bool>> predicate)
    {
        try
        {
            List<FacturaDTO> facturas = new List<FacturaDTO>();
            PortalProveedoresEntities db = new PortalProveedoresEntities();

            List<Invoice> list = db.Invoice.Where(predicate).ToList();

            foreach (Invoice invoice in list)
            {
                FacturaDTO factura = new FacturaDTO(invoice.InvoiceKey);
                facturas.Add(factura);
            }
            return facturas;
        }
        catch
        {
            return new List<FacturaDTO>();
        }
    }

    private static List<FacturaDTOs> ObtenerDeTablaInvoice20(string VendorId, string Folio, string Serie, string Fecha, string FechaR, string Total, string UUID, string Estado, string document_type, Expression<Func<Invoice, bool>> predicate, string order_col, string order_dir)
    {
        try
        {
            List<FacturaDTOs> facturas = new List<FacturaDTOs>();

            try
            {

                SqlConnection sqlConnection1 = new SqlConnection();
                sqlConnection1 = SqlConnectionDB("PortalConnection");
                sqlConnection1.Open();

                if (VendorId == "[-Seleccione proveedor-]") { VendorId = ""; }
                if (Estado == "0") { Estado = ""; }

                string sSQL = "spSelectInv2020";
                List<SqlParameter> parsT = new List<SqlParameter>();
                parsT.Add(new SqlParameter("@Vendor", VendorId));
                parsT.Add(new SqlParameter("@Folio", Folio));
                parsT.Add(new SqlParameter("@Serie", Serie));
                parsT.Add(new SqlParameter("@Fecha", Fecha));
                parsT.Add(new SqlParameter("@FechaR", FechaR));
                parsT.Add(new SqlParameter("@Total", Total));
                parsT.Add(new SqlParameter("@UUID", UUID));
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
                        if (rdr.GetInt32(0) != null)
                        {
                            FacturaDTOs factura = new FacturaDTOs(rdr.GetInt32(0));
                            facturas.Add(factura);
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

            return facturas;
        }
        catch
        {
            return new List<FacturaDTOs>();
        }
    }

    public static List<FacturaDTO> ObtenerPorFolio(int folio, bool directo_en_vista = false)
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
                predicate = (a => a.InvoiceKey == folio);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.InvoiceKey == folio && VendorsIds.Contains(a.VendorKey));
            }

            return ObtenerDeTablaInvoice(predicate);
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<FacturaDTO>();
        }
    }
    private static List<FacturaDTO> Obtener(string document_type, bool directo_en_vista = false)
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
            return new List<FacturaDTO>();
        }
    }

    private static List<FacturaDTOs> Obtener20(string order_col, string order_dir, string VendorId, string Folio, string Serie, string Fecha, string FechaR, string Total, string UUID, string Estado, string document_type, bool directo_en_vista = false)
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

            return ObtenerDeTablaInvoice20(VendorId, Folio, Serie, Fecha, FechaR, Total, UUID, Estado, "IN", predicate, order_col, order_dir);
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<FacturaDTOs>();
        }
    }
    public static List<FacturaDTO> ObtenerFacturas(bool directo_en_vista = false)
    {
        return Obtener("IN", directo_en_vista);
    }

    public static List<FacturaDTOs> ObtenerFacturas20(string VendorId, string Folio, string Serie, string Fecha, string FechaR, string Total, string UUID, string Estado, string order_col, string order_dir)
    {
        return Obtener20(order_col, order_dir, VendorId, Folio, Serie, Fecha, FechaR, Total, UUID, Estado, "IN");
    }
    public static List<FacturaDTO> ObtenerNotasDeCredito()
    {
        return Obtener("CM");
    }
    public static List<FacturaDTO> ObtenerNotasDeDebito()
    {
        return Obtener("DM");
    }

    public static List<FacturaDTO> ObtenerFacturas(List<string> UUIDs)
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
                predicate = (a => a.TranType == "IN" && UUIDs.Contains(a.UUID));
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.TranType == "IN" && a.CompanyID == company.CompanyID && UUIDs.Contains(a.UUID) && VendorsIds.Contains(a.VendorKey));
            }

            return ObtenerDeTablaInvoice(predicate);
        }
        catch
        {
            return new List<FacturaDTO>();
        }
    }

    public static List<FacturaDTO> ObtenerFacturasSinContrarecibo()
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
                //predicate = (a => a.TranType == "IN" && a.InvcRcptDetails.Count == 0);
                //La siguiente linea fue modificada por: Cesilio Hernández
                predicate = (a => a.TranType == "IN" && a.InvcRcptDetails.Count == 0 && a.Status == 4);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.TranType == "IN" && a.CompanyID == company.CompanyID && a.InvcRcptDetails.Count == 0 && VendorsIds.Contains(a.VendorKey));
            }

            List<FacturaDTO> facturas_sincontrarecibo = new List<FacturaDTO>();
            facturas_sincontrarecibo = ObtenerDeTablaInvoice(predicate);

            List<FacturaDTO> facturas = new List<FacturaDTO>();
            sage500_appEntities db_sage = new sage500_appEntities();

            foreach (FacturaDTO factura in facturas_sincontrarecibo)
                if (db_sage.tapPendVoucher.Where(p => p.CompanyID == factura.Compania && p.VendKey == factura.VendKey && p.TranNo == factura.Folio).FirstOrDefault() == null
                    && db_sage.tapVoucher.Where(p => p.CompanyID == factura.Compania && p.VendKey == factura.VendKey && p.TranNo == factura.Folio).FirstOrDefault() != null)
                    facturas.Add(factura);
            return facturas;
        }
        catch (Exception ex)
        {
            string exstring = ex.ToString();
            return new List<FacturaDTO>();
        }
    }

    public static List<FacturaDTO> ObtenerFacturasSinContrarecibo(string VendorId, string Folio, string Serie, string Fecha, string FechaAp, string Total, string UUID, string Estado)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId.Contains("[-Seleccione proveedor-]"))
                VendorId = "";
            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";
            List<FacturaDTO> facturas = ObtenerFacturasSinContrarecibo();
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId != "null")
                facturas = facturas.Where(f => f.Proveedor_Nombre.ToUpper().Contains(VendorId.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
                facturas = facturas.Where(f => f.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Serie) && Serie != "null")
                facturas = facturas.Where(f => f.Serie.ToUpper().Contains(Serie.ToUpper())).ToList();

            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                facturas = facturas.Where(f => f.Fecha_Recepcion == Fecha).ToList();

            if (!string.IsNullOrWhiteSpace(FechaAp) && FechaAp != "null")
                facturas = facturas.Where(f => f.Fecha_Aprobacion == FechaAp).ToList();

            if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
            {
                double total = Convert.ToDouble(Total.Replace(",", "."));
                facturas = facturas.Where(f => f.Total_In_Double == total).ToList();
            }
            if (!string.IsNullOrWhiteSpace(UUID) && UUID != "null")
                facturas = facturas.Where(f => f.UUID.ToUpper().Contains(UUID.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Estado) && Estado != "null")
            {
                Estado = Tools.GetDocumentStatusDescription(Estado);
                facturas = facturas.Where(p => p.Estado == Estado).ToList();
            }

            return facturas;
        }
        catch
        {
            return new List<FacturaDTO>();
        }
    }

    public static List<FacturaDTO> ObtenerFacturas(string VendorId, string Folio, string Fecha, string FechaR, string contrarecibo, string solicitud, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId.Contains("[-Seleccione proveedor-]"))
                VendorId = "";
            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";
            List<FacturaDTO> facturas = ObtenerFacturas();

            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId != "null")
                facturas = facturas.Where(f => f.Proveedor_Nombre.ToUpper().Contains(VendorId.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
                facturas = facturas.Where(f => f.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(contrarecibo) && contrarecibo != "null")
                facturas = facturas.Where(f => f.Contrarecibo_Folio.ToUpper().Contains(contrarecibo.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(solicitud) && solicitud != "null")
                facturas = facturas.Where(f => f.Solicitud_Folio.ToUpper().Contains(solicitud.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                facturas = facturas.Where(f => f.Fecha == Fecha).ToList();
            if (!string.IsNullOrWhiteSpace(FechaR) && FechaR != "null")
                facturas = facturas.Where(f => f.Fecha_Recepcion == FechaR).ToList();
            if (!string.IsNullOrWhiteSpace(Estado) && Estado != "null")
            {
                Estado = Tools.GetDocumentStatusDescription(Estado);
                facturas = facturas.Where(p => p.Estado == Estado).ToList();
            }

            return facturas;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<FacturaDTO>();
        }
    }


    public static List<FacturaDTO> ObtenerFacturas(string VendorId, string Folio, string Serie, string Fecha, string FechaR, string Total, string UUID, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId.Contains("[-Seleccione proveedor-]"))
                VendorId = "";
            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";
            List<FacturaDTO> facturas = ObtenerFacturas();

            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId != "null")
                facturas = facturas.Where(f => f.Proveedor_Nombre.ToUpper().Contains(VendorId.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
                facturas = facturas.Where(f => f.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Serie) && Serie != "null")
                facturas = facturas.Where(f => f.Serie.ToUpper().Contains(Serie.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                facturas = facturas.Where(f => f.Fecha == Fecha).ToList();
            if (!string.IsNullOrWhiteSpace(FechaR) && FechaR != "null")
                facturas = facturas.Where(f => f.Fecha_Recepcion == FechaR).ToList();
            if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
            {
                try
                {
                    double total = Convert.ToDouble(Total.Replace(",", "."));
                    facturas = facturas.Where(f => f.Total_In_Double == total).ToList();
                }
                catch
                {
                    throw new MulticonsultingException("El dato brindado como valor de total a filtrar no es válido");
                }
            }
            if (!string.IsNullOrWhiteSpace(UUID))
                facturas = facturas.Where(f => f.UUID.ToUpper().Contains(UUID.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Estado))
            {
                Estado = Tools.GetDocumentStatusDescription(Estado);
                facturas = facturas.Where(p => p.Estado == Estado).ToList();
            }

            return facturas;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<FacturaDTO>();
        }
    }

    public static List<FacturaDTOs> ObtenerFacturas20(string VendorId, string Folio, string Serie, string Fecha, string FechaR, string Total, string UUID, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId.Contains("[-Seleccione proveedor-]"))
                VendorId = "";

            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";


            List<FacturaDTOs> facturas = ObtenerFacturas20(VendorId, Folio, Serie, Fecha, FechaR, Total, UUID, Estado);



            //if (!string.IsNullOrWhiteSpace(VendorId) && VendorId != "null")
            //    facturas = facturas.Where(f => f.Proveedor_Nombre.ToUpper().Contains(VendorId.ToUpper())).ToList();


            //if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
            //    facturas = facturas.Where(f => f.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();


            //if (!string.IsNullOrWhiteSpace(Serie) && Serie != "null")
            //    facturas = facturas.Where(f => f.Serie.ToUpper().Contains(Serie.ToUpper())).ToList();


            //if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
            //    facturas = facturas.Where(f => f.Fecha == Fecha).ToList();


            //if (!string.IsNullOrWhiteSpace(FechaR) && FechaR != "null")
            //    facturas = facturas.Where(f => f.Fecha_Recepcion == FechaR).ToList();


            //if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
            //{
            //    try
            //    {
            //        double total = Convert.ToDouble(Total.Replace(",", "."));
            //        facturas = facturas.Where(f => f.Total_In_Double == total).ToList();
            //    }
            //    catch
            //    {
            //        throw new MulticonsultingException("El dato brindado como valor de total a filtrar no es válido");
            //    }
            //}


            //if (!string.IsNullOrWhiteSpace(UUID))
            //    facturas = facturas.Where(f => f.UUID.ToUpper().Contains(UUID.ToUpper())).ToList();


            //if (!string.IsNullOrWhiteSpace(Estado))
            //{
            //    Estado = Tools.GetDocumentStatusDescription(Estado);
            //    facturas = facturas.Where(p => p.Estado == Estado).ToList();
            //}

            return facturas;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<FacturaDTOs>();
        }
    }

    public static void ActualizarEstadoFacturas(bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return;
            sage500_appEntities db_sage = new sage500_appEntities();
            PortalProveedoresEntities db = new PortalProveedoresEntities();

            for (int i = 0; i < db.Invoice.Where(f => f.CompanyID == company.CompanyID).ToList().Count(); i++)
            {
                var factura = db.Invoice.ToList()[i];
                if (factura.Status < 7) // No se analizan las facturas en proceso de pago o pagadas (¿o eliminadas?)
                {
                    if (db_sage.tapVoucher.Where(p => p.CompanyID == factura.CompanyID && p.VendKey == factura.VendorKey && p.TranNo == factura.Folio).FirstOrDefault() != null)
                        factura.Status = 4;
                    if (factura.InvcRcptDetails != null && factura.InvcRcptDetails.Count > 0)
                    {
                        factura.Status = 5;
                        if (factura.InvcRcptDetails.First().InvoiceReceipt.ChkReqDetail != null && factura.InvcRcptDetails.First().InvoiceReceipt.ChkReqDetail.Count > 0)
                            factura.Status = 6;
                    }
                }
            }
            db.SaveChanges();
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
        }
    }



    public static void ActualizarEstadoFacturasSql(bool directo_en_vista = false)
    {
        try
        {
            string Rst = string.Empty;
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return;
            string Company = HttpContext.Current.Session["IDCompany"].ToString();

            if (!UpdateFacs(Company)) { Rst = "1"; }

            if (!UpdatePagos(Company)) { Rst = "2"; }

        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
        }
    }

    public static bool UpdateFacs(string Company)
    {
        bool Cancel = true;
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string sSQL;

            sSQL = "spUpdateInvoice";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@Company", Company));

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
                sqlConnection1.Close();

            }
        }
        catch (Exception ex)
        {
            Cancel = false;
            throw new MulticonsultingException(ex.Message);
        }
        return Cancel;
    }

    public static bool UpdatePagos(string Company)
    {
        bool Cancel = true;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spRevPago", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 6 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@VendKey", Value = 0 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@IDC", Value = Company });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Fol", Value = 0 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
            }

        }
        catch (Exception ex)
        {
            Cancel = false;
            throw new MulticonsultingException(ex.Message);
        }
        return Cancel;
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
            return null;
            throw new MulticonsultingException(ex.Message);

        }
    }

    //Empieza codigo de pago de luis
    public static string ActualizarPagoFacturas(string bytes1, string Cont, bool directo_en_vista = false)
    {
        try
        {
            int Filas = 0;
            try
            {
                var js = new JavaScriptSerializer();
                var UUIDs_obj = (IEnumerable<object>)js.DeserializeObject(Cont);
                string Dats = "";

                foreach (var item in UUIDs_obj)
                {
                    if (Dats == "")
                    {
                        Dats = item.ToString();
                    }
                    else
                    {
                        Dats = Dats + "," + item.ToString();
                    }

                }

                SqlConnection sqlConnection1 = new SqlConnection();
                sqlConnection1 = SqlConnectionDB("PortalConnection");
                if (sqlConnection1.State == ConnectionState.Open) { sqlConnection1.Close(); }
                sqlConnection1.Open();

                using (var sqlQuery = new SqlCommand("", sqlConnection1))
                {
                    sqlQuery.CommandText = "Select distinct VendorKey From invoice Where InvoiceKey IN ( " + Dats.ToString() + ")";
                    sqlQuery.CommandType = CommandType.Text;
                    SqlDataReader rdr = sqlQuery.ExecuteReader();
                    DataTable Registros = new DataTable();
                    Registros.Load(rdr);
                    Filas = Registros.Rows.Count;
                }
                sqlConnection1.Close();
            }
            catch (Exception exp)
            {
                //token = "error al actualizar contrarrecibo";
            }

            if (Filas > 1)
            {
                return "Ha seleccionado facturas que son de diferentes proveedores.";
            }


            //byte[] fileByte = Convert.FromBase64String(bytes1);
            string encodedStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(bytes1));
            byte[] fileByte = Convert.FromBase64String(encodedStr);



            Company company = Tools.EmpresaAutenticada();
            if (company == null) { return "Usuario no Autentificado"; }

            string Respuesta = "";
            try
            {
                var js = new JavaScriptSerializer();
                var UUIDs_obj = (IEnumerable<object>)js.DeserializeObject(Cont);

                foreach (var item in UUIDs_obj)
                {

                    SqlConnection sqlConnection1 = new SqlConnection();
                    sqlConnection1 = SqlConnectionDB("PortalConnection");
                    if (sqlConnection1.State == ConnectionState.Open) { sqlConnection1.Close(); }
                    sqlConnection1.Open();
                    using (var sqlQuery = new SqlCommand("", sqlConnection1))
                    {
                        sqlQuery.CommandText = "Select VendorKey as VK,Folio,ISNULL(NodeOC,'') as OC,GETDATE() as Fecha,Total from invoice where invoicekey = " + item.ToString() + " ";
                        sqlQuery.CommandType = CommandType.Text;
                        //sqlQuery.ExecuteNonQuery();
                        SqlDataReader rdr = sqlQuery.ExecuteReader();
                        while (rdr.Read())
                        {
                            try
                            {
                                string Vendor = rdr["VK"].ToString();
                                string Folio = rdr["Folio"].ToString();
                                string OCom = rdr["OC"].ToString();
                                string fec1 = rdr["Fecha"].ToString();
                                string fec2 = rdr["Fecha"].ToString();
                                string Totsl = rdr["Total"].ToString();

                                Folio = Folio.TrimEnd(' ');
                                string UUID = Folio + "-" + OCom;
                                string UUIDF = GETUUID(Folio, OCom, Vendor);

                                //Sube Archivos al Portal
                                string Respues = Execute(fileByte, Vendor, UUID, Folio, "", fec1, fec2, "4.0", Totsl).ToString();
                                if (Respues != "1")
                                {
                                    ResetPago(UUID);
                                    Respuesta = "Se encontrarón problemas al cargar los datos generales del comprobante, Verifica que estén correctos y/o que no se haya cargado anteriormente este documento ";
                                    return Respuesta;
                                }

                                if (Desglose(UUID, UUIDF, Totsl, Totsl, Totsl, Totsl, Folio, "MXN", "PUE") == false)
                                {
                                    ResetPago(UUID);
                                    Respuesta = "Se encontrarón problemas al cargar el desglose del comprobante, Verifica que los datos estén correctos";
                                    return Respuesta;
                                }
                                ActualizaStado2(UUIDF);
                                Respuesta = "Ok";
                            }
                            catch (Exception exp)
                            {

                            }
                        }
                    }
                    sqlConnection1.Close();

                }

            }
            catch (Exception exp)
            {
                //token = "error al actualizar contrarrecibo";
            }

            return Respuesta;

        }
        catch (Exception exp)
        {
            if (directo_en_vista)
            {
                throw new MulticonsultingException(exp.ToString());
            }
            return "Error al intentar Actualizar";
        }
    }

    private static bool ActualizaStado2(string UUID)
    {
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string compant = HttpContext.Current.Session["IDCompany"].ToString();
            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "Update Invoice Set Status = 8 Where UUID ='" + UUID + "' AND CompanyID = '" + compant + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }

    }

    protected static void ResetPago(string UU)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Caden = "spResetPay";
                SqlCommand cmd1 = new SqlCommand(Caden, conn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.Parameters.Add(new SqlParameter()
                { ParameterName = "@UUID", Value = UU });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
            }
        }
        catch (Exception ex)
        {

        }

    }

    protected static string GETUUID(string Fecha, string OC, string Vendor)
    {
        string Cuenta = "";
        try
        {
            string sql;


            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            sql = @"SELECT top 1 UUID From Invoice a inner join Vendors b on a.VendorKey = b.vendorkey Where Folio ='" + Fecha + "' And b.VendName = '" + Vendor + "' AND NodeOC = '" + OC + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();
        }
        catch (Exception ex)
        {
            int iLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int iUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string CompanyID = HttpContext.Current.Session["IDCompany"].ToString();
            //LogError(iLogKey, iUserKey, "Carga de Complemento de Pagos_VerFechaP", ex.Message, CompanyID);
            Cuenta = "";
        }
        //return Rest;
        return Cuenta;
    }

    private static bool Desglose(string UUIDP, string Fac, string pago, string Parcial, string SalT, string SAlN, string FolP, string Mon, string Meto)
    {
        bool Res = false;
        int key = 0;
        int UUID = 0;
        string Fk = string.Empty;
        string Pk = string.Empty;

        try
        {
            // foreach (GridViewRow gvr in GridView2.Rows)
            //{
            SalT = SalT.Replace(",", ".");
            SAlN = SAlN.Replace(",", ".");
            pago = pago.Replace(",", ".");
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Caden = "spGetPayAppl";
                SqlCommand cmd1 = new SqlCommand(Caden, conn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.Parameters.Add(new SqlParameter()
                { ParameterName = "@UPago", Value = Fac });

                cmd1.Parameters.Add(new SqlParameter()
                { ParameterName = "@Factura", Value = UUIDP });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();

                while (rdr1.Read())
                {
                    Pk = rdr1["PKey"].ToString();
                    Fk = rdr1["FKey"].ToString();
                }

                key = int.Parse(Pk.ToString());
                UUID = int.Parse(Fk.ToString());


                if (UUID != 0)
                {
                    /////////////////////////////////////////////////////////////////////////////////

                    string Cades = "spInsertPayAppl";
                    string Result = string.Empty;
                    SqlCommand cmd = new SqlCommand(Cades, conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@PaymentKey", Value = key });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@ApplUUID", Value = UUIDP });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@ApplFolio", Value = FolP });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@PaymtApplied", Value = pago });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@PartialNumber", Value = 1 });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@BalanceAnt", Value = SalT });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@BalanceOut", Value = "0.00" });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@Moneda", Value = Mon });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@MetodoPago", Value = Meto });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@Folio", Value = UUID });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        if (rdr["Resultado"].ToString() == "1")
                        {
                            Res = true;
                        }
                        else
                        {
                            Res = false;
                        }
                    }
                }
            }
            //}
        }
        catch (Exception ex)
        {
            string Error = ex.Message;
            Res = false;
        }
        return Res;
    }

    public static string Execute(Byte[] bytes1, string VendKey, string UUID, string Folio, string Serie, string F1, string F2, string Version, string Total)
    {
        string res;

        try
        {
            string Filename = "Archivo de PAgo TSYS";

            //Fabian
            //Byte[] bytes1 = FileUpload2.FileBytes;
            //Byte[] bytes2 = FileUpload1.FileBytes;
            //}
            string Ext = ".XML";
            string Ext2 = ".pdf";
            string Var1 = HttpContext.Current.Session["IDComTran"].ToString();
            string Var2 = HttpContext.Current.Session["VendKey"].ToString();
            string Var3 = HttpContext.Current.Session["UserKey"].ToString();
            //int VendKey = GetVkey();
            int UserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());

            Total = Total.Replace(",", ".");

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Cades = "spInsertPay";
                string Result = string.Empty;
                SqlCommand cmd = new SqlCommand(Cades, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@File", Value = bytes1 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@File2", Value = bytes1 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Vendedor", Value = VendKey });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = HttpContext.Current.Session["IDCompany"].ToString() });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserKey", Value = UserKey });

                //cmd.Parameters.Add(new SqlParameter()
                //{ ParameterName = "@F1", Value = F1 });

                //cmd.Parameters.Add(new SqlParameter()
                //{ ParameterName = "@F2", Value = F2 });

                DateTime Fe1 = Convert.ToDateTime(F1);
                DateTime Fe2 = Convert.ToDateTime(F2);

                String Fe1w = String.Format(Fe1.ToShortDateString(), "dd/mm/aaaa");
                String Fe2w = String.Format(Fe2.ToShortDateString(), "dd/mm/aaaa");

                //DateTime Fe1 = DateTime.ParseExact(F1, "dd/MM/yyyy", null);
                //DateTime Fe2 = DateTime.ParseExact(F2, "dd/MM/yyyy", null);

                cmd.Parameters.Add(new SqlParameter("@F1", SqlDbType.DateTime) { Value = Fe1w });
                cmd.Parameters.Add(new SqlParameter("@F2", SqlDbType.DateTime) { Value = Fe2w });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UUID", Value = UUID });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Serie", Value = Serie });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Folio", Value = Folio });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Version", Value = Version });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Total", Value = Total });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Ext", Value = Ext });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Ext2", Value = Ext2 });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    if (rdr["Resultado"].ToString() == "1")
                    {
                        Result = rdr["Resultado"].ToString();
                    }
                    else
                    {
                        Result = rdr["Resultado"].ToString();
                        throw new Exception(Result);
                    }
                }
                res = Result;
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return res;
        }
        catch (Exception Rt)
        {
            res = Rt.Message;
            return res;
        }
    }
}

