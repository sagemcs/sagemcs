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
/// Summary description for ValidacionDTO
/// </summary>
public class ValidacionDTO
{

    public string Articulo { get; set; }
    public string Fecha { get; set; }
    public DateTime Date { get; set; }
    public string Descripcion { get; set; }
    public string Etapa { get; set; }


    public ValidacionDTO()
    {

    }
    public ValidacionDTO(string Articulo, string Fecha, string Descripcion, string Etapa, DateTime Date)
    {
        this.Articulo = Articulo;
        this.Fecha = Fecha;
        this.Date = Date == null ? DateTime.MinValue : Convert.ToDateTime(Date);
        this.Descripcion = Descripcion;
        this.Etapa = Etapa;
    }
}


public class Validaciones
{
    public Validaciones()
    {
    }

    public static List<ValidacionDTO> ObtenerValidaciones(bool directo_en_vista = false)
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
            List<SAGE_Model.tapAPILogValidacion> list = db_sage.tapAPILogValidacion.ToList();
            //List<tapAPILogValidacion> list = new List<tapAPILogValidacion>();

            List<ValidacionDTO> list_dto = new List<ValidacionDTO>();
            foreach (var item in list)
            {
                SAGE_Model.tapLoadFilesAPDtl a = db_sage.tapLoadFilesAPDtl.Where(b => b.dtlKey == item.dtlKey).FirstOrDefault();
                //tapLoadFilesAPDtl a = db_sage.tapLoadFilesAPDtl.Where(b => b.dtlKey == item.dtlKey).FirstOrDefault();
                if (a != null)
                    list_dto.Add(new ValidacionDTO(
                        a != null ? a.ItemID : "-",
                        item.fechaError != null ? item.fechaError.Value.Date.ToString("dd/MM/yyyy") : "",
                        item.ErrorValidacion,
                        item.Proceso,
                        item.fechaError == null ? DateTime.MinValue : item.fechaError.Value.Date));
            }

            return list_dto;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ValidacionDTO>();
        }
    }

    public static List<ValidacionDTO> ObtenerValidaciones(string ItemID, string fechaerror, bool directo_en_vista = false)
    {
        try
        {
            List<ValidacionDTO> items = ObtenerValidaciones();

            if (items == null)
                return null;

            if (!string.IsNullOrWhiteSpace(ItemID) && ItemID != "null")
                items = items.Where(a => a.Articulo.ToUpper().Contains(ItemID.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(fechaerror) && fechaerror != "null")
                items = items.Where(a => a.Fecha == fechaerror).ToList();

            return items;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ValidacionDTO>();
        }
    }

}