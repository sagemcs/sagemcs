//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA MANTENIMIENTO A CUENTA DE EMISION DE CORREOS T|SYS|

//REFERENCIAS UTILIZADAS
using EASendMail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logged_Administradores_SettingsEmail : System.Web.UI.Page
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
                        if (!Buscar("General") == true)
                        {

                        }
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

    protected void SelUno(object sender, EventArgs e)
    {
        try
        {
            if (!Buscar("General") == true)
            {

            }
        }
        catch
        {

        }
    }

    protected void SelDos(object sender, EventArgs e)
    {
        try
        {
            //if (!Buscar(SolCont) == true)
            //{

            //}
        }
        catch
        {

        }
    }

    protected void Save(object sender, EventArgs e)
    {
        try
        {
            string titulo, Msj, tipo;

            if (Email_Ok(EmailC.Text) == true)
            {
                if (spSave() == true)
                {
                    tipo = "success";
                    Msj = "Actualización Exitosa";
                    titulo = "Exito";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
                else
                {
                    tipo = "error";
                    Msj = "Se generaron Errores al intentar guardar los cambios, Intentalo nuevamenene";
                    titulo = "Error";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
            }
            else
            {            
             tipo = "error";
             Msj = "Formato de Correo Invalido, Favor de Verificar";
             titulo = "Error";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            } 
        }
        catch
        {

        }
    }

    protected bool Buscar(string Control)
    {
        bool ry = false;
        try
        {
            EmailC.Text = "";
            Pass.Text = "";
            Host.Text = "";
            Puert.Text = "";
            ////DatosV.Visisble = false;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSettingsMail", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter(){ ParameterName = "@Opcion", Value = 1 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Task", Value = Control});
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = 0 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@user", Value = 0 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Pass", Value = 0 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Host", Value = 0 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Puerto", Value = 0 });

                if (conn.State == ConnectionState.Open) { conn.Close(); } conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                  if (rdr["Usuario"].ToString() != null) { EmailC.Text = rdr["Usuario"].ToString();}
                  if (rdr["Password"].ToString() != null) { Pass.Text = rdr["Password"].ToString(); }
                  if (rdr["Host"].ToString() != null) { Host.Text = rdr["Host"].ToString(); }
                  if (rdr["Puerto"].ToString() != null) { Puert.Text = rdr["Puerto"].ToString(); }
                }
            }
            //GridEmails();
            ry = true;
        }
        catch
        {
            ry = false;
        }
        return ry;
    }

    protected bool spSave()
    {
        bool Rt = false;

        try
        {   
            
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSettingsMail", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 2 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Task", Value = "General" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = 0 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@user", Value = EmailC.Text });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Pass", Value = Pass.Text });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Host", Value = Host.Text });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Puerto", Value = Puert.Text });

                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
           }

            Rt = true;
        }
        catch
        {

        }
        return Rt;
    }

    private bool Email_Ok(string email)
    {
        String expresion;
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

    //protected void AddU(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        string titulo, Msj, tipo;
    //        if (Email_Ok(Email.Text) == true)
    //        {
    //            foreach (GridViewRow gvr in GridView1.Rows)
    //            {
    //                if (Email.Text == HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString()))
    //                {
    //                    tipo = "error";
    //                    Msj = "El Correo Ingresado ya se encuentra en la lista de Destinatarios asigandos a esta tarea ";
    //                    titulo = "Error";
    //                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
    //                    return;
    //                }
    //            }

    //            try
    //            {
    //                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
    //                {
    //                    DataSet dsProveedores = new DataSet();
    //                    SqlCommand cmd = new SqlCommand("spAddMail", conn);
    //                    cmd.CommandType = CommandType.StoredProcedure;

    //                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = Email.Text });
    //                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = SelProv.SelectedItem.ToString() });
    //                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 2 });

    //                    if (conn.State == ConnectionState.Open) { conn.Close(); }
    //                    conn.Open();
    //                    SqlDataReader rdr = cmd.ExecuteReader();
    //                }
    //                Email.Text = "";
    //                GridEmails();
    //            }
    //            catch
    //            {
    //                tipo = "error";
    //                Msj = "Se genero un error al intentar guardar los datos, Revisa la conexion con la BD Portal Proveedores";
    //                titulo = "Error";
    //                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
    //            }
    //        }
    //        else
    //        {
    //            tipo = "error";
    //            Msj = "Formato de Correo Invalido, Favor de Verificar";
    //            titulo = "Error";
    //            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
    //        }
    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //    Email.Text = "";
    //}

    //protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //    GridView1.PageIndex = e.NewPageIndex;
    //}

    //protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {

    //    }
    //}

    //protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    //{

    //}

    //protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    //{
    //    try
    //    {
    //        int i, x;
    //        i = e.RowIndex;
    //        //DatosV.Visisble = false;
    //        string EmailDelete = GridView1.Rows[i].Cells[1].Text.ToString();
    //        if (EmailDelete == "Usuario")
    //        {
    //            string titulo, Msj, tipo;
    //            tipo = "error";
    //            Msj = "Este item no se puede Eliminar";
    //            titulo = "Error";
    //            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
    //            return;
    //        }


    //        DataTable dt = new DataTable();
    //        dt.Columns.Add("Email");

    //        x = GridView1.Rows.Count;

    //        if (x == 1)
    //        {
    //            try
    //            {
    //                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
    //                {
    //                    DataSet dsProveedores = new DataSet();
    //                    SqlCommand cmd = new SqlCommand("spAddMail", conn);
    //                    cmd.CommandType = CommandType.StoredProcedure;

    //                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = EmailDelete });
    //                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = "General" });
    //                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 3 });

    //                    if (conn.State == ConnectionState.Open) { conn.Close(); }
    //                    conn.Open();
    //                    SqlDataReader rdr = cmd.ExecuteReader();
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                string titulo, Msj, tipo;
    //                tipo = "error";
    //                Msj = "Se genero un error al intentar eliminar los datos, Revisa la conexion con la BD Portal Proveedores";
    //                titulo = "Error";
    //                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
    //            }
    //            GridView1.DataSource = "";
    //        }
    //        else
    //        {
    //            foreach (GridViewRow gvr in GridView1.Rows)
    //            {
    //                if (gvr.RowIndex != i)
    //                {
    //                    DataRow dr = dt.NewRow();
    //                    dr["Email"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
    //                    dt.Rows.Add(dr);
    //                }
    //                else
    //                {
    //                    try
    //                    {
    //                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
    //                        {
    //                            DataSet dsProveedores = new DataSet();
    //                            SqlCommand cmd = new SqlCommand("spAddMail", conn);
    //                            cmd.CommandType = CommandType.StoredProcedure;

    //                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = EmailDelete });
    //                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value =SelProv.SelectedItem.ToString() });
    //                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 3 });

    //                            if (conn.State == ConnectionState.Open) { conn.Close(); }
    //                            conn.Open();
    //                            SqlDataReader rdr = cmd.ExecuteReader();
    //                        }
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        string titulo, Msj, tipo;
    //                        tipo = "error";
    //                        Msj = "Se genero un error al intentar eliminar los datos, Revisa la conexion con la BD Portal Proveedores";
    //                        titulo = "Error";
    //                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
    //                    }
    //                }
    //            }
    //        }

    //        GridView1.DataSource = dt;
    //        GridView1.DataBind();

    //        if (dt.Rows.Count == 0)
    //        { DatosV.Visible = true; }
    //        }
    //    catch (Exception v)
    //    {

    //    }
    //}

    //protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    //{
    //    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
    //    {

    //    }
    //}

    //protected void GridEmails()
    //{
    //    try
    //    {
    //        DatosV.Visible = false;
    //        DataTable dt = new DataTable();
    //        dt.Columns.Add("Email");
    //        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
    //        {
    //            SqlCommand cmd = new SqlCommand("spAddMail", conn);
    //            cmd.CommandType = CommandType.StoredProcedure;

    //            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = "xx" });
    //            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = SelProv.SelectedItem.ToString() });
    //            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 1 });

    //            if (conn.State == ConnectionState.Open) { conn.Close(); }
    //            conn.Open();
    //            SqlDataReader rdr = cmd.ExecuteReader();
    //            while (rdr.Read())
    //            {
    //                DataRow dr = dt.NewRow();
    //                dr["Email"] = HttpUtility.HtmlDecode(rdr["Destinatario"].ToString());
    //                dt.Rows.Add(dr);
    //            }

    //            GridView1.DataSource = dt;
    //            GridView1.DataBind();

    //            if (dt.Rows.Count == 0)
    //            { DatosV.Visible = true;} 
    //        }
    //    }
    //    catch
    //    {
    //        string titulo, Msj, tipo;
    //        tipo = "error";
    //        Msj = "Se genero un error al intentar guardar los datos, Revisa la conexion con la BD Portal Proveedores";
    //        titulo = "Error";
    //        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
    //    }
    //}

    private bool EmailTsys(string Destinatario)
    {
        bool rest = false;
        try
        {

            //string Body, PassNew;
            //PassNew = "1234568";
            //using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionOKUser.html")))
            //{
            //    Body = reader.ReadToEnd();
            //    Body = Body.Replace("{PassTemp}", PassNew);
            //    Body = Body.Replace("{UserTemp}", "lgarcia@multiconsulting.com");
            //}

            string Body = "Este es un Email de Prueba , Configuracion Exitosa!";
            rest = Global.EmailGlobal(Destinatario, Body, "Correo de Prueba");

        }

        catch
        {
            //Seccion Errores
        }
        return rest;
    }

    protected void EmailTest1(object sender, EventArgs e)
    {
        try
        {
            string titulo, Msj, tipo;
            if (Email_Ok(test.Text) == true)
            {
                if (EmailTsys(test.Text) == true)
                {
                    tipo = "success";
                    Msj = "¡Configuración Exitosa! , Se ha enviado un correo de prueba";
                    titulo = "Exito";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
                else
                {
                    tipo = "error";
                    Msj = "Configuración de Correo Incorrecta, Verifícala";
                    titulo = "Error";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
            }
            else
            {
                tipo = "error";
                Msj = "formato de Email Incorrecto, Verificalo!";
                titulo = "Error";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
        }
        catch (Exception ex)
        {

        }
    }



}