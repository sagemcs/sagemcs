//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS
using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Linq;

public class UsuarioDTO
{
    public string Correo { get; set; }
    public string Nombre { get; set; }
    public string Compania { get; set; }
    //public string Proveedor { get; set; }
    //public string ProveedorId { get; set; }
    public string Interno { get; set; }
    public string Creacion { get; set; }
    public string Actualizacion { get; set; }
    public string Estado { get; set; }

    public UsuarioDTO()
    {

    }
    //public UsuarioDTO(string Correo, string Nombre, string Compania, string Proveedor, string ProveedorId, string Interno, string Creacion, string Actualizacion, string Estado)
    //{
    public UsuarioDTO(string Correo, string Nombre, string Compania, string Interno, string Creacion, string Actualizacion, string Estado)
    {

        this.Correo = Correo;
        this.Nombre = Nombre;
        this.Compania = Compania;
        //this.Proveedor = Proveedor;
        //this.ProveedorId = ProveedorId;
        this.Interno = Interno.ToUpper();
        this.Creacion = Creacion;
        this.Actualizacion = Actualizacion;
        this.Estado = Estado;
    }

}

public class Usuarios
{

    public Usuarios()
    {
    }

    public static List<UsuarioDTO> ObtenerUsuarios(bool directo_en_vista = false)
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

            PortalProveedoresEntities db = new PortalProveedoresEntities();

            bool is_tsys_user = authenticated_user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (!is_tsys_user)
            {
                //try
                //{
                //    StatusUsers status = null;
                //    List<UsuarioDTO> usuarios_list = new List<UsuarioDTO>();
                //    if (authenticated_user.Status != null)
                //        status = db.StatusUsers.Where(s => s.Status == authenticated_user.Status).FirstOrDefault();
                //    usuarios_list.Add(new UsuarioDTO(authenticated_user.UserID, authenticated_user.UserName, authenticated_user.Company.FirstOrDefault() != null ? authenticated_user.Company.First().CompanyName : string.Empty, authenticated_user.Vendors.FirstOrDefault() != null ? authenticated_user.Vendors.First().VendName : string.Empty, authenticated_user.Vendors.FirstOrDefault() != null ? authenticated_user.Vendors.First().VendorID : string.Empty, authenticated_user.Vendors.FirstOrDefault() != null ? "NO" : "SI", authenticated_user.CreateDate != null ? authenticated_user.CreateDate.Value.ToShortDateString() : string.Empty, authenticated_user.UpdateDate != null ? authenticated_user.UpdateDate.Value.ToShortDateString() : string.Empty, status != null ? status.Descripcion : "No definido"));
                //    return usuarios_list;
                //}
                //catch
                //{

                //}
                return null;
            }

            List<Users> list = db.UsersInCompany.Where(u => u.CompanyID == company.CompanyID).Select(u => u.Users).ToList();
            List<UsuarioDTO> usuarios = new List<UsuarioDTO>();

            foreach (Users user in list)
            {
                StatusUsers status = null;
                if (user.Status != null)
                    status = db.StatusUsers.Where(s => s.Status == user.Status).FirstOrDefault();
                //usuarios.Add(new UsuarioDTO(user.UserID, user.UserName, user.Company.FirstOrDefault() != null ? user.Company.First().CompanyName : string.Empty, user.Vendors.FirstOrDefault() != null ? user.Vendors.First().VendName : string.Empty, user.Vendors.FirstOrDefault() != null ? user.Vendors.First().VendorID : string.Empty, user.Vendors.FirstOrDefault() != null ? "NO" : "SI", user.CreateDate != null ? user.CreateDate.Value.ToShortDateString() : string.Empty, user.UpdateDate != null ? user.UpdateDate.Value.ToShortDateString() : string.Empty, status != null ? status.Descripcion : "No definido"));
                Company com = user.Company.FirstOrDefault();
                usuarios.Add(new UsuarioDTO(
                    user.UserID,
                    user.UserName,
                    com != null ? com.CompanyName : string.Empty,
                    user.Vendors.FirstOrDefault() != null ? "NO" : "SI",
                    user.CreateDate != null ? user.CreateDate.Value.ToShortDateString() : string.Empty,
                    user.UpdateDate != null ? user.UpdateDate.Value.ToShortDateString() : string.Empty,
                    status != null ? status.Descripcion : "No definido"));
            }

            return usuarios;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<UsuarioDTO>();
        }
    }

    public static List<UsuarioDTO> ObtenerUsuarios(string Nombre, string Proveedor, string Interno, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Estado) && (Estado == "0" || Estado == "00"))
                Estado = "";
            List<UsuarioDTO> usuarios = ObtenerUsuarios();

            if (usuarios == null)
                return null;

            if (!string.IsNullOrWhiteSpace(Proveedor) && Proveedor != "null")
                usuarios = usuarios.Where(u => u.Nombre.ToUpper().Contains(Proveedor.ToUpper())).ToList();

            if (!string.IsNullOrWhiteSpace(Nombre) && Nombre != "null")
                usuarios = usuarios.Where(u => u.Correo.ToUpper().Contains(Nombre.ToUpper())).ToList();

            //if (!string.IsNullOrWhiteSpace(Proveedor) && Proveedor != "null")
            //    usuarios = usuarios.Where(u => u.ProveedorId.ToUpper().Contains(Proveedor.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Interno) && Interno != "null")
            {
                Interno = Interno.ToUpper();
                usuarios = usuarios.Where(u => u.Interno == Interno).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Estado) && Estado != "null")
            {
                Estado = Tools.GetUserStatusDescription(Estado);
                usuarios = usuarios.Where(p => p.Estado == Estado).ToList();
            }
            return usuarios;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<UsuarioDTO>();
        }
    }

}

