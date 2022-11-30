//PORTAL DE PROVEDORES T|SYS|
//2 - AGOSTO, 2022
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : RAFAEL BOZA
//PANTALLA PARA TARJETAS CORPORATIVAS
//REFERENCIAS UTILIZADAS

using System;
using System.IO;
using uCFDsLib.v33;
using uCFDsLib.v40;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Net;
using System.Linq;
using System.Drawing;
using ComprobanteComplemento = uCFDsLib.v33.ComprobanteComplemento;
using ComprobanteAddenda = uCFDsLib.v33.ComprobanteAddenda;
using ComprobanteConceptoImpuestos = uCFDsLib.v33.ComprobanteConceptoImpuestos;
using c_Impuesto = uCFDsLib.v33.c_Impuesto;
using Proveedores_Model;
using WebSite1;

public partial class Logged_Administradores_TarjetaEmpleado : System.Web.UI.Page
{
    #region Variables

    private int iVendKey;
    private int iLogKey;
    private int iUserKey;
    private string iCompanyID;
    private bool is_valid;
    private PortalProveedoresEntities _context = new PortalProveedoresEntities();
    private List<ItemDTO> gridList = new List<ItemDTO>();
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
                    //string Var = HttpContext.Current.Session["VendKey"].ToString();
                    //string Var2 = HttpContext.Current.Session["RolUser"].ToString();
                    //string Var3 = HttpContext.Current.Session["IDCompany"].ToString();

                    //pVendKey = Convert.ToInt32(HttpContext.Current.Session["VendKey"].ToString());
                    pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
                    pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
                    pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
                    fill_fileUploads();
                    BindGridView();

                    if(HttpContext.Current.Session["is_valid"] != null)
                    {
                        is_valid = (bool)HttpContext.Current.Session["is_valid"];
                    }
                    else 
                    {
                        HttpContext.Current.Session["is_valid"] = false;                        
                    }
                    btnSage.Enabled = is_valid;


                    pVendKey = 0;
                    MultiView1.SetActiveView(View_General);
                    //pLogKey = 0;
                    //pUserKey = 0;
                    //pCompanyID = "";


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
                get_taxes();
                get_items(pCompanyID);
                HttpContext.Current.Session["GridList"] = null;
                HttpContext.Current.Session["GridTaxes"] = null;               
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

    private void get_taxes()
    {
        List<TaxesDTO> lista =Doc_Tools.get_taxes(pCompanyID);       
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

    protected void Page_LoadComplete(object sender, EventArgs e)
    {

        //sesiones para el modo edicion     

    }

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        eventName = "OnPreInit";
    }

    protected void Load_TiposGasto()
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                DataSet dsFacturas = new DataSet();
                string strSelectCmd = "SELECT [Id] ,[Name] FROM [ExpenseType]";
                SqlDataAdapter da = new SqlDataAdapter(strSelectCmd, conn);
                conn.Open();
                da.Fill(dsFacturas, "Invoice");
                DataView dvInvoice = dsFacturas.Tables["Invoice"].DefaultView;
                STipoGasto.DataSource = dvInvoice;
                STipoGasto.DataValueField = "Id";
                STipoGasto.DataTextField = "Name";
                STipoGasto.DataBind();
                STipoGasto.SelectedIndex = -1;
            }
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Reembolso-Empleados:BindSTipoGasto", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
        }

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

        if (HttpContext.Current.Session["fu_voucher"] != null)
        {
            var f_voucher = (FileUpload)HttpContext.Current.Session["fu_voucher"];
            tbx_voucher.Text = f_voucher.FileName;
        }
    }

    private bool Check_Exist(int userKey, decimal importe)
    {
        decimal count = 0;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT count(*) FROM CorporateCard where UpdateUserKey = @UpdateUserKey and Amount = @Amount and UpdateDate = @UpdateDate";
            cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = importe;
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = userKey;
            cmd.Parameters.Add("@UpdateDate", SqlDbType.Date).Value = DateTime.Today.Date;
            cmd.Connection.Open();
            var lector = cmd.ExecuteReader();
            while (lector.Read())
            {
                count = lector.GetInt32(0);
            }
            if (count > 0)
            {
                return true;
            }
        }
        return false;
    }
    private void Sage()
    {
        is_valid = (bool)HttpContext.Current.Session["is_valid"];
        if (is_valid)
        {
            fu_xml = (FileUpload)HttpContext.Current.Session["fu_xml"];
            var fu_xml_bytes = (byte[])HttpContext.Current.Session["fu_xml_bytes"];

            fu_pdf = (FileUpload)HttpContext.Current.Session["fu_pdf"];
            var fu_pdf_bytes = (byte[])HttpContext.Current.Session["fu_pdf_bytes"];

            fu_voucher = (FileUpload)HttpContext.Current.Session["fu_voucher"];
            var fu_voucher_bytes = (byte[])HttpContext.Current.Session["fu_voucher_bytes"];

            //Escribir info en BD
            int tipo_moneda = int.Parse(drop_currency.SelectedValue);
            DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);
            //int tipo_gasto = int.Parse(STipoGasto.SelectedValue);
            decimal importe_gasto = decimal.Parse(tbx_importe.Text);
            string motivo_gasto = tbx_motivo.Text;
            int status = 1;
            var lista_detalles = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];
            string rol = HttpContext.Current.Session["RolUser"].ToString();
            if (rol != "T|SYS| - Empleado")
            {
                List<RolDTO> roles = Doc_Tools.get_RolesValidadores().ToList();
                if (roles.FirstOrDefault(x => x.ID == rol).Key == roles.Max(z => z.Key))
                {
                    status = 2;
                }
            }
            int carga = WriteToDb(tipo_moneda, fecha_gasto, importe_gasto, fu_xml, fu_pdf, fu_voucher, pUserKey, pCompanyID, lista_detalles,status, motivo_gasto);

            if (carga == -1)
            {
                gvGastos.DataSource = null;
                //gvValidacion.DataSource = null;
                //gvValidacion.Visible = false;
                gvGastos.Visible = false;
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B4);", true);
                System.Threading.Thread.Sleep(5000);
                ClearControls();
                return;
            }

            EnviarCorreo();
            BindGridView();
            ClearControls();
            HttpContext.Current.Session["GridList"] = null;
            HttpContext.Current.Session["GridTaxes"] = null;         
            GvItems.DataSource = null;
            GvItems.DataBind();
        }
        
    }

    private void BindGridView()
    {
        gvGastos.DataSource = null;
        //gvValidacion.DataSource = null;
        //gvValidacion.Visible = false;
        gvGastos.Visible = true;
        gvGastos.DataSource = ReadFromDb(pUserKey);
        gvGastos.DataBind();
    }

    private void ClearControls()
    {
        tbx_fechagasto.Text = string.Empty;
        tbx_importe.Text = string.Empty;      
        STipoGasto.ClearSelection();
        drop_currency.ClearSelection();
        tbx_pdf.Text = string.Empty;
        if(fu_xml!=null)
        {
            fu_xml.Attributes.Clear();
        }
        if(fu_pdf!=null)
        {
            fu_pdf.Attributes.Clear();
        }
        if(fu_voucher!=null)
        {
            fu_voucher.Attributes.Clear();
        }       
        
        tbx_voucher.Text = string.Empty;
        tbx_xml.Text = string.Empty;
        btnSage.Enabled = false;
        tbx_motivo.Text = string.Empty;
    }

    /// <summary>
    /// Metodo para Guardar el Gasto en BD
    /// </summary>
    /// <returns>Entero</returns>
    private int WriteToDb(int tipo_moneda, DateTime fecha_gasto, decimal importe_gasto, FileUpload fu_xml, FileUpload fu_pdf, FileUpload fu_voucher, int userkey, string companyId, List<ExpenseDetailDTO> expenseDetails,int status, string motivo_gasto)
    {
        string val = "";
        int id = 0;
        int? approval_level = null;
        try
        {
            //Xml
            string xml_filename = string.Empty;
            byte[] bytes_xml = new byte[0];
            if (fu_xml != null)
            {
                if (fu_xml.HasFile)
                {
                    Stream fs_xml = fu_xml.PostedFile.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs_xml);
                    bytes_xml = (byte[])HttpContext.Current.Session["fu_xml_bytes"];
                    xml_filename = fu_xml.FileName;
                    //bytes_xml = fu_xml.FileBytes;
                }
            }
            //Pdf
            string pdf_filename = string.Empty;
            byte[] bytes_pdf = new byte[0];
            if (fu_pdf != null)
            {
                if (fu_pdf.HasFile)
                {
                    Stream fs_pdf = fu_pdf.PostedFile.InputStream;
                    System.IO.BinaryReader br2 = new System.IO.BinaryReader(fs_pdf);
                    bytes_pdf = (byte[])HttpContext.Current.Session["fu_pdf_bytes"];
                    pdf_filename = fu_pdf.FileName;
                    //bytes_pdf = fu_pdf.FileBytes;
                }
            }
            //Voucher
            string voucher_filename = string.Empty;
            byte[] bytes_pdf_voucher = new byte[0];
            if (fu_voucher != null)
            {
                if (fu_voucher.HasFile)
                {
                    Stream fs_pdf_voucher = fu_voucher.PostedFile.InputStream;
                    System.IO.BinaryReader br2 = new System.IO.BinaryReader(fs_pdf_voucher);
                    bytes_pdf_voucher = (byte[])HttpContext.Current.Session["fu_voucher_bytes"];
                    voucher_filename = fu_voucher.FileName;
                    //bytes_pdf_voucher = fu_voucher.FileBytes;
                }
            }

            //set Nivel
            string rol = HttpContext.Current.Session["RolUser"].ToString();
            int role_key = Doc_Tools.get_Roles().FirstOrDefault(x => x.ID == rol).Key;
            if (role_key == 8)
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
                cmd.CommandText = "Insert Into CorporateCard (Date, Currency, Amount, FileBinaryXml,FileNameXml, FileBinaryPdf, FileNamePdf,FileBinaryPdfVoucher,FileNamePdfVoucher, CreateDate, UpdateDate, UpdateUserKey, CompanyId, Status, ApprovalLevel, ExpenseReason) VALUES ( @Date, @Currency, @Amount, @FileBinaryXml, @FileNameXml, @FileBinaryPdf, @FileNamePdf, @FileBinaryPdfVoucher, @FileNamePdfVoucher, @CreateDate, @UpdateDate, @UpdateUserKey, @CompanyId, @Status, @ApprovalLevel, @ExpenseReason); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = fecha_gasto;
                cmd.Parameters.Add("@Currency", SqlDbType.Int).Value = tipo_moneda;
                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = importe_gasto;
                //Documentos
                cmd.Parameters.Add("@FileBinaryXml", SqlDbType.VarBinary, bytes_xml.Length).Value = bytes_xml;
                cmd.Parameters.Add("@FileNameXml", SqlDbType.NVarChar).Value = xml_filename;
                cmd.Parameters.Add("@FileBinaryPdf", SqlDbType.VarBinary, bytes_pdf.Length).Value = bytes_pdf;
                cmd.Parameters.Add("@FileNamePdf", SqlDbType.NVarChar).Value = pdf_filename;
                cmd.Parameters.Add("@FileBinaryPdfVoucher", SqlDbType.VarBinary, bytes_pdf_voucher.Length).Value = bytes_pdf_voucher;
                cmd.Parameters.Add("@FileNamePdfVoucher", SqlDbType.NVarChar).Value = voucher_filename;
                //End Documentos
                cmd.Parameters.Add("@CreateDate", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@UpdateDate", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = userkey;
                cmd.Parameters.Add("@CompanyId", SqlDbType.NVarChar).Value = companyId;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status; //Estado inicial Pendiente
                if (approval_level == null)
                {
                    cmd.Parameters.Add("@ApprovalLevel", SqlDbType.Int).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("@ApprovalLevel", SqlDbType.Int).Value = approval_level.Value;
                }
                cmd.Parameters.Add("@ExpenseReason", SqlDbType.VarChar).Value = motivo_gasto ?? (object)DBNull.Value;
                cmd.Connection.Open();
                var modified = cmd.ExecuteScalar();
                val = modified.ToString();
                id = Convert.ToInt32(val);

                foreach (ExpenseDetailDTO detail in expenseDetails)
                {
                    cmd.CommandText = "INSERT INTO CorporateCardDetail (CorporateCardId,Type,ItemKey,Qty,UnitCost,Amount,CreateDate,UpdateDate,CreateUser,CompanyId,STaxCodeKey,TaxAmount) VALUES (@_CorporateCardId,@_Type, @_ItemKey,@_Qty,@_UnitCost,@_Amount,@_CreateDate,@_UpdateDate,@_CreateUser,@_CompanyId,@_STaxCodeKey,@_TaxAmount);";
                    cmd.Parameters.Add("@_CorporateCardId", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@_Type", SqlDbType.Int).Value = detail.Type;
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
                    cmd.Parameters.Clear();
                }               
                cmd.Connection.Close();
            }

            return id;
        }
        catch (Exception ex)
        {
            if (id > 0)
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "DELETE FROM CorporateCard WHERE CorporateCardId = @ExpenseId;";
                    cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = id;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            LogError(pLogKey, pUserKey, "Insertar-Pago-Tarjeta-Empleados:Insertar-Pago", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            return -1;
        }
    }
          

    private List<CorporateCardDTO> ReadFromDb(int user_id)
    {
        List<CorporateCardDTO> gastos = new List<CorporateCardDTO>();

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CorporateCardId ,Date ,Currency ,Amount, Status, FileNameXml ,FileNamePdf ,FileNamePdfVoucher FROM CorporateCard where UpdateUserKey = @UpdateUserKey;";
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;
            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var tarjeta = new CorporateCardDTO();
                tarjeta.CorporateCardId = dataReader.GetInt32(0);              
                tarjeta.Date = dataReader.GetDateTime(1);
                tarjeta.Currency = Doc_Tools.Dict_moneda().First(x => x.Key == dataReader.GetInt32(2)).Value;
                tarjeta.Amount = dataReader.GetDecimal(3);
                tarjeta.Status = Doc_Tools.Dict_status().First(x => x.Key == dataReader.GetInt32(4)).Value;
                tarjeta.FileNameXml = dataReader.GetString(5);
                tarjeta.FileNamePdf = dataReader.GetString(6);
                tarjeta.FileNamePdfVoucher = dataReader.GetString(7);               
                gastos.Add(tarjeta);
            }
        }

        return gastos;

    }

    protected void btnSage_Click(object sender, EventArgs e)
    {
        Sage();
    }

    protected void btnFinalizar_Click(object sender, EventArgs e)
    {
        //logica para enviar los correos
        //Logica para lanzar Reporte de Reembolsos
        Response.Redirect("~/Logged/Reports/TarjetaEmpleado");
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

    protected void STipoGasto_SelectedIndexChanged(object sender, EventArgs e)
    {
        HttpContext.Current.Session["is_valid"] = false;
        btnSage.Enabled = (bool)HttpContext.Current.Session["is_valid"];

        if (STipoGasto.SelectedValue != "")
        {
            tipo = "info";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == STipoGasto.SelectedValue).Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;

        }
    }

    protected void tbx_importe_TextChanged(object sender, EventArgs e)
    {
        if(tbx_importe.Text.Any(d => !char.IsDigit(d) && (d != '.') && (d != ',')))
        {
            tbx_importe.Text = string.Empty;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B16);", true);
            System.Threading.Thread.Sleep(5000);
            return;
        }       
    }
   
    private CorporateCardDTO LoadCardById(int expense_id, int user_id)
    {
        var card = new CorporateCardDTO();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CorporateCardId ,Date ,Currency ,Amount, Status, FileNameXml ,FileNamePdf ,FileNamePdfVoucher, ExpenseReason FROM CorporateCard where UpdateUserKey = @UpdateUserKey and CorporateCardId = @CorporateCardId and Status = 1;";
            cmd.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = user_id;
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
                card.ExpenseReason = dataReader.GetString(8);
            }
        }
        return card;
    }

    protected void gvGastos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        HttpContext.Current.Session["CorporateCard"] = null;
        int rowIndex = int.Parse(e.CommandArgument.ToString());
        GridViewRow row = gvGastos.Rows[rowIndex];

        if (e.CommandName == "Select")
        {
            int card_id = int.Parse(row.Cells[0].Text);
            var card = LoadCardById(card_id, pUserKey);
            HttpContext.Current.Session["CorporateCard"] = card;
            Response.Redirect("EditTarjeta");
        }

    }

    protected void gvGastos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[4].Text != "Pendiente")
            {
                Button btnEdit = (Button)e.Row.Cells[5].Controls[0];
                btnEdit.Visible = false;
            }
        }
    }
   
    protected void GvItems_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        is_valid = false;       
        HttpContext.Current.Session["is_valid"] = is_valid;

        //Borrar articulo del grid   
        int rowIndex = int.Parse(e.CommandArgument.ToString());
        GridViewRow row = GvItems.Rows[rowIndex];

        if (e.CommandName == "Select")
        {
            int itemKey = int.Parse(row.Cells[0].Text);
            var lista = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];
            if (lista.Count == 1)
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB50").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            var detail = lista.FirstOrDefault(x => x.ItemKey == itemKey);
            lista.Remove(detail);
            HttpContext.Current.Session["GridList"] = lista;
            GvItems.DataSource = lista;
            GvItems.DataBind();
            tbx_importe.Text = lista.Sum(x => x.Amount + x.TaxAmount).ToString("0.00");
        }
       
    }

    protected void GvItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }   

    protected void btn_new_article_Click(object sender, EventArgs e)
    {
        is_valid = false;
        HttpContext.Current.Session["is_valid"] = is_valid;

        if(drop_currency.SelectedValue =="")
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B14").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }
        //validacion de fecha
        if (string.IsNullOrEmpty(tbx_fechagasto.Text))
        {            
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB24").Value;     
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }


        MultiView1.SetActiveView(View_Articulos);
    }

    protected void btn_additem_Click(object sender, EventArgs e)
    {
        //validacion de tipo Gasto
        if (string.IsNullOrEmpty(STipoGasto.SelectedValue.ToString()))
        {           
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB26").Value;           
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //validaciones
        if (drop_articulos.SelectedValue == "0")
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B40").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        //validacion del importe del articulo
        if (tbx_importegasto.Text == string.Empty || int.Parse(tbx_importegasto.Text) <= 0)
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B41").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        else 
        {         
            if (tbx_importegasto.Text.Any(x => !char.IsDigit(x) && (x != '.') && (x != ',')))
            {
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B16").Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                MultiView1.SetActiveView(View_Articulos);
                return;
            }           
        }

        //validacion texto en cantidad
        if (tbx_cantidad.Text.Any(x => !char.IsNumber(x)))
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B39").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }
        if (tbx_cantidad.Text == string.Empty || int.Parse(tbx_cantidad.Text) <= 0)
        {
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B39").Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            MultiView1.SetActiveView(View_Articulos);
            return;
        }

        //Lista de articulos
        var items = (List<ItemDTO>)HttpContext.Current.Session["Items"];
        var taxes = (List<TaxesDTO>)HttpContext.Current.Session["Taxes"];
        //Agregar articulo al grid
        var detalle = new ExpenseDetailDTO();
        detalle.ItemKey = int.Parse(drop_articulos.SelectedItem.Value);
        detalle.Type = int.Parse(STipoGasto.SelectedValue);
        detalle.TipoGasto = STipoGasto.SelectedItem.Text;
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
            lista.Add(detalle);
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
        drop_taxes.ClearSelection();
        tbx_importegasto.Text = string.Empty;
        tbx_cantidad.Text = string.Empty;
        drop_taxes.ClearSelection();      
        MultiView1.SetActiveView(View_General);

    }       
     
    protected void btn_cancelar_item_Click(object sender, EventArgs e)
    {
        is_valid = false;
        HttpContext.Current.Session["is_valid"] = is_valid;
        //Limpiar controles
        drop_articulos.ClearSelection();
        STipoGasto.ClearSelection();
        tbx_cantidad.Text = string.Empty;
        tbx_importegasto.Text = string.Empty;
        MultiView1.SetActiveView(View_General);
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

        var jerarquia = Doc_Tools.get_JerarquiaValidadores(((int)Doc_Tools.DocumentType.CorporateCard));
        var orden = jerarquia.Get_Orden(jerarquia);

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

    protected void btn_validar_Click(object sender, EventArgs e)
    {
        btnSage.Enabled = false;
        is_valid = false;

        //validacion de fecha
        if (string.IsNullOrEmpty(tbx_fechagasto.Text))
        {
            string titulo, Msj, tipo;
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB24").Value;
            titulo = "T|SYS|";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Solo se admiten gastos del mes en curso
        DateTime fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);
        if (DateTime.Today.Year == fecha_gasto.Year)
        {
            if (DateTime.Today.Month != fecha_gasto.Month)
            {
                string titulo, Msj, tipo;
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB38").Value;
                titulo = "T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
        }
        else
        {
            string titulo, Msj, tipo;
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB38").Value;
            titulo = "T|SYS|";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Validacion de moneda
        if (string.IsNullOrEmpty(drop_currency.SelectedValue.ToString()))
        {
            string titulo, Msj, tipo;
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B14").Value;
            titulo = "T|SYS|";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

       
        //validacion de importe del Gasto
        if (string.IsNullOrEmpty(tbx_importe.Text))
        {
            string titulo, Msj, tipo;
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B15").Value;
            titulo = "T|SYS|";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }

        //Escribir info en BD
        int tipo_moneda = int.Parse(drop_currency.SelectedValue);
        fecha_gasto = DateTime.Parse(tbx_fechagasto.Text);
        int tipo_gasto = int.Parse(STipoGasto.SelectedValue);
        decimal importe_gasto = decimal.Parse(tbx_importe.Text);

        //PDF No Obligatorio
        if (fu_pdf.HasFile)
        {
            if (fu_pdf.PostedFile.ContentType.ToString() != "application/pdf")
            {
                gvGastos.DataSource = null;
                string titulo, Msj, tipo;
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B8").Value;
                titulo = "T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }

            if (fu_pdf.PostedFile.ContentLength > 1000000 * 15)
            {
                gvGastos.DataSource = null;
                gvGastos.Visible = false;

                string titulo, Msj, tipo;
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB27").Value;
                titulo = "T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            HttpContext.Current.Session["fu_pdf"] = fu_pdf;
            HttpContext.Current.Session["fu_pdf_bytes"] = fu_pdf.FileBytes;
        }

        //Si tiene fichero XML (No Obligatorio)
        if (fu_xml.HasFile)
        {
            //Validación del Formato del Archivo XML
            if (fu_xml.PostedFile.ContentType.ToString() != "text/xml")
            {
                gvGastos.DataSource = null;
                gvGastos.Visible = false;
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
                gvGastos.DataSource = null;
                //gvValidacion.DataSource = null;;

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
                string titulo, Msj, tipo;
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B11").Value;
                titulo = "T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            HttpContext.Current.Session["fu_xml"] = fu_xml;
            HttpContext.Current.Session["fu_xml_bytes"] = fu_xml.FileBytes;
        }

        //Si tiene fichero PDF Voucher (No Obligatorio)
        if (fu_voucher.HasFile)
        {
            if (fu_voucher.PostedFile.ContentType.ToString() != "application/pdf")
            {
                gvGastos.DataSource = null;

                string titulo, Msj, tipo;
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "B8").Value;
                titulo = "T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }

            if (fu_voucher.PostedFile.ContentLength > 1000000 * 15)
            {
                gvGastos.DataSource = null;
                gvGastos.Visible = false;

                string titulo, Msj, tipo;
                tipo = "error";
                Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB28").Value;
                titulo = "T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                return;
            }
            HttpContext.Current.Session["fu_voucher"] = fu_voucher;
            HttpContext.Current.Session["fu_voucher_bytes"] = fu_voucher.FileBytes;
        }

        var lista_detalles = (List<ExpenseDetailDTO>)HttpContext.Current.Session["GridList"];     

        if (lista_detalles == null)
        {

            string titulo, Msj, tipo;
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB35").Value;
            titulo = "T|SYS|";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            return;
        }
      
        if (Check_Exist(pUserKey, importe_gasto))
        {
            string titulo, Msj, tipo;
            tipo = "error";
            Msj = Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB39").Value;
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

        if (HttpContext.Current.Session["fu_voucher"] != null)
        {
            fu_voucher = (FileUpload)HttpContext.Current.Session["fu_voucher"];
        }

        if (!fu_voucher.HasFile)
        {
            Msjj += Doc_Tools.get_msg().FirstOrDefault(x => x.Key == "MB33").Value + "\n";
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
        if (fu_voucher.HasFile)
        {
            HttpContext.Current.Session["fu_voucher"] = fu_voucher;
            HttpContext.Current.Session["fu_voucher_bytes"] = fu_voucher.FileBytes;
        }
       
    }

  
}