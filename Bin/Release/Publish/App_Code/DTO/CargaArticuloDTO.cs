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
/// Summary description for CargaArticuloDTO
/// </summary>
public class CargaArticuloDTO
{
    public string Articulo { get; set; }
    public string Cantidad { get; set; }
    public string Costo { get; set; }
    public string Monto { get; set; }
    public string Comentario { get; set; }
    public string Estado { get; set; }
    public string IDEstado { get; set; }
    public double Cantidad_inDouble { get; set; }
    public double Costo_inDouble { get; set; }
    public double Monto_inDouble { get; set; }
    public CargaArticuloDTO()
    {

    }

    public CargaArticuloDTO(string IDEstado, string Articulo, string Cantidad, string Costo, string Monto, string Comentario, string Status)
    {
        this.Articulo = Articulo;
        this.Cantidad = Cantidad;
        this.Cantidad_inDouble = Convert.ToDouble(Cantidad);
        this.Costo = Costo;
        this.Costo_inDouble = Convert.ToDouble(Costo);
        this.Monto = Monto;
        this.Monto_inDouble = Convert.ToDouble(Monto);
        this.Comentario = Comentario;
        this.Estado = Status;
        this.IDEstado = IDEstado;
    }

}


public class CargaArticulo
{
    public CargaArticulo()
    {
    }

    public static List<CargaArticuloDTO> ObtenerCargaArticulos(bool directo_en_vista = false)
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
            List<CargaArticuloDTO> list_dto = new List<CargaArticuloDTO>();
            List<SAGE_Model.tapLoadFilesAPDtl> list = db_sage.tapLoadFilesAPDtl.ToList();

            foreach (var item in list)
            {
                //tapLFStatus status = item.status == null ? null : db.tapLFStatus.Where(s => s.Status == item.status).FirstOrDefault();
                //list_dto.Add(new CargaArticuloDTO(item.ItemID, item.Qty != null ? item.Qty.ToString() : "0", item.UnitCost != null ? Math.Round(Convert.ToDecimal(item.UnitCost), 2).ToString() : "0",
                //item.TranAmt != null ? Math.Round(Convert.ToDecimal(item.TranAmt), 2).ToString() : "0", item.DescError, status != null ? status.Descripcion : "No definido"));

                tapLFStatus status = item.status == null ? null : db.tapLFStatus.Where(s => s.Status == item.status).FirstOrDefault();
                list_dto.Add(new CargaArticuloDTO(
                    item.status.ToString(),
                    item.ItemID,
                    item.Qty != null ? Math.Round(Convert.ToDecimal(item.Qty), 4).ToString() : "0",
                    item.UnitCost != null ? Math.Round(Convert.ToDecimal(item.UnitCost), 2).ToString() : "0",
                    item.TranAmt != null ? Math.Round(Convert.ToDecimal(item.TranAmt), 2).ToString() : "0",
                    item.DescError,
                    status != null ? status.Descripcion : "No definido"));



            }

            return list_dto;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<CargaArticuloDTO>();
        }
    }



    public static List<CargaArticuloDTO> ObtenerCargaArticulos(string ItemID, string Qty, string Status, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Status) && Status == "0")
                Status = "";

            List<CargaArticuloDTO> items = ObtenerCargaArticulos();

            if (items == null)
                return null;

            if (!string.IsNullOrWhiteSpace(ItemID) && ItemID != "null")
                items = items.Where(a => a.Articulo.ToUpper().Contains(ItemID.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Qty) && Qty != "null")
                //items = items.Where(a => a.Cantidad == Qty).ToList();
                items = items.Where(a => a.Cantidad.ToString().Contains(Qty.ToString())).ToList();
            if (!string.IsNullOrWhiteSpace(Status) && Status != "null")
                items = items.Where(a => a.IDEstado == Status).ToList();
            return items;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<CargaArticuloDTO>();
        }
    }

}