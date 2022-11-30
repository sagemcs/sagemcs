//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA PARA PROVEEDOR DE CONSULTA DE FACTURAS CARGADAS AL PORTAL

//REFERENCIAS UTILIZADAS

using System;
using System.IO;
using uCFDsLib.v33;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using ConsultaSAT;
using System.Web;
using System.Web.UI;

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
    string eventName = String.Empty;

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
            Label4.Text = err;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);
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
            if (HttpContext.Current.Session["IDCompany"] == null)
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                {

                pVendKey = Convert.ToInt32(HttpContext.Current.Session["VendKey"].ToString());
                pLogKey = Convert.ToInt32(Session["LogKey"]);
                pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
                pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
                DatosV.Visible = true;
                //BindGrid();

            }
                else
                {
                    HttpContext.Current.Session.RemoveAll();
                    Context.GetOwinContext().Authentication.SignOut();
                    Response.Redirect("~/Account/Login.aspx");
                }
            }
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        try
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
            Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
            Page.Response.Cache.SetNoStore();
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (!isAuth)
            {
                HttpContext.Current.Session.RemoveAll();
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }

            Global.Docs();
            int Dias = Convert.ToInt16(HttpContext.Current.Session["Docs"].ToString());
            if (Dias == 0)
            {
                Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            }
            else if (Dias < 0)
            {
                Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            }
            else if (Dias == 30)
            {
                Page.MasterPageFile = "MenuPreP.master";
            }
            else if (Dias == 25)
            {
                Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            }
            else if (Dias == 26)
            {
                Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            }
            else if (Dias == 27)
            {
                Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            }
            else if (Dias == 28)
            {
                Page.MasterPageFile = "MenuPreP.master";
            }
            else if (Dias == 22)
            {
                Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            }
            else if (Dias <= 10 && Dias > 0)
            {
                Page.MasterPageFile = "MenuPreP.master";
            }
            else if (Dias > 10)
            {
                Page.MasterPageFile = "MenuPreP.master";
            }

            //if (Dias == 0)
            //{
            //    Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            //}
            //else if (Dias < 0)
            //{
            //    Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            //}
            //else if (Dias == 24)
            //{
            //    Page.MasterPageFile = "MenuPreP.master";
            //}
            //else if (Dias == 26)
            //{
            //    Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            //}
            //else if (Dias == 28)
            //{
            //    Page.MasterPageFile = "MenuPreP.master";
            //}
            //else if (Dias == 22)
            //{
            //    Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            //}
            //else if (Dias <= 10 && Dias > 0)
            //{
            //    Page.MasterPageFile = "MenuPreP.master";
            //}
            //else if (Dias > 10)
            //{
            //    Page.MasterPageFile = "MenuPreP.master";
            //}
            //if ((HttpContext.Current.Session["Docs"].ToString() == "1"))
            //{
            //    //Page.MasterPageFile = "MenuP.master";
            //    Global.RevDocs();
            //    if ((HttpContext.Current.Session["Docs"].ToString() == "1"))
            //    {
            //        //Page.MasterPageFile = "MenuP.master";
            //        Response.Redirect("~/Logged/Proveedores/Default.aspx", false);

            //    }
            //    //Response.Redirect("~/Logged/Proveedores/Default.aspx",false);
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

            sSQL = @"SELECT VendName FROM tapVendor WHERE VendKey = @varID and CompanyID=@CompanyID";


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
            LogError(pLogKey, pUserKey, "Carga-Factura:BindGridView", ex.Message, pCompanyID);
            return "XXXX";
        }

    }
    
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

                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = sSQL;

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

                if (Table.Rows.Count == 0) { DatosV.Visible = true; }


            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar procedimiento almacenado, Error: " + ex.ToString());
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

    protected void Buscar_Click1(object sender, EventArgs e)
    {
        DatosV.Visible = false;
        BindGrid();
    }

    protected void BindGrid()
    {
        try
        {
            DatosV.Visible = false;
            DateTime fechaini;
            DateTime fechafin;

            if (chkFechas.Checked)
            {
                fechaini = DateTime.Parse(txtdtp.Text);
                fechafin = DateTime.Parse(txtdtp.Text);
            }
            else
            {
                fechaini = DateTime.Parse("2000-01-01");
                fechafin = DateTime.Parse("2000-01-01");
            }

            BindGridInvoices(VendorID(Convert.ToString(pVendKey), pCompanyID), "", Folio.Text,"", NoOC.Text, Convert.ToInt32(dpEstatus.SelectedItem.Value), chkFechas.Checked ? 1 : 0, fechaini, fechafin, pCompanyID);
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:BindGridView", ex.Message, pCompanyID);
        }

    }

    protected void Limpiar_Click1(object sender, EventArgs e)
    {
        //IdProveedor.Text = "";
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

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {

            MemoryStream memoryStream = new MemoryStream();

            int index = Convert.ToInt32(e.CommandArgument);
            String archivo;

            GridViewRow row = gvFacturas.Rows[index];
            archivo = HttpUtility.HtmlDecode(row.Cells[1].Text);


            if (e.CommandName == "Documento_1" | e.CommandName == "Documento_2" | e.CommandName == "Documento_3")
            {
                if (e.CommandName == "Documento_1")
                {
                    memoryStream = databaseFileRead("InvoiceKey", row.Cells[0].Text, "FileBinary1");
                    archivo += ".xml";
                    HttpContext.Current.Response.ContentType = "text/xml";
                }
                if (e.CommandName == "Documento_2")
                {
                    memoryStream = databaseFileRead("InvoiceKey", row.Cells[0].Text, "FileBinary2");
                    archivo += ".pdf";
                    HttpContext.Current.Response.ContentType = "application/pdf";
                }
                if (e.CommandName == "Documento_3")
                {
                    memoryStream = databaseFileRead("InvoiceKey", row.Cells[0].Text, "FileBinary3");
                    archivo += ".pdf";
                    HttpContext.Current.Response.ContentType = "application/pdf";
                }

                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + archivo + "\"");
                HttpContext.Current.Response.AppendHeader("Content-Length", memoryStream.Length.ToString());
                HttpContext.Current.Response.BinaryWrite(memoryStream.ToArray());
                HttpContext.Current.Response.End();

            }
            else if (e.CommandName == "Aprobar")
            {
                InsertSage(row.Cells[0].Text, row.Cells[1].Text);
            }
            else if (e.CommandName == "Cancelar")
            {
                StatusFactura(row.Cells[0].Text, "3");
                BindGrid();
            }


        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:GridView1_RowCommand", ex.Message, pCompanyID);
        }
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
                    sqlQuery.CommandText = "UPDATE Invoice SET Status=" + Status + " ,AprovUserKey=" + pUserKey.ToString() + ",AprovDate=" + DateTime.Now.ToString("dd/MM/yyyy") + " WHERE InvoiceKey = " + InvoiceKey;

                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
            }

            return true;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
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

            if (consulta == "InvoiceKey")
                sql = @"SELECT " + columna + " FROM InvoiceFile WHERE InvoiceKey = @varID";
            else
                sql = @"SELECT " + columna + " FROM InvoiceFile WHERE InvcFileKey = @varID";


            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.Parameters.AddWithValue("@varID", InvoiceKey);
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        sqlQueryResult.Read();
                        var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                        sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                        //using (var fs = new MemoryStream(memoryStream, FileMode.Create, FileAccess.Write)) {
                        memoryStream.Write(blob, 0, blob.Length);
                        //}
                    }
            }

            sqlConnection1.Close();


            return memoryStream;
        }
        catch (Exception ex)
        {
            string err;
            err = ex.Message;

            return null;
        }
    }

    private void InsertSage(String InvoiceKey, String VendId)
    {
        try
        {
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

            sSQL = @"SELECT Folio,NodeOc,UUID,VendorKey FROM Invoice WHERE InvoiceKey = @varID";

            string Folio = "";
            string NodeOc = "";
            string UUID = "";
            string VendorKey = "";

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

                        }
                    }
            }

            sqlConnection1.Close();

            ///Consulta Sage

            sqlConnection1 = SqlConnectionDB("ConnectionString");
            sqlConnection1.Open();

            sSQL = "Select Count(*) from tapLoadFilesAP WHERE UUID =" + "'" + UUID.ToString() + "'";
            int Conteo = 0;

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
                    string err;
                    err = ex.Message;
                    return;
                }
            }

            sqlConnection1.Close();


            if (Conteo == 0)
            {
                sSQL = "Select Count(*) from tapVoucherCEx WHERE iTranNo =" + "'" + Folio + "'";

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
                            else
                            {
                                lblMsj1.Text = "Carga Exitosa";
                                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL2);", true);
                            }

                            sqlConnection1.Close();
                        }
                        catch (Exception ex)
                        {
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
                                lblMsj.Text = ("Error al cargar a Sage");
                                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
                                return;
                            }
                            else
                            {
                                lblMsj1.Text = "Carga Exitosa";
                                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL2);", true);
                            }

                            sqlConnection1.Close();
                        }
                        catch (Exception ex)
                        {
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
            lblMsj.Text = ex.Message;
            LogError(pLogKey, pUserKey, "Carga-Factura:GridView1_RowCommand", ex.Message, pCompanyID);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL1);", true);
        }
    }


    protected void dpEstatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindGrid();
    }
}