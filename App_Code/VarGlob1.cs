//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//CLASE GLOBAL DE PROCESOS PORTAL

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;


/// <summary>
/// Contains my site's global variables.
/// </summary>
public static class Global
{
    /// <summary>
    /// Global variable storing important stuff.
    /// </summary>
    static string _importantData;

    /// <summary>
    /// Get or set the static important data.
    /// </summary>
    public static string ImportantData
    {
        get
        {
            return _importantData;
        }
        set
        {
            _importantData = value;
        }
    }

    public static void Docs()
    {
        string Msj = string.Empty;
        try

        {

            string User = HttpContext.Current.Session["UserKey"].ToString();
            string Company = HttpContext.Current.Session["IDCompany"].ToString();

            using (System.Data.SqlClient.SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spDocsU2", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Key",
                    Value = User
                });


                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Company",
                    Value = Company
                });
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    HttpContext.Current.Session["Docs"] = rdr["Docs"].ToString();
                    string val = rdr["Docs"].ToString();
                }
            }
        }
        catch (Exception ex)
        {
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            Company = "TSC";
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : VarGlob1.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "VarGlob1.cs_" + nombreMetodo, Msj, Company);
        }

    }

    public static void OpDocrv()
    {
        SqlConnection sqlConnection1 = new SqlConnection();
        sqlConnection1 = SqlConnectionDB("PortalConnection");
        string Vendk = HttpContext.Current.Session["VendKey"].ToString();
        string company = HttpContext.Current.Session["IDCompany"].ToString();
        string Cuenta;
        sqlConnection1.Open();
        string sql = @"Select UpdateDate From Documents where DocID = 9 And VendorKey = " + Vendk + " And CompanyID = '" + company + "'";
        using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
        {
            sqlQuery.CommandType = CommandType.Text;
            sqlQuery.CommandText = sql;
            Cuenta = sqlQuery.ExecuteScalar().ToString();
        }

        sqlConnection1.Close();

        DateTime UpdateDoc;
        DateTime Hoy = DateTime.Now.Date;    //Fecha Hoy 

        if (Cuenta == "")
        {
            sqlConnection1.Open();
            sql = @"Select CreateDate From Documents where DocID = 9 And VendorKey = " + Vendk + " And CompanyID = '" + company + "'";
            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }
            sqlConnection1.Close();
        }

        UpdateDoc = Convert.ToDateTime(Cuenta).Date;  // Fecha ultima Actualización
        DateTime UpdateL = UpdateDoc.AddMonths(6); // Fecha ultima Actualización + 6 Meses
        DateTime UpdateL1 = Hoy.AddMonths(-6); // Fecha Hoy - 6 Meses


        if (UpdateL <= Hoy)  // Si La Fecha de Carga + 6 Meses es Menor a la Fecha actual, ya expiro el tiempo de Prorroga
        {
            HttpContext.Current.Session["Docs"] = "1";
        }
        else
        {
            HttpContext.Current.Session["Docs"] = "0";
        }

    }

    public static void RevDocs()
    {
        int Msj;
        try
        {
            string sql;
            string Cuenta;
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            string Vendk = HttpContext.Current.Session["VendKey"].ToString();
            string company = HttpContext.Current.Session["IDCompany"].ToString();
            sqlConnection1.Open();
            sql = @"Select UpdateDate From Documents where DocID = 9 And VendorKey = " + Vendk + " And CompanyID = '" + company + "'";
            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            DateTime UpdateDoc;
            DateTime Hoy = DateTime.Now.Date;    //Fecha Hoy 

            if (Cuenta == "")
            {
                sqlConnection1.Open();
                sql = @"Select CreateDate From Documents where DocID = 9 And VendorKey = " + Vendk + " And CompanyID = '" + company + "'";
                using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
                {
                    sqlQuery.CommandType = CommandType.Text;
                    sqlQuery.CommandText = sql;
                    Cuenta = sqlQuery.ExecuteScalar().ToString();
                }
                sqlConnection1.Close();
            }

            UpdateDoc = Convert.ToDateTime(Cuenta).Date;  // Fecha ultima Actualización
            DateTime UpdateL = UpdateDoc.AddMonths(6); // Fecha ultima Actualización + 6 Meses
            DateTime UpdateL1 = Hoy.AddMonths(-6); // Fecha Hoy - 6 Meses


            if (UpdateL <= Hoy)  // Si La Fecha de Carga + 6 Meses es Menor a la Fecha actual, ya expiro el tiempo de Prorroga
            {
                //if (UpdateD(Vendk, company) == true)
                //{
                ///Msj = 11;
                HttpContext.Current.Session["Docs"] = "1";
                //}
                //else { Msj = -1; }
            }
            else
            {
                DateTime DateDays = UpdateL.AddDays(-10); //Obtenemos la Fecla limite - 10 Dias
                TimeSpan Days = UpdateL - Hoy; // Obtenemos la diferencia de dias entre la fecha limite y Hoy
                int Dias = Days.Days; //Los dias en Numero
                if (Dias <= 10)
                {
                    //if (UpdateD(Vendk, company) == true) // Actualiza a Pendiente
                    //{
                    //Msj = Dias;
                    HttpContext.Current.Session["Docs"] = "0";// Muestra el Menu completo
                    //}
                    //else { Msj = -1; }
                }
                else
                {
                    int rev = RevisaStatus();
                    if (rev == 1)
                    {
                        //Msj = 0;
                        HttpContext.Current.Session["Docs"] = "0";// Pasa Normal
                    }
                    else
                    {
                        //Msj = 10;
                        HttpContext.Current.Session["Docs"] = "1"; //Envia Alerta
                    }

                }
            }

        }
        catch (Exception)
        {
            //Msj = -1;
        }
        //return Msj;
    }

    public static int RevisaStatus()
    {
        int Stado = 0;
        try
        {
            string sql;
            string Cuenta;
            int contador = 0;
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            string Vendk = HttpContext.Current.Session["VendKey"].ToString();
            string company = HttpContext.Current.Session["IDCompany"].ToString();
            sqlConnection1.Open();

            DateTime UpdateDoc;
            DateTime Hoy = DateTime.Now.Date;    //Fecha Hoy 

            sql = @"Select CreateDate From Documents where DocID = 9 And VendorKey = " + Vendk + " And CompanyID = '" + company + "'";
            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            UpdateDoc = Convert.ToDateTime(Cuenta).Date;  // Fecha ultima Actualización
            DateTime UpdateL = UpdateDoc.AddMonths(6); // Fecha ultima Actualización + 6 Meses
            DateTime UpdateL1 = Hoy.AddMonths(-6); // Fecha Hoy - 6 Meses
            UpdateL1 = UpdateL1.AddDays(10);

            if (UpdateDoc <= UpdateL1)
            {
                contador = 1;
            }
            else
            {
                contador = 0;
            }

            sql = @"Select CreateDate From Documents where DocID = 5 And VendorKey = " + Vendk + " And CompanyID = '" + company + "'";
            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            //}

            UpdateDoc = Convert.ToDateTime(Cuenta).Date;  // Fecha ultima Actualización
            UpdateL = UpdateDoc.AddMonths(6); // Fecha ultima Actualización + 6 Meses
            UpdateL1 = Hoy.AddMonths(-6); // Fecha Hoy - 6 Meses
            UpdateL1 = UpdateL1.AddDays(10);

            if (UpdateDoc <= UpdateL1)
            {
                contador = contador + 1;
            }
            else
            {
                contador = 1;
            }



            if (contador >= 2)
            {
                Stado = 1;
            }
            else
            {
                Stado = 0;
            }

            sqlConnection1.Close();
        }
        catch (Exception ex)
        {


        }
        return Stado;
    }

    public static void OpDoc()
    {
        string Msj = string.Empty;
        try

        {

            string User = HttpContext.Current.Session["UserKey"].ToString();
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            string VednorK = HttpContext.Current.Session["VendKey"].ToString();
            string Cuentas;
            string CuentDoc;
            string Cuenta;
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            string sql1 = @"select Count(*) From Documents Where VendorKey = '" + VednorK + "' AND status = 2";
            using (var sqlQuery = new SqlCommand(sql1, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql1;
                Cuentas = sqlQuery.ExecuteScalar().ToString();
            }

            if (Convert.ToInt16(Cuentas) == 6)
            {
                string sql2 = @"select status From Documents Where VendorKey = '" + VednorK + "' AND DocID = '9'";
                using (var sqlQuery = new SqlCommand(sql2, sqlConnection1))
                {
                    sqlQuery.CommandType = CommandType.Text;
                    sqlQuery.CommandText = sql2;
                    CuentDoc = sqlQuery.ExecuteScalar().ToString();
                }

                if (Convert.ToInt16(CuentDoc) == 1 || Convert.ToInt16(CuentDoc) == 4)
                {
                    HttpContext.Current.Session["Docs"] = "0";
                }

                string sql = @"Select CreateDate From Documents where DocID = 5 And VendorKey = '" + VednorK + "'";
                using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
                {
                    sqlQuery.CommandType = CommandType.Text;
                    sqlQuery.CommandText = sql;
                    Cuenta = sqlQuery.ExecuteScalar().ToString();
                }

                DateTime Hoy = DateTime.Now.Date;    //Fecha Hoy 
                DateTime UpdateDoc = Convert.ToDateTime(Cuenta).Date;  // Fecha ultima Actualización
                DateTime UpdateL = UpdateDoc.AddMonths(6); // Fecha ultima Actualización + 6 Meses

                if (Hoy > UpdateL)  // Si La Fecha de Carga + 6 Meses es Menor a la Fecha actual, ya expiro el tiempo de Prorroga
                {
                    HttpContext.Current.Session["Docs"] = "0";
                }
                else
                {
                    HttpContext.Current.Session["Docs"] = "1";
                }



            }

        }
        catch (Exception ex)
        {
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            Company = "TSC";
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : VarGlob1.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "VarGlob1.cs_" + nombreMetodo, Msj, Company);
        }

    }

    public static void CatchError(Exception ex)
    {

    }

    public static void VarSesion(string user, string Empresa)
    {

        HttpContext.Current.Session["VendKey"] = "";
        HttpContext.Current.Session["UserKey"] = "";
        HttpContext.Current.Session["IDCompany"] = "";
        HttpContext.Current.Session["IDComTran"] = "";
        HttpContext.Current.Session["RolUser"] = "";
        HttpContext.Current.Session["Status"] = "";

        try
        {
            using (System.Data.SqlClient.SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectVarGlobal", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Ususario", Value = user });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Empresa", Value = Empresa });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (HttpContext.Current.Session["IDCompany"].ToString() == "")
                    {
                        HttpContext.Current.Session["IDCompany"] = rdr["IDCompany"].ToString();
                    }
                    HttpContext.Current.Session["VendKey"] = rdr["VendKey"].ToString();
                    HttpContext.Current.Session["UserKey"] = rdr["UserKey"].ToString();
                    HttpContext.Current.Session["IDComTran"] = rdr["IDCompany"].ToString();
                    HttpContext.Current.Session["RolUser"] = rdr["Rol"].ToString();
                    HttpContext.Current.Session["Status"] = rdr["Status"].ToString();
                }

                string User = HttpContext.Current.Session["UserKey"].ToString();
                string Rol = HttpContext.Current.Session["RolUser"].ToString();
                string Status = HttpContext.Current.Session["Status"].ToString();
                string Company = HttpContext.Current.Session["IDCompany"].ToString();

                if (HttpContext.Current.Session["UserKey"].ToString() == "") { throw new Exception("Variable de Sesion UserKey == Null"); }
                if (HttpContext.Current.Session["RolUser"].ToString() == "") { throw new Exception("Variable de Sesion RolUser == Null"); }
                if (HttpContext.Current.Session["Status"].ToString() == "") { throw new Exception("Variable de Sesion Status == Null"); }
                if (HttpContext.Current.Session["IDCompany"].ToString() == "") { throw new Exception("Variable de Sesion IDCompany == Null"); }

                conn.Close();
            }
        }
        catch (Exception ex)
        {
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            if (HttpContext.Current.Session["IDCompany"].ToString() == "") { Company = "TSC"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            LogKey = 0;
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : VarGlob1.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "VarGlob1.cs_" + nombreMetodo, Msj, Company);
        }
    }

    public static void VarSesionlOGIN(string user, string Empresa)
    {

        HttpContext.Current.Session["IDCompany"] = "";
        HttpContext.Current.Session["IDComTran"] = "";
        HttpContext.Current.Session["Status"] = "";

        try
        {
            using (System.Data.SqlClient.SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectVarGlobal", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Ususario", Value = user });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Empresa", Value = Empresa });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (HttpContext.Current.Session["IDCompany"].ToString() == "")
                    {
                        HttpContext.Current.Session["IDCompany"] = rdr["IDCompany"].ToString();
                    }
                    HttpContext.Current.Session["IDComTran"] = rdr["IDCompany"].ToString();
                    HttpContext.Current.Session["Status"] = rdr["Status"].ToString();
                }

                string Status = HttpContext.Current.Session["Status"].ToString();
                string Company = HttpContext.Current.Session["IDCompany"].ToString();

                if (HttpContext.Current.Session["Status"] == null) { throw new Exception("Variable de Sesion Status == Null"); }
                if (HttpContext.Current.Session["IDCompany"] == null) { throw new Exception("Variable de Sesion IDCompany == Null"); }
                conn.Close();
            }
        }
        catch (Exception ex)
        {
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"] == null) { Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            if (HttpContext.Current.Session["IDCompany"] == null) { Company = "TSM"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["VendKey"] == null) { VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            LogKey = 0;
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : VarGlob1.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "VarGlob1.cs_" + nombreMetodo, Msj, Company);
        }
    }

    public static void LoginStatus(string user)
    {
        HttpContext.Current.Session["Status"] = "";

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("SpLoginStatus", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Ususario", Value = user });
                //cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Company", Value = CompanyId });
                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    HttpContext.Current.Session["Status"] = rdr["Status"].ToString();
                }

                conn.Close();
            }
        }
        catch (Exception ex)
        {
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            Company = "TSC";
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : VarGlob1.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "VarGlob1.cs_" + nombreMetodo, Msj, Company);
        }


    }

    public static bool EmailGlobal(string Destinatario, string Body, string Subjet)
    {
        bool rest = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSetingEmail", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = "General" });
                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                string Errores = string.Empty;
                string UsuarioE, PassEm, Puerto1, Host;
                string Emailportal = string.Empty;
                string smtp = string.Empty;
                string PassPortal = string.Empty;
                string PPortal = string.Empty;
                int puerto;
                Host = string.Empty;
                Puerto1 = string.Empty;
                PassEm = string.Empty;
                UsuarioE = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UsuarioE = rdr["UserMail"].ToString();
                    PassEm = rdr["Pass"].ToString();
                    Puerto1 = rdr["Puerto"].ToString();
                    Host = rdr["Host"].ToString();
                }


                Emailportal = UsuarioE;
                smtp = Host;
                PassPortal = PassEm.Replace(" ", "");
                PPortal = Puerto1;
                puerto = Convert.ToInt32(PPortal);


                //CORREO
                MailMessage Correo = new MailMessage();
                Correo.From = new System.Net.Mail.MailAddress(Emailportal);
                Correo.To.Add(Destinatario);
                Correo.Subject = (Subjet);
                Correo.Body = Body;
                Correo.Priority = System.Net.Mail.MailPriority.Normal;
                Correo.IsBodyHtml = true;

                //SMPT
                System.Net.Mail.SmtpClient ServerMail = new System.Net.Mail.SmtpClient();
                ServerMail.Port = Convert.ToInt16(puerto);
                ServerMail.Host = smtp;

                if (PassPortal != "")
                {
                    ServerMail.Credentials = new NetworkCredential(Emailportal, PassPortal);
                    ServerMail.EnableSsl = true;
                }
                else
                {
                    ServerMail.Credentials = CredentialCache.DefaultNetworkCredentials;
                }

                try
                {
                    ServerMail.Send(Correo);
                    rest = true;
                }
                catch (Exception ex)
                {
                    ServerMail.Dispose();
                    rest = false;
                    string Msj = string.Empty;
                    StackTrace st = new StackTrace(ex, true);
                    StackFrame frame = st.GetFrame(st.FrameCount - 1);
                    int LogKey, Userk, VendK;
                    string Company = string.Empty;
                    if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
                    Company = HttpContext.Current.Session["IDCompany"].ToString();
                    if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
                    if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
                    Msj = Msj + ex.Message;
                    string nombreMetodo = frame.GetMethod().Name.ToString();
                    int linea = frame.GetFileLineNumber();
                    Msj = Msj + " || Metodo : VarGlob1.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
                    LogError(LogKey, Userk, "VarGlob1.cs_" + nombreMetodo, Msj, Company);
                }

            }
        }

        catch (Exception ex)
        {
            rest = false;
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            Company = HttpContext.Current.Session["IDCompany"].ToString();
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : VarGlob1.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "VarGlob1.cs_" + nombreMetodo, Msj, Company);
        }
        return rest;
    }

    public static bool VerTipoProvE(int VendKey)
    {
        bool rest = false;
        try
        {
            string User = HttpContext.Current.Session["UserKey"].ToString();
            string Company = HttpContext.Current.Session["IDCompany"].ToString();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spVerTipo", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Key", Value = VendKey });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Company", Value = Company });
                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                string UsuarioE = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UsuarioE = rdr["Tipo"].ToString();
                }


                if (UsuarioE == "E")
                {
                    rest = true;
                }

                //if (UsuarioE == "S") 
                //{
                //    rest = true;
                //}

            }
        }

        catch (Exception ex)
        {
            rest = false;
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            Company = HttpContext.Current.Session["IDCompany"].ToString();
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : VarGlob1.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "VarGlob1.cs_" + nombreMetodo, Msj, Company);
        }
        return rest;
    }

    public static bool VerTipoProv(int VendKey)
    {
        bool rest = false;
        try
        {
            string User = HttpContext.Current.Session["UserKey"].ToString();
            string Company = HttpContext.Current.Session["IDCompany"].ToString();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spVerTipo", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Key", Value = VendKey });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Company", Value = Company });
                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                string UsuarioE = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UsuarioE = rdr["Tipo"].ToString();
                }


                //if (UsuarioE == "E")
                //{
                //    rest = true;
                //}

                if (UsuarioE == "S")
                {
                    rest = true;
                }

            }
        }

        catch (Exception ex)
        {
            rest = false;
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            Company = HttpContext.Current.Session["IDCompany"].ToString();
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : VarGlob1.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "VarGlob1.cs_" + nombreMetodo, Msj, Company);
        }
        return rest;
    }

    public static bool EmailGlobalAdd(string Destinatario, string Body, string Subjet, Stream File, string FileName)
    {
        bool rest = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSetingEmail", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = "General" });
                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                string Errores = string.Empty;
                string UsuarioE, PassEm, Puerto1, Host;
                string Emailportal = string.Empty;
                string smtp = string.Empty;
                string PassPortal = string.Empty;
                string PPortal = string.Empty;
                int puerto;
                Host = string.Empty;
                Puerto1 = string.Empty;
                PassEm = string.Empty;
                UsuarioE = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UsuarioE = rdr["UserMail"].ToString();
                    PassEm = rdr["Pass"].ToString();
                    Puerto1 = rdr["Puerto"].ToString();
                    Host = rdr["Host"].ToString();
                }


                Emailportal = UsuarioE;
                smtp = Host;
                PassPortal = PassEm.Replace(" ", "");
                PPortal = Puerto1;
                puerto = Convert.ToInt32(PPortal);

                //Memory
                File.Seek(0, SeekOrigin.Begin);
                ContentType contentType = new ContentType();
                contentType.MediaType = MediaTypeNames.Application.Octet;
                contentType.Name = FileName;
                Attachment attachment = new Attachment(File, contentType);

                //CORREO
                MailMessage Correo = new MailMessage();
                Correo.From = new System.Net.Mail.MailAddress(Emailportal);
                Correo.To.Add(Destinatario);
                Correo.Subject = (Subjet);
                System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Text.Plain);
                Correo.Attachments.Add(attachment);
                Correo.Body = Body;
                Correo.Priority = System.Net.Mail.MailPriority.Normal;
                Correo.IsBodyHtml = true;

                //SMPT
                System.Net.Mail.SmtpClient ServerMail = new System.Net.Mail.SmtpClient();
                ServerMail.Port = Convert.ToInt16(puerto);
                ServerMail.Host = smtp;

                if (PassPortal != "")
                {
                    ServerMail.Credentials = new NetworkCredential(Emailportal, PassPortal);
                    ServerMail.EnableSsl = true;
                }
                else
                {
                    ServerMail.Credentials = CredentialCache.DefaultNetworkCredentials;
                }

                try
                {
                    ServerMail.Send(Correo);
                    rest = true;
                }
                catch (Exception ex)
                {
                    ServerMail.Dispose();
                    rest = false;
                    string Msj = string.Empty;
                    StackTrace st = new StackTrace(ex, true);
                    StackFrame frame = st.GetFrame(st.FrameCount - 1);
                    int LogKey, Userk, VendK;
                    string Company = string.Empty;
                    if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
                    Company = HttpContext.Current.Session["IDCompany"].ToString();
                    if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
                    if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
                    Msj = Msj + ex.Message;
                    string nombreMetodo = frame.GetMethod().Name.ToString();
                    int linea = frame.GetFileLineNumber();
                    Msj = Msj + " || Metodo : VarGlob1.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
                    LogError(LogKey, Userk, "VarGlob1.cs_" + nombreMetodo, Msj, Company);
                }

            }
        }

        catch (Exception ex)
        {
            rest = false;
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            Company = HttpContext.Current.Session["IDCompany"].ToString();
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : VarGlob1.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "VarGlob1.cs_" + nombreMetodo, Msj, Company);
        }
        return rest;
    }

    //Rutina Manejar Errores
    public static void LogError(int LogKey, int UpdateUserKey, String proceso, String mensaje, String CompanyID)
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
            HttpContext.Current.Session["Error"] = err;
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey2, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            Company = "TSC";
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey2 = 0; } else { LogKey2 = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : VarGlob1.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey2, Userk, "VarGlob1.cs_" + nombreMetodo, Msj, Company);

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
            Msj = Msj + " || Metodo : VarGlob1.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, "VarGlob1.cs_" + nombreMetodo, Msj, Company);
            return null;
        }
    }


}