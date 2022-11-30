using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logged_Administradores_ComentariosValidador : System.Web.UI.Page
{
    #region Variables

    private int iVendKey;
    private int iLogKey;
    private int iUserKey;
    private string iCompanyID;    

    public int pVendKey
    {
        get
        {
            return this.iVendKey;
        }
        set
        {
            this.iVendKey = value;
        }
    }
    public string pCompanyID
    {
        get
        {
            return this.iCompanyID;
        }
        set
        {
            this.iCompanyID = value;
        }
    }
    public int pLogKey
    {
        get
        {
            return this.iLogKey;
        }
        set
        {
            this.iLogKey = value;
        }
    }
    public int pUserKey
    {
        get
        {
            return this.iUserKey;
        }
        set
        {
            this.iUserKey = value;
        }
    }

    public Dictionary<string, string> Dict_rutas ()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>
        {            
            { "ValidadorAnticipos", "Advance" },
            { "ValidadorReembolsos", "Expense" },
            { "ValidadorTarjetas", "CorporateCard" },
            { "ValidadorGastosMedicosMenores", "MinorMedicalExpense" }
        };
        return dict;
    }

    string eventName = String.Empty;

    #endregion
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

            string sSQL;

            sSQL = "spapErrorLog";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@LogKey", LogKey));
            parsT.Add(new SqlParameter("@UpdateUserKey", UpdateUserKey));
            parsT.Add(new SqlParameter("@proceso", proceso));
            parsT.Add(new SqlParameter("@mensaje", mensaje));
            parsT.Add(new SqlParameter("@CompanyID", CompanyID));

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

    public Dictionary<int, string> Dict_Level()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>
        {
            { 1, "T|SYS| - Validador" },
            { 2, "T|SYS|- Gerente-de-Área" },
            { 3, "T | SYS | -ValidadorTesorería" },
            { 4, "T | SYS | -ValidadorFinanzas" }
        };
        return dict;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        try
        {
            string rol = HttpContext.Current.Session["RolUser"].ToString();
            var validadores = Doc_Tools.get_RolesValidadores();
            if (!validadores.Any(x=> x.ID == rol))
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                try
                {                   
                    pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
                    pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
                    pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
                    int level = Dict_Level().FirstOrDefault(x=> x.Value == rol).Key;

                    if (!IsPostBack)
                    {
                        if (HttpContext.Current.Session["ExpenseComentId"]!=null)
                        {
                            int expense_id = (int)HttpContext.Current.Session["ExpenseComentId"];
                            var doc_type = (Doc_Tools.DocumentType)HttpContext.Current.Session["DocumentType"];

                            tbx_ListaComentarios.Text = string.Empty;
                            tbx_coment.Text = string.Empty;

                            var comentarios = read_coments(expense_id, pCompanyID, doc_type);
                            if(comentarios.Any(x=> x.ApprovalLevel == level))
                            {
                                tbx_coment.ReadOnly = true;
                                btn_GuardarComentario.Enabled = false;
                            }
                            foreach (var comentario in comentarios)
                            {
                                var nivel_desc = Dict_Level().FirstOrDefault(x => x.Key == level).Value;
                                tbx_ListaComentarios.Text += string.Format("{0} -- {1}", nivel_desc,comentario.DeniedReason)  + Environment.NewLine;
                            }
                        }
                    }

                    pVendKey = 0;
                                   

                    if (IsPostBackEventControlRegistered)
                    {
                        HttpContext.Current.Session["Evento"] = null;                       
                    }

                }
                catch (Exception xD)
                {
                    LogError(0, 1, "Facturas_Load", "Error al Cargar Variables de Sesion : " + xD.Message, pCompanyID);
                }
            }

            if (!IsPostBack)
            {                
                // HttpContext.Current.Session["Error"] = "";
            }
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
        }
    }

    protected void save_coment(int expense_id, int userkey, string coment, int level, string company_id, Doc_Tools.DocumentType doc_type)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = string.Format("INSERT INTO {0}DeniedComent ({0}Id, DeniedReason,UpdateUserKey,CreateDate,ApprovalLevel,CompanyId) VALUES (@{0}Id,@DeniedReason,@UpdateUserKey,@CreateDate,@ApprovalLevel,@CompanyId)", doc_type.ToString());
            cmd.Parameters.Add(string.Format("@{0}Id", doc_type), SqlDbType.VarChar).Value = expense_id;
            cmd.Parameters.Add("@DeniedReason", SqlDbType.VarChar).Value = coment;
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = userkey;
            cmd.Parameters.Add("@CreateDate", SqlDbType.Date).Value = DateTime.Now;
            cmd.Parameters.Add("@ApprovalLevel", SqlDbType.Int).Value = level;
            cmd.Parameters.Add("@CompanyId", SqlDbType.VarChar).Value = company_id;
            cmd.Connection.Open();
            var modified = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
    }

    protected List<DeniedComentDTO> read_coments(int expense_id, string company_id, Doc_Tools.DocumentType doc_type)
    {
        List<DeniedComentDTO> lista = new List<DeniedComentDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = string.Format("SELECT ApprovalLevel, DeniedReason FROM {0}DeniedComent  where  {0}Id = @{0}Id and CompanyId = @CompanyId", doc_type.ToString());
            cmd.Parameters.Add(string.Format("@{0}Id",doc_type.ToString()), SqlDbType.VarChar).Value = expense_id;
            cmd.Parameters.Add("@CompanyId", SqlDbType.VarChar).Value = company_id;
            cmd.Connection.Open();
            SqlDataReader lector = cmd.ExecuteReader();
            while (lector.Read())
            {
                var coment = new DeniedComentDTO();
                coment.ApprovalLevel = lector.GetInt32(0);
                coment.DeniedReason = lector.GetString(1);
                lista.Add(coment);
            }
            cmd.Connection.Close();
        }
        return lista;
    }

    protected void btn_GuardarComentario_Click(object sender, EventArgs e)
    {
        string comentario = tbx_coment.Text;
        int expense_id = (int)HttpContext.Current.Session["ExpenseComentId"];
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        int nivel = Dict_Level().FirstOrDefault(x => x.Value == rol).Key;
        var doc_type = (Doc_Tools.DocumentType)HttpContext.Current.Session["DocumentType"];

        if (string.IsNullOrEmpty(comentario))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B23);", true);
            System.Threading.Thread.Sleep(1000);
            return;
        }       

        save_coment(expense_id, pUserKey, tbx_coment.Text, nivel,pCompanyID, doc_type);
        Response.Redirect(Dict_rutas().FirstOrDefault(x=> x.Value == doc_type.ToString()).Key);
    }

    protected void btn_cancel_Click(object sender, EventArgs e)
    {
        var doc_type = (Doc_Tools.DocumentType)HttpContext.Current.Session["DocumentType"];
        Response.Redirect(Dict_rutas().FirstOrDefault(x => x.Value == doc_type.ToString()).Key);
    }
}