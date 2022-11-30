//PORTAL DE PROVEDORES T|SYS|
//31 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA REGISTRO DE USUARIOS AL PORTAL (PROVEEDORES Y USUARIOS T|SYS|)

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Interop;

public partial class Documentacion_Empleado : System.Web.UI.Page
{
    private readonly Page Me;
    private static Boolean existe;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        if (!IsPostBack)
        {
            if (HttpContext.Current.Session["IDCompany"] == null)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Empleado")
                {
                    if (HttpContext.Current.Session["UserKey"].ToString() != "")
                        existe = validarEmpleado(Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString()));
                }
                else
                {
                    HttpContext.Current.Session.RemoveAll();
                    Context.GetOwinContext().Authentication.SignOut();
                    Response.Redirect("~/Account/Login.aspx");
                }
            }
        }
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        try
        {
            bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (!isAuth)
            {
                HttpContext.Current.Session.RemoveAll();
                Response.AppendHeader("Pragma", "no-cache");
                Response.AppendHeader("Cache-Control", "no-cache");
                Response.CacheControl = "no-cache"; Response.Expires = -1;
                Response.ExpiresAbsolute = new DateTime(1900, 1, 1);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }

            if (HttpContext.Current.Session["RolUser"].ToString() != "T|SYS| - Empleado")
            {
                HttpContext.Current.Session.RemoveAll();
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
        }
        catch
        {
            HttpContext.Current.Session.RemoveAll();
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }
    }

    protected void Unnamed1_Clean(object sender, EventArgs e)
    {
        AreaE.Text = "";
        GefeE.Text = "";
        NombreC.Text = "";
        AreaE.Visible = true;
        NombreC.ReadOnly = false;
        NombreC.Focus();
    }

    protected Boolean RegistroTsys(string Nombre, string Puesto, string Area, string GefeInmediato, string CorreoGefe, string Correo, int UserKey)
    {
        Boolean Res = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spInEmpTsys", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Email", Value = Correo });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserKey", Value = UserKey });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Nombre", Value = Nombre });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Puesto", Value = Puesto });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Area", Value = Area });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Fecha", Value = DateTime.Now});

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@GefeInmediato", Value = GefeInmediato });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@CorreoGefe", Value = CorreoGefe });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                Res = true;
                conn.Close();
            }
        }
        catch (Exception ex)
        {
            Res = false;
        }
        return Res;
    }

    private String obtenerCorreoElectronico(int userKey)
    {
        String correoElectronico = "";

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("uspObtenerUsuario", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@userKey", Value = userKey });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    correoElectronico = rdr["UserID"].ToString();
                }

                conn.Close();
                return correoElectronico;
            }
        }
        catch (Exception e)
        {

        }
        return correoElectronico;
    }

    private Boolean actualizarDatosEmpleado(string Nombre, string Puesto, string Area, string GefeInmediato, string CorreoGefe, int UserKey)
    {
        Boolean bandera = false;

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("UpdateEmployee", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserKey", Value = UserKey });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Nombre", Value = Nombre });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Puesto", Value = Puesto });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Area", Value = Area });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@GefeImediato", Value = GefeInmediato });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@CorreoGefe", Value = CorreoGefe });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                conn.Close();

                return bandera = true;
            }
        }
        catch (Exception e)
        {

        }
        return bandera;
    }

    protected void btn_EnviarTsy(object sender, EventArgs e)
    {
        try
        {
            string Nombre, Correo, Puesto, Lista, Area, GefeInmediato, CorreoGefe;
            Lista = "";

            int UserKey;

            if (NombreC.Text == "")
            {
                string tipo = "error";
                string Msj = "El campo Nombre es obligatorio.";
                string titulo = "Campo obligatorio.";
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
            else if (PuestoE.Text == "")
            {
                string tipo = "error";
                string Msj = "El campo Puesto es obligatorio.";
                string titulo = "Campo obligatorio.";
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
            else if (AreaE.Text == "")
            {
                string tipo = "error";
                string Msj = "El campo Área es obligatorio.";
                string titulo = "Campo obligatorio.";
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
            else if (GefeE.Text == "")
            {
                string tipo = "error";
                string Msj = "El campo Gefe Inmediato es obligatorio.";
                string titulo = "Campo obligatorio.";
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
            else if (CorreoGefeE.Text == "")
            {
                string tipo = "error";
                string Msj = "El campo Correo Gefe Inmediato es obligatorio.";
                string titulo = "Campo obligatorio.";
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
            else
            {
                if (obtenerCorreoElectronico(Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString())) != "")
                {
                    Correo = obtenerCorreoElectronico(Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString()));
                    UserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"]);
                    Nombre = NombreC.Text;
                    Puesto = PuestoE.Text;
                    Area = AreaE.Text;
                    GefeInmediato = GefeE.Text;
                    CorreoGefe = CorreoGefeE.Text;

                    if (existe)
                    {
                        //Actualización
                        if (actualizarDatosEmpleado(Nombre, Puesto, Area, GefeInmediato, CorreoGefe, UserKey))
                        {
                            string tipo = "success";
                            string Msj = "La actualización de los datos del empleado ha sido exitosa.";
                            string titulo = "Actualización exitosa.";
                            ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                            existe = validarEmpleado(Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString()));
                        }
                        else
                        {
                            string tipo = "error";
                            string Msj = "Ocurrio un error al actualizar los datos del Empleado";
                            string titulo = "Error de actualización.";
                            ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                            existe = validarEmpleado(Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString()));
                        }
                    }
                    else
                    {
                        //Nuevo registro
                        if (RegistroTsys(Nombre, Puesto, Area, GefeInmediato, CorreoGefe, Correo, UserKey))
                        {
                            string tipo = "success";
                            string Msj = "La documentación del empleado se ha guardado exitosamente.";
                            string titulo = "Registro exitoso.";
                            ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                            existe = validarEmpleado(Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString()));
                        }
                        else
                        {
                            string tipo = "error";
                            string Msj = "Ocurrio un error al registrar los datos del Empleado.";
                            string titulo = "Error al guardar.";
                            ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            string titulo, Msj, tipo;
            tipo = "error";
            Msj = "Algunos Datos son Invalidos no se puedieron registrar los correos: Verifica que no se hayan registrado anteriormente";
            titulo = "T|SYS| - Error de Ejecucion SP";
        }
    }

    private void mensaje(String titulo, String msj, String tipo)
    {
        ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + msj + "','" + tipo + "');", true);
    }

    protected void btn_Editar(object sender, EventArgs e)
    {
        try
        {
            NombreC.ReadOnly = false;
            NombreC.Focus();
            PuestoE.ReadOnly = false;
            AreaE.ReadOnly = false;
            GefeE.ReadOnly = false;
            CorreoGefeE.ReadOnly = false;
        }
        catch (Exception ex)
        {
            string err;
            err = ex.Message;
        }
    }

    private void LogError(int LogKey, int UpdateUserKey, String proceso, String mensaje, String CompanyID)
    {
        try
        {

            int vkey, val1;
            vkey = 0;
            val1 = 0;
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string sSQL;

            sSQL = "spapErrorLog";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@LogKey", LogKey));
            parsT.Add(new SqlParameter("@UpdateUserKey", UpdateUserKey));
            parsT.Add(new SqlParameter("@proceso", proceso));
            parsT.Add(new SqlParameter("@mensaje", mensaje));
            parsT.Add(new SqlParameter("@CompanyID", CompanyID));
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

    public static SqlConnection SqlConnectionDB(string cnx)
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

    private Boolean validarEmpleado(int userKey)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spValidateEmployee", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@userKey", Value = userKey });
                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    NombreC.Text = rdr["Nombre"].ToString();
                    NombreC.ReadOnly = true;

                    PuestoE.Text = rdr["Puesto"].ToString();
                    PuestoE.ReadOnly = true;

                    AreaE.Text = rdr["Area"].ToString();
                    AreaE.ReadOnly = true;

                    GefeE.Text = rdr["GefeInmediato"].ToString();
                    GefeE.ReadOnly = true;

                    CorreoGefeE.Text = rdr["CorreoGefe"].ToString();
                    CorreoGefeE.ReadOnly = true;

                    return true;
                }

                conn.Close();
            }
        }
        catch (Exception ex)
        {
            string err;
            err = ex.Message;
        }
        return false;
    }

}