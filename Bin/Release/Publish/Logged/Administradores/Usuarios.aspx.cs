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

public partial class Pagos : System.Web.UI.Page
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
                        GridView1.AllowPaging = true;
                        GridView1.PageSize = 15;
                        GridView2.AllowPaging = true;
                        GridView2.PageSize = 15;
                        GridView1.AllowSorting = true;
                        GridView2.AllowSorting = true;
                        Button1.Visible = true;
                        Button2.Visible = true;
                        BindGridView(); //Proveedores
                        BindGridView2(); //TSYS
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

    private void BindGridView()
    {
        DatosV.Visible = false;
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            // Create a DataSet object. 
            DataSet dsProveedores = new DataSet();
            SqlCommand cmd = new SqlCommand("spSelectUsers", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Opcion",
                Value = 1
            });

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();
            if (!rdr.HasRows)
            {
                DatosV.Visible = true;
                Button1.Visible = false;
            }
            while (rdr.Read())
            {
                DataRow row = dt.NewRow();
                row["Fecha"] = (rdr["Fecha"].ToString());
                row["UserID"] = HttpUtility.HtmlDecode(rdr["UserID"].ToString());
                row["Username"] = HttpUtility.HtmlDecode(rdr["VendName"].ToString());
                row["Empresa"] = HttpUtility.HtmlDecode(rdr["Empresa"].ToString());
                row["Estado"] = HttpUtility.HtmlDecode(rdr["Status"].ToString());

                dt.Rows.Add(row);
            }
            GridView1.DataSource = dt;
            GridView1.DataBind();
              
        }

    }

    string eventName = String.Empty;

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set the index of the new display page.  
        GridView1.PageIndex = e.NewPageIndex;


        // Rebind the GridView control to  
        // show data in the new page. 
        BindGridView();
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
        }
    }

    protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {

    }

    protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView1.EditIndex = e.NewEditIndex;
        try
        {
            int x;
            DataTable dt = new DataTable();

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");
            //dt.Columns.Add("Comentarios");

            x = GridView1.Rows.Count;

            if (x == 0)
            {
                GridView1.DataSource = "";
                DatosV.Visible = true;
                Button1.Visible = false;
            }
            else
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = HttpUtility.HtmlDecode(gvr.Cells[5].Text.ToString());
                    //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                    // HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    dt.Rows.Add(dr);
                 }
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();

            //GridView1.EditIndex = -1;
            //BindGridView();
        }
        catch (Exception v)
        {

        }

        // Rebind the GridView control to show data in edit mode. 
        //BindGridView();
    }

    protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        // Exit edit mode. 
        try
        {
            int i, x;
            i = e.RowIndex;
            GridView1.EditIndex = -1;
            DataTable dt = new DataTable();

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");
            //dt.Columns.Add("Comentarios");

            x = GridView1.Rows.Count;

            if (x == 0)
            {
                GridView1.DataSource = "";
                DatosV.Visible = true;
                Button1.Visible = false;
            }
            else
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = HttpUtility.HtmlDecode(gvr.Cells[5].Text.ToString());
                        //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = "No Aprobado";
                        //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                }
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();
            //
            //BindGridView();
        }
        catch (Exception v)
        {

        }
    }

    protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        try
        {   
            int i, x;
            i = e.RowIndex;
            GridView1.EditIndex = -1;
            DataTable dt = new DataTable();

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");
            //dt.Columns.Add("Comentarios");

            x = GridView1.Rows.Count;

            if (x == 0)
            {
                GridView1.DataSource = "";
                DatosV.Visible = true;
                Button1.Visible = false;
            }
            else
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = HttpUtility.HtmlDecode(gvr.Cells[5].Text.ToString());
                        //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = "Aprobado";
                        //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                }
            }
            
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
        catch (Exception v)
        {

        }
         
    }

    private void BindGridView2()
    {
        DatosT.Visible = false;
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            DataSet dsProveedores = new DataSet();
            SqlCommand cmd = new SqlCommand("spSelectUsers", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Opcion",
                Value = 2
            });

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            //dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();
            if (!rdr.HasRows)
            {
                DatosT.Visible = true;
                Button2.Visible = false;
            }
            while (rdr.Read())
            {
                DataRow row = dt.NewRow();
                row["Fecha"] = HttpUtility.HtmlDecode(rdr["Fecha"].ToString());
                row["UserID"] = HttpUtility.HtmlDecode(rdr["UserID"].ToString());
                row["UserName"] = HttpUtility.HtmlDecode(rdr["UserName"].ToString());
                //row["Empresa"] = HttpUtility.HtmlDecode(rdr["Empresa"].ToString());
                row["Estado"] = HttpUtility.HtmlDecode(rdr["Status"].ToString());

                ///dr["Fecha"] = HttpUtility.HtmlDecode(rdr["Fecha"].ToString());

                dt.Rows.Add(row);
            }
            GridView2.DataSource = dt;
            GridView2.DataBind();
        }

    }

    protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView2.PageIndex = e.NewPageIndex;
        BindGridView2();
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
            int x;
            DataTable dt = new DataTable();

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            //dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");
            //dt.Columns.Add("Comentarios");

            x = GridView2.Rows.Count;

            if (x == 0)
            {
                GridView2.DataSource = "";
                DatosT.Visible = true;
                Button2.Visible = false;
            }
            else
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    dr["Estado"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                    //
                    dt.Rows.Add(dr);
                }
            }

            GridView2.DataSource = dt;
            GridView2.DataBind();
        }
        catch (Exception v)
        {

        }
    }

    protected void GridView2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        // Exit edit mode. 
        try
        {
            int i, x;
            i = e.RowIndex;
            GridView2.EditIndex = -1;
            DataTable dt = new DataTable();

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            //dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");
            //dt.Columns.Add("Comentarios");

            x = GridView2.Rows.Count;

            if (x == 0)
            {
                GridView2.DataSource = "";
                DatosT.Visible = true;
                Button2.Visible = false;
            }
            else
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = "No Aprobado";
                        dt.Rows.Add(dr);
                    }
                }
            }

            GridView2.DataSource = dt;
            GridView2.DataBind();
            //
            //BindGridView();
        }
        catch (Exception v)
        {

        }

    }

    protected void GridView2_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int i, x;
            i = e.RowIndex;
            GridView2.EditIndex = -1;
            DataTable dt = new DataTable();

            dt.Columns.Add("Fecha");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserName");
            //dt.Columns.Add("Empresa");
            dt.Columns.Add("Estado");
            //dt.Columns.Add("Comentarios");

            x = GridView2.Rows.Count;

            if (x == 0)
            {
                GridView2.DataSource = "";
                DatosT.Visible = true;
                Button2.Visible = false;
            }
            else
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["Fecha"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["USerID"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["UserName"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        dr["Estado"] = "Aprobado";
                        //dr["Comentarios"] = gvr.Cells[6].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                }
            }

            GridView2.DataSource = dt;
            GridView2.DataBind();
        }
        catch (Exception v)
        {

        }
    }

    protected void Aceptar()
    {
        try
        {
            int cont = 0;
            int errs = 0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                if (gvr.Cells[5].Text.ToString() == "Aprobado")
                {
                    string Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    string Razon = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    string Company = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    if (Alta(Email, Razon, Company, 1, 2) == true)
                    {
                        int cte = ConsultaUserKeyDB(Email);
                        if ((CancelarD1(cte, 1, 1, 3) == 0))
                        {
                            if (EmailP(Email) == true)
                            {
                                CancelarD(cte, 1, 2, 3);
                                cont = cont + 1;
                            }

                        }
                        //cont = cont + 1;
                    }
                    else
                    {
                        errs = errs + 1;  //cont = cont - 1;
                    }
                }
                else if (gvr.Cells[5].Text.ToString() == "No Aprobado")
                {
                    string Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    string Razon = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    string Company = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    if (Alta(Email, Razon, Company, 2, 2) == true)
                    {
                        cont = cont + 1;
                    }
                    else
                    {
                        errs = errs + 1;
                    }
                    
                }
            }

            if (errs > 0)
            {
                string titulo, Msj, tipo;
                tipo = "info";
                Msj = "Se ha Generado un error durante la conexión con el servidor, intentalo nuevamente si el problema persiste contacta al área de soporte";
                titulo = "Notificaciones T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
            else
            {
                if (cont >= 1)
                {
                    string titulo, Msj, tipo;
                    tipo = "success";
                    Msj = " Se Completo el Proceso Exitosamente!!";
                    titulo = "Aprobacion de Usuario";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    //BindGridView(); //TSYS
                }
                else
                {
                    Response.Redirect("Usuarios.aspx",false);
                    //string titulo, Msj, tipo;
                    //tipo = "info";
                    //Msj = "No se Ha Detectado Ningun Cambio";
                    //titulo = "Notificaciones T|SYS|";
                    //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
            }

        }
        catch(Exception ex)
        {
            int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string CompanyIDs = HttpContext.Current.Session["IDCompany"].ToString();
            LogError(pLogKey, pUserKey, "Aprobación de Usuarios Provedores", "Error en rutina de Aprobación - " + ex.Message, CompanyIDs);
            string titulo, Msj, tipo;
            tipo = "info";
            Msj = "Se ha Generado un error durante ejecución de la actividad, intentalo nuevamente si el problema persiste contacta al área de soporte";
            titulo = "Aprobacion de Usuario";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
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

    private bool EmailP(string CorreoUs)
    {
        bool Resut = false;
        try
        {

            string PassNew = Membership.GeneratePassword(8, 1);
            string PassHAs = PassNew.ToString();
            PassHAs = PassHAs.Replace(" ", "");

            ApplicationDbContext context = new ApplicationDbContext();
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
            string hashedNewPassword = UserManager.PasswordHasher.HashPassword(PassHAs);
            bool UpdatePass = UpdatePAss(CorreoUs, hashedNewPassword);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionOkUserT.html")))
            {
                body = reader.ReadToEnd();
                body = body.Replace("{UserTemp}", CorreoUs);
                body = body.Replace("{PassTemp}", PassHAs);
            }

            Resut = Global.EmailGlobal(CorreoUs, body, "BIENVENIDO AL PORTAL T|SYS|");
            
        }
        catch (Exception b)
        {
            Resut = false;
        }
        return Resut;
    }

    private bool EmailT(string CorreoUs)
    {
        bool Resut = false;
        try
        {

         string PassNew = Membership.GeneratePassword(8, 1);
         string PassHAs = PassNew.ToString();
         PassHAs = PassHAs.Replace(" ","");

         ApplicationDbContext context = new ApplicationDbContext();
         UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
         UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
         string hashedNewPassword = UserManager.PasswordHasher.HashPassword(PassHAs);
         bool UpdatePass = UpdatePAss(CorreoUs, hashedNewPassword);
         PassHAs = PassHAs.Replace(" ", "");
         string body = string.Empty;
         using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionOkUser.html")))
         {
           body = reader.ReadToEnd();
           body = body.Replace("{UserTemp}", CorreoUs);
           body = body.Replace("{PassTemp}", PassHAs);
         }

            Resut = Global.EmailGlobal(CorreoUs, body, "BIENVENIDO AL PORTAL T|SYS|");

        }
        catch (Exception b)
        {
            Resut = false;
        }
        return Resut;
    }

    protected void AceptarTsys()
    {
        try
        {
            int cont = 0;
            int errs = 0;
            foreach (GridViewRow gvr in GridView2.Rows)
            {
                if (gvr.Cells[4].Text.ToString() == "Aprobado")
                {
                    string Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    string Razon = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    //string Company = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    if (Alta(Email, Razon," ", 1, 1) == true)
                    {
                        int cte = ConsultaUserKeyDB(Email);
                        if ((CancelarD1(cte, 1, 1, 3) == 0))
                        {
                            if (EmailP(Email) == true)
                            {
                                cont = cont + 1;
                                CancelarD(cte, 1, 2, 3);
                            }
                        }
                    }
                    else
                    {
                        errs = errs + 1;
                    }

                    

                }
                else if (gvr.Cells[4].Text.ToString() == "No Aprobado")
                {
                    string Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    string Razon = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    //string Company = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    if (Alta(Email, Razon, " ", 2,1) == true)
                    {
                        cont = cont + 1;
                    }
                    else
                    {
                        errs = errs + 1;
                    }
                }
            }

            if (errs > 0)
            {
                string titulo, Msj, tipo;
                tipo = "info";
                Msj = "Se ha Generado un error durante la conexión con el servidor, intentalo nuevamente si el problema persiste contacta al área de soporte";
                titulo = "Notificaciones T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
            else
            {
                if (cont >= 1)
                {
                    string titulo, Msj, tipo;
                    tipo = "success";
                    Msj = " Se Completo el Proceso Exitosamente!!";
                    titulo = "Aprobacion de Usuario";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    //BindGridView2(); //TSYS
                }
                else
                {
                    Response.Redirect("Usuarios.aspx");
                }
            }
              
        }
        catch(Exception ex)
        {
            int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string CompanyIDs = HttpContext.Current.Session["IDCompany"].ToString();
            LogError(pLogKey, pUserKey, "Aprobación de Usuarios TSYS", "Error en rutina de Aprobación - " + ex.Message , CompanyIDs);
            string titulo, Msj, tipo;
            tipo = "danger";
            Msj = "Se ha Generado un error durante ejecución de la actividad, intentalo nuevamente si el problema persiste contacta al área de soporte";
            titulo = "Aprobacion de Usuario";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
    }

    private int ConsultaUserKeyDB(string Email)
    {
        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            sql = @"Select top 1 UserKey From AspNetUsers Where UserName ='" + Email + "'";

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
            //LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            //HttpContext.Current.Session["Error"] = err;
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return 0;
        }

    }

    protected bool Alta(string Email,string Razon,string Company,int Valor,int Opcion)
    {
        bool val = false;
        try
        {
            string[] Arreglo = Company.Split('-'); //Genera Arreglo de la Descripcion y obtiene los Datos 
            string CompanysID = Arreglo[0].ToString().TrimEnd(); // ID De Articulo
            string Key = HttpContext.Current.Session["UserKey"].ToString();
            string LogKey = HttpContext.Current.Session["LogKey"].ToString();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spUpdateUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@ID",
                    Value = Razon
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Companys",
                    Value = CompanysID
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = Email
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@UserKey",
                    Value = Key
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Valor",
                    Value = Valor
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Opcion",
                    Value = Opcion
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@LogKey",
                    Value = LogKey
                });


                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                string Rest = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Rest = rdr["Resultado"].ToString(); // 0 ok
                }
                if (Rest == "1")
                {
                    val = true;
                }
                else
                {
                    val = false;
                }                
                conn.Close();
            }
        }
        catch (Exception b)
        {
            int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string CompanyIDs = HttpContext.Current.Session["IDCompany"].ToString();
            LogError(pLogKey, pUserKey, "Aprobación de Usuarios", "Error en SP spUpdateUser de Aprobación - " + b.Message, CompanyIDs);
        }
        return val;
    }

    protected void btn_Enviar(object sender, EventArgs e)
    {
        try
        {
            if (GridView1.Rows.Count >= 1)
            {
                Aceptar();
                BindGridView();
                BindGridView2();                
            }
            else
            {
                //string tipo, Msj, titulo;
                //tipo = "error";
                //Msj = "No hay Datos Para Enviar";
                //titulo = "Aprobacion de Usuarios";
                //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);

            }
            
        }
        catch
        {
            //string tipo, Msj, titulo;
            //tipo = "error";
            //Msj = "Error al Ejecutar el Procedimiento de Guardado";
            //titulo = "Aprobacion de Usuarios";
            //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
    }

    protected void btn_EnviarTsys(object sender, EventArgs e)
    {
        try
        {
            if (GridView2.Rows.Count >= 1)
            {
                AceptarTsys();
                BindGridView();
                BindGridView2();                
                //HttpContext.Current.Response.Flush();
                //HttpContext.Current.ApplicationInstance.CompleteRequest();
                //HttpContext.Current.Response.End();
            }
            else
            {
                //string tipo, Msj, titulo;
                //tipo = "error";
                //Msj = "No hay Datos Para Enviar";
                //titulo = "Aprobacion de Usuarios";
                //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);

            }
        }
        catch
        {
            //string tipo, Msj, titulo;
            //tipo = "error";
            //Msj = "Error al Ejecutar el Procedimiento de Guardado";
            //titulo = "Aprobacion de Usuarios";
            //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
    }

    protected int CancelarD1(int FacKey, int Not, int Op, int Tipo)
    {
        int val1 = -1;
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string sSQL;
            string Rest = string.Empty;

            sSQL = "Notificaciones_Fc";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@Invc", FacKey));
            parsT.Add(new SqlParameter("@Not", Not));
            parsT.Add(new SqlParameter("@Opcion", Op));
            parsT.Add(new SqlParameter("@Tipo", Tipo));

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
                    Rest = rdr["Resultado"].ToString(); // 0 ok
                }
                sqlConnection1.Close();
                val1 = Convert.ToInt32(Rest);

            }
        }
        catch (Exception ex)
        {
            throw new MulticonsultingException(ex.Message);
        }
        return val1;
    }

    protected bool CancelarD(int FacKey, int Not, int Op, int Tipo)
    {
        bool vr = false;
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string sSQL;

            sSQL = "Notificaciones_Fc";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@Invc", FacKey));
            parsT.Add(new SqlParameter("@Not", Not));
            parsT.Add(new SqlParameter("@Opcion", Op));
            parsT.Add(new SqlParameter("@Tipo", Tipo));

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
                //while (rdr.Read())
                //{
                //    val1 = rdr.GetInt32(0); // 0 ok
                //}
                sqlConnection1.Close();
                vr = true;

            }
        }
        catch (Exception ex)
        {
            throw new MulticonsultingException(ex.Message);
        }
        return vr;
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

    protected bool Registro(string CrtMail, string UserMaill, string Empresa)
    {
        bool Res = false;
        try
        {
            string Company = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spUpdateUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@CreateMail", Value = CrtMail });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserMail", Value = UserMaill });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Company });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Status", Value = 1 });

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
        catch
        {
            Res = false;
        }
        return Res;
    }

}