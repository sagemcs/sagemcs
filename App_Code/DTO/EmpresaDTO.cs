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
/// Summary description for EmpresaDTO
/// </summary>
public class EmpresaDTO
{
    public string ID { get; set; }
    public string Nombre { get; set; }
    public string RFC { get; set; }
    public string Fecha { get; set; }
    public DateTime Date { get; set; }
    public string Actualizacion { get; set; }
    public string Estado { get; set; }

    public EmpresaDTO()
    { }

    public EmpresaDTO(string ID, string Nombre, string RFC, string Fecha, string Actualización, string Estado, DateTime Dat)
    {
        this.ID = ID;
        this.Nombre = Nombre;
        this.RFC = RFC;
        this.Fecha = Fecha;
        this.Date = Dat == null ? DateTime.MinValue : Convert.ToDateTime(Dat);
        this.Actualizacion = Actualización;
        this.Estado = Estado;
    }
}

public class Empresas
{

    public Empresas()
    {
    }

    public static List<EmpresaDTO> ObtenerEmpresas(bool directo_en_vista = false)
    {
        try
        {
            Company authenticated_company = Tools.EmpresaAutenticada();
            if (authenticated_company == null)
                return null;
            //throw new MulticonsultingException("No está autenticado por ninguna empresa");
            Users authenticated_user = Tools.UsuarioAutenticado();
            if (authenticated_user == null)
                return null;
            //throw new MulticonsultingException("No está autenticado");

            bool is_tsys_user = authenticated_user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (!is_tsys_user)
                return null;

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            sage500_appEntities db_sage = new sage500_appEntities();
            List<Company> list = is_tsys_user ? db.Company.ToList() : authenticated_user.Vendors.Select(v => v.Company).ToList();

            List<EmpresaDTO> empresas = new List<EmpresaDTO>();
            foreach (Company company in list)
            {
                tsmCompany tsmCompany = db_sage.tsmCompany.Where(c => c.CompanyID == company.CompanyID).FirstOrDefault();
                StatusUsers status = db.StatusUsers.Where(s => s.Status == company.Status).FirstOrDefault();
                empresas.Add(new EmpresaDTO(
                    company.CompanyID,
                    company.CompanyName,
                    tsmCompany != null ? tsmCompany.FedID : string.Empty,
                    company.CreateDate != null ? company.CreateDate.Date.ToString("dd/MM/yyyy") : string.Empty,
                    company.UpdateDate != null ? company.UpdateDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                    status != null ? status.Descripcion : "No definido",
                    company.CreateDate != null ? company.CreateDate : DateTime.MinValue));
            }
            return empresas;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<EmpresaDTO>();
        }
    }

    public static List<EmpresaDTO> ObtenerEmpresas(string Nombre, string RFC, bool directo_en_vista = false)
    {
        try
        {
            List<EmpresaDTO> empresas = ObtenerEmpresas();

            if (empresas == null)
                return null;

            if (!string.IsNullOrWhiteSpace(Nombre) && Nombre != "null")
                empresas = empresas.Where(e => e.Nombre.ToUpper().Contains(Nombre.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(RFC) && RFC != "null")
                empresas = empresas.Where(e => e.RFC.ToUpper().Contains(RFC.ToUpper())).ToList();
            return empresas;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<EmpresaDTO>();
        }
    }
}