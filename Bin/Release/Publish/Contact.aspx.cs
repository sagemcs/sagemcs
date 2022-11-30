using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Contact : Page
{
    string eventName = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //if (!IsPostBack)
        //{

        //    if (HttpContext.Current.Session["IDCompany"] == null)
        //    {
        //        Context.GetOwinContext().Authentication.SignOut();
        //        Response.Redirect("~/Account/Login.aspx");
        //    }
        //    else
        //    {
        //        if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
        //        {

        //        }
        //        else
        //        {
        //            HttpContext.Current.Session.RemoveAll();
        //            Context.GetOwinContext().Authentication.SignOut();
        //            Response.Redirect("~/Account/Login.aspx");
        //        }
        //    }
        //}
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        try
        {
            bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (!isAuth)
            {
                //HttpContext.Current.Session.RemoveAll();
                //Context.GetOwinContext().Authentication.SignOut();
                //Response.Redirect("~/Account/Login.aspx");
                Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
                Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
                Page.Response.Cache.SetNoStore();
                Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Current.Session.RemoveAll();
                Context.GetOwinContext().Authentication.SignOut();
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
                {
                    Page.MasterPageFile = "~/Logged/Administradores/MasterPageContb.master";
                }
                if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
                {
                    Page.MasterPageFile = "~/Logged/Administradores/SiteVal.master";
                }

                if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
                {
                    Page.MasterPageFile = "~/Site.master";
                }
                if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                {
                    Global.Docs();
                    //string Valor = HttpContext.Current.Session["Docs"].ToString();

                    int Valor = Convert.ToInt16(HttpContext.Current.Session["Docs"].ToString());
                    int Dias = Valor;
                    if (Dias == 0)
                    {
                        Page.MasterPageFile = "~/Logged/Proveedores/MenuP.master";
                    }
                    else if (Dias < 0)
                    {
                        Page.MasterPageFile = "~/Logged/Proveedores/MenuP.master";
                    }
                    else if (Dias == 30)
                    {
                        Page.MasterPageFile = "~/Logged/Proveedores/MenuPreP.master";
                    }
                    else if (Dias == 25)
                    {
                        Page.MasterPageFile = "~/Logged/Proveedores/MenuP.master";
                    }
                    else if (Dias == 26)
                    {
                        Page.MasterPageFile = "~/Logged/Proveedores/MenuP.master";
                    }
                    else if (Dias == 27)
                    {
                        Page.MasterPageFile = "~/Logged/Proveedores/MenuP.master";
                    }
                    else if (Dias == 28)
                    {
                        Page.MasterPageFile = "~/Logged/Proveedores/MenuPreP.master";
                    }
                    else if (Dias == 22)
                    {
                        Page.MasterPageFile = "~/Logged/Proveedores/MenuP.master";
                    }
                    else if (Dias <= 10 && Dias > 0)
                    {
                        Page.MasterPageFile = "~/Logged/Proveedores/MenuPreP.master";
                    }
                    else if (Dias > 10)
                    {
                        Page.MasterPageFile = "~/Logged/Proveedores/MenuPreP.master";
                    }

                }

                if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Empleado")
                {
                    Page.MasterPageFile = "~/Logged/Administradores/SiteEmpleado.master";
                }
            }
            //    bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            //    if (!isAuth)
            //    {
            //        HttpContext.Current.Session.RemoveAll();
            //        Context.GetOwinContext().Authentication.SignOut();
            //        Response.Redirect("~/Account/Login.aspx");
            //    }

            //    if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
            //    {
            //        Page.MasterPageFile = "MasterPageContb.master";
            //    }
            //    if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
            //    {
            //        Page.MasterPageFile = "SiteVal.master";
            //    }
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