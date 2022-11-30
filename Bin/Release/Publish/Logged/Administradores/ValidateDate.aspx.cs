using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logged_Administradores_ValidateDate : System.Web.UI.Page
{
    string eventName = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (HttpContext.Current.Session["UserKey"].ToString() == "1")
            {
                DataTable Registros = new DataTable();
                Tabla.DataSource = Registros;
                Tabla.DataBind();
                DatosV.Visible = true;
            }
            else
            {
                Response.Redirect("~/Logged/Administradores/Default.aspx", false);
            }
        }
        catch (Exception ex)
        {
            RutinaError(ex);
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

            if (HttpContext.Current.Session["RolUser"].ToString() != "T|SYS| - Admin")
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

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        eventName = "OnPreInit";
    }

    protected void CargarDatos(object sender, EventArgs e)
    {
        try
        {
            if (FileUpload.HasFile)
            {

            string Archivo = FileUpload.FileName.ToString();
            string inputContent;
            using (StreamReader inputStreamReader = new StreamReader(FileUpload.PostedFile.InputStream))
            {
              inputContent = inputStreamReader.ReadToEnd();
            }
            DataTable Registros = new DataTable();
            DatosV.Visible = true;
            for (int i = 0; i < Tabla.Columns.Count; i++)
            {
              Tabla.Columns.RemoveAt(1);
            }
            Tabla.DataSource = Registros;
            Tabla.DataBind();

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDBug();
            sqlConnection1.Open();

            string SQL = inputContent;
            //string SQL = Consulta.Text;
            //SQL = "Select GetDate()";

            using (var sqlQuery = new SqlCommand(SQL, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = SQL;
                SqlDataReader Cuenta = sqlQuery.ExecuteReader();
                
                Registros.Load(Cuenta);
                Tabla.DataSource = Registros;
                Tabla.DataBind();
            }

            sqlConnection1.Close();

            if (Registros.Rows.Count ==0)
            {
                Label1.Text = "Consulta Ejecutada Exitosamente,no Arroja Resultados";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(AL2);", true);
                DatosV.Visible = true;
            }
            else
            {
                DatosV.Visible = false;
            }


        }
            else
            {
            Label2.Text = "Ingresa una Consulta";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(AL);", true);
            }


    }
        catch (Exception ex)
        {
            Label2.Text = ex.Message;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);
        }

    }

    //Rutina de Conexión
    protected SqlConnection SqlConnectionDBug()
    {
        try
        {

            string Base = string.Empty;
            string dataBase = string.Empty;

            if (SolCont.SelectedValue == "1" || SolCont.SelectedValue == "3")
            {
                Base = "ConnectionString";
            }
            else
            {
                Base = "PortalConnection";
            }

            dataBase = SolCont.SelectedItem.ToString();

            SqlConnection SqlConnectionDB = new SqlConnection();
            ConnectionStringSettings connSettings = ConfigurationManager.ConnectionStrings[Base];
            if ((connSettings != null) && (connSettings.ConnectionString != null))
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connSettings.ConnectionString);
                string server = builder.DataSource;
                string user = builder.UserID;
                string Pass = builder.Password;
                SqlConnectionDB.ConnectionString = "Data Source=" + server + ";Initial Catalog=" + dataBase + ";User ID=" + user + ";Password=" + Pass + "";
            }

            return SqlConnectionDB;
        }
        catch (Exception ex)
        {
            RutinaError(ex);
            return null;
        }
    }

    protected void Limpiar_Click1(object sender, EventArgs e)
    {
        try
        {
            DataTable Registros = new DataTable();
            Tabla.DataSource = Registros;
            Tabla.DataBind();
            DatosV.Visible = true;
            Consulta.Text = "";
        }
        catch(Exception ex)
        {
            RutinaError(ex);
        }
    }

    //Todos los Catch
    protected void RutinaError(Exception ex)
    {
        string Msj = string.Empty;
        StackTrace st = new StackTrace(ex, true);
        StackFrame frame = st.GetFrame(st.FrameCount - 1);
        int LogKey, Userk, VendK;
        string Company = string.Empty;
        if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
        if (HttpContext.Current.Session["IDCompany"].ToString() == "") { Msj = Msj + "," + "Variable IDCompany null"; Company = "TSM"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
        if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
        if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
        Msj = Msj + ex.Message;
        string nombreMetodo = frame.GetMethod().Name.ToString();
        int linea = frame.GetFileLineNumber();
        Msj = Msj + " || Metodo : Administracion_Doc.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
        LogError(LogKey, Userk, " Administracion_Doc.aspx.cs_" + nombreMetodo, Msj, Company);
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
            if (HttpContext.Current.Session["IDCompany"].ToString() == "") { Msj = Msj + "," + "Variable IDCompany null"; Company = "TSM"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Administracion_Doc.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, " Administracion_Doc.aspx.cs_" + nombreMetodo, Msj, Company);
            return null;
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
            if (HttpContext.Current.Session["IDCompany"].ToString() == "") { Msj = Msj + "," + "Variable IDCompany null"; Company = "TSM"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey2 = 0; } else { LogKey2 = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Administracion_Doc.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey2, Userk, "Administracion_Doc.aspx.cs_" + nombreMetodo, Msj, Company);

        }
    }



}