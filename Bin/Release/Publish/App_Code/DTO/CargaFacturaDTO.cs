//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS
using Proveedores_Model;
using SAGE_Model;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Summary description for CargaFacturaDTO
/// </summary>
public class CargaFacturaDTO
{

    public string Factura { get; set; }
    public string Proveedor { get; set; }
    public string RFC { get; set; }
    public string Orden { get; set; }
    public string Subtotal { get; set; }
    public double Subtotal_in_Docuble { get; set; }
    public string Impuestos { get; set; }
    public double Impuestos_in_Docuble { get; set; }
    public string Total { get; set; }
    public double Total_in_Docuble { get; set; }
    public string Estado { get; set; }
    public string Retenciones { get; set; }
    public double Retenciones_in_Docuble { get; set; }
    public string Traslados { get; set; }
    public double Traslados_in_Docuble { get; set; }
    public string Moneda { get; set; }
    public int IdEstado { get; set; }

    public CargaFacturaDTO()
    {

    }

    public CargaFacturaDTO(string Moneda, string Factura, string Proveedor, string RFC, string Orden, string Subtotal, string Impuestos, string Total, string Status, int IdEstado)
    {
        this.Factura = Factura;
        this.Proveedor = Proveedor;
        this.RFC = RFC;
        this.Orden = Orden;
        this.Subtotal = Subtotal;
        this.Subtotal_in_Docuble = Convert.ToDouble(Subtotal);
        this.Impuestos = Impuestos;
        this.Impuestos_in_Docuble = Convert.ToDouble(Impuestos);
        this.Total = Total;
        this.Total_in_Docuble = Convert.ToDouble(Total);
        this.Estado = Status;
        this.Traslados = this.Retenciones = "0";
        this.Traslados_in_Docuble = Convert.ToDouble(Traslados);
        this.Moneda = Moneda;
        this.IdEstado = IdEstado;
    }
}


public class CargaFactura
{
    public CargaFactura()
    {
    }

    public static List<CargaFacturaDTO> ObtenerCargaFacturas(bool directo_en_vista = false)
    {
        try
        {
            #region ¿Qué usuario es y que tipo de usuario es? Si no es de TSYS devolver null en el método

            Users authenticated_user = Tools.UsuarioAutenticado();
            if (authenticated_user == null)
                return null;

            bool is_tsys_user = authenticated_user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (!is_tsys_user)
                return null;

            #endregion ¿Qué usuario es y que tipo de usuario es? Si no es de TSYS devolver null en el método

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            sage500_appEntities db_sage = new sage500_appEntities();
            List<CargaFacturaDTO> list_dto = new List<CargaFacturaDTO>();
            List<SAGE_Model.tapLoadFilesAP> list = db_sage.tapLoadFilesAP.ToList();

            foreach (var item in list)
            {
                tapVendor vendor = null;
                if (item.Vendkey != null)
                {
                    int vk = Convert.ToInt32(item.Vendkey);
                    vendor = db_sage.tapVendor.Where(b => b.VendKey == vk).FirstOrDefault();
                }

                tapLFStatus status = item.status == null ? null : db.tapLFStatus.Where(s => s.Status == item.status).FirstOrDefault();
                list_dto.Add(new CargaFacturaDTO(
                    item.CurrID.ToString(),
                    item.NumVoucher,
                    vendor != null ? vendor.VendName.ToString() : string.Empty,
                    item.RFC,
                    item.POTranID,
                    item.Subtotal != null ? Math.Round(Convert.ToDecimal(item.Subtotal), 2).ToString() : "0",
                //Math.Round(Convert.ToDecimal(item.Impuestos), 2).ToString(), Math.Round(Convert.ToDecimal(item.Total), 2).ToString(), status != null ? status.Descripcion : "No definido"));
                    item.Impuestos != null ? Math.Round(Convert.ToDecimal(item.Impuestos), 2).ToString() : "0",
                    item.Total != null ? Math.Round(Convert.ToDecimal(item.Total), 2).ToString() : "0",
                    status != null ? status.Descripcion : "No definido",
                    status.Status));
            }

            return list_dto;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<CargaFacturaDTO>();
        }
    }

    public static List<CargaFacturaDTO> ObtenerCargaFacturas(string NumVoucher, string VendId, string RFC, string POTranID, string Total, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendId) && VendId.Contains("[-Seleccione proveedor-]"))
                VendId = "";

            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";

            List<CargaFacturaDTO> items = ObtenerCargaFacturas();

            if (items == null)
                return null;

            if (!string.IsNullOrWhiteSpace(NumVoucher) && NumVoucher != "null")
                items = items.Where(a => a.Factura.ToUpper().Contains(NumVoucher.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(VendId) && VendId != "null")
                items = items.Where(a => a.Proveedor == VendId).ToList();
            if (!string.IsNullOrWhiteSpace(RFC) && RFC != "null")
                items = items.Where(e => e.RFC.ToUpper().Contains(RFC.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(POTranID) && POTranID != "null")
                items = items.Where(a => a.Orden.ToUpper().Contains(POTranID.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
                //items = items.Where(a => a.Total == Total).ToList();
                items = items.Where(a => a.Total.ToString().Contains(Total.ToString())).ToList();
            if (!string.IsNullOrWhiteSpace(Estado) && Estado != "null")
                items = items.Where(a => a.IdEstado.ToString() == Estado).ToList();
            return items;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<CargaFacturaDTO>();
        }
    }

}