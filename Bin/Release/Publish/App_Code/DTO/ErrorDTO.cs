//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS
using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Summary description for ErrorDTO
/// </summary>
public class ErrorDTO
{
    public string Usuario { get; set; }
    public string Compania { get; set; }
    public string Fecha { get; set; }
    public DateTime Date { get; set; }
    public string Comentario { get; set; }

    public ErrorDTO()
    { }

    public ErrorDTO(string Usuario, string Compañía, string Fecha, string Comentario, DateTime Dat)
    {
        this.Usuario = Usuario;
        this.Compania = Compañía;
        this.Fecha = Fecha;
        this.Date = Dat == null ? DateTime.MinValue : Dat;
        this.Comentario = Comentario;
    }
}

public class Errores
{
    public Errores()
    {
    }

    public static List<ErrorDTO> FiltroErrores(string Fecha, bool directo_en_vista = false)
    {
        try
        {
            List<ErrorDTO> Errores = ObtenerErrores();
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                Errores = Errores.Where(f => f.Fecha == Fecha).ToList();
            return Errores;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ErrorDTO>();
        }
    }

    public static List<ErrorDTO> ObtenerErrores(bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            //throw new MulticonsultingException("No está autenticado por ninguna empresa");
            Users authenticated_user = Tools.UsuarioAutenticado();
            if (authenticated_user == null)
                return null;
            //throw new MulticonsultingException("No está autenticado");

            bool is_tsys_user = authenticated_user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (!is_tsys_user)
                return null;
            //throw new MulticonsultingException("Acceso Denegado");

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            List<ErrorLog> list = db.ErrorLog.ToList();
            List<ErrorDTO> errores = new List<ErrorDTO>();

            foreach (ErrorLog error in list.Where(a => a.CompanyID == company.CompanyID))
            {

                Company comp = error.Users.Company.FirstOrDefault();
                errores.Add(new ErrorDTO(
                    error.Users != null ? error.Users.UserID : string.Empty,
                    comp != null ? comp.CompanyID : string.Empty,
                    error.ErrorDate != null ? error.ErrorDate.Date.ToString("dd/MM/yyyy") : string.Empty,
                    error.Message,
                    error.ErrorDate));
            }




            return errores;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ErrorDTO>();
        }
    }
}