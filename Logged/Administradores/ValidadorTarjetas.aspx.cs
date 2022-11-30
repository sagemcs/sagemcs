//PORTAL DE PROVEDORES T|SYS|
//20 - JULIO, 2022
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : RAFAEL BOZA
//PANTALLA PARA REEMBOLSOS DE EMPLEADOS
//REFERENCIAS UTILIZADAS

using System;
using System.IO;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Linq;
using Proveedores_Model;
using WebSite1;
using System.Text;

public partial class Logged_Administradores_ValidadorTarjetas : System.Web.UI.Page
{
    #region Variables

    private int iVendKey;
    private int iLogKey;
    private int iUserKey;
    private string iCompanyID;    
    private PortalProveedoresEntities _context = new PortalProveedoresEntities();
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

    string eventName = String.Empty;
    string titulo = "T|SYS|", Msj, tipo;

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
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        List<RolDTO> roles = Doc_Tools.get_RolesValidadores().ToList();
        try
        {
            string rol = HttpContext.Current.Session["RolUser"].ToString();
            int level = roles.FirstOrDefault(x => x.ID == rol).Key;
            if (!roles.Any(x => x.ID == HttpContext.Current.Session["RolUser"].ToString()))
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                try
                {
                    //string Var = HttpContext.Current.Session["VendKey"].ToString();
                    //string Var2 = HttpContext.Current.Session["RolUser"].ToString();
                    //string Var3 = HttpContext.Current.Session["IDCompany"].ToString();

                    //pVendKey = Convert.ToInt32(HttpContext.Current.Session["VendKey"].ToString());
                    pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
                    pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
                    pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
                    BindPackageInfo();
                    
                    upPackage.Visible = level <= 1;

                    //BindGridView();
                    if (!IsPostBack)
                    {
                        BindEmpleados();       
                    }
                   
                    pVendKey = 0;
                    //pLogKey = 0;
                    //pUserKey = 0;
                    //pCompanyID = "";

                    //Load_TiposGasto();

                    if (IsPostBackEventControlRegistered)
                    {
                        HttpContext.Current.Session["Evento"] = null;
                        //Sage();
                    }

                }
                catch (Exception xD)
                {
                    LogError(0, 1, "Facturas_Load", "Error al Cargar Variables de Sesion : " + xD.Message, "TSM");
                }
            }

            if (!IsPostBack)
            {

                //gvFacturas.AllowPaging = true;
                // gvFacturas.PageSize = 15;           
                // gvFacturas.AllowSorting = true;
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

    private void BindGridView(int user_id, int? status_id)
    {
        var paquete = get_Package(pUserKey);
        gvGastos.DataSource = null;        
        gvGastos.Visible = true;        
        DateTime? final = !string.IsNullOrEmpty(tbx_fecha_fin.Text) ? (DateTime?)DateTime.Parse(tbx_fecha_fin.Text) : null;
        DateTime? inicio = !string.IsNullOrEmpty(tbx_fecha_inicio.Text) ? (DateTime?)DateTime.Parse(tbx_fecha_inicio.Text) : null;
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        var roles = Doc_Tools.get_RolesValidadores();
        int level = roles.FirstOrDefault(x => x.ID == rol).Key;
        List<CorporateCardDTO> gastos = ReadFromDb(user_id, level).ToList();
        if (inicio!=null)
        {
            gastos = gastos.Where(x => x.CreateDate >= inicio.Value).ToList();
        }
        if(final!=null)
        {
            gastos = gastos.Where(x => x.CreateDate <= final.Value).ToList();
        }
        if(status_id!=0)
        {
            gastos = gastos.Where(x => x.Status == Doc_Tools.Dict_status().FirstOrDefault(d=> d.Key == status_id).Value).ToList();
        }
        gvGastos.DataSource = gastos;
        gvGastos.DataBind();
    }

    private void BindEmpleados()
    {
        List<EmpleadoDTO> empleados = new List<EmpleadoDTO>();
        List<RolDTO> roles = Doc_Tools.get_RolesValidadores().ToList();
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        int level = roles.FirstOrDefault(x => x.ID == rol).Key;
        empleados = Doc_Tools.GetEmpleados(pUserKey, level, Doc_Tools.DocumentType.CorporateCard);

        empleados.Add(new EmpleadoDTO() { UserKey = 0, Nombre = "" });
        drop_empleados.DataSource = empleados.Select(x => new { Id = x.UserKey, Nombre = x.Nombre }).OrderBy(o => o.Id).ToList();
        drop_empleados.DataTextField = "Nombre";
        drop_empleados.DataValueField = "Id";
        drop_empleados.DataBind();
        drop_empleados.SelectedIndex = -1;
    }

    private void BindPackageInfo()
    {        
        var paquete = get_Package(pUserKey);
        tbx_no_paquete.Text = paquete.PackageId.ToString();
        tbx_cant_reembolsos.Text = get_expenses_package(paquete.PackageId).ToString();
        
    }   

    public Dictionary<int, string> Dict_policy()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>
        {
            { 1, "Politica 1" },
            { 2, "Politica 2" },
            { 3, "Politica 3" },
            { 4, "Politica 4" },
            { 5, "Politica 5" },
            { 6, "Politica 6" },
            { 7, "Politica 7" },
            { 8, "Politica 8" }
        };
        return dict;
    }

    private List<CorporateCardDTO> ReadFromDb(int user_id, int level)
    {
        List<CorporateCardDTO> gastos = new List<CorporateCardDTO>();

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CorporateCardId ,Date ,Currency ,Amount, Status, CreateDate, Isnull(DeniedReason,''), Isnull(PackageId,0) , Isnull(ApprovalLevel,0) FROM CorporateCard where UpdateUserKey = @UpdateUserKey;";
                      
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var expense = new CorporateCardDTO();
                expense.CorporateCardId = dataReader.GetInt32(0);             
                expense.Date = dataReader.GetDateTime(1);
                expense.Currency = Doc_Tools.Dict_moneda().First(x => x.Key == dataReader.GetInt32(2)).Value;
                expense.Amount = dataReader.GetDecimal(3);
                expense.Status = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(4)).Value;
                expense.CreateDate = dataReader.GetDateTime(5);
                expense.DeniedReason = dataReader.GetString(6);
                expense.PackageId = dataReader.GetInt32(7);
                expense.ApprovalLevel = dataReader.GetInt32(8);
                gastos.Add(expense);
            }
        }
        HttpContext.Current.Session["Tarjetas"] = gastos;
        return gastos;
    }

    private CorporateCardDTO LoadCardById(int expense_id)
    {
        var card = new CorporateCardDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CorporateCardId ,Date ,Currency ,Amount, Status, FileNameXml ,FileNamePdf ,FileNamePdfVoucher, CompanyId FROM CorporateCard whereCorporateCardId = @CorporateCardId ;";
            cmd.Parameters.Add("@CorporateCardId", SqlDbType.Int).Value = expense_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                card.CorporateCardId = dataReader.GetInt32(0);              
                card.Date = dataReader.GetDateTime(1);
                card.Currency = Doc_Tools.Dict_moneda().First(x => x.Key == dataReader.GetInt32(2)).Value;
                card.Amount = dataReader.GetDecimal(3);
                card.Status = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(4)).Value;
                card.FileNameXml = dataReader.GetString(5);
                card.FileNamePdf = dataReader.GetString(6);
                card.FileNamePdfVoucher = dataReader.GetString(7);
                card.CompanyId = dataReader.GetString(8);
            }
        }
        return card;
    }

    private List<ExpenseDetailDTO> Load_Articles_By_Expense(int expense_id, string company_id)
    {
        var lista = (List<ItemDTO>)HttpContext.Current.Session["Items"];
        var taxes = (List<TaxesDTO>)HttpContext.Current.Session["Taxes"];
        List<ExpenseDetailDTO> articles = new List<ExpenseDetailDTO>();      
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT DetailId,CorporateCardId,ItemKey,Qty,UnitCost, STaxCodeKey,TaxAmount FROM CorporateCardDetail where CorporateCardId = @ExpenseId and CompanyId = @CompanyId;";
            cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = expense_id;
            cmd.Parameters.Add("@CompanyId", SqlDbType.VarChar).Value = company_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var article = new ExpenseDetailDTO();
                article.DetailId = dataReader.GetInt32(0);
                article.ExpenseId = dataReader.GetInt32(1);
                article.ItemKey = dataReader.GetInt32(2);
                article.Qty = dataReader.GetDecimal(3);
                article.UnitCost = dataReader.GetDecimal(4);
                article.ItemId = lista.FirstOrDefault(x => x.ItemKey == article.ItemKey).ItemId;
                article.STaxCodeKey = dataReader.GetInt32(5);
                article.TaxAmount = dataReader.GetDecimal(6);
                article.STaxCodeID = taxes.FirstOrDefault(x => x.STaxCodeKey == article.STaxCodeKey).STaxCodeID;
               
                articles.Add(article);
                
            }
        }
        return articles;
    }

    protected void gvGastos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int rowIndex = ((GridViewRow)((Control)e.CommandSource).BindingContainer).RowIndex;
        GridViewRow row = gvGastos.Rows[rowIndex];
        int card_id = int.Parse(row.Cells[0].Text);
        var paquete = get_Package(pUserKey);
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        var roles = Doc_Tools.get_RolesValidadores();
        int level_validador = roles.FirstOrDefault(x => x.ID == rol).Key;

        int user_id = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);

        if (paquete.PackageId == 0)
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B34").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        if (e.CommandName == "Select")
        {
            int status = 2;
            Update_Expense(card_id, paquete.PackageId, status, string.Empty, level: level_validador);            
            EnviarCorreo(true);
            BindPackageInfo();
            BindGridView(user_id, status_id);
        }
        if (e.CommandName == "Deny")
        {
            int status = 3;
            TextBox motivo = (TextBox)(Control)row.Cells[9].Controls[1];
            if (string.IsNullOrEmpty(motivo.Text))
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B31").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }

            Update_Expense(card_id, paquete.PackageId, status, motivo.Text, level: level_validador);
            EnviarCorreo(false);
            BindPackageInfo();           
            BindGridView(user_id, status_id);
        }
        if (e.CommandName == "Coment")
        {
            HttpContext.Current.Session["ExpenseComentId"] = card_id;
            HttpContext.Current.Session["DocumentType"] = Doc_Tools.DocumentType.CorporateCard;
            Response.Redirect("ComentariosValidador.aspx");
        }
        if (e.CommandName == "Integrate")
        {
            //Coger los datos del documento y lanzar las api Integrar
            var card = LoadCardById(card_id);
            if(!card.SageIntegration)
            {
                var detalles = Load_Articles_By_Expense(card_id, pCompanyID);
                int oRetvalue = Doc_Tools.ExecuteVoucherApi("admin", card, detalles, Doc_Tools.DocumentType.CorporateCard);
                if (oRetvalue == 1)
                {
                    Doc_Tools.CheckSageIntegration(card_id, Doc_Tools.DocumentType.CorporateCard);

                    if (drop_empleados.SelectedItem != null)
                    {
                        user_id = int.Parse(drop_empleados.SelectedItem.Value);
                    }
                    status_id = int.Parse(drop_status.SelectedItem.Value);
                    BindGridView(user_id, status_id);

                    tipo = "success";
                    Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B32").Value;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    return;
                }
                else
                {
                    tipo = "error";
                    Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B33").Value;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    return;
                }
            }
            else 
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B35").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            
        }
    }

    protected void Create_Package(int user_id, string comment)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "update CorporateCardPackage set ClosedAt = GETDATE() where ClosedAt is null and UserId = @UserId";
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = user_id;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                cmd.CommandText = "INSERT INTO CorporateCardPackage (UserId, Description, CreatedAt) VALUES (@UserId, @Description, @CreatedAt); SELECT SCOPE_IDENTITY();";
                //cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = user_id;
                cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = comment;
                cmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.Today;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();               
                cmd.Connection.Close();
            }           
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Crear-Paquete-Tarjeta-Empleados:Crear-Paquete", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;            
        }
    }

    protected void Update_Expense(int card_id, int package_id, int status, string motivo, int level=1)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE CorporateCard SET Status = @Status ");
                if (level == 1)
                {
                    sb.Append(", PackageId = @PackageId ");
                    cmd.Parameters.Add("@PackageId", SqlDbType.Int).Value = package_id;
                }
               
                sb.Append(",UpdateDate = @UpdateDate, DeniedReason = @DeniedReason, ApprovalLevel = @ApprovalLevel  WHERE CorporateCardId = @CorporateCardId;");
                cmd.CommandText = sb.ToString();
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status;               
                cmd.Parameters.Add("@CorporateCardId", SqlDbType.Int).Value = card_id;
                cmd.Parameters.Add("@UpdateDate", SqlDbType.DateTime).Value = DateTime.Today;
                cmd.Parameters.Add("@DeniedReason", SqlDbType.VarChar).Value = motivo;
                cmd.Parameters.Add("@ApprovalLevel", SqlDbType.Int).Value = level;
                cmd.Connection.Open();               
                var modified = cmd.ExecuteNonQuery();               
                cmd.Connection.Close();
            }
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Update-Gasto-Reembolso-Empleados:Update-Gasto", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;           
        }

    }

    protected void drop_empleados_SelectedIndexChanged(object sender, EventArgs e)
    {
        int user_id = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id);
        }

    protected void btn_filtrar_Click(object sender, EventArgs e)
    {
        tbx_fecha_fin.Text = string.Empty;
        tbx_fecha_inicio.Text = string.Empty;
        drop_empleados.SelectedIndex = -1;
        int user_id = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id);
    }

    protected void tbx_fecha_inicio_TextChanged(object sender, EventArgs e)
    {
        int user_id = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id);
    }

    protected void tbx_fecha_fin_TextChanged(object sender, EventArgs e)
    {
        int user_id = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id);
    }

    protected int get_expenses_package(int PackageId)
    {
        int expense_count = 0;
        
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT count(*)  FROM CorporateCard where PackageId = @PackageId;";
            cmd.Parameters.Add("@PackageId", SqlDbType.Int).Value = PackageId;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                expense_count = dataReader.GetInt32(0);                
            }
        }
        return expense_count;
    }

    protected void btn_crear_paquete_Click(object sender, EventArgs e)
    {
        //Logica para crear un paquete
        var paquete = get_Package(pUserKey);
        //diferente de paquete inicial
        if(paquete.PackageId!=0)
        {
            if (get_expenses_package(paquete.PackageId) == 0)
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B30").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }            
        }
        
        Create_Package(pUserKey, "comentario vacio");
        BindPackageInfo();        
        
    }
    
    protected Doc_Tools.Paquete get_Package(int user_id)
    {        
        int PackageId;
        DateTime CreatedAt;
        var paquete = new Doc_Tools.Paquete();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT PackageId, CreatedAt FROM CorporateCardPackage where UserId = @UserId and ClosedAt is null order by CreatedAt desc;";
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = user_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {                
                PackageId = dataReader.GetInt32(0);
                CreatedAt = dataReader.GetDateTime(1);
                paquete.PackageId = PackageId;
                paquete.CreatedAt = CreatedAt;                          
            }
        }
        return paquete;
    }

    protected void drop_status_SelectedIndexChanged(object sender, EventArgs e)
    {
        int user_id = 0;
        if (drop_empleados.SelectedItem != null)
        {
            user_id = int.Parse(drop_empleados.SelectedItem.Value);
        }
        int status_id = int.Parse(drop_status.SelectedItem.Value);
        BindGridView(user_id, status_id);
    }

    public void EnviarCorreo(bool accion)
    {
        string acction_text = accion ? "Aprobado" : "Denegado";
        //Segun el nivel del usuario logueado traer la matriz
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        List<RolDTO> roles = Doc_Tools.get_RolesValidadores().ToList();
        int level = roles.FirstOrDefault(x => x.ID == rol).Key;
        var matrix = Doc_Tools.get_MatrizValidadores(pUserKey, level);
        var jerarquia = Doc_Tools.get_JerarquiaValidadores(((int)Doc_Tools.DocumentType.CorporateCard));
        var orden = jerarquia.Get_Orden(jerarquia);
        string server_address = ConfiguracionCorreoElectronico.server_address;
        int server_port = ConfiguracionCorreoElectronico.server_port;
        string user = ConfiguracionCorreoElectronico.user;
        string password = ConfiguracionCorreoElectronico.password;
        bool enable_ssl = ConfiguracionCorreoElectronico.enable_ssl;
        CorreoElectronico email = new CorreoElectronico(server_address, server_port, user, password, enable_ssl);

        //Tomar el nivel superior que corresponde
        string from = Doc_Tools.getUserEmail(pUserKey);
        string to = string.Empty;
        foreach (var item in orden) if (to == string.Empty)
            {
                if (item.Value == "RecursosHumanos" && matrix.Rh != 0)
                {
                    to = Doc_Tools.getUserEmail(matrix.Rh);
                }
                if (item.Value == "GerenciaArea" && matrix.Gerente != 0)
                {
                    to = Doc_Tools.getUserEmail(matrix.Rh);
                }
                if (item.Value == "CuentasxPagar" && matrix.ValidadorCx != 0)
                {
                    to = Doc_Tools.getUserEmail(matrix.ValidadorCx);
                }
                if (item.Value == "Tesoreria" && matrix.Tesoreria != 0)
                {
                    to = Doc_Tools.getUserEmail(matrix.Tesoreria);
                }
                if (item.Value == "Finanzas" && matrix.Finanzas != 0)
                {
                    to = Doc_Tools.getUserEmail(matrix.Finanzas);
                }

            }
        string subject = string.Format("El usuario {0} ha añadido un {1} para su revisión", from, Doc_Tools.DocumentType.CorporateCard.ToString());
        string text = string.Format("El usuario {0} ha añadido un {1} para su revisión", from, Doc_Tools.DocumentType.CorporateCard.ToString());
        bool is_text_html = false;
        email.Enviar(from, to, subject, text, is_text_html);
    }

    protected void gvGastos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        List<CorporateCardDTO> tarjetas = (List<CorporateCardDTO>)HttpContext.Current.Session["Tarjetas"];
        string rol = HttpContext.Current.Session["RolUser"].ToString();
        List<RolDTO> roles = Doc_Tools.get_RolesValidadores().SkipWhile(x => x.Key == 1).ToList();
        int level = roles.FirstOrDefault(x => x.ID == rol).Key;
        //9 - Aprobar
        //10 - Denegar
        //11 - Motivos Denegacion
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int card_id = int.Parse(e.Row.Cells[0].Text);
            var card = tarjetas.FirstOrDefault(x => x.CorporateCardId == card_id);

            Button btn_aprobar = (Button)e.Row.Cells[7].Controls[1];
            Button btn_denegar = (Button)e.Row.Cells[8].Controls[1];
            TextBox tbx_motivo = (TextBox)e.Row.Cells[9].Controls[1];
            Button btn_comentar = (Button)e.Row.Cells[10].Controls[1];
            Button btn_integrar = (Button)e.Row.Cells[11].Controls[1];

            if (card.ApprovalLevel == roles.Max(x => x.Key) && level == 2)
            {
                btn_integrar.Enabled = true;
            }
            else
            {
                btn_integrar.Enabled = false;
            }
            //3 escenarios a plantear  (CXP) (Tesoreria) (Finanzas)
            switch (level)
            {
                case 2:  //CXP
                    if (level - card.ApprovalLevel == 1 || card.ApprovalLevel == 0)
                    {
                        if (e.Row.Cells[6].Text == "Pendiente")
                        {
                            btn_aprobar.Visible = true;
                            btn_denegar.Visible = true;
                            tbx_motivo.Visible = true;
                            tbx_motivo.ReadOnly = false;

                        }
                        if (e.Row.Cells[6].Text == "Aprobado")
                        {
                            if (card.ApprovalLevel == level)
                            {
                                btn_aprobar.Visible = false;
                                btn_denegar.Visible = false;
                                tbx_motivo.Visible = true;
                                tbx_motivo.ReadOnly = true;
                            }
                            else
                            {
                                btn_aprobar.Visible = true;
                                btn_denegar.Visible = true;
                                tbx_motivo.Visible = true;
                                tbx_motivo.ReadOnly = false;
                            }

                        }
                        if (e.Row.Cells[6].Text == "Denegado")
                        {
                            btn_aprobar.Visible = false;
                            btn_denegar.Visible = false;
                            tbx_motivo.Visible = true;
                            tbx_motivo.ReadOnly = true;
                        }
                        if (e.Row.Cells[6].Text == "Integrado")
                        {
                            btn_aprobar.Visible = false;
                            btn_denegar.Visible = false;
                            btn_comentar.Visible = false;
                            tbx_motivo.Visible = true;
                            tbx_motivo.ReadOnly = true;
                        }
                    }
                    break;
                case 3: case 4: //Tesoreria//Finanzas
                    if (level - card.ApprovalLevel == 1)
                    {
                        if (e.Row.Cells[6].Text == "Pendiente")
                        {
                            btn_aprobar.Visible = true;
                            btn_denegar.Visible = true;
                            tbx_motivo.Visible = true;
                            tbx_motivo.ReadOnly = false;

                        }
                        if (e.Row.Cells[6].Text == "Aprobado")
                        {
                            if (card.ApprovalLevel == level)
                            {
                                btn_aprobar.Visible = false;
                                btn_denegar.Visible = false;
                                tbx_motivo.Visible = true;
                                tbx_motivo.ReadOnly = true;
                            }
                            else
                            {
                                btn_aprobar.Visible = true;
                                btn_denegar.Visible = true;
                                tbx_motivo.Visible = true;
                                tbx_motivo.ReadOnly = false;
                            }

                        }
                        if (e.Row.Cells[6].Text == "Denegado")
                        {
                            btn_aprobar.Visible = false;
                            btn_denegar.Visible = false;
                            tbx_motivo.Visible = true;
                            tbx_motivo.ReadOnly = true;
                        }
                        if (e.Row.Cells[6].Text == "Integrado")
                        {
                            btn_aprobar.Visible = false;
                            btn_denegar.Visible = false;
                            btn_comentar.Visible = false;
                            tbx_motivo.Visible = true;
                            tbx_motivo.ReadOnly = true;
                        }
                    }
                    else
                    {
                        btn_aprobar.Visible = false;
                        btn_comentar.Visible = false;
                        btn_denegar.Visible = false;
                        tbx_motivo.ReadOnly = true;

                        if (e.Row.Cells[6].Text == "Integrado")
                        {
                            btn_integrar.Visible = false;
                            btn_aprobar.Visible = false;
                            btn_denegar.Visible = false;
                            btn_comentar.Visible = false;
                            tbx_motivo.Visible = true;
                            tbx_motivo.ReadOnly = true;
                        }
                    }
                    break;              
            }                 
            
        }
    }
}