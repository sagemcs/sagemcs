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
using System.Linq;
using System.Linq.Expressions;
using System.Web;
public class ProveedorDTO
{
    public string ID { get; set; }
    public string Nombre { get; set; }
    public string Social { get; set; }
    public string Compania { get; set; }
    public string RFC { get; set; }
    public string Fecha { get; set; }
    public DateTime Date { get; set; }
    public DateTime DateA { get; set; }
    public string Actualizacion { get; set; }
    public string Estado { get; set; }

    public string Correo { get; set; }

    public string Condiciones { get; set; }
    public string Condiciones_Descripcion { get; set; }

    public ProveedorDTO()
    {

    }
    public ProveedorDTO(string ID, string Nombre, string Social, string Compania, string RFC, string Fecha, string Actualizacion, string Estado, DateTime Date, DateTime DateUp)
    {
        this.ID = ID;
        this.Nombre = Nombre;
        this.Social = Social;
        this.Compania = Compania;
        this.RFC = RFC;
        this.Fecha = Fecha;
        this.Date = Date == null ? DateTime.MinValue : Convert.ToDateTime(Date);
        this.DateA = DateUp == null ? DateTime.MinValue : Convert.ToDateTime(DateUp);
        this.Actualizacion = Actualizacion;
        this.Estado = Estado;
    }
}

public class Proveedores
{
    public Proveedores()
    {
    }

    public static List<ProveedorDTO> ObtenerProveedores(bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            Expression<Func<Vendors, bool>> predicate;
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.CompanyID == company.CompanyID);
            else
            {
                return null;
                //List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                //predicate = (a => a.CompanyID == company.CompanyID && VendorsIds.Contains(a.VendorKey));
            }

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            sage500_appEntities db_sage = new sage500_appEntities();
            List<Vendors> list = db.Vendors.Where(predicate).ToList();
            List<ProveedorDTO> proveedores = new List<ProveedorDTO>();

            foreach (var vendor in list)
            {
                tapVendor tapvendor = db_sage.tapVendor.Where(v => v.VendKey == vendor.VendorKey).FirstOrDefault();
                string RFC = tapvendor != null ? tapvendor.VendDBA : "Información de proveedor no encontrada";
                StatusUsers status = null;
                if (vendor.Status != null)
                    status = db.StatusUsers.Where(s => s.Status == vendor.Status).FirstOrDefault();
                proveedores.Add(new ProveedorDTO(
                    vendor.VendName,
                    vendor.Users != null ? vendor.Users.UserID : "",
                    vendor.VendName,
                    "",
                    RFC,
                    vendor.CreateDate != null ? vendor.CreateDate.Value.ToString("dd/MM/yyyy") : "",
                    vendor.UpdateDate != null ? vendor.UpdateDate.Value.ToString("dd/MM/yyyy") : "",
                    status != null ? status.Descripcion : "No definido",
                    vendor.CreateDate != null ? vendor.CreateDate.Value : DateTime.MinValue,
                    vendor.UpdateDate != null ? vendor.UpdateDate.Value : DateTime.MinValue));
            }

            return proveedores;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ProveedorDTO>();
        }
    }

    public static List<ProveedorDTO> ObtenerProveedoresEmpleado(bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            List<ProveedorDTO> proveedores = new List<ProveedorDTO>();
            string SQL = string.Empty;
            string usker = HttpContext.Current.Session["UserKey"].ToString();
            SQL = "";
            SQL += "Select '' as RFC,VendName,VendorID,CreateDate ,UpdateDate ,Status from Vendors Where Superior = " + usker;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.CommandType = CommandType.Text;
                if (conn.State == ConnectionState.Open)
                { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ProveedorDTO proveedor = new ProveedorDTO();
                    proveedores.Add(new ProveedorDTO(
                    rdr["VendName"] != null ? rdr["VendName"].ToString() : "",
                    rdr["VendorID"] != null ? rdr["VendorID"].ToString() : "",
                    rdr["VendName"] != null ? rdr["VendName"].ToString() : "",
                    "",
                    rdr["RFC"] != null ? rdr["RFC"].ToString() : "",
                    rdr["CreateDate"] != null ? rdr["CreateDate"].ToString() : "",
                    rdr["UpdateDate"] != null ? rdr["UpdateDate"].ToString() : "",
                    rdr["status"] != null ? rdr["status"].ToString() : "No definido",
                    rdr["CreateDate"] != null ? Convert.ToDateTime(rdr["CreateDate"].ToString()) : DateTime.MinValue,
                    rdr["UpdateDate"] != null ? Convert.ToDateTime(rdr["UpdateDate"].ToString()) : DateTime.MinValue
                    ));
                }
                conn.Close();
            }

            return proveedores;
        }
        catch (Exception a)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(a.ToString());
            return new List<ProveedorDTO>();
        }
    }

    public static List<ProveedorDTO> ObtenerProveedores(string VendorId, string UserId, string VendorName, string RFC, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";
            List<ProveedorDTO> proveedores = ObtenerProveedores();

            if (proveedores == null)
                return null;

            if (!string.IsNullOrWhiteSpace(VendorId) && VendorId != "null")
                proveedores = proveedores.Where(p => p.ID.ToUpper().Contains(VendorId.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(UserId) && UserId != "null")
                proveedores = proveedores.Where(p => p.Nombre.ToUpper().Contains(UserId.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(VendorName) && VendorName != "null")
                proveedores = proveedores.Where(p => p.Social.ToUpper().Contains(VendorName.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(RFC) && RFC != "null")
                proveedores = proveedores.Where(p => p.RFC.ToUpper().Contains(RFC.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Estado) && Estado != "null")
            {
                Estado = Tools.GetUserStatusDescription(Estado);
                proveedores = proveedores.Where(p => p.Estado == Estado).ToList();
            }

            return proveedores;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ProveedorDTO>();
        }
    }

    public static ProveedorDTO BuscarProveedorEnSAGE(string VendorId)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(VendorId))
            {
                //using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    int cont;
                    SqlCommand cmd = new SqlCommand("spGetVendors", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@term",
                        Value = VendorId
                    });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    cont = 0;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ProveedorDTO proveedor = new ProveedorDTO()
                        {
                            Condiciones = rdr["condiciones"].ToString(),
                            Condiciones_Descripcion = rdr["condiciones_desc"].ToString(),
                            RFC = rdr["RFC"].ToString(),
                            Social = rdr["Nombre"].ToString(),
                            Correo = rdr["Email"].ToString()
                        };
                        return proveedor;
                    }

                    conn.Close();

                    if (cont == 0)
                    {
                    }
                }
            }
        }
        catch (Exception a)
        {
            string exp = a.ToString();
        }
        return null;
    }

}


