using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

public partial class SiteMaster : MasterPage
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;

    protected void Page_Init(object sender, EventArgs e)
    {
        // The code below helps to protect against XSRF attacks
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        Guid requestCookieGuidValue;
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
        {
            // Use the Anti-XSRF token from the cookie
            _antiXsrfTokenValue = requestCookie.Value;
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        else
        {
            // Generate a new Anti-XSRF token and save to the cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            Page.ViewStateUserKey = _antiXsrfTokenValue;

            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                HttpOnly = true,
                Value = _antiXsrfTokenValue
            };
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }
            Response.Cookies.Set(responseCookie);
        }

        try {
            if (HttpContext.Current.Session["JWTKey"]!=null)
            {
                Text_Sec.Text = HttpContext.Current.Session["JWTKey"].ToString();
            }            
        }
        catch { }
        Page.PreLoad += master_Page_PreLoad;
    }

    protected void master_Page_PreLoad(object sender, EventArgs e)
    {
        bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

        if (!isAuth)
        {
            HttpContext.Current.Session.RemoveAll();
            Response.AppendHeader("Cache-Control", "no-cache");
            Response.CacheControl = "no-cache"; Response.Expires = -1;
            Response.ExpiresAbsolute = new DateTime(1900, 1, 1);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Context.GetOwinContext().Authentication.SignOut();
        }


        if (!IsPostBack)
        {
            // Set Anti-XSRF token
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
        }
        else
        {
            // Validate the Anti-XSRF token
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
            {
                throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        Context.GetOwinContext().Authentication.SignOut();
        HttpContext.Current.Session.RemoveAll();
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Session.Clear();
        Session.Abandon();
        Session.RemoveAll();
        Response.Redirect("~/Account/Login.aspx");
    }

    public bool HasRightsForSpecifiedMenu(string menuItemName)
    {

        int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());

        if (menuItemName == "Facturas")
        {
            return vermenu(pUserKey, "Facturas");
        }
        else if (menuItemName == "Reembolso")
        {
            return vermenu(pUserKey, "Reembolso");
        }
        else if (menuItemName == "Anticipo")
        {
            return vermenu(pUserKey, "Anticipo");
        }
        else if (menuItemName == "Tarjeta")
        {
            return vermenu(pUserKey, "Tarjeta");
        }
        else if (menuItemName == "GMedicos")
        {
            return vermenu(pUserKey, "GMedicos");
        }

        return true;
    }

    protected bool vermenu(int userkey, string Menu)
    {
        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();
            sql = @"SELECT " + Menu + " FROM PermEmpleados WHERE UserKey = " + userkey + "";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            if (Cuenta == "True")
                return true;
            else
                return false;
        }
        catch (Exception ex) { }
        return false;
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

            return null;
        }
    }
}