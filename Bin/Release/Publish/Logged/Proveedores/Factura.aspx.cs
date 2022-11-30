//PORTAL DE PROVEDORES T|SYS|
//09 - JULIO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA PARA CARGA DE FACTURAS  DE PROVEEDOR

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using c_Impuesto = uCFDsLib.v33.c_Impuesto;

//using ComprobanteComplemento = uCFDsLib.v40.ComprobanteComplemento;
//using ComprobanteAddenda = uCFDsLib.v40.ComprobanteAddenda;
//using ComprobanteConceptoImpuestos = uCFDsLib.v40.ComprobanteConceptoImpuestos;
//using c_Impuesto = uCFDsLib.v40.c_Impuesto;

public partial class Factura : System.Web.UI.Page
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
    //private object Factura;

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
            parsT.Add(new SqlParameter("@CompanyID", pCompanyID));

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

        try {

            if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
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

                    pVendKey = Convert.ToInt32(HttpContext.Current.Session["VendKey"].ToString());
                    pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
                    pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
                    pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());

                    //pVendKey = 0;
                    //pLogKey = 0;
                    //pUserKey = 0;
                    //pCompanyID = "";

                    if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                    {

                    }
                    else
                    {
                        HttpContext.Current.Session.RemoveAll();
                        Context.GetOwinContext().Authentication.SignOut();
                        Response.Redirect("~/Account/Login.aspx");
                    }


                    if (IsPostBackEventControlRegistered)
                    {
                       //HttpContext.Current.Session["Evento"] = null;
                        //Sage();
                    }                  

                }
                catch(Exception xD)
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
            else { }//CargaProv(); 
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
            
            /*
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
                
                */

        }
        catch(Exception ex)
        {
            HttpContext.Current.Session.RemoveAll();
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }

    }

    protected bool Bloqueo()
    {
        bool Bloqueos = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSuspender", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                int Opcion = 2;
                DateTime Desde;
                DateTime Hasta;
                DateTime Hoy = DateTime.Today;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Userkey", Value = 1 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@In1", Value = 1 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@In2", Value = 1 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Desde", Value = "" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Hasta", Value = "" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@In3", Value = 1 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@In4", Value = 1 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Desde1", Value = "" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Hasta1", Value = "" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = Opcion });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["FacIN"].ToString() == "1")
                    {
                        Desde = Convert.ToDateTime(rdr["Desde"].ToString());
                        Hasta = Convert.ToDateTime(rdr["Hasta"].ToString());
                        if (Desde <= Hoy && Hasta >= Hoy) { Bloqueos = true; }                        
                    }
                }
                conn.Close();
            }
        }
        catch (Exception ex)
        {
            Bloqueos = false;
        }

        return Bloqueos;
    }

    protected void CargaProv()
    {
        try
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectUserDoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                string Company = HttpContext.Current.Session["IDCompany"].ToString();
                int Opc = 1;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = Opc });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = "" });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Company });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SelProv.Items.Clear();
                //SelComp.Items.Clear();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Nombre"].ToString().Length > 1)
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Nombre"].ToString());
                        SelProv.Items.Insert(0, Linea);
                    }

                }

                conn.Close();
            }
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

    protected void RutinaError(Exception ex)
    {
        string Msj = string.Empty;
        StackTrace st = new StackTrace(ex, true);
        StackFrame frame = st.GetFrame(st.FrameCount - 1);
        int LogKey, Userk, VendK;
        string Company = string.Empty;
        if (HttpContext.Current.Session["UserKey"] == null) { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
        if (HttpContext.Current.Session["IDCompany"] == null) { Msj = Msj + "," + "Variable IDCompany null"; Company = "TSM"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
        if (HttpContext.Current.Session["VendKey"] == null) { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
        if (HttpContext.Current.Session["LogKey"] == null) { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
        Msj = Msj + ex.Message;
        string nombreMetodo = frame.GetMethod().Name.ToString();
        int linea = frame.GetFileLineNumber();
        Msj = Msj + " || Metodo : Administracion_Doc.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
        LogError(LogKey, Userk, " Administracion_Doc.aspx.cs_" + nombreMetodo, Msj, Company);
        //lblMsj.Text = ex.Message;
    }

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        eventName = "OnPreInit";
    }

    private void BindGridView(int InvoiceKey, int vendKey, string CompanyId)
    {
        // Get the connection string from Web.config.  
        // When we use Using statement,  
        // we don't need to explicitly dispose the object in the code,  
        // the using statement takes care of it. 
        try {

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                // Create a DataSet object. 
                DataSet dsFacturas = new DataSet();

                // Create a SELECT query. 
                string strSelectCmd = "select Invoice.InvoiceKey,UUID,FechaTransaccion from Invoice inner join InvoiceFile on Invoice.InvoiceKey = InvoiceFile.InvoiceKey WHERE Invoice.VendorKey =" + vendKey + " and Invoice.InvoiceKey=" + InvoiceKey + " and Invoice.CompanyID='" + CompanyId + "'";

                // Create a SqlDataAdapter object 
                // SqlDataAdapter represents a set of data commands and a  
                // database connection that are used to fill the DataSet and  
                // update a SQL Server database.  
                SqlDataAdapter da = new SqlDataAdapter(strSelectCmd, conn);

                // Open the connection 
                conn.Open();

                // Fill the DataTable named "Person" in DataSet with the rows 
                // returned by the query.new n 
                da.Fill(dsFacturas, "Invoice");


                // Get the DataView from Person DataTable. 
                DataView dvInvoice = dsFacturas.Tables["Invoice"].DefaultView;


                // Set the sort column and sort order. 
                //dvPerson.Sort = ViewState["SortExpression"].ToString();


                // Bind the GridView control. 
                gvFacturas.DataSource = dvInvoice;
                gvFacturas.DataBind();


            }
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:BindGridView", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
        }
    }

    private void BindLogGridView(int InvoiceKey, int vendKey, string CompanyId)
    {
        // Get the connection string from Web.config.  
        // When we use Using statement,  
        // we don't need to explicitly dispose the object in the code,  
        // the using statement takes care of it. 
        try
        {

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                // Create a DataSet object. 
                DataSet dsFacturas = new DataSet();

                // Create a SELECT query. 
                string strSelectCmd = "select Invoice.InvoiceKey,invoice.folio,Invoice.uuid,Invoice.NodeOc,ErrorValidacion,FechaError from Invoice inner join InvcLogValidacion on InvcLogValidacion.InvoiceKey = Invoice.InvoiceKey WHERE Invoice.VendorKey =" + vendKey + " and Invoice.InvoiceKey=" + InvoiceKey + " and Invoice.CompanyID='" + CompanyId + "'";

                // Create a SqlDataAdapter object 
                // SqlDataAdapter represents a set of data commands and a  
                // database connection that are used to fill the DataSet and  
                // update a SQL Server database.  
                SqlDataAdapter da = new SqlDataAdapter(strSelectCmd, conn);

                // Open the connection 
                conn.Open();

                // Fill the DataTable named "Person" in DataSet with the rows 
                // returned by the query.new n 
                da.Fill(dsFacturas, "Invoice");


                // Get the DataView from Person DataTable. 
                DataView dvInvoice = dsFacturas.Tables["Invoice"].DefaultView;


                // Set the sort column and sort order. 
                //dvPerson.Sort = ViewState["SortExpression"].ToString();


                // Bind the GridView control. 
                gvValidacion.DataSource = dvInvoice;
                gvValidacion.DataBind();


            }
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:BindGridView", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
        }
    }

    public void btnSage_Click(object sender, EventArgs e)
    {
        try
        {            
            Sage();
        }
        catch (Exception ex) 
        {
        
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
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:GridView1_RowCommand", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
        }
    }

    protected void GridView2_RowCommand(object sender, EventArgs e)
    {
        try
        {
            GridViewRow row = ((GridViewRow)((CheckBox)sender).NamingContainer);
            int index = row.RowIndex;
            CheckBox cb1 = (CheckBox)GridView1.Rows[index].FindControl("Check");
            TextBox tex = (TextBox)GridView1.Rows[index].FindControl("cant");

            if (cb1.Checked == true)
            {
                tex.Enabled = true;
            }
            else 
            {
                tex.Enabled = false;
            }
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:GridView2_RowCommand", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
        }
    }

    protected void Check(int index)
    {
        try
        {
            CheckBox Checka = (CheckBox)GridView1.Rows[index].Cells[1].FindControl("Check");
            //DataTable dt = new DataTable();

            //dt.Columns.Add("Cantidad");
            //dt.Columns.Add("Art");
            //dt.Columns.Add("ShortDesc");
            //dt.Columns.Add("UnitCost");
            //dt.Columns.Add("ExtAmt");

            //foreach (GridViewRow gvr in GridView1.Rows)
            //{
            //    if (Checka.Checked == true)
            //    {
            //        DataRow dr = dt.NewRow();
            //        dr["Status"] = "Aprobado";
            //        dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
            //        dr["Archivo"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
            //        dr["Fecha"] = gvr.Cells[4].Text.ToString();
            //        dr["Fecha"] = gvr.Cells[5].Text.ToString();
            //        dt.Rows.Add(dr);
            //    }
            //    else
            //    {
            //        DataRow dr = dt.NewRow();
            //        dr["Cantidad"] = "No Aprobado";
            //        dr["Art"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
            //        dr["ShortDesc"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
            //        dr["UnitCost"] = gvr.Cells[4].Text.ToString();
            //        dr["ExtAmt"] = gvr.Cells[5].Text.ToString();
            //        dt.Rows.Add(dr);

            //    }
            //}

            //gvFacturas.DataSource = dt;
            //gvFacturas.DataBind();
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

    protected void Btn_Buscar(object sender, EventArgs e)
    {
        try
        {
            if (NoOc.Text != "")
            {
                SelProv.Items.Clear();
                string SQL = string.Empty;
                string Company = HttpContext.Current.Session["IDCompany"].ToString();
                string Vendkey = HttpContext.Current.Session["VendKey"].ToString();

                SQL = "";
                SQL = " select d.POLineNo as PoLine,c.vendname as Proveedor,CAST(Round((l.qtyord - l.qtyInvcd),4) AS DECIMAL(20,4)) as Cantidad, a.ItemID As Art ,b.ShortDesc , d.UnitCost , d.ExtAmt ";
                SQL = SQL + " from tpopurchorder o ";
                SQL = SQL + " join tapvendor c on o.vendkey = c.vendkey ";
                SQL = SQL + " inner join tpoPOLine AS d on o.POKey = d.POKey ";
                SQL = SQL + " inner join tpoPOLinedist l on l.POLineKey = d.POLineKey ";
                SQL = SQL + " inner join timitem a on d.itemkey = a.itemkey ";
                SQL = SQL + " inner join timItemDescription b on b.itemkey = a.itemkey ";
                SQL = SQL + " where tranNo = '" + NoOc.Text + "' AND a.CompanyID = '" + Company + "' AND o.VendKey  = '" + Vendkey + "' AND CAST(Round((l.qtyord - l.qtyInvcd),4) AS DECIMAL(20,4)) > 0 ";

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                {
                        SqlCommand cmd = new SqlCommand(SQL, conn);
                        cmd.CommandType = CommandType.Text;
                        if (conn.State == ConnectionState.Open) {conn.Close();} conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        DataTable Registros = new DataTable();
                        Registros.Load(rdr);
                        int Filas = Registros.Rows.Count;
                        if (Filas == 0)
                        {
                            conn.Close();
                            string Caja = "B6";
                            Label4.Text = "No se encontró ninguna O.C. ligada a este Proveedor";
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
                        }
                        else if (Filas >= 1)
                        {
                            DataRow Fila = Registros.Rows[0];
                            ListItem Lin = new ListItem();
                            Lin.Value = (Fila["Proveedor"].ToString());
                            SelProv.Items.Insert(0, Lin);
                            llenatabla(Registros);
                        }
                        conn.Close();
                }
            }
            else
            {
               string Caja = "AID";
                Label4.Text = "Numero de Orden No Encontrada, Verificalo.";
               ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
            }
        }
        catch (Exception a)
        {
            string Eror = a.Message.ToString();
            string Caja = "B6";
            Label4.Text = Eror;
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
        }

    }

    protected void llenatabla(DataTable dts)
    {
        try
        {

            HttpContext.Current.Session["Error"] = "";
            Label4.Text = "";
            gvFacturas.DataSource = null;
            gvValidacion.DataSource = null;
            gvValidacion.Visible = false;
            gvFacturas.Visible = false;

            int i = 0;
            DataTable dt = new DataTable();
            dt.Columns.Add("PoLine");
            dt.Columns.Add("Cantidad");
            dt.Columns.Add("Art");
            dt.Columns.Add("ShortDesc");
            dt.Columns.Add("UnitCost");
            dt.Columns.Add("ExtAmt");

            string[] cantidad = new string[dts.Rows.Count];
            foreach (DataRow row in dts.Rows)
            {   
                DataRow dr = dt.NewRow();                
                cantidad[i] = HttpUtility.HtmlDecode(row["Cantidad"].ToString());

                decimal cantidadd = Convert.ToDecimal(HttpUtility.HtmlDecode(row["Cantidad"].ToString()));
                decimal preciouni = Convert.ToDecimal(HttpUtility.HtmlDecode(row["UnitCost"].ToString()));
                decimal montoline = Convert.ToDecimal(cantidadd * preciouni);

                dr["PoLine"] = HttpUtility.HtmlDecode(row["PoLine"].ToString());
                dr["Art"] = HttpUtility.HtmlDecode(row["Art"].ToString());
                dr["ShortDesc"] = HttpUtility.HtmlDecode(row["ShortDesc"].ToString());
                dr["UnitCost"] = HttpUtility.HtmlDecode(row["UnitCost"].ToString());
                //dr["ExtAmt"] = HttpUtility.HtmlDecode(row["ExtAmt"].ToString());
                dr["ExtAmt"] = Math.Round(Convert.ToDecimal(montoline), 4).ToString();
                dt.Rows.Add(dr);
                i++;
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();
            GridView1.Visible = true;
            i = 0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                TextBox cantidades = (TextBox)gvr.Cells[1].FindControl("cant");
                cantidades.Text = cantidad[i].ToString();
                cantidades.Enabled = false;
                i++;
            }

        }
        catch (Exception ex)
        {
            string err;
            err = ex.Message;

            LogError(pLogKey, pUserKey, "Carga-Factura:btnSage_Click", ex.Message, pCompanyID);
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B6);", true);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "hideshow();", true);
            return;
        }
    }

    protected void limpiargrids()
    {
        try
        {
            gvFacturas.DataSource = null;
            gvValidacion.DataSource = null;
            GridView1.DataSource = null;
            gvValidacion.Visible = false;
            gvFacturas.Visible = false;
            GridView1.Visible = false;
        }
        catch (Exception ex) 
        {
        
        }
    }

    protected int validaCantidades() 
    {
        int conteo = 0;
        try
        {
            
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                CheckBox Checka = (CheckBox)gvr.Cells[1].FindControl("Check");
                if (Checka.Checked == true)
                {
                    conteo++;
                    ///TextBox canti = (TextBox)gvr.Cells[1].FindControl("cant");
                }
                
            }
            
        }
        catch (Exception ex) 
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Valida Cantidades", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
        }
        return conteo;
    }

    protected string validaCantidades2()
    {
        string conteo = "";
        try
        {

            foreach (GridViewRow gvr in GridView1.Rows)
            {
                CheckBox Checka = (CheckBox)gvr.Cells[1].FindControl("Check");
                if (Checka.Enabled == true)
                {
                    TextBox canti2 = (TextBox)gvr.Cells[1].FindControl("cant");
                    string linea = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    string aticulo = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());                  
                    string OdeC = NoOc.Text;
                    string CompanyID = HttpContext.Current.Session["IDCompany"].ToString();
                    decimal CantidadO = 0;
                    decimal Cantidad1 = Convert.ToDecimal(canti2.Text);

                    SqlConnection sqlConnection1 = new SqlConnection();
                    sqlConnection1 = SqlConnectionDB("ConnectionString");
                    string sSQL;
                    sqlConnection1.Open();

                    string consulta = "Select CAST(Round((b.qtyord - b.qtyInvcd),4) AS DECIMAL(20,4)) as Cantidad ";
                    consulta += " From tpoPurchOrder c";
                    consulta += " inner join tpoPOLine a on a.POKey = c.POKey ";
                    consulta += " inner join tpoPOLineDist b on a.POLineKey = b.POLineKey ";
                    consulta += " inner join timItem d on a.ItemKey = d.ItemKey ";
                    consulta += " Where c.TranNo = @OC And c.CompanyId = @Company AND a.PoLineNo = @Linea";
                    sSQL = @"" + consulta;

                    using (var sqlQuery = new SqlCommand(sSQL, sqlConnection1))
                    {
                        sqlQuery.Parameters.AddWithValue("@OC", OdeC);
                        sqlQuery.Parameters.AddWithValue("@Company", CompanyID);
                        sqlQuery.Parameters.AddWithValue("@Linea", linea);

                        using (var sqlQueryResult = sqlQuery.ExecuteReader())
                        if (sqlQueryResult != null)
                        {
                           while (sqlQueryResult.Read())
                           {
                              CantidadO = Convert.ToDecimal(sqlQueryResult.GetValue(0));
                           }
                        }
                    }
                    sqlConnection1.Close();

                    if (Cantidad1 > CantidadO) 
                    {
                        conteo = "La Cantidad del articulo " + aticulo + " Excede de la Cantidad pendiente de Facturar de la Orden de Compra, Revisalo.";
                    }                    
                }
            }

        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Valida Cantidades", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
        }
        return conteo;
    }

  
    protected void Sage()
    {
        try
        {
           
            HttpContext.Current.Session["Error"] = "";
            Label4.Text = "";
            limpiargrids();            

            //valida NO OC
            if (NoOc.Text == "")
            {
                limpiargrids();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B10);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }

            //valida Folio Doc
            if (FolioF.Text == "")
            {
                limpiargrids();
                Label4.Text = "Ingrese Folio de Factura";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B6);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }

            if (validaCantidades() == 0) 
            {
                //limpiargrids();
                Label4.Text = "Debe de Seleccionar al menos un Articulo";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B6);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }
            string cants = validaCantidades2();
            if (cants != "")
            {
                //limpiargrids();
                Label4.Text = cants;
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B6);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }

            string error2 = HttpContext.Current.Session["Error"].ToString();

            //valida que se carguen los 3 Documentos
            //if (!FileUpload1.HasFile)
            //{
            //    gvFacturas.DataSource = null;
            //    gvValidacion.DataSource = null;
            //    gvValidacion.Visible = false;
            //    gvFacturas.Visible = false;
            //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B1);", true);
            //    System.Threading.Thread.Sleep(1000);
            //    return;
            //}
            if (!FileUpload2.HasFile)
            {
                limpiargrids();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B2);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }
            if (!FileUpload3.HasFile)
            {
                limpiargrids();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B3);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }

            //Validación del Formato del Archivo
            //if (FileUpload1.PostedFile.ContentType.ToString() != "text/xml")
            //{
            //    gvFacturas.DataSource = null;
            //    gvValidacion.DataSource = null;
            //    gvValidacion.Visible = false;
            //    gvFacturas.Visible = false;
            //    ////LimpiarCajas();                 
            //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B7);", true);
            //    System.Threading.Thread.Sleep(1000);
            //    return;

            //}
            if (FileUpload2.PostedFile.ContentType.ToString() != "application/pdf")
            {
                limpiargrids();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B8);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }
            if (FileUpload3.PostedFile.ContentType.ToString() != "application/pdf")
            {
                limpiargrids();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B9);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }

            //Validación del Tamaño
            //if (FileUpload1.PostedFile.ContentLength > 1000000 * 15)
            //{
            //    gvFacturas.DataSource = null;
            //    gvValidacion.DataSource = null;
            //    //LimpiarCajas();
            //    Label4.Text = "Archivo no admitido, el documento XML FACTURA supera el tamaño máximo permitido (15 MB), favor de verificar el archivo e intentar nuevamente.";
            //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B6);", true);
            //    System.Threading.Thread.Sleep(1000);
            //    return;
            //}

            if (FileUpload2.PostedFile.ContentLength > 1000000 * 15)
            {
                limpiargrids();
                Label4.Text = "Archivo no admitido, el documento PDF FACTURA supera el tamaño máximo permitido (15 MB), favor de verificar el archivo e intentar nuevamente.";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B6);", true);
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "hideshow();", true);
                return;
            }
            if (FileUpload3.PostedFile.ContentLength > 1000000 * 15)
            {
                limpiargrids();
                Label4.Text = "Archivo no admitido, el documento ANEXO PDF supera el tamaño máximo permitido (15 MB), favor de verificar el archivo e intentar nuevamente.";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B6);", true);
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "hideshow();", true);
                return;
            }

            string CompanyID = HttpContext.Current.Session["IDCompany"].ToString();
            int carga = databaseFilePutDB(FileUpload2.PostedFile.InputStream, FileUpload3.PostedFile.InputStream, pVendKey, 0, CompanyID);
            string error3 = HttpContext.Current.Session["Error"].ToString();

            if (carga == -1)
            {
                limpiargrids();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B4);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }

            //limpiargrids();

            MemoryStream memoryStream = new MemoryStream();
            memoryStream = databaseFileRead("InvcFileKey", Convert.ToString(carga), "FileBinary2");
            string error = HttpContext.Current.Session["Error"].ToString();

            if (error != "") { return; }

            if (CargarXML(memoryStream, carga))
            {  
                gvValidacion.DataSource = null;
                gvValidacion.Visible = false;
                gvFacturas.Visible = true;
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B5);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }
            else
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataBind();
                gvValidacion.Visible = true;
                gvFacturas.Visible = false;

                if (gvValidacion.Rows.Count > 0) 
                {
                    gvValidacion.Visible = true;
                    gvFacturas.Visible = false;
                }
 
                DelInvFiles(0,carga);
                Label4.Text = HttpContext.Current.Session["Error"].ToString();
                if (Label4.Text != "")
                {
                 ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B6);", true);
                 //System.Threading.Thread.Sleep(1000);
                }
                return;
            }
        }
        catch (Exception ex)
        {
            string err;
            err = ex.Message;
            
            LogError(pLogKey, pUserKey, "Carga-Factura:btnSage_Click", ex.Message, pCompanyID);
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B6);", true);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "hideshow();", true);
            return;
        }
    }

    private bool EmailC(int Opcion, string Destinatario)
    {
        bool Resut = false;
        try
        {

                string PassNew = "<div ID='Tabla'><div class='panel panel-success'><div class='table-responsive'><table class='table table-striped' id='table-to-result-ass-indic'><tbody> ";

                if (Opcion == 2)
                {
                    PassNew = PassNew + "<tr class='table-primary'><th><strong>Proveedor :  </strong>     </th><td> " + User.Identity.Name.ToString() + "</td></tr><br/>";
                }

                PassNew = PassNew + "</tbody></ table ></ div ></ div ></ div > ";

                    string body = string.Empty;

                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionFacts.html")))
                    {
                        body = reader.ReadToEnd();
                        body = body.Replace("{PassTemp}", PassNew);

                    }

            Global.EmailGlobal(Destinatario, body, "CARGA DE FACTURAS");

        }
        catch (Exception b)
        {
            Resut = false;
        }
        return Resut;
    }

    protected bool ListEm()
    {
        bool uno;
        uno = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spAddMail", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = "xx" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = "Notificación Facturas" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 1 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string Desti = HttpUtility.HtmlDecode(rdr["Destinatario"].ToString());

                    if (Desti != "" && Desti != "Usuario")
                    {
                        EmailC(2, Desti);
                    }
                }
            }
        }
        catch
        {

        }
        return uno;
    }

    protected string NuevoSAT(string RFC1, string RFC2, decimal Total, string UUID)
    {
        string Respuesta = string.Empty;
        try
        {
            var _url = "https://consultaqr.facturaelectronica.sat.gob.mx/ConsultaCFDIService.svc?wsdl";
            var _action = "http://tempuri.org/IConsultaCFDIService/Consulta";

            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(RFC1, RFC2, Total, UUID);
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();


            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                XmlDocument doc = new XmlDocument(); doc.LoadXml(soapResult); XmlNode myNode = doc.DocumentElement;
                if (doc.FirstChild.FirstChild.FirstChild.FirstChild.ChildNodes[2] == null)
                {
                    LogError(iLogKey, iUserKey, "Consulta SAT", "Nodo de respuesta Vacio", iCompanyID);
                    Respuesta = "Error de respuesta SAT";
                }
                else
                {
                    string stado = doc.FirstChild.FirstChild.FirstChild.FirstChild.ChildNodes[2].InnerText;
                    Respuesta = stado;
                }

            }
        }
        catch(Exception ex)
        {
            Respuesta = "Error de de Consulta SAT, error de conexión";
            LogError(iLogKey, iUserKey, "Consulta SAT", ex.Message, iCompanyID);
        }
        return Respuesta;
    }

    private static HttpWebRequest CreateWebRequest(string url, string action)
    {
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
        webRequest.Headers.Add("SOAPAction", action);
        webRequest.ContentType = "text/xml; charset=\"utf-8\"";
        webRequest.Accept = "text/xml";
        webRequest.Method = "POST";
        return webRequest;
    }

    private static XmlDocument CreateSoapEnvelope(string RFC1, string RFC2, decimal Total, string UUID)
    {
        string querySend = "?re=" + RFC1 + "&rr=" + RFC2 + "&tt=" + Total + "&id=" + UUID;
        XmlDocument soapEnvelop = new XmlDocument();
        soapEnvelop.LoadXml(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/""><soapenv:Header/><soapenv:Body><tem:Consulta><!--Optional:--><tem:expresionImpresa><![CDATA[" + querySend + "]]></tem:expresionImpresa></tem:Consulta></soapenv:Body></soapenv:Envelope>");
        return soapEnvelop;
    }

    private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
    {
        using (Stream stream = webRequest.GetRequestStream())
        {
            soapEnvelopeXml.Save(stream);
        }
    }

    protected int GetVkey() 
    {
        int vkey = 0;
        try
        {
            string SQL = string.Empty;
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            string prov = SelProv.SelectedItem.ToString();
            //string Company = iCompanyID;
            //Company = "IEP";

            SQL = "";
            SQL = SQL + " Select VendorKey From Vendors Where VendName =  '" + prov + "' AND CompanyID = '" + Company + "'";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.CommandType = CommandType.Text;
                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable Registros = new DataTable();
                Registros.Load(rdr);
                int Filas = Registros.Rows.Count;
                if (Filas >= 1)
                {
                    DataRow Fila = Registros.Rows[0];
                    vkey = Convert.ToInt32(Fila["VendorKey"].ToString());

                }
                conn.Close();
            }
        }
        catch (Exception ex) 
        {
        
        }
        return vkey;
    }

    protected void actuuid(string vkey, string rfc)
    {
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            string sSQL;
            sqlConnection1.Open();
            string uuid = rfc + "-" + vkey;

            string consulta = "Update invoice set UUID = '" + uuid + "' ";
            consulta += " Where invoicekey = @OC ";
            sSQL = @"" + consulta;

            using (var sqlQuery = new SqlCommand(consulta, sqlConnection1))
            {
                sqlQuery.Parameters.AddWithValue("@OC", vkey);
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {

                    }
            }
            sqlConnection1.Close();
        }
        catch (Exception ex)
        {

        }
    }

    private bool CargarXML(MemoryStream fs, int InvcFileKey)
    {
        try
        {
            int VendKey;
            int LogKey;
            int UserKey;
            string CompanyID;
            string error;

            error = HttpContext.Current.Session["Error"].ToString();
            VendKey = pVendKey;
            LogKey = pLogKey;
            UserKey = pUserKey;
            CompanyID = pCompanyID;

            error = HttpContext.Current.Session["Error"].ToString();
            UserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            //VendKey = GetVkey();
            CompanyID = HttpContext.Current.Session["IDCompany"].ToString();
            error = HttpContext.Current.Session["Error"].ToString();

            string folio = "";
            DateTime FechaTransaccion = DateTime.Now;
            DateTime FechaTimbre = DateTime.Now;
            string Moneda = "";
            string TipoComprobante = "";
            string NombreEmisor = "";
            string NombreReceptor = "";
            string RFCReceptor = "";
            string OdeC = "";
            decimal ImpuestoImporteTraslado = 0;
            decimal Subtotal = 0;
            decimal Total = 0;

            decimal importimp = 0;
            string strDescr = "";
            string strUnidad = "";
            string strItemIdSAT = "";

            decimal UnitCost = 0;
            decimal Monto = 0;
            decimal Cantidad = 0;
            decimal DescuentoItm = 0;

            int vkey;
            int vkeydtl;
            int Status = 1;
            int resultado = 0;
            int validacion = 0;
            int x = 0;
            string NFolio = string.Empty;
            string RFCEmisor1 = string.Empty;
            string Art = string.Empty;
            int Partidas = 0;


            //Obtiene Adenda
            OdeC = NoOc.Text;
            TipoComprobante = "I";
            strItemIdSAT = "01010101";
  
            ///consulta si esta cancelada entonces borrar tablas de facturacion
            //if (!ConsultaFacturaFol(OdeC))
            //{

            //}
            //else
            //{
            //    HttpContext.Current.Session["Error"] = "Ya has ingresado la Factura correspondiente a la Orden de Compra " + OdeC + " ";
            //    return false;
            //}
            error = HttpContext.Current.Session["Error"].ToString();
            try
            {
                SqlConnection sqlConnection1 = new SqlConnection();
                sqlConnection1 = SqlConnectionDB("ConnectionString");
                string sSQL;
                sqlConnection1.Open();

                string consulta = "Select top 1 d.VendDBA,e.FedID,d.VendName,e.CompanyName,' ' as Folio,c.TranDate,c.CurrID ,c.STaxAmt,c.PurchAmt,c.TranAmt  ";
                consulta += " From tpoPurchOrder c";
                //consulta += " inner join tpoPurchOrdVouch b on a.VoucherKey = b.VoucherKey";
                //consulta += " inner join tpoPurchOrder c on b.POKey = c.POKey";
                consulta += " inner join tapVendor d on c.VendKey = d.VendKey";
                consulta += " inner join tsmCompany e on c.CompanyID = e.CompanyID";
                consulta += " Where c.TranNo = @OC And c.CompanyId = @Company And d.Vendkey = @Vendkey";
                sSQL = @"" + consulta;

                using (var sqlQuery = new SqlCommand(sSQL, sqlConnection1))
                {
                    sqlQuery.Parameters.AddWithValue("@OC", OdeC);
                    sqlQuery.Parameters.AddWithValue("@Company", CompanyID);
                    sqlQuery.Parameters.AddWithValue("@Vendkey", VendKey);


                    using (var sqlQueryResult = sqlQuery.ExecuteReader())
                        if (sqlQueryResult != null)
                        {
                            while (sqlQueryResult.Read())
                            {
                                RFCEmisor1 = Convert.ToString(sqlQueryResult.GetValue(0));
                                RFCReceptor = Convert.ToString(sqlQueryResult.GetValue(1));
                                NombreEmisor = Convert.ToString(sqlQueryResult.GetValue(2));
                                NombreReceptor = Convert.ToString(sqlQueryResult.GetValue(3));
                                //folio = Convert.ToString(sqlQueryResult.GetValue(4));
                                folio = FolioF.Text;
                                FechaTransaccion = Convert.ToDateTime(sqlQueryResult.GetValue(5));
                                Moneda = Convert.ToString(sqlQueryResult.GetValue(6));
                                ImpuestoImporteTraslado = Convert.ToDecimal(sqlQueryResult.GetValue(7));
                                Subtotal = Convert.ToDecimal(sqlQueryResult.GetValue(8));
                                Total = Convert.ToDecimal(sqlQueryResult.GetValue(9));
                            }
                        }
                }

                sqlConnection1.Close();
                ///Consulta Sage
            }
            catch (Exception ex)
            {
                string err;
                err = ex.Message;
                HttpContext.Current.Session["Error"] = err;
                return false;
            }

            if (RFCEmisor1 == "")
            {
                HttpContext.Current.Session["Error"] = "No se encontraron datos relacionados a la O.C. " + OdeC + " ,Revisalo e intenta nuevamente";
                return false;
            }

            //Insertar Encabezado en BD
            Tuple<int, int> resutl = InsertInvoiceDB(VendKey, RFCEmisor1, RFCReceptor, NombreEmisor, NombreReceptor, folio, "", OdeC, "", FechaTransaccion, FechaTransaccion, Moneda, "", TipoComprobante, "", "", ImpuestoImporteTraslado, 0, 0, Subtotal, Total, 0, 1, Status, UserKey, CompanyID, LogKey);
            vkey = resutl.Item1;
            resultado = resutl.Item2;
            if (vkey < 0 & resultado != 1)
            {
                DelInvFiles(vkey, InvcFileKey);
                HttpContext.Current.Session["Error"] = "Error al Insertar Encabezado de Factura.";
                return false;
            }
            else
            {
                try
                {
                    actuuid(vkey.ToString(), RFCEmisor1);
                    foreach (GridViewRow gvr in GridView1.Rows)
                    {
                        CheckBox Checka = (CheckBox)gvr.Cells[1].FindControl("Check");
                        if (Checka.Checked == true) 
                        {
                            
                            TextBox canti = (TextBox)gvr.Cells[2].FindControl("cant");
                            string linea = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                            string montt = HttpUtility.HtmlDecode(gvr.Cells[7].Text.ToString());

                            SqlConnection sqlConnection1 = new SqlConnection();
                            sqlConnection1 = SqlConnectionDB("ConnectionString");
                            string sSQL;
                            sqlConnection1.Open();

                            string consulta = "Select d.ItemID,a.Description,a.UnitCost,b.QtyOrd,a.ExtAmt,0 as descuento,a.POLineNo ";
                            consulta += " From tpoPurchOrder c";
                            consulta += " inner join tpoPOLine a on a.POKey = c.POKey ";
                            consulta += " inner join tpoPOLineDist b on a.POLineKey = b.POLineKey ";
                            consulta += " inner join timItem d on a.ItemKey = d.ItemKey ";
                            consulta += " Where c.TranNo = @OC And c.CompanyId = @Company AND a.PoLineNo = @Linea";
                            sSQL = @"" + consulta;

                            using (var sqlQuery = new SqlCommand(consulta, sqlConnection1))
                            {
                                sqlQuery.Parameters.AddWithValue("@OC", OdeC);
                                sqlQuery.Parameters.AddWithValue("@Company", CompanyID);
                                sqlQuery.Parameters.AddWithValue("@Linea", linea);

                                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                                    if (sqlQueryResult != null)
                                    {
                                        while (sqlQueryResult.Read())
                                        {
                                            Art = Convert.ToString(sqlQueryResult.GetValue(0));
                                            strDescr = Convert.ToString(sqlQueryResult.GetValue(1));
                                            UnitCost = Convert.ToDecimal(sqlQueryResult.GetValue(2));
                                            //Cantidad = Convert.ToDecimal(sqlQueryResult.GetValue(3));
                                            Cantidad = Convert.ToDecimal(canti.Text);
                                            //Monto = Convert.ToDecimal(sqlQueryResult.GetValue(4));
                                            Monto = Convert.ToDecimal(montt);
                                            DescuentoItm = Convert.ToDecimal(sqlQueryResult.GetValue(5));
                                            Partidas = Convert.ToInt32(sqlQueryResult.GetValue(6));

                                            Tuple<int, int> resutlDtl = InsertInvoiceDtlDB(vkey, Art, strItemIdSAT, strUnidad, strDescr, UnitCost, Cantidad, Monto, DescuentoItm, Status, UserKey, CompanyID, LogKey, Partidas);
                                            vkeydtl = resutlDtl.Item1;
                                            resultado = resutlDtl.Item2;

                                            //VERIFICA la Insercion sea Correcta
                                            if (vkeydtl > 0 & resultado == 1)
                                            {
                                                Tuple<int, int> resutlDtlTaxT = InsertInvoiceDtlTaxDB(vkey, vkeydtl, importimp, "002", 0, "T", Status, UserKey, CompanyID, LogKey);
                                                int vkeydtltx = resutlDtlTaxT.Item1;
                                                resultado = resutlDtlTaxT.Item2;

                                                if (!(vkeydtltx > 0 & resultado == 1))
                                                {
                                                    DelInvFiles(vkey, InvcFileKey);
                                                    HttpContext.Current.Session["Error"] = "Error en impuesto del artículo" + Art;
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                DelInvFiles(vkey, InvcFileKey);
                                                HttpContext.Current.Session["Error"] = "Error al Insertar artículo de Factura";
                                                return false;
                                            }
                                        }
                                    }
                            }
                            sqlConnection1.Close();
                            ///Consulta Sage
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    string err;
                    err = ex.Message;
                    HttpContext.Current.Session["Error"] = err;
                    //Label4.Text = err;
                    LogError(pLogKey, pUserKey, "Carga-Factura: Obtener Encabezado Extranjero", ex.Message, pCompanyID);
                    //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B6);", true);
                    return false;
                }

                //Valida la inserscion de Datos Segun el Tipo de Comprobante
                if (TipoComprobante == "I")
                {
                    SincNcInv(vkey, 0, "IN");
                    validacion = ValidatetInvoiceDB(VendKey, vkey, CompanyID);
                }

                //validacion = 0; -- Carga Exitosa!
                if (validacion == 0)
                {
                    if (SincInvFiles(vkey, InvcFileKey))
                    {
                        BindGridView(vkey, pVendKey, pCompanyID);
                        return true;
                    }
                    else
                    {
                        DelInvFiles(vkey, InvcFileKey);
                        return false;
                    }
                }
                else
                {
                    CompanyID = HttpContext.Current.Session["IDCompany"].ToString();
                    BindLogGridView(vkey, VendKey, CompanyID);
                    DelInvFiles(vkey, InvcFileKey);
                    return false;
                }
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
    }

    private bool ConsultaFacturaFol(string UUID)
    {
        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            int VK = Convert.ToInt16(HttpContext.Current.Session["VendKey"].ToString());
            sqlConnection1.Open();

            sql = @"SELECT COUNT(*) As 'Cuenta' FROM Invoice WHERE Folio ='" + UUID + "' And VendorKey = " + VK;

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            if (Convert.ToInt32(Cuenta) > 0)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return false;
        }

    }

    private bool ConsultaFacturaDB(string UUID)
    {
        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            sql = @"SELECT COUNT(*) As 'Cuenta' FROM Invoice WHERE UUID ='" + UUID + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            if (Convert.ToInt32(Cuenta) > 0)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return false;
        }

    }


    private bool SincInvFiles(int InvoiceKey, int InvcFileKey)
    {
        try
        {

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "UPDATE InvoiceFile SET InvoiceKey= '" + InvoiceKey + "' WHERE InvcFileKey = '" + InvcFileKey + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
            }

            return true;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return false;
        }
    }

    private void SincNcInv(int InvoiceKey, int invk, String TranType)
    {
        try
        {

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            if (TranType == "IN")
                invk = InvoiceKey;


            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "UPDATE Invoice SET ApplyToInvcKey= " + invk + ", TranType='" + TranType + "' WHERE InvoiceKey = " + InvoiceKey;
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
            }


        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();

        }
    }

    private bool DelInvFiles(int InvoiceKey, int InvcFileKey)
    {
        try
        {

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "Delete from InvoiceFile WHERE InvcFileKey = '" + InvcFileKey + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "Delete from InvoiceTaxLine WHERE InvoiceKey = '" + InvoiceKey + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "Delete from InvoiceLines WHERE InvoiceKey = '" + InvoiceKey + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "Delete from Invoice WHERE InvoiceKey = '" + InvoiceKey + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
            }

            return true;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return false;
        }
    }
    
    private Tuple<int, int> InsertInvoiceDB(int VendorKey, String RFCEmisor, String RFCReceptor, String NombreEmisor, String NombreReceptor, String Folio, String Serie, String NodeOc, String UUID, DateTime FechaTransaccion, DateTime FechaTimbrado, String Moneda, String UsoCFDi, String TipoComprobante, String FormaPago, String MetodoPago, Decimal ImpuestoImporteTrs, Decimal ImpuestoImporteRtn, Decimal Descuento, Decimal Subtotal, Decimal Total, int Cancelado, int Timbrado, int Status, int UpdateUserKey, String CompanyID, int LogKey)
    {
        try {


            int vkey, val1;
            vkey = 0;
            val1 = 0;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            string sSQL;

            sSQL = "spapInsertInvoice";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@VendKey", VendorKey));
            parsT.Add(new SqlParameter("@RFCEmisor", RFCEmisor));
            parsT.Add(new SqlParameter("@RFCReceptor", RFCReceptor));
            parsT.Add(new SqlParameter("@NombreEmisor", NombreEmisor));
            parsT.Add(new SqlParameter("@NombreReceptor", NombreReceptor));
            parsT.Add(new SqlParameter("@Folio", Folio));
            parsT.Add(new SqlParameter("@Serie", Serie));
            parsT.Add(new SqlParameter("@NodeOc", NodeOc));
            parsT.Add(new SqlParameter("@UUID", UUID));
            parsT.Add(new SqlParameter("@FechaTransaccion", FechaTransaccion));
            parsT.Add(new SqlParameter("@FechaTimbrado", FechaTimbrado));
            parsT.Add(new SqlParameter("@Moneda", Moneda));
            parsT.Add(new SqlParameter("@UsoCFDi", UsoCFDi));
            parsT.Add(new SqlParameter("@TipoComprobante", TipoComprobante));
            parsT.Add(new SqlParameter("@FormaPago", FormaPago));
            parsT.Add(new SqlParameter("@MetodoPago", MetodoPago));
            parsT.Add(new SqlParameter("@ImpuestoImporteTrs", ImpuestoImporteTrs));
            parsT.Add(new SqlParameter("@ImpuestoImporteRtn", ImpuestoImporteRtn));
            parsT.Add(new SqlParameter("@Descuentos", Descuento));
            parsT.Add(new SqlParameter("@Subtotal", Subtotal));
            parsT.Add(new SqlParameter("@Total", Total));
            parsT.Add(new SqlParameter("@Timbrado", Timbrado));
            parsT.Add(new SqlParameter("@Cancelado", Cancelado));
            parsT.Add(new SqlParameter("@Status", Status));
            parsT.Add(new SqlParameter("@UpdateUserKey", UpdateUserKey));
            parsT.Add(new SqlParameter("@CompanyID", CompanyID));
            parsT.Add(new SqlParameter("@LogKey", LogKey));

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
                    vkey = rdr.GetInt32(0); //> 0 ok
                    val1 = rdr.GetInt32(1); //= 1-OK

                }

                sqlConnection1.Close();

            }

            var result = Tuple.Create<int, int>(vkey, val1);
            return result;

        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            var result = Tuple.Create<int, int>(-1, -1);//Error
            return result;
        }
    }

    private Tuple<int, int> InsertInvoiceDtlDB(int InvoiceKey, String Codigo, String ClaveProd, String ClaveUni, String Descripcion, decimal ValorUnitario, decimal Cantidad, decimal Importe, decimal Descuento, int status, int UpdateUserKey, String CompanyID, int LogKey,int Partida)
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

            sSQL = "spapInsertInvoiceDtl";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@InvoiceKey", InvoiceKey));
            parsT.Add(new SqlParameter("@NoPartida", Partida));
            parsT.Add(new SqlParameter("@Codigo", Codigo));
            parsT.Add(new SqlParameter("@ClaveProd", ClaveProd));
            parsT.Add(new SqlParameter("@ClaveUni", ClaveUni));
            parsT.Add(new SqlParameter("@Descripcion", Descripcion));
            parsT.Add(new SqlParameter("@ValorUnitario", ValorUnitario));
            parsT.Add(new SqlParameter("@Cantidad", Cantidad));
            parsT.Add(new SqlParameter("@Importe", Importe));
            parsT.Add(new SqlParameter("@Descuento", Descuento));
            parsT.Add(new SqlParameter("@status", status));
            parsT.Add(new SqlParameter("@UpdateUserKey", UpdateUserKey));
            parsT.Add(new SqlParameter("@CompanyID", CompanyID));
            parsT.Add(new SqlParameter("@LogKey", LogKey));


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

                    System.Data.SqlClient.SqlDataReader rdr = null;

                    rdr = Cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        vkey = rdr.GetInt32(0); //> 0 ok
                        val1 = rdr.GetInt32(1); //= 1-OK

                    }
                }


                catch (Exception ex)
                {
                    string err;
                    err = ex.Message;
                    sqlConnection1.Close();
                }
            }

            var result = Tuple.Create<int, int>(vkey, val1);

            sqlConnection1.Close();

            return result;

        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            var result = Tuple.Create<int, int>(-1, -1);//Error
            return result;
        }
    }

    private Tuple<int, int> InsertInvoiceDtlTaxDB(int InvoiceKey, int InvoicedtlKey, Decimal TaxAmt, String TaxID, decimal rate, String Tipo, int status, int UpdateUserKey, String CompanyID, int LogKey)
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

            sSQL = "spapInsertInvoiceDtlTax";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@InvoiceKey", InvoiceKey));
            parsT.Add(new SqlParameter("@InvoicedtlKey", InvoicedtlKey));
            parsT.Add(new SqlParameter("@TaxAmt", TaxAmt));
            parsT.Add(new SqlParameter("@TaxID", TaxID));
            parsT.Add(new SqlParameter("@rate", rate));
            parsT.Add(new SqlParameter("@Tipo", Tipo));
            parsT.Add(new SqlParameter("@status", status));
            parsT.Add(new SqlParameter("@UpdateUserKey", UpdateUserKey));
            parsT.Add(new SqlParameter("@LogKey", LogKey));
            parsT.Add(new SqlParameter("@CompanyID", CompanyID));

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

                    System.Data.SqlClient.SqlDataReader rdr = null;

                    rdr = Cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        vkey = rdr.GetInt32(0); //> 0 ok
                        val1 = rdr.GetInt32(1); //= 1-OK

                    }
                }


                catch (Exception ex)
                {
                    string err;
                    err = ex.Message;
                    sqlConnection1.Close();
                }
            }

            var result = Tuple.Create<int, int>(vkey, val1);
            return result;

        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            var result = Tuple.Create<int, int>(-1, -1);//Error
            return result;
        }
    }

    private int ValidatetInvoiceDB(int VendorKey, int InvoiceKey, String CompanyID)
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

            sSQL = "spapiValidateInvoice";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@VendKey", VendorKey));
            parsT.Add(new SqlParameter("@InvoiceKey", InvoiceKey));
            parsT.Add(new SqlParameter("@CompanyID", CompanyID));

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


                    System.Data.SqlClient.SqlDataReader rdr = null;

                    rdr = Cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        val1 = rdr.GetInt32(0); // 0 ok
                    }

                    sqlConnection1.Close();
                }


                catch (Exception ex)
                {
                    string err;
                    err = ex.Message;
                    sqlConnection1.Close();
                }
            }

            var result = Tuple.Create<int, int>(vkey, val1);
            return val1;

        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return -1;
        }
    }

    private int databaseFilePutDB(Stream fs2, Stream fs3, int VendorKey, int InvoiceKey, String CompanyID)
    {
        try
        {

            string val = "";
            int id = 0;

            //System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            //byte[] bytes = FileUpload1.FileBytes;

            System.IO.BinaryReader br2 = new System.IO.BinaryReader(fs2);
            byte[] bytes2 = FileUpload2.FileBytes;

            System.IO.BinaryReader br3 = new System.IO.BinaryReader(fs3);
            byte[] bytes3 = FileUpload3.FileBytes;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            using (var sqlWrite = new SqlCommand("Insert Into InvoiceFile (InvoiceKey,VendorKey,FileBinary2,FileBinary3,FileType2,FileType3,UpdateDate,UpdateUserKey,CompanyId) VALUES (@InvoiceKey,@VendorKey,@FileBinary2,@FileBinary3,@FileType2,@FileType3,@UpdateDate,@UpdateUserKey,@CompanyId);SELECT SCOPE_IDENTITY();", sqlConnection1))
            {
                sqlWrite.Parameters.Add("@InvoiceKey", SqlDbType.Int).Value = InvoiceKey;
                sqlWrite.Parameters.Add("@VendorKey", SqlDbType.Int).Value = VendorKey;
                //sqlWrite.Parameters.Add("@FileBinary1", SqlDbType.VarBinary, bytes2.Length).Value = bytes2;
                sqlWrite.Parameters.Add("@FileBinary2", SqlDbType.VarBinary, bytes2.Length).Value = bytes2;
                sqlWrite.Parameters.Add("@FileBinary3", SqlDbType.VarBinary, bytes3.Length).Value = bytes3;
                //sqlWrite.Parameters.Add("@FileType1", SqlDbType.VarChar, 5).Value = "xml";
                sqlWrite.Parameters.Add("@FileType2", SqlDbType.VarChar, 5).Value = "pdf";
                sqlWrite.Parameters.Add("@FileType3", SqlDbType.VarChar, 5).Value = "pdf";
                sqlWrite.Parameters.Add("@UpdateDate", SqlDbType.DateTime).Value = DateTime.Now;
                sqlWrite.Parameters.Add("@UpdateUserKey", SqlDbType.Int).Value = pUserKey;
                sqlWrite.Parameters.Add("@CompanyId", SqlDbType.VarChar, 3).Value = CompanyID;

                var modified = sqlWrite.ExecuteScalar();
                val = modified.ToString();
                id = Convert.ToInt32(val);

                sqlConnection1.Close();
            }

            return id;

        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = "Error de conexión SQL BD Portal";
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return -1;
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
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return null;
        }
    }

    protected void TextBox1_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow row = ((GridViewRow)((TextBox)sender).NamingContainer);
            int index = row.RowIndex;
            TextBox tex = (TextBox)GridView1.Rows[index].FindControl("cant");
            decimal cant = Convert.ToDecimal(tex.Text);
            if (cant > 0) 
            {
                decimal costo = Convert.ToDecimal(GridView1.Rows[index].Cells[6].Text);
                GridView1.Rows[index].Cells[7].Text = Math.Round(Convert.ToDecimal(cant * costo),4).ToString();
            }

        }
        catch (Exception ex) 
        {
        
        }
    }

}