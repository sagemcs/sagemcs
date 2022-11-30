using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logged_Administradores_AnticipoEmpleados : System.Web.UI.Page
{
    #region Variables

    private int iVendKey;
    private int iLogKey;
    private int iUserKey;
    private string iCompanyID;
    private decimal daily_amount = 1300;
    private bool is_valid;
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
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
            // ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B6);", true);

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

    private void Vencer_Anticipos(int user_id)
    {        
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Advance SET Status = 4  WHERE AdvanceType = 1 and CheckDate < @CheckDate and UpdateUserKey = @UpdateUserKey";
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;
            cmd.Parameters.Add("@CheckDate", SqlDbType.Date).Value = DateTime.Today;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();            
        }       
    }
   
    protected void Page_Load(object sender, EventArgs e)
    {       
        if (HttpContext.Current.Session["is_valid"] == null)
        {
            HttpContext.Current.Session["is_valid"] = false;
        }    

        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        List<RolDTO> roles = Doc_Tools.get_Roles();
        try
        {
            if (!roles.Any(x => x.ID == HttpContext.Current.Session["RolUser"].ToString()))
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
                    
                    HttpContext.Current.Session["Documento"] = Doc_Tools.DocumentType.Advance;
                    pVendKey = 0;                   

                    tbx_jefe_inmediato.Text = get_JefeInmediato(pUserKey);
                    DateTime hoy = DateTime.Today;
                    tbx_folio.Text =  string.Format("ANT-{0}-{1}/{2}/{3}" ,get_last_AdvanceId(), hoy.Day, hoy.Month, hoy.Year);
                    Vencer_Anticipos(pUserKey);
                    BindGridView();

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

    protected void btnSage_Click(object sender, EventArgs e)
    {
        InsertAdvance();
    }

    protected void btnFinalizar_Click(object sender, EventArgs e)
    {        
        //Logica para lanzar Reporte de Reembolsos
        Response.Redirect("~/Logged/Reports/Anticipos");
    }   

    private int WriteToDb(int AdvanceType,string Folio, decimal Amount,int Currency, DateTime? DepartureDate, DateTime? ArrivalDate,
        DateTime CheckDate, string ImmediateBoss, int UpdateUserKey, string CompanyId, int Status, string motivo)
    {
        try
        {
            string val = "";
            int id = 0;
            int? approval_level = null;

            //set Nivel
            string rol = HttpContext.Current.Session["RolUser"].ToString();
            int role_key = Doc_Tools.get_Roles().FirstOrDefault(x => x.ID == rol).Key;
            if(role_key == 8)
            {
                approval_level = null;
            }
            else 
            {                
                approval_level = Doc_Tools.get_RolesValidadores().FirstOrDefault(x => x.ID == rol).Key;
            }           

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "Insert Into Advance (AdvanceType,Folio,Currency, Amount,DepartureDate,ArrivalDate,CheckDate,ImmediateBoss,UpdateUserKey,CreateDate, UpdateDate,CompanyId,Status, ApprovalLevel, ExpenseReason) VALUES (@AdvanceType,@Folio,@Currency ,@Amount,@DepartureDate,@ArrivalDate,@CheckDate,@ImmediateBoss,@UpdateUserKey, @CreateDate,@UpdateDate,@CompanyId,@Status, @ApprovalLevel, @ExpenseReason); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.Add("@AdvanceType", SqlDbType.Int).Value = AdvanceType;
                cmd.Parameters.Add("@Folio", SqlDbType.NVarChar).Value = Folio;
                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Amount;
                cmd.Parameters.Add("@Currency", SqlDbType.Int).Value = Currency;
                cmd.Parameters.Add("@DepartureDate", SqlDbType.Date).Value = DepartureDate ?? (object)DBNull.Value;
                cmd.Parameters.Add("@ArrivalDate", SqlDbType.Date).Value = ArrivalDate ?? (object)DBNull.Value;
                cmd.Parameters.Add("@CheckDate", SqlDbType.Date).Value = CheckDate;
                cmd.Parameters.Add("@ImmediateBoss", SqlDbType.VarChar).Value = ImmediateBoss;               
                cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = UpdateUserKey;
                cmd.Parameters.Add("@CreateDate", SqlDbType.Date).Value = DateTime.Today;
                cmd.Parameters.Add("@UpdateDate", SqlDbType.Date).Value = DateTime.Today;
                cmd.Parameters.Add("@CompanyId", SqlDbType.NVarChar).Value = CompanyId;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = Status; //Estado inicial Pendiente
                cmd.Parameters.Add("@ApprovalLevel", SqlDbType.Int).Value = approval_level ?? (object)DBNull.Value; 
                cmd.Parameters.Add("@ExpenseReason", SqlDbType.VarChar).Value = motivo ?? (object)DBNull.Value; 
                cmd.Connection.Open();
                var modified = cmd.ExecuteScalar();
                val = modified.ToString();
                id = Convert.ToInt32(val);
                cmd.Connection.Close();
            }

            return id;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Insertar-Anticipo-Empleados:Insertar-Anticipo", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            return -1;
        }
    }

    private void ClearControls()
    {
        tbx_fecha_comprobacion.Text = string.Empty;
        tbx_fecha_llegada.Text = string.Empty;
        tbx_fecha_salida.Text = string.Empty;
        tbx_folio.Text = string.Empty;
        tbx_importe.Text = string.Empty;
        tbx_jefe_inmediato.Text = string.Empty;
        drop_tipo_anticipo.ClearSelection();
        drop_currency.ClearSelection();
        tbx_motivo.Text = string.Empty;
        btnSage.Enabled = false;
    }

    private void BindGridView()
    {
        gvAnticipos.DataSource = null;
        //gvValidacion.DataSource = null;
        //gvValidacion.Visible = false;
        gvAnticipos.Visible = true;
        gvAnticipos.DataSource = ReadFromDb(pUserKey);
        gvAnticipos.DataBind();
    }
   
    public Dictionary<int, string> Dict_type()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>
        {
            { 1, "Viaje" },
            { 2, "Compra Extraordinaria" }           
        };
        return dict;
    }
    private List<AdvanceDTO> ReadFromDb(int user_id)
    {
        List<AdvanceDTO> anticipos = new List<AdvanceDTO>();

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT AdvanceId,AdvanceType,Folio,Currency, Amount,DepartureDate,ArrivalDate,CheckDate,ImmediateBoss,UpdateUserKey,CreateDate, UpdateDate,CompanyId,Status FROM Advance where UpdateUserKey = @UpdateUserKey;";
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {                  
                var advance = new AdvanceDTO();
                advance.AdvanceId = dataReader.GetInt32(0);
                advance.AdvanceType = Dict_type().FirstOrDefault(x=> x.Key== dataReader.GetInt32(1)).Value;
                advance.Folio = dataReader.GetString(2);
                advance.Currency = dataReader.GetInt32(3);
                advance.Amount = dataReader.GetDecimal(4);
                if (!dataReader.IsDBNull(5))
                {
                    advance.DepartureDate = dataReader.GetDateTime(5);
                }
                if(!dataReader.IsDBNull(6))
                {
                    advance.ArrivalDate = dataReader.GetDateTime(6);
                }                
                advance.CheckDate = dataReader.GetDateTime(7);
                advance.ImmediateBoss = dataReader.GetString(8);
                advance.UpdateUserKey = dataReader.GetInt32(9);
                advance.CreateDate = dataReader.GetDateTime(10);
                advance.UpdateDate = dataReader.GetDateTime(11);
                advance.CompanyId = dataReader.GetString(12);
                advance.Status = Doc_Tools.Dict_status().FirstOrDefault(x=> x.Key == dataReader.GetInt32(13)).Value;
                anticipos.Add(advance);
            }
        }

        return anticipos;
    }

    private int get_last_AdvanceId()
    {
        int advance_id = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT top(1) (AdvanceId) FROM Advance order by AdvanceId desc";            
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                advance_id = dataReader.GetInt32(0);        
            }
        }
        return advance_id + 1;
    }

    public void EnviarCorreo()
    {             
        var matrix = Doc_Tools.get_MatrizValidadores(pUserKey, 1);
        string server_address = ConfiguracionCorreoElectronico.server_address;
        int server_port = ConfiguracionCorreoElectronico.server_port;
        string user = ConfiguracionCorreoElectronico.user;
        string password = ConfiguracionCorreoElectronico.password;
        bool enable_ssl = ConfiguracionCorreoElectronico.enable_ssl;
        CorreoElectronico email = new CorreoElectronico(server_address, server_port, user, password, enable_ssl);
        
        var jerarquia = Doc_Tools.get_JerarquiaValidadores(((int)Doc_Tools.DocumentType.Advance));
        var orden = jerarquia.Get_Orden(jerarquia);

        string from = Doc_Tools.getUserEmail(pUserKey);
        string to = string.Empty;
       foreach(var item in orden) if(to == string.Empty)
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
        
        string subject = string.Format("El usuario {0} ha añadido un {1} para su revisión", from, Doc_Tools.DocumentType.Advance.ToString());
        string text = string.Format("El usuario {0} ha añadido un {1} para su revisión", from, Doc_Tools.DocumentType.Advance.ToString());
        bool is_text_html = false;
        email.Enviar(from, to, subject, text, is_text_html);
    }

    private void InsertAdvance()
    {
        is_valid = (bool)HttpContext.Current.Session["is_valid"];
        if (is_valid)
        {
            //Variables a insertar
            int advance_type, currency;
            DateTime? departure_date, arrival_date;
            DateTime check_date;
            decimal amount;
            string inmediate_boss, folio, motivo_gasto;
            int status = 1;

            advance_type = int.Parse(drop_tipo_anticipo.SelectedValue.ToString());
            currency = int.Parse(drop_currency.SelectedValue.ToString());
            folio = tbx_folio.Text;
            departure_date = string.IsNullOrEmpty(tbx_fecha_salida.Text) ? null : (DateTime?)DateTime.Parse(tbx_fecha_salida.Text);
            arrival_date = string.IsNullOrEmpty(tbx_fecha_llegada.Text) ? null : (DateTime?)DateTime.Parse(tbx_fecha_llegada.Text);
            check_date = DateTime.Parse(tbx_fecha_comprobacion.Text);
            inmediate_boss = tbx_jefe_inmediato.Text;
            amount = decimal.Parse(tbx_importe.Text);
            motivo_gasto = tbx_motivo.Text;
            string rol = HttpContext.Current.Session["RolUser"].ToString();
            if (rol != "T|SYS| - Empleado")
            {
                List<RolDTO> roles = Doc_Tools.get_RolesValidadores().ToList();
                if (roles.FirstOrDefault(x => x.ID == rol).Key == roles.Max(z=> z.Key))
                {
                    status = 2;
                }
            }

            int carga = WriteToDb(advance_type, folio, amount, currency, departure_date, arrival_date, check_date, inmediate_boss, pUserKey, pCompanyID, status, motivo_gasto);

            if (carga == -1)
            {
                gvAnticipos.DataSource = null;
                HttpContext.Current.Session["is_valid"] = false;
                gvAnticipos.Visible = false;
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B4);", true);
                System.Threading.Thread.Sleep(5000);
                ClearControls();
                return;
            }
            EnviarCorreo();
            BindGridView();
            ClearControls();
            HttpContext.Current.Session["is_valid"] = false;
        }
        else 
        {
            btnSage.Enabled = false;
        }
        
    }

    protected string get_JefeInmediato(int user_id)
    {
        string jefe_inmediato = string.Empty;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(GefeInmediato,'') as GefeInmediato FROM AspEmpleado where UserKey = @UserKey;";
            cmd.Parameters.Add("@UserKey", SqlDbType.Int).Value = user_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {                
                jefe_inmediato = dataReader.GetString(0);                
            }
        }
        return jefe_inmediato;
    }

    protected void tbx_fecha_llegada_TextChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        DateTime fecha_llegada;
        if (DateTime.TryParse(tbx_fecha_llegada.Text, out fecha_llegada))
        {
            DateTime comprobacion = fecha_llegada.AddDays(5);            
            while (comprobacion.DayOfWeek != DayOfWeek.Tuesday)
            {
                comprobacion = comprobacion.AddDays(1);
            }
            
            tbx_fecha_comprobacion.Text = comprobacion.ToString("D", CultureInfo.CreateSpecificCulture("es-MX"));
        }
        else
        {
            tbx_fecha_comprobacion.Text = string.Empty;
        }
    }

    protected void tbx_importe_TextChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];      
       
    }

    private AdvanceDTO LoadAdvanceById(int advance_id, int user_id)
    {
        var advance = new AdvanceDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT AdvanceId,AdvanceType,Folio,Amount,DepartureDate,ArrivalDate,CheckDate,ImmediateBoss,UpdateUserKey,UpdateDate,CompanyId,Status, CreateDate, ExpenseReason FROM Advance where UpdateUserKey = @UpdateUserKey and AdvanceId = @AdvanceId;";
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;
            cmd.Parameters.Add("@AdvanceId", SqlDbType.Int).Value = advance_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                advance.AdvanceId = dataReader.GetInt32(0);
                advance.AdvanceType = Dict_type().FirstOrDefault(x => x.Key == dataReader.GetInt32(1)).Value;
                advance.Folio = dataReader.GetString(2);
                advance.Amount = dataReader.GetDecimal(3);
                if(!dataReader.IsDBNull(4))
                {
                    advance.DepartureDate = dataReader.GetDateTime(4);
                }
                if (!dataReader.IsDBNull(5))
                {
                    advance.ArrivalDate = dataReader.GetDateTime(5);
                }               
                advance.CheckDate = dataReader.GetDateTime(6);
                advance.ImmediateBoss = dataReader.GetString(7);
                advance.UpdateUserKey = dataReader.GetInt32(8);
                advance.UpdateDate = dataReader.GetDateTime(9);
                advance.CompanyId = dataReader.GetString(10);
                advance.Status = Doc_Tools.Dict_status().FirstOrDefault(x => x.Key == dataReader.GetInt32(11)).Value;
                advance.CreateDate = dataReader.GetDateTime(12);
                advance.ExpenseReason = dataReader.GetString(13);
            }
        }
        return advance;
    }

    private void DeleteAdvance(int advance_id, int user_id)
    {      
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "Delete FROM Advance where UpdateUserKey = @UpdateUserKey and AdvanceId = @AdvanceId;";
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;
            cmd.Parameters.Add("@AdvanceId", SqlDbType.Int).Value = advance_id;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
        }
    }

    protected void gvAnticipos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        BindGridView();
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];
        ClearControls();
        HttpContext.Current.Session["Advance"] = null;
        int rowIndex = int.Parse(e.CommandArgument.ToString());
        GridViewRow row = gvAnticipos.Rows[rowIndex];

        if (e.CommandName == "Select")
        {
            int advance_id = int.Parse(row.Cells[0].Text);
            var advance = LoadAdvanceById(advance_id, pUserKey);
            HttpContext.Current.Session["Advance"] = advance;
            Response.Redirect("EditAnticipo");
        }        
    }

    protected void gvAnticipos_RowDataBound(Object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {            
            if (e.Row.Cells[7].Text != "Pendiente")
            {
                Button btnEdit = (Button)e.Row.Cells[8].Controls[0];
                Button btnDelete = (Button)e.Row.Cells[9].Controls[1];
                btnEdit.Visible = false;
                btnDelete.Visible = false;
            }
        }
    }

    protected void drop_tipo_anticipo_SelectedIndexChanged(object sender, EventArgs e)
    {       
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        switch (drop_tipo_anticipo.SelectedValue)
        {
            //Viaje
            case "1":
                tbx_fecha_salida.Text = string.Empty;
                tbx_fecha_salida.Enabled = true;
                tbx_fecha_llegada.Text = string.Empty;
                tbx_fecha_llegada.Enabled = true;
                tbx_fecha_comprobacion.Text = string.Empty;
                break;
            //Gasto extraordinario
            case "2":
                tbx_fecha_salida.Text = string.Empty;
                tbx_fecha_salida.Enabled = false;
                tbx_fecha_llegada.Text = string.Empty;
                tbx_fecha_llegada.Enabled = false;
                DateTime comprobacion = DateTime.Today.AddDays(5);
                while (comprobacion.DayOfWeek != DayOfWeek.Tuesday)
                {
                    comprobacion = comprobacion.AddDays(1);
                }
                tbx_fecha_comprobacion.Text = comprobacion.ToString("D", CultureInfo.CreateSpecificCulture("es-MX"));               
                break;           
        }
    }

    protected void btn_validar_Click(object sender, EventArgs e)
    {       
        btnSage.Enabled = false;

        //validacion de tipo Anticipo
        if (string.IsNullOrEmpty(drop_tipo_anticipo.SelectedValue.ToString()))
        {            
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B13").Value;          
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Folio
        if (string.IsNullOrEmpty(tbx_folio.Text))
        {           
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B10").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        if (drop_tipo_anticipo.SelectedValue == "1")
        {
            //Fecha de salida
            if (string.IsNullOrEmpty(tbx_fecha_salida.Text))
            {                
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B1").Value;               
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            //Fecha salida mayor-igual a fecha actual
            if (DateTime.Parse(tbx_fecha_salida.Text) < DateTime.Today)
            {                
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B19").Value;               
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            //Fecha de LLegada
            if (string.IsNullOrEmpty(tbx_fecha_llegada.Text))
            {               
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B2").Value;               
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            //Fecha Llegada antes de Fecha salida
            if (DateTime.Parse(tbx_fecha_llegada.Text) < DateTime.Parse(tbx_fecha_salida.Text))
            {               
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B3").Value;              
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
        }

        //validacion de tipo Moneda
        if (string.IsNullOrEmpty(drop_currency.SelectedValue.ToString()))
        {           
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B14").Value;          
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        if (tbx_importe.Text.Any(x => !char.IsDigit(x) && (x != '.') && (x != ',')))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B16").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //validacion de importe del Gasto
        if (string.IsNullOrEmpty(tbx_importe.Text) || decimal.Parse(tbx_importe.Text) <= 0)
        {          
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B15").Value;          
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }
        
        //Jefe inmediato
        if (string.IsNullOrEmpty(tbx_jefe_inmediato.Text))
        {          
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B17").Value;           
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //varios anticipos
        if (ReadFromDb(pUserKey).Count(x => x.Status == "Pendiente") > 1)
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B18").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Variables a insertar
        int advance_type, currency;
        DateTime? departure_date, arrival_date;
        DateTime check_date;
        decimal amount;
        string inmediate_boss, folio;

        advance_type = int.Parse(drop_tipo_anticipo.SelectedValue.ToString());
        currency = int.Parse(drop_currency.SelectedValue.ToString());
        folio = tbx_folio.Text;
        departure_date = string.IsNullOrEmpty(tbx_fecha_salida.Text) ? null : (DateTime?)DateTime.Parse(tbx_fecha_salida.Text);
        arrival_date = string.IsNullOrEmpty(tbx_fecha_llegada.Text) ? null : (DateTime?)DateTime.Parse(tbx_fecha_llegada.Text);
        check_date = DateTime.Parse(tbx_fecha_comprobacion.Text);
        inmediate_boss = tbx_jefe_inmediato.Text;
        amount = decimal.Parse(tbx_importe.Text);

        is_valid = true;
        HttpContext.Current.Session["is_valid"] = is_valid;        

        //validaciones del anticipo
        //               Alimentos Casetas      Gasolina   Total por día
        //1 día    $     600.00    $ 300.00    $  400.00   $  1,300.00
        decimal max_amount = 0;
        //Anticipo de viaje
        if (drop_tipo_anticipo.SelectedValue == "1")
        {
            int travel_days = arrival_date.Value.Subtract(departure_date.Value).Days;
            max_amount = (travel_days + 1) * daily_amount;
        }
        //Anticipo de Gasto extraordinario
        else
        {
            max_amount = 2000;
        }
        if (amount > max_amount)
        {            
            tipo = "warning";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B20").Value;          
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
        if (is_valid)
        {
            btnSage.Enabled = true;
        }
    }

    protected void drop_currency_SelectedIndexChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];
    }

    protected void tbx_fecha_salida_TextChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];
    }

    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        BindGridView();
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];
      
        HttpContext.Current.Session["Advance"] = null;

        int rowIndex = ((System.Web.UI.WebControls.GridViewRow)((System.Web.UI.Control)sender).NamingContainer).RowIndex; //int.Parse(e.CommandArgument.ToString());
        GridViewRow row = gvAnticipos.Rows[rowIndex];
        
        if (e.CommandName == "Delete")
        {
            int advance_id = int.Parse(row.Cells[0].Text);
            DeleteAdvance(advance_id, pUserKey);
            BindGridView();
        }
    }
}