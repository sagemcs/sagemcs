//PORTAL DE PROVEDORES T|SYS|
//18 DE OCTUBRE, 2022
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : CESILIO HERNÁNDEZ
//PANTALLA: ASIGNACIÓN DE VALIDADORES

//REFERENCIAS UTILIZADAS
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Windows.Interop;

public partial class Logged_Administradores_AsignacionValidador : System.Web.UI.Page
{
    string eventName = String.Empty;

    private static DataTable dtUsuario_Validador = new DataTable();
    
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

    private void BindGridView(String userName) 
    {
        try
        {
            DatosV.Visible = false;
            Boolean bandera = false;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {

                SqlCommand cmd = new SqlCommand("spSelectUserRole", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter(){ ParameterName = "@userName", Value = userName });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();

                SqlDataReader rdr = cmd.ExecuteReader();

                if (dtUsuario_Validador.Columns.Count == 0) 
                {
                    dtUsuario_Validador.Columns.Add("UserKey");
                    dtUsuario_Validador.Columns.Add("CorreoElec");
                    dtUsuario_Validador.Columns.Add("UserName");
                    dtUsuario_Validador.Columns.Add("Role");
                    dtUsuario_Validador.Columns.Add("UserValidadorCX");
                    dtUsuario_Validador.Columns.Add("UserGerente");
                    dtUsuario_Validador.Columns.Add("UserRH");
                    dtUsuario_Validador.Columns.Add("UserTesoreria");
                    dtUsuario_Validador.Columns.Add("UserFinanzas");
                }

                if (gvValidadores.Rows.Count == 0)
                {
                    dtUsuario_Validador.Clear();
                }

                while (rdr.Read()) 
                {
                    foreach (GridViewRow gvr in gvValidadores.Rows)
                    {
                        if (Convert.ToInt32(rdr["UserKey"].ToString()) == Convert.ToInt32(gvr.Cells[1].Text.ToString()))
                        {
                            bandera = true;
                            return;
                        }
                    }

                    if (bandera == false)
                    {
                        DataRow row = dtUsuario_Validador.NewRow();

                        if(rdr["RoleName"].ToString() != "T|SYS| - Finanzas") 
                        {
                            row["UserKey"] = rdr["UserKey"].ToString();
                            row["CorreoElec"] = rdr["CorreoElec"].ToString();
                            row["UserName"] = rdr["UserName"].ToString();
                            row["Role"] = rdr["RoleName"].ToString();

                            row["UserValidadorCX"] = (rdr["UserValidadorCX"].ToString() == "" || rdr["UserValidadorCX"] == null) ? "0" : rdr["UserValidadorCX"].ToString();
                            row["UserGerente"] = (rdr["UserGerente"].ToString() == "" || rdr["UserGerente"] == null) ? "0" : rdr["UserGerente"].ToString();
                            row["UserRH"] = (rdr["UserRH"].ToString() == "" || rdr["UserRH"] == null) ? "0" : rdr["UserRH"].ToString();
                            row["UserTesoreria"] = (rdr["UserTesoreria"].ToString() == "" || rdr["UserTesoreria"] == null) ? "0" : rdr["UserTesoreria"].ToString();
                            row["UserFinanzas"] = (rdr["UserFinanzas"].ToString() == "" || rdr["UserFinanzas"] == null) ? "0" : rdr["UserFinanzas"].ToString();

                            dtUsuario_Validador.Rows.Add(row);
                        }
                        else 
                        {
                            ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + "error" + "','" + "El usuario con rol de Finanzas no tiene validadores" + "','" + "T|SYS|" + "');", true);
                        }
                    }
                }

                if(gvValidadores.Rows.Count > 0) 
                {
                    gvValidadores.DataSource = null;
                    gvValidadores.DataBind();
                }

                gvValidadores.DataSource = dtUsuario_Validador;
                gvValidadores.DataBind();
            }
        }
        catch (Exception e) 
        {
        
        }
    }

    protected void ValidadoresGridView_RowCommand(Object sender, GridViewCommandEventArgs e) 
    {
        try 
        {
            if (e.CommandName == "Eliminar")
            {
                if (!String.IsNullOrEmpty(e.CommandArgument.ToString()))
                {
                    int index = Convert.ToInt32(e.CommandArgument);
                    GridViewRow fila = gvValidadores.Rows[index];
                    DataRow[] dr = dtUsuario_Validador.Select("UserKey = " + fila.Cells[1].Text);

                    foreach (DataRow row in dr)
                    {
                        dtUsuario_Validador.Rows.Remove(row);
                    }

                    gvValidadores.DataSource = null;
                    gvValidadores.DataBind();
                    gvValidadores.DataSource = dtUsuario_Validador;
                    gvValidadores.DataBind();
                }
            }
        }
        catch(Exception exp)
        {

        }
    }

    protected void ValidadoresGridView_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        String role = "";
        String lblValidadorCX = "";
        String lblGerente = "";
        String lblRecursosH = "";
        String lblTesoreria = "";
        String lblFinanzas = "";

        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[1].Visible = false;
            e.Row.Cells[3].Visible = false;
            e.Row.Cells[4].Visible = false;
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[1].Visible = false;
            e.Row.Cells[3].Visible = false;
            e.Row.Cells[4].Visible = false;
        }
        if (gvValidadores.Rows.Count > 0)
        {
            foreach (GridViewRow gvr in gvValidadores.Rows)
            {
                DropDownList validadorCX = gvr.Cells[5].FindControl("ValidadorCX") as DropDownList;
                DropDownList gerente = gvr.Cells[6].FindControl("Gerentes") as DropDownList;
                DropDownList recursosH = gvr.Cells[7].FindControl("RHs") as DropDownList;
                DropDownList tesoreria = gvr.Cells[8].FindControl("Tesorerias") as DropDownList;
                DropDownList finanzas = gvr.Cells[9].FindControl("Finanzass") as DropDownList;

                role = gvr.Cells[4].Text.ToString();

                if (role != "")
                {
                    if (role == "T|SYS| - Empleado")
                    {
                        validadorCX.Items.Clear();
                        validadorCX.DataSource = cargarValidadores("T|SYS| - Validador");
                        validadorCX.DataTextField = "UserNameRole";
                        validadorCX.DataValueField = "UserKey";
                        validadorCX.DataBind();
                        validadorCX.Items.Insert(0, new ListItem("Seleccione un validador","0"));
                        lblValidadorCX = (gvr.Cells[5].FindControl("lblValidadorCX") as Label).Text;
                        validadorCX.Items.FindByValue(lblValidadorCX).Selected = true;

                        gerente.Items.Clear();
                        gerente.DataSource = cargarValidadores("T|SYS| - Gerente");
                        gerente.DataTextField = "UserNameRole";
                        gerente.DataValueField = "UserKey";
                        gerente.DataBind();
                        gerente.Items.Insert(0, new ListItem("Seleccione un validador","0"));
                        lblGerente = (gvr.Cells[6].FindControl("lblGerente") as Label).Text;
                        gerente.Items.FindByValue(lblGerente).Selected = true;

                        recursosH.Items.Clear();
                        recursosH.DataSource = cargarValidadores("T|SYS| - Gerencia de Capital Humano");
                        recursosH.DataTextField = "UserNameRole";
                        recursosH.DataValueField = "UserKey";
                        recursosH.DataBind();
                        recursosH.Items.Insert(0, new ListItem("Seleccione un validador","0"));
                        lblRecursosH = (gvr.Cells[7].FindControl("lblRH") as Label).Text;
                        recursosH.Items.FindByValue(lblRecursosH).Selected = true;

                        tesoreria.Items.Clear();
                        tesoreria.DataSource = cargarValidadores("T|SYS| - Tesoreria");
                        tesoreria.DataTextField = "UserNameRole";
                        tesoreria.DataValueField = "UserKey";
                        tesoreria.DataBind();
                        tesoreria.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblTesoreria = (gvr.Cells[8].FindControl("lblTesoreria") as Label).Text;
                        tesoreria.Items.FindByValue(lblTesoreria).Selected = true;

                        finanzas.Items.Clear();
                        finanzas.DataSource = cargarValidadores("T|SYS| - Finanzas");
                        finanzas.DataTextField = "UserNameRole";
                        finanzas.DataValueField = "UserKey";
                        finanzas.DataBind();
                        finanzas.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblFinanzas = (gvr.Cells[9].FindControl("lblFinanzas") as Label).Text;
                        finanzas.Items.FindByValue(lblFinanzas).Selected = true;
                    }
                    else if (role == "T|SYS| - Validador")
                    {
                        validadorCX.Items.Clear();
                        validadorCX.DataSource = cargarValidadores("T|SYS| - Validador");
                        validadorCX.DataTextField = "UserNameRole";
                        validadorCX.DataValueField = "UserKey";
                        validadorCX.DataBind();
                        validadorCX.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblValidadorCX = (gvr.Cells[5].FindControl("lblValidadorCX") as Label).Text;
                        validadorCX.Items.FindByValue(lblValidadorCX).Selected = true;

                        gerente.Items.Clear();
                        gerente.DataSource = cargarValidadores("T|SYS| - Gerente");
                        gerente.DataTextField = "UserNameRole";
                        gerente.DataValueField = "UserKey";
                        gerente.DataBind();
                        gerente.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblGerente = (gvr.Cells[6].FindControl("lblGerente") as Label).Text;
                        gerente.Items.FindByValue(lblGerente).Selected = true;

                        recursosH.Items.Clear();
                        recursosH.DataSource = cargarValidadores("T|SYS| - Gerencia de Capital Humano");
                        recursosH.DataTextField = "UserNameRole";
                        recursosH.DataValueField = "UserKey";
                        recursosH.DataBind();
                        recursosH.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblRecursosH = (gvr.Cells[7].FindControl("lblRH") as Label).Text;
                        recursosH.Items.FindByValue(lblRecursosH).Selected = true;

                        tesoreria.Items.Clear();
                        tesoreria.DataSource = cargarValidadores("T|SYS| - Tesoreria");
                        tesoreria.DataTextField = "UserNameRole";
                        tesoreria.DataValueField = "UserKey";
                        tesoreria.DataBind();
                        tesoreria.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblTesoreria = (gvr.Cells[8].FindControl("lblTesoreria") as Label).Text;
                        tesoreria.Items.FindByValue(lblTesoreria).Selected = true;

                        finanzas.Items.Clear();
                        finanzas.DataSource = cargarValidadores("T|SYS| - Finanzas");
                        finanzas.DataTextField = "UserNameRole";
                        finanzas.DataValueField = "UserKey";
                        finanzas.DataBind();
                        finanzas.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblFinanzas = (gvr.Cells[9].FindControl("lblFinanzas") as Label).Text;
                        finanzas.Items.FindByValue(lblFinanzas).Selected = true;

                        validadorCX.Enabled = false;
                        gerente.Enabled = false;
                        recursosH.Enabled = false;
                    }
                    else if (role == "T|SYS| - Gerente" || role == "T|SYS| - Gerencia de Capital Humano")
                    {
                        validadorCX.Items.Clear();
                        validadorCX.DataSource = cargarValidadores("T|SYS| - Validador");
                        validadorCX.DataTextField = "UserNameRole";
                        validadorCX.DataValueField = "UserKey";
                        validadorCX.DataBind();
                        validadorCX.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblValidadorCX = (gvr.Cells[5].FindControl("lblValidadorCX") as Label).Text;
                        validadorCX.Items.FindByValue(lblValidadorCX).Selected = true;

                        gerente.Items.Clear();
                        gerente.DataSource = cargarValidadores("T|SYS| - Gerente");
                        gerente.DataTextField = "UserNameRole";
                        gerente.DataValueField = "UserKey";
                        gerente.DataBind();
                        gerente.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblGerente = (gvr.Cells[6].FindControl("lblGerente") as Label).Text;
                        gerente.Items.FindByValue(lblGerente).Selected = true;

                        recursosH.Items.Clear();
                        recursosH.DataSource = cargarValidadores("T|SYS| - Gerencia de Capital Humano");
                        recursosH.DataTextField = "UserNameRole";
                        recursosH.DataValueField = "UserKey";
                        recursosH.DataBind();
                        recursosH.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblRecursosH = (gvr.Cells[7].FindControl("lblRH") as Label).Text;
                        recursosH.Items.FindByValue(lblRecursosH).Selected = true;

                        tesoreria.Items.Clear();
                        tesoreria.DataSource = cargarValidadores("T|SYS| - Tesoreria");
                        tesoreria.DataTextField = "UserNameRole";
                        tesoreria.DataValueField = "UserKey";
                        tesoreria.DataBind();
                        tesoreria.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblTesoreria = (gvr.Cells[8].FindControl("lblTesoreria") as Label).Text;
                        tesoreria.Items.FindByValue(lblTesoreria).Selected = true;

                        finanzas.Items.Clear();
                        finanzas.DataSource = cargarValidadores("T|SYS| - Finanzas");
                        finanzas.DataTextField = "UserNameRole";
                        finanzas.DataValueField = "UserKey";
                        finanzas.DataBind();
                        finanzas.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblFinanzas = (gvr.Cells[9].FindControl("lblFinanzas") as Label).Text;
                        finanzas.Items.FindByValue(lblFinanzas).Selected = true;

                        gerente.Enabled = false;
                        recursosH.Enabled = false;
                    }
                    else if (role == "T|SYS| - Tesoreria")
                    {
                        validadorCX.Items.Clear();
                        validadorCX.DataSource = cargarValidadores("T|SYS| - Validador");
                        validadorCX.DataTextField = "UserNameRole";
                        validadorCX.DataValueField = "UserKey";
                        validadorCX.DataBind();
                        validadorCX.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblValidadorCX = (gvr.Cells[5].FindControl("lblValidadorCX") as Label).Text;
                        validadorCX.Items.FindByValue(lblValidadorCX).Selected = true;

                        gerente.Items.Clear();
                        gerente.DataSource = cargarValidadores("T|SYS| - Gerente");
                        gerente.DataTextField = "UserNameRole";
                        gerente.DataValueField = "UserKey";
                        gerente.DataBind();
                        gerente.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblGerente = (gvr.Cells[6].FindControl("lblGerente") as Label).Text;
                        gerente.Items.FindByValue(lblGerente).Selected = true;

                        recursosH.Items.Clear();
                        recursosH.DataSource = cargarValidadores("T|SYS| - Gerencia de Capital Humano");
                        recursosH.DataTextField = "UserNameRole";
                        recursosH.DataValueField = "UserKey";
                        recursosH.DataBind();
                        recursosH.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblRecursosH = (gvr.Cells[7].FindControl("lblRH") as Label).Text;
                        recursosH.Items.FindByValue(lblRecursosH).Selected = true;

                        finanzas.Items.Clear();
                        finanzas.DataSource = cargarValidadores("T|SYS| - Finanzas");
                        finanzas.DataTextField = "UserNameRole";
                        finanzas.DataValueField = "UserKey";
                        finanzas.DataBind();
                        finanzas.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblFinanzas = (gvr.Cells[9].FindControl("lblFinanzas") as Label).Text;
                        finanzas.Items.FindByValue(lblFinanzas).Selected = true;

                        tesoreria.Items.Clear();
                        tesoreria.DataSource = cargarValidadores("T|SYS| - Tesoreria");
                        tesoreria.DataTextField = "UserNameRole";
                        tesoreria.DataValueField = "UserKey";
                        tesoreria.DataBind();
                        tesoreria.Items.Insert(0, new ListItem("Seleccione un validador", "0"));
                        lblTesoreria = (gvr.Cells[8].FindControl("lblTesoreria") as Label).Text;
                        tesoreria.Items.FindByValue(lblTesoreria).Selected = true;

                        validadorCX.Enabled = false;
                        gerente.Enabled = false;
                        recursosH.Enabled = false;
                        tesoreria.Enabled = false;
                    }
                }
            }
        }
    }

    private DataTable cargarValidadores(String role) 
    {
        DataTable dt = new DataTable();
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {

                SqlCommand cmd = new SqlCommand("spSelectUsersInRole", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Role", Value = role });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();

                SqlDataReader rdr = cmd.ExecuteReader();

                dt.Columns.Add("UserKey");
                dt.Columns.Add("UserNameRole");

                while (rdr.Read()) 
                {
                    DataRow row = dt.NewRow();

                    row["UserKey"] = Convert.ToString(rdr.GetValue(0));
                    row["UserNameRole"] = Convert.ToString(rdr.GetValue(1));
                    dt.Rows.Add(row);
                }
                
                return dt;
            }
        }
        catch (Exception e) 
        {
        
        }

        return dt = null;
    }

    protected void guardar(object sender, EventArgs e) 
    {
        try
        {
            int userKey = 0;
            String userName = "";
            String correoElec = "";
            String role = "";

            if (gvValidadores.Rows.Count > 0) 
            {
                foreach (GridViewRow gvr in gvValidadores.Rows) 
                {
                    userKey = Convert.ToInt32(gvr.Cells[1].Text.ToString());
                    userName = gvr.Cells[2].Text.ToString();
                    correoElec = gvr.Cells[3].Text.ToString();
                    role = gvr.Cells[4].Text.ToString();

                    DropDownList validadorCX = gvr.Cells[5].FindControl("ValidadorCX") as DropDownList;
                    DropDownList gerente = gvr.Cells[6].FindControl("Gerentes") as DropDownList;
                    DropDownList recursosH = gvr.Cells[7].FindControl("RHs") as DropDownList;
                    DropDownList tesoreria = gvr.Cells[8].FindControl("Tesorerias") as DropDownList;
                    DropDownList finanzas = gvr.Cells[9].FindControl("Finanzass") as DropDownList;

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand("spapInsertValidatingUser", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter outParam1 = new SqlParameter();

                        outParam1.SqlDbType = System.Data.SqlDbType.Int;
                        outParam1.ParameterName = "SaveOrEdit";
                        outParam1.Value = 0;
                        outParam1.Direction = System.Data.ParameterDirection.Output;

                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey ", Value = Convert.ToInt32(userKey) });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserName", Value = userName });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@CorreoElec", Value = correoElec });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Role", Value = role });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserValidadorCX", Value = validadorCX.SelectedValue.ToString() });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserGerente", Value = gerente.SelectedValue.ToString() });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserRH", Value = recursosH.SelectedValue.ToString() });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserTesoreria", Value = tesoreria.SelectedValue.ToString() });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserFinanzas", Value = finanzas.SelectedValue.ToString() });
                        cmd.Parameters.Add(outParam1);

                        if (conn.State == ConnectionState.Open) { conn.Close(); }

                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();

                        if ((int)cmd.Parameters["SaveOrEdit"].Value == 1)
                        {
                            string tipo = "success";
                            string Msj = "La asignación de validadores ha tenido exito.";
                            string titulo = "Guardado exitoso";
                            ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                        }
                        else if ((int)cmd.Parameters["SaveOrEdit"].Value == 2)
                        {
                            string tipo = "success";
                            string Msj = "La actualizacion de validadores ha tenido exito.";
                            string titulo = "Actualización exitoso";
                            ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                        }
                        conn.Close();
                    }
                }
                gvValidadores.DataSource = null;
                gvValidadores.DataBind();
            }
        }
        catch (Exception ex) 
        {
            
        }
    }

    protected void Buscar(object sender, EventArgs e)
    {
        try
        {
            if (Email.Text != "") 
            {
                BindGridView(Email.Text);
                Email.Text = "";
            }
        }
        catch (Exception ex)
        {

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
           
        }
        return null;
    }
}