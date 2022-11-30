//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS
using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Linq;

public class PagoDTO
{
    public string Folio { get; set; }
    public string Serie { get; set; }
    public string Fecha { get; set; }
    public string FechaR { get; set; }
    public DateTime Date { get; set; }
    public DateTime DateR { get; set; }
    public string Proveedor { get; set; }
    public string Subtotal { get; set; }
    public double Subtotal_In_Double { get; set; }
    public string Total { get; set; }
    public double Total_In_Double { get; set; }
    public string UUID { get; set; }
    public string Usuario { get; set; }
    public string Estado { get; set; }
    public string Moneda { get; set; }

    public PagoDTO()
    { }

    public PagoDTO(string Moneda, string Folio, string Serie, string Fecha, string FechaR, string Proveedor, string Subotal, string Total, string UUID, string Usuario, string Estado, DateTime Date, DateTime DateR)
    {
        this.Folio = Folio;
        this.Serie = Serie;
        this.Fecha = Fecha;
        this.FechaR = FechaR;
        this.Date = Date == null ? DateTime.MinValue : Convert.ToDateTime(Date);
        this.DateR = DateR == null ? DateTime.MinValue : Convert.ToDateTime(DateR);
        this.Proveedor = Proveedor;
        this.Subtotal = Subotal;
        this.Subtotal_In_Double = Convert.ToDouble(Subtotal);
        this.Total = Total;
        this.Total_In_Double = Convert.ToDouble(Total);
        this.UUID = UUID;
        this.Usuario = Usuario;
        this.Estado = Estado;
        this.Moneda = Moneda;
    }
}

public class Pagos
{

    public Pagos()
    {
    }

    public static List<PagoDTO> ObtenerPagos(bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;

            #region ¿Qué usuario es y que tipo de usuario es? Si no es de TSYS devolver null en el método

            Users authenticated_user = Tools.UsuarioAutenticado();
            if (authenticated_user == null)
                return null;

            bool is_tsys_user = authenticated_user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (!is_tsys_user)
                return null;

            #endregion ¿Qué usuario es y que tipo de usuario es? Si no es de TSYS devolver null en el método


            PortalProveedoresEntities db = new PortalProveedoresEntities();
            List<Payment> list = db.Payment.ToList();
            List<PagoDTO> pagos = new List<PagoDTO>();
            foreach (Payment pago in list.Where(a => a.CompanyID == company.CompanyID))
            {
                int vendor_key = Convert.ToInt32(pago.VendorKey);
                Vendors vendor = db.Vendors.Where(v => v.VendorKey == vendor_key).FirstOrDefault();
                Int32 AprovUserKey = Convert.ToInt32(pago.AprovUserKey);
                Users user = db.Users.Where(u => u.UserKey == AprovUserKey).FirstOrDefault();
                Int32 LLavePago = Convert.ToInt32(pago.PaymentKey);
                PaymentAppl DtllPay = db.PaymentAppl.Where(u => u.PaymentKey == LLavePago).FirstOrDefault();
                //PaymentAppl DtllPay = db.PaymentAppl.Where(c => c.PaymentKey == pago.PaymentKey).FirstOrDefault();
                StatusDocs status = null;
                status = db.StatusDocs.Where(s => s.Status == pago.Status).FirstOrDefault();
                pagos.Add(new PagoDTO(
                    DtllPay != null ? DtllPay.Moneda : "",
                    pago.Folio,
                    pago.Serie,
                    pago.PaymentDate != null ? pago.PaymentDate.Date.ToString("dd/MM/yyyy") : string.Empty,
                    pago.CreateDate != null ? pago.CreateDate.Date.ToString("dd/MM/yyyy") : string.Empty,
                    vendor != null ? vendor.VendName : string.Empty,
                    pago.Subtotal != null ? Math.Round(Convert.ToDecimal(pago.Subtotal), 2).ToString() : "0",
                   Math.Round(Convert.ToDecimal(pago.Total), 2).ToString(),
                   pago.UUID,
                   user != null ? user.UserID : string.Empty,
                   status != null ? status.Descripcion : "No definido",
                   pago.PaymentDate != null ? pago.PaymentDate : DateTime.MinValue,
                   pago.CreateDate != null ? pago.CreateDate : DateTime.MinValue));
            }

            return pagos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<PagoDTO>();
        }
    }

    public static List<PagoDTO> ObtenerPagos(string Folio, string Serie, string Fecha, string FechaR, string Proveedor, string Total, string UUID, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";
            List<PagoDTO> pagos = ObtenerPagos();

            if (pagos == null)
                return null;

            if (!string.IsNullOrWhiteSpace(Folio) && Folio != "null")
                pagos = pagos.Where(p => p.Folio.ToUpper().Contains(Folio.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Serie) && Serie != "null")
                pagos = pagos.Where(p => p.Serie.ToUpper().Contains(Serie.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                pagos = pagos.Where(p => p.Fecha == Fecha).ToList();

            if (!string.IsNullOrWhiteSpace(FechaR) && FechaR != "null")
                pagos = pagos.Where(p => p.FechaR == FechaR).ToList();

            if (!string.IsNullOrWhiteSpace(Proveedor) && Proveedor != "null")
                pagos = pagos.Where(p => p.Proveedor.ToUpper().Contains(Proveedor.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
            {
                try
                {
                    double total = Convert.ToDouble(Total.Replace(",", "."));
                    pagos = pagos.Where(f => f.Total_In_Double == total).ToList();
                }
                catch
                {
                    throw new MulticonsultingException("El dato brindado como valor de total a filtrar no es válido");
                }
            }
            if (!string.IsNullOrWhiteSpace(UUID) && UUID != "null")
                pagos = pagos.Where(p => p.UUID.ToUpper().Contains(UUID.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Estado) && Estado != "null")
            {
                Estado = Tools.GetDocumentStatusDescription(Estado);
                pagos = pagos.Where(p => p.Estado == Estado).ToList();
            }

            return pagos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<PagoDTO>();
        }
    }

}

