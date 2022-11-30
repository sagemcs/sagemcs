//PORTAL DE PROVEDORES T|SYS|
//10 - JUNIO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA ADMINISTRACION DE FACTURAS CARGADAS POR PROVEEDORES

//REFERENCIAS UTILIZADAS
using System;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI;
using System.Diagnostics;

public partial class Logged_AdmFacturas : System.Web.UI.Page
{
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

    //Rutina Manejar Errores
    public static void LogError(int LogKey, int UpdateUserKey, String proceso, String mensaje, String CompanyID)
    {
        try
        {

            int val1;
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
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey2, Userk;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"] == null) { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            if (HttpContext.Current.Session["IDCompany"] == null) { Msj = Msj + "," + "Variable IDCompany null"; Company = "MGS"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["LogKey"] == null) { Msj = Msj + "," + "Variable LogKey null"; LogKey2 = 0; } else { LogKey2 = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : AdmFacturas.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey2, Userk, "AdmFacturas.aspx.cs_" + nombreMetodo, Msj, Company);

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
            string Msj = string.Empty;
            StackTrace st = new StackTrace(ex, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int LogKey, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"] == null) { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            if (HttpContext.Current.Session["IDCompany"] == null) { Msj = Msj + "," + "Variable IDCompany null"; Company = "MGS"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["VendKey"] == null) { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"] == null) { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : AdmFacturas.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, " AdmFacturas.aspx.cs_" + nombreMetodo, Msj, Company);
            return null;
        }
    }

    string eventName = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        try
        {

                if (HttpContext.Current.Session["IDCompany"] == null)
                {
                    Context.GetOwinContext().Authentication.SignOut();
                    Response.Redirect("~/Account/Login.aspx");
                }
                else
                {
                    if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
                    {
                       pVendKey = 0;
                       pLogKey = Convert.ToInt32(Session["LogKey"]);
                       pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
                       pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
                       DatosV.Visible = false;
                       ActualizarFacturas();

                    if (HttpContext.Current.Session["Evento"] != null)
                    {
                        if (IsPostBackEventControlRegistered)
                        {
                            HttpContext.Current.Session["Evento"] = null;
                        }
                        else
                        {
                            if (HttpContext.Current.Session["Evento"].ToString() == "Java")
                            {
                                int Cont = 0;
                                if (HttpContext.Current.Session["Prv"] != null) { IdProveedor.Text = HttpContext.Current.Session["Prv"].ToString(); Cont = Cont + 1; }
                                if (HttpContext.Current.Session["Fol"] != null) { Folio.Text = HttpContext.Current.Session["Fol"].ToString(); Cont = Cont + 1; }
                                if (HttpContext.Current.Session["NOC"] != null) { NoOC.Text = HttpContext.Current.Session["NOC"].ToString(); Cont = Cont + 1; }
                                if (HttpContext.Current.Session["Sts"].ToString() != null) { dpEstatus.SelectedIndex = Convert.ToInt32(HttpContext.Current.Session["Sts"].ToString()) - 1; Cont = Cont + 1; }
                                if (HttpContext.Current.Session["Chk"].ToString() == "true")
                                {
                                    chkFechas.Checked = true;
                                    if (HttpContext.Current.Session["Chk"].ToString() != null) { chkFechas.Checked = Convert.ToBoolean(HttpContext.Current.Session["Chk"].ToString()); Cont = Cont + 1; }
                                    if (HttpContext.Current.Session["Fe1"] != null) { txtdtp.Text = HttpContext.Current.Session["Fe1"].ToString(); Cont = Cont + 1; }
                                    if (HttpContext.Current.Session["Fe2"] != null) { txtdtp2.Text = HttpContext.Current.Session["Fe2"].ToString(); Cont = Cont + 1; }
                                }
                                else
                                {
                                    chkFechas.Checked = false;
                                }
                                HttpContext.Current.Session["Evento"] = null;
                                if (Cont >0)
                                {
                                    BindGrid();
                                }                                
                            }
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
        catch (Exception ex)
        {
            //RutinaError(ex);
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

            if (HttpContext.Current.Session["RolUser"] != null)
            {

                if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas")
                {
                    Page.MasterPageFile = "MasterPageContb.master";
                }

                if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
                {
                    Page.MasterPageFile = "SiteVal.master";
                }

                if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Empleado")
                {
                    Page.MasterPageFile = "~/Logged/Administradores/SiteEmpleado.master";
                }
            }
        }
        catch (Exception ex)
        {
            //RutinaError(ex);
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

    protected String VendorID(string VendorKey, string CompanyID)
    {
        try
        {

            SqlCommand sqlSelectCommand1 = new SqlCommand();
            SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter();
            SqlConnection sqlConnection1 = new SqlConnection();

            sqlConnection1 = SqlConnectionDB("ConnectionString");

            string sSQL;

            sqlConnection1.Open();

            sSQL = @"SELECT VendID FROM tapVendor WHERE VendKey = @varID and CompanyID=@CompanyID";


            string VendorId = "";

            using (var sqlQuery = new SqlCommand(sSQL, sqlConnection1))
            {
                sqlQuery.Parameters.AddWithValue("@varID", VendorKey);
                sqlQuery.Parameters.AddWithValue("@CompanyID", CompanyID);

                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        while (sqlQueryResult.Read())
                        {
                            VendorId = Convert.ToString(sqlQueryResult.GetValue(0));
                        }
                    }
            }

            sqlConnection1.Close();

            return VendorId;

        }

        catch (Exception ex)
        {
            RutinaError(ex);
            return "XXXX";
        }

    }

    //protected void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType != DataControlRowType.DataRow)
    //        return;

    //    //se recupera la entidad que genera la row
    //    //Product prod = e.Row.DataItem as Product;
    //    string valor = e.Row.Cells[1].Text.ToString();

    //    //se verifica si el producto esta discontinuo
    //    //para quitar el boton de edicion
    //    if (valor == "Extranjero")
    //    {
    //        ImageButton img = e.Row.FindControl("imgEdit") as ImageButton;
    //        e.Row.Cells[7].Controls.Remove(img);
    //    }

    //}

    protected void BindGridInvoices(string VendId, string RFC, string Folio, string UUID, string NodeOC, int estatus, int opcion, DateTime FechaInicial, DateTime FechaFinal, string CompanyID)
    {
        string sSQL = "";

        SqlConnection sqlConnection1 = new SqlConnection();
        sqlConnection1 = SqlConnectionDB("PortalConnection");
        sqlConnection1.Open();

        using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
        {
            try
            {
                sSQL = "spSelectInvoice";

                List<SqlParameter> parsT = new List<SqlParameter>();
                parsT.Add(new SqlParameter("@VendId", VendId));
                parsT.Add(new SqlParameter("@RFC", RFC));
                parsT.Add(new SqlParameter("@Folio", Folio));
                parsT.Add(new SqlParameter("@UUID", UUID));
                parsT.Add(new SqlParameter("@NodeOC", NodeOC));
                parsT.Add(new SqlParameter("@estatus", estatus));
                parsT.Add(new SqlParameter("@opcion", opcion));
                parsT.Add(new SqlParameter("@FechaInicial", FechaInicial));
                parsT.Add(new SqlParameter("@FechaFinal", FechaFinal));
                parsT.Add(new SqlParameter("@CompanyID", CompanyID));

                if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Empleado") 
                {
                    string user = HttpContext.Current.Session["UserKey"].ToString();
                    sSQL = "spSelectInvoiceEmple";
                    parsT.Add(new SqlParameter("@userkey", user));
                }

                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = sSQL;

                System.Data.SqlClient.SqlParameter[] sqlParameter = parsT.ToArray();
                foreach (System.Data.SqlClient.SqlParameter par in sqlParameter)
                {
                    Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);
                }

                System.Data.SqlClient.SqlDataAdapter Data = new System.Data.SqlClient.SqlDataAdapter(Cmd);
                System.Data.DataTable Table = new System.Data.DataTable();

                Data.Fill(Table);
                gvFacturas.DataSource = Table;
                gvFacturas.DataBind();


                if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas" || dpEstatus.SelectedItem.ToString() != "Pendiente")
                {
                    gvFacturas.Columns[10].Visible = false;
                    gvFacturas.Columns[11].Visible = false;
                }
                else
                {
                    gvFacturas.Columns[10].Visible = true;
                    gvFacturas.Columns[11].Visible = true;
                }

                if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Empleado" || dpEstatus.SelectedItem.ToString() != "Pendiente")
                {
                    gvFacturas.Columns[10].Visible = false;
                    gvFacturas.Columns[11].Visible = false;
                }

                //gvFacturas.Columns[0].Visible = false;

                if (Table.Rows.Count == 0) { DatosV.Visible = true; }


            }
            catch (Exception ex)
            {
                RutinaError(ex);
                //throw new Exception("Error al ejecutar procedimiento almacenado, Error: " + ex.ToString());
            }
            finally
            {
                if (sqlConnection1.State == ConnectionState.Open)
                {
                    sqlConnection1.Close();
                }
            }
        }
    }

    protected void Limpiar_Click1(object sender, EventArgs e)
    {
        IdProveedor.Text = "";
        //RFC.Text = "";
        //UUID.Text = "";
        NoOC.Text = "";
        Folio.Text = "";
        dpEstatus.SelectedValue = "1";
        chkFechas.Checked = false;
        txtdtp.Text = "";
        txtdtp2.Text = "";
        BindGrid();
    }

    protected void Buscar_Click1(object sender, EventArgs e)
    {
        BindGrid();
    }

    protected void BindGrid()
    {
        try
        {
            DateTime fechaini;
            DateTime fechafin;
            DatosV.Visible = false;
            if (chkFechas.Checked)
            {
                fechaini = DateTime.Parse(txtdtp.Text);
                fechafin = DateTime.Parse(txtdtp2.Text);
            }
            else
            {
                fechaini = DateTime.Parse("2000-01-01");
                fechafin = DateTime.Parse("2000-01-01");
            }

            BindGridInvoices(IdProveedor.Text, "", Folio.Text, "", NoOC.Text, Convert.ToInt32(dpEstatus.SelectedItem.Value), chkFechas.Checked ? 1 : 0, fechaini, fechafin, pCompanyID);

        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }

    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            MemoryStream memoryStream = new MemoryStream();
            int index = Convert.ToInt32(e.CommandArgument);
            String archivo;

            GridViewRow row = gvFacturas.Rows[index];
            archivo = HttpUtility.HtmlDecode(row.Cells[1].Text);
            string folio = HttpUtility.HtmlDecode(row.Cells[0].Text);
            string llave = HttpUtility.HtmlDecode(row.Cells[3].Text);

            if (e.CommandName == "Documento_1" | e.CommandName == "Documento_2" | e.CommandName == "Documento_3")
            {
                if (e.CommandName == "Documento_1")
                {
                    memoryStream = databaseFileRead("InvoiceKey", row.Cells[0].Text, "FileBinary1"); // XML
                    archivo += " Factura - " + HttpUtility.HtmlDecode(row.Cells[3].Text);
                    archivo += ".xml";
                    HttpContext.Current.Response.ContentType = "text/xml";
                }
                if (e.CommandName == "Documento_2")
                {
                    memoryStream = databaseFileRead("InvoiceKey", row.Cells[0].Text, "FileBinary2"); // PDF
                    archivo += " Factura - " + HttpUtility.HtmlDecode(row.Cells[3].Text);
                    archivo += ".pdf";
                    HttpContext.Current.Response.ContentType = "application/pdf";
                }
                if (e.CommandName == "Documento_3")
                {
                    memoryStream = databaseFileRead("InvoiceKey", row.Cells[0].Text, "FileBinary3"); //PDF
                    archivo += " Factura - " + HttpUtility.HtmlDecode(row.Cells[3].Text) + " Anexo - " + HttpUtility.HtmlDecode(row.Cells[2].Text);
                    archivo += ".pdf";
                    HttpContext.Current.Response.ContentType = "application/pdf";
                }


                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + archivo + "\"");
                HttpContext.Current.Response.AppendHeader("Content-Length", memoryStream.Length.ToString());
                HttpContext.Current.Response.BinaryWrite(memoryStream.ToArray());
                HttpContext.Current.Response.ContentType = Page.ContentType;
                HttpContext.Current.Response.End();


            }
            else if (e.CommandName == "Aprobar")
            {
                InsertSage(row.Cells[0].Text, row.Cells[1].Text);
            }
            else if (e.CommandName == "Cancelar")
            {
                int Fila = row.RowIndex + 1;
                Rechaza(llave, folio, Fila);
                //int Vrs = Convert.ToInt32(folio);
                //if ((CancelarD(Vrs, 0, 1, 1) == 0))
                //{
                //    int Fila = row.RowIndex + 1;
                //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "Pregunta('" + folio + "','" + llave + "','" + Fila + "');", true);
                //    BindGrid();
                //}
                //HttpContext.Current.Response.AddHeader("REFRESH", "2;URL=AdmFacturas");
                //Thread.Sleep(5000);
                //Response.Redirect("miUrl");
            }


        }
        catch (Exception ex)
        {
            //RutinaError(ex);
            //LogError(pLogKey, pUserKey, "Carga-Factura:GridView1_RowCommand", ex.Message, pCompanyID);
        }
    }

    protected void Rechaza(string InvoiceKey,string Folios,int Fila)
    {
        try
        {
                SqlCommand sqlSelectCommand1 = new SqlCommand();
                SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter();
                SqlConnection sqlConnection1 = new SqlConnection();
                sqlConnection1 = SqlConnectionDB("PortalConnection");

                string sSQL;
                sqlConnection1.Open();
                sSQL = @"SELECT Folio,NodeOc,UUID,VendorKey,Trantype FROM Invoice WHERE InvoiceKey = @varID";

                string Folio = "";
                string NodeOc = "";
                string UUID = "";
                string VendorKey = "";
                string Trantype = "";

                using (var sqlQuery = new SqlCommand(sSQL, sqlConnection1))
                {
                    sqlQuery.Parameters.AddWithValue("@varID", Folios);
                    using (var sqlQueryResult = sqlQuery.ExecuteReader())
                        if (sqlQueryResult != null)
                        {
                            while (sqlQueryResult.Read())
                            {
                                Folio = Convert.ToString(sqlQueryResult.GetValue(0));
                                NodeOc = Convert.ToString(sqlQueryResult.GetValue(1));
                                UUID = Convert.ToString(sqlQueryResult.GetValue(2));
                                VendorKey = Convert.ToString(sqlQueryResult.GetValue(3));
                                Trantype = Convert.ToString(sqlQueryResult.GetValue(4));
                            }
                        }
                }

                sqlConnection1.Close();

                ///Consulta Sage
                sqlConnection1 = SqlConnectionDB("ConnectionString");
                sqlConnection1.Open();
                int Conteo = 0;

                sSQL = "Select Count(*) from tapVoucherCEx WHERE iTranNo =" + "'" + Folio + "' And iVendKey = " + VendorKey;
                sqlConnection1 = SqlConnectionDB("ConnectionString");
                sqlConnection1.Open();
                using (SqlCommand Cmd = new SqlCommand(sSQL, sqlConnection1))
                {
                    try
                    {
                        Cmd.CommandType = CommandType.Text;
                        Cmd.CommandText = sSQL;
                        Conteo = Convert.ToInt32(Cmd.ExecuteScalar());
                    }
                    catch (Exception ex)
                    {
                        RutinaError(ex);
                        lblMsj.Text = ex.Message;
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
                        return;
                    }
                }

                sqlConnection1.Close();
                if (Conteo == 0)
                {
                    int Vrs = Convert.ToInt32(Folios);
                    if ((CancelarD(Vrs, 0, 1, 1) == 0))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "Pregunta('" + Folios + "','" + InvoiceKey + "','" + Fila + "');", true);
                        BindGrid();
                    }
                }
                else
                {
                    Label4.Text = "La Factura ya se encuentra Procesada en SAGE";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);
                }
        }
        catch(Exception ex)
        {
            RutinaError(ex);
            lblMsj.Text = ex.Message;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
        }
    }

    protected int CancelarD(int FacKey, int Not, int Op,int Tipo)
    {
        int val1 = -1;
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string sSQL;
            string Rest = string.Empty;

            sSQL = "Notificaciones_Fc";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@Invc", FacKey));
            parsT.Add(new SqlParameter("@Not", Not));
            parsT.Add(new SqlParameter("@Opcion", Op));
            parsT.Add(new SqlParameter("@Tipo", Tipo));

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
                    Rest = rdr["Resultado"].ToString(); // 0 ok
                }
                sqlConnection1.Close();
                val1 = Convert.ToInt32(Rest);

            }
        }
        catch (Exception ex)
        {
            throw new MulticonsultingException(ex.Message);
        }
        return val1;
    }

    private bool StatusFactura(String InvoiceKey, String Status)
    {
        try
        {

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                if (Status == "3")
                    sqlQuery.CommandText = "UPDATE Invoice SET Status=" + Status + " WHERE InvoiceKey =" + InvoiceKey;
                else
                    sqlQuery.CommandText = "UPDATE Invoice SET Status=" + Status + " ,AprovUserKey=" + pUserKey.ToString() + ",AprovDate='" + DateTime.Now.ToString("MM/dd/yyyy") + "' WHERE InvoiceKey = " + InvoiceKey;

                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
            }

            return true;
        }
        catch (Exception ex)
        {
            RutinaError(ex);
            //LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            return false;
        }
    }

    private MemoryStream databaseFileRead(string consulta, string InvoiceKey, string columna)
    {
        try
        {
            string sql = "";
            MemoryStream memoryStream = new MemoryStream();
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            sql = @"SELECT " + columna + " FROM InvoiceFile WHERE InvoiceKey = @varID";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.Parameters.AddWithValue("@varID", InvoiceKey);
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        sqlQueryResult.Read();
                        var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                        sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                        memoryStream.Write(blob, 0, blob.Length);
                    }
            }

            sqlConnection1.Close();


            return memoryStream;
        }
        catch (Exception ex)
        {
            RutinaError(ex);

            return null;
        }
    }

    private void InsertSage(String InvoiceKey, String VendId)
    {
        try
        {

            if (!ValidaArt(InvoiceKey))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
                return;
            }

            SqlCommand sqlSelectCommand1 = new SqlCommand();
            SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter();
            SqlConnection sqlConnection1 = new SqlConnection();
            List<SqlParameter> parsT;

            sqlConnection1 = SqlConnectionDB("PortalConnection");

            string sSQL;
            int val1 = 0;
            int val2 = 0;
            int val3 = 0;

            sqlConnection1.Open();

            sSQL = @"SELECT Folio,NodeOc,UUID,VendorKey,Trantype FROM Invoice WHERE InvoiceKey = @varID";

            string Folio = "";
            string NodeOc = "";
            string UUID = "";
            string VendorKey = "";
            string Trantype = "";

            using (var sqlQuery = new SqlCommand(sSQL, sqlConnection1))
            {
                sqlQuery.Parameters.AddWithValue("@varID", InvoiceKey);

                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        while (sqlQueryResult.Read())
                        {

                            Folio = Convert.ToString(sqlQueryResult.GetValue(0));
                            NodeOc = Convert.ToString(sqlQueryResult.GetValue(1));
                            UUID = Convert.ToString(sqlQueryResult.GetValue(2));
                            VendorKey = Convert.ToString(sqlQueryResult.GetValue(3));
                            Trantype = Convert.ToString(sqlQueryResult.GetValue(4));
                        }
                    }
            }

            sqlConnection1.Close();


            if (Trantype == "CM")
            {
                lblMsj1.Text = "Nota Aprobada.";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL2);", true);
                StatusFactura(InvoiceKey, "2");
                BindGrid();
                return;
            }

            ///Consulta Sage
            int Conteo = 0;

            if (UUID == "")
            {
                Conteo = 0;
            }
            else 
            { 
                sqlConnection1 = SqlConnectionDB("ConnectionString");
                sqlConnection1.Open();
                sSQL = "Select Count(*) from tapLoadFilesAP WHERE UUID =" + "'" + UUID.ToString() + "'";
                using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
                {
                    try
                    {
                        Cmd.CommandType = CommandType.Text;
                        Cmd.CommandText = sSQL;

                        Conteo = Convert.ToInt32(Cmd.ExecuteScalar());

                    }
                    catch (Exception ex)
                    {
                        RutinaError(ex);
                        return;
                    }
                }
                sqlConnection1.Close();
            }

            if (Conteo == 0)
            {
                sSQL = "Select Count(*) from tapVoucherCEx WHERE iTranNo =" + "'" + Folio + "' And iVendKey = " + VendorKey;

                sqlConnection1 = SqlConnectionDB("ConnectionString");
                sqlConnection1.Open();

                using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
                {
                    try
                    {
                        Cmd.CommandType = CommandType.Text;
                        Cmd.CommandText = sSQL;
                        Conteo = Convert.ToInt32(Cmd.ExecuteScalar());

                    }
                    catch (Exception ex)
                    {
                        RutinaError(ex);
                        lblMsj.Text = ex.Message;
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
                        return;
                    }
                }

                sqlConnection1.Close();

                if (Conteo == 0)
                {
                    sSQL = "spapiInsertInvSage";
                    parsT = new List<SqlParameter>();
                    parsT.Add(new SqlParameter("@Invckey", InvoiceKey));
                    parsT.Add(new SqlParameter("@VendKey", VendorKey));
                    parsT.Add(new SqlParameter("@UserID", "admin"));
                    parsT.Add(new SqlParameter("@CompanyID", pCompanyID));

                    sqlConnection1 = SqlConnectionDB("PortalConnection");

                    using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
                    {
                        try
                        {
                            Cmd.CommandType = CommandType.StoredProcedure;
                            Cmd.CommandText = sSQL;

                            foreach (System.Data.SqlClient.SqlParameter par in parsT)
                            {
                                Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);
                            }

                            SqlParameter outParam = new SqlParameter();
                            SqlParameter outParam2 = new SqlParameter();
                            SqlParameter outParam3 = new SqlParameter();

                            outParam.SqlDbType = System.Data.SqlDbType.Int;
                            outParam.ParameterName = "oIDBitacora";
                            outParam.Value = 0;
                            outParam.Direction = System.Data.ParameterDirection.Output;
                            Cmd.Parameters.Add(outParam);

                            outParam2.SqlDbType = System.Data.SqlDbType.Int;
                            outParam2.ParameterName = "ovKey";
                            outParam2.Value = 0;
                            outParam2.Direction = System.Data.ParameterDirection.Output;
                            Cmd.Parameters.Add(outParam2);

                            outParam3.SqlDbType = System.Data.SqlDbType.Int;
                            outParam3.ParameterName = "oRetVal";
                            outParam3.Value = 0;
                            outParam3.Direction = System.Data.ParameterDirection.Output;
                            Cmd.Parameters.Add(outParam3);


                            sqlConnection1.Open();

                            System.Data.SqlClient.SqlDataReader rdr = null;

                            rdr = Cmd.ExecuteReader();

                            val1 = (int)Cmd.Parameters["oIDBitacora"].Value;
                            val2 = (int)Cmd.Parameters["ovKey"].Value;
                            val3 = (int)Cmd.Parameters["oRetVal"].Value;


                            if (val3 == -1)
                            {
                                lblMsj.Text = ("Error al cargar a Sage");
                                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
                                return;
                            }

                            sqlConnection1.Close();
                        }
                        catch (Exception ex)
                        {
                            RutinaError(ex);
                            lblMsj.Text = ex.Message;
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
                            return;
                        }
                    }

                    sSQL = "spapiPreLoadSage";
                    parsT = new List<SqlParameter>();
                    parsT.Add(new SqlParameter("@iLote", val1));
                    parsT.Add(new SqlParameter("@vKey", val2));
                    parsT.Add(new SqlParameter("@UserID", "admin"));
                    parsT.Add(new SqlParameter("@CompanyID", pCompanyID));

                    sqlConnection1 = SqlConnectionDB("ConnectionString");

                    using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
                    {
                        try
                        {
                            Cmd.CommandType = CommandType.StoredProcedure;
                            Cmd.CommandText = sSQL;

                            foreach (System.Data.SqlClient.SqlParameter par in parsT)
                            {
                                Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);
                            }

                            SqlParameter outParam3 = new SqlParameter();

                            outParam3.SqlDbType = System.Data.SqlDbType.Int;
                            outParam3.ParameterName = "oRetVal";
                            outParam3.Value = 0;
                            outParam3.Direction = System.Data.ParameterDirection.Output;
                            Cmd.Parameters.Add(outParam3);

                            sqlConnection1.Open();

                            System.Data.SqlClient.SqlDataReader rdr = null;

                            rdr = Cmd.ExecuteReader();

                            val3 = (int)Cmd.Parameters["oRetVal"].Value;

                            if (val3 == -1)
                            {
                                string Msj = TraerMsj(val1,val2,pCompanyID);
                                if (Msj == "Error al obtener mensaje de Validación")
                                {
                                    Msj = "Error al cargar a Sage";
                                }
                                //Traer errores de la tabla 
                                lblMsj.Text = Msj;
                                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
                                return;
                            }
                            else
                            {
                                //lblMsj1.Text = "Carga Exitosa";
                                //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL2);", true);
                            }

                            sqlConnection1.Close();
                        }
                        catch (Exception ex)
                        {
                            RutinaError(ex);
                            lblMsj.Text = ex.Message;
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
                            return;
                        }
                    }

                    sSQL = "sppaVoucherAPI";
                    parsT = new List<SqlParameter>();
                    parsT.Add(new SqlParameter("@Batchkey", val1));
                    parsT.Add(new SqlParameter("@vKey", val2));
                    parsT.Add(new SqlParameter("@Company", pCompanyID));

                    sqlConnection1 = SqlConnectionDB("ConnectionString");

                    using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
                    {
                        try
                        {
                            Cmd.CommandType = CommandType.StoredProcedure;
                            Cmd.CommandText = sSQL;

                            foreach (System.Data.SqlClient.SqlParameter par in parsT)
                            {
                                Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);
                            }

                            SqlParameter outParam3 = new SqlParameter();

                            outParam3.SqlDbType = System.Data.SqlDbType.Int;
                            outParam3.ParameterName = "RetVal";
                            outParam3.Value = 0;
                            outParam3.Direction = System.Data.ParameterDirection.Output;
                            Cmd.Parameters.Add(outParam3);

                            System.Data.SqlClient.SqlDataReader rdr = null;

                            sqlConnection1.Open();

                            rdr = Cmd.ExecuteReader();

                            val3 = (int)Cmd.Parameters["RetVal"].Value;

                            if (val3 > 2)
                            {
                                lblMsj.Text = ("Error al cargar a Sage");
                                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
                                return;
                            }
                            else
                            {
                                lblMsj1.Text = "Factura Aprobada y cargada en Sage.";
                                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL2);", true);
                                StatusFactura(InvoiceKey, "2");
                                BindGrid();
                            }

                            sqlConnection1.Close();


                        }
                        catch (Exception ex)
                        {
                            RutinaError(ex);
                            lblMsj.Text = ex.Message;
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);

                        }
                    }
                }
                else
                {
                    lblMsj.Text = "La factura ya se encuentra procesada en Sage.";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
                }
            }
            else
            {
                lblMsj.Text = "La factura ya se encuentra procesada.";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
            }
        }
        catch (Exception ex)
        {
            RutinaError(ex);
            lblMsj.Text = ex.Message;
            LogError(pLogKey, pUserKey, "Carga-Factura:GridView1_RowCommand", ex.Message, pCompanyID);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
        }
    }

    private bool ValidaArt(string invoicKey)
    {
        bool Resultado = true;
        try
        {
            string Estado=string.Empty;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string ItemKey = "Select Codigo From InvoiceLines Where InvoiceKey = " + invoicKey;
            using (SqlCommand Cmd = new SqlCommand(ItemKey, sqlConnection1))
            {

                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = ItemKey;
                SqlDataReader rdr = null;
                rdr = Cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string IKey = rdr["Codigo"].ToString(); 

                    string Ssql = "Select Status From TimItem Where ItemID = '" + IKey + "' AND CompanyID = '" + iCompanyID + "'";

                    SqlConnection sqlConnection2 = new SqlConnection();
                    sqlConnection2 = SqlConnectionDB("ConnectionString");
                    sqlConnection2.Open();
                    using (SqlCommand Cmdd = new SqlCommand(Ssql, sqlConnection2))
                    {
                        Cmdd.CommandType = CommandType.Text;
                        Cmdd.CommandText = Ssql;
                        Estado = Cmdd.ExecuteScalar().ToString();
                    }
                    sqlConnection2.Close();

                    if (Estado == "2")
                    {
                        Resultado = false;
                        lblMsj.Text = "El articulo  " + IKey + "  se encuentra con estatus Inactivo , la carga no puede continuar, verificalo en SAGE.";
                        return Resultado;
                    }

                    if (Estado == "4")
                    {
                        Resultado = false;
                        lblMsj.Text = "El articulo  " + IKey + "  se encuentra con estatus Borrado , la carga no puede continuar, verificalo en SAGE.";
                        return Resultado;
                    }

                    Estado = string.Empty;
                }
                sqlConnection1.Close();
            } 

        }
        catch (Exception ex)
        {
            Resultado = false;
            string error = ex.Message;
            lblMsj.Text = "Se produjo un error al realizar la validación de los articulos.";
        }
        return Resultado;
    }

    protected string TraerMsj(int Lote,int vkey,string Company)
    {
        string Conteo=string .Empty;

        try
        {
            SqlCommand sqlSelectCommand1 = new SqlCommand();
            SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter();
            SqlConnection sqlConnection1 = new SqlConnection();
            string sSQL = "Select top 1 ErrorValidacion from tapAPILogValidacion WHERE iLote =" + "'" + Lote + "' And vKey = '" + vkey + "' And CompanyID = '" + Company + "'";

            sqlConnection1 = SqlConnectionDB("ConnectionString");
            sqlConnection1.Open();

            using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
            {
                try
                {
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = sSQL;
                    Conteo = Convert.ToString(Cmd.ExecuteScalar());

                }
                catch (Exception ex)
                {
                    RutinaError(ex);
                    lblMsj.Text = ex.Message;
                }
            }
        }
        catch (Exception ex)
        {
            string msj = ex.Message;
            Conteo = "Error al obtener mensaje de Validación";
        }

        return Conteo;
    }

    protected void dpEstatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        //ActualizarFacturas();
        if (dpEstatus.SelectedValue == "7")
        {   
            RevisaPagos();
        }

        BindGrid();
    }

    protected void RevisaPagos()
    {
        string IDCom = HttpContext.Current.Session["IDCompany"].ToString();
        int Logkey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
        int Uskey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spRevPago", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 6 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@VendKey", Value = 0 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@IDC", Value = IDCom });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Fol", Value = 0 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
            }

        }
        catch (Exception ex)
        {
            LogError(Logkey, Uskey, "Actualización Estado Factura a Pagada", ex.Message, IDCom);
        }
    }

    protected void RutinaError(Exception ex)
    {
        string Msj = string.Empty;
        StackTrace st = new StackTrace(ex, true);
        StackFrame frame = st.GetFrame(st.FrameCount - 1);
        int LogKey, Userk;
        string Company = string.Empty;
        if (HttpContext.Current.Session["UserKey"] == null) { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
        if (HttpContext.Current.Session["IDCompany"] == null) { Msj = Msj + "," + "Variable IDCompany null"; Company = "MGS"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
        if (HttpContext.Current.Session["LogKey"] == null) { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
        Msj = Msj + ex.Message;
        string nombreMetodo = frame.GetMethod().Name.ToString();
        int linea = frame.GetFileLineNumber();
        Msj = Msj + " || Metodo : AdmFacturas.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
        LogError(LogKey, Userk, " AdmFacturas.aspx.cs_" + nombreMetodo, Msj, Company);
        lblMsj.Text = ex.Message;
    }

    protected void ActualizarFacturas()
    {
        try
        {
            if (HttpContext.Current.Session["IDCompany"] != null)
            {
                int val1;
                val1 = 0;
                SqlConnection sqlConnection1 = new SqlConnection();
                sqlConnection1 = SqlConnectionDB("PortalConnection");
                sqlConnection1.Open();
                string pCompanyID = HttpContext.Current.Session["IDCompany"].ToString();
                string sSQL;

                sSQL = "spUpdateInvoice";
                List<SqlParameter> parsT = new List<SqlParameter>();
                parsT.Add(new SqlParameter("@Company", pCompanyID));

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
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

}


