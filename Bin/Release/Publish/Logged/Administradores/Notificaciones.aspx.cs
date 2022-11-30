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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebSite1;

public partial class Logged_Administradores_Notificaciones : System.Web.UI.Page
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

                    if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
                    {
                        Consulta();
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
            string titulo, Msj, tipo, Error;
            tipo = DRol.SelectedItem.ToString();
            Msj = Nombre.Text;
            titulo = Email.Text;
            Error = String.Empty;

            if (Msj == "") { Error = "Ingrese el mensaje para la notificación"; }
            if (titulo == "") { Error = "Ingrese un titulo para la notificación"; }

            if (Error != "") 
            {
                Label4.Text = Error.ToString();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);
            }
            else
            {
                string Texr= string.Empty;
                Texr = "swal.mixin({confirmButtonText: 'Ok',showCancelButton: false,customClass: 'swal-wide'}).queue([ ";

                if (Imagen.Text != "")
                {
                    Texr = "swal.mixin({confirmButtonText: 'Ok',showCancelButton: false,customClass: 'swal-widei'}).queue([ ";
                    Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "',imageUrl: '" + Imagen.Text + "', imageWidth: 1200, imageHeight: 600, imageAlt: 'Custom image', },";
                }
                else 
                {
                    Texr = "swal.mixin({confirmButtonText: 'Ok',showCancelButton: false,customClass: 'swal-wide'}).queue([ ";
                    Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                }              
                Texr = Texr + " ])";

                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "Varis(" + Texr + ");", true);
            }
            
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
            Cclientes.SelectedValue = "1";
            F1.Text = "";
            F2.Text = "";
            Email.Text = "";
            Nombre.Text = "";
            Imagen.Text = "";
        }
        catch (Exception ex)
        {

        }
    }

    protected void Save(object sender, EventArgs e)
    {
        try
        {
            string titulo, Msj, tipo, Error,Fecha1,Fecha2;
            tipo = DRol.SelectedItem.ToString();
            Msj = Nombre.Text;
            titulo = Email.Text;
            Fecha1 = F1.Text;
            Fecha2 = F2.Text;
            Error = String.Empty;        
            
            if (Msj == "") { Error = "Ingrese el mensaje para la notificación"; }
            if (Fecha2 == "") { Error = "Ingrese Fecha de finalización para mostrar la notificación"; }
            if (Fecha1 == "") { Error = "Ingrese Fecha de inicio para mostrar la notificación"; }            
            if (titulo == "") { Error = "Ingrese un titulo para la notificación"; }
            if (Fecha1 != "" && Fecha2 != "")
            {
                DateTime Fe1 = Convert.ToDateTime(Fecha1.ToString());
                DateTime Fe2 = Convert.ToDateTime(Fecha2.ToString());
                if (Fe2 < Fe1) { Error = "La Fecha de finalización del mensaje no puede ser menor a la fecha de inicio"; }
            }

            if (Error != "")
            {
                Label4.Text = Error.ToString();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);
            }
            else
            {
                string Usuario;
                Usuario = User.Identity.Name.ToString();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "Pregunta('" + Usuario + "');", true);
            }
        }
        catch (Exception ex)
        {

        }
    }

    protected void Guardar(object sender, EventArgs e)
    {
        try
        {
            BindGridView();
        }
        catch (Exception ex)
        {
        
        }
    }

    protected void Desactivar(object sender, EventArgs e)
    {
        try
        {
            string Mensaje = string.Empty;
            DateTime Inicio; DateTime Hasta; DateTime Inicio1; DateTime Hasta1;

            if (Radio2.Checked == true) { if (TextBox2.Text == "") Mensaje = "Defina fecha de finalización para la suspención de actividades de Facturas"; }
            if (Radio2.Checked == true) { if (TextBox1.Text == "") Mensaje = "Defina fecha de inicio para la suspención de actividades de Facturas"; }

            if (RadioButton2.Checked == true) { if (TextBox5.Text == "") Mensaje = "Defina fecha de finalización para la suspención de actividades de Complementos de Pago"; }
            if (RadioButton2.Checked == true) { if (TextBox4.Text == "") Mensaje = "Defina fecha de inicio para la suspención de actividades de Complementos de Pago"; }

            if (TextBox2.Text != "" && TextBox1.Text != "") { if (Radio2.Checked == true) { Inicio = Convert.ToDateTime(TextBox1.Text.ToString()); Hasta = Convert.ToDateTime(TextBox2.Text.ToString()); if (Inicio > Hasta) { Mensaje = "Fecha de inicio no puede ser mayor que la fecha de finalización - Suspención de actividades de Facturas"; } } }
            if (TextBox4.Text != "" && TextBox5.Text != "") { if (RadioButton2.Checked == true) { Inicio1 = Convert.ToDateTime(TextBox4.Text.ToString()); Hasta1 = Convert.ToDateTime(TextBox5.Text.ToString()); if (Inicio1 > Hasta1) { Mensaje = "Fecha de inicio no puede ser mayor que la fecha de finalización - Suspención de actividades de Complemento de Pago"; } } }

            if (Radio2.Checked == false && Radio1.Checked == false) { Mensaje = "Seleccione una Opción para Carga de Facturas"; }
            if (RadioButton1.Checked == false && RadioButton2.Checked == false) { Mensaje = "Seleccione una Opción para Carga de Complementos de Pago"; }
            if (Mensaje != "")
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertame('Alertas TSYS','" + Mensaje + "','error');", true);
            }
            else 
            {          
                string Usuario;
                Usuario = User.Identity.Name.ToString();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "Pregunta0('" + Usuario + "');", true);
            }
        }
        catch (Exception ex)
        {

        }
    }

    protected void Consulta()
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSuspender", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                int Opcion = 2;

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Userkey", Value = 1 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@In1", Value = 1 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@In2", Value = 1 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Desde", Value = "" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Hasta", Value = "" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@In3", Value = 1 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@In4", Value = 1 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Desde1", Value = "" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Hasta1", Value = "" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = Opcion });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["FacIN"].ToString() == "1"){ Radio1.Checked = false; Radio2.Checked = true; f5.Visible = true; f6.Visible = true; TextBox1.Text =  string.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(rdr["Desde"].ToString())); TextBox2.Text = string.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(rdr["Hasta"].ToString()));} 
                    else { Radio1.Checked = true; Radio2.Checked = false; f5.Visible = false; f6.Visible = false; TextBox1.Text = ""; TextBox2.Text = ""; }

                    if (rdr["PagoIN"].ToString() == "1") { RadioButton1.Checked = false; RadioButton2.Checked = true; f3.Visible = true; f4.Visible = true; TextBox4.Text = string.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(rdr["Desde1"].ToString())); TextBox5.Text = string.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(rdr["Hasta1"].ToString())); }
                    else { RadioButton1.Checked = true; RadioButton2.Checked = false; f3.Visible = false; f4.Visible = false; TextBox4.Text = ""; TextBox5.Text = ""; }
                }
                conn.Close();
            }
        }
        catch (Exception ex)
        {

        }
    }


    protected void Group1_CheckedChanged(Object sender, EventArgs e)
    {
        if (RadioButton2.Checked)
        {
            f3.Visible = true;
            f4.Visible = true;
        }

        if (RadioButton1.Checked)
        {
            f3.Visible = false;
            f4.Visible = false;
        }

        if (Radio2.Checked)
        {
            f5.Visible = true;
            f6.Visible = true;
        }

        if (Radio1.Checked)
        {
            f5.Visible = false;
            f6.Visible = false;
        }
    }

    private void BindGridView()
    {
        try 
        {
        DatosV1.Visible = false;
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = new SqlCommand("spNotificaciones", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Titulo", Value = "11" });
            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Desde", Value = "01/01/2020" });
            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Hasta", Value = "01/01/2020" });
            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Estilo", Value = "11" });
            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Estatus", Value = "1" });
            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Mensaje", Value = "1" });
            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Userkey", Value = "1" });
            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Url", Value = "1" });
            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 2 });

            if (conn.State == ConnectionState.Open){conn.Close();}conn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            dt.Columns.Add("Numero");
            dt.Columns.Add("Titulo");
            dt.Columns.Add("Tipo");
            dt.Columns.Add("Desde");
            dt.Columns.Add("Hasta");

            int cont = 0;
            
            while (rdr.Read())
            {
                DataRow row = dt.NewRow();
                row["Numero"] = Convert.ToString(rdr.GetValue(0));
                row["Titulo"] = Convert.ToString(rdr.GetValue(1));
                row["Tipo"] = Convert.ToString(rdr.GetValue(2));
                row["Desde"] = Convert.ToString(Convert.ToDateTime(rdr.GetValue(3)).ToString("dd/MM/yyyy"));
                row["Hasta"] = Convert.ToString(Convert.ToDateTime(rdr.GetValue(4)).ToString("dd/MM/yyyy"));             
                dt.Rows.Add(row);
            }
        }

        GridView2.DataSource = dt;
        GridView2.DataBind();

        if (GridView2.Rows.Count == 0) { DatosV1.Visible = true; }
            else
            {
                int cont = 0;
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spNotificaciones", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Titulo", Value = "11" });
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Desde", Value = "01/01/2020" });
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Hasta", Value = "01/01/2020" });
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Estilo", Value = "11" });
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Estatus", Value = "1" });
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Mensaje", Value = "1" });
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Userkey", Value = "1" });
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Url", Value = "1" });
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 2 });

                    if (conn.State == ConnectionState.Open) { conn.Close(); } conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        DropDownList Status = GridView2.Rows[cont].Cells[5].FindControl("Stat") as DropDownList;
                        Status.SelectedIndex = Convert.ToInt16(rdr.GetValue(5))-1;
                        cont = cont + 1;
                    }
                }
            }
        }
        catch (Exception ex)
        {
        
        }
    }

    protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {

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
        GridView2.EditIndex = e.NewEditIndex;
        try
        {
            int i, x, z;
            z = 0;
            i = e.NewEditIndex;
            BindGridView();
        }
        catch (Exception v)
        {

        }
    }

    protected void GridView2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        try
        {
            BindGridView();
            int i, x, z;
            z = 0;
            i = e.RowIndex;
            GridView2.EditIndex = -1;
            BindGridView();
        }
        catch (Exception v)
        {

        }

    }

    protected void GridView2_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int i, x, z;
            z = 0;
            i = e.RowIndex;
            GridView2.EditIndex = -1;

            DropDownList Status = GridView2.Rows[e.RowIndex].Cells[5].FindControl("Stat") as DropDownList;
            string Estilo = Convert.ToString(Convert.ToInt32(Status.SelectedIndex.ToString())+1);
            string Numero = HttpUtility.HtmlDecode(GridView2.Rows[e.RowIndex].Cells[0].Text.ToString());

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spNotificaciones", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Titulo", Value = "11" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Desde", Value = "01/01/2020" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Hasta", Value = "01/01/2020" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Estilo", Value = "1" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Estatus", Value = Estilo });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Mensaje", Value = "1" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Userkey", Value = Numero });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Url", Value = "1" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 3 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
            }

            BindGridView();

        }
        catch (Exception v)
        {
            string err = v.Message;
        }
    }

    protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = GridView2.Rows[index];
        }
        catch (Exception ex)
        {
            string Error = ex.Message;
            //RutinaError(ex);
            //LogError(pLogKey, pUserKey, "Carga-Factura:GridView1_RowCommand", ex.Message, pCompanyID);
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

}