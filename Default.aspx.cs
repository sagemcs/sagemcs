using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : Page
{
    string eventName = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        //Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        //Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        //Page.Response.Cache.SetNoStore();
        //Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {

        try
        {
            bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuth)
            {
                Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
                Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
                Page.Response.Cache.SetNoStore();
                Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Current.Session.RemoveAll();
                Context.GetOwinContext().Authentication.SignOut();
                Page.Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    if (HttpContext.Current.Session["IDCompany"] == null)
                    {
                        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
                        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
                        Page.Response.Cache.SetNoStore();
                        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        HttpContext.Current.Session.RemoveAll();
                        Context.GetOwinContext().Authentication.SignOut();
                        Response.Redirect("~/Account/Login.aspx");
                    }
                    else
                    {
                        if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
                        {
                            Response.Redirect("~/Logged/Administradores/Default.aspx");
                        }

                        else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
                        {
                            Response.Redirect("~/Logged/Administradores/Default.aspx");
                        }

                        else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
                        {
                            Response.Redirect("~/Logged/Administradores/Default.aspx");
                        }

                        else if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                        {
                            Response.Redirect("~/Logged/Proveedores/Default.aspx");
                        }
                        //else
                        //{
                        //    Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
                        //    Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
                        //    Page.Response.Cache.SetNoStore();
                        //    Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        //    HttpContext.Current.Session.RemoveAll();
                        //    Context.GetOwinContext().Authentication.SignOut();
                        //    Response.Redirect("~/Account/Login.aspx");
                        //}
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }

        //try
        //{
        //    bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        //    if (!isAuth)
        //    {
        //        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        //        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        //        Page.Response.Cache.SetNoStore();
        //        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //        HttpContext.Current.Session.RemoveAll();
        //        Context.GetOwinContext().Authentication.SignOut();
        //        Response.Redirect("~/Account/Login.aspx");
        //    }
        //    else
        //    {
        //        if (!IsPostBack)
        //        {
        //            if (HttpContext.Current.Session["IDCompany"] == null)
        //            {
        //                Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        //                Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        //                Page.Response.Cache.SetNoStore();
        //                Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //                HttpContext.Current.Session.RemoveAll();
        //                Context.GetOwinContext().Authentication.SignOut();
        //                Response.Redirect("~/Account/Login.aspx");
        //            }
        //            else
        //            {
        //                if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
        //                {
        //                    Response.Redirect("~/Logged/Administradores/Default.aspx");
        //                }

        //                else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
        //                {
        //                    Response.Redirect("~/Logged/Administradores/Default.aspx");
        //                }

        //                else if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
        //                {
        //                    Response.Redirect("~/Logged/Administradores/Default.aspx");
        //                }

        //                else if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
        //                {
        //                    Response.Redirect("~/Logged/Proveedores/Default.aspx");
        //                }
        //                else
        //                {
        //                    Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        //                    Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        //                    Page.Response.Cache.SetNoStore();
        //                    Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //                    HttpContext.Current.Session.RemoveAll();
        //                    Context.GetOwinContext().Authentication.SignOut();
        //                    Response.Redirect("~/Account/Login.aspx");
        //                }
        //            }
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        //    Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        //    Page.Response.Cache.SetNoStore();
        //    Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    HttpContext.Current.Session.RemoveAll();
        //    Context.GetOwinContext().Authentication.SignOut();
        //    Response.Redirect("~/Account/Login.aspx");
        //}
    }

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        eventName = "OnPreInit";
    }



}