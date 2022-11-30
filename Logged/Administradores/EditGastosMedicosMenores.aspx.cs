using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using WebSite1;
public partial class Logged_Administradores_EditGastosMedicosMenores : System.Web.UI.Page
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
    private bool is_valid;
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
                    MultiView1.SetActiveView(View_General);

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
                get_items(pCompanyID);
                get_taxes();
                HttpContext.Current.Session["GridList"] = null;
                HttpContext.Current.Session["GridTaxes"] = null;
                if (HttpContext.Current.Session["MinorMedicalExpense"] != null)
                {
                    var expense = (MinorMedicalExpenseDTO)HttpContext.Current.Session["MinorMedicalExpense"];
                    tbx_importe.Text = expense.Amount.ToString("0.00");
                    tbx_fechagasto.Text = expense.Date.ToString("yyyy-MM-dd");                    
                    tbx_pdf.Text = expense.FileNamePdf;
                    tbx_xml.Text = expense.FileNameXml;
                }
            }
            if (HttpContext.Current.Session["MinorMedicalExpense"] != null)
            {
                var expense = (MinorMedicalExpenseDTO)HttpContext.Current.Session["MinorMedicalExpense"];
                //tbx_importe.Text = expense.Amount.ToString();
                //tbx_fechagasto.Text = expense.Date.ToString("yyyy-MM-dd");                
                Load_Articles_By_Expense(expense.MinorMedicalExpenseId, pUserKey, pCompanyID);
                //tbx_pdf.Text = expense.FileNamePdf;
                //tbx_xml.Text = expense.FileNameXml;
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

    private void Load_Articles_By_Expense(int expense_id, int user_id, string company_id)
    {
        var lista = (List<ItemDTO>)HttpContext.Current.Session["Items"];
        var taxes = (List<TaxesDTO>)HttpContext.Current.Session["Taxes"];
        List<ExpenseDetailDTO> articles = new List<ExpenseDetailDTO>();    
        if(HttpContext.Current.Session["GridList"]!=null)
        {
            articles = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];
        }
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT DetailId,MinorMedicalExpenseId,ItemKey,Qty,UnitCost, STaxCodeKey,TaxAmount FROM MinorMedicalExpenseDetail where MinorMedicalExpenseId = @MinorMedicalExpenseId and CreateUser = @CreateUser and CompanyId = @CompanyId;";
            cmd.Parameters.Add("@CreateUser", SqlDbType.Int).Value = user_id;
            cmd.Parameters.Add("@MinorMedicalExpenseId", SqlDbType.Int).Value = expense_id;
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
                if (!articles.Any(x=> x.ItemKey == article.ItemKey))
                {
                    articles.Add(article);
                }       
                
            }
        }
        GvItems.DataSource = null;
        GvItems.DataSource = articles;
        HttpContext.Current.Session["GridList"] = articles;
        GvItems.DataBind();
    }

    private void get_taxes()
    {
        List<TaxesDTO> lista = Doc_Tools.get_taxes(pCompanyID);
        drop_taxes.DataSource = lista.ToList();
        drop_taxes.DataTextField = "STaxCodeID";
        drop_taxes.DataValueField = "STaxCodeKey";
        drop_taxes.DataBind();
        drop_taxes.SelectedIndex = -1;
        HttpContext.Current.Session["Taxes"] = null;
        HttpContext.Current.Session["Taxes"] = lista;

    }

    private void get_items(string company_id)
    {
        List<ItemDTO> lista = Doc_Tools.get_items(company_id);
        drop_articulos.DataSource = lista;
        drop_articulos.DataTextField = "ItemId";
        drop_articulos.DataValueField = "ItemKey";
        drop_articulos.DataBind();
        drop_articulos.SelectedIndex = -1;
        HttpContext.Current.Session["Items"] = null;
        HttpContext.Current.Session["Items"] = lista;


    }

    public Dictionary<int, string> Dict_status()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>
        {
            { 1, "Pendiente" },
            { 2, "Aprobado" },
            { 3, "Cancelado" }
        };
        return dict;
    }
      

    private bool CompruebaMontoFactura(MemoryStream fs, decimal importe)
    {
        try
        {
            StreamReader streamReader = null;
            TextReader reader = null;
            XmlSerializer Xmls = null;

            uCFDsLib.v33.Comprobante Factura = new uCFDsLib.v33.Comprobante();
            uCFDsLib.v40.Comprobante Facturas = new uCFDsLib.v40.Comprobante();

            try
            {

                string xmlOutput = string.Empty;
                fs.Position = 0;
                streamReader = new StreamReader(fs);
                xmlOutput = streamReader.ReadToEnd();
                streamReader.Close();
                reader = new StringReader(xmlOutput);

                try
                {
                    Xmls = new XmlSerializer(Facturas.GetType());
                    Facturas = (uCFDsLib.v40.Comprobante)Xmls.Deserialize(reader);
                    if (Facturas.Total == importe)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        reader = new StringReader(xmlOutput);
                        Xmls = new XmlSerializer(Factura.GetType());
                        Factura = (uCFDsLib.v33.Comprobante)Xmls.Deserialize(reader);
                        if (Factura.Total == importe)
                        {
                            return true;
                        }
                    }
                    catch (Exception exs)
                    {
                        //LOG Err
                        HttpContext.Current.Session["Error"] = "Tu archivo no tiene la estructura valida por el SAT.";
                        string Mensaje = "Error al Deserializar el Archivo ";
                        Mensaje = Mensaje + exs.Message;
                        if (exs.InnerException != null)
                        {
                            Mensaje = Mensaje + " || " + exs.InnerException;
                        }
                        LogError(iLogKey, iUserKey, "Carga de Factura_CargarXML()", Mensaje, iCompanyID);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                //LOG Err
                HttpContext.Current.Session["Error"] = "Tu archivo no tiene la estructura valida por el SAT.";
                string Mensaje = "Error al Deserializar el Archivo ";
                Mensaje = Mensaje + ex.Message;
                if (ex.InnerException != null)
                {
                    Mensaje = Mensaje + " || " + ex.InnerException;
                }
                LogError(iLogKey, iUserKey, "Carga de Factura_CargarXML()", Mensaje, iCompanyID);
                return false;
            }
        }
        catch (Exception ex)
        {
            string err;
            err = ex.Message;
            if (HttpContext.Current.Session["Error"].ToString() == "")
            {
                err = err + HttpContext.Current.Session["Error"].ToString();
            }
            LogError(pLogKey, pUserKey, "Carga-Factura:btnSage_Click", err, pCompanyID);

            if (HttpContext.Current.Session["Error"].ToString() == "")
            {
                HttpContext.Current.Session["Error"] = err;
            }
            return false;
        }
        return false;
    }

    protected void btnSage_Click(object sender, EventArgs e)
    {
        is_valid = (bool)HttpContext.Current.Session["is_valid"];
        if (is_valid)
        {
            DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);
            decimal importe_gasto = decimal.Parse(tbx_importe.Text);

            var medical_expense = (MinorMedicalExpenseDTO)HttpContext.Current.Session["MinorMedicalExpense"];
            var lista_detalles = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];
            
            var result = WriteToDb(medical_expense.MinorMedicalExpenseId, fecha_gasto, importe_gasto, fu_xml, fu_pdf, pUserKey, pCompanyID, lista_detalles);
            if (result == -1)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B4);", true);
                System.Threading.Thread.Sleep(5000);
                btnSage.Enabled = false;
                MultiView1.SetActiveView(View_General);
                return;
            }
            HttpContext.Current.Session["is_valid"] = false;
            Response.Redirect("GastosMedicosMenoresEmpleados");
            
        }
        else
        {
            btnSage.Enabled = false;
        }
        
    }

    private int WriteToDb(int medical_expense_id, DateTime fecha_gasto, decimal importe_gasto, FileUpload fu_xml, FileUpload fu_pdf, int userkey, string companyId, List<ExpenseDetailDTO> expenseDetails)
    {
        int id = 0;
        string xml_filename = string.Empty;
        string pdf_filename = string.Empty;
        try
        {
            
            byte[] bytes_xml = new byte[0];
            if (fu_xml != null)
            {
                if (fu_xml.HasFile)
                {
                    Stream fs_xml = fu_xml.PostedFile.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs_xml);
                    bytes_xml = (byte[])HttpContext.Current.Session["fu_xml_bytes"];
                    xml_filename = fu_xml.FileName;                   
                }
            }
           
            byte[] bytes_pdf = new byte[0];
            if (fu_pdf != null)
            {
                if (fu_pdf.HasFile)
                {
                    Stream fs_pdf = fu_pdf.PostedFile.InputStream;
                    System.IO.BinaryReader br2 = new System.IO.BinaryReader(fs_pdf);
                    bytes_pdf = (byte[])HttpContext.Current.Session["fu_pdf_bytes"];
                    pdf_filename = fu_pdf.FileName;                    
                }
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = conn.CreateCommand();
                StringBuilder sb = new StringBuilder();
                //Cadena Inicial
                sb.Append("UPDATE MinorMedicalExpense SET Date = @Date, Amount = @Amount");

                if (fu_xml.HasFile)
                {
                    sb.Append(", FileBinaryXml = @FileBinaryXml");
                    cmd.Parameters.Add("@FileBinaryXml", SqlDbType.VarBinary, bytes_xml.Length).Value = bytes_xml;
                    sb.Append(", FileNameXml = @FileNameXml");                    
                    cmd.Parameters.Add("@FileNameXml", SqlDbType.VarChar).Value = xml_filename;
                }
                if (fu_pdf.HasFile)
                {
                    sb.Append(", FileBinaryPdf = @FileBinaryPdf");
                    cmd.Parameters.Add("@FileBinaryPdf", SqlDbType.VarBinary, bytes_pdf.Length).Value = bytes_pdf;
                    sb.Append(", FileNamePdf = @FileNamePdf");
                    cmd.Parameters.Add("@FileNamePdf", SqlDbType.VarChar).Value = pdf_filename;
                }
                sb.Append(", UpdateDate = @UpdateDate Where UpdateUserKey = @UpdateUserKey and MinorMedicalExpenseId = @MinorMedicalExpenseId;");
                  cmd.CommandText = sb.ToString();
                
                cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = fecha_gasto;                
                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = importe_gasto;
                cmd.Parameters.Add("@UpdateDate", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = userkey;
                cmd.Parameters.Add("@MinorMedicalExpenseId", SqlDbType.Int).Value = medical_expense_id;
                cmd.Connection.Open();
                var modified = cmd.ExecuteScalar();

                foreach (ExpenseDetailDTO detail in expenseDetails)
                {
                    if (detail.DetailId == null)
                    {                        
                        cmd.Parameters.Clear();
                        cmd.CommandText = "INSERT INTO MinorMedicalExpenseDetail (MinorMedicalExpenseId,ItemKey,Qty,UnitCost,Amount,CreateDate,UpdateDate,CreateUser,CompanyId, STaxCodeKey,TaxAmount) VALUES (@_ExpenseId,@_ItemKey,@_Qty,@_UnitCost,@_Amount,@_CreateDate,@_UpdateDate,@_CreateUser,@_CompanyId, @_STaxCodeKey, @_TaxAmount);";
                        cmd.Parameters.Add("@_ExpenseId", SqlDbType.Int).Value = medical_expense_id;
                        cmd.Parameters.Add("@_ItemKey", SqlDbType.Int).Value = detail.ItemKey;
                        cmd.Parameters.Add("@_Qty", SqlDbType.Decimal).Value = detail.Qty;
                        cmd.Parameters.Add("@_UnitCost", SqlDbType.Decimal).Value = detail.UnitCost;
                        cmd.Parameters.Add("@_Amount", SqlDbType.Decimal).Value = detail.Amount;
                        cmd.Parameters.Add("@_CreateDate", SqlDbType.DateTime).Value = DateTime.Now;
                        cmd.Parameters.Add("@_UpdateDate", SqlDbType.DateTime).Value = DateTime.Now;
                        cmd.Parameters.Add("@_CreateUser", SqlDbType.Int).Value = userkey;
                        cmd.Parameters.Add("@_CompanyId", SqlDbType.VarChar).Value = companyId;
                        cmd.Parameters.Add("@_STaxCodeKey", SqlDbType.Decimal).Value = detail.STaxCodeKey;
                        cmd.Parameters.Add("@_TaxAmount", SqlDbType.Decimal).Value = detail.TaxAmount;
                        var inserted = cmd.ExecuteScalar();                        
                    }
                }
                
                cmd.Connection.Close();
            }

            return id;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Actualizar-Pago-GastosMedicos-Empleados:Actualizar-Pago", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            return -1;
        }
    }

    protected void GvItems_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //Borrar articulo del grid   
        int rowIndex = int.Parse(e.CommandArgument.ToString());
        GridViewRow row = GvItems.Rows[rowIndex];

        if (e.CommandName == "Select")
        {
            int itemKey = int.Parse(row.Cells[0].Text);
            var lista = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];
            var detail = lista.FirstOrDefault(x => x.ItemKey == itemKey);
            lista.Remove(detail);
            if(detail.DetailId!=null)
            {
                if(check_article(detail.DetailId.Value))
                {
                    delete_article(detail.DetailId.Value);
                }
            }
            HttpContext.Current.Session["GridList"] = lista;
            GvItems.DataSource = lista;
            GvItems.DataBind();
            tbx_importe.Text = lista.Sum(x => x.Amount + x.TaxAmount).ToString("0.00");
        }
    }

    protected void GvItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void btn_additem_Click(object sender, EventArgs e)
    {
        //validaciones
        if (drop_articulos.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B18);", true);
            System.Threading.Thread.Sleep(5000);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        
        //validacion del importe del articulo
        if (tbx_importegasto.Text == string.Empty)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B20);", true);
            System.Threading.Thread.Sleep(5000);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        else
        {
            foreach (char d in tbx_importegasto.Text.ToArray())
            {
                if (!char.IsControl(d) && !char.IsDigit(d) && (d != '.') && (d != ','))
                {
                    tbx_importegasto.Text = string.Empty;
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B16);", true);
                    System.Threading.Thread.Sleep(5000);
                    MultiView1.SetActiveView(View_Articulos);
                    return;
                }
            }
            if (int.Parse(tbx_importegasto.Text) == 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B20);", true);
                System.Threading.Thread.Sleep(5000);
                MultiView1.SetActiveView(View_Articulos);
                return;
            }
        }
       
        //Validacion de la cantidad
        if (tbx_cantidad.Text == string.Empty)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B19);", true);
            System.Threading.Thread.Sleep(5000);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        else
        {
            foreach (char d in tbx_cantidad.Text.ToArray())
            {
                if (!char.IsControl(d) && !char.IsDigit(d) && (d != '.') && (d != ','))
                {
                    tbx_cantidad.Text = string.Empty;
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B16);", true);
                    System.Threading.Thread.Sleep(5000);
                    MultiView1.SetActiveView(View_Articulos);
                    return;
                }
            }
            if (int.Parse(tbx_cantidad.Text) == 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B19);", true);
                System.Threading.Thread.Sleep(5000);
                MultiView1.SetActiveView(View_Articulos);
                return;
            }
        }
        //validaciones taxes
        
        //Lista de articulos
        var items = (List<ItemDTO>)HttpContext.Current.Session["Items"];
        var taxes = (List<TaxesDTO>)HttpContext.Current.Session["Taxes"];

        //Agregar articulo al grid
        var detalle = new ExpenseDetailDTO();
        detalle.ItemKey = int.Parse(drop_articulos.SelectedItem.Value);
        detalle.ItemId = drop_articulos.SelectedItem.Text;
        detalle.Qty = decimal.Parse(tbx_cantidad.Text);
        detalle.UnitCost = decimal.Parse(tbx_importegasto.Text);
        if (drop_taxes.SelectedValue != "0")
        {
            detalle.STaxCodeKey = int.Parse(drop_taxes.SelectedItem.Value);
            detalle.STaxCodeID = drop_taxes.SelectedItem.Text;
            detalle.TaxAmount = taxes.FirstOrDefault(x => x.STaxCodeKey == detalle.STaxCodeKey).Rate * detalle.Amount;
        }

        if (HttpContext.Current.Session["GridList"] != null)
        {
            var lista = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];
            if (!lista.Any(x => x.ItemKey == detalle.ItemKey))
            {
                lista.Add(detalle);
            }
            HttpContext.Current.Session["GridList"] = lista;
            GvItems.DataSource = null;
            GvItems.DataSource = lista;
            GvItems.DataBind();
            tbx_importe.Text = lista.Sum(x => x.Amount + x.TaxAmount).ToString("0.00");
        }
        else
        {
            var lista = new List<ExpenseDetailDTO>
            {
                detalle
            };
            HttpContext.Current.Session["GridList"] = lista;
            GvItems.DataSource = null;
            GvItems.DataSource = lista;
            GvItems.DataBind();
            tbx_importe.Text = lista.Sum(x => x.Amount + x.TaxAmount).ToString("0.00");
        }

        //Limpiar controles
        drop_articulos.ClearSelection();
        tbx_cantidad.Text = string.Empty;
        tbx_importegasto.Text = string.Empty;
        drop_taxes.ClearSelection();        
        MultiView1.SetActiveView(View_General);

    }

    protected void tbx_fechagasto_TextChanged(object sender, EventArgs e)
    {
        //Solo se admiten gastos del mes en curso
        DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);
        if (DateTime.Today.Year == fecha_gasto.Year)
        {
            if (DateTime.Today.Month != fecha_gasto.Month)
            {
                tbx_fechagasto.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B17);", true);
                System.Threading.Thread.Sleep(5000);
                return;
            }
        }
        else
        {
            tbx_fechagasto.Text = string.Empty;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B17);", true);
            System.Threading.Thread.Sleep(5000);
            return;
        }
    }
   

    protected void btn_new_article_Click(object sender, EventArgs e)
    {
        MultiView1.SetActiveView(View_Articulos);
    }

    protected void tbx_tax_amount_TextChanged(object sender, EventArgs e)
    {
        
    }   

    protected bool check_article(int id)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "Select count(*) FROM MinorMedicalExpenseDetail WHERE DetailId = @ExpenseId;";
            cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = id;
            cmd.Connection.Open();
            int count = (int)cmd.ExecuteScalar();
            cmd.Connection.Close();
            if (count > 0)
            {
                return true;
            }
            return false;
        }
    }

    protected void delete_article(int id)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM MinorMedicalExpenseDetail WHERE DetailId = @ExpenseId;";
            cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = id;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
    }
    

    protected void btn_cancelar_item_Click(object sender, EventArgs e)
    {
        //Limpiar controles
        drop_articulos.ClearSelection();
        tbx_cantidad.Text = string.Empty;
        tbx_importegasto.Text = string.Empty;
        MultiView1.SetActiveView(View_General);
    }

    public void fill_fileUploads()
    {
        if (HttpContext.Current.Session["fu_pdf"] != null)
        {
            var f_pdf = (FileUpload)HttpContext.Current.Session["fu_pdf"];
            tbx_pdf.Text = f_pdf.FileName;
        }

        if (HttpContext.Current.Session["fu_xml"] != null)
        {
            var f_xml = (FileUpload)HttpContext.Current.Session["fu_xml"];
            tbx_xml.Text = f_xml.FileName;
        }
      
    }

    protected void View_General_Activate(object sender, EventArgs e)
    {
        fill_fileUploads();
    }

    protected void View_General_Deactivate(object sender, EventArgs e)
    {
        if (fu_xml.HasFile)
        {
            HttpContext.Current.Session["fu_xml"] = fu_xml;
            HttpContext.Current.Session["fu_xml_bytes"] = fu_xml.FileBytes;
        }
        if (fu_pdf.HasFile)
        {
            HttpContext.Current.Session["fu_pdf"] = fu_pdf;
            HttpContext.Current.Session["fu_pdf_bytes"] = fu_pdf.FileBytes;
        }        
    }

    protected void btn_validar_Click(object sender, EventArgs e)
    {
        btnSage.Enabled = false;
        is_valid = false;

        //validacion de fecha
        if (string.IsNullOrEmpty(tbx_fechagasto.Text))
        {
            MultiView1.SetActiveView(View_General);

            string titulo, Msj, tipo;
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB24").Value;
            titulo = "T|SYS|";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //validacion de importe del Gasto
        if (string.IsNullOrEmpty(tbx_importe.Text))
        {
            MultiView1.SetActiveView(View_General);

            string titulo, Msj, tipo;
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B15").Value;
            titulo = "T|SYS|";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }
        //Si se subio archivo PDF
        if (fu_pdf.HasFile)
        {
            //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B2);", true);
            //System.Threading.Thread.Sleep(5000);
            //return;
            if (fu_pdf.PostedFile.ContentType.ToString() != "application/pdf")
            {
                MultiView1.SetActiveView(View_General);

                string titulo, Msj, tipo;
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B8").Value;
                titulo = "T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }

            if (fu_pdf.PostedFile.ContentLength > 1000000 * 15)
            {
                MultiView1.SetActiveView(View_General);

                string titulo, Msj, tipo;
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB27").Value;
                titulo = "T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
        }

        //Escribir info en BD

        DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);
        decimal importe_gasto = decimal.Parse(tbx_importe.Text);

        if (fu_xml.HasFile)
        {
            //Validación del Formato del Archivo
            if (fu_xml.PostedFile.ContentType.ToString() != "text/xml")
            {
                MultiView1.SetActiveView(View_General);

                string titulo, Msj, tipo;
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B7").Value;
                titulo = "T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            //Validación del Tamaño
            if (fu_xml.PostedFile.ContentLength > 1000000 * 15)
            {
                MultiView1.SetActiveView(View_General);

                string titulo, Msj, tipo;
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB30").Value;
                titulo = "T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }

            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(fu_xml.FileBytes, 0, fu_xml.FileBytes.Length);
            //Validacion de importe
            if (!CompruebaMontoFactura(memoryStream, importe_gasto))
            {
                MultiView1.SetActiveView(View_General);

                string titulo, Msj, tipo;
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B11").Value;
                titulo = "T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
        }
        fill_fileUploads();

        var medical_expense = (MinorMedicalExpenseDTO)HttpContext.Current.Session["MinorMedicalExpense"];
        var lista_detalles = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];
       

        if (lista_detalles == null)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B24);", true);
            System.Threading.Thread.Sleep(5000);
            return;

            string titulo, Msj, tipo;
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB41").Value;
            titulo = "T|SYS|";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Validaciones del Importe y Articulos - Impuestos
        if (importe_gasto != lista_detalles.Sum(x => x.Amount + x.TaxAmount))
        {
            string titulo, Msj, tipo;
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB40").Value;
            titulo = "T|SYS|";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Solo alertas sin retorno
        is_valid = true;
        string tituloo, tipoo;
        tipoo = "error";
        tituloo = "T|SYS|";
        string Msjj = "";
        HttpContext.Current.Session["is_valid"] = is_valid;

        if (HttpContext.Current.Session["fu_xml"] != null)
        {
            fu_xml = (FileUpload)HttpContext.Current.Session["fu_xml"];
        }

        if (!fu_xml.HasFile)
        {
            Msjj += Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB31").Value + "\n";
        }

        if (HttpContext.Current.Session["fu_pdf"] != null)
        {
            fu_pdf = (FileUpload)HttpContext.Current.Session["fu_pdf"];
        }

        if (!fu_pdf.HasFile)
        {
            Msjj += Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB32").Value + "\n";
        }

        if (Msjj != "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + tituloo + "','" + Msjj + "','" + tipoo + "');", true);
        }

        if (is_valid)
        {
            btnSage.Enabled = true;
        }
    }

    protected void btn_Cancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect("GastosMedicosMenoresEmpleados");
    }
}