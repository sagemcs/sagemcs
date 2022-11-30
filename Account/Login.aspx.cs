//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA PRINCIPAL DE INICIO DE SESSIÓN

//REFERENCIAS UTILIZADAS
using Microsoft.AspNet.Identity;
using System;
using System.Text;
using System.Web.UI;
using WebSite1;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.IO;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Diagnostics;
using System.Collections.Generic;

//CLASE PRINCIPAL
public partial class Account_Login : Page
{   
    //CARGA DE PAGINA
    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["UserKey"] != null)
        {
            HttpContext.Current.Session.RemoveAll();
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }

        else
        {
            if (EmpresaID.Items.Count == 0)
            {
                EmpresaID.Visible = false;
                LEmpresas.Visible = false;
            }
        }


    }

    //PROCESO DE LOGGUEO
    protected void LogIn(object sender, EventArgs e)
    {
        try
        {
            string usuario = UserName1.Text.Trim();
            if (Email_Ok(usuario) == true)
            {
                ApplicationDbContext context = new ApplicationDbContext();
                UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
                //UserValidator<ApplicationUser> Val = new UserValidator<ApplicationUser>(UserManager);
                //Val.AllowOnlyAlphanumericUserNames = false;
                var manager = new UserManager();
                //manager.UserValidator = Val;
                
                string Pass = Password1.Text.Trim();
                ApplicationUser Busca = manager.Find(usuario, Pass);
                if (Busca != null)
                {
                    Global.LoginStatus(usuario);
                    if (HttpContext.Current.Session["Status"].ToString() != "Activo")
                    {
                        UserName1.Text = "";
                        Password1.Text = "";
                        EmpresaID.Items.Clear();
                        LEmpresas.Visible = false;
                        EmpresaID.Visible = false;
                        string titulo, Msj, tipo;
                        tipo = "error";
                        Msj = "Usuario No Activo, Verificalo con el Administrador";
                        titulo = "Error";
                        ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    }
                    else
                    {
                        if (EmpresaID.Visible == true && EmpresaID.SelectedItem.Value != "N/D")
                        {

                            bool err = false;
                            Global.VarSesion(usuario, EmpresaID.SelectedItem.ToString());
                            if (HttpContext.Current.Session["UserKey"].ToString() == "") { err = true; }
                            if (HttpContext.Current.Session["Status"].ToString() == "") { err = true; }
                            if (HttpContext.Current.Session["IDCompany"].ToString() == "") { err = true;}
                            if (HttpContext.Current.Session["RolUser"].ToString() == "") { err = true; }
                            else { if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor") { if (HttpContext.Current.Session["VendKey"].ToString() == "") { err = true; } } }

                            if (err == true)
                            {
                                string titulos, Msjs, tipos;
                                titulos = "Inicio de sessión Portal T|SYS|";
                                tipos = "error";
                                Msjs = "Se ha producido un error al intentar cargar tus datos, comunicate con el Area de sistemas para ofrecerte una Solución.";
                                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulos + "','" + Msjs + "','" + tipos + "');", true);
                                LEmpresas.Visible = false;
                                EmpresaID.Visible = false;
                                return;
                            }

                            if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                            {
                                int Vk = Convert.ToInt16(HttpContext.Current.Session["VendKey"].ToString());
                                string CompanyID = HttpContext.Current.Session["IDCompany"].ToString();
                                if (EstausSAGE(Vk, CompanyID) > 1)
                                {
                                    string titulos, Msjs, tipos;
                                    titulos = "Inicio de sessión Portal T|SYS|";
                                    tipos = "error";
                                    Msjs = "Se ha producido un error al intentar iniciar sesión, usuario INACTIVO en SAGE, comunicate con el área de sistemas para ofrecerte una Solución.";
                                    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulos + "','" + Msjs + "','" + tipos + "');", true);
                                    LEmpresas.Visible = false;
                                    EmpresaID.Visible = false;
                                    return;
                                }

                                //int VkP = Convert.ToInt16(HttpContext.Current.Session["VendKey"].ToString());
                                //string CompanyIDP = HttpContext.Current.Session["IDCompany"].ToString();
                                //if (EstausPortal(VkP, CompanyIDP) > 1)
                                //{
                                //    string titulos, Msjs, tipos;
                                //    titulos = "Inicio de sessión Portal T|SYS|";
                                //    tipos = "error";
                                //    Msjs = "Se ha producido un error al intentar iniciar sesión, usuario INACTIVO en para la Compañia Seleccionada, comunicate con el área de sistemas para ofrecerte una Solución.";
                                //    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulos + "','" + Msjs + "','" + tipos + "');", true);
                                //    LEmpresas.Visible = false;
                                //    EmpresaID.Visible = false;
                                //    return;
                                //}
                            }

                            string key = HttpContext.Current.Session["UserKey"].ToString();
                            int PassT = VerificarPass(key);
                            HttpContext.Current.Session["Passw"] = "0";
                            string titulo, Msj, tipo;
                            titulo = "Actualización de Password";
                            if (PassT == 2)
                            {
                                tipo = "warning";
                                Msj = "Tu Password esta a punto de vencer,te invitamos a renovarlo a la brevedad ya que de lo contrario este se actualizara y tendrás que ejecutar la recuperación de contraseña ";
                                HttpContext.Current.Session["Passw"] = "1";
                                //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);

                            }
                            if (PassT == 3)
                            {
                                tipo = "error";
                                Msj = "Has superado las 72 Horas de tiempo limite para renovar tu password,te invitamos a realizar la recuperacion de contraseña y actualizarlo a la brevedad";
                                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                                LEmpresas.Visible = false;
                                EmpresaID.Visible = false;
                                return;
                            }
                            if (PassT == 4)
                            {
                                tipo = "error";
                                Msj = "Por motivos de seguridad debes de actualizar tu password cada 3 meses y has superado este peridio sin renovarlo, te invitamos a realizar la  recuperacion de contraseña y actualizarlo a la brevedad";
                                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                                LEmpresas.Visible = false;
                                EmpresaID.Visible = false;
                                return;
                            }
                            //else if (PassT == 0)
                            //{
                            if (IsValid)
                            {
                                using (ApplicationDbContext db = new ApplicationDbContext())
                                {

                                    if (DatosLogin() == false)
                                    {
                                        tipo = "error";
                                        Msj = "Se generaron errores al cargar los datos de inicio de sesión, comunícate con el área de soporte para ofrecer una solución";
                                        titulo = "Inicio de Sesión T|SYS|";
                                        ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                                        LEmpresas.Visible = false;
                                        EmpresaID.Visible = false;
                                        return;
                                    }

                                    if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                                    {
                                        IdentityHelper.SignIn(manager, Busca, false);
                                        Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
                                    }
                                    else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
                                    {
                                        IdentityHelper.SignIn(manager, Busca, false);
                                        Response.Redirect("~/Logged/Administradores/Default.aspx", false);
                                    }
                                    else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
                                    {
                                        IdentityHelper.SignIn(manager, Busca, false);
                                        Response.Redirect("~/Logged/Administradores/Default.aspx", false);
                                    }
                                    else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
                                    {
                                        IdentityHelper.SignIn(manager, Busca, false);
                                        Response.Redirect("~/Logged/Administradores/Default.aspx", false);
                                    }

                                    else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Tesoreria")
                                    {
                                        IdentityHelper.SignIn(manager, Busca, false);
                                        Response.Redirect("~/Logged/Administradores/Default.aspx", false);
                                    }

                                    else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Finanzas")
                                    {
                                        IdentityHelper.SignIn(manager, Busca, false);
                                        Response.Redirect("~/Logged/Administradores/Default.aspx", false);
                                    }

                                    else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Empleado")
                                    {
                                        IdentityHelper.SignIn(manager, Busca, false);
                                        Response.Redirect("~/Logged/Administradores/Default_Empleado.aspx", false);
                                    }

                                    else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Gerente")
                                    {
                                        IdentityHelper.SignIn(manager, Busca, false);
                                        Response.Redirect("~/Logged/Administradores/Default.aspx", false);
                                    }

                                    else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Gerencia de Capital Humano")
                                    {
                                        IdentityHelper.SignIn(manager, Busca, false);
                                        Response.Redirect("~/Logged/Administradores/Default.aspx", false);
                                    }
                                    //}
                                }
                            }
                        }///
                        else
                        {
                            User_Empresas(EmpresaID, Busca.UserName.ToString());
                        }
                    }
                }
                else
                {
                    string titulo, Msj, tipo;
                    tipo = "error";
                    Msj = "El Correo y/o Contraseña Invalido, Favor de Verificar";
                    titulo = "Error";
                    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
            }
            else
            {
                string titulo, Msj, tipo;
                tipo = "error";
                Msj = "Formato de Email Invalido";
                titulo = "Error";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }

        }
        catch (Exception ex)
        {
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk;
            string Company = string.Empty;
            Msj = Msj + ex.Message;
            LogKey = 0;
            Userk = 0;
            Company = "tsm";
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Login.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "Login.aspx.cs_" + nombreMetodo, Msj, Company);
        }

    }

    //Validar estado en SAGE
    protected int EstausSAGE(int Vkey,string CompanyID)
    {
        int status = 0;
        try
        {

            string sql;
            string Status;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("ConnectionString");

            sqlConnection1.Open();

            sql = @"SELECT Status from tapvendor Where Vendkey = '" + Vkey + "' And CompanyID = '" + CompanyID + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Status = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            status = Convert.ToInt32(Status);

        }
        catch (Exception ex)
        {
            //Log Errores
        }
        return status;
    }

    //Validar estado en Portal
    protected int EstausPortal(int Vkey, string CompanyID)
    {
        int status = 0;
        try
        {

            string sql;
            string Status;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            sql = @"SELECT Status from Vendors Where Vendkey = '" + Vkey + "' And CompanyID = '" + CompanyID + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Status = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            status = Convert.ToInt32(Status);

        }
        catch (Exception ex)
        {
            //Log Errores
        }
        return status;
    }

    //CAMBIO DE PANTALLA LOG IN A RECUPERACION DE CONTRASEÑA
    protected void Unnamed1_Click(object sender, EventArgs e)
    {
        ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "Reset();", true);
    }

    //CREACION DE PASSWORD ALTERNO
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

    //PROCESO RECUPERACION DE CONTRASEÑA
    protected void btnVer_Click(object sender, EventArgs e)
    {
        try
        {
        string titulo, Msj, tipo;
        bool blnValidUser;
        bool SendEmail;
        bool UpdatePass;

        //if (ctrlGoogleReCaptcha.Validate())
        //{

        blnValidUser = IsValidUser();
        if (blnValidUser == true)
        {
            string PassNew = CrearPassword(8);    
            string PassHAs = PassNew.ToString();
            PassHAs = PassHAs.Replace(" ", "");
            ApplicationDbContext context = new ApplicationDbContext();
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
            UserValidator<ApplicationUser> Val = new UserValidator<ApplicationUser>(UserManager);
            Val.AllowOnlyAlphanumericUserNames = false;
                    string User = UserName.Text.Trim();
            string hashedNewPassword = UserManager.PasswordHasher.HashPassword(PassHAs);
            UpdatePass = UpdatePAss(User, hashedNewPassword);

            if (UpdatePass == true)
            {   
                string Body;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ResetPass.html")))
                {
                 Body = reader.ReadToEnd();
                 Body = Body.Replace("{PassTemp}", PassNew);
                }

                 SendEmail = Global.EmailGlobal(UserName.Text, Body, "Recuperación de Contraseña");
                if (SendEmail == true)
                {
                    tipo = "success";
                    Msj = "Se ha enviado un correo electronico, sigue las instrucciones para restablecer la contraseña";
                    titulo = "Correo de Restablecimiento de Contraseña";
                }
                else
                {
                    tipo = "error";
                    Msj = "Se ha generado un error al enviar el correo, comunícate con el área de soporte para ofrecer una solución";
                    titulo = "Error";
                }
            }
            else
            {
                tipo = "error";
                Msj = "Se ha generado un error al intentar ejecutar la operación, comunícate con el área de soporte para ofrecer una solución";
                titulo = "Error";
            }
        }
        else
        {
            tipo = "error";
            Msj = "El Correo Ingresado, No Existe dentro de los Registros, Favor de Verificar";
            titulo = "Error";
        }

        ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        //}
        //else
        //{
        //    string Caja = "Al";
        //    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
        //}
        }
        catch (Exception ex)
        {
            string Msj1 = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk;
            string Company = string.Empty;
            Msj1 = Msj1 + ex.Message;
            LogKey = 0;
            Userk = 0;
            Company = "tsm";
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj1 = Msj1 + " || Metodo : Login.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "Login.aspx.cs_" + nombreMetodo, Msj1, Company);
        }

    }

    //VALIDACION DE CORREO PARA RECUPERACION DE CONTRASEÑA
    private bool IsValidUser()
    {
        bool cont = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {

                SqlCommand cmd = new SqlCommand("spValUserPortal", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = UserName.Text
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                cont = false;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string Result = rdr["Resultado"].ToString();
                    if (Result != "0")
                    {
                        cont = true;
                    }
                }
                conn.Close();
            }
        }
        catch(Exception ex)
        {
            string Msj1 = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk;
            string Company = string.Empty;
            Msj1 = Msj1 + ex.Message;
            LogKey = 0;
            Userk = 0;
            Company = "tsm";
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj1 = Msj1 + " || Metodo : Login.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "Login.aspx.cs_" + nombreMetodo, Msj1, Company);
            //Tools.LogError(this.ToString() + " _IsValidUser", ex.Message);
            return cont;
        }
        return cont;
    }

    //VALIDACION DE FORMATO DE EMAIL
    private bool Email_Ok(string email)
    {
        try
        {
            string expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
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
        catch (Exception ex)
        {
            string Msj1 = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk;
            string Company = string.Empty;
            Msj1 = Msj1 + ex.Message + " Error de validacion Email";
            LogKey = 0;
            Userk = 0;
            Company = "tsm";
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj1 = Msj1 + " || Metodo : Login.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "Login.aspx.cs_" + nombreMetodo, Msj1, Company);
            return false;
        }
    }

    //CARGA DE EMPRESAS EN LAS QUE ESTA REGISTRADO EL USUARIO
    protected void User_Empresas(DropDownList Caja, string Usuario)
    {
        try
        {
            Caja.Items.Clear();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectCompanyLog", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = Usuario });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                string Errores = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Empresas"].ToString() != "")
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Empresas"].ToString());
                        Caja.Items.Insert(0, Linea);
                        EmpresaID.Visible = true;
                        LEmpresas.Visible = true;
                    }
                    else
                    {
                        throw new Exception(Errores);
                    }
                }
                if (EmpresaID.Items.Count == 0)
                {
                    ListItem Linea = new ListItem();
                    Linea.Value = ("N/D");
                    Caja.Items.Insert(0, Linea);
                    EmpresaID.Visible = true;
                    LEmpresas.Visible = true;
                    throw new Exception("Usuario no Registrado en Empresa");
                }
                conn.Close();
            }
        }
        catch (Exception ex)
        {
            EmpresaID.Items.Clear();
            EmpresaID.Visible = false;
            LEmpresas.Visible = false;
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk;
            string Company = string.Empty;
            Msj = Msj + ex.Message;
            LogKey = 0;
            Userk = ConsultaUserKey (Usuario);
            Company = "tsm";
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Login.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "Login.aspx.cs_" + nombreMetodo, Msj, Company);
            string tipo,titulo;
            tipo = "error";
            titulo = "Inicio de Sesión Portal T|SYS|";
            Msj = "Se ha producido un error al intentar cargar tus datos, comunicate con el Area de sistemas para ofrecerte una Solución.";
            ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            //Tools.LogError(this.ToString() + " _UserEmpresas", ex.Message);
        }
    }

    //PROCESO DE RESETEP DE PASSWORD
    protected bool UpdatePAss(string Email, string NewPass)
    {
        bool Resultado = false;
        try
        {   

            string oldPass = string.Empty;
            string IdUser = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                        SqlCommand cmd1 = new SqlCommand("spResetPass", conn);
                        cmd1.CommandType = CommandType.StoredProcedure;

                        cmd1.Parameters.Add(new SqlParameter()
                        { ParameterName = "@Pass", Value = NewPass });

                        cmd1.Parameters.Add(new SqlParameter()
                        { ParameterName = "@Email", Value = Email });

                        cmd1.Parameters.Add(new SqlParameter()
                        { ParameterName = "@Opcion", Value = 2 });

                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }

                        conn.Open();
                        SqlDataReader rdr1 = cmd1.ExecuteReader();
                        Resultado = true;
                        conn.Close();
                    }
            }

        catch (Exception ex)
        {
            string Msj1 = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk;
            string Company = string.Empty;
            Msj1 = Msj1 + ex.Message + " Error de Acualización Password";
            LogKey = 0;
            Userk = 0;
            Company = "tsm";
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj1 = Msj1 + " || Metodo : Login.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "Login.aspx.cs_" + nombreMetodo, Msj1, Company);
            //Tools.LogError(this.ToString() + " _UpdatePass", ex.Message);
            //string Error = ex.Message;
            Resultado = false;
        }
        return Resultado;
    }

    //PROCESO DE OBTENCION DE ip
    public static string GetDireccionIp(HttpRequest request)
    {
        try
        {
            string ipAddress1 = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            string ipAddress2 = request.ServerVariables["REMOTE_ADDR"];
            string ipAddress = string.IsNullOrEmpty(ipAddress1) ? ipAddress2 : ipAddress1;
            return ipAddress;
        }
        catch (Exception ex)
        {
            string Msj1 = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk;
            string Company = string.Empty;
            Msj1 = Msj1 + ex.Message + " Error al Obtener IP";
            LogKey = 0;
            Userk = 0;
            Company = "tsm";
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj1 = Msj1 + " || Metodo : Login.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "Login.aspx.cs_" + nombreMetodo, Msj1, Company);
            string ipAddress = "N/A";
            return ipAddress;
        }

    }

    //PROCESO DE CARGAR VARIABLES GLOBALES DE USUARIO
    protected bool DatosLogin()
    {
        try
        {
            string IPP = GetDireccionIp(Request);

            string IP = Request.UserHostAddress;
            string User = HttpContext.Current.Session["UserKey"].ToString();
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spAccessLog", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Userkey", Value = User });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Company });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@IP", Value = IP });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                string Errores = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    HttpContext.Current.Session["LogKey"] = Convert.ToInt64(rdr["LogKey"].ToString());
                }
                HttpContext.Current.Session["JWTKey"] = DateTime.Now.Ticks.ToString() + Guid.NewGuid().ToString();

                if (HttpContext.Current.Session["LogKey"] .ToString() == "")
                {
                    throw new Exception("Error al Obtener LogKey, Variable vacia");
                }

            }
            return true;
        }
        catch(Exception ex)
        {
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk;
            string Company = string.Empty;
            Msj = Msj + ex.Message;
            if (HttpContext.Current.Session["UserKey"] == null) { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            if (HttpContext.Current.Session["IDCompany"] == null) { Msj = Msj + "," + "Variable IDCompany null"; Company = "tsm"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["LogKey"] == null) { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Login.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "Login.aspx.cs_" + nombreMetodo, Msj, Company);
            return false;
            //string rest = ex.Message;
            //Tools.LogError(this.ToString() + " _DatosLogin", ex.Message);
            //Response.Redirect("~/Logged/Error?error=Hubo un error mientras se intentaba cargar el reporte de accesos al sistema");
        }

    }

    //PROCESO DE VERIFICACION DEL PASSWORD
    protected int VerificarPass(string Userkey)
    {
        int Resultado = 0;
        try
        {
            if (Userkey != "1")
            {
            int Status = 0;
            string fecha = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("PassTemp", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Userkey", Value = Userkey });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 1 });
                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            if (rdr["StatusPass"] == DBNull.Value  || rdr["UpdateDatePass"] == DBNull.Value)
                            {
                                //Actualiza Password
                                PassCadu(Userkey);
                                Resultado = 3;
                            }
                            else
                            {
                                Status = Convert.ToInt16(rdr["StatusPass"].ToString());
                                fecha = rdr["UpdateDatePass"].ToString();

                                if (Status == 2) //Password Normal
                                {
                                    Resultado = 0;
                                    DateTime fechaF = Convert.ToDateTime(fecha).Date.AddMonths(3);
                                    DateTime fechaL = Convert.ToDateTime(fechaF).Date.AddDays(-3);
                                    DateTime FechAc = DateTime.Now.Date;

                                    if (FechAc > fechaF)
                                    {
                                        //Actualiza Password
                                        PassCadu(Userkey);
                                        Resultado = 4;
                                    }
                                    else
                                    {
                                        //Password por Vencer , Notifica
                                        if (FechAc > fechaL && FechAc < fechaF)
                                        {
                                            Resultado = 2;
                                        }
                                    }


                                }
                                else //Password Temporal
                                {
                                    DateTime fechaF = Convert.ToDateTime(fecha).Date.AddHours(72);
                                    DateTime FechAc = DateTime.Now.Date;

                                    if (FechAc > fechaF)
                                    {
                                        //Actualiza Password
                                        PassCadu(Userkey);
                                        Resultado = 3;
                                    }
                                    else
                                    {
                                        //Password por Vencer , Notifica
                                        Resultado = 2;
                                    }

                                }

                            }
                        }
                    }
                    else
                    {
                        //Actualiza Password
                        PassCadu(Userkey);
                        Resultado = 3;
                    }
             conn.Close();
            }
          }
        }
        catch(Exception ex)
        {
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk;
            string Company = string.Empty;
            Msj = Msj + ex.Message;
            LogKey = 0;
            Userk = Convert.ToInt16(Userkey);
            if (HttpContext.Current.Session["IDCompany"] .ToString() == "") { Msj = Msj + "," + "Variable IDCompany null"; Company = "tsm"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Login.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "Login.aspx.cs_" + nombreMetodo, Msj, Company);
            //Tools.LogError(this.ToString() + " VerificarPass", ex.Message);
        }
        return Resultado;
    }

    //PROCESO DE VALIDACION DE CONTRASEÑA EXPIRADA
    protected void PassCadu(string userKey)
    {
        try
        {
            bool UpdatePass;
            string PassNew = CrearPassword(8);
            string PassHAs = PassNew.ToString();
            PassHAs = PassHAs.Replace(" ", "");
            ApplicationDbContext context = new ApplicationDbContext();
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
            UserValidator<ApplicationUser> Val = new UserValidator<ApplicationUser>(UserManager);
            Val.AllowOnlyAlphanumericUserNames = false;
            string Ususrio = UserName1.Text.Trim();
            string hashedNewPassword = UserManager.PasswordHasher.HashPassword(PassHAs);
            UpdatePass = UpdatePAss(Ususrio, hashedNewPassword);

            if (UpdatePass == false)
            {
                throw new Exception("Error al intentar Actualizar el Password");
          }
        }
        catch(Exception ex)
        {
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk;
            string Company = string.Empty;
            Msj = Msj + ex.Message;
            LogKey = 0;
            Userk = Convert.ToInt16(userKey);
            if (HttpContext.Current.Session["IDCompany"] .ToString() == "") { Msj = Msj + "," + "Variable IDCompany null"; Company = "tsm"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Login.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "Login.aspx.cs_" + nombreMetodo, Msj, Company);
            //Tools.LogError(this.ToString() + " _PassCadu", ex.Message);
        }
    }

    //OBTENCION DE KEYUSER PARA USUARIO
    private int ConsultaUserKey(string Key)
    {

        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();


            sql = @"SELECT UserKey FROM Users WHERE UserID ='" + Key + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }


            sqlConnection1.Close();

            if (Convert.ToInt32(Cuenta) > 0)
                return Convert.ToInt32(Cuenta);
            else
                return Convert.ToInt32(Cuenta);
        }
        catch (Exception ex)
        {
            return 0;
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
            if (HttpContext.Current.Session["UserKey"]== null) { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            if (HttpContext.Current.Session["IDCompany"] == null) { Msj = Msj + "," + "Variable IDCompany null"; Company = "tsm"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["VendKey"] == null) { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"] == null) { Msj = Msj + "," + "Variable LogKey null"; LogKey2 = 0; } else { LogKey2 = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Login.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey2, Userk, "Login.aspx.cs_" + nombreMetodo, Msj, Company);

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
            if (HttpContext.Current.Session["UserKey"] == null) { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            if (HttpContext.Current.Session["IDCompany"] == null) { Msj = Msj + "," + "Variable IDCompany null"; Company = "TSM"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["VendKey"] == null) { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"] == null) { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Login.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "Login.aspx.cs_" + nombreMetodo, Msj, Company);
            return null;
        }
    }

    //Rutina de Variable de Sesion para la Empresa
    protected void EmpresaID_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
          Global.VarSesionlOGIN(UserName.ToString(), EmpresaID.SelectedItem.ToString());
        }
        catch (Exception ex)
        {

        }
    }

}
