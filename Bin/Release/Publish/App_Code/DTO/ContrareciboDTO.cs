//PORTAL DE PROVEDORES T|SYS|
//12 - MARZO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS
using CrystalDecisions.CrystalReports.Engine;
using Proveedores_Model;
using SAGE_Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI.WebControls;
/// <summary>
/// Summary description for ContrareciboDTO
/// </summary>
public class ContrareciboDTO
{
    public int Id { get; set; }
    public string Folio { get; set; }
    public string Proveedor { get; set; }
    public string RFC { get; set; }
    public string Fecha_Recepcion { get; set; }
    public DateTime DateRx { get; set; }
    public string Condiciones { get; set; }
    public Double Condiiones_In_Double { get; set; }
    public string Fecha_Programada_Pago { get; set; }
    public DateTime DatePr { get; set; }
    public string Moneda { get; set; }
    public string Fecha_AprobacionF { get; set; }
    public DateTime DateAr { get; set; }
    public string Total { get; set; }
    public double Total_In_Double { get; set; }
    public ContrareciboDTO()
    { }
    public ContrareciboDTO(string Moneda, int Key, string Folio, string Proveedor, string RFC, string Condiciones, string Fecha_Recepcion, string Fecha_Programada_Pago, string Total, int InvKey, DateTime Fr, DateTime Fpr)
    {
        this.Moneda = Moneda;
        this.Id = Key;
        this.Folio = Folio;
        this.Proveedor = Proveedor;
        this.RFC = RFC;
        this.Condiciones = Condiciones;
        //this.Condiiones_In_Double = Convert.ToDouble(Condiciones);
        this.Fecha_Recepcion = Fecha_Recepcion;
        this.DateRx = Fr == null ? DateTime.MinValue : Convert.ToDateTime(Fr);
        this.Fecha_Programada_Pago = Fecha_Programada_Pago;
        this.DatePr = Fpr == null ? DateTime.MinValue : Convert.ToDateTime(Fpr);
        this.Total = Total;
        this.Total_In_Double = Convert.ToDouble(Total);

        PortalProveedoresEntities db = new PortalProveedoresEntities();
        Vendors vendor = db.Vendors.Where(v => v.VendorID == this.Proveedor).FirstOrDefault();
        Invoice Factura = db.Invoice.Where(b => b.InvoiceKey == InvKey).FirstOrDefault();

        this.Fecha_AprobacionF = Factura.AprovDate == null ? "Dato No disponible" : Factura.AprovDate.Value.ToString("dd/MM/yyyy");
        this.DateAr = Factura.AprovDate == null ? DateTime.MinValue : Convert.ToDateTime(Factura.AprovDate);
    }
}

public class ContrareciboDTO2
{
    public int Id { get; set; }
    public string Folio { get; set; }
    public string Proveedor { get; set; }
    public string RFC { get; set; }
    public string Fecha_Recepcion { get; set; }
    public DateTime DateRx { get; set; }
    public string Condiciones { get; set; }
    public Double Condiiones_In_Double { get; set; }
    public string Fecha_Programada_Pago { get; set; }
    public DateTime DatePr { get; set; }
    public string Moneda { get; set; }
    public string Fecha_AprobacionF { get; set; }
    public DateTime DateAr { get; set; }
    public string Total { get; set; }
    public double Total_In_Double { get; set; }
    public string Aprobado { get; set; }
    public string Aprobador { get; set; }
    public string Bloqueo { get; set; }
    public string Genera { get; set; }
    public ContrareciboDTO2()
    { }
    public ContrareciboDTO2(string Moneda, int Key, string Folio, string Proveedor, string RFC, string Condiciones, string Fecha_Recepcion, string Fecha_Programada_Pago, string Total, int InvKey, DateTime Fr, DateTime Fpr, string Fechapro, DateTime Fapr, string Aprobadors, string Bloqueo, string Genera)
    {
        this.Moneda = Moneda;
        this.Id = Key;
        this.Folio = Folio;
        this.Proveedor = Proveedor;
        this.RFC = RFC;
        this.Condiciones = Condiciones;

        this.Fecha_Recepcion = Fecha_Recepcion == null ? "Dato No disponible" : Fecha_Recepcion;
        //this.Fecha_Recepcion = Fecha_Recepcion == null ? "Dato No disponible" : Convert.ToString(DateTime.ParseExact(Fecha_Recepcion, "dd/MM/YYYY", null).ToString());
        this.DateRx = Fr == null ? DateTime.MinValue : Convert.ToDateTime(Fr);

        this.Fecha_Programada_Pago = Fecha_Programada_Pago == null ? "Dato No disponible" : Fecha_Programada_Pago;
        //this.Fecha_Programada_Pago = Fecha_Programada_Pago == null ? "Dato No disponible" : Convert.ToString(DateTime.ParseExact(Fecha_Programada_Pago, "dd/MM/YYYY", null).ToString());
        this.DatePr = Fpr == null ? DateTime.MinValue : Convert.ToDateTime(Fpr);

        this.Total = Total;
        this.Total_In_Double = Convert.ToDouble(Total);

        //this.Condiiones_In_Double = Convert.ToDouble(Condiciones);
        //PortalProveedoresEntities db = new PortalProveedoresEntities();
        //Vendors vendor = db.Vendors.Where(v => v.VendorID == this.Proveedor).FirstOrDefault();
        //Invoice Factura = db.Invoice.Where(b => b.InvoiceKey == InvKey).FirstOrDefault();
        //this.Fecha_AprobacionF = Factura.AprovDate == null ? "Dato No disponible" : Factura.AprovDate.Value.ToString("dd/MM/yyyy");
        //this.DateAr = Factura.AprovDate == null ? DateTime.MinValue : Convert.ToDateTime(Factura.AprovDate);
        //string  sdate = Convert.ToString(DateTime.ParseExact(Fechapro, "dd/MM/YYYY", null).ToString());

        this.Fecha_AprobacionF = Fechapro == null ? "Dato No disponible" : Fechapro;
        //this.Fecha_AprobacionF = Fechapro == null ? "Dato No disponible" : Convert.ToString(DateTime.ParseExact(Fechapro, "dd/MM/YYYY", null).ToString());
        this.DateAr = Fapr == null ? DateTime.MinValue : Convert.ToDateTime(Fapr);

        this.Aprobado = Aprobadors;

        this.Bloqueo = Bloqueo;

        this.Genera = Genera;

    }
}

public class Contrarecibos
{
    public Contrarecibos()
    { }

    public static List<ContrareciboDTO> ObtenerContrarecibos(bool sin_solicitud = false, bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            Expression<Func<InvoiceReceipt, bool>> predicate;
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.CompanyID == company.CompanyID);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.CompanyID == company.CompanyID && VendorsIds.Contains(a.VendorKey));
            }

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            sage500_appEntities db_sage = new sage500_appEntities();
            List<InvoiceReceipt> list = (sin_solicitud) ? db.InvoiceReceipt.Where(predicate).Where(r => r.ChkReqDetail.Count == 0).ToList() : db.InvoiceReceipt.Where(predicate).ToList();
            List<ContrareciboDTO> contrarecibos = new List<ContrareciboDTO>();

            foreach (InvoiceReceipt contra in list.Where(a => a.CompanyID == company.CompanyID))
            {

                ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(contra.Vendors.VendorID);

                string DBA = "Información de Proveedor no encontrada";
                short? condiciones = 0;
                if (proveedor != null)
                {
                    if (!string.IsNullOrWhiteSpace(proveedor.RFC))
                        DBA = proveedor.RFC;
                    if (string.IsNullOrWhiteSpace(proveedor.Condiciones))
                        //throw new MulticonsultingException("El proveedor no tiene definido las condiciones de pago");
                        //condiciones = Convert.ToInt16(proveedor.Condiciones);
                        condiciones = null;
                }
                InvcRcptDetails invcRcptDetails = contra.InvcRcptDetails.FirstOrDefault();
                contrarecibos.Add(new ContrareciboDTO(
                    invcRcptDetails.Moneda,
                    contra.InvcRcptKey,
                    contra.Folio.ToString(),
                    contra.Vendors != null ? contra.Vendors.VendName : string.Empty,
                    DBA,
                    condiciones != null ? proveedor.Condiciones_Descripcion /*condiciones.ToString()*/ : "Información no encontrada",
                    contra.CreateDate != null ? contra.CreateDate.Date.ToString("dd/MM/yyy") : string.Empty,
                    contra.PaymentDate != null ? contra.PaymentDate.Date.ToString("dd/MM/yyy") : string.Empty,
                    Math.Round(contra.Total, 2).ToString(),
                    invcRcptDetails != null ? invcRcptDetails.InvoiceKey : -1,
                    contra.CreateDate != null ? contra.CreateDate : DateTime.MinValue,
                    contra.PaymentDate != null ? contra.PaymentDate : DateTime.MinValue));
            }

            return contrarecibos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ContrareciboDTO>();
        }
    }

    public static List<ContrareciboDTO> ObtenerContrarecibosEmpleado(bool sin_solicitud = false, bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            List<ContrareciboDTO> contrarecibos = new List<ContrareciboDTO>();
            string SQL = string.Empty;
            string usker = HttpContext.Current.Session["UserKey"].ToString();
            SQL = "spGetDocsEmpleados";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 1 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = usker });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Prov", Value = "" });
                if (conn.State == ConnectionState.Open)
                { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    contrarecibos.Add(new ContrareciboDTO(
                    rdr["Moneda"] != null ? rdr["Moneda"].ToString() : "MXN",
                    rdr["InvcRcptKey"] != null ? Convert.ToInt32(rdr["InvcRcptKey"].ToString()) : -1,
                    rdr["Folio"] != null ? rdr["Folio"].ToString() : "",
                    rdr["VendName"] != null ? rdr["VendName"].ToString() : "",
                    rdr["RFC"] != null ? rdr["RFC"].ToString() : "",
                    rdr["condiciones_desc"] != null ? rdr["condiciones_desc"].ToString() : "",
                    rdr["CreateDate"] != null ? rdr["CreateDate"].ToString() : "",
                    rdr["PaymentDate"] != null ? rdr["PaymentDate"].ToString() : "",
                    rdr["Total"] != null ? Math.Round(Convert.ToDecimal(rdr["Total"]), 2).ToString() : "0.00",
                    rdr["InvoiceKey"] != null ? Convert.ToInt32(rdr["InvoiceKey"].ToString()) : -1,
                    rdr["CreateDate"] != null ? Convert.ToDateTime(rdr["CreateDate"].ToString()) : DateTime.MinValue,
                    rdr["PaymentDate"] != null ? Convert.ToDateTime(rdr["PaymentDate"].ToString()) : DateTime.MinValue
                    ));
                }
                conn.Close();
            }

            return contrarecibos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ContrareciboDTO>();
        }
    }

    public static List<ContrareciboDTO> ObtenerContrarecibos(List<int> Keys_list, bool sin_solicitud = false, bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            Expression<Func<InvoiceReceipt, bool>> predicate;
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.CompanyID == company.CompanyID);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.CompanyID == company.CompanyID && VendorsIds.Contains(a.VendorKey));
            }

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            sage500_appEntities db_sage = new sage500_appEntities();
            List<InvoiceReceipt> list = (sin_solicitud) ? db.InvoiceReceipt.Where(predicate).Where(r => r.ChkReqDetail.Count == 0).ToList() : db.InvoiceReceipt.Where(predicate).ToList();

            List<ContrareciboDTO> contrarecibos = new List<ContrareciboDTO>();

            foreach (InvoiceReceipt contra in list.Where(a => Keys_list.Contains(a.InvcRcptKey)))
            {
                ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(contra.Vendors.VendorID);
                string DBA = "Información de Proveedor no encontrada";
                if (proveedor != null && !string.IsNullOrWhiteSpace(proveedor.RFC))
                    DBA = proveedor.RFC;

                InvcRcptDetails invcRcptDetails = contra.InvcRcptDetails.FirstOrDefault();
                contrarecibos.Add(new ContrareciboDTO(
                    invcRcptDetails != null ? invcRcptDetails.Moneda : "",
                    contra.InvcRcptKey,
                    contra.Folio.ToString(),
                    contra.Vendors != null ? contra.Vendors.VendorID : string.Empty,
                    DBA,
                    proveedor != null ? proveedor.Condiciones_Descripcion : "Información no encontrada" /*contra.PaymentTerms*/,
                    contra.RcptDate != null ? contra.RcptDate.ToShortDateString() : "",
                    contra.PaymentDate != null ? contra.PaymentDate.ToShortDateString() : "",
                    Math.Round(contra.Total, 2).ToString(),
                    invcRcptDetails != null ? invcRcptDetails.InvoiceKey : -1,
                    contra.CreateDate == null ? DateTime.MinValue : contra.CreateDate,
                    contra.PaymentDate == null ? DateTime.MinValue : contra.PaymentDate));
            }

            return contrarecibos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ContrareciboDTO>();
        }
    }

    public static List<ContrareciboDTO> ObtenerContrarecibos(string Folio, string Proveedor, string RFC, string Total, string Fecha, string FechaP, bool sin_solicitud = false, bool directo_en_vista = false)
    {
        try
        {
            List<ContrareciboDTO> contrarecibos = ObtenerContrarecibos(sin_solicitud);

            if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
            {
                contrarecibos = contrarecibos.Where(c => c.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Proveedor) && Proveedor != "null")
            {
                contrarecibos = contrarecibos.Where(c => c.Proveedor.ToUpper().Contains(Proveedor.ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(RFC) && RFC != "null")
            {
                contrarecibos = contrarecibos.Where(c => c.RFC.ToUpper().Contains(RFC.ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
            {
                try
                {
                    double total = Convert.ToDouble(Total.Replace(",", "."));
                    contrarecibos = contrarecibos.Where(f => f.Total_In_Double.ToString().Contains(total.ToString())).ToList();
                    //contrarecibos = contrarecibos.Where(f => f.Total_In_Double == total).ToList();
                }
                catch
                {
                    throw new MulticonsultingException("El dato brindado como valor de total a filtrar no es válido");
                }
            }
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
            {
                contrarecibos = contrarecibos.Where(c => c.Fecha_Recepcion == Fecha).ToList();
            }
            if (!string.IsNullOrWhiteSpace(FechaP) && FechaP != "null")
            {
                contrarecibos = contrarecibos.Where(c => c.Fecha_Programada_Pago == FechaP).ToList();
            }

            return contrarecibos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ContrareciboDTO>();
        }
    }

    public static List<ContrareciboDTO2> ObtenerContrarecibos_2_0(string order_col, string order_dir, string Folio, string Proveedor, string RFC, string Total, string Fecha, string FechaP, bool sin_solicitud = false, bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            Expression<Func<InvoiceReceipt, bool>> predicate;
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.CompanyID == company.CompanyID);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.CompanyID == company.CompanyID && VendorsIds.Contains(a.VendorKey));
            }

            List<ContrareciboDTO2> contrarecibos = new List<ContrareciboDTO2>();
            string roles = HttpContext.Current.Session["RolUser"].ToString();

            //string SQL = " SELECT T3.Moneda,t1.InvcRcptKey,t3.Folio,d.vendName,a.VendDBA,c.PmtTermsID,T1.CreateDate,T1.PaymentDate,t3.Total,t3.InvoiceKey,t1.Approved,t4.AprovDate,FORMAT (T1.CreateDate, 'dd/MM/yyyy ') as CreateDate,FORMAT (t1.PaymentDate, 'dd/MM/yyyy ') as PaymentDate,FORMAT (t4.AprovDate, 'dd/MM/yyyy ') as AprovDate ";
            string SQL = " SELECT T3.Moneda,t1.InvcRcptKey,t3.Folio,d.vendName,a.VendDBA,c.PmtTermsID,T1.CreateDate,T1.PaymentDate,t3.Total,t3.InvoiceKey,ISNULL(t5.username + ' -' + t7.roleID,''),t4.AprovDate,FORMAT (T1.CreateDate, 'dd/MM/yyyy ') as CreateDate,FORMAT (t1.PaymentDate, 'dd/MM/yyyy ') as PaymentDate,FORMAT (t4.AprovDate, 'dd/MM/yyyy ') as AprovDate ";
            SQL += "  FROM InvoiceReceipt t1 ";
            SQL += "  LEFT JOIN invcRcptDetails t3 on t1.InvcRcptKey = t3.InvcRcptKey ";
            SQL += "  LEFT JOIN invoice t4 on t3.InvoiceKey = t4.InvoiceKey ";
            SQL += "  LEFT JOIN Vendors d on t1.VendorKey = d.VendorKey ";
            SQL += "  LEFT JOIN[sage500_Portal].dbo.TapVendor a ON a.VendKey = d.VendorKey ";
            SQL += "  LEFT JOIN[sage500_Portal].dbo.tciPaymentTerms c ON a.PmtTermsKey = c.PmtTermsKey ";
            SQL += "  LEFT JOIN users t5 on t1.Aprobador = t5.userkey ";
            SQL += "  LEFT JOIN usersinroles t6 on t5.userkey = t6.userkey ";
            SQL += "  LEFT JOIN roles t7 on t6.rolekey = t7.rolekey ";
            SQL += "  WHERE NOT EXISTS(SELECT NULL FROM ChkReqDetail t2 WHERE t2.InvcRcptKey = t1.InvcRcptKey) ";

            string sSQL = "";

            if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
            {
                sSQL += " AND t3.Folio LIKE '%" + Folio + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(Proveedor) && Proveedor != "null")
            {
                sSQL += " AND d.vendName LIKE '%" + Proveedor + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(RFC) && RFC != "null")
            {
                sSQL += " AND a.VendDBA LIKE '%" + RFC + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
            {
                sSQL += " AND t3.Total LIKE '%" + Total + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
            {
                sSQL += " AND FORMAT (T1.CreateDate, ''dd/MM/yyyy '') LIKE '%" + Fecha + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(FechaP) && FechaP != "null")
            {
                sSQL += " AND FORMAT (t1.PaymentDate, ''dd/MM/yyyy '') LIKE '%" + FechaP + "%' ";
            }

            if (order_col == "1") { sSQL += " Order by t3.Folio   "; }
            if (order_col == "2") { sSQL += " Order by d.vendName "; }
            if (order_col == "3") { sSQL += " Order by a.VendDBA  "; }
            if (order_col == "4") { sSQL += " Order by c.PmtTermsID "; }
            if (order_col == "5") { sSQL += " Order by T1.CreateDate "; }
            if (order_col == "6") { sSQL += " Order by T1.PaymentDate "; }
            if (order_col == "7") { sSQL += " Order by t3.Total "; }
            if (order_dir == "desc") { sSQL += " desc "; } else { sSQL += " asc "; }

            SQL += sSQL;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelSO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Filtros", Value = sSQL });
                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader sqlQueryResult = cmd.ExecuteReader();
                while (sqlQueryResult.Read())
                {
                    string Moneda = string.Empty;
                    string Folioc = string.Empty;
                    string Vendor = string.Empty;
                    string RFCven = string.Empty;
                    string Condic = string.Empty;
                    string Create = string.Empty;
                    string Paymen = string.Empty;
                    string Totalc = string.Empty;
                    string Aprov = string.Empty;
                    string FecAp = string.Empty;
                    string Bloqueo = string.Empty;
                    string Genera = string.Empty;
                    int InvcR = -1;
                    int Invci = -1;
                    Bloqueo = "NO";
                    Genera = "NO";
                    DateTime fcrea = DateTime.MinValue;
                    DateTime fpaym = DateTime.MinValue;
                    DateTime fapro = DateTime.MinValue;
                    try
                    {
                        if (sqlQueryResult.GetValue(0).ToString() != null) { Moneda = sqlQueryResult.GetValue(0).ToString(); } else { Moneda = "MXN"; } // Moneda
                        if (sqlQueryResult.GetValue(2).ToString() != null) { Folioc = sqlQueryResult.GetValue(2).ToString(); } else { Folioc = ""; }   // Folio
                        if (sqlQueryResult.GetValue(3).ToString() != null) { Vendor = sqlQueryResult.GetValue(3).ToString(); } else { Vendor = ""; }   // VendName
                        if (sqlQueryResult.GetValue(4).ToString() != null) { RFCven = sqlQueryResult.GetValue(4).ToString(); } else { RFCven = ""; }   // RFC
                        if (sqlQueryResult.GetValue(5).ToString() != null) { Condic = sqlQueryResult.GetValue(5).ToString(); } else { Condic = ""; }   // Condiciones
                        if (sqlQueryResult.GetValue(6).ToString() != null) { Create = sqlQueryResult.GetValue(12).ToString(); } else { Create = ""; }   // CreateDate
                        if (sqlQueryResult.GetValue(7).ToString() != null) { Paymen = sqlQueryResult.GetValue(13).ToString(); } else { Paymen = ""; }   // PaymentDate
                        if (sqlQueryResult.GetValue(8).ToString() != null) { Totalc = sqlQueryResult.GetValue(8).ToString(); } else { Totalc = ""; }   // Total
                        if (sqlQueryResult.GetValue(10).ToString() != null) { Aprov = sqlQueryResult.GetValue(10).ToString(); } else { Aprov = ""; }    // Aprobador
                        if (sqlQueryResult.GetValue(11).ToString() != null) { FecAp = sqlQueryResult.GetValue(14).ToString(); } else { FecAp = ""; }    // Fecha Aprobacion

                        if (sqlQueryResult.GetValue(1).ToString() != null) { InvcR = Convert.ToInt32(sqlQueryResult.GetValue(1).ToString()); } else { InvcR = -1; } // InvcRcptKey
                        if (sqlQueryResult.GetValue(9).ToString() != null) { Invci = Convert.ToInt32(sqlQueryResult.GetValue(9).ToString()); } else { Invci = -1; } // InvoiceKey

                        if (sqlQueryResult.GetValue(6).ToString() != null) { fcrea = Convert.ToDateTime(sqlQueryResult.GetValue(6).ToString()); } else { fcrea = DateTime.MinValue; } // CreateDate
                        if (sqlQueryResult.GetValue(7).ToString() != null) { fpaym = Convert.ToDateTime(sqlQueryResult.GetValue(7).ToString()); } else { fpaym = DateTime.MinValue; } // PaymentDate
                        if (sqlQueryResult.GetValue(11).ToString() != null) { fapro = Convert.ToDateTime(sqlQueryResult.GetValue(11).ToString()); } else { fapro = DateTime.MinValue; }   // Fecha Aprobacion

                        if (Aprov != "" || Aprov != null) { Bloqueo = revisarol(Aprov, roles); }
                        if (roles.Contains("Admin") || roles.Contains("Finanzas")) { Genera = "SI"; }
                    }
                    catch (Exception ex)
                    {
                        var erro = ex.Message;
                    }

                    contrarecibos.Add(new ContrareciboDTO2(Moneda, InvcR, Folioc, Vendor, RFCven, Condic, Create, Paymen, Totalc, Invci, fcrea, fpaym, FecAp, fapro, Aprov, Bloqueo, Genera));
                }
                conn.Close();
            }


            //SqlConnection sqlConnection1 = new SqlConnection();
            //sqlConnection1 = SqlConnectionDB("PortalConnection");
            //sqlConnection1.Open();
            //using (var sqlQuery = new SqlCommand(SQL, sqlConnection1))
            //{
            //    using (var sqlQueryResult = sqlQuery.ExecuteReader())
            //        if (sqlQueryResult != null)
            //        {
            //            while (sqlQueryResult.Read())
            //            {
            //                string Moneda = string.Empty;
            //                string Folioc = string.Empty;
            //                string Vendor = string.Empty;
            //                string RFCven = string.Empty;
            //                string Condic = string.Empty;
            //                string Create = string.Empty;
            //                string Paymen = string.Empty;
            //                string Totalc = string.Empty;
            //                string Aprov = string.Empty;
            //                string FecAp = string.Empty;
            //                string Bloqueo = string.Empty;
            //                string Genera = string.Empty;
            //                int InvcR  = -1;
            //                int Invci  = -1;
            //                Bloqueo = "NO";
            //                Genera = "NO";
            //                DateTime fcrea = DateTime.MinValue;
            //                DateTime fpaym = DateTime.MinValue;
            //                DateTime fapro = DateTime.MinValue;
            //                try
            //                {
            //                    if (sqlQueryResult.GetValue(0).ToString() != null)  { Moneda = sqlQueryResult.GetValue(0).ToString(); } else { Moneda = "MXN"; } // Moneda
            //                    if (sqlQueryResult.GetValue(2).ToString() != null)  { Folioc = sqlQueryResult.GetValue(2).ToString(); } else { Folioc = ""; }   // Folio
            //                    if (sqlQueryResult.GetValue(3).ToString() != null)  { Vendor = sqlQueryResult.GetValue(3).ToString(); } else { Vendor = ""; }   // VendName
            //                    if (sqlQueryResult.GetValue(4).ToString() != null)  { RFCven = sqlQueryResult.GetValue(4).ToString(); } else { RFCven = ""; }   // RFC
            //                    if (sqlQueryResult.GetValue(5).ToString() != null)  { Condic = sqlQueryResult.GetValue(5).ToString(); } else { Condic = ""; }   // Condiciones
            //                    if (sqlQueryResult.GetValue(6).ToString() != null)  { Create = sqlQueryResult.GetValue(12).ToString(); } else { Create = ""; }   // CreateDate
            //                    if (sqlQueryResult.GetValue(7).ToString() != null)  { Paymen = sqlQueryResult.GetValue(13).ToString(); } else { Paymen = ""; }   // PaymentDate
            //                    if (sqlQueryResult.GetValue(8).ToString() != null)  { Totalc = sqlQueryResult.GetValue(8).ToString(); } else { Totalc = ""; }   // Total
            //                    if (sqlQueryResult.GetValue(10).ToString() != null) { Aprov = sqlQueryResult.GetValue(10).ToString(); } else { Aprov = ""; }    // Aprobador
            //                    if (sqlQueryResult.GetValue(11).ToString() != null) { FecAp = sqlQueryResult.GetValue(14).ToString(); } else { FecAp = ""; }    // Fecha Aprobacion

            //                    if (sqlQueryResult.GetValue(1).ToString() != null)  { InvcR = Convert.ToInt32(sqlQueryResult.GetValue(1).ToString()); } else { InvcR = -1; } // InvcRcptKey
            //                    if (sqlQueryResult.GetValue(9).ToString() != null)  { Invci = Convert.ToInt32(sqlQueryResult.GetValue(9).ToString()); } else { Invci = -1; } // InvoiceKey

            //                    if (sqlQueryResult.GetValue(6).ToString() != null)  { fcrea = Convert.ToDateTime(sqlQueryResult.GetValue(6).ToString()); }  else { fcrea = DateTime.MinValue; } // CreateDate
            //                    if (sqlQueryResult.GetValue(7).ToString() != null)  { fpaym = Convert.ToDateTime(sqlQueryResult.GetValue(7).ToString()); }  else { fpaym = DateTime.MinValue; } // PaymentDate
            //                    if (sqlQueryResult.GetValue(11).ToString() != null) { fapro = Convert.ToDateTime(sqlQueryResult.GetValue(11).ToString()); } else { fapro = DateTime.MinValue; }   // Fecha Aprobacion

            //                    if (Aprov != "" || Aprov != null){ Bloqueo = revisarol(Aprov,roles);}
            //                    if (roles.Contains("Admin") || roles.Contains("Tesoreria")) { Genera = "SI"; }
            //                }
            //                catch (Exception ex)
            //                {
            //                    var erro = ex.Message;
            //                }

            //                contrarecibos.Add(new ContrareciboDTO2(Moneda,InvcR,Folioc,Vendor,RFCven,Condic,Create,Paymen,Totalc,Invci,fcrea,fpaym,FecAp, fapro, Aprov, Bloqueo, Genera));
            //            }
            //        }
            //}
            //sqlConnection1.Close();

            return contrarecibos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ContrareciboDTO2>();
        }
    }

    protected static string revisarol(string aprov, string rol)
    {
        string valor = "SI";
        try
        {
            //if (rol == "T|SYS| - Admin") { valor = "SI"; return valor; }
            //if (rol == "T|SYS| - Validador" && aprov == "" || aprov == null) { valor = "SI"; return valor; }
            //if (rol == "T|SYS| - Tesoreria" && aprov.Contains("Validador")) { valor = "SI"; return valor; }
            //if (rol == "T|SYS| - Finanzas" && aprov.Contains("Tesoreria")) { valor = "SI"; return valor; }
            //if (rol == "T|SYS| - Finanzas" && aprov.Contains("Admin")) { valor = "SI"; return valor; }

            if (rol == "T|SYS| - Admin") { valor = "NO"; return valor; }
            if (rol == "T|SYS| - Validador" && aprov == "" || aprov == null) { valor = "NO"; return valor; }
            //if (rol == "T|SYS| - Validador" && aprov.Contains("Admin")) { valor = "NO"; return valor; }
            if (rol == "T|SYS| - Tesoreria" && aprov.Contains("Validador") || aprov.Contains("Admin")) { valor = "NO"; return valor; }
            if (rol == "T|SYS| - Finanzas" && aprov.Contains("Tesoreria") || aprov.Contains("Admin")) { valor = "NO"; return valor; }
            //if (rol == "T|SYS| - Finanzas" && aprov.Contains("Admin")) { valor = "NO"; return valor; }
        }
        catch (Exception ex) { }
        return valor;

    }

    public static string CrearToken()
    {
        string token = string.Empty;
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            string usker = HttpContext.Current.Session["UserKey"].ToString();
            string roles = HttpContext.Current.Session["RolUser"].ToString();
            string SQL = " ";
            SQL += " Select token From [SecurityToken] Where Userkey = " + usker + " AND CreationDate >= dateadd(minute,-15,getdate()) And Activo = 1";

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand(SQL, sqlConnection1))
            {
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        while (sqlQueryResult.Read())
                        {
                            try
                            {
                                if (sqlQueryResult.GetValue(0).ToString() != null) { token = sqlQueryResult.GetValue(0).ToString(); } else { token = ""; } // Moneda
                            }
                            catch (Exception ex)
                            {
                                var erro = ex.Message;
                            }
                        }
                    }
            }
            sqlConnection1.Close();

            if (token == "")
            {
                token = GeneraryEnviar();
            }
            else
            {
                token = "enviado";
            }
        }
        catch (Exception exp)
        {
            throw new MulticonsultingException(exp.ToString());
        }
        return token;
    }

    public static string GeneraryEnviar()
    {
        string token = string.Empty;
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            string usker = HttpContext.Current.Session["UserKey"].ToString();
            string roles = HttpContext.Current.Session["RolUser"].ToString();

            SqlConnection sqlConnection1 = new SqlConnection();

            //DESACTIVA TOKENS ANTERIOES
            try
            {
                sqlConnection1 = SqlConnectionDB("PortalConnection");
                sqlConnection1.Open();
                using (var sqlQuery = new SqlCommand("", sqlConnection1))
                {
                    sqlQuery.CommandText = "Update [SecurityToken] set Activo = 0 Where Userkey = " + usker;
                    sqlQuery.CommandType = CommandType.Text;
                    sqlQuery.ExecuteNonQuery();
                    token = "enviado";
                }
                sqlConnection1.Close();
            }
            catch (Exception exp)
            {
                token = "error";
            }

            //GENERA NUEVO TOKEN
            string PassNew = CrearPassword(8);
            string SSl = "insert into SecurityToken (Activo,userkey,token,CreationDate,ExpirationDate) values (1," + usker + ",'" + PassNew + "',GETDATE(),GETDATE())";
            try
            {
                sqlConnection1 = SqlConnectionDB("PortalConnection");
                sqlConnection1.Open();
                using (var sqlQuery = new SqlCommand("", sqlConnection1))
                {
                    sqlQuery.CommandText = SSl;
                    sqlQuery.CommandType = CommandType.Text;
                    sqlQuery.ExecuteNonQuery();
                    token = "enviado";
                }
                sqlConnection1.Close();
            }
            catch (Exception exp)
            {
                token = "error";
            }

            //ENVIA POR CORREO NUEVO TOKEN
            if (token == "enviado")
            {
                string Body;
                bool SendEmail;
                string correo = HttpContext.Current.User.Identity.Name.ToString();
                using (StreamReader reader = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/Account/Templates Email/GetTokenSolCh.html")))
                {
                    Body = reader.ReadToEnd();
                    Body = Body.Replace("{PassTemp}", PassNew);
                    Body = Body.Replace("{Time}", "15");
                }

                //correo = "luis.ang.b1one@gmail.com";
                SendEmail = Global.EmailGlobal(correo, Body, "Tu token de Seguridad Solicitud de Cheque");
                if (SendEmail == true)
                {
                    token = "enviado";
                }
                else
                {
                    token = "error envio";
                }
            }
        }
        catch (Exception exp)
        {
            throw new MulticonsultingException(exp.ToString());
        }
        return token;
    }

    public static string RevToken(string ids)
    {
        string token = string.Empty;
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            string usker = HttpContext.Current.Session["UserKey"].ToString();
            string roles = HttpContext.Current.Session["RolUser"].ToString();
            string SQL = " ";
            SQL += " Select token From [SecurityToken] Where Userkey = " + usker + " AND Token ='" + ids + "'  AND CreationDate >= dateadd(minute,-15,getdate()) And Activo = 1";

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand(SQL, sqlConnection1))
            {
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        while (sqlQueryResult.Read())
                        {
                            try
                            {
                                if (sqlQueryResult.GetValue(0).ToString() != null) { token = sqlQueryResult.GetValue(0).ToString(); } else { token = ""; } // Moneda
                            }
                            catch (Exception ex)
                            {
                                var erro = ex.Message;
                            }
                        }
                    }
            }
            sqlConnection1.Close();

            if (token == "")
            {
                token = "no valido";
            }
            else
            {
                token = "correcto";
            }
        }
        catch (Exception exp)
        {
            throw new MulticonsultingException(exp.ToString());
        }
        return token;
    }

    public static string updatetoken(string ids)
    {
        string token = string.Empty;
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            string usker = HttpContext.Current.Session["UserKey"].ToString();

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            try
            {
                var js = new JavaScriptSerializer();
                var UUIDs_obj = (IEnumerable<object>)js.DeserializeObject(ids);
                if (sqlConnection1.State == ConnectionState.Open) { sqlConnection1.Close(); }
                sqlConnection1.Open();
                foreach (var item in UUIDs_obj)
                {
                    using (var sqlQuery = new SqlCommand("", sqlConnection1))
                    {
                        sqlQuery.CommandText = "Update InvoiceReceipt set Aprobador = " + usker + " Where InvcRcptKey = " + item.ToString();
                        sqlQuery.CommandType = CommandType.Text;
                        sqlQuery.ExecuteNonQuery();
                        token = "actualizado";
                    }
                }
                sqlConnection1.Close();
            }
            catch (Exception exp)
            {
                token = "error al actualizar contrarrecibo";
            }
        }
        catch (Exception exp)
        {
            throw new MulticonsultingException(exp.ToString());
        }
        return token;
    }

    protected static string GetEmailAddress()
    {
        MembershipUser currUser = null;
        if (HttpContext.Current.User != null)
        {
            currUser = Membership.GetUser(true);
            return currUser.Email;
        }
        return currUser.Email;
    }

    private static string CrearPassword(int longitud)
    {
        string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        StringBuilder res = new StringBuilder();
        Random rnd = new Random();
        while (0 < longitud--)
        {
            res.Append(caracteres[rnd.Next(caracteres.Length)]);
        }
        return res.ToString();
    }

    public static List<ContrareciboDTO> ObtenerContrarecibosEmpleado(string Folio, string Proveedor, string RFC, string Total, string Fecha, string FechaP, bool sin_solicitud = false, bool directo_en_vista = false)
    {
        try
        {
            List<ContrareciboDTO> contrarecibos = ObtenerContrarecibosEmpleado(sin_solicitud);

            if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
            {
                contrarecibos = contrarecibos.Where(c => c.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Proveedor) && Proveedor != "null")
            {
                contrarecibos = contrarecibos.Where(c => c.Proveedor.ToUpper().Contains(Proveedor.ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(RFC) && RFC != "null")
            {
                contrarecibos = contrarecibos.Where(c => c.RFC.ToUpper().Contains(RFC.ToUpper())).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
            {
                try
                {
                    double total = Convert.ToDouble(Total.Replace(",", "."));
                    contrarecibos = contrarecibos.Where(f => f.Total_In_Double.ToString().Contains(total.ToString())).ToList();
                    //contrarecibos = contrarecibos.Where(f => f.Total_In_Double == total).ToList();
                }
                catch
                {
                    throw new MulticonsultingException("El dato brindado como valor de total a filtrar no es válido");
                }
            }
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
            {
                contrarecibos = contrarecibos.Where(c => c.Fecha_Recepcion == Fecha).ToList();
            }
            if (!string.IsNullOrWhiteSpace(FechaP) && FechaP != "null")
            {
                contrarecibos = contrarecibos.Where(c => c.Fecha_Programada_Pago == FechaP).ToList();
            }

            return contrarecibos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ContrareciboDTO>();
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
        }
    }

    public static string correo3(string VendID)
    {
        string correo = string.Empty;
        try
        {
            string SQL = "  Select ISNULL(f.UserID,(Select UserID from vendors a LEFT JOIN Users f on a.UserKey = f.UserKey Where VendorID = @VendID)) ";
            SQL += " As Correo from vendors a LEFT JOIN Users f on a.superior = f.UserKey Where VendorID = @VendID";
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand(SQL, sqlConnection1))
            {
                sqlQuery.Parameters.AddWithValue("@VendID", VendID);
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        while (sqlQueryResult.Read())
                        {
                            correo = Convert.ToString(sqlQueryResult.GetValue(0));
                        }
                    }
            }

            sqlConnection1.Close();

        }
        catch (Exception ec)
        {

        }
        return correo;

    }

    public static int GenerarContrarecibo(List<string> UUIDs, string rpt_path)
    {
        try
        {
            string Folios1 = string.Empty;
            string Fec = string.Empty;
            int Conta = 0;
            sage500_appEntities db_sage = new sage500_appEntities();

            string company_id = HttpContext.Current.Session["IDCompany"].ToString();
            tsmCompany company = db_sage.tsmCompany.Where(c => c.CompanyID == company_id).FirstOrDefault();
            if (company == null)
                throw new MulticonsultingException("No está autenticado por ninguna empresa");

            PortalProveedoresEntities db = new PortalProveedoresEntities();

            ReportDocument report_document = new ReportDocument();
            report_document.Load(rpt_path);
            //List<Invoice> list = db.Invoice.Where(i => i.CompanyID == company.CompanyID && UUIDs.Contains(i.UUID)).ToList();
            //La siguiente linea fue modificada por: Cesilio Hernández
            List<Invoice> list = db.Invoice.Where(i => i.CompanyID == company.CompanyID && UUIDs.Contains(i.UUID)).Where(i => i.Status.Value == 4).ToList();

            if (list.Count > 1 && list.Where(l => l.UpdateDate.Value.Date != list.First().UpdateDate.Value.Date).FirstOrDefault() != null)
                throw new MulticonsultingException("Hay facturas que son de diferentes fechas"); // Son de diferentes fechas
            if (list.Count > 1 && list.Where(l => l.VendorKey != list.First().VendorKey).FirstOrDefault() != null)
                throw new MulticonsultingException("Hay facturas que son de diferentes proveedores"); // Son de diferentes proveedores
            List<FacturaDTO> facturas = new List<FacturaDTO>();
            List<NotaDTO> Notas = new List<NotaDTO>();
            decimal Total = 0;

            foreach (Invoice invoice in list)
            {
                try
                {
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
                        //Nota.Total = "$ " + Nota.Total;
                        //Nota.Traslados = "$ " + Nota.Traslados;
                        //Nota.Subtotal = "$ " + Nota.Subtotal;
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
                    //factura.Total = "$ " + factura.Total;
                    //factura.Traslados = "$ " + factura.Traslados;
                    //factura.Subtotal = "$ " + factura.Subtotal;

                    facturas.Add(factura);
                }
                catch { }
            }

            if (list.Count > 0)
            {
                int folio = 1;
                DateTime Fecha_Recepcion = DateTime.MinValue;
                DateTime Fecha_Programada_Pago = DateTime.MinValue;
                string Terminos_de_Pago = "Información de proveedor no encontrada";
                InvoiceReceipt ultimo_contrarecibo = db.InvoiceReceipt.OrderByDescending(c => c.CreateDate).FirstOrDefault();
                if (ultimo_contrarecibo != null)
                    folio = Convert.ToInt32(ultimo_contrarecibo.Folio) + 1;

                ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(list.First().Vendors.VendorID);
                if (proveedor != null)
                {
                    if (string.IsNullOrWhiteSpace(proveedor.Condiciones))
                        throw new MulticonsultingException("El proveedor no tiene definido las condiciones de pago");

                    //Terminos_de_Pago = proveedor.Condiciones;
                    Terminos_de_Pago = "60";
                    Fecha_Recepcion = Tools.ObtenerFechaRecepcion(Convert.ToDateTime(list.First().UpdateDate));
                    //Fecha_Recepcion = Tools.ObtenerFechaRecepcion(Convert.ToDateTime(list.First().AprovDate));
                    Fec = Tools.FechaCortaEsp(DateTime.Now);
                    if (!string.IsNullOrWhiteSpace(Terminos_de_Pago))
                        Fecha_Programada_Pago = Tools.ObtenerFechaProgramadaPago(Fecha_Recepcion, Convert.ToInt32(proveedor.Condiciones));

                }
                else
                    throw new MulticonsultingException("No existe el proveedor"); // Vendor no encontrado en SAGE

                DateTime CreateDate = DateTime.Now;
                InvoiceReceipt nuevo_contrarecibo = new InvoiceReceipt()
                {
                    UserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"]),
                    VendorKey = list.First().Vendors.VendorKey,
                    CompanyID = company.CompanyID,
                    CreateDate = CreateDate,
                    CreateUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"]),
                    UpdateDate = CreateDate,
                    UpdateUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"]),
                    Folio = Tools.RellenoConCeros(folio),
                    RcptDate = Fecha_Recepcion,
                    PaymentDate = Fecha_Programada_Pago,
                    PaymentTerms = Terminos_de_Pago,
                    Total = Total
                };

                db.InvoiceReceipt.Add(nuevo_contrarecibo);
                db.SaveChanges();
                foreach (FacturaDTO factura in facturas)
                {
                    InvcRcptDetails details = new InvcRcptDetails()
                    {
                        InvcRcptKey = nuevo_contrarecibo.InvcRcptKey,
                        InvoiceKey = factura.Key,
                        UUID = factura.UUID,
                        Folio = factura.Folio,
                        Tipo = factura.Tipo,
                        Moneda = factura.Moneda,
                        IVA = Math.Round(Convert.ToDecimal(factura.Traslados), 2),
                        ISR = Math.Round(Convert.ToDecimal(factura.Retenciones), 2),
                        TotalTax = Convert.ToDecimal(factura.Retenciones) + Convert.ToDecimal(factura.Traslados),
                        Total = Convert.ToDecimal(factura.Total)
                    };
                    Folios1 = Folios1 + factura.Folio + ", ";
                    Conta = Conta + 1;
                    db.InvcRcptDetails.Add(details);
                }

                for (int i = 0; i < facturas.Count; i++)
                {
                    facturas[i].Total = "$ " + facturas[i].Total;
                    facturas[i].Traslados = "$ " + facturas[i].Traslados;
                    facturas[i].Subtotal = "$ " + facturas[i].Subtotal;
                }
                for (int i = 0; i < Notas.Count; i++)
                {
                    Notas[i].Total = "$ " + Notas[i].Total;
                    Notas[i].Traslados = "$ " + Notas[i].Traslados;
                    Notas[i].Subtotal = "$ " + Notas[i].Subtotal;
                }

                report_document.Database.Tables[0].SetDataSource(facturas);
                report_document.Database.Tables[1].SetDataSource(Notas);

                report_document.SetParameterValue("contrarecibo_no", nuevo_contrarecibo.Folio.ToString());
                report_document.SetParameterValue("razon_social_compannia", company.CompanyName);
                report_document.SetParameterValue("rfc", company.FedID);
                //report_document.SetParameterValue("fecha_pago", nuevo_contrarecibo.PaymentDate.ToShortDateString());
                report_document.SetParameterValue("fecha_pago", Tools.FechaCortaEsp(nuevo_contrarecibo.PaymentDate));
                report_document.SetParameterValue("fecha_datos", Tools.FechaEnEspañol(nuevo_contrarecibo.CreateDate));
                report_document.SetParameterValue("proveedor", proveedor.Social);
                report_document.SetParameterValue("total", Total);

                Stream stream = report_document.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                BinaryReader br = new BinaryReader(stream);

                InvcRcptFile invcRcptFile = new InvcRcptFile()
                {
                    InvcRcptkey = nuevo_contrarecibo.InvcRcptKey,
                    FileType = "pdf",
                    FileBinary = br.ReadBytes(Convert.ToInt32(stream.Length)),
                    Counter = (db.InvcRcptFile.Count() + 1).ToString()
                };
                db.InvcRcptFile.Add(invcRcptFile);
                db.SaveChanges();
                int TotalF = Folios1.Length - 2;
                string resultado = Folios1.Substring(0, TotalF);
                string Cadnv = string.Empty;
                if (Conta == 1)
                {
                    Cadnv = " la Factura : ";
                }
                else
                {
                    Cadnv = " las facturas : ";
                }

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Account/Templates Email/ConfirmacionRec.html")))
                {
                    body = reader.ReadToEnd();
                    body = body.Replace("{Folio}", resultado);
                    body = body.Replace("{Fecha}", Fec);
                    body = body.Replace("{Texto}", Cadnv);

                }

                string corre = correo3(list.First().Vendors.VendorID);
                if (corre == "")
                {
                    Global.EmailGlobalAdd(proveedor.Correo, body, "Contrarecibo", stream, "Contrarecibo.pdf");
                }
                else
                {
                    Global.EmailGlobalAdd(corre, body, "Contrarecibo", stream, "Contrarecibo.pdf");
                }

                Facturas.ActualizarEstadoFacturasSql();

                return nuevo_contrarecibo.InvcRcptKey;
            }
        }
        catch (Exception e)
        {
            throw new MulticonsultingException(e.Message, e.InnerException);
        }
        return -1;
    }
}