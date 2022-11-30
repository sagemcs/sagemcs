//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA INFORMATIVA DE CONTACTO

//REFERENCIAS UTILIZADAS
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

        if (!IsPostBack)
        {

            if (HttpContext.Current.Session["IDCompany"] == null)
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                {

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
            bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (!isAuth)
            {
                HttpContext.Current.Session.RemoveAll();
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }


            Global.Docs();
            int Valor = Convert.ToInt16(HttpContext.Current.Session["Docs"].ToString());
            int Dias = Valor;
            if (Dias == 0)
            {
                Page.MasterPageFile = "MenuP.master";
            }
            else if (Dias < 0)
            {
                Page.MasterPageFile = "MenuP.master";
            }
            else if (Dias == 30)
            {
                Page.MasterPageFile = "MenuPreP.master";
            }
            else if (Dias == 26)
            {
                Page.MasterPageFile = "MenuP.master";
            }
            else if (Dias == 27)
            {
                Page.MasterPageFile = "MenuP.master";
            }
            else if (Dias == 28)
            {
                Page.MasterPageFile = "MenuPreP.master";
            }
            else if (Dias == 22)
            {
                Page.MasterPageFile = "MenuP.master";
            }
            else if (Dias <= 10 && Dias > 0)
            {
                Page.MasterPageFile = "MenuPreP.master";
            }
            else if (Dias > 10)
            {
                Page.MasterPageFile = "MenuPreP.master";
            }

            //Global.Docs();
            //Global.RevDocs();
            //if ((HttpContext.Current.Session["Docs"].ToString() == "0"))
            //{
            //    Page.MasterPageFile = "MenuP.master";
            //}
            //else if ((HttpContext.Current.Session["Status"].ToString() == "Activo"))
            //{
            //    if (HttpContext.Current.Session["UpDoc"].ToString() == "1") { Page.MasterPageFile = "MenuP.master"; }
            //    else { Page.MasterPageFile = "MenuPreP.master"; }
            //}
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
}