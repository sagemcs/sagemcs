using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logged_Administradores_EditAnticipo : System.Web.UI.Page
{
    #region Variables

    private int iVendKey;
    private int iLogKey;
    private int iUserKey;
    private string iCompanyID;
    private decimal daily_amount = 1300;
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
    private bool is_valid;
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
    protected void Page_Load(object sender, EventArgs e)
    {
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

                    pVendKey = 0;

                    if (IsPostBackEventControlRegistered)
                    {
                        HttpContext.Current.Session["Evento"] = null;
                    }

                }
                catch (Exception xD)
                {
                    LogError(0, 1, "Facturas_Load", "Error al Cargar Variables de Sesion : " + xD.Message, "TSM");
                }
            }

            if (!IsPostBack)
            {
                if (HttpContext.Current.Session["Advance"] != null)
                {
                    var advance = (AdvanceDTO)HttpContext.Current.Session["Advance"];
                    tbx_folio.Text = advance.Folio;
                    tbx_importe.Text = advance.Amount.ToString("0.00");
                    drop_tipo_anticipo.SelectedValue = Dict_type().FirstOrDefault(x => x.Value == advance.AdvanceType).Key.ToString();
                    tbx_fecha_salida.Text = advance.DepartureDate != null ? advance.DepartureDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                    tbx_fecha_llegada.Text = advance.ArrivalDate != null ? advance.ArrivalDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                    tbx_fecha_comprobacion.Text = advance.CheckDate.ToString("D", CultureInfo.CreateSpecificCulture("es-MX"));
                    tbx_jefe_inmediato.Text = advance.ImmediateBoss;
                    if(advance.AdvanceType == "Compra Extraordinaria")
                    {
                        tbx_fecha_llegada.Enabled = false;
                        tbx_fecha_salida.Enabled = false;
                    }
                    tbx_motivo.Text = advance.ExpenseReason;
                }
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
            cmd.CommandText = "SELECT AdvanceId,AdvanceType,Folio,Amount,DepartureDate,ArrivalDate,CheckDate,ImmediateBoss,UpdateUserKey,UpdateDate,CompanyId,Status FROM Advance where UpdateUserKey = @UpdateUserKey;";
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var advance = new AdvanceDTO();
                advance.AdvanceId = dataReader.GetInt32(0);
                advance.AdvanceType = Dict_type().FirstOrDefault(x => x.Key == dataReader.GetInt32(1)).Value;
                advance.Folio = dataReader.GetString(2);
                advance.Amount = dataReader.GetDecimal(3);
                if(!dataReader.IsDBNull(4))
                {
                    advance.DepartureDate = dataReader.GetDateTime(4);
                }
                if(!dataReader.IsDBNull(5))
                {
                    advance.ArrivalDate =  dataReader.GetDateTime(5) ;
                }                             
                advance.CheckDate = dataReader.GetDateTime(6);
                advance.ImmediateBoss = dataReader.GetString(7);
                advance.UpdateUserKey = dataReader.GetInt32(8);
                advance.UpdateDate = dataReader.GetDateTime(9);
                advance.CompanyId = dataReader.GetString(10);
                advance.Status = Doc_Tools.Dict_status().FirstOrDefault(x => x.Key == dataReader.GetInt32(11)).Value;
                anticipos.Add(advance);
            }
        }

        return anticipos;
    }

    private int WriteToDb(int AdvanceId,int AdvanceType, decimal Amount, DateTime? DepartureDate, DateTime? ArrivalDate,
        DateTime CheckDate, string ImmediateBoss, int UpdateUserKey, string CompanyId, int Status, string motivo_gasto)
    {
        try
        {           
            int id = 0;
                  
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE Advance  SET AdvanceType = @AdvanceType, Amount = @Amount,DepartureDate = @DepartureDate,ArrivalDate = @ArrivalDate,CheckDate = @CheckDate,ImmediateBoss = @ImmediateBoss,UpdateUserKey = @UpdateUserKey,UpdateDate = @UpdateDate,CompanyId = @CompanyId, Status = @Status, ExpenseReason = @ExpenseReason Where UpdateUserKey=@UpdateUserKey and AdvanceId=@AdvanceId;";
                cmd.Parameters.Add("@AdvanceType", SqlDbType.Int).Value = AdvanceType;                
                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Amount;
                cmd.Parameters.Add("@DepartureDate", SqlDbType.Date).Value = DepartureDate ?? (object)DBNull.Value;
                cmd.Parameters.Add("@ArrivalDate", SqlDbType.Date).Value = ArrivalDate ?? (object)DBNull.Value;
                cmd.Parameters.Add("@CheckDate", SqlDbType.Date).Value = CheckDate;
                cmd.Parameters.Add("@ImmediateBoss", SqlDbType.VarChar).Value = ImmediateBoss;
                cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = UpdateUserKey;
                cmd.Parameters.Add("@UpdateDate", SqlDbType.Date).Value = DateTime.Today;
                cmd.Parameters.Add("@CompanyId", SqlDbType.NVarChar).Value = CompanyId;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = Status; //Estado inicial Pendiente
                cmd.Parameters.Add("@AdvanceId", SqlDbType.Int).Value = AdvanceId;
                cmd.Parameters.Add("@ExpenseReason", SqlDbType.VarChar).Value = motivo_gasto ?? (object)DBNull.Value;
                cmd.Connection.Open();
                var modified = cmd.ExecuteNonQuery();                
                cmd.Connection.Close();
            }

            return id;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Actualizar-Anticipo-Empleados:Actualizar-Anticipo", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            return -1;
        }
    }

    private void InsertAdvance()
    {
        is_valid = (bool)HttpContext.Current.Session["is_valid"];
        if(is_valid)
        {
            //Variables a insertar
            int advance_type, status;
            DateTime? departure_date, arrival_date;
            DateTime check_date;
            decimal amount;
            string inmediate_boss, motivo_gasto;

            advance_type = int.Parse(drop_tipo_anticipo.SelectedValue.ToString());
          
            departure_date = string.IsNullOrEmpty(tbx_fecha_salida.Text) ? null : (DateTime?)DateTime.Parse(tbx_fecha_salida.Text);
            arrival_date = string.IsNullOrEmpty(tbx_fecha_llegada.Text) ? null : (DateTime?)DateTime.Parse(tbx_fecha_llegada.Text);
            check_date = DateTime.Parse(tbx_fecha_comprobacion.Text);
            inmediate_boss = tbx_jefe_inmediato.Text;
            amount = decimal.Parse(tbx_importe.Text);
            motivo_gasto = tbx_motivo.Text;

            var advance = (AdvanceDTO)HttpContext.Current.Session["Advance"];
            int carga = WriteToDb(advance.AdvanceId, advance_type, amount, departure_date, arrival_date, check_date, inmediate_boss, pUserKey, pCompanyID, status = 1, motivo_gasto);

            if (carga == -1)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B4);", true);
                System.Threading.Thread.Sleep(5000);
                return;
            }
            Response.Redirect("AnticipoEmpleados");
        }
        else 
        {
            btn_Guardar.Enabled = false;
        }
        
    }

    protected void btn_Guardar_Click(object sender, EventArgs e)
    {
        InsertAdvance();
    }   

    protected void tbx_importe_TextChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btn_Guardar.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        //if (tbx_importe.Text.All(x=> !char.IsDigit(x) && (x != '.') && (x != ',')))
        //{
        //    tbx_importe.Text = string.Empty;
        //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B16);", true);
        //    System.Threading.Thread.Sleep(5000);
        //    return;
        //}        
    }

    protected void tbx_fecha_llegada_TextChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btn_Guardar.Enabled = (bool)HttpContext.Current.Session["is_valid"];

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

    protected void btn_Cancelar_Click(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btn_Guardar.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        Response.Redirect("AnticipoEmpleados");
    }

    protected void drop_tipo_anticipo_SelectedIndexChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btn_Guardar.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        var advance = (AdvanceDTO)HttpContext.Current.Session["Advance"];
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
                DateTime comprobacion = advance.CreateDate.AddDays(5);
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
        btn_Guardar.Enabled = false;
        //Validar Informacion y habilitar Guardar

        //validacion de tipo Anticipo
        if (string.IsNullOrEmpty(drop_tipo_anticipo.SelectedValue.ToString()))
        {            
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB26").Value;           
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }
        //validacion de importe del Gasto
        if (string.IsNullOrEmpty(tbx_importe.Text))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B15").Value;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        if (drop_tipo_anticipo.SelectedValue == "1")
        {
            //Fecha de salida
            if (string.IsNullOrEmpty(tbx_fecha_salida.Text))
            {               
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B1").Value;              
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            //Fecha salida mayor-igual a fecha actual
            //if (DateTime.Parse(tbx_fecha_salida.Text) < DateTime.Today)
            //{               
            //    tipo = "error";
            //    Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B3").Value;              
            //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            //    return;
            //}
            //Fecha de LLegada
            if (string.IsNullOrEmpty(tbx_fecha_llegada.Text))
            {               
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B2").Value;               
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            //Fecha Llegada antes de Fecha salida
            if (DateTime.Parse(tbx_fecha_llegada.Text) < DateTime.Parse(tbx_fecha_salida.Text))
            {              
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B3").Value;               
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
        }       
       
        //Jefe inmediato
        if (string.IsNullOrEmpty(tbx_jefe_inmediato.Text))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B17").Value;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //varios anticipos
        if (ReadFromDb(pUserKey).Count(x => x.Status == "Vencido") > 0)
        {           
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B18").Value;           
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Variables a insertar
        int advance_type;
        DateTime? departure_date, arrival_date;
        DateTime check_date;
        decimal amount;
        string inmediate_boss;

        advance_type = int.Parse(drop_tipo_anticipo.SelectedValue.ToString());        
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
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B20").Value;           
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }
        if (is_valid)
        {
            btn_Guardar.Enabled = true;
        }
    }

    protected void tbx_fecha_salida_TextChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btn_Guardar.Enabled = (bool)HttpContext.Current.Session["is_valid"];
    }
}