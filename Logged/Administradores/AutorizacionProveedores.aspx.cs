//PORTAL DE PROVEDORES T|SYS|
//25 DE JULIO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA GESTION DE USUARIOS

//REFERENCIAS UTILIZADAS
using System;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.IO;
using Microsoft.AspNet.Identity;
using WebSite1;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

public partial class AutorizacionProveedores : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

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

        if (!IsPostBack)
        {
            if (HttpContext.Current.Session["IDCompany"] == null)
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
                {

                    if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
                    {
                        CargarProveedores();
                        CargarProveedores2();
                        CargarUsersTSYS();
                        string Prov = DropDownList2.SelectedItem.ToString();
                        Datos(Prov);
                        CheckBox1.Checked = false;
                        CheckBox3.Checked = false;
                    }
                    else
                    {
                        HttpContext.Current.Session.RemoveAll();
                        Context.GetOwinContext().Authentication.SignOut();
                        Response.Redirect("~/Account/Login.aspx");
                    }

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

    string eventName = String.Empty;

    protected void CargarUsersTSYS()
    {
        try
        {
            int Cuenta = 0;
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectUserDoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                string Company = HttpContext.Current.Session["IDCompany"].ToString();
                int Opc = 16;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = Opc });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = "" });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Company });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                DropDownList2.Items.Clear();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Nombre"].ToString().Length > 1)
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Nombre"].ToString());
                        DropDownList2.Items.Insert(0, Linea);
                        Cuenta = Cuenta + 1;
                    }

                }

                conn.Close();
            }
        }
        catch (Exception ex)
        {
            int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string CompanyIDs = HttpContext.Current.Session["IDCompany"].ToString();
            LogError(pLogKey, pUserKey, "Asignacion de Usuarios  Carga de Provedores", "Error en rutina de Carga Combo - " + ex.Message, CompanyIDs);
            string titulo, Msj, tipo;
            tipo = "info";
            Msj = "Se ha Generado un error durante ejecución de la actividad, intentalo nuevamente si el problema persiste contacta al área de soporte";
            titulo = "Aprobacion de Usuario";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
    }

    protected void CargarProveedores()
    {
        try
        {
            int Cuenta = 0;
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectUserDoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                string Company = HttpContext.Current.Session["IDCompany"].ToString();
                int Opc = 15;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = Opc });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = "" });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Company });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                STipoProv.Items.Clear();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Nombre"].ToString().Length > 1)
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Nombre"].ToString());
                        STipoProv.Items.Insert(0, Linea);
                        Cuenta = Cuenta + 1;
                    }

                }

                conn.Close();
            }
        }
        catch (Exception ex)
        {
            int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string CompanyIDs = HttpContext.Current.Session["IDCompany"].ToString();
            LogError(pLogKey, pUserKey, "Asignacion de Usuarios  Carga de Provedores", "Error en rutina de Carga Combo - " + ex.Message, CompanyIDs);
            string titulo, Msj, tipo;
            tipo = "info";
            Msj = "Se ha Generado un error durante ejecución de la actividad, intentalo nuevamente si el problema persiste contacta al área de soporte";
            titulo = "Aprobacion de Usuario";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
    }

    protected void CargarProveedores2()
    {
        try
        {
            int Cuenta = 0;
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectUserDoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                string Company = HttpContext.Current.Session["IDCompany"].ToString();
                int Opc = 17;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = Opc });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = "" });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Company });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                DropDownList1.Items.Clear();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Nombre"].ToString().Length > 1)
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Nombre"].ToString());
                        DropDownList1.Items.Insert(0, Linea);
                        Cuenta = Cuenta + 1;
                    }

                }

                conn.Close();
            }
            if (Cuenta > 0) 
            {
                try
                {
                    string Prov = DropDownList1.SelectedItem.ToString();
                    Datos2(Prov);
                }
                catch (Exception ex)
                {
                    RutinaError(ex);
                }
            }
        }
        catch (Exception ex)
        {
            int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string CompanyIDs = HttpContext.Current.Session["IDCompany"].ToString();
            LogError(pLogKey, pUserKey, "Asignacion de Usuarios  Carga de Provedores", "Error en rutina de Carga Combo - " + ex.Message, CompanyIDs);
            string titulo, Msj, tipo;
            tipo = "info";
            Msj = "Se ha Generado un error durante ejecución de la actividad, intentalo nuevamente si el problema persiste contacta al área de soporte";
            titulo = "Aprobacion de Usuario";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
    }

    protected void CargarProveedores3(string user)
    {
        try
        {
            int Cuenta = 0;
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectUserDoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                string Company = HttpContext.Current.Session["IDCompany"].ToString();
                int Opc = 17;
                if (user != "") { Opc = 18; } else { user = ""; }

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = Opc });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = user });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Company });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                DropDownList3.Items.Clear();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Nombre"].ToString().Length > 1)
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Nombre"].ToString());
                        DropDownList3.Items.Insert(0, Linea);
                        Cuenta = Cuenta + 1;
                    }

                }

                conn.Close();
            }
        }
        catch (Exception ex)
        {
            int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string CompanyIDs = HttpContext.Current.Session["IDCompany"].ToString();
            LogError(pLogKey, pUserKey, "Asignacion de Usuarios  Carga de Provedores", "Error en rutina de Carga Combo - " + ex.Message, CompanyIDs);
            string titulo, Msj, tipo;
            tipo = "info";
            Msj = "Se ha Generado un error durante ejecución de la actividad, intentalo nuevamente si el problema persiste contacta al área de soporte";
            titulo = "Aprobacion de Usuario";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
    }

    protected void Unnamed1_Clean(object sender, EventArgs e)
    {
        Nombre.Text = "";
        GridView3.DataSource = null;
        GridView3.Visible = false;
        GridView3.DataBind();
        DatosV.Visible = true;
    }

    protected void Unnamed2_Clean(object sender, EventArgs e)
    {
        try
        {
            CheckBox1.Checked = false;
            Cambio.Visible = false;
            CheckBox3.Checked = false;
            Div1.Visible = false;
        }
        catch (Exception ex) 
        {
            RutinaError(ex);
        }
    }

    protected void SelProv_SelectedIndexChanged1(object sender, EventArgs e)
    {
        try
        {
            string Prov = DropDownList2.SelectedItem.ToString();
            Datos(Prov);
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

    protected void Datos(string ID)
    {
        try
        {
            GridView3.DataSource = null;
            GridView3.DataBind();
            GridView3.Visible = false;
            string SQL = string.Empty;
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            SQL = "";
            SQL = " Select a.VendorID as ID,a.VendName as Proveedor,b.Descripcion as Estatus,c.UserID as Useri ";
            SQL = SQL + " From Vendors a ";
            SQL = SQL + " inner join StatusUsers b on a.Status = b.Status ";
            SQL = SQL + " inner join Users c on a.Aprobador = c.UserKey ";
            SQL = SQL + " Where c.UserName = '" + ID + "'";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.CommandType = CommandType.Text;
                if (conn.State == ConnectionState.Open){conn.Close();} conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable Registros = new DataTable();
                Registros.Load(rdr);
                int Filas = Registros.Rows.Count;
                if (Filas >= 1)
                {
                    DataRow Fila = Registros.Rows[0];
                    Nombre.Text = (Fila["Useri"].ToString());
                    GridView3.DataSource = Registros;
                    GridView3.DataBind();
                    GridView3.Visible = true;
                    DatosV.Visible = false;
                }
            }
            if (GridView3.Rows.Count == 0) 
            {
                GridView3.Visible = false;
                DatosV.Visible = true;
                SQL = " Select UserID as Useri From Users Where UserName = '" + ID + "'";
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand(SQL, conn);
                    cmd.CommandType = CommandType.Text;
                    if (conn.State == ConnectionState.Open) { conn.Close(); }
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    DataTable Registros = new DataTable();
                    Registros.Load(rdr);
                    int Filas = Registros.Rows.Count;
                    if (Filas >= 1)
                    {
                        DataRow Fila = Registros.Rows[0];
                        Nombre.Text = (Fila["Useri"].ToString());                       
                    }
                }

            }
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

    protected void Datos2(string ID)
    {
        string key = string.Empty;
        try
        {
            string SQL = string.Empty;
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            SQL = "";
            SQL = "  Select b.userId as Correo From vendors a inner join Users b on a.UserKey = b.UserKey ";
            SQL = SQL + " Where a.VendName = '" + ID + "' And a.CompanyID = '" + Company + "'";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.CommandType = CommandType.Text;
                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable Registros = new DataTable();
                Registros.Load(rdr);
                int Filas = Registros.Rows.Count;
                if (Filas >= 1)
                {
                    DataRow Fila = Registros.Rows[0];
                    Email.Text = (Fila["Correo"].ToString());
                }
            }
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
        try
        {
            string SQL = string.Empty;
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            SQL = "";
            SQL = "  Select b.UserName as Actual,b.userkey as UKey From vendors a inner join Users b on a.Aprobador = b.UserKey ";
            SQL = SQL + " Where a.VendName = '" + ID + "' And a.CompanyID = '" + Company + "'";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.CommandType = CommandType.Text;
                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable Registros = new DataTable();
                Registros.Load(rdr);
                int Filas = Registros.Rows.Count;
                if (Filas >= 1)
                {
                    DataRow Fila = Registros.Rows[0];
                    TextBox2.Text = (Fila["Actual"].ToString());
                    key = (Fila["UKey"].ToString());
                }
            }
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
        try
        {
            CargarProveedores3(key);  
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

    protected void CambioCheck(object sender, EventArgs e)
    {
        try
        {
            if (CheckBox1.Checked == true)
            {
                Cambio.Visible = true;
                Div1.Visible = false;
                CheckBox3.Checked = false;
            }
            else 
            {
                Cambio.Visible = false;
            }
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

    protected void CambioCheck2(object sender, EventArgs e)
    {
        try
        {
            if (CheckBox3.Checked == true)
            {
                Cambio.Visible = false;
                Div1.Visible = true;
                CheckBox1.Checked = false;
            }
            else
            {
                Div1.Visible = false;
            }
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

    protected void Campos(object sender, EventArgs e)
    {
        string Caja = string.Empty;

        // *** AddedControl Luis Angel Mayo 2022 *** \\\
        if (Nombre.Text == "")
        {
            Caja = "AID3";
            tplabel.Text = "El campo Nombre del usuario no puede estar vacio";
        }
        else if (STipoProv.Text == "")
        {
            Caja = "AID3";
            tplabel.Text = "El campo proveedor no puede estar vacio";
        }
        else if (DropDownList2.Text == "")
        {
            Caja = "AID3";
            tplabel.Text = "El campo Usuario no puede estar vacio";
        }

        //
        // *** AddedControl Luis Angel Mayo 2022 *** \\\

        if (Caja != "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);

        }
        else
        {
            string Dock = STipoProv.SelectedItem.ToString();
            string Doc = DropDownList2.SelectedItem.ToString();
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "Pregunta('" + Dock + "','" + Doc + "');", true);
            //llenado();
        }
    }

    protected void Campos2(object sender, EventArgs e)
    {
        string Caja = string.Empty;

        // *** AddedControl Luis Angel Mayo 2022 *** \\\
        if (CheckBox1.Checked == false && CheckBox3.Checked == false)
        {
            Caja = "AID";
            Label2.Text = "Seleccione al menos una ópción para el Proveedor";
        }
        else if (Email.Text == "" && CheckBox3.Checked == true)
        {
            Caja = "AID";
            Label2.Text = "El campo de correo electronico del proveedor no puede estar vacio";
        }
        // *** AddedControl Luis Angel Mayo 2022 *** \\\

        if (Caja != "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);

        }
        else
        {
            string prove = DropDownList1.SelectedItem.ToString();
            string valors = "0";
            string nuevos = Email.Text.ToString();

            if (CheckBox3.Checked == true)
            {
                valors = "0";
                nuevos = Email.Text.ToString();
            }
            else 
            {
                valors = "1";
                nuevos = DropDownList3.SelectedItem.ToString();
            }

            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "Pregunta2('" + prove + "','" + valors + "','" + nuevos + "');", true);
        }
    }

    protected void SelProv_SelectedIndexChanged2(object sender, EventArgs e)
    {
        try
        {
            string Prov = DropDownList1.SelectedItem.ToString();
            Datos2(Prov);
        }
        catch (Exception ex)
        {
            RutinaError(ex);
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
            return null;
            throw new MulticonsultingException(ex.Message);

        }
    }

    //Rutina Manejar Errores
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
            string CompanyIDs = HttpContext.Current.Session["IDCompany"].ToString();
            string sSQL;

            sSQL = "spapErrorLog";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@LogKey", LogKey));
            parsT.Add(new SqlParameter("@UpdateUserKey", UpdateUserKey));
            parsT.Add(new SqlParameter("@proceso", proceso));
            parsT.Add(new SqlParameter("@mensaje", mensaje));
            parsT.Add(new SqlParameter("@CompanyID", CompanyIDs));

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
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
            // ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B6);", true);
        }
    }

    protected void RutinaError(Exception ex)
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
        Msj = Msj + " || Metodo : Administracion_Doc.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
        LogError(LogKey, Userk, " Administracion_Doc.aspx.cs_" + nombreMetodo, Msj, Company);
        //lblMsj.Text = ex.Message;
    }

}