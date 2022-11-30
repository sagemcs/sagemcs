//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA PRINCIPAL DE USUARIO T|SYS| LOGUEADO

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

public partial class _Default : Page
{
    string eventName = String.Empty;
    string Texr = string.Empty;
    int Im = 0;

    protected void Page_Load(object sender, EventArgs e)
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
                    if ((HttpContext.Current.Session["Passw"].ToString() == null)) { }
                    else if ((HttpContext.Current.Session["Passw"].ToString() == "1"))
                    {
                        string titulo, Msj, tipo;
                        tipo = "warning";
                        Msj = "Tu Password esta a punto de vencer,te invitamos a renovarlo a la brevedad ya que de lo contrario este se actualizará y tendrás que ejecutar la recuperación de contraseña nuevamente ";
                        titulo = "Actualización de Password"; 
                        Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                        //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    }

                    RevisaNotificaciones();

                    if (Texr != "")
                    {
                        string Mensaje;
                        if (Im > 0) { Mensaje = "swal.mixin({confirmButtonText: 'Ok',showCancelButton: false,customClass: 'swal-widei'}).queue([ "; }
                        else { Mensaje = "swal.mixin({confirmButtonText: 'Ok',showCancelButton: false,customClass: 'swal-wide'}).queue([ "; }
                        Mensaje = Mensaje + Texr.Substring(0, (Texr.Length) - 1);
                        Mensaje = Mensaje + " ])";
                        ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "Varis(" + Mensaje + ");", true);
                    }

                    //if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
                    //{
                    //    Page.MasterPageFile = "MasterPageContb.master";
                    //}

                    //if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
                    //{

                    //}

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

    protected void RevisaNotificaciones()
    {
        try
        {
            Im = 0;
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
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 4 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                string titulo, Msj, tipo, Img;

                while (rdr.Read())
                {
                    Msj = Convert.ToString(rdr.GetValue(0));
                    tipo = Convert.ToString(rdr.GetValue(1));
                    titulo = Convert.ToString(rdr.GetValue(2));
                    if (Convert.ToString(rdr.GetValue(3)) != "")
                    {
                        Img = Convert.ToString(rdr.GetValue(3));
                        Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "', imageUrl: '" + Img + "'  },";
                        Im = +1;
                    }
                    else
                    {
                        Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                    }

                }
            }

        }
        catch (Exception)
        {

        }
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        try
        {
            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
            {
                Page.MasterPageFile = "MasterPageContb.master";
            }
            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
            {
                Page.MasterPageFile = "SiteVal.master";
            }
            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Finanzas")
            {
                Page.MasterPageFile = "SiteVal.master";
            }
            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Tesoreria")
            {
                Page.MasterPageFile = "SiteVal.master";
            }
            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Empleado")
            {
                Response.Redirect("~/Logged/Administradores/Default_Empleado.aspx", false);
                //Page.MasterPageFile = "~/Logged/Administradores/SiteEmpleado.master";
            }
            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Gerencia de Capital Humano")
            {
                Page.MasterPageFile = "SiteVal.master";
            }
            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Gerente")
            {
                Page.MasterPageFile = "SiteVal.master";
            }
        }
        catch (Exception ex)
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



}
