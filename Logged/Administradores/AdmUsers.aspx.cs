//PORTAL DE PROVEDORES T|SYS|
//15 DE FEBRERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA DE APROBACION DE USUARIOS

//REFERENCIAS UTILIZADAS
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Protocols.WSTrust;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebSite1;

public partial class Logged_Administradores_AdmUsers : System.Web.UI.Page
{
    string eventName = String.Empty;

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
                    cargarPeriodos(2);
                    cargarPeriodos(1);
                    if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
                    {
                        Button3.Visible = false;
                        Cam1.Visible = false;
                        EmailUp.Text = "";
                        Nvoemail.Text = "";
                        DataTable Vacio = new DataTable();
                        GridView4.DataSource = Vacio;
                        GridView4.DataBind();
                        BindGridView();
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

    protected void Buscar(object sender, EventArgs e)
    {
        try
        {
            BindGridView();
        }
        catch (Exception ex)
        {

        }
    }

    protected void btn_Guardar(object sender, EventArgs e)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spInsertExpirationToken", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@PeriodoKey", Value = Convert.ToInt32(Periodo.SelectedValue) });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Periodo", Value = Convert.ToString(Periodo.SelectedItem) });
                
                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                conn.Close();

                mensaje("Notificaciones T|SYS|", "Notificacion de configuración de periodo del Token exitoso.", "success");
            }
        }
        catch(Exception exc)
        {
            mensaje("Notificaciones T|SYS|", "Error: " + exc.Message , "error");
        }
    }

    protected void mensaje(String titulo, String msj, String tipo)
    {
        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + msj + "','" + tipo + "');", true);
    }

    protected void cargarPeriodos(int opcion)
    {
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
                if(rdr != null)

                {
                    if (opcion == 2)
                    {
                        while (rdr.Read())
                        {
                            lblPeriodo.Text = Convert.ToString(rdr.GetValue(1));
                        }
                    }
                    else if (opcion == 1)
                    {
                        Periodo.DataSource = rdr;
                        Periodo.DataValueField = "PeriodoKey";
                        Periodo.DataTextField = "Periodo";
                        Periodo.DataBind();
                        Periodo.Items.Insert(0, new ListItem("Seleccione un elemento", "0"));
                        Periodo.Items.FindByValue(lblPeriodo.Text).Selected = true;
                    }
                }
                conn.Close();
            }
        }
        catch(Exception e)
        {

        }
    }

    protected void Buscar1(object sender, EventArgs e)
    {
        try
        {
            BindGridView1();
        }
        catch (Exception ex)
        {

        }
    }

    protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = GridView1.Rows[index];

            if (e.CommandName == "Editar")
            {
                int Fila = row.RowIndex + 1;
                string Razon = HttpUtility.HtmlDecode(row.Cells[1].Text);
                string Correo = HttpUtility.HtmlDecode(row.Cells[2].Text);
                int EStatus = 0;
                DropDownList Status = row.Cells[5].FindControl("Stat") as DropDownList;
                if (Status.SelectedItem.ToString() == "Activo")
                {
                    EStatus = 0;
                }
                else if (Status.SelectedItem.ToString() == "Inactivo")
                {
                    EStatus = 1;
                }
                else if (Status.SelectedItem.ToString() == "Pendiente")
                {
                    EStatus = 2;
                }
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "Update('" + Razon + "','" + Correo + "','" + EStatus + "');", true);
            }


        }
        catch (Exception ex)
        {
            string Error = ex.Message;
            //RutinaError(ex);
            //LogError(pLogKey, pUserKey, "Carga-Factura:GridView1_RowCommand", ex.Message, pCompanyID);
        }
    }

    protected void GridView5_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = GridView5.Rows[index];

            if (e.CommandName == "Editar")
            {
                int Fila = row.RowIndex + 1;
                string Razon = HttpUtility.HtmlDecode(row.Cells[1].Text);
                string Correo = HttpUtility.HtmlDecode(row.Cells[2].Text);
                int EStatus = 0;
                DropDownList Status = row.Cells[5].FindControl("Stat1") as DropDownList;
                if (Status.SelectedItem.ToString() == "Activo")
                {
                    EStatus = 0;
                }
                else if (Status.SelectedItem.ToString() == "Inactivo")
                {
                    EStatus = 1;
                }
                else if (Status.SelectedItem.ToString() == "Pendiente")
                {
                    EStatus = 2;
                }
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "Update('" + Razon + "','" + Correo + "','" + EStatus + "');", true);
            }


        }
        catch (Exception ex)
        {
            string Error = ex.Message;
            //RutinaError(ex);
            //LogError(pLogKey, pUserKey, "Carga-Factura:GridView1_RowCommand", ex.Message, pCompanyID);
        }
    }

    private void BindGridView()
    {
        DatosV.Visible = false;
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            string SpFiltros;
            SpFiltros = Filtros();
            DataSet dsProveedores = new DataSet();
            SqlCommand cmd = new SqlCommand(SpFiltros, conn);

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            conn.Open();

            string Errores = string.Empty;
            SqlDataReader rdr = cmd.ExecuteReader();

            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            //dt.Columns.Add("Fecha");
            dt.Columns.Add("Creador");

            while (rdr.Read())
            {
                DataRow row = dt.NewRow();

                row["Nombre"] = Convert.ToString(rdr.GetValue(0));
                row["Correo"] = Convert.ToString(rdr.GetValue(1));
                //row["Fecha"] = Convert.ToString(rdr.GetValue(2));
                row["Creador"] = Convert.ToString(rdr.GetValue(3));
                dt.Rows.Add(row);

            }
        }

        GridView1.Columns[6].Visible = false;
        GridView1.DataSource = dt;
        GridView1.DataBind();



        if (dt.Rows.Count == 0) { DatosV.Visible = true; }
        else
        {
            int cont = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string SpFiltros;
                SpFiltros = Filtros();
                DataSet dsProveedores = new DataSet();
                SqlCommand cmd = new SqlCommand(SpFiltros, conn);

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DropDownList Status = GridView1.Rows[cont].Cells[4].FindControl("Stat") as DropDownList;
                    DropDownList Tipo = GridView1.Rows[cont].Cells[5].FindControl("Tipo") as DropDownList;
                    Status.SelectedIndex = Convert.ToInt16(rdr.GetValue(4)) - 1;
                    Tipo.SelectedValue = Convert.ToString(rdr.GetValue(5));
                    cont = cont + 1;
                }
            }
        }
    }

    private void BindGridView1()
    {
        DatosV1.Visible = false;
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            string SpFiltros;
            SpFiltros = Filtros1();
            DataSet dsProveedores = new DataSet();
            SqlCommand cmd = new SqlCommand(SpFiltros, conn);

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            conn.Open();

            string Errores = string.Empty;
            SqlDataReader rdr = cmd.ExecuteReader();

            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            dt.Columns.Add("Creador");

            while (rdr.Read())
            {
                DataRow row = dt.NewRow();

                row["Nombre"] = Convert.ToString(rdr.GetValue(0));
                row["Correo"] = Convert.ToString(rdr.GetValue(1));
                row["Creador"] = Convert.ToString(rdr.GetValue(3));
                dt.Rows.Add(row);

            }
        }

        GridView5.Columns[6].Visible = false;
        GridView5.DataSource = dt;
        GridView5.DataBind();



        if (dt.Rows.Count == 0) { DatosV1.Visible = true; }
        else
        {
            int cont = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string SpFiltros;
                SpFiltros = Filtros1();
                DataSet dsProveedores = new DataSet();
                SqlCommand cmd = new SqlCommand(SpFiltros, conn);

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DropDownList Status = GridView5.Rows[cont].Cells[4].FindControl("Stat1") as DropDownList;
                    //DropDownList Tipo = GridView5.Rows[cont].Cells[5].FindControl("Tipo1") as DropDownList;
                    Status.SelectedIndex = Convert.ToInt16(rdr.GetValue(4)) - 1;
                    //Tipo.SelectedValue = Convert.ToString(rdr.GetValue(5));
                    cont = cont + 1;
                }
            }
        }
    }

    private string Filtros1()
    {
        string Cadena = string.Empty;
        //Cadena = "Select a.UserName,a.UserID,CONVERT(VARCHAR(10),a.CreateDate, 103),b.Descripcion,d.RoleID,a.Status From Users a inner join StatusUsers b on a.Status = b.Status inner join UsersInRoles c on a.UserKey = c.UserKey inner join Roles d ON d.RoleKey =c.RoleKey Where b.Descripcion = '";
        //Cadena = "Select a.UserName,a.UserID,b.Descripcion,d.RoleID,a.Status,ISNULL(e.Tipo,'A') as Tipo From Users a inner join StatusUsers b on a.Status = b.Status inner join UsersInRoles c on a.UserKey = c.UserKey inner join Roles d ON d.RoleKey =c.RoleKey inner join Vendors e on a.UserKey = e.UserKey  Where e.VendorKey <> 0 AND b.Descripcion = '";
        
        Cadena = "Select a.UserName,a.UserID,b.Descripcion,d.RoleID,a.Status From Users a inner join StatusUsers b on a.Status = b.Status inner join UsersInRoles c on a.UserKey = c.UserKey inner join Roles d ON d.RoleKey =c.RoleKey  Where d.RoleID <>'Proveedor' AND b.Descripcion = '";
        Cadena = Cadena + Cclientes1.SelectedItem.ToString() + "' ";

        if (Email1.Text != "") { Cadena = Cadena + " AND a.UserID LIKE '%" + Email1.Text + "%' "; }
        if (Nombre1.Text != "") { Cadena = Cadena + " AND a.UserName LIKE '%" + Nombre1.Text + "%' "; }
        if (DRol1.SelectedItem.ToString() != "Seleccione Rol") { Cadena = Cadena + " AND d.RoleID LIKE '%" + DRol1.SelectedItem.ToString() + "%' "; }

        //if (Tipo.SelectedItem.ToString() == "Seleccione Tipo" || Tipo.SelectedItem.ToString() == "Con Adenda") { Cadena = Cadena; }
        //else { Cadena = Cadena + " AND e.Tipo LIKE '%" + Tipo.SelectedItem.ToString() + "%' "; }

        if (F11.Text != "") { Cadena = Cadena + " AND a.CreateDate >= '" + F11.Text + "' "; }
        if (F21.Text != "") { Cadena = Cadena + " AND a.CreateDate <= '" + F21.Text + " 23:59:59' "; }
        return Cadena;

    }

    private string Filtros()
    {
        string Cadena = string.Empty;
        //Cadena = "Select a.UserName,a.UserID,CONVERT(VARCHAR(10),a.CreateDate, 103),b.Descripcion,d.RoleID,a.Status From Users a inner join StatusUsers b on a.Status = b.Status inner join UsersInRoles c on a.UserKey = c.UserKey inner join Roles d ON d.RoleKey =c.RoleKey Where b.Descripcion = '";
        Cadena = "Select a.UserName,a.UserID,b.Descripcion,d.RoleID,a.Status,ISNULL(e.Tipo,'A') as Tipo From Users a inner join StatusUsers b on a.Status = b.Status inner join UsersInRoles c on a.UserKey = c.UserKey inner join Roles d ON d.RoleKey =c.RoleKey inner join Vendors e on a.UserKey = e.UserKey  Where e.VendorKey <> 0 AND b.Descripcion = '";
        //Cadena = "Select a.UserName,a.UserID,b.Descripcion,d.RoleID,a.Status From Users a inner join StatusUsers b on a.Status = b.Status inner join UsersInRoles c on a.UserKey = c.UserKey inner join Roles d ON d.RoleKey =c.RoleKey  Where b.Descripcion = '";
        Cadena = Cadena + Cclientes.SelectedItem.ToString() + "' ";

        if (Email.Text != "") { Cadena = Cadena + " AND a.UserID LIKE '%" + Email.Text + "%' "; }
        if (Nombre.Text != "") { Cadena = Cadena + " AND a.UserName LIKE '%" + Nombre.Text + "%' "; }
        if (DRol.SelectedItem.ToString() != "Seleccione Rol") { Cadena = Cadena + " AND d.RoleID LIKE '%" + DRol.SelectedItem.ToString() + "%' "; }

        if (Tipo.SelectedItem.ToString() == "Seleccione Tipo" || Tipo.SelectedItem.ToString() == "Con Adenda") { Cadena = Cadena; }
        else { Cadena = Cadena + " AND e.Tipo LIKE '%" + Tipo.SelectedItem.ToString() + "%' "; }
        //if (DropDownList1.SelectedItem.ToString() != "Seleccione Compañia") { Cadena = Cadena + " AND G.CompanyID LIKE '%" + DropDownList1.SelectedItem.ToString() + "%' "; }
        if (F1.Text != "") { Cadena = Cadena + " AND a.CreateDate >= '" + F1.Text + "' "; } if (F2.Text != "") { Cadena = Cadena + " AND a.CreateDate <= '" + F2.Text + " 23:59:59' "; }
        return Cadena;

    }

    protected void Clean1(object sender, EventArgs e)
{
    try
    {
        DRol1.SelectedValue = "1";
        //Tipo.SelectedValue = "X";
        Cclientes1.SelectedValue = "1";
        F11.Text = "";
        F21.Text = "";
        Email1.Text = "";
        Nombre1.Text = "";
        //EmpresS();
        BindGridView1();
    }
    catch (Exception ex)
    {

    }
}

    protected void Clean(object sender, EventArgs e)
    {
        try
        {
            DRol.SelectedValue = "1";
            Tipo.SelectedValue = "X";
            Cclientes.SelectedValue = "1";
            F1.Text = "";
            F2.Text = "";
            Email.Text = "";
            Nombre.Text = "";
            //EmpresS();
            BindGridView();
        }
        catch (Exception ex)
        {

        }
    }

    protected void Limp(object sender, EventArgs e)
    {
        try
        {
            EmailUp.Text = "";
            EmailUp.ReadOnly = false;
            Cam1.Visible = false;
            Nvoemail.Text = "";
            DataTable Vacio = new DataTable();
            GridView4.DataSource = Vacio;
            GridView4.DataBind();
            if (GridView3.Rows.Count>=1)
            {
                GridView2.DataSource = Vacio;
                GridView3.DataSource = Vacio;
                GridView2.DataBind();
                GridView3.DataBind();
                Button3.Visible = false;
            }
            else
            {

            }

        }
        catch (Exception ex)
        {

        }
    }

    protected void Roles(DropDownList Caja)
    {
        try
        {
            Caja.Items.Clear();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectRol", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Rol"].ToString() == "T|SYS| - Admin" || rdr["Rol"].ToString() == "T|SYS| - Validador" || rdr["Rol"].ToString() == "T|SYS| - Consultas")
                    {
                        ListItem Rol = new ListItem();
                        Rol.Value = (rdr["Rol"].ToString());
                        Caja.Items.Insert(0, Rol);
                    }
                }
                conn.Close();
            }
        }
        catch
        {

        }
    }

    private bool Email_Ok(string email)
    {
        string expresion;
        expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
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

    protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //GridView1.PageIndex = e.NewPageIndex;
        //BindGridView();
    }

    protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

        }
    }

    protected void GridView2_RowCreated(object sender, GridViewRowEventArgs e)
    {

    }

    protected void GridView2_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView1.EditIndex = e.NewEditIndex;
        try
        {
            int i, x, z,y;
            z = 0;
            y = 0;
            //i = e.NewEditIndex;
            int[] num = new int[800];
            int[] tip = new int[800];
            DataTable dt = new DataTable();

            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            //dt.Columns.Add("Fecha");
            dt.Columns.Add("Creador");

            x = GridView1.Rows.Count;

            if (x == 0)
            {
                GridView1.DataSource = "";
                DatosV.Visible = true;
            }
            else
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    DataRow dr = dt.NewRow();
                    DropDownList Status = gvr.Cells[4].FindControl("Stat") as DropDownList;
                    DropDownList Tipo = gvr.Cells[5].FindControl("Tipo") as DropDownList;
                    dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    //dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    dr["Creador"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    num[z] = Status.SelectedIndex + 1;
                    tip[y] = Tipo.SelectedIndex + 1;
                    z = z + 1;
                    y = y + 1;
                    dt.Rows.Add(dr);
                }
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();

            z = 0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                DropDownList Status = gvr.Cells[4].FindControl("Stat") as DropDownList;
                Status.SelectedIndex = num[z] - 1;
                z = z + 1;
            }

            y = 0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                DropDownList Tipo = gvr.Cells[5].FindControl("Tipo") as DropDownList;
                Tipo.SelectedIndex = tip[y] - 1;
                y = y + 1;
            }


        }
        catch (Exception v)
        {

        }
    }

    protected void GridView2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        try
        {
            int i, x, z, y;
            z = 0;
            y = 0;
            //i = e.NewEditIndex;
            int[] num = new int[800];
            int[] tip = new int[800];
            i = e.RowIndex;
            GridView1.EditIndex = -1;
            DataTable dt = new DataTable();

            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            //dt.Columns.Add("Fecha");
            dt.Columns.Add("Creador");

            x = GridView1.Rows.Count;

            if (x == 0)
            {
                GridView1.DataSource = "";
                DatosV.Visible = true;
            }
            else
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    DataRow dr = dt.NewRow();
                    DropDownList Status = gvr.Cells[4].FindControl("Stat") as DropDownList;
                    DropDownList Tipo = gvr.Cells[5].FindControl("Tipo") as DropDownList;
                    dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    //dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    dr["Creador"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    num[z] = Status.SelectedIndex + 1;
                    tip[y] = Tipo.SelectedIndex + 1;
                    z = z + 1;
                    y = y + 1;
                    dt.Rows.Add(dr);
                }
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();

            z = 0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                DropDownList Status = gvr.Cells[4].FindControl("Stat") as DropDownList;
                Status.SelectedIndex = num[z] - 1;
                z = z + 1;
            }

            y = 0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                DropDownList Tipo = gvr.Cells[5].FindControl("Tipo") as DropDownList;
                Tipo.SelectedIndex = tip[y] - 1;
                y = y + 1;
            }

        }
        catch (Exception v)
        {

        }

    }

    protected void GridView2_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int i, x, z,y;
            z = 0;
            y = 0;
            i = e.RowIndex;
            GridView1.EditIndex = -1;
            int[] num = new int[800];
            int[] tip = new int[800];
            DataTable dt = new DataTable();

            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            //dt.Columns.Add("Fecha");
            dt.Columns.Add("Creador");

            string strName = ((TextBox)GridView1.Rows[e.RowIndex].Cells[5].Controls[0]).Text;
            string EmailUs = HttpUtility.HtmlDecode(GridView1.Rows[e.RowIndex].Cells[2].Text.ToString());

            x = GridView1.Rows.Count;

            if (x == 0)
            {
                GridView1.DataSource = "";
                DatosV.Visible = true;
            }
            else
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    DataRow dr = dt.NewRow();
                    DropDownList Status = gvr.Cells[4].FindControl("Stat") as DropDownList;
                    DropDownList Tipo = gvr.Cells[5].FindControl("Stat") as DropDownList;
                    dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    //dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    dr["Creador"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    num[z] = Status.SelectedIndex + 1;
                    tip[y] = Tipo.SelectedIndex + 1;
                    z = z + 1;
                    y = y + 1;
                    dt.Rows.Add(dr);
                }
            }



            if (Email_Ok(strName) == true)
            {
                if (EmailT(strName, EmailUs) == true)
                {
                    Label4.Text = "Actualizacion de password exitosa,Se ha enviado un correo de restablecimiento de contraseña a " + strName;
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);
                }
                else
                {
                    Labe5.Text = "Se Generaron errores al cargar la configuracion para el restablecimiento de contraseña, Verifica la conexion SQL";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
                }
            }
            else
            {
                Labe5.Text = "Formato de correo Invalido, Revisalo";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
            }


            GridView1.DataSource = dt;
            GridView1.DataBind();

            z = 0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                DropDownList Status = gvr.Cells[4].FindControl("Stat") as DropDownList;
                Status.SelectedIndex = num[z] - 1;
                z = z + 1;
            }

            y = 0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                DropDownList Tipo = gvr.Cells[5].FindControl("Tipo") as DropDownList;
                Tipo.SelectedIndex = num[y] - 1;
                y = y + 1;
            }
        }
        catch (Exception v)
        {
            string err = v.Message;
        }
    }


    protected void GridView5_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView5.EditIndex = e.NewEditIndex;
        try
        {
            int i, x, z, y;
            z = 0;
            y = 0;
            //i = e.NewEditIndex;
            int[] num = new int[800];
            int[] tip = new int[800];
            DataTable dt = new DataTable();

            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            dt.Columns.Add("Creador");

            x = GridView5.Rows.Count;

            if (x == 0)
            {
                GridView5.DataSource = "";
                DatosV1.Visible = true;
            }
            else
            {
                foreach (GridViewRow gvr in GridView5.Rows)
                {
                    DataRow dr = dt.NewRow();
                    DropDownList Status = gvr.Cells[4].FindControl("Stat") as DropDownList;
                    //DropDownList Tipo = gvr.Cells[5].FindControl("Tipo") as DropDownList;
                    dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    //dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    dr["Creador"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    num[z] = Status.SelectedIndex + 1;
                    //tip[y] = Tipo.SelectedIndex + 1;
                    z = z + 1;
                    //y = y + 1;
                    dt.Rows.Add(dr);
                }
            }

            GridView5.DataSource = dt;
            GridView5.DataBind();

            z = 0;
            foreach (GridViewRow gvr in GridView5.Rows)
            {
                DropDownList Status = gvr.Cells[4].FindControl("Stat1") as DropDownList;
                Status.SelectedIndex = num[z] - 1;
                z = z + 1;
            }

            //y = 0;
            //foreach (GridViewRow gvr in GridView1.Rows)
            //{
            //    DropDownList Tipo = gvr.Cells[5].FindControl("Tipo") as DropDownList;
            //    Tipo.SelectedIndex = tip[y] - 1;
            //    y = y + 1;
            //}


        }
        catch (Exception v)
        {

        }
    }

    protected void GridView5_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        try
        {
            int i, x, z, y;
            z = 0;
            y = 0;
            //i = e.NewEditIndex;
            int[] num = new int[800];
            int[] tip = new int[800];
            i = e.RowIndex;
            GridView5.EditIndex = -1;
            DataTable dt = new DataTable();

            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            dt.Columns.Add("Creador");

            x = GridView5.Rows.Count;

            if (x == 0)
            {
                GridView5.DataSource = "";
                DatosV1.Visible = true;
            }
            else
            {
                foreach (GridViewRow gvr in GridView5.Rows)
                {
                    DataRow dr = dt.NewRow();
                    DropDownList Status = gvr.Cells[4].FindControl("Stat") as DropDownList;
                    //DropDownList Tipo = gvr.Cells[5].FindControl("Tipo") as DropDownList;
                    dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    //dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    dr["Creador"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    num[z] = Status.SelectedIndex + 1;
                    //tip[y] = Tipo.SelectedIndex + 1;
                    z = z + 1;
                    //y = y + 1;
                    dt.Rows.Add(dr);
                }
            }

            GridView5.DataSource = dt;
            GridView5.DataBind();

            z = 0;
            foreach (GridViewRow gvr in GridView5.Rows)
            {
                DropDownList Status = gvr.Cells[4].FindControl("Stat1") as DropDownList;
                Status.SelectedIndex = num[z] - 1;
                z = z + 1;
            }

            //y = 0;
            //foreach (GridViewRow gvr in GridView1.Rows)
            //{
            //    DropDownList Tipo = gvr.Cells[5].FindControl("Tipo") as DropDownList;
            //    Tipo.SelectedIndex = tip[y] - 1;
            //    y = y + 1;
            //}

        }
        catch (Exception v)
        {

        }

    }

    protected void GridView5_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int i, x, z, y;
            z = 0;
            y = 0;
            i = e.RowIndex;
            GridView5.EditIndex = -1;
            int[] num = new int[800];
            int[] tip = new int[800];
            DataTable dt = new DataTable();

            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            //dt.Columns.Add("Fecha");
            dt.Columns.Add("Creador");

            string strName = ((TextBox)GridView5.Rows[e.RowIndex].Cells[5].Controls[0]).Text;
            string EmailUs = HttpUtility.HtmlDecode(GridView5.Rows[e.RowIndex].Cells[2].Text.ToString());

            x = GridView5.Rows.Count;

            if (x == 0)
            {
                GridView5.DataSource = "";
                DatosV1.Visible = true;
            }
            else
            {
                foreach (GridViewRow gvr in GridView5.Rows)
                {
                    DataRow dr = dt.NewRow();
                    DropDownList Status = gvr.Cells[4].FindControl("Stat") as DropDownList;
                    //DropDownList Tipo = gvr.Cells[5].FindControl("Stat") as DropDownList;
                    dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    //dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    dr["Creador"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    num[z] = Status.SelectedIndex + 1;
                    //tip[y] = Tipo.SelectedIndex + 1;
                    z = z + 1;
                    //y = y + 1;
                    dt.Rows.Add(dr);
                }
            }



            if (Email_Ok(strName) == true)
            {
                if (EmailT(strName, EmailUs) == true)
                {
                    Label2.Text = "Actualizacion de password exitosa,Se ha enviado un correo de restablecimiento de contraseña a " + strName;
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL11);", true);
                }
                else
                {
                    Label3.Text = "Se Generaron errores al cargar la configuracion para el restablecimiento de contraseña, Verifica la conexion SQL";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL111);", true);
                }
            }
            else
            {
                Label3.Text = "Formato de correo Invalido, Revisalo";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL111);", true);
            }


            GridView5.DataSource = dt;
            GridView5.DataBind();

            z = 0;
            foreach (GridViewRow gvr in GridView5.Rows)
            {
                DropDownList Status = gvr.Cells[4].FindControl("Stat1") as DropDownList;
                Status.SelectedIndex = num[z] - 1;
                z = z + 1;
            }

            //y = 0;
            //foreach (GridViewRow gvr in GridView1.Rows)
            //{
            //    DropDownList Tipo = gvr.Cells[5].FindControl("Tipo") as DropDownList;
            //    Tipo.SelectedIndex = num[y] - 1;
            //    y = y + 1;
            //}
        }
        catch (Exception v)
        {
            string err = v.Message;
        }
    }

    protected void Cclientes_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindGridView();
    }

    protected void Cclientes_SelectedIndexChanged1(object sender, EventArgs e)
    {
        BindGridView1();
    }

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
            string Error = ex.Message;
            Resultado = false;
        }
        return Resultado;
    }

    private bool EmailT(string CorreoUs, string UserEmail)
    {
        bool Resut = false;
        try
        {
            string PassNew = Membership.GeneratePassword(8, 1);
            string PassHAs = PassNew.ToString();
            ApplicationDbContext context = new ApplicationDbContext();
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
            string hashedNewPassword = UserManager.PasswordHasher.HashPassword(PassHAs);
            bool UpdatePass = UpdatePAss(UserEmail, hashedNewPassword);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ResetPass.html")))
            {
                body = reader.ReadToEnd();
                body = body.Replace("{PassTemp}", PassHAs);

            }

            Resut = Global.EmailGlobal(CorreoUs, body, "RECUPERACIÓN DE CONTRASEÑA");
        }
        catch (Exception b)
        {
            Resut = false;
        }
        return Resut;
    }

    protected void Save1(object sender, EventArgs e)
    {
        try
        {
            if (GridView5.Rows.Count >= 1)
            {
                foreach (GridViewRow gvr in GridView5.Rows)
                {
                    string Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    int Ukey = Convert.ToInt16(HttpContext.Current.Session["UserKey"].ToString());
                    DropDownList Status = gvr.Cells[4].FindControl("Stat1") as DropDownList;
                    //DropDownList Tipo = gvr.Cells[5].FindControl("Tipo") as DropDownList;

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand("spUpdateUserPass", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = Email });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = Ukey });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Valor", Value = Status.SelectedItem.ToString() });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tipo", Value = "" });

                        if (conn.State == ConnectionState.Open) { conn.Close(); }

                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        conn.Close();
                    }
                }

                Label2.Text = "Actualizacion Exitosa ";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL11);", true);
                BindGridView1();
            }
            else
            {

            }
        }
        catch (Exception ex)
        {
            Label3.Text = "Se generaron errores al intentar actualizar al usuario, verifica la conexion y datos SQL ";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL111);", true);
        }
    }

    protected void Save(object sender, EventArgs e)
    {
        try
        {
            if (GridView1.Rows.Count >= 1)
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    string Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    int Ukey = Convert.ToInt16(HttpContext.Current.Session["UserKey"].ToString());
                    DropDownList Status = gvr.Cells[4].FindControl("Stat") as DropDownList;
                    DropDownList Tipo = gvr.Cells[5].FindControl("Tipo") as DropDownList;

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand("spUpdateUserPass", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = Email });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = Ukey });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Valor", Value = Status.SelectedItem.ToString() });
                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tipo", Value = Tipo.SelectedItem.ToString() });

                        if (conn.State == ConnectionState.Open) { conn.Close(); }

                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        conn.Close();
                    }
                }

                Label4.Text = "Actualizacion Exitosa ";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);
                //EmpresS();
                BindGridView();
            }
            else
            {

            }
        }
        catch (Exception ex)
        {
            Labe5.Text = "Se generaron errores al intentar actualizar al usuario, verifica la conexion y datos SQL ";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
        }
    }

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindGridView();
    }

    protected void Llenado_Lista(string Email,int Opcion)
    {
        try
        {
            string SQL;
            string Fila = string.Empty;
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            if (Opcion == 1)
            {
                SQL = "Select b.VendName as Nombre,c.CompanyName As Compania From AspNetUsers a inner join Vendors b on a.UserKey = b.UserKey inner join Company c On b.CompanyID = c.CompanyID  Where a.UserName = '" + Email + "'";
            }
            else
            {
                SQL = "Select d.UserName as Nombre,c.CompanyName As Compania From AspNetUsers a Inner join UsersInCompany b on a.UserKey = b.UserKey inner join Users d on a.UserKey = d.UserKey inner join Company c on c.CompanyID = b.CompanyID Where a.UserName = '" + Email + "'";
            }
            //SQL = "Select b.VendName as Nombre,c.CompanyName As Compania From AspNetUsers a inner join Vendors b on a.UserKey = b.UserKey inner join Company c On b.CompanyID = c.CompanyID  Where a.UserName = '" + Email + "'";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@VendKey", Email));
            using (SqlCommand Cmd = new SqlCommand(SQL, sqlConnection1))
            {

                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = SQL;
                foreach (SqlParameter par in parsT)
                {Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);}
                SqlDataReader rdr = null;
                rdr = Cmd.ExecuteReader();
                GridView4.DataSource = rdr;
                GridView4.DataBind();                
                sqlConnection1.Close();
            }
        }
        catch (Exception ex)
        {

        }
    }

    protected string Obtener_Nombre(string Email,int Opcion)
    {
        string Valor = string.Empty;
        try
        {
            string SQL;
            string Fila = string.Empty;
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spGetVendorsPortal", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = EmailUp.Text.Trim()
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable Registros = new DataTable();
                Registros.Load(rdr);
                conn.Close();
                int Filas = Registros.Rows.Count;

                if (Filas == 0)
                {
                    SQL = "Select Top 1 d.UserName as Nombre,c.CompanyName As Compania From AspNetUsers a Inner join UsersInCompany b on a.UserKey = b.UserKey inner join Users d on a.UserKey = d.UserKey inner join Company c on c.CompanyID = b.CompanyID Where a.UserName = '" + Email + "'";
                }
                else
                {
                    SQL = "Select b.VendName as Nombre From AspNetUsers a inner join Vendors b on a.UserKey = b.UserKey inner join Company c On b.CompanyID = c.CompanyID  Where a.UserName = '" + Email + "'";
                }
                

            }

            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@VendKey", Email));
            using (SqlCommand Cmd = new SqlCommand(SQL, sqlConnection1))
            {

                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = SQL;
                foreach (SqlParameter par in parsT)
                { Cmd.Parameters.AddWithValue(par.ParameterName, par.Value); }
                SqlDataReader rdr = null;
                rdr = Cmd.ExecuteReader();
                while (rdr.Read())
                {
                   Valor = Valor + " / " + rdr["Nombre"].ToString();
                }
                int lg = Valor.Length-3;
                Valor = Valor.Substring(3, lg);
            }
        }
        catch (Exception ex)
        {

        }
        return Valor;
    }

    protected void Btn_Buscar(object sender, EventArgs e)
    {
        try
        {
            Nvoemail.Text = "";
            DataTable Vacio = new DataTable();
            GridView4.DataSource = Vacio;
            GridView4.DataBind();

            if (EmailUp.Text != "")
            {
                if (Email_Ok(EmailUp.Text.Trim()) == true)
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand("spGetVendorsPortal", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@Email",
                            Value = EmailUp.Text.Trim()
                        });

                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }

                        conn.Open();

                        SqlDataReader rdr = cmd.ExecuteReader();
                        DataTable Registros = new DataTable();
                        Registros.Load(rdr);

                        int Filas = Registros.Rows.Count;

                        if (Filas == 0)
                        {
                            if (BuscarNvo(EmailUp.Text.Trim()) == true)
                            {
                                conn.Close();
                                Cam1.Visible = false;
                                string Caja = "AID2";
                                Lsage.Text = "Email No Encontrado dentro de los Registros del Portal, Verificalo";
                                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
                            }
                            else
                            {
                                EmailUp.ReadOnly = true;
                                Llenado_Lista(EmailUp.Text.Trim(),2);
                                Cam1.Visible = true;
                                if (GridView4.Rows.Count == 0)
                                {
                                    Cam1.Visible = false;
                                    string Caja = "AID2";
                                    Lsage.Text = "Se Genero Un Problema al intentar Obtener el Nombre";
                                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
                                    EmailUp.ReadOnly = false;
                                }
                            }

                        }
                        else
                        {
                            Llenado_Lista(EmailUp.Text.Trim(),1);
                            EmailUp.ReadOnly = true;
                            Cam1.Visible = true;
                        }
                        conn.Close();
                    }



                }
                else
                {
                    string Caja = "AID2";
                    Lsage.Text = "Formato de Email Invalido";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
                }
            }
            else
            {
                string Caja = "AID2";
                Lsage.Text = "Ingresa un Email";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
            }
        }
        catch (Exception ex)
        {
        }
    }

    protected bool VolvBus()
    {
        bool Res = true;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spGetVendorsPortal", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = EmailUp.Text.Trim()
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable Registros = new DataTable();
                Registros.Load(rdr);

                int Filas = Registros.Rows.Count;

                if (Filas == 0)
                {
                    if (BuscarNvo(EmailUp.Text.Trim()) == true)
                    {
                        Res = false;
                    }
                }
                conn.Close();
            }
        }
        catch (Exception ex)
        {

        }
        return Res;
    }

    protected void Campos(object sender, EventArgs e)
    {
        try
        {
            
            if (EmailUp.Text == "")
            {
                Lsage.Text = "Ingresa un Email";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AID2);", true);
                return;
            }

            if (Email_Ok(EmailUp.Text.Trim()) == false)
            {
                Lsage.Text = "Formato de Email invalido";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AID2);", true);
                return;
            }

            if (Nvoemail.Text == "")
            {
                Lsage.Text = "Ingresa el nuevo Email de Proveedor";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AID2);", true);
                return;
            }

            if (Email_Ok(Nvoemail.Text.Trim()) == false)
            {
                Lsage.Text = "Formato del nuevo Email invalido";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AID2);", true);
                return;
            }

            if (BuscarNvo(Nvoemail.Text.Trim()) == false)
            {
                Lsage.Text = "El Nuevo email para proveedor ya pertenece a un usuario del Portal, Verificalo";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AID2);", true);
                return;
            }

            if (DentroTabla(EmailUp.Text.Trim()) == true)
            {
                Lsage.Text = "El Email ingresado ya pertenece a la Tabla de Resgistros por Actualizar";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AID2);", true);
                return;
            }

            if (DentroTabla(Nvoemail.Text.Trim()) == true)
            {
                Lsage.Text = "El nuevo Email ingresado ya pertenece a la Tabla de Resgistros por Actualizar";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AID2);", true);
                return;
            }

            if (VolvBus() == false)
            {   
                Lsage.Text = "Email No Encontrado dentro de los Registros del Portal, Verificalo";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AID2);", true);
                return;
            }

            LlenadoUpdate();

        }
        catch (Exception ex)
        {

        }
    }

    protected bool BuscarNvo(string Nuevo)
    {
        bool Resultado = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spValUserPortalT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = Nuevo });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Company", Value = "" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 2 });
                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Resultado"].ToString() == "0")
                    {
                        Resultado = true;
                    }
                }
                conn.Close();
            }
        }
        catch (Exception ex)
        {
            Resultado = false;
        }
        return Resultado;
    }

    protected bool DentroTabla(string Email)
    {
        bool Resut = false;
        try
        {
            if (GridView2.Rows.Count >= 1)
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                    string Valor = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    if (Valor == Email)
                    {
                        Resut = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Resut = false;
        }

        return Resut;

    }

    protected void LlenadoUpdate()
    {
        DataTable dt = new DataTable();

        dt.Columns.Add("Razon Social");
        dt.Columns.Add("Email");
        dt.Columns.Add("NvoEmail");

        if (GridView2.Rows.Count >= 1)
        {
            foreach (GridViewRow gvr in GridView2.Rows)
            {
                DataRow dr = dt.NewRow();
                dr["Razon Social"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                dr["Email"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                dr["NvoEmail"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                dt.Rows.Add(dr);
            }

        }

        DataRow row = dt.NewRow();
        string Nombre = Obtener_Nombre(EmailUp.Text.Trim(),2);
        row["Razon Social"] = Nombre;
        row["Email"] = EmailUp.Text.Trim();
        row["NvoEmail"] = Nvoemail.Text.Trim();

        dt.Rows.Add(row);
        GridView2.DataSource = dt;
        GridView2.DataBind();

        if (GridView2.Rows.Count == 0)
        {
            Button3.Visible = false;
        }
        else
        {
            Button3.Visible = true;
        }


        EmailUp.Text = "";
        DataTable Vacio = new DataTable();
        GridView4.DataSource = Vacio;
        GridView4.DataBind();
        Nvoemail.Text = "";
        EmailUp.ReadOnly = false;
        Cam1.Visible = false;
        EmailUp.Focus();

    }

    protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        try
        {
            int i, x;
            i = e.RowIndex;
            DataTable dt = new DataTable();

            dt.Columns.Add("Razon Social");
            dt.Columns.Add("Email");
            dt.Columns.Add("NvoEmail");

            x = GridView2.Rows.Count;

            if (x == 1)
            {
                GridView2.DataSource = "";
            }
            else
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Razon Social"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["Email"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["NvoEmail"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dt.Rows.Add(dr);

                    }
                }
            }

            GridView2.DataSource = dt;
            GridView2.DataBind();

            if (GridView2.Rows.Count == 0)
            {
                Button3.Visible = false;
            }
            else
            {
                Button3.Visible = true;
            }

        }
        catch (Exception v)
        {

        }
    }

    protected string DatosTsys(string UUID)
    {
        string Cuenta;
        Cuenta = string.Empty;
        try
        {
            string sql;


            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            sql = @"SELECT UserName As 'Cuenta' FROM Users WHERE UserID ='" + UUID + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

        }
        catch (Exception ex)
        {
            //LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            Cuenta = string.Empty;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
        }
        return Cuenta;
    }

    protected void Guardar(object sender, EventArgs e)
    {
        try
        {
            DataTable Vacio = new DataTable();
            GridView3.DataSource = Vacio;
            GridView3.DataBind();

            if (GridView2.Rows.Count >= 1)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Email");
                dt.Columns.Add("Nuevo");
                dt.Columns.Add("Error");
                int Contador = 0;
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                    string Correo = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    string Nuevo = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    string Resultado = actualiza(Correo, Nuevo);
                    if (Resultado!= "")
                    {
                        DataRow dr = dt.NewRow();
                        dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["Email"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["Nuevo"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dr["Error"] = Resultado;
                        dt.Rows.Add(dr);
                        Contador = Contador + 1;
                    }
                }

                if (Contador >= 1)
                {
                    GridView2.DataSource = Vacio;
                    GridView2.DataBind();
                    GridView3.DataSource = dt;
                    GridView3.DataBind();
                    Button3.Visible = false;
                }
                else
                {
                    GridView3.DataSource = Vacio;
                    GridView2.DataSource = Vacio;
                    GridView2.DataBind();
                    GridView3.DataBind();
                    Button3.Visible = false;
                    Label1.Text = "Actualización Exitosa";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AID3);", true);
                }

            }
            else
            {
                Button3.Visible = false;
                Lsage.Text = "No Existen Datos para Actualizar";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AID2);", true);
                return;
            }
        }
        catch (Exception ex)
        {

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
        }
    }

    protected string actualiza(string Original,string Nuevo)
    {
        string Valor = string.Empty;
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            int UserK = Convert.ToInt16(HttpContext.Current.Session["UserKey"]);
            string sSQL;

            sSQL = "spUpdateEmail";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@Email_Orig", Original));
            parsT.Add(new SqlParameter("@Email_Nvo", Nuevo));
            parsT.Add(new SqlParameter("@UserKey", UserK));

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
                    if (rdr["Resultado"].ToString() != "Ok")
                    {
                        Valor = rdr["Resultado"].ToString();
                    }
                    //string Resultado = rdr["Resultado"].ToString(); // 0 ok
                }

                sqlConnection1.Close();

            }
        }
        catch (Exception ex)
        {

        }
        return Valor;
    }

}