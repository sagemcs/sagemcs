//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA ACTUALIZACION DE PASSWORD PARA ADMINISTRADORES // VALIDADORES // CONSULTAS  - T|SYS|

//REFERENCIAS UTILIZADAS
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Protocols.WSTrust;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Interop;
using WebSite1;
public partial class Account_Manage : System.Web.UI.Page
{
    string eventName = String.Empty;

    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["RolUser"] != null)
        {
            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
            {
                Page.MasterPageFile = "~/Logged/Administradores/MasterPageContb.master";
            }

            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
            {
                Page.MasterPageFile = "~/Logged/Administradores/SiteVal.master";
            }

            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Empleado")
            {
                Page.MasterPageFile = "~/Logged/Administradores/SiteEmpleado.master";
            }
        }
    }

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        eventName = "OnPreInit";
    }

    protected string SuccessMessage
    {
        get;
        private set;
    }

    protected bool CanRemoveExternalLogins
    {
        get;
        private set;
    }

    private bool Email_Ok(string email)
    {
        string expresion;
        expresion = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
        if (Regex.IsMatch(email, expresion))
        {
            if (Regex.Replace(email, expresion, string.Empty).Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool HasPassword(UserManager manager)
    {
        var user = manager.FindById(User.Identity.GetUserId());
        return (user != null && user.PasswordHash != null);
    }

    protected void Page_Load()
    {

        try
        {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);


        bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

        if (!isAuth)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
            Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
            Page.Response.Cache.SetNoStore();
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Session.RemoveAll();
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }

        if (!IsPostBack)
        {
            if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
            {
                String rol = HttpContext.Current.Session["RolUser"].ToString();

                if (rol == "T|SYS| - Validador" || rol == "T|SYS| - Tesoreria" || rol == "T|SYS| - Finanzas" || rol == "T|SYS| - Gerente" || rol == "T|SYS| - Gerencia de Capital Humano")
                {
                    Seguridad.Visible = true;
                }
                else
                {
                    Seguridad.Visible = false;
                }
            }

                // Determine the sections to render
                UserManager manager = new UserManager();
            if (HasPassword(manager))
            {
                changePasswordHolder.Visible = true;
            }
            else
            {
                setPassword.Visible = true;
                changePasswordHolder.Visible = false;
            }
            CanRemoveExternalLogins = manager.GetLogins(User.Identity.GetUserId()).Count() > 1;

            // Render success message
            var message = Request.QueryString["m"];
            if (message != null)
            {
                // Strip the query string from action
                Form.Action = ResolveUrl("~/Account/Manage");

                SuccessMessage =
                    message == "ChangePwdSuccess" ? "Your password has been changed."
                    : message == "SetPwdSuccess" ? "Your password has been set."
                    : message == "RemoveLoginSuccess" ? "The account was removed."
                    : String.Empty;
                successMessage.Visible = !String.IsNullOrEmpty(SuccessMessage);

                try
                {
                    if (message.ToString() == "ChangePwdSuccess")
                    {
                        string key = HttpContext.Current.Session["UserKey"].ToString();
                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                        {
                            SqlCommand cmd = new SqlCommand("PassTemp", conn);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Userkey", Value = key });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 3 });
                            if (conn.State == ConnectionState.Open) { conn.Close(); }

                            conn.Open();
                            SqlDataReader rdr = cmd.ExecuteReader();
                        }

                        HttpContext.Current.Session["Passw"] = "0";
                        string tipo = "success";
                        string Msj = "Su contraseña ha cambiado";
                        string titulo = "Actualización de Password Exitosa";
                        ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    }
                }
                catch (Exception ex)
                {
                    string Msj = string.Empty;
                    StackTrace st = new StackTrace(ex, true);
                    StackFrame frame = st.GetFrame(st.FrameCount - 1);
                    int LogKey2, Userk, VendK;
                    string Company = string.Empty;
                    if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
                    if (HttpContext.Current.Session["IDCompany"].ToString() == "") { Msj = Msj + "," + "Variable IDCompany null"; Company = "MGS"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
                    if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
                    if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey2 = 0; } else { LogKey2 = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
                    Msj = Msj + ex.Message;
                    string nombreMetodo = frame.GetMethod().Name.ToString();
                    int linea = frame.GetFileLineNumber();
                    Msj = Msj + " || Metodo : Manage.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
                    LogError(LogKey2, Userk, "Manage.cs_" + nombreMetodo, Msj, Company);
                    //Tools.LogError(this.ToString() + " _ChangePwdSuccess", ex.Message);
                }
            }
        }
    }
        catch (Exception ex)
        {
            HttpContext.Current.Session.RemoveAll();
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }
    }

    protected void ChangePassword_Click(object sender, EventArgs e)
    {
        try
        { 
            if (Email_Ok(NewPassword.Text) == true)
            {
                if (IsValid)
                {
                    string Var = User.Identity.GetUserId();
                    UserManager manager = new UserManager();
                    IdentityResult result = manager.ChangePassword(User.Identity.GetUserId(), CurrentPassword.Text, NewPassword.Text);
                    if (result.Succeeded)
                    {
                        var user = manager.FindById(User.Identity.GetUserId());
                        IdentityHelper.SignIn(manager, user, isPersistent: false);
                        Response.Redirect("~/Account/Manage?m=ChangePwdSuccess");
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }

            }
            else
            {    
                string Msj1 = "";
                Msj1 = Msj1 + "La Contraseña debe de contar con:" + "<br/>";
                Msj1 = Msj1 + "Al menos una letra mayúscula" + "<br/>";
                Msj1 = Msj1 + "Al menos una letra minúscula" + "<br/>";
                Msj1 = Msj1 + "Al menos un dígito" + "<br/>";
                Msj1 = Msj1 + "Al menos un caracter especial" + "<br/>";
                Msj1 = Msj1 + "Mínimo ocho digítos de largo" + "<br/>";

                Label2.Text = Msj1;
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);

            }
        }
        catch(Exception ex)
        {
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey2, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            if (HttpContext.Current.Session["IDCompany"].ToString() == "") { Msj = Msj + "," + "Variable IDCompany null"; Company = "MGS"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey2 = 0; } else { LogKey2 = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Manage.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey2, Userk, "Manage.cs_" + nombreMetodo, Msj, Company);
            //Tools.LogError(this.ToString() + " _ChangePassword_Click", ex.Message);
        }

    }

    protected void GenerarToken_Click(object sender, EventArgs e)
    {
        try
        {
            int userKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            String email = obtenerCorreoElectronico(userKey);
            bool Resut = false;

            string PassNew = Membership.GeneratePassword(8, 1);
            string PassHAs = PassNew.ToString();
            PassHAs = PassHAs.Replace(" ", "");

            ApplicationDbContext context = new ApplicationDbContext();
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
            DateTime fechaExp = DateTime.Now.Date;

            Tuple<DateTime, Boolean> result = obtenerFechaExpiracion(2, FormatoFecha().ToString());

            if (result != null)
            {
                if (!result.Item2)
                {
                    //Todo bien
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionToken.html")))
                    {
                        body = reader.ReadToEnd();
                        body = body.Replace("{TokenPass}", PassHAs);
                        body = body.Replace("{CreationDate}", FormatoFecha().ToString());
                        body = body.Replace("{ExpirationDate}", result.Item1.ToString());
                    }

                    Resut = Global.EmailGlobal(email, body, "BIENVENIDO AL PORTAL T|SYS|");

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand("spapInsertSecurityToken", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = userKey });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Token", Value = CrearPassword(8) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@CreationDate", Value = FormatoFecha() });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@ExpirationDate", Value = result.Item1 });

                        if (conn.State == ConnectionState.Open) { conn.Close(); }

                        conn.Open();

                        SqlDataReader rdr = cmd.ExecuteReader();
                        mensaje("Notificaciones T|SYS|", "Notificacion de generación de Token exitoso.", "success");
                    }
                }
                else
                {
                    //Hay que configurar el periodo de expiración del token
                    mensaje("Notificaciones T|SYS|", "Notificacion de configuración de periodo del Token", "warning");
                }
            }
        }
        catch (Exception exc)
        {

        }
    }

    protected DateTime FormatoFecha()
    {
        DateTime fActual = DateTime.Now;
        int año = fActual.Year;
        int mes = fActual.Month;
        int dia = fActual.Day;
        int hora = fActual.Hour;
        int minutos = fActual.Minute;
        int segundos = fActual.Second;
        DateTime formatoCorrecto = new DateTime(año, mes, dia, hora, minutos, 0);
        return formatoCorrecto;
    }
    //private static Tuple<int, int> IntegerDivide(int dividend, int divisor)
    protected Tuple<DateTime, Boolean> obtenerFechaExpiracion(int opcion, String fecha)
    {
        //DateTime fechaExpiracion = new DateTime();
        DateTime fechaExpiracion = Convert.ToDateTime(fecha);

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectPeriodos", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = opcion });


                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();

                SqlDataReader rdr = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(rdr);

                if (dt.Rows[0].ItemArray[0].ToString() != "")
                {
                    rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        switch (rdr["Periodo"].ToString())
                        {

                            case "5 minutos":
                                fechaExpiracion = Convert.ToDateTime(fecha, new CultureInfo("es-ES"));
                                fechaExpiracion = fechaExpiracion.AddMinutes(5);
                                break;
                            case "15 minutos":
                                fechaExpiracion = Convert.ToDateTime(fecha, new CultureInfo("es-ES"));
                                fechaExpiracion = fechaExpiracion.AddMinutes(15);
                                break;
                            case "25 minutos":
                                fechaExpiracion = Convert.ToDateTime(fecha, new CultureInfo("es-ES"));
                                fechaExpiracion = fechaExpiracion.AddMinutes(25);
                                break;
                            case "35 minutos":
                                fechaExpiracion = Convert.ToDateTime(fecha, new CultureInfo("es-ES"));
                                fechaExpiracion = fechaExpiracion.AddMinutes(35);
                                break;
                            case "45 minutos":
                                fechaExpiracion = Convert.ToDateTime(fecha, new CultureInfo("es-ES"));
                                fechaExpiracion = fechaExpiracion.AddMinutes(45);
                                break;
                            case "60 minutos":
                                fechaExpiracion = Convert.ToDateTime(fecha, new CultureInfo("es-ES"));
                                fechaExpiracion = fechaExpiracion.AddHours(1);
                                break;
                            case "Diario":
                                fechaExpiracion = Convert.ToDateTime(fecha, new CultureInfo("es-ES"));
                                fechaExpiracion = fechaExpiracion.AddDays(1);
                                break;

                            case "Semanal":
                                fechaExpiracion = Convert.ToDateTime(fecha, new CultureInfo("es-ES"));
                                fechaExpiracion = fechaExpiracion.AddDays(7);
                                break;

                            case "Quincenal":
                                fechaExpiracion = Convert.ToDateTime(fecha, new CultureInfo("es-ES"));
                                fechaExpiracion = fechaExpiracion.AddDays(15);
                                break;

                            case "Mensual":
                                fechaExpiracion = Convert.ToDateTime(fecha, new CultureInfo("es-ES"));
                                fechaExpiracion = fechaExpiracion.AddDays(31);
                                break;

                            case "Bimestral":
                                fechaExpiracion = Convert.ToDateTime(fecha, new CultureInfo("es-ES"));
                                fechaExpiracion = fechaExpiracion.AddDays(62);
                                break;

                            case "Semestral":
                                fechaExpiracion = Convert.ToDateTime(fecha, new CultureInfo("es-ES"));
                                fechaExpiracion = fechaExpiracion.AddDays(182);
                                break;

                            case "Anual":
                                fechaExpiracion = Convert.ToDateTime(fecha, new CultureInfo("es-ES"));
                                fechaExpiracion = fechaExpiracion.AddDays(365);
                                break;
                        }
                        return new Tuple<DateTime, Boolean>(fechaExpiracion, false);
                        //return fechaExpiracion;
                    }
                }
                else
                {
                    return new Tuple<DateTime, Boolean>(fechaExpiracion, true);
                }
            }
        }
        catch (Exception exc)
        {

        }
        return new Tuple<DateTime, Boolean>(fechaExpiracion, true);
    }

    private void mensaje(String titulo, String msj, String tipo)
    {
        ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + msj + "','" + tipo + "');", true);
    }

    protected String obtenerCorreoElectronico(int userKey)
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

    public string CrearPassword(int longitud)
    {
        string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890$%&#!@";
        StringBuilder res = new StringBuilder();
        Random rnd = new Random();
        while (0 < longitud--)
        {
            res.Append(caracteres[rnd.Next(caracteres.Length)]);
        }
        return res.ToString();
    }

    protected void SetPassword_Click(object sender, EventArgs e)
    {
        if (IsValid)
        {
            // Create the local login info and link the local account to the user
            UserManager manager = new UserManager();
            IdentityResult result = manager.AddPassword(User.Identity.GetUserId(), password.Text);
            if (result.Succeeded)
            {
                Response.Redirect("~/Account/Manage?m=SetPwdSuccess");
            }
            else
            {
                AddErrors(result);
            }
        }
    }

    public IEnumerable<UserLoginInfo> GetLogins()
    {
        UserManager manager = new UserManager();
        var accounts = manager.GetLogins(User.Identity.GetUserId());
        CanRemoveExternalLogins = accounts.Count() > 1 || HasPassword(manager);
        return accounts;
    }

    public void RemoveLogin(string loginProvider, string providerKey)
    {
        UserManager manager = new UserManager();
        var result = manager.RemoveLogin(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        string msg = String.Empty;
        if (result.Succeeded)
        {
            var user = manager.FindById(User.Identity.GetUserId());
            IdentityHelper.SignIn(manager, user, isPersistent: false);
            msg = "?m=RemoveLoginSuccess";
        }
        Response.Redirect("~/Account/Manage" + msg);
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error);
        }
    }

    //Rutina Manejar Errores
    public static void LogError(int LogKey, int UpdateUserKey, String proceso, String mensaje, String CompanyID)
    {
        try
        {

            int val1;
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
            HttpContext.Current.Session["Error"] = err;
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey2, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            if (HttpContext.Current.Session["IDCompany"].ToString() == "") { Msj = Msj + "," + "Variable IDCompany null"; Company = "MGS"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey2 = 0; } else { LogKey2 = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Manage.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey2, Userk, "Manage.cs_" + nombreMetodo, Msj, Company);

        }
    }

    //Rutina de Conexión
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
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            if (HttpContext.Current.Session["IDCompany"].ToString() == "") { Msj = Msj + "," + "Variable IDCompany null"; Company = "MGS"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Manage.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "Manage.cs_" + nombreMetodo, Msj, Company);
            return null;
        }
    }
}