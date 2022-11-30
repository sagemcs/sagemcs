//PORTAL DE PROVEDORES T|SYS|
//12 MARZO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS

using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

/// <summary>
/// Summary description for Tools
/// </summary>
public static class Tools
{
    static public Expression<Func<InvoiceReceipt, bool>> GetPredicateForInvoiceReceipt()
    {
        Company company = Tools.EmpresaAutenticada();
        if (company == null)
            return null;
        Users user = Tools.UsuarioAutenticado();
        if (user == null)
            return null;

        Expression<Func<InvoiceReceipt, bool>> predicate;

        try
        {
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.CompanyID == company.CompanyID);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.CompanyID == company.CompanyID && VendorsIds.Contains(a.VendorKey));
            }
        }
        catch (Exception ex)
        {
            string desc = ex.ToString();
            predicate = null;
        }

        return predicate;
    }

    static public Expression<Func<CheckRequest, bool>> GetPredicateForCheckRequest()
    {
        Company company = Tools.EmpresaAutenticada();
        if (company == null)
            return null;
        Users user = Tools.UsuarioAutenticado();
        if (user == null)
            return null;

        Expression<Func<CheckRequest, bool>> predicate;

        try
        {
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.CompanyID == company.CompanyID);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.CompanyID == company.CompanyID && VendorsIds.Contains(a.VendorKey));
            }
        }
        catch (Exception ex)
        {
            string desc = ex.ToString();
            predicate = null;
        }

        return predicate;
    }
    public static bool EsTokenValido(string supuesto_token)
    {
        try
        {
            var payload = new Dictionary<string, object>{
                    { "iss", ConfigurationManager.AppSettings["as:Issuer"]},
                    { "aud", ConfigurationManager.AppSettings["as:AudienceId"]}
                };
            //var secret = ConfigurationManager.AppSettings["as:AudienceSecret"];
            var secret = HttpContext.Current.Session["JWTKey"] != null ? HttpContext.Current.Session["JWTKey"].ToString() : "";
            IJwtEncoder encoder = new JwtEncoder(new HMACSHA256Algorithm(), new JsonNetSerializer(), new JwtBase64UrlEncoder());

            string real_token = "Bearer " + encoder.Encode(payload, secret);

            if (supuesto_token == real_token)
                return true;
        }
        catch { }
        return false;
    }
    public static string GetDocumentStatusDescription(int id)
    {
        try
        {
            PortalProveedoresEntities db = new PortalProveedoresEntities();
            StatusDocs status = db.StatusDocs.Where(s => s.Status == id).FirstOrDefault();
            return status != null ? status.Descripcion : "No definido";
        }
        catch
        {
            return "Dato no válido";
        }
    }

    public static string GetDocumentStatusDescription(string id)
    {
        try
        {
            return GetDocumentStatusDescription(Convert.ToInt32(id));
        }
        catch
        {
            return "Dato no válido";
        }
    }

    public static string GetUserStatusDescription(int id)
    {
        try
        {
            PortalProveedoresEntities db = new PortalProveedoresEntities();
            StatusUsers status = db.StatusUsers.Where(s => s.Status == id).FirstOrDefault();
            return status != null ? status.Descripcion : "No definido";
        }
        catch
        {
            return "Dato no válido";
        }
    }

    public static string GetUserStatusDescription(string id)
    {
        try
        {
            return GetUserStatusDescription(Convert.ToInt32(id));
        }
        catch
        {
            return "Dato no válido";
        }
    }

    public static Company EmpresaAutenticada()
    {
        try
        {
            PortalProveedoresEntities db = new PortalProveedoresEntities();
            if (HttpContext.Current.Session["IDCompany"] == null)
                return null;
            string company_id = HttpContext.Current.Session["IDCompany"].ToString();
            return db.Company.Where(c => c.CompanyID == company_id).FirstOrDefault();
        }
        catch (Exception ex) { }
        return null;
    }

    public static Users UsuarioAutenticado()
    {
        try
        {
            PortalProveedoresEntities db = new PortalProveedoresEntities();
            if (HttpContext.Current.Session["UserKey"] == null)
                return null;
            int user_key = Convert.ToInt32(HttpContext.Current.Session["UserKey"]);
            return db.Users.Where(c => c.UserKey == user_key).FirstOrDefault();
        }
        catch { }
        return null;
    }

    public static string GetMasterPage()
    {
        try
        {
            if (UsuarioAutenticado() != null)
                if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                {
                    return "~/Logged/Proveedores/MenuPreP.Master";
                }
                else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
                {
                    return "~/Site.Master";
                }
                else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
                {
                    return "~/Logged/Administradores/SiteVal.master";
                }
                else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
                {
                    return "~/Logged/Administradores/MasterPageContb.master";
                }
                else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Finanzas")
                {
                    return "~/Logged/Administradores/SiteVal.master";
                }
                else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Tesoreria")
                {
                    return "~/Logged/Administradores/SiteVal.master";
                }
                else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Empleado")
                {
                    return "~/Logged/Administradores/SiteEmpleado.master";
                }
                else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Gerencia de Capital Humano")
                {
                    return "~/Logged/Administradores/SiteVal.master";
                }
                else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Gerente")
                {
                    return "~/Logged/Administradores/SiteVal.master";
                }
        }
        catch { }
        return "~/Logged/Proveedores/MenuPreP.Master";
    }

    public static bool EsProveedor()
    {
        try
        {
            if ((UsuarioAutenticado() != null) && ((HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin") || (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador") || (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Empleado")))
                return true;
            return false;
        }
        catch
        { }
        return true;
    }

    public static List<string> Mensaje_Estado_de_Facturas()
    {
        List<string> mensajes = new List<string>();
        PortalProveedoresEntities db = new PortalProveedoresEntities();

        if (HttpContext.Current.Session["VendKey"] != null && HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
            try
            {
                int vendor_key = Convert.ToInt32(HttpContext.Current.Session["VendKey"]);
                Vendors vendor = db.Vendors.Where(v => v.VendorKey == vendor_key).FirstOrDefault();
                if (vendor != null)
                {
                    if (vendor.Invoice != null && vendor.Invoice.Count > 0)
                    {
                        int facturas_pendientes = vendor.Invoice.Where(f => f.Status == 1).AsEnumerable().Count();
                        if (facturas_pendientes > 0)
                            mensajes.Add("Tiene " + (facturas_pendientes == 1 ? " una factura pendiente" : facturas_pendientes.ToString() + " facturas pendientes"));
                        int facturas_canceladas = vendor.Invoice.Where(f => f.Status == 3).AsEnumerable().Count();
                        if (facturas_canceladas > 0)
                            mensajes.Add("Tiene " + (facturas_canceladas == 1 ? " una factura cancelada" : facturas_canceladas.ToString() + " facturas canceladas"));
                        int facturas_eliminadas = vendor.Invoice.Where(f => f.Status == 4).AsEnumerable().Count();
                        if (facturas_eliminadas > 0)
                            mensajes.Add("Tiene " + (facturas_eliminadas == 1 ? " una factura eliminada" : facturas_eliminadas.ToString() + " facturas eliminadas"));
                        int facturas_complemento_pago_pendiente = vendor.Invoice.Where(f => f.Status == 7).AsEnumerable().Count();
                        if (facturas_complemento_pago_pendiente > 0)
                            //mensajes.Add("Tiene " + (facturas_complemento_pago_pendiente == 1 ? " una factura eliminada" : facturas_complemento_pago_pendiente.ToString() + " facturas eliminadas"));
                            mensajes.Add("Tiene complementos de pago pendientes por subir");
                    }

                }
            }
            catch (Exception e)
            {
                string ee = e.ToString();
            }
        else if ((HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin") || (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador"))
            try
            {
                int facturas_pendientes = db.Invoice.Where(f => f.Status == 1).AsEnumerable().Count();
                if (facturas_pendientes > 0)
                    mensajes.Add("Existe" + (facturas_pendientes == 1 ? " una factura pendiente" : "n " + facturas_pendientes.ToString() + " facturas pendientes"));
            }
            catch
            { }
        return mensajes;
    }

    public static List<string> Mensaje_Estado_de_Pagos()
    {
        List<string> mensajes = new List<string>();
        PortalProveedoresEntities db = new PortalProveedoresEntities();

        if (HttpContext.Current.Session["VendKey"] != null && HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
            try
            {
                int vendor_key = Convert.ToInt32(HttpContext.Current.Session["VendKey"]);
                Vendors vendor = db.Vendors.Where(v => v.VendorKey == vendor_key).FirstOrDefault();
                if (vendor != null)
                {
                    if (vendor.Payment != null && vendor.Payment.Count > 0)
                    {
                        int pagos_pendientes = vendor.Payment.Where(f => f.Status == 1).AsEnumerable().Count();
                        if (pagos_pendientes > 0)
                            mensajes.Add("Tiene " + (pagos_pendientes == 1 ? " un pago pendiente" : pagos_pendientes.ToString() + " pagos pendientes"));
                        int pagos_cancelados = vendor.Payment.Where(f => f.Status == 3).AsEnumerable().Count();
                        if (pagos_cancelados > 0)
                            mensajes.Add("Tiene " + (pagos_cancelados == 1 ? " un pago cancelado" : pagos_cancelados.ToString() + " pagos cancelados"));
                        int pagos_eliminados = vendor.Payment.Where(f => f.Status == 4).AsEnumerable().Count();
                        if (pagos_eliminados > 0)
                            mensajes.Add("Tiene " + (pagos_eliminados == 1 ? " un pago eliminado" : pagos_eliminados.ToString() + " pagos eliminados"));

                    }

                }
            }
            catch
            {
            }
        else if ((HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin") || (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador"))
            try
            {
                int pagos_pendientes = db.Payment.Where(f => f.Status == 1).AsEnumerable().Count();
                if (pagos_pendientes > 0)
                    mensajes.Add("Existe" + (pagos_pendientes == 1 ? " un pago pendiente" : "n " + pagos_pendientes.ToString() + " pagos pendientes"));
            }
            catch
            { }

        return mensajes;
    }

    public static List<string> Mensaje_Estado_de_Documentos_de_Proveedor()
    {
        List<string> mensajes = new List<string>();
        PortalProveedoresEntities db = new PortalProveedoresEntities();

        if (HttpContext.Current.Session["VendKey"] != null && HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
            try
            {
                int vendor_key = Convert.ToInt32(HttpContext.Current.Session["VendKey"]);
                Vendors vendor = db.Vendors.Where(v => v.VendorKey == vendor_key).FirstOrDefault();
                if (vendor != null)
                {

                    if (vendor.Documents != null && vendor.Documents.Count > 0)
                    {
                        int documentos_pendientes = vendor.Documents.Where(f => f.Status == 1).AsEnumerable().Count();
                        if (documentos_pendientes > 0)
                            mensajes.Add("Tiene " + (documentos_pendientes == 1 ? " un documento pendiente" : documentos_pendientes.ToString() + " documentos pendientes"));
                        int documentos_cancelados = vendor.Documents.Where(f => f.Status == 3).AsEnumerable().Count();
                        if (documentos_cancelados > 0)
                            mensajes.Add("Tiene " + (documentos_cancelados == 1 ? " un documento cancelado" : documentos_cancelados.ToString() + " documentos eliminados"));
                        int documentos_eliminados = vendor.Documents.Where(f => f.Status == 4).AsEnumerable().Count();
                        if (documentos_eliminados > 0)
                            mensajes.Add("Tiene " + (documentos_eliminados == 1 ? " un documento rechazado" : documentos_eliminados.ToString() + " documentos rechazados"));
                    }
                }
            }
            catch
            {
            }
        else if ((HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin") || (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador"))
            try
            {
                int documentos_pendientes = db.Documents.Where(f => f.Status == 1).AsEnumerable().Count();
                if (documentos_pendientes > 0)
                    mensajes.Add("Existe" + (documentos_pendientes == 1 ? " un documento de proveedor pendiente" : "n " + documentos_pendientes.ToString() + " documentos de proveedor pendientes"));
            }
            catch
            { }

        return mensajes;
    }

    public static string ObtenerFechaEnFormato(string fecha)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(fecha))
            {
                string[] camps = fecha.Split('/');
                DateTime date = new DateTime(Convert.ToInt32(camps[2]), Convert.ToInt32(camps[1]), Convert.ToInt32(camps[0]));
                fecha = date.ToShortDateString();
            }
        }
        catch
        { }
        return fecha;
    }

    public static string ObtenerFechaEnFormatoNew(string Fecha)
    {
        try
        {
            if (Fecha == null) { Fecha = ""; }
            else
            {
                if (Fecha != "")
                {
                    DateTime Fec = Convert.ToDateTime(Fecha);
                    string Dia = Fec.Day.ToString(); if (Dia.Length == 1) { Dia = "0" + Dia; }
                    string Mes = Fec.Month.ToString(); if (Mes.Length == 1) { Mes = "0" + Mes; }
                    string Anio = Fec.Year.ToString();
                    Fecha = Dia + "/" + Mes + "/" + Anio;
                }
            }
        }
        catch
        { }
        return Fecha;
    }

    public static string ObtenerFechaEnFormatoNewPago(string Fecha)
    {
        try
        {
            if (Fecha == null) { Fecha = ""; }
            else
            {
                if (Fecha != "")
                {
                    DateTime Fec = Convert.ToDateTime(Fecha);
                    string Dia = Fec.Day.ToString(); if (Dia.Length == 1) { Dia = "0" + Dia; }
                    string Mes = Fec.Month.ToString(); if (Mes.Length == 1) { Mes = "0" + Mes; }
                    string Anio = Fec.Year.ToString();
                    Fecha = Anio + "/" + Mes + "/" + Dia;
                }
            }
        }
        catch
        { }
        return Fecha;
    }

    public static string RellenoConCeros(int número, int dígitos = 5)
    {
        string rellenado = número.ToString();
        while (rellenado.Length < dígitos)
            rellenado = "0" + rellenado;
        return rellenado;
    }

    public static string FechaEnEspañol(DateTime date)
    {
        string fecha = string.Empty;

        switch (date.DayOfWeek)
        {
            case DayOfWeek.Monday:
                fecha = "Lunes";
                break;
            case DayOfWeek.Tuesday:
                fecha = "Martes";
                break;
            case DayOfWeek.Wednesday:
                fecha = "Miércoles";
                break;
            case DayOfWeek.Thursday:
                fecha = "Jueves";
                break;
            case DayOfWeek.Friday:
                fecha = "Viernes";
                break;
            case DayOfWeek.Saturday:
                fecha = "Sábado";
                break;
            case DayOfWeek.Sunday:
                fecha = "Domingo";
                break;
        }

        fecha += ", " + (date.Day == 1 ? "1ro" : date.Day.ToString()) + " de ";

        switch (date.Month)
        {
            case 1:
                fecha += "Enero";
                break;
            case 2:
                fecha += "Febrero";
                break;
            case 3:
                fecha += "Marzo";
                break;
            case 4:
                fecha += "Abril";
                break;
            case 5:
                fecha += "Mayo";
                break;
            case 6:
                fecha += "Junio";
                break;
            case 7:
                fecha += "Julio";
                break;
            case 8:
                fecha += "Agosto";
                break;
            case 9:
                fecha += "Septiembre";
                break;
            case 10:
                fecha += "Octubre";
                break;
            case 11:
                fecha += "Noviembre";
                break;
            case 12:
                fecha += "Diciembre";
                break;
        }

        fecha += " del " + date.Year;

        return fecha;
    }

    public static DateTime ObtenerFechaProgramadaPago(DateTime FechaRecepcion, int CondicionesPago)
    {
        //FechaRecepcion = FechaRecepcion.AddDays(CondicionesPago);
        DateTime fecha = FechaRecepcion;
        CondicionesPago = 60;
        switch (CondicionesPago)
        {
            case 0:
            case 7:
                fecha = ObtenerLunes(FechaRecepcion, 1);
                break;
            case 15:
                fecha = ObtenerLunes(FechaRecepcion, 2);
                break;
            case 30:
                fecha = ObtenerLunes(FechaRecepcion, 4);
                break;
            case 45:
                fecha = ObtenerLunes(FechaRecepcion, 6);
                break;
            case 60:
                fecha = ObtenerLunes(FechaRecepcion, 9);
                break;
            default:
                FechaRecepcion = FechaRecepcion.AddDays(CondicionesPago);
                fecha = ObtenerLunes(FechaRecepcion, 1);
                break;
        }

        //while (FechaRecepcion.DayOfWeek != DayOfWeek.Monday)
        //    FechaRecepcion = FechaRecepcion.AddDays(1);
        //return FechaRecepcion; // Ya en este momento es la fecha Programada de Pago

        return fecha;
    }

    private static DateTime ObtenerLunes(DateTime fecha, int num)
    {
        while (fecha.DayOfWeek != DayOfWeek.Monday)
        {
            fecha = fecha.AddDays(1);
        }
        //if (num == 1)
        //{
        //    return fecha;
        //}
        //else
        //{
        for (int i = 0; i < num; i++)
        {
            fecha = fecha.AddDays(7);
        }
        //}
        return fecha;
    }

    public static DateTime ObtenerFechaRecepcion(DateTime FechaTransacción)
    {
        while (FechaTransacción.DayOfWeek != DayOfWeek.Monday)
            FechaTransacción = FechaTransacción.AddDays(1);
        return FechaTransacción;
    }

    private static SqlConnection SqlConnectionDB(string cnx)
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
    public static void LogError(String proceso, String mensaje)
    {
        try
        {

            int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"]);
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"]);
            string pCompanyID = HttpContext.Current.Session["IDCompany"].ToString();

            int vkey, val1;
            vkey = 0;
            val1 = 0;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            string sSQL;

            sSQL = "spapErrorLog";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@LogKey", pLogKey));
            parsT.Add(new SqlParameter("@UpdateUserKey", pUserKey));
            parsT.Add(new SqlParameter("@proceso", proceso));
            parsT.Add(new SqlParameter("@mensaje", mensaje));
            parsT.Add(new SqlParameter("@CompanyID", pCompanyID));

            using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
            {

                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = sSQL;

                foreach (System.Data.SqlClient.SqlParameter par in parsT)
                {
                    Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);
                }

                System.Data.SqlClient.SqlDataReader rdr = null;

                rdr = Cmd.ExecuteReader();

                while (rdr.Read())
                {
                    val1 = rdr.GetInt32(0); // 0 ok
                }

                sqlConnection1.Close();

            }

        }
        catch (Exception ex)
        {
            string err;
            err = ex.Message;
        }

    }
    public static string FechaCortaEsp(DateTime date)

    {
        string fecha = string.Empty;
        fecha = date.Day.ToString() + "/";
        switch (date.Month)
        {
            case 1:
                fecha += "Enero";
                break;
            case 2:
                fecha += "Febrero";
                break;
            case 3:
                fecha += "Marzo";

                break;
            case 4:
                fecha += "Abril";
                break;
            case 5:
                fecha += "Mayo";
                break;
            case 6:
                fecha += "Junio";
                break;
            case 7:
                fecha += "Julio";
                break;
            case 8:
                fecha += "Agosto";
                break;
            case 9:
                fecha += "Septiembre";
                break;
            case 10:
                fecha += "Octubre";
                break;
            case 11:
                fecha += "Noviembre";
                break;
            case 12:
                fecha += "Diciembre";
                break;
        }

        fecha += "/" + date.Year;
        return fecha;

    }

    public static string getUserEmail(int userkey)
    {
        string email = string.Empty;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT UserName FROM AspNetUsers where UserKey = @UserKey";
            cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = userkey;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                email = dataReader.GetString(0);
            }
        }
        return email;
    }
    public static ValidatingUserDTO get_MatrizValidadoresAnticipos(int pUserKey, int level)
    {

        ValidatingUserDTO matriz = new ValidatingUserDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            switch (level)
            {
                case 1:
                    cmd.CommandText = "SELECT UserKey, Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserKey = @UserKey";
                    break;
                case 2:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserValidadorCX = @UserKey";
                    break;
                case 3:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserGerente = @UserKey";
                    break;
                case 4:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserRH = @UserKey";
                    break;
                case 5:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserTesoreria = @UserKey";
                    break;
                case 6:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserFinanzas = @UserKey";
                    break;
                default:
                    break;
            }

            cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = pUserKey;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                matriz.UserKey = dataReader.GetInt32(0);
                matriz.ValidadorCx = dataReader.GetInt32(1);
                matriz.Gerente = dataReader.GetInt32(2);
                matriz.Rh = dataReader.GetInt32(3);
                matriz.Tesoreria = dataReader.GetInt32(4);
                matriz.Finanzas = dataReader.GetInt32(5);
            }
        }
        return matriz;
    }
    public static ValidatingUserDTO get_MatrizValidadores(int pUserKey, int level)
    {
        ValidatingUserDTO matriz = new ValidatingUserDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            switch (level)
            {
                case 1:
                    cmd.CommandText = "SELECT UserKey, Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserKey = @UserKey";
                    break;
                case 2:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserValidadorCX = @UserKey";
                    break;
                case 3:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserGerente = @UserKey";
                    break;
                case 4:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserRH = @UserKey";
                    break;
                case 5:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserTesoreria = @UserKey";
                    break;
                case 6:
                    cmd.CommandText = "SELECT UserKey,Isnull(UserValidadorCX,0), Isnull(UserGerente,0), Isnull(UserRH,0), Isnull(UserTesoreria,0), Isnull(UserFinanzas,0) FROM validatingUser where UserFinanzas = @UserKey";
                    break;
                default:
                    break;
            }

            cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = pUserKey;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                matriz.UserKey = dataReader.GetInt32(0);
                matriz.ValidadorCx = dataReader.GetInt32(1);
                matriz.Gerente = dataReader.GetInt32(2);
                matriz.Rh = dataReader.GetInt32(3);
                matriz.Tesoreria = dataReader.GetInt32(4);
                matriz.Finanzas = dataReader.GetInt32(5);
            }
        }
        return matriz;
    }

    public static ValidatingTree get_JerarquiaValidadores(int document_type)
    {
        ValidatingTree jerarquia = new ValidatingTree();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT RecursosHumanos,GerenciaArea,CuentasxPagar,Tesoreria,Finanzas FROM ValidatingTree where TypeKey = @TypeKey";
            cmd.Parameters.Add("@TypeKey", SqlDbType.Int).Value = document_type;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                jerarquia.RecursosHumanos = dataReader.GetBoolean(0);
                jerarquia.GerenciaArea = dataReader.GetBoolean(1);
                jerarquia.CuentasxPagar = dataReader.GetBoolean(2);
                jerarquia.Tesoreria = dataReader.GetBoolean(3);
                jerarquia.Finanzas = dataReader.GetBoolean(4);
            }
        }
        return jerarquia;
    }

    public enum DocumentType
    {
        Advance = 1, Expense = 2, CorporateCard = 3, MinorMedicalExpense = 4
    }

    public static List<RolDTO> get_Roles()
    {
        List<RolDTO> roles = new List<RolDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT RoleKey, RoleID FROM Roles where RoleKey in (7,8,9,10,11,12,13,14,15)";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var rol = new RolDTO();
                rol.Key = dataReader.GetInt32(0);
                rol.ID = dataReader.GetString(1);
                roles.Add(rol);
            }
        }
        return roles;

    }

    public static List<RolDTO> get_RolesValidadores()
    {
        List<RolDTO> roles = new List<RolDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Level, RoleID FROM ValidatingRoles";
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var rol = new RolDTO();
                rol.Key = dataReader.GetInt32(0);
                rol.ID = dataReader.GetString(1);
                roles.Add(rol);
            }
        }
        return roles;

    }

    //Api Sage-Portal
    
    public static void Insert_tapApiLogErrorCgGst(int Batchkey, int vKey, int i_dtlkey, string errores)
    {        
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "insert into tapApiLogErrorCgGst values (@Batchkey,@vKey,@i_dtlkey,@errores)";
            cmd.Parameters.Add("@Batchkey", SqlDbType.Int).Value = Batchkey;
            cmd.Parameters.Add("@vKey", SqlDbType.Int).Value = vKey;
            cmd.Parameters.Add("@i_dtlkey", SqlDbType.Int).Value = i_dtlkey;
            cmd.Parameters.Add("@errores", SqlDbType.VarChar).Value = errores;
            
            cmd.Connection.Open();
            try
            {
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Connection.Close();
                throw new Exception(ex.Message);
            }           
        }
    }

    public static int Insert_tapBatchCExtGst(tapBatchCExtGst tap)
    {  
        int iLote = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO dbo.tapBatchCExtGst (iBatchNo, iBatchKey, iBatchCmnt, iBatchOvrdSegVal, iBatchType, iInterCompany, iInterCoBatchKey, iOrigUserID, iPostDate, iSourceCompanyID, iTranCtrl, iHold, iHoldReason, iPrivate, iCashAcctKey, iCashAcctID, iImportLogKey, iLogSuccessful, oRetVal, oBatchKey, eliminado, status, FechaCreacion, FechaModificado, CompanyID, UserID, oSpid) VALUES" +
                                                             " (@iBatchNo, @iBatchKey, @iBatchCmnt, @iBatchOvrdSegVal, @iBatchType, @iInterCompany, @iInterCoBatchKey, @iOrigUserID, @iPostDate, @iSourceCompanyID, @iTranCtrl, @iHold, @iHoldReason, @iPrivate, @iCashAcctKey, @iCashAcctID, @iImportLogKey, @iLogSuccessful, @oRetVal, @oBatchKey, @eliminado, @status, @FechaCreacion, @FechaModificado, @CompanyID, @UserID, @oSpid); SELECT SCOPE_IDENTITY();";
            
            cmd.Parameters.Add("@iBatchNo", SqlDbType.Int).Value = tap.iBatchNo != null ? tap.iBatchNo : (object)DBNull.Value;
            cmd.Parameters.Add("@iBatchKey", SqlDbType.Int).Value = tap.iBatchKey != null ? tap.iBatchKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iBatchCmnt", SqlDbType.VarChar).Value = tap.iBatchCmnt != null ? tap.iBatchCmnt : (object)DBNull.Value;
            cmd.Parameters.Add("@iBatchOvrdSegVal", SqlDbType.VarChar).Value = tap.iBatchOvrdSegVal != null ? tap.iBatchOvrdSegVal : (object)DBNull.Value;
            cmd.Parameters.Add("@iBatchType", SqlDbType.Int).Value = tap.iBatchType != null ? tap.iBatchType : (object)DBNull.Value;
            cmd.Parameters.Add("@iInterCompany", SqlDbType.Int).Value = tap.iInterCompany != null ? tap.iInterCompany : (object)DBNull.Value;
            cmd.Parameters.Add("@iInterCoBatchKey", SqlDbType.Int).Value = tap.iInterCoBatchKey != null ? tap.iInterCoBatchKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iOrigUserID", SqlDbType.VarChar).Value = tap.iOrigUserID != null ? tap.iOrigUserID : (object)DBNull.Value;
            cmd.Parameters.Add("@iPostDate", SqlDbType.DateTime).Value = tap.iPostDate != null ? tap.iPostDate : (object)DBNull.Value;
            cmd.Parameters.Add("@iSourceCompanyID", SqlDbType.VarChar).Value = tap.iSourceCompanyID != null ? tap.iSourceCompanyID : (object)DBNull.Value;
            cmd.Parameters.Add("@iTranCtrl", SqlDbType.Decimal).Value = tap.iTranCtrl != null ? tap.iTranCtrl : (object)DBNull.Value;
            cmd.Parameters.Add("@iHold", SqlDbType.Int).Value = tap.iHold != null ? tap.iHold : (object)DBNull.Value;
            cmd.Parameters.Add("@iHoldReason", SqlDbType.VarChar).Value = tap.iHoldReason != null ? tap.iHoldReason : (object)DBNull.Value;
            cmd.Parameters.Add("@iPrivate", SqlDbType.Int).Value = tap.iPrivate != null ? tap.iPrivate : (object)DBNull.Value;
            cmd.Parameters.Add("@iCashAcctKey", SqlDbType.Int).Value = tap.iCashAcctKey != null ? tap.iCashAcctKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iCashAcctID", SqlDbType.VarChar).Value = tap.iCashAcctID != null ? tap.iCashAcctID : (object)DBNull.Value;
            cmd.Parameters.Add("@iImportLogKey", SqlDbType.Int).Value = tap.iImportLogKey != null ? tap.iImportLogKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iLogSuccessful", SqlDbType.Int).Value = tap.iLogSuccessful != null ? tap.iLogSuccessful : (object)DBNull.Value;
            cmd.Parameters.Add("@oRetVal", SqlDbType.Int).Value = tap.oRetVal != null ? tap.oRetVal : (object)DBNull.Value;
            cmd.Parameters.Add("@oBatchKey", SqlDbType.Int).Value = tap.oBatchKey != null ? tap.oBatchKey : (object)DBNull.Value;
            cmd.Parameters.Add("@eliminado", SqlDbType.Bit).Value = tap.eliminado != null ? tap.eliminado : (object)DBNull.Value;
            cmd.Parameters.Add("@status", SqlDbType.Int).Value = tap.status != null ? tap.status : (object)DBNull.Value;
            cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime).Value = tap.FechaCreacion != null ? tap.FechaCreacion : (object)DBNull.Value;
            cmd.Parameters.Add("@FechaModificado", SqlDbType.DateTime).Value = tap.FechaModificado != null ? tap.FechaModificado : (object)DBNull.Value;
            cmd.Parameters.Add("@CompanyID", SqlDbType.VarChar).Value = tap.CompanyID != null ? tap.CompanyID : (object)DBNull.Value;
            cmd.Parameters.Add("@UserID", SqlDbType.VarChar).Value = tap.UserID != null ? tap.UserID : (object)DBNull.Value;
            cmd.Parameters.Add("@oSpid", SqlDbType.Int).Value = tap.oSpid != null ? tap.oSpid : (object)DBNull.Value;
            cmd.Connection.Open();
            try
            {
                var scalar = cmd.ExecuteScalar();
                iLote = int.Parse(scalar.ToString());
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Connection.Close();
                throw new Exception(ex.Message);
            }                 
            
        }
        return iLote;
    }
    public static int Insert_tapVoucherCExGst(tapVoucherCExGst tap)
    {  
        int vkey = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO dbo.tapVoucherCExGst (iLote, iApplySeqNo, iBatchKey, iRemitToVendAddrKey, iRemitToVendAddrID, iRemitToCopyKey, iRemitToCopyID, iRemitToAddrName, iRTAddrLine1, iRTAddrLine2, iRTAddrLine3, iRTAddrLine4, iRTAddrLine5,  iRTCity, iRTState, iRTCountryID, iRTPostalCode, iCashAcctKey, iCashAcctID, iCntcKey, iCntcName, iCurrExchRate, iCurrExchSchdKey, iCurrExchSchdID, iCurrID, iHoldPmt, iVendClassKey, iVendClassID, iVendKey, iVendID, iFOBKey, iFOBID, iImportLogKey, iPmtTermsKey, iPmtTermsID, iReasonCodeKey, iReasonCodeID, iFreightAmt, iSeparateCheck, iShipMethKey, iShipMethID, iPurchFromVendAddrKey, iPurchFromVendAddrID, iPurchToCopyKey, iPurchToCopyID, iPurchFromAddrName, iPurchAddrLine1, iPurchAddrLine2, iPurchAddrLine3, iPurchAddrLine4, iPurchAddrLine5, iPurchCity, iPurchState, iPurchCountryID, iPurchPostalCode, iShipZoneKey, iShipZoneID, iTranCmnt, iTranDate, iTranNo, iTranType, iUniqueID, iDefaultIfNull, iUserID, iV1099Box, iV1099BoxText, iV1099Form, iRetntAmt, iVouchNo, iInvcRcptDate, oVouchKey, oTranID, iDueDate, iDiscAmt, iDiscDate, iExemptKey, iSTaxAmt, iPurchAmt, iTranAmt, oRetVal) VALUES " +
                "(@iLote, @iApplySeqNo, @iBatchKey, @iRemitToVendAddrKey, @iRemitToVendAddrID, @iRemitToCopyKey, @iRemitToCopyID, @iRemitToAddrName, @iRTAddrLine1, @iRTAddrLine2, @iRTAddrLine3, @iRTAddrLine4, @iRTAddrLine5, @iRTCity, @iRTState, @iRTCountryID, @iRTPostalCode, @iCashAcctKey, @iCashAcctID, @iCntcKey, @iCntcName, @iCurrExchRate, @iCurrExchSchdKey, @iCurrExchSchdID, @iCurrID, @iHoldPmt, @iVendClassKey, @iVendClassID, @iVendKey, @iVendID, @iFOBKey, @iFOBID, @iImportLogKey, @iPmtTermsKey, @iPmtTermsID, @iReasonCodeKey, @iReasonCodeID, @iFreightAmt, @iSeparateCheck, @iShipMethKey, @iShipMethID, @iPurchFromVendAddrKey, @iPurchFromVendAddrID, @iPurchToCopyKey, @iPurchToCopyID, @iPurchFromAddrName, @iPurchAddrLine1, @iPurchAddrLine2, @iPurchAddrLine3, @iPurchAddrLine4, @iPurchAddrLine5, @iPurchCity, @iPurchState, @iPurchCountryID, @iPurchPostalCode, @iShipZoneKey, @iShipZoneID, @iTranCmnt, @iTranDate, @iTranNo, @iTranType, @iUniqueID, @iDefaultIfNull, @iUserID, @iV1099Box, @iV1099BoxText, @iV1099Form, @iRetntAmt, @iVouchNo, @iInvcRcptDate, @oVouchKey, @oTranID, @iDueDate, @iDiscAmt, @iDiscDate, @iExemptKey, @iSTaxAmt, @iPurchAmt, @iTranAmt, @oRetVal); SELECT SCOPE_IDENTITY();";
            
            cmd.Parameters.Add("@iLote", SqlDbType.Int).Value = tap.iLote != null ? tap.iLote : (object)DBNull.Value;
            cmd.Parameters.Add("@iApplySeqNo", SqlDbType.Int).Value = tap.iApplySeqNo != null ? tap.iApplySeqNo : (object)DBNull.Value;
            cmd.Parameters.Add("@iBatchKey", SqlDbType.Int).Value = tap.iBatchKey != null ? tap.iBatchKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iRemitToVendAddrKey", SqlDbType.Int).Value = tap.iRemitToVendAddrKey != null ? tap.iRemitToVendAddrKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iRemitToVendAddrID", SqlDbType.Int).Value = tap.iRemitToVendAddrID != null ? tap.iRemitToVendAddrID : (object)DBNull.Value;
            cmd.Parameters.Add("@iRemitToCopyKey", SqlDbType.Int).Value = tap.iRemitToCopyKey != null ? tap.iRemitToCopyKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iRemitToCopyID", SqlDbType.VarChar).Value = tap.iRemitToCopyID != null ? tap.iRemitToCopyID : (object)DBNull.Value;
            cmd.Parameters.Add("@iRemitToAddrName", SqlDbType.VarChar).Value = tap.iRemitToAddrName != null ? tap.iRemitToAddrName : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTAddrLine1", SqlDbType.VarChar).Value = tap.iRTAddrLine1 != null ? tap.iRTAddrLine1 : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTAddrLine2", SqlDbType.VarChar).Value = tap.iRTAddrLine2 != null ? tap.iRTAddrLine2 : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTAddrLine3", SqlDbType.VarChar).Value = tap.iRTAddrLine3 != null ? tap.iRTAddrLine3 : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTAddrLine4", SqlDbType.VarChar).Value = tap.iRTAddrLine4 != null ? tap.iRTAddrLine4 : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTAddrLine5", SqlDbType.VarChar).Value = tap.iRTAddrLine5 != null ? tap.iRTAddrLine5 : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTCity", SqlDbType.VarChar).Value = tap.iRTCity != null ? tap.iRTCity : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTState", SqlDbType.VarChar).Value = tap.iRTState != null ? tap.iRTState : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTCountryID", SqlDbType.VarChar).Value = tap.iRTCountryID != null ? tap.iRTCountryID : (object)DBNull.Value;
            cmd.Parameters.Add("@iRTPostalCode", SqlDbType.VarChar).Value = tap.iRTPostalCode != null ? tap.iRTPostalCode : (object)DBNull.Value;
            cmd.Parameters.Add("@iCashAcctKey", SqlDbType.Int).Value = tap.iCashAcctKey != null ? tap.iCashAcctKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iCashAcctID", SqlDbType.Int).Value = tap.iCashAcctKey != null ? tap.iCashAcctKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iCntcKey", SqlDbType.Int).Value = tap.iCntcKey != null ? tap.iCntcKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iCntcName", SqlDbType.VarChar).Value = tap.iCntcName != null ? tap.iCntcName : (object)DBNull.Value;
            cmd.Parameters.Add("@iCurrExchRate", SqlDbType.Decimal).Value = tap.iCurrExchRate != null ? tap.iCurrExchRate : (object)DBNull.Value;
            cmd.Parameters.Add("@iCurrExchSchdKey", SqlDbType.Int).Value = tap.iCurrExchSchdKey != null ? tap.iCurrExchSchdKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iCurrExchSchdID", SqlDbType.VarChar).Value = tap.iCurrExchSchdID != null ? tap.iCurrExchSchdID : (object)DBNull.Value;
            cmd.Parameters.Add("@iCurrID", SqlDbType.VarChar).Value = tap.iCurrID != null ? tap.iCurrID : (object)DBNull.Value;
            cmd.Parameters.Add("@iHoldPmt", SqlDbType.Int).Value = tap.iHoldPmt != null ? tap.iHoldPmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iVendClassKey", SqlDbType.Int).Value = tap.iVendClassKey != null ? tap.iVendClassKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iVendClassID", SqlDbType.VarChar).Value = tap.iVendClassID != null ? tap.iVendClassID : (object)DBNull.Value;
            cmd.Parameters.Add("@iVendKey", SqlDbType.Int).Value = tap.iVendKey != null ? tap.iVendKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iVendID", SqlDbType.VarChar).Value = tap.iVendID != null ? tap.iVendID : (object)DBNull.Value;
            //iFOBKey, iFOBID, iImportLogKey, iPmtTermsKey, iPmtTermsID, iReasonCodeKey, iReasonCodeID, iFreightAmt, iSeparateCheck, iShipMethKey, iShipMethID, iPurchFromVendAddrKey, iPurchFromVendAddrID, iPurchToCopyKey, 
            cmd.Parameters.Add("@iFOBKey", SqlDbType.Int).Value = tap.iFOBKey != null ? tap.iFOBKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iFOBID", SqlDbType.VarChar).Value = tap.iFOBID != null ? tap.iFOBID : (object)DBNull.Value;
            cmd.Parameters.Add("@iImportLogKey", SqlDbType.Int).Value = tap.iImportLogKey != null ? tap.iImportLogKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iPmtTermsKey", SqlDbType.Int).Value = tap.iPmtTermsKey != null ? tap.iPmtTermsKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iPmtTermsID", SqlDbType.VarChar).Value = tap.iPmtTermsID != null ? tap.iPmtTermsID : (object)DBNull.Value;
            cmd.Parameters.Add("@iReasonCodeKey", SqlDbType.Int).Value = tap.iReasonCodeKey != null ? tap.iReasonCodeKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iReasonCodeID", SqlDbType.VarChar).Value = tap.iReasonCodeID != null ? tap.iReasonCodeID : (object)DBNull.Value;
            cmd.Parameters.Add("@iFreightAmt", SqlDbType.Decimal).Value = tap.iFreightAmt != null ? tap.iFreightAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iSeparateCheck", SqlDbType.Int).Value = tap.iSeparateCheck != null ? tap.iSeparateCheck : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipMethKey", SqlDbType.Int).Value = tap.iShipMethKey != null ? tap.iShipMethKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipMethID", SqlDbType.VarChar).Value = tap.iShipMethID != null ? tap.iShipMethID : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchFromVendAddrKey", SqlDbType.Int).Value = tap.iPurchFromVendAddrKey != null ? tap.iPurchFromVendAddrKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchFromVendAddrID", SqlDbType.VarChar).Value = tap.iPurchFromVendAddrID != null ? tap.iPurchFromVendAddrID : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchToCopyKey", SqlDbType.Int).Value = tap.iPurchToCopyKey != null ? tap.iPurchToCopyKey : (object)DBNull.Value;
            //iPurchToCopyID, iPurchFromAddrName, iPurchAddrLine1, iPurchAddrLine2, iPurchAddrLine3, iPurchAddrLine4, iPurchAddrLine5, iPurchCity, iPurchState, iPurchCountryID, iPurchPostalCode, iShipZoneKey, iShipZoneID, 
            cmd.Parameters.Add("@iPurchToCopyID", SqlDbType.VarChar).Value = tap.iPurchToCopyID != null ? tap.iPurchToCopyID : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchFromAddrName", SqlDbType.VarChar).Value = tap.iPurchFromAddrName != null ? tap.iPurchFromAddrName : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchAddrLine1", SqlDbType.VarChar).Value = tap.iPurchAddrLine1 != null ? tap.iPurchAddrLine1 : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchAddrLine2", SqlDbType.VarChar).Value = tap.iPurchAddrLine2 != null ? tap.iPurchAddrLine2 : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchAddrLine3", SqlDbType.VarChar).Value = tap.iPurchAddrLine3 != null ? tap.iPurchAddrLine3 : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchAddrLine4", SqlDbType.VarChar).Value = tap.iPurchAddrLine4 != null ? tap.iPurchAddrLine4 : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchAddrLine5", SqlDbType.VarChar).Value = tap.iPurchAddrLine5 != null ? tap.iPurchAddrLine5 : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchCity", SqlDbType.VarChar).Value = tap.iPurchCity != null ? tap.iPurchCity : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchState", SqlDbType.VarChar).Value = tap.iPurchState != null ? tap.iPurchState : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchCountryID", SqlDbType.VarChar).Value = tap.iPurchCountryID != null ? tap.iPurchCountryID : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchPostalCode", SqlDbType.VarChar).Value = tap.iPurchPostalCode != null ? tap.iPurchPostalCode : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipZoneKey", SqlDbType.Int).Value = tap.iShipZoneKey != null ? tap.iShipZoneKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipZoneID", SqlDbType.VarChar).Value = tap.iShipZoneID != null ? tap.iShipZoneID : (object)DBNull.Value;
            //iTranCmnt, iTranDate, iTranNo, iTranType, iUniqueID, iDefaultIfNull, iUserID, iV1099Box, iV1099BoxText, iV1099Form, iRetntAmt,
            //iVouchNo, iInvcRcptDate, oVouchKey, oTranID, iDueDate, iDiscAmt, iDiscDate, iExemptKey, 
            cmd.Parameters.Add("@iTranCmnt", SqlDbType.VarChar).Value = tap.iTranCmnt != null ? tap.iTranCmnt : (object)DBNull.Value;
            cmd.Parameters.Add("@iTranDate", SqlDbType.DateTime).Value = tap.iTranDate != null ? tap.iTranDate : (object)DBNull.Value;
            cmd.Parameters.Add("@iTranNo", SqlDbType.VarChar).Value = tap.iTranNo != null ? tap.iTranNo : (object)DBNull.Value;
            cmd.Parameters.Add("@iTranType", SqlDbType.Int).Value = tap.iTranType != null ? tap.iTranType : (object)DBNull.Value;
            cmd.Parameters.Add("@iUniqueID", SqlDbType.VarChar).Value = tap.iUniqueID != null ? tap.iUniqueID : (object)DBNull.Value;
            cmd.Parameters.Add("@iDefaultIfNull", SqlDbType.Int).Value = tap.iDefaultIfNull != null ? tap.iDefaultIfNull : (object)DBNull.Value;
            cmd.Parameters.Add("@iUserID", SqlDbType.VarChar).Value = tap.iUserID != null ? tap.iUserID : (object)DBNull.Value;
            cmd.Parameters.Add("@iV1099Box", SqlDbType.VarChar).Value = tap.iV1099Box != null ? tap.iV1099Box : (object)DBNull.Value;
            cmd.Parameters.Add("@iV1099BoxText", SqlDbType.VarChar).Value = tap.iV1099BoxText != null ? tap.iV1099BoxText : (object)DBNull.Value;
            cmd.Parameters.Add("@iV1099Form", SqlDbType.Int).Value = tap.iV1099Form != null ? tap.iV1099Form : (object)DBNull.Value;
            cmd.Parameters.Add("@iRetntAmt", SqlDbType.Decimal).Value = tap.iRetntAmt != null ? tap.iRetntAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iVouchNo", SqlDbType.VarChar).Value = tap.iVouchNo != null ? tap.iVouchNo : (object)DBNull.Value;
            cmd.Parameters.Add("@iInvcRcptDate", SqlDbType.DateTime).Value = tap.iInvcRcptDate != null ? tap.iInvcRcptDate : (object)DBNull.Value;
            cmd.Parameters.Add("@oVouchKey", SqlDbType.Int).Value = tap.oVouchKey != null ? tap.oVouchKey : (object)DBNull.Value;
            cmd.Parameters.Add("@oTranID", SqlDbType.VarChar).Value = tap.oTranID != null ? tap.oTranID : (object)DBNull.Value;
            cmd.Parameters.Add("@iDueDate", SqlDbType.DateTime).Value = tap.iDueDate != null ? tap.iDueDate : (object)DBNull.Value;
            cmd.Parameters.Add("@iDiscAmt", SqlDbType.Decimal).Value = tap.iDiscAmt != null ? tap.iDiscAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iDiscDate", SqlDbType.DateTime).Value = tap.iDiscDate != null ? tap.iDiscDate : (object)DBNull.Value;
            cmd.Parameters.Add("@iExemptKey", SqlDbType.Int).Value = tap.iExemptKey != null ? tap.iExemptKey : (object)DBNull.Value;
            //iSTaxAmt, iPurchAmt, iTranAmt, oRetVal
            cmd.Parameters.Add("@iSTaxAmt", SqlDbType.Decimal).Value = tap.iSTaxAmt != null ? tap.iSTaxAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iPurchAmt", SqlDbType.Decimal).Value = tap.iPurchAmt != null ? tap.iPurchAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iTranAmt", SqlDbType.Decimal).Value = tap.iTranAmt != null ? tap.iTranAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@oRetVal", SqlDbType.Int).Value = tap.oRetVal != null ? tap.oRetVal : (object)DBNull.Value;

            cmd.Connection.Open();
            try
            {
                var scalar = cmd.ExecuteScalar();
                vkey = int.Parse(scalar.ToString());
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Connection.Close();
                throw new Exception(ex.Message);
            }
        }
        return vkey;
    }
    public static int Insert_tapVoucherDtlCExGst(tapVoucherDtlCExGst tap)
    {
       
        int dtlKey = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO dbo.tapVoucherDtlCExGst (iLote, vKey, iTranNo, iTargetCompanyID, iVouchKey, iCmntOnly, iDescription, iExtAmt, iRetntAmt, iRetntRate, iExtCmnt, iItemKey, iItemID, iQuantity, iGLAcctKey, iGLAcctNo, iSTaxClassKey, iSTaxClassID, iUnitCost, iUnitMeasKey, iUnitMeasID, iAcctRefKey, iAcctRefCode, iFOBKey, iFOBID, iFreightAmt, iShipMethKey, iShipMethID, iShipZoneKey, iShipZoneID, iSTaxSchdKey, iSTaxSchdID, iDefaultIfNull, iPerformOverride, iSpid,  iCommissionFlag, iPOKey, iPONo, iPOLineKey, iPOLineNo, iRcvrLineKey, iMatchStatus, iReturnType, oRetVal) VALUES" +
                " (@iLote, @vKey, @iTranNo, @iTargetCompanyID, @iVouchKey, @iCmntOnly, @iDescription, @iExtAmt, @iRetntAmt, @iRetntRate, @iExtCmnt, @iItemKey, @iItemID, @iQuantity, @iGLAcctKey, @iGLAcctNo, @iSTaxClassKey, @iSTaxClassID, @iUnitCost, @iUnitMeasKey, @iUnitMeasID, @iAcctRefKey, @iAcctRefCode, @iFOBKey, @iFOBID, @iFreightAmt, @iShipMethKey, @iShipMethID, @iShipZoneKey, @iShipZoneID, @iSTaxSchdKey, @iSTaxSchdID, @iDefaultIfNull, @iPerformOverride, @iSpid,  @iCommissionFlag, @iPOKey, @iPONo, @iPOLineKey, @iPOLineNo, @iRcvrLineKey, @iMatchStatus, @iReturnType, @oRetVal); SELECT SCOPE_IDENTITY();";
            cmd.Parameters.Add("@iLote", SqlDbType.Int).Value = tap.iLote != null ? tap.iLote : (object)DBNull.Value;
            cmd.Parameters.Add("@vKey", SqlDbType.Int).Value = tap.vKey != null ? tap.vKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iTranNo", SqlDbType.VarChar).Value = tap.iTranNo != null ? tap.iTranNo : (object)DBNull.Value;
            cmd.Parameters.Add("@iTargetCompanyID", SqlDbType.VarChar).Value = tap.iTargetCompanyID != null ? tap.iTargetCompanyID : (object)DBNull.Value;
            cmd.Parameters.Add("@iVouchKey", SqlDbType.Int).Value = tap.iVouchKey != null ? tap.iVouchKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iCmntOnly", SqlDbType.Int).Value = tap.iCmntOnly != null ? tap.iCmntOnly : (object)DBNull.Value;
            cmd.Parameters.Add("@iDescription", SqlDbType.VarChar).Value = tap.iDescription != null ? tap.iDescription : (object)DBNull.Value;
            cmd.Parameters.Add("@iExtAmt", SqlDbType.Decimal).Value = tap.iExtAmt != null ? tap.iExtAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iRetntAmt", SqlDbType.Decimal).Value = tap.iRetntAmt != null ? tap.iRetntAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iRetntRate", SqlDbType.Decimal).Value = tap.iRetntRate != null ? tap.iRetntRate : (object)DBNull.Value;

            cmd.Parameters.Add("@iExtCmnt", SqlDbType.VarChar).Value = tap.iExtCmnt != null ? tap.iExtCmnt : (object)DBNull.Value;
            cmd.Parameters.Add("@iItemKey", SqlDbType.Int).Value = tap.iItemKey != null ? tap.iItemKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iItemID", SqlDbType.VarChar).Value = tap.iItemID != null ? tap.iItemID : (object)DBNull.Value;
            cmd.Parameters.Add("@iQuantity", SqlDbType.Decimal).Value = tap.iQuantity != null ? tap.iQuantity : (object)DBNull.Value;
            cmd.Parameters.Add("@iGLAcctKey", SqlDbType.Int).Value = tap.iGLAcctKey != null ? tap.iGLAcctKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iGLAcctNo", SqlDbType.VarChar).Value = tap.iGLAcctNo != null ? tap.iGLAcctNo : (object)DBNull.Value;
            cmd.Parameters.Add("@iSTaxClassKey", SqlDbType.Int).Value = tap.iSTaxClassKey != null ? tap.iSTaxClassKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iSTaxClassID", SqlDbType.VarChar).Value = tap.iSTaxClassID != null ? tap.iSTaxClassID : (object)DBNull.Value;
            cmd.Parameters.Add("@iUnitCost", SqlDbType.Decimal).Value = tap.iUnitCost != null ? tap.iUnitCost : (object)DBNull.Value;
            cmd.Parameters.Add("@iUnitMeasKey", SqlDbType.Int).Value = tap.iUnitMeasKey != null ? tap.iUnitMeasKey : (object)DBNull.Value;

            cmd.Parameters.Add("@iUnitMeasID", SqlDbType.VarChar).Value = tap.iUnitMeasID != null ? tap.iUnitMeasID : (object)DBNull.Value;
            cmd.Parameters.Add("@iAcctRefKey", SqlDbType.Int).Value = tap.iAcctRefKey != null ? tap.iAcctRefKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iAcctRefCode", SqlDbType.VarChar).Value = tap.iAcctRefCode != null ? tap.iAcctRefCode : (object)DBNull.Value;
            cmd.Parameters.Add("@iFOBKey", SqlDbType.Int).Value = tap.iFOBKey != null ? tap.iFOBKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iFOBID", SqlDbType.VarChar).Value = tap.iFOBID != null ? tap.iFOBID : (object)DBNull.Value;
            cmd.Parameters.Add("@iFreightAmt", SqlDbType.Decimal).Value = tap.iFreightAmt != null ? tap.iFreightAmt : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipMethKey", SqlDbType.Int).Value = tap.iShipMethKey != null ? tap.iShipMethKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipMethID", SqlDbType.VarChar).Value = tap.iShipMethID != null ? tap.iShipMethID : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipZoneKey", SqlDbType.Int).Value = tap.iShipZoneKey != null ? tap.iShipZoneKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iShipZoneID", SqlDbType.VarChar).Value = tap.iShipZoneID != null ? tap.iShipZoneID : (object)DBNull.Value;
           
            cmd.Parameters.Add("@iSTaxSchdKey", SqlDbType.Int).Value = tap.iSTaxSchdKey != null ? tap.iSTaxSchdKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iSTaxSchdID", SqlDbType.VarChar).Value = tap.iSTaxSchdID != null ? tap.iSTaxSchdID : (object)DBNull.Value;
            cmd.Parameters.Add("@iDefaultIfNull", SqlDbType.Int).Value = tap.iDefaultIfNull != null ? tap.iDefaultIfNull : (object)DBNull.Value;
            cmd.Parameters.Add("@iPerformOverride", SqlDbType.Int).Value = tap.iPerformOverride != null ? tap.iPerformOverride : (object)DBNull.Value;
            cmd.Parameters.Add("@iSpid", SqlDbType.Int).Value = tap.iSpid != null ? tap.iSpid : (object)DBNull.Value;
            cmd.Parameters.Add("@iCommissionFlag", SqlDbType.VarChar).Value = tap.iCommissionFlag != null ? tap.iCommissionFlag : (object)DBNull.Value;
            cmd.Parameters.Add("@iPOKey", SqlDbType.Int).Value = tap.iPOKey != null ? tap.iPOKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iPONo", SqlDbType.VarChar).Value = tap.iPONo != null ? tap.iPONo : (object)DBNull.Value;
            cmd.Parameters.Add("@iPOLineKey", SqlDbType.Int).Value = tap.iPOLineKey != null ? tap.iPOLineKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iPOLineNo", SqlDbType.Int).Value = tap.iPOLineNo != null ? tap.iPOLineNo : (object)DBNull.Value;
           
            cmd.Parameters.Add("@iRcvrLineKey", SqlDbType.Int).Value = tap.iRcvrLineKey != null ? tap.iRcvrLineKey : (object)DBNull.Value;
            cmd.Parameters.Add("@iMatchStatus", SqlDbType.Int).Value = tap.iMatchStatus != null ? tap.iMatchStatus : (object)DBNull.Value;
            cmd.Parameters.Add("@iReturnType", SqlDbType.Int).Value = tap.iReturnType != null ? tap.iReturnType : (object)DBNull.Value;
            cmd.Parameters.Add("@oRetVal", SqlDbType.Int).Value = tap.oRetVal != null ? tap.oRetVal : (object)DBNull.Value;
            cmd.Connection.Open();
            try
            {
                var scalar = cmd.ExecuteScalar();
                dtlKey = int.Parse(scalar.ToString());
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Connection.Close();
                throw new Exception(ex.Message);
            }
        }
        return dtlKey;
    }
    public static int Exec_sppaVoucherAPIGst(int Batchkey, int vKey, string Company)
    {
        int RetVal = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "sppaVoucherAPIGst";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Batchkey", SqlDbType.Int).Value = Batchkey;
            cmd.Parameters.Add("@vKey", SqlDbType.Int).Value = vKey;
            cmd.Parameters.Add("@Company", SqlDbType.VarChar).Value = Company;
            cmd.Parameters.Add("@RetVal", SqlDbType.Int).Value = RetVal;
            cmd.Parameters["@RetVal"].Direction = ParameterDirection.InputOutput;

            cmd.Connection.Open();
            cmd.CommandTimeout = 300;
            try
            {
                var scalar = cmd.ExecuteScalar();
                if(scalar != null)
                {
                    RetVal = int.Parse(scalar.ToString());
                }               
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Connection.Close();
                throw new Exception(ex.Message);
            }
            
        }
        return RetVal;
    }

    //public static void ExecuteVoucherApi(string username, Document document)
    //{        
    //    int iLote;
    //    int vkey;
    //    int dtlKey;
    //    int RetVal;       

    //    tapBatchCExtGst tapBatch = new tapBatchCExtGst
    //    {
    //        iBatchType = 401,
    //        iOrigUserID = username,
    //        iSourceCompanyID = document.CompanyId,
    //        iHold = 1,
    //        iPrivate = 1,
    //        status = 0,
    //        FechaCreacion = document.UpdateDate,
    //        FechaModificado = document.UpdateDate,
    //        CompanyID = document.CompanyId,
    //        UserID = username
    //    };

    //    iLote = Insert_tapBatchCExtGst(tapBatch);
    //    tapVoucherCExGst tapVoucher = new tapVoucherCExGst
    //    {
    //        iLote = iLote,
    //        iRemitToVendAddrKey = 48,
    //        iRemitToCopyKey = 48,
    //        iCurrID = "MXN", //Moneda del gasto
    //        iHoldPmt = 0,
    //        iVendKey = 27,
    //        iVendID = "Hellman",
    //        iPmtTermsKey = 10,
    //        iSeparateCheck = 0,
    //        iPurchFromVendAddrKey = 48,
    //        iPurchToCopyKey = 48,
    //        iTranDate = DateTime.Now,
    //        iTranNo = "3456", 
    //        iTranType = 401,
    //        iUserID = username,
    //        iInvcRcptDate = DateTime.Now,
    //        iDueDate = DateTime.Now,
    //        iDiscAmt = 0,
    //        iSTaxAmt = document.Amount,
    //        iPurchAmt = document.Amount,
    //        iTranAmt = document.Amount
    //    };

    //    vkey = Insert_tapVoucherCExGst(tapVoucher);
    //    tapVoucherDtlCExGst tapVoucherDtl = new tapVoucherDtlCExGst
    //    {
    //        iLote = iLote,
    //        vKey = vkey,
    //        iTranNo = string.Empty,
    //        iTargetCompanyID = document.CompanyId,
    //        iCmntOnly = 0,
    //        iExtAmt = document.Amount,
    //        iItemKey = 776,
    //        iQuantity = 1,
    //        iGLAcctKey = 926,
    //        iSTaxClassKey = 7,
    //        iSTaxClassID = "IVA 16",
    //        iUnitCost = document.Amount,
    //        iUnitMeasID = "Unidad",
    //        iFreightAmt = 0,
    //        iSTaxSchdKey = 4,
    //        iSTaxSchdID = "IEPS Import",
    //        iDefaultIfNull = 1,
    //        iMatchStatus = 1,
    //        iReturnType = 1
    //    };

    //    dtlKey = Insert_tapVoucherDtlCExGst(tapVoucherDtl);
    //    RetVal = Exec_sppaVoucherAPIGst(iLote, vkey, "IEP");

    //}

    public static List<EmpleadoDTO> GetEmpleados(int logged_user, int level, DocumentType type)
    {
        List<EmpleadoDTO> empleados = new List<EmpleadoDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();          

            switch (type)
            {
                case DocumentType.Advance:
                    cmd.CommandText = "SELECT distinct UpdateUserKey FROM Advance;";
                    break;
                case DocumentType.Expense:
                    cmd.CommandText = "SELECT distinct UpdateUserKey FROM Expense;";
                    break;
                case DocumentType.CorporateCard:
                    cmd.CommandText = "SELECT distinct UpdateUserKey FROM CorporateCard;";
                    break;
                case DocumentType.MinorMedicalExpense:
                    cmd.CommandText = "SELECT distinct UpdateUserKey FROM MinorMedicalExpense;";
                    break;
            }

            List<int> keys = new List<int>();           
            List<int> employees_keys = new List<int>();           
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                int employee_key = dataReader.GetInt32(0);
                keys.Add(employee_key);
            }
            cmd.Connection.Close();

            //switcheo por niveles patra obtener usuario inferiores en la matriz
            string rol = HttpContext.Current.Session["RolUser"].ToString();

            switch (rol)
            {
                //si el usuario es Cuentas por Cobrar 
                case "T|SYS| - Validador":
                    cmd.CommandText = "SELECT Isnull(UserKey,0) FROM validatingUser where UserValidadorCX = @UserKey";
                    cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    SqlDataReader dataReader_1 = cmd.ExecuteReader();
                    while (dataReader_1.Read())
                    {
                        int employee_key = dataReader_1.GetInt32(0);
                        if (employee_key != 0)
                        {
                            employees_keys.Add(employee_key);
                        }
                    }
                    cmd.Connection.Close();
                    break;
                //si el usuario es Tesoreria --> traer Cuentas por Cobrar
                case "T|SYS| - Tesoreria":
                    cmd.CommandText = "SELECT Isnull(UserKey,0) FROM validatingUser where UserTesoreria = @UserKey";
                    cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    SqlDataReader dataReader_2 = cmd.ExecuteReader();
                    while (dataReader_2.Read())
                    {
                        int employee_key = dataReader_2.GetInt32(0);
                        if (employee_key != 0)
                        {
                            employees_keys.Add(employee_key);
                        }
                    }
                    cmd.Connection.Close();
                    break;
                //si el usuario es Finanzas --> traer Tesoreria
                case "T|SYS| - Finanzas":
                    cmd.CommandText = "SELECT Isnull(UserKey,0) FROM validatingUser where UserFinanzas = @UserKey";
                    cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    SqlDataReader dataReader_3 = cmd.ExecuteReader();
                    while (dataReader_3.Read())
                    {
                        int employee_key = dataReader_3.GetInt32(0);
                        if (employee_key != 0)
                        {
                            employees_keys.Add(employee_key);
                        }
                    }
                    cmd.Connection.Close();
                    break;
                //si el usuario es Finanzas --> traer Tesoreria
                case "T|SYS| - Gerente":
                    cmd.CommandText = "SELECT Isnull(UserKey,0) FROM validatingUser where UserGerente = @UserKey";
                    cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    SqlDataReader dataReader_4 = cmd.ExecuteReader();
                    while (dataReader_4.Read())
                    {
                        int employee_key = dataReader_4.GetInt32(0);
                        if (employee_key != 0)
                        {
                            employees_keys.Add(employee_key);
                        }
                    }
                    cmd.Connection.Close();
                    break;
                //si el usuario es Finanzas --> traer Tesoreria
                case "T|SYS| - Gerencia de Capital Humano":
                    cmd.CommandText = "SELECT Isnull(UserKey,0) FROM validatingUser where UserRH = @UserKey";
                    cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    SqlDataReader dataReader_5 = cmd.ExecuteReader();
                    while (dataReader_5.Read())
                    {
                        int employee_key = dataReader_5.GetInt32(0);
                        if (employee_key != 0)
                        {
                            employees_keys.Add(employee_key);
                        }
                    }
                    cmd.Connection.Close();
                    break;
            }


            foreach (int employee_key in employees_keys)
            {
                if(keys.Contains(employee_key))
                {
                    cmd.CommandText = "SELECT UserKey ,UserID ,UserName FROM Users where UserKey = @userkey;";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@userkey", SqlDbType.Int).Value = employee_key;
                    cmd.Connection.Open();
                    SqlDataReader Reader = cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        var empleado = new EmpleadoDTO();
                        empleado.UserKey = Reader.GetInt32(0);
                        empleado.Correo = Reader.GetString(1);
                        empleado.Nombre = Reader.GetString(2);
                        empleados.Add(empleado);
                    }
                    cmd.Connection.Close();
                }
            }
        }

        return empleados;
    }

    public static List<EmpleadoDTO> GetEmpleadosValidadores(int logged_user, int level)
    {
        List<EmpleadoDTO> empleados = new List<EmpleadoDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            List<int> employees_keys = new List<int>();
            switch (level)
            {
                //si el usuario es Cuentas por Cobrar --> traer Gerente y GerenteRH
                case 2:
                    cmd.CommandText = "SELECT Isnull(UserGerente,0) FROM validatingUser where UserValidadorCX = @logged_user";
                    cmd.Parameters.Add("@logged_user", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    SqlDataReader dataReader_1 = cmd.ExecuteReader();
                    while (dataReader_1.Read())
                    {
                        int employee_key = dataReader_1.GetInt32(0);
                        employees_keys.Add(employee_key);
                    }
                    cmd.Connection.Close();
                    cmd.CommandText = "SELECT Isnull(UserRH,0) FROM validatingUser where UserValidadorCX = @logged_user";
                    cmd.Connection.Open();
                    dataReader_1 = cmd.ExecuteReader();
                    while (dataReader_1.Read())
                    {
                        int employee_key = dataReader_1.GetInt32(0);
                        employees_keys.Add(employee_key);
                    }
                    cmd.Connection.Close();
                    break;
                //si el usuario es Tesoreria --> traer Cuentas por Cobrar
                case 3:
                    cmd.CommandText = "SELECT Isnull(UserGerente,0) FROM validatingUser where UserTesoreria = @logged_user";
                    cmd.Parameters.Add("@logged_user", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    SqlDataReader dataReader_2 = cmd.ExecuteReader();
                    while (dataReader_2.Read())
                    {
                        int employee_key = dataReader_2.GetInt32(0);
                        employees_keys.Add(employee_key);
                    }
                    cmd.Connection.Close();
                    cmd.CommandText = "SELECT Isnull(UserRH,0) FROM validatingUser where UserTesoreria = @logged_user";
                    cmd.Connection.Open();
                    dataReader_2 = cmd.ExecuteReader();
                    while (dataReader_2.Read())
                    {
                        int employee_key = dataReader_2.GetInt32(0);
                        employees_keys.Add(employee_key);
                    }

                    cmd.Connection.Close();
                    cmd.CommandText = "SELECT Isnull(UserValidadorCX,0) FROM validatingUser where UserTesoreria = @logged_user";
                    cmd.Parameters.Add("@logged_user", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    dataReader_2 = cmd.ExecuteReader();
                    while (dataReader_2.Read())
                    {
                        int employee_key = dataReader_2.GetInt32(0);
                        employees_keys.Add(employee_key);
                    }
                    cmd.Connection.Close();
                    break;
                //si el usuario es Finanzas --> traer Tesoreria
                case 4:
                    cmd.CommandText = "SELECT Isnull(UserGerente,0) FROM validatingUser where UserFinanzas = @logged_user";
                    cmd.Parameters.Add("@logged_user", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    SqlDataReader dataReader_3 = cmd.ExecuteReader();
                    while (dataReader_3.Read())
                    {
                        int employee_key = dataReader_3.GetInt32(0);
                        employees_keys.Add(employee_key);
                    }
                    cmd.Connection.Close();
                    cmd.CommandText = "SELECT Isnull(UserRH,0) FROM validatingUser where UserFinanzas = @logged_user";
                    cmd.Connection.Open();
                    dataReader_3 = cmd.ExecuteReader();
                    while (dataReader_3.Read())
                    {
                        int employee_key = dataReader_3.GetInt32(0);
                        employees_keys.Add(employee_key);
                    }

                    cmd.Connection.Close();
                    cmd.CommandText = "SELECT Isnull(UserValidadorCX,0) FROM validatingUser where UserFinanzas = @logged_user";
                    cmd.Parameters.Add("@logged_user", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    dataReader_3 = cmd.ExecuteReader();
                    while (dataReader_3.Read())
                    {
                        int employee_key = dataReader_3.GetInt32(0);
                        employees_keys.Add(employee_key);
                    }
                    cmd.Connection.Close();
                    cmd.CommandText = "SELECT Isnull(UserTesoreria,0) FROM validatingUser where UserFinanzas = @logged_user";
                    cmd.Parameters.Add("@logged_user", SqlDbType.Int).Value = logged_user;
                    cmd.Connection.Open();
                    dataReader_3 = cmd.ExecuteReader();
                    while (dataReader_3.Read())
                    {
                        int employee_key = dataReader_3.GetInt32(0);
                        employees_keys.Add(employee_key);
                    }
                    cmd.Connection.Close();
                    break;
            }


            foreach (int employee_key in employees_keys)
            {
                cmd.CommandText = "SELECT UserKey ,UserID ,UserName FROM Users where UserKey = @UserKey;";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@userkey", SqlDbType.Int).Value = employee_key;
                cmd.Connection.Open();
                SqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    var empleado = new EmpleadoDTO();
                    empleado.UserKey = Reader.GetInt32(0);
                    empleado.Correo = Reader.GetString(1);
                    empleado.Nombre = Reader.GetString(2);
                    empleados.Add(empleado);
                }
                cmd.Connection.Close();
            }
        }
        return empleados;
    }
}
