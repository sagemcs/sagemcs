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
/// Summary description for AccesoDTO
/// </summary>
public class AccesoDTO
{
    public int Id { get; set; }
    public string Usuario { get; set; }
    public string Nombre { get; set; }
    public string Compania { get; set; }
    public string Fecha { get; set; }
    public DateTime Date { get; set; }
    public string IP { get; set; }

    public AccesoDTO()
    { }

    public AccesoDTO(int Key, string Usuario, string Nombre, string Compania, string Fecha, string IP, DateTime Date)
    {
        this.Id = Key;
        this.Usuario = Usuario;
        this.Nombre = Nombre;
        this.Compania = Compania;
        this.Fecha = Fecha;
        this.Date = Date == null ? DateTime.MinValue : Convert.ToDateTime(Date);
        this.IP = IP;
    }
}

public class Accesos
{
    public Accesos()
    {
    }

    public static List<AccesoDTO> ObtenerAccesosDB(string order_col, string order_dir, string Usuario, string Nombre, string IP, int start = 0, int length = 10, bool directo_en_vista = false)
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

            //PortalProveedoresEntities db = new PortalProveedoresEntities();
            //List<AccessLog> list = db.AccessLog.ToList();
            List<AccessLog> list = new List<AccessLog>();
            IEnumerable<AccessLog> query = getPredicate(company, Usuario, Nombre, IP);




            if (order_col == "1")
                if (order_dir == "desc")
                    query = query.OrderByDescending(a => a.Users.UserName);
                else
                    query = query.OrderBy(a => a.Users.UserName);
            else if (order_col == "2")
                if (order_dir == "desc")
                    query = query.OrderByDescending(a => a.LoginDate);
                else
                    query = query.OrderBy(a => a.LoginDate);
            else if (order_col == "3")
                if (order_dir == "desc")
                    query = query.OrderByDescending(a => a.IP);
                else
                    query = query.OrderBy(a => a.IP);
            else
                query = query.OrderBy(a => a.LogKey);

            //query = query.OrderBy(a => a.LogKey);
            query = query.Skip(start).Take(length);

            query.Where(x => query.OrderBy(y => y.LogKey).Select(y => y.LogKey).Skip(start).Take(length).Contains(x.LogKey));
            //.ToList();
            //string aa = query.ToString();
            list = query.ToList();
            //var list1 = query.Select(a => a);
            //list = list1.ToList();
            List<AccesoDTO> accesos = new List<AccesoDTO>();

            foreach (AccessLog access in list)
            {
                Company com = null;
                if (access.Users != null)
                {
                    com = access.Users.Company.FirstOrDefault();
                }
                accesos.Add(new AccesoDTO(
                    access.LogKey,
                    access.Users != null ? access.Users.UserID : "",
                    access.Users != null ? access.Users.UserName : "",
                    com != null ? com.CompanyName : string.Empty,
                    access.LoginDate != null ? access.LoginDate.Date.ToString("dd/MM/yyyy") : string.Empty,
                    access.IP,
                    access.LoginDate));
            }


            return accesos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<AccesoDTO>();
        }
    }

    public static int ObtenerCountAccesos(string Usuario, string Nombre, string IP)
    {
        Company company = Tools.EmpresaAutenticada();
        IEnumerable<AccessLog> query = getPredicate(company, Usuario, Nombre, IP);
        return query.Count();
    }

    public static IQueryable<AccessLog> getPredicate(Company company, string Usuario, string Nombre, string IP)
    {
        PortalProveedoresEntities db = new PortalProveedoresEntities();

        IQueryable<AccessLog> query;
        query = db.AccessLog.Where(a => a.CompanyID == company.CompanyID);

        if (!string.IsNullOrWhiteSpace(Usuario) && Usuario != "null")
            query = query.Where(a => a.Users.UserName.ToUpper().Contains(Usuario.ToUpper()));
        if (!string.IsNullOrWhiteSpace(Nombre) && Nombre != "null")
            //accesos = accesos.Where(a => a.Nombre.ToUpper().Contains(Nombre.ToUpper())).ToList();
            query = query.Where(a => a.LoginDate.ToShortDateString() == Usuario);
        if (!string.IsNullOrWhiteSpace(IP) && IP != "null")
            query = query.Where(a => a.IP == IP);

        return query;
    }

    public static List<AccesoDTO> ObtenerAccesos(string order_col, string order_dir, string Usuario, string Nombre, string IP, int start = 0, int length = 10, bool directo_en_vista = false)
    {
        try
        {
            List<AccesoDTO> accesos = ObtenerAccesosDB(order_col, order_dir, Usuario, Nombre, IP, start, length);

            if (accesos == null)
                return null;

            //if (!string.IsNullOrWhiteSpace(Usuario) && Usuario != "null")
            //    accesos = accesos.Where(a => a.Nombre.ToUpper().Contains(Usuario.ToUpper())).ToList();
            //if (!string.IsNullOrWhiteSpace(Nombre) && Nombre != "null")
            //    //accesos = accesos.Where(a => a.Nombre.ToUpper().Contains(Nombre.ToUpper())).ToList();
            //    accesos = accesos.Where(a => a.Fecha == Nombre).ToList();
            //if (!string.IsNullOrWhiteSpace(IP) && IP != "null")
            //    accesos = accesos.Where(a => a.IP == IP).ToList();
            return accesos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<AccesoDTO>();
        }
    }
}