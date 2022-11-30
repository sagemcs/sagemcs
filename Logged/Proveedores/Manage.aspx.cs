//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA ACTUALIZACION DE PASSWORD PARA PROVEEDORES

//REFERENCIAS UTILIZADAS
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using WebSite1;

public partial class Account_Manage : System.Web.UI.Page
{
    string eventName = String.Empty;

    protected string SuccessMessage
    {
        get;
        private set;
    }

    protected bool CanRemoveExternalLogins
    {
        get;
        private set;
    }

    private bool HasPassword(UserManager manager)
    {
        var user = manager.FindById(User.Identity.GetUserId());
        return (user != null && user.PasswordHash != null);
    }

    protected void Page_Load()
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
                    // Determine the sections to render
                    UserManager manager = new UserManager();
                    if (HasPassword(manager))
                    {
                        changePasswordHolder.Visible = true;
                    }
                    else
                    {
                        setPassword.Visible = true;
                        changePasswordHolder.Visible = false;
                    }
                    CanRemoveExternalLogins = manager.GetLogins(User.Identity.GetUserId()).Count() > 1;

                    // Render success message
                    var message = Request.QueryString["m"];
                    if (message != null)
                    {
                        // Strip the query string from action
                        Form.Action = ResolveUrl("~/Logged/Proveedores/Manage");

                        SuccessMessage =
                            message == "ChangePwdSuccess" ? "Your password has been changed."
                            : message == "SetPwdSuccess" ? "Your password has been set."
                            : message == "RemoveLoginSuccess" ? "The account was removed."
                            : String.Empty;
                        successMessage.Visible = !String.IsNullOrEmpty(SuccessMessage);

                        if (message.ToString() == "ChangePwdSuccess")
                        {

                            string key = HttpContext.Current.Session["UserKey"].ToString();
                            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                            {
                                SqlCommand cmd = new SqlCommand("PassTemp", conn);
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Userkey", Value = key });
                                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 3 });
                                if (conn.State == ConnectionState.Open) { conn.Close(); }

                                conn.Open();
                                SqlDataReader rdr = cmd.ExecuteReader();
                            }
                            HttpContext.Current.Session["Passw"] = "0";
                            string tipo = "success";
                            string Msj = "Su contraseña ha cambiado";
                            string titulo = "Actualización de Password Exitosa";
                            ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                        }

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
            else if (Dias == 25)
            {
                Page.MasterPageFile = "MenuP.master";
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

            //if ((HttpContext.Current.Session["Docs"].ToString() == "1"))
            //{
            //    Global.RevDocs();
            //    if ((HttpContext.Current.Session["Docs"].ToString() == "1"))
            //    {
            //        //Page.MasterPageFile = "MenuP.master";
            //        Response.Redirect("~/Logged/Proveedores/Default.aspx", false);

            //    }
            //    //Page.MasterPageFile = "MenuP.master";
            //}
            //else if ((HttpContext.Current.Session["Status"].ToString() == "Activo"))
            //{
            //    //if (HttpContext.Current.Session["UpDoc"].ToString() == "1") { Page.MasterPageFile = "MenuP.master"; }
            //    //else { Page.MasterPageFile = "MenuPreP.master"; }
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

    private bool Email_Ok(string email)
    {
        string expresion;
        expresion = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
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

    protected void ChangePassword_Click(object sender, EventArgs e)
    {
        if (Email_Ok(NewPassword.Text) == true)
        {

        if (IsValid)
        {
            string Var = User.Identity.GetUserId();
            UserManager manager = new UserManager();
            IdentityResult result = manager.ChangePassword(User.Identity.GetUserId(), CurrentPassword.Text, NewPassword.Text);
            if (result.Succeeded)
            {
                var user = manager.FindById(User.Identity.GetUserId());
                IdentityHelper.SignIn(manager, user, isPersistent: false);
                Response.Redirect("~/Logged/Proveedores/Manage?m=ChangePwdSuccess");
            }
            else
            {
                AddErrors(result);
            }
          }
        }
        else
        {
            string Msj1 = "";
            Msj1 = Msj1 + "La Contraseña debe de contar con:" + "<br/>";
            Msj1 = Msj1 + "Al menos una letra mayúscula" + "<br/>";
            Msj1 = Msj1 + "Al menos una letra minúscula" + "<br/>";
            Msj1 = Msj1 + "Al menos un dígito" + "<br/>";
            Msj1 = Msj1 + "Al menos un caracter especial" + "<br/>";
            Msj1 = Msj1 + "Mínimo ocho digítos de largo" + "<br/>";

            Label2.Text = Msj1;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);

        }
    }

    protected void SetPassword_Click(object sender, EventArgs e)
    {
        if (IsValid)
        {
            // Create the local login info and link the local account to the user
            UserManager manager = new UserManager();
            IdentityResult result = manager.AddPassword(User.Identity.GetUserId(), password.Text);
            if (result.Succeeded)
            {
                Response.Redirect("~/Logged/Proveedores/Manage?m=SetPwdSuccess");
            }
            else
            {
                AddErrors(result);
            }
        }
    }

    public IEnumerable<UserLoginInfo> GetLogins()
    {
        UserManager manager = new UserManager();
        var accounts = manager.GetLogins(User.Identity.GetUserId());
        CanRemoveExternalLogins = accounts.Count() > 1 || HasPassword(manager);
        return accounts;
    }

    public void RemoveLogin(string loginProvider, string providerKey)
    {
        UserManager manager = new UserManager();
        var result = manager.RemoveLogin(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        string msg = String.Empty;
        if (result.Succeeded)
        {
            var user = manager.FindById(User.Identity.GetUserId());
            IdentityHelper.SignIn(manager, user, isPersistent: false);
            msg = "?m=RemoveLoginSuccess";
        }
        Response.Redirect("~/Logged/Proveedores/Manage" + msg);
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error);
        }
    }
}