//PORTAL DE PROVEDORES T|SYS|
//09 - JULIO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA PARA CARGA DE FACTURAS  DE PROVEEDOR

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

//using ComprobanteComplemento = uCFDsLib.v40.ComprobanteComplemento;
//using ComprobanteAddenda = uCFDsLib.v40.ComprobanteAddenda;
//using ComprobanteConceptoImpuestos = uCFDsLib.v40.ComprobanteConceptoImpuestos;
//using c_Impuesto = uCFDsLib.v40.c_Impuesto;

public partial class Facturas : System.Web.UI.Page
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
    private object Factura;

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
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
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
                    string Var = HttpContext.Current.Session["VendKey"].ToString();
                    string Var2 = HttpContext.Current.Session["RolUser"].ToString();
                    string Var3 = HttpContext.Current.Session["IDCompany"].ToString();

                    pVendKey = Convert.ToInt32(HttpContext.Current.Session["VendKey"].ToString());
                    pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
                    pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
                    pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());


                    if (IsPostBackEventControlRegistered)
                    {
                       HttpContext.Current.Session["Evento"] = null;
                       Sage();
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

            bool baja = Bloqueo();
            if (baja) 
            {
                Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            }

            int VendKey = Convert.ToInt32(HttpContext.Current.Session["VendKey"].ToString());

            bool Adenda = Global.VerTipoProv(VendKey);
            if (Adenda == true)
            {
                //HttpContext.Current.Session.RemoveAll();
                //Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Logged/Proveedores/Factura1.aspx", false);
            }

            bool Extranjero = Global.VerTipoProvE(VendKey);
            if (Extranjero == true)
            {
                //HttpContext.Current.Session.RemoveAll();
                //Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Logged/Proveedores/Factura.aspx", false);
            }

            //Global.Docs();
            //if ((HttpContext.Current.Session["Docs"].ToString() == "1"))
            //{
            //Page.MasterPageFile = "MenuP.master";
            //Response.Redirect("~/Logged/Proveedores/Default.aspx",false);
            //Global.RevDocs();

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
            //if ((HttpContext.Current.Session["Docs"].ToString() == "1"))
            //{
            //    //Page.MasterPageFile = "MenuP.master";
            //    Response.Redirect("~/Logged/Proveedores/Default.aspx", false);

            //}
            // }

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
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
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

    protected void LimpiarCajas()
    {
        try
        {
            //FileUpload1.ClearAllFilesFromPersistedStore();
            //FileUpload2.ClearAllFilesFromPersistedStore();
            //FileUpload3.ClearAllFilesFromPersistedStore();
        }
        catch (Exception ex)
        {
            HttpContext.Current.Session["Error"] = ex.Message;
        }
    }

    public void btnSage_Click(object sender, EventArgs e)
    {
        Sage();
    }

    protected void Sage()
    {
        try
        {
           
            HttpContext.Current.Session["Error"] = "";
            Label4.Text = "";
            gvFacturas.DataSource = null;
            gvValidacion.DataSource = null;
            gvValidacion.Visible = false;
            gvFacturas.Visible = false;

            //valida que se carguen los 3 Documentos
            if (!FileUpload1.HasFile)
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                gvValidacion.Visible = false;
                gvFacturas.Visible = false;
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B1);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }
            if (!FileUpload2.HasFile)
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                gvValidacion.Visible = false;
                gvFacturas.Visible = false;
                //LimpiarCajas();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B2);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }
            if (!FileUpload3.HasFile)
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                gvValidacion.Visible = false;
                gvFacturas.Visible = false;
                //LimpiarCajas();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B3);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }

            //Validación del Formato del Archivo
            if (FileUpload1.PostedFile.ContentType.ToString() != "text/xml")
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                gvValidacion.Visible = false;
                gvFacturas.Visible = false;
                ////LimpiarCajas();                 
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B7);", true);
                System.Threading.Thread.Sleep(1000);
                return;

            }
            if (FileUpload2.PostedFile.ContentType.ToString() != "application/pdf")
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                ////LimpiarCajas();                 
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B8);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }
            if (FileUpload3.PostedFile.ContentType.ToString() != "application/pdf")
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                gvValidacion.Visible = false;
                gvFacturas.Visible = false;
                ////LimpiarCajas();                 
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B9);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }

            //Validación del Tamaño
            if (FileUpload1.PostedFile.ContentLength > 1000000 * 15)
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                //LimpiarCajas();
                Label4.Text = "Archivo no admitido, el documento XML FACTURA supera el tamaño máximo permitido (15 MB), favor de verificar el archivo e intentar nuevamente.";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B6);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }
            if (FileUpload2.PostedFile.ContentLength > 1000000 * 15)
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                gvValidacion.Visible = false;
                gvFacturas.Visible = false;
                //LimpiarCajas();
                Label4.Text = "Archivo no admitido, el documento PDF FACTURA supera el tamaño máximo permitido (15 MB), favor de verificar el archivo e intentar nuevamente.";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B6);", true);
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "hideshow();", true);
                return;
            }
            if (FileUpload3.PostedFile.ContentLength > 1000000 * 15)
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                gvValidacion.Visible = false;
                gvFacturas.Visible = false;
                //LimpiarCajas(); 
                Label4.Text = "Archivo no admitido, el documento ANEXO PDF supera el tamaño máximo permitido (15 MB), favor de verificar el archivo e intentar nuevamente.";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B6);", true);
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "hideshow();", true);
                return;
            }


            int carga = databaseFilePutDB(FileUpload1.PostedFile.InputStream, FileUpload2.PostedFile.InputStream, FileUpload3.PostedFile.InputStream, pVendKey, 0, pCompanyID);

            if (carga == -1)
            {
                gvFacturas.DataSource = null;
                gvValidacion.DataSource = null;
                gvValidacion.Visible = false;
                gvFacturas.Visible = false;
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B4);", true);
                System.Threading.Thread.Sleep(1000);
                return;
            }

            gvFacturas.DataSource = null;
            gvValidacion.DataSource = null;
            gvValidacion.Visible = false;
            gvFacturas.Visible = false;

            MemoryStream memoryStream = new MemoryStream();
            memoryStream = databaseFileRead("InvcFileKey", Convert.ToString(carga), "FileBinary1");

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
                //gvFacturas.DataBind();
                gvValidacion.DataBind();
                gvValidacion.Visible = true;
                gvFacturas.Visible = false;

                ////LimpiarCajas();  
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
            //string err;
            //err = ex.Message;
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

    private bool CargarXML(MemoryStream fs, int InvcFileKey)
    {
        try
        {
            bool valido = false;
            bool complementovalido = false;
            bool adendavalida = false;

            int VendKey;
            int LogKey;
            int UserKey;
            string CompanyID;

            VendKey = pVendKey;
            LogKey = pLogKey;
            UserKey = pUserKey;
            CompanyID = pCompanyID;


            string folio = "";
            string serie = "";
            DateTime FechaTransaccion = DateTime.Now;
            DateTime FechaTimbre = DateTime.Now;
            string Moneda = "";
            string UsoCFDI = "";
            string TipoComprobante = "";
            string FormaPago = "";
            string MetodoPago = "";
            string UUID = "";

            string NombreEmisor = "";
            string RFCEmisor = "";
            string NombreReceptor = "";
            string RFCReceptor = "";
            string complemento = "";
            string OdeC = "";
            string tipo = "";
            string UUIDRel = "";

            decimal ImpuestoImporteTraslado = 0;
            decimal ImpuestoImporteRetenido = 0;
            decimal Descuento = 0;
            decimal Subtotal = 0;
            decimal Total = 0;

            decimal importimp = 0;
            decimal tasaimp = 0;
            decimal baseimp = 0;
            string strDescr = "";
            string strUnidad = "";
            string strItemIdSAT = "";

            decimal UnitCost = 0;
            decimal Monto = 0;
            decimal Cantidad = 0;
            decimal DescuentoItm = 0;
            int longImpTsl = 0;
            int longImpRtn = 0;
            string tipoImp = "";

            int vkey;
            int vkeydtl;
            int Cancelado = 0;
            int Timbrado = 0;
            int Status = 1;
            int resultado = 0;
            int validacion = 0;
            int ContServ = 0;
            int ContPro = 0;
            int x = 0;
            string NFolio = string.Empty;
            int indicador;

            StreamReader objStreamReader;
            StreamReader streamReader = null;
            TextReader reader = null;
            XmlSerializer Xml = null;
            XmlSerializer Xmls = null;

            uCFDsLib.v33.Comprobante  Factura  = new uCFDsLib.v33.Comprobante();
            uCFDsLib.v40.Comprobante  Facturas = new uCFDsLib.v40.Comprobante();

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
                    indicador = 1;
                }
                catch (Exception ex) 
                {
                    try
                    {
                        reader = new StringReader(xmlOutput);

                        Xmls = new XmlSerializer(Factura.GetType());
                        Factura = (uCFDsLib.v33.Comprobante)Xmls.Deserialize(reader);
                        indicador = 0;
                    }
                    catch(Exception exs)
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
            }  //Verifica estrucutra XML

            if (indicador == 1) 
            {
                return version4(Facturas, InvcFileKey);
                //return false;
            }

            string UUID1 = string.Empty;
            uCFDsLib.v33.TimbreFiscalDigital Base = new uCFDsLib.v33.TimbreFiscalDigital();

            #region validacion
            // Obtiene Complemento
            if (Factura.Complemento[0].Any != null && Factura.Complemento[0].Any.Length > 0)
            {
                int longitud;

                longitud = Factura.Complemento[0].Any.Length;
                ComprobanteComplemento cmp;
                cmp = Factura.Complemento[0];

                for (int i = 0; i < longitud; i++)
                {
                    XmlElement xe;
                    String nombre;

                    xe = cmp.Any[i];
                    nombre = xe.Name.ToString();

                    if (nombre == "tfd:TimbreFiscalDigital")
                    {
                        complemento = cmp.Any[i].OuterXml;

                        Stream s = new MemoryStream(System.Text.ASCIIEncoding.Default.GetBytes(complemento));
                        objStreamReader = new StreamReader(s);
                        Xml = new XmlSerializer(Base.GetType());
                        Base = (uCFDsLib.v33.TimbreFiscalDigital)Xml.Deserialize(objStreamReader);
                        objStreamReader.Close();

                        if (Base.UUID == null || Base.UUID.ToString().Length == 0)
                        {
                            HttpContext.Current.Session["Error"] = "El comprobante no cuenta con UUID.";
                            return false;
                        }

                        DateTime HoyT = DateTime.Now.AddMonths(-1);
                        if (Base.FechaTimbrado == null)
                        {
                            HttpContext.Current.Session["Error"] = "El comprobante no cuenta con Fecha de Timbrado";
                            return false;
                        }
                        //if ( Base.FechaTimbrado < HoyT)
                        //{
                        //    HttpContext.Current.Session["Error"] = "La Fecha de Timbrado del Documento no se encuentra dentro del período actual.";
                        //    return false;
                        //}
                        UUID = Base.UUID.ToString();
                        UUID1 = UUID;
                        FechaTimbre = Base.FechaTimbrado;
                        complementovalido = true;

                    }
                }
            }
            else
            {
                HttpContext.Current.Session["Error"] = "El Xml no tiene complemento.";
                return false;
            }

            //Obtiene Adenda
            if (Factura.Addenda == null) { HttpContext.Current.Session["Error"] = "El XML no contiene Adenda"; return false; }
            else
            {
                int longitud;
                int cnt = 0;
                longitud = Factura.Addenda.Any.Length;
                ComprobanteAddenda cmp;
                cmp = Factura.Addenda;
                for (int i = 0; i < longitud; i++)
                {
                    XmlElement xe;
                    String nombre;
                    xe = cmp.Any[i];
                    nombre = xe.Name.ToString();
                    var adenda = cmp.Any[i].OuterXml;
                    XmlDocument doc = new XmlDocument(); doc.LoadXml(adenda); XmlNode myNode = doc.DocumentElement;
                    if (doc.LastChild.Attributes["OdeC"] == null)
                    {
                        cnt = 0;
                    }
                    else
                    {
                        OdeC = doc.LastChild.Attributes["OdeC"].InnerText;
                        if (OdeC.Length == 0)
                        {
                            HttpContext.Current.Session["Error"] = "El número de compra no es correcto.";
                            return false;
                        }
                        else
                        {
                            cnt = 1;
                            break;
                        }
                    }
                }

                if (cnt == 0)
                {
                    HttpContext.Current.Session["Error"] = "La adenda no contiene el nodo Odec.";
                    return false;
                }

            }

            // Obtiene Datos Basicos para Verificar el Timbrado
            if (Factura.Receptor.Rfc == null || Factura.Receptor.Rfc.Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene RFC de Receptor."; return false; } else { RFCReceptor = Factura.Receptor.Rfc.ToString(); }
            if (Factura.Emisor.Rfc == null || Factura.Emisor.Rfc.Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene RFC de Emisor."; return false; } else { RFCEmisor = Factura.Emisor.Rfc.ToString(); }
            if (Factura.Total == 0 || Factura.Total.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Total"; return false; } else { Total = Factura.Total; }

            //Verifica timbrado
            string titulo, Msj, tipo1;
            string SAT = NuevoSAT(Factura.Emisor.Rfc, Factura.Receptor.Rfc, Factura.Total, UUID1);
            SAT = "Vigente";
            // Verifica Timbrado
            if (SAT != "Vigente")
            {
                string Resultado;
                if (SAT == "Cancelado") { Resultado = "Comprobante registrado ante el SAT como CANCELADO , verifica tu archivo"; }
                else { Resultado = "Comprobante no registrado ante el SAT, verifica tu archivo"; }
                Label4.Text = Resultado;
                titulo = "Servicio SAT";
                Msj = Resultado;
                tipo1 = "error";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo1 + "');", true);
                return false;
            }
            else
            {
                Timbrado = 1;
            }

            if (Factura.Folio == null || Factura.Folio.Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Folio."; return false; } else { folio = Factura.Folio.ToString(); }
            if (Factura.Serie == null) { serie = ""; } else { serie = Factura.Serie; }
            if (serie != null)
            {
                if (serie.Length >= 2)
                {
                    NFolio = NFolio + serie.Substring(0, 2);
                }
                else
                {
                    NFolio = NFolio + serie.ToString();
                }
            }

            if (Factura.Folio.Length > 10)
            {
                if (serie == "")
                {
                    if (Factura.Folio.Length > 10)
                    {
                        NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 10, 10).ToString();
                    }
                    else
                    {
                        NFolio = NFolio + Factura.Folio.ToString();
                    }
                    //NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 10, 10).ToString();
                }
                else
                {
                    if (Factura.Folio.Length > 8)
                    {
                        NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 8, 8).ToString();
                    }
                    else
                    {
                        NFolio = NFolio + Factura.Folio.ToString();
                    }                        
                }
                
            }
            else
            {
                if (serie == "")
                {
                    if (Factura.Folio.Length > 10)
                    {
                        NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 10, 10).ToString();
                    }
                    else
                    {
                        NFolio = NFolio + Factura.Folio.ToString();
                    }
                    //NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 10, 10).ToString();
                }
                else
                {
                    if (Factura.Folio.Length > 8)
                    {
                        NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 8, 8).ToString();
                    }
                    else
                    {
                        NFolio = NFolio + Factura.Folio.ToString();
                    }
                    //NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 8, 8).ToString();
                }
                //NFolio = NFolio + Factura.Folio.ToString();
            }

            folio = NFolio;

            //Consulta UUID No Repetido en Base de Datos
            if (ConsultaFacturaDB(UUID))
            {
                ///consulta si esta cancelada entonces borrar tablas de facturacion
                if (ConsultaFacturaStatusDB(UUID) == 3)
                {
                    int cvkey;
                    cvkey = ConsultaFacturaKeyDB(UUID);
                    if (!DelInvFilesCancel(cvkey))
                    {
                        HttpContext.Current.Session["Error"] = "Ocurrio un errror al reemplazar la factura cancelada " + UUID + " , consultar con soporte.";
                        return false;
                    }

                }
                else
                {
                    HttpContext.Current.Session["Error"] = "El UUID " + UUID + " ya se encuentra registrado";
                    return false;
                }
            }

            //Consulta Folio No Repetido en Base de Datos
            if (ConsultaFacturaFol(folio))
            {
                ///consulta si esta cancelada entonces borrar tablas de facturacion
                if (ConsultaFacturaStatusDB(UUID) == 3)
                {
                    int cvkey;
                    cvkey = ConsultaFacturaKeyDB(UUID);
                    if (!DelInvFilesCancel(cvkey))
                    {
                        HttpContext.Current.Session["Error"] = "Ocurrio un errror al reemplazar la factura cancelada " + UUID + " , consultar con soporte.";
                        return false;
                    }

                }
                else
                {
                    HttpContext.Current.Session["Error"] = "Ya has ingresado la Factura con el Folio " + folio + " ";
                    return false;
                }
            }

            //Informacion del contribuyente
            if (Factura.Emisor.Nombre == null || Factura.Emisor.Nombre.Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene nombre de Emisor."; return false; } else { NombreEmisor = Factura.Emisor.Nombre.ToString(); }
            if (Factura.Emisor.RegimenFiscal.ToString() == "Item000" || Factura.Emisor.RegimenFiscal.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Regimen Fiscal de Emisor."; return false; }

            //Informacion del Receptor
            if (Factura.Receptor.Nombre == null || Factura.Receptor.Nombre.Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene nombre de Receptor."; return false; } else { NombreReceptor = Factura.Receptor.Nombre.ToString(); }
            if (Factura.Receptor.UsoCFDI.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Uso de CFDI de Receptor."; return false; } else { UsoCFDI = Factura.Receptor.UsoCFDI.ToString(); }

            //Datos Generales (Impuestos, Metodo,Forma de Pago, Descuentos)
            if (Factura.FormaPagoSpecified == false) { HttpContext.Current.Session["Error"] = "El XML no contiene Forma de Pago"; return false; } else { FormaPago = Factura.FormaPago.ToString(); }
            if (Factura.MetodoPagoSpecified == false) { HttpContext.Current.Session["Error"] = "El XML no contiene Metodo de Pago"; return false; } else { MetodoPago = Factura.MetodoPago.ToString(); }
            if (Factura.Impuestos == null) { ImpuestoImporteTraslado = 0; ImpuestoImporteRetenido = 0; }
            else
            {
                if (Factura.Impuestos.TotalImpuestosTrasladadosSpecified && Factura.Impuestos.TotalImpuestosTrasladados > 0)
                {
                    ImpuestoImporteTraslado = Factura.Impuestos.TotalImpuestosTrasladados;
                }
                else
                {
                    ImpuestoImporteTraslado = 0;
                }

                if (Factura.Impuestos.TotalImpuestosRetenidosSpecified && Factura.Impuestos.TotalImpuestosRetenidos > 0)
                {
                    ImpuestoImporteRetenido = Factura.Impuestos.TotalImpuestosRetenidos;
                }
                else
                {
                    ImpuestoImporteRetenido = 0;
                }
            }
            if (Factura.Descuento.ToString().Length == 0) { Descuento = 0; } else { Descuento = Factura.Descuento; }

            //Datos del Comprobante
            DateTime Hoy = DateTime.Now.AddMonths(-1);
            if (Factura.Fecha < Hoy) { HttpContext.Current.Session["Error"] = "La Fecha del Documento no se encuentra dentro del período actual."; return false; } else { FechaTransaccion = Factura.Fecha; }
            if (Factura.SubTotal == 0 || Factura.SubTotal.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Sub Total"; return false; } else { Subtotal = Factura.SubTotal; }
            if (Factura.Moneda.ToString() == "TTT" || Factura.Moneda.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Moneda"; return false; } else { Moneda = Factura.Moneda.ToString(); }
            if (Factura.TipoDeComprobante.ToString() == "X" || Factura.TipoDeComprobante.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Tipo de Comprobante"; return false; }
            else
            {
                TipoComprobante = Factura.TipoDeComprobante.ToString();
                if (TipoComprobante == "E")
                {
                    if (Factura.CfdiRelacionados == null) { HttpContext.Current.Session["Error"] = "El Comprobante no contiene Cfdi Relacionado,revise."; return false; }
                    var cta = Factura.CfdiRelacionados.CfdiRelacionado.Length;

                    for (int r = 0; r < cta; r++)
                    {
                        UUIDRel = Factura.CfdiRelacionados.CfdiRelacionado[r].UUID.ToString();
                    }
                }
            }
            if (TipoComprobante == "E")
            {
                if (!ConsultaFacturaDB(UUIDRel))
                {
                    HttpContext.Current.Session["Error"] = "El Comprobante al que hace referencia la nota de crédito, no está registrado.";
                    return false;
                }
            }

            //Insertar Encabezado en BD
            Tuple<int, int> resutl = InsertInvoiceDB(VendKey, RFCEmisor, RFCReceptor, NombreEmisor, NombreReceptor, folio, serie, OdeC, UUID, FechaTransaccion, FechaTimbre, Moneda, UsoCFDI, TipoComprobante, FormaPago, MetodoPago, ImpuestoImporteTraslado, ImpuestoImporteRetenido, Descuento, Subtotal, Total, Cancelado, Timbrado, Status, UserKey, CompanyID, LogKey);
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

                if (Factura.Conceptos == null) { HttpContext.Current.Session["Error"] = "El XML no contiene Conceptos"; return false; }
                if (Factura.Conceptos.Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Conceptos"; return false; }
                else
                {
                    string[] ListaID = new string[Factura.Conceptos.Length];
                    int[] Partida = new int[Factura.Conceptos.Length];

                    for (int i = 0; i < Factura.Conceptos.Length; i++)
                    {
                        if (Factura.Conceptos[i].ValorUnitario.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El Concepto " + i + " no contiene Valor Unitario"; return false; } else { UnitCost = Factura.Conceptos[i].ValorUnitario; }
                        if (Factura.Conceptos[i].Importe.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El Concepto " + i + " no contiene Importe"; return false; } else { Monto = Factura.Conceptos[i].Importe; }
                        if (Factura.Conceptos[i].Cantidad.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El Concepto " + i + " no contiene Cantidad"; return false; } else { Cantidad = Factura.Conceptos[i].Cantidad; }
                        if (Factura.Conceptos[i].Descripcion == null) { HttpContext.Current.Session["Error"] = "El Concepto " + i + " no contiene Descripción"; return false; }
                        else
                        {
                            string strDescrr = Factura.Conceptos[i].Descripcion.ToString();
                            string[] Arreglo = strDescrr.Split(' '); //Genera Arreglo de la Descripcion y obtiene los Datos 
                            string Art = Arreglo[0].ToString(); // ID De Articulo
                            string Cuentas;
                            //int Conteo = 0;

                            //Verifica Impuestos de los Conceptos
                            if (Factura.Conceptos[i].Impuestos != null)
                            {
                                ComprobanteConceptoImpuestos Imp = Factura.Conceptos[i].Impuestos;
                                if (Imp.Traslados != null)
                                {
                                    longImpTsl = Imp.Traslados.Length;
                                    if (longImpTsl > 0)
                                    {
                                        for (int j = 0; j < longImpTsl; j++)
                                        {

                                            importimp = Imp.Traslados[j].Importe;
                                            tasaimp = Imp.Traslados[j].TasaOCuota;
                                            baseimp = Imp.Traslados[j].Base;
                                            tipoImp = Imp.Traslados[j].Impuesto.ToString();

                                            if (!Enum.IsDefined(typeof(c_Impuesto), tipoImp.ToString()))
                                            {
                                                DelInvFiles(vkey, InvcFileKey);
                                                HttpContext.Current.Session["Error"] = "El impuesto del artículo" + Art + " no es válido:" + tipoImp;
                                                return false;
                                            }

                                            if (tasaimp > 0)
                                            {
                                                if (importimp == 0)
                                                {
                                                    DelInvFiles(vkey, InvcFileKey);
                                                    HttpContext.Current.Session["Error"] = "El impuesto del artículo" + Art + " no es válido:" + tipoImp;
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (Imp.Retenciones != null)
                                {
                                    longImpRtn = Imp.Retenciones.Length;
                                    if (longImpTsl > 0)
                                    {
                                        for (int j = 0; j < longImpRtn; j++)
                                        {
                                            importimp = Imp.Retenciones[j].Importe;
                                            tasaimp = Imp.Retenciones[j].TasaOCuota;
                                            tipoImp = Imp.Retenciones[j].Impuesto.ToString();

                                            if (!Enum.IsDefined(typeof(c_Impuesto), tipoImp.ToString()))
                                            {
                                                DelInvFiles(vkey, InvcFileKey);
                                                HttpContext.Current.Session["Error"] = "El impuesto del artículo" + strItemIdSAT + " no es válido:" + tipoImp;
                                                return false;
                                            }

                                            if (tasaimp > 0)
                                            {
                                                if (importimp == 0)
                                                {
                                                    DelInvFiles(vkey, InvcFileKey);
                                                    HttpContext.Current.Session["Error"] = "El impuesto del artículo" + strItemIdSAT + " no es válido:" + tipoImp;
                                                    return false;
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                            if (Factura.Conceptos[i].DescuentoSpecified)
                            {
                                DescuentoItm = Factura.Conceptos[i].Descuento;
                            }

                            SqlConnection sqlConnection1 = new SqlConnection();
                            sqlConnection1 = SqlConnectionDB("ConnectionString");
                            sqlConnection1.Open();

                            try
                            {
                                string sql1 = @"select Count(*) From timItem where ItemID = '" + Art + "' and CompanyID = '" + pCompanyID + "'";
                                using (var sqlQuery = new SqlCommand(sql1, sqlConnection1))
                                {
                                    sqlQuery.CommandType = CommandType.Text;
                                    sqlQuery.CommandText = sql1;
                                    Cuentas = sqlQuery.ExecuteScalar().ToString();
                                }

                                if (Convert.ToInt16(Cuentas) == 0)
                                {
                                    DelInvFiles(vkey, InvcFileKey);
                                    HttpContext.Current.Session["Error"] = "No se encontro articulo con ID " + Art + " ,Verificalo con la Orden de compra.";
                                    Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                    return false;
                                }

                            }
                            catch (Exception ex)
                            {
                                LogError(pLogKey, pUserKey, "Carga-Factura:ConsultaTipoArt", ex.Message, pCompanyID);
                                string err;
                                err = ex.Message;
                                DelInvFiles(vkey, InvcFileKey);
                                HttpContext.Current.Session["Error"] = "No se encontro articulo con ID " + Art + ", el comprobante no puede ser procesado, Verificalo con la Orden de compra.";
                                Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                return false;
                            }

                            try
                            {
                                string Ssql = "Select Status From TimItem Where ItemID = '" + Art + "' AND CompanyID = '" + pCompanyID + "'";
                                string Estado = string.Empty;

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
                                    DelInvFiles(vkey, InvcFileKey);
                                    HttpContext.Current.Session["Error"] = "El articulo  " + Art + "  se encuentra con estatus Inactivo, Comunícate con el área de Compras.";
                                    Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                    return false;
                                }

                                if (Estado == "4")
                                {
                                    DelInvFiles(vkey, InvcFileKey);
                                    HttpContext.Current.Session["Error"] = "El articulo  " + Art + "  se encuentra con estatus Borrado, Comunícate con el área de Compras.";
                                    Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                    return false;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogError(pLogKey, pUserKey, "Carga-Factura:ConsultaTipoArt", ex.Message, pCompanyID);
                                string err;
                                err = ex.Message;
                                DelInvFiles(vkey, InvcFileKey);
                                HttpContext.Current.Session["Error"] = "El articulo con ID " + Art + " Se encuentra con estatus Invalido, el comprobante no puede ser procesado, Verificalo con la Orden de compra.";
                                Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                return false;

                            }


                            string Clas = ConsultaTipoArt(Arreglo[0].ToString());
                            if (Clas == "")
                            {
                                LogError(pLogKey, pUserKey, "Carga-Factura:CargaXML", "No se encontro clase de Producto para la Partida " + Art, pCompanyID);
                                DelInvFiles(vkey, InvcFileKey);
                                HttpContext.Current.Session["Error"] = "El artículo " + Art + " no cuenta con una clase de Producto valida en SAGE,el comprobante no puede ser procesado, contacte al Área de compras.";
                                return false;
                            }
                            else
                            {
                                if (Clas == "S" || Clas == "s")
                                {
                                    try
                                    {
                                        int Part = Convert.ToInt32(Arreglo[1].ToString()); //Obtiene Numero de Partida del Servicio
                                        ContServ = ContServ + 1; //Contador de Servicios

                                        if (Part ==0)
                                        {
                                            DelInvFiles(vkey, InvcFileKey);
                                            HttpContext.Current.Session["Error"] = "El Comprobante contiene el Numeros de Partida 0, esto no esta permitido , el comprobante no puede ser procesado, Verificalo con la Orden de Compra";
                                            Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                            return false;
                                        }

                                        for (int a = 0; a <=i ; a++)
                                        {
                                            if (Partida[a] == Part)
                                            {
                                                DelInvFiles(vkey, InvcFileKey);
                                                HttpContext.Current.Session["Error"] = "El Comprobante contiene el Numeros de Partida Duplicados , el comprobante no puede ser procesado, Verificalo con la Orden de Compra";
                                                Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                                return false;
                                            }
                                        }

                                        Partida[i] = Convert.ToInt32(Arreglo[1].ToString()); //Obtiene Numero de Partida del Servicio
                                    }
                                    catch (Exception ex)
                                    {
                                        string err = ex.Message;
                                        DelInvFiles(vkey, InvcFileKey);
                                        LogError(pLogKey, pUserKey, "Carga-Factura:CargaXML", "Error al Obtener Partida en Item " + Art + "Factura " + folio, pCompanyID);
                                        HttpContext.Current.Session["Error"] = "Se Genero un Error al intententar obtener el numero de partida en el Articulo " + Art + " ,verificalo con la Orden de Compra" + "<br/>";
                                        return false;
                                    }
                                }
                                else
                                {
                                    ContPro = ContPro + 1; //Contador de Productivos

                                    for (int a = 0; a <= i; a++)
                                    {
                                        if (ListaID[a] == Art)
                                        {
                                            DelInvFiles(vkey, InvcFileKey);
                                            HttpContext.Current.Session["Error"] = "El Comprobante contiene partes de productos Duplicados, el comprobante no puede ser procesado, Verificalo con la Orden de Compra";
                                            Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                            return false;
                                        }
                                    }

                                    ListaID[i] = Art; //ID de Articulo
                                    Partida[i] = 0; // Asigna Valor 0 Por ser Productivo y se sabe que es Productivo
                                }
                              
                            }

                            // Determina si Todas las partidas son Productivo o son Servicios
                            if (ContPro >= 1 && ContServ >= 1)
                            {
                                DelInvFiles(vkey, InvcFileKey);
                                HttpContext.Current.Session["Error"] = "El Comprobante contiene partes de productos y de servicios, no puede ser procesado, contacte al área de compras.";
                                return false;
                            }

                            //Inserta Detalle en BS
                            if (Factura.Conceptos[i].ClaveProdServ == null) { strItemIdSAT = "01010101"; } else { strItemIdSAT = Factura.Conceptos[i].ClaveProdServ.ToString().Substring(Factura.Conceptos[i].ClaveProdServ.ToString().Length - 8, 8); }

                            Tuple<int, int> resutlDtl = InsertInvoiceDtlDB(vkey, Art, strItemIdSAT, strUnidad, strDescr, UnitCost, Cantidad, Monto, DescuentoItm, Status, UserKey, CompanyID, LogKey, Partida[i]);
                            vkeydtl = resutlDtl.Item1;
                            resultado = resutlDtl.Item2;

                            //VERIFICA la Insercion sea Correcta
                            if (vkeydtl > 0 & resultado == 1)
                            {
                                if (Factura.Conceptos[i].Impuestos != null)
                                {
                                    ComprobanteConceptoImpuestos Imp = Factura.Conceptos[i].Impuestos;
                                    if (Imp.Traslados != null)
                                    {
                                        longImpTsl = Imp.Traslados.Length;

                                        if (longImpTsl > 0)
                                        {
                                            for (int j = 0; j < longImpTsl; j++)
                                            {
                                                importimp = Imp.Traslados[j].Importe;
                                                tasaimp = Imp.Traslados[j].TasaOCuota;
                                                tipoImp = Imp.Traslados[j].Impuesto.ToString().Substring(Imp.Traslados[j].Impuesto.ToString().Length - 3, 3);

                                                Tuple<int, int> resutlDtlTaxT = InsertInvoiceDtlTaxDB(vkey, vkeydtl, importimp, tipoImp, tasaimp, "T", Status, UserKey, CompanyID, LogKey);
                                                int vkeydtltx = resutlDtlTaxT.Item1;
                                                resultado = resutlDtlTaxT.Item2;

                                                if (!(vkeydtltx > 0 & resultado == 1))
                                                {
                                                    DelInvFiles(vkey, InvcFileKey);
                                                    HttpContext.Current.Session["Error"] = "Error en impuesto del artículo" + strItemIdSAT;
                                                    return false;
                                                }

                                            }
                                        }
                                    }
                                    if (Imp.Retenciones != null)
                                    {

                                        longImpRtn = Imp.Retenciones.Length;

                                        if (longImpTsl > 0)
                                        {
                                            for (int j = 0; j < longImpRtn; j++)
                                            {
                                                importimp = Imp.Retenciones[j].Importe;
                                                tasaimp = Imp.Retenciones[j].TasaOCuota;
                                                tipoImp = Imp.Retenciones[j].Impuesto.ToString().Substring(Imp.Retenciones[j].Impuesto.ToString().Length - 3, 3);

                                                Tuple<int, int> resutlDtlTaxR = InsertInvoiceDtlTaxDB(vkey, vkeydtl, importimp, tipoImp, tasaimp, "R", Status, UserKey, CompanyID, LogKey);
                                                int vkeydtltx = resutlDtlTaxR.Item1;
                                                resultado = resutlDtlTaxR.Item2;

                                                if (!(vkeydtltx > 0 & resultado == 1))
                                                {
                                                    DelInvFiles(vkey, InvcFileKey);
                                                    HttpContext.Current.Session["Error"] = "Error en impuesto del artículo" + strItemIdSAT;
                                                    return false;
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                DelInvFiles(vkey, InvcFileKey);
                                HttpContext.Current.Session["Error"] = "Error al Insertar artículo de Factura";
                                return false;
                            }
                        }

                    } // Termina For
                }

                //Valida la inserscion de Datos Segun el Tipo de Comprobante
                if (TipoComprobante == "I")
                {
                    SincNcInv(vkey, 0, "IN");
                    validacion = ValidatetInvoiceDB(VendKey, vkey, CompanyID);
                }

                else if (TipoComprobante == "E")
                {
                    int invk = ConsultaFacturaKeyDB(UUIDRel);
                    SincNcInv(vkey, invk, "CM");
                    validacion = ValidateNCDB(VendKey, vkey, CompanyID);
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
                    BindLogGridView(vkey, pVendKey, pCompanyID);
                    DelInvFiles(vkey, InvcFileKey);
                    return false;
                }
            }
#endregion
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

    private bool version4(uCFDsLib.v40.Comprobante Factura, int InvcFileKey)
    {
        bool valor = true;

        bool valido = false;
        bool complementovalido = false;
        bool adendavalida = false;

        int VendKey;
        int LogKey;
        int UserKey;
        string CompanyID;

        VendKey = pVendKey;
        LogKey = pLogKey;
        UserKey = pUserKey;
        CompanyID = pCompanyID;


        string folio = "";
        string serie = "";
        DateTime FechaTransaccion = DateTime.Now;
        DateTime FechaTimbre = DateTime.Now;
        string Moneda = "";
        string UsoCFDI = "";
        string TipoComprobante = "";
        string FormaPago = "";
        string MetodoPago = "";
        string UUID = "";

        string NombreEmisor = "";
        string RFCEmisor = "";
        string NombreReceptor = "";
        string RFCReceptor = "";
        string complemento = "";
        string OdeC = "";
        string tipo = "";
        string UUIDRel = "";

        decimal ImpuestoImporteTraslado = 0;
        decimal ImpuestoImporteRetenido = 0;
        decimal Descuento = 0;
        decimal Subtotal = 0;
        decimal Total = 0;

        decimal importimp = 0;
        decimal tasaimp = 0;
        decimal baseimp = 0;
        string strDescr = "";
        string strUnidad = "";
        string strItemIdSAT = "";

        decimal UnitCost = 0;
        decimal Monto = 0;
        decimal Cantidad = 0;
        decimal DescuentoItm = 0;
        int longImpTsl = 0;
        int longImpRtn = 0;
        string tipoImp = "";

        int vkey;
        int vkeydtl;
        int Cancelado = 0;
        int Timbrado = 0;
        int Status = 1;
        int resultado = 0;
        int validacion = 0;
        int ContServ = 0;
        int ContPro = 0;
        int x = 0;
        string NFolio = string.Empty;
        int indicador;

        StreamReader objStreamReader;
        StreamReader streamReader = null;
        TextReader reader = null;
        XmlSerializer Xml = null;

        //uCFDsLib.v40.Comprobante Factura = new uCFDsLib.v40.Comprobante();

        try
        {

            ////string xmlOutput = string.Empty;
            ////fs.Position = 0;
            ////streamReader = new StreamReader(fs);
            ////xmlOutput = streamReader.ReadToEnd();
            ////streamReader.Close();

            ////reader = new StringReader(xmlOutput);
            ////Xml = new XmlSerializer(Factura.GetType());
            ////Factura = (uCFDsLib.v40.Comprobante)Xml.Deserialize(reader);
        }
        catch (Exception ex)
        {
            //LOG Err
            ////HttpContext.Current.Session["Error"] = "Tu archivo no tiene la estructura valida por el SAT.";
            ////string Mensaje = "Error al Deserializar el Archivo ";
            ////Mensaje = Mensaje + ex.Message;
            ////if (ex.InnerException != null)
            ////{
            ////    Mensaje = Mensaje + " || " + ex.InnerException;
            ////}
            ////LogError(iLogKey, iUserKey, "Carga de Factura_CargarXML()", Mensaje, iCompanyID);
            ////return false;
        }  //Verifica estrucutra XML

        string UUID1 = string.Empty;
        uCFDsLib.v40.TimbreFiscalDigital Base = new uCFDsLib.v40.TimbreFiscalDigital();

        #region validacion
        // Obtiene Complemento
        if (Factura.Complemento[0].Any != null && Factura.Complemento[0].Any.Length > 0)
        {
            int longitud;

            longitud = Factura.Complemento[0].Any.Length;
            uCFDsLib.v40.ComprobanteComplemento cmp;
            cmp = Factura.Complemento[0];

            for (int i = 0; i < longitud; i++)
            {
                XmlElement xe;
                String nombre;

                xe = cmp.Any[i];
                nombre = xe.Name.ToString();

                if (nombre == "tfd:TimbreFiscalDigital")
                {
                    complemento = cmp.Any[i].OuterXml;

                    Stream s = new MemoryStream(System.Text.ASCIIEncoding.Default.GetBytes(complemento));
                    objStreamReader = new StreamReader(s);
                    Xml = new XmlSerializer(Base.GetType());
                    Base = (uCFDsLib.v40.TimbreFiscalDigital)Xml.Deserialize(objStreamReader);
                    objStreamReader.Close();

                    if (Base.UUID == null || Base.UUID.ToString().Length == 0)
                    {
                        HttpContext.Current.Session["Error"] = "El comprobante no cuenta con UUID.";
                        return false;
                    }

                    DateTime HoyT = DateTime.Now.AddMonths(-1);
                    if (Base.FechaTimbrado == null)
                    {
                        HttpContext.Current.Session["Error"] = "El comprobante no cuenta con Fecha de Timbrado";
                        return false;
                    }
                    if (Base.FechaTimbrado < HoyT)
                    {
                        HttpContext.Current.Session["Error"] = "La Fecha de Timbrado del Documento no se encuentra dentro del período actual.";
                        return false;
                    }
                    UUID = Base.UUID.ToString();
                    UUID1 = UUID;
                    FechaTimbre = Base.FechaTimbrado;
                    complementovalido = true;

                }
            }
        }
        else
        {
            HttpContext.Current.Session["Error"] = "El Xml no tiene complemento.";
            return false;
        }

        //Obtiene Adenda
        if (Factura.Addenda == null) { HttpContext.Current.Session["Error"] = "El XML no contiene Adenda"; return false; }
        else
        {
            int longitud;
            int cnt = 0;
            longitud = Factura.Addenda.Any.Length;
            uCFDsLib.v40.ComprobanteAddenda cmp;
            cmp = Factura.Addenda;
            for (int i = 0; i < longitud; i++)
            {
                XmlElement xe;
                String nombre;
                xe = cmp.Any[i];
                nombre = xe.Name.ToString();
                var adenda = cmp.Any[i].OuterXml;
                XmlDocument doc = new XmlDocument(); doc.LoadXml(adenda); XmlNode myNode = doc.DocumentElement;
                if (doc.LastChild.Attributes["OdeC"] == null)
                {
                    cnt = 0;
                }
                else
                {
                    OdeC = doc.LastChild.Attributes["OdeC"].InnerText;
                    if (OdeC.Length == 0)
                    {
                        HttpContext.Current.Session["Error"] = "El número de compra no es correcto.";
                        return false;
                    }
                    else
                    {
                        cnt = 1;
                        break;
                    }
                }
            }

            if (cnt == 0)
            {
                HttpContext.Current.Session["Error"] = "La adenda no contiene el nodo Odec.";
                return false;
            }

        }

        // Obtiene Datos Basicos para Verificar el Timbrado
        if (Factura.Receptor.Rfc == null || Factura.Receptor.Rfc.Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene RFC de Receptor."; return false; } else { RFCReceptor = Factura.Receptor.Rfc.ToString(); }
        if (Factura.Emisor.Rfc == null || Factura.Emisor.Rfc.Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene RFC de Emisor."; return false; } else { RFCEmisor = Factura.Emisor.Rfc.ToString(); }
        if (Factura.Total == 0 || Factura.Total.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Total"; return false; } else { Total = Factura.Total; }

        //Verifica timbrado
        string titulo, Msj, tipo1;
        string SAT = NuevoSAT(Factura.Emisor.Rfc, Factura.Receptor.Rfc, Factura.Total, UUID1);
        SAT = "Vigente";
        // Verifica Timbrado
        if (SAT != "Vigente")
        {
            string Resultado;
            if (SAT == "Cancelado") { Resultado = "Comprobante registrado ante el SAT como CANCELADO , verifica tu archivo"; }
            else { Resultado = "Comprobante no registrado ante el SAT, verifica tu archivo"; }
            Label4.Text = Resultado;
            titulo = "Servicio SAT";
            Msj = Resultado;
            tipo1 = "error";
            ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo1 + "');", true);
            return false;
        }
        else
        {
            Timbrado = 1;
        }

        if (Factura.Folio == null || Factura.Folio.Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Folio."; return false; } else { folio = Factura.Folio.ToString(); }
        if (Factura.Serie == null) { serie = ""; } else { serie = Factura.Serie; }
        if (serie != null)
        {
            if (serie.Length >= 2)
            {
                NFolio = NFolio + serie.Substring(0, 2);
            }
            else
            {
                NFolio = NFolio + serie.ToString();
            }
        }

        if (Factura.Folio.Length > 10)
        {
            if (serie == "")
            {
                if (Factura.Folio.Length > 10)
                {
                    NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 10, 10).ToString();
                }
                else
                {
                    NFolio = NFolio + Factura.Folio.ToString();
                }
                //NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 10, 10).ToString();
            }
            else
            {
                if (Factura.Folio.Length > 8)
                {
                    NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 8, 8).ToString();
                }
                else
                {
                    NFolio = NFolio + Factura.Folio.ToString();
                }
            }

        }
        else
        {
            if (serie == "")
            {
                if (Factura.Folio.Length > 10)
                {
                    NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 10, 10).ToString();
                }
                else
                {
                    NFolio = NFolio + Factura.Folio.ToString();
                }
                //NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 10, 10).ToString();
            }
            else
            {
                if (Factura.Folio.Length > 8)
                {
                    NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 8, 8).ToString();
                }
                else
                {
                    NFolio = NFolio + Factura.Folio.ToString();
                }
                //NFolio = NFolio + Factura.Folio.Substring(Factura.Folio.Length - 8, 8).ToString();
            }
            //NFolio = NFolio + Factura.Folio.ToString();
        }

        folio = NFolio;

        //Consulta UUID No Repetido en Base de Datos
        if (ConsultaFacturaDB(UUID))
        {
            ///consulta si esta cancelada entonces borrar tablas de facturacion
            if (ConsultaFacturaStatusDB(UUID) == 3)
            {
                int cvkey;
                cvkey = ConsultaFacturaKeyDB(UUID);
                if (!DelInvFilesCancel(cvkey))
                {
                    HttpContext.Current.Session["Error"] = "Ocurrio un errror al reemplazar la factura cancelada " + UUID + " , consultar con soporte.";
                    return false;
                }

            }
            else
            {
                HttpContext.Current.Session["Error"] = "El UUID " + UUID + " ya se encuentra registrado";
                return false;
            }
        }

        //Consulta Folio No Repetido en Base de Datos
        if (ConsultaFacturaFol(folio))
        {
            ///consulta si esta cancelada entonces borrar tablas de facturacion
            if (ConsultaFacturaStatusDB(UUID) == 3)
            {
                int cvkey;
                cvkey = ConsultaFacturaKeyDB(UUID);
                if (!DelInvFilesCancel(cvkey))
                {
                    HttpContext.Current.Session["Error"] = "Ocurrio un errror al reemplazar la factura cancelada " + UUID + " , consultar con soporte.";
                    return false;
                }

            }
            else
            {
                HttpContext.Current.Session["Error"] = "Ya has ingresado la Factura con el Folio " + folio + " ";
                return false;
            }
        }

        //Informacion del contribuyente
        if (Factura.Emisor.Nombre == null || Factura.Emisor.Nombre.Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene nombre de Emisor."; return false; } else { NombreEmisor = Factura.Emisor.Nombre.ToString(); }
        if (Factura.Emisor.RegimenFiscal.ToString() == "Item000" || Factura.Emisor.RegimenFiscal.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Regimen Fiscal de Emisor."; return false; }

        //Informacion del Receptor
        if (Factura.Receptor.Nombre == null || Factura.Receptor.Nombre.Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene nombre de Receptor."; return false; } else { NombreReceptor = Factura.Receptor.Nombre.ToString(); }
        if (Factura.Receptor.UsoCFDI.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Uso de CFDI de Receptor."; return false; } else { UsoCFDI = Factura.Receptor.UsoCFDI.ToString(); }

        //Datos Generales (Impuestos, Metodo,Forma de Pago, Descuentos)
        if (Factura.FormaPagoSpecified == false) { HttpContext.Current.Session["Error"] = "El XML no contiene Forma de Pago"; return false; } else { FormaPago = Factura.FormaPago.ToString(); }
        if (Factura.MetodoPagoSpecified == false) { HttpContext.Current.Session["Error"] = "El XML no contiene Metodo de Pago"; return false; } else { MetodoPago = Factura.MetodoPago.ToString(); }
        if (Factura.Impuestos == null) { ImpuestoImporteTraslado = 0; ImpuestoImporteRetenido = 0; }
        else
        {
            if (Factura.Impuestos.TotalImpuestosTrasladadosSpecified && Factura.Impuestos.TotalImpuestosTrasladados > 0)
            {
                ImpuestoImporteTraslado = Factura.Impuestos.TotalImpuestosTrasladados;
            }
            else
            {
                ImpuestoImporteTraslado = 0;
            }

            if (Factura.Impuestos.TotalImpuestosRetenidosSpecified && Factura.Impuestos.TotalImpuestosRetenidos > 0)
            {
                ImpuestoImporteRetenido = Factura.Impuestos.TotalImpuestosRetenidos;
            }
            else
            {
                ImpuestoImporteRetenido = 0;
            }
        }
        if (Factura.Descuento.ToString().Length == 0) { Descuento = 0; } else { Descuento = Factura.Descuento; }

        //Datos del Comprobante
        DateTime Hoy = DateTime.Now.AddMonths(-1);
        if (Factura.Fecha < Hoy) { HttpContext.Current.Session["Error"] = "La Fecha del Documento no se encuentra dentro del período actual."; return false; } else { FechaTransaccion = Factura.Fecha; }
        if (Factura.SubTotal == 0 || Factura.SubTotal.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Sub Total"; return false; } else { Subtotal = Factura.SubTotal; }
        if (Factura.Moneda.ToString() == "TTT" || Factura.Moneda.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Moneda"; return false; } else { Moneda = Factura.Moneda.ToString(); }
        if (Factura.TipoDeComprobante.ToString() == "X" || Factura.TipoDeComprobante.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Tipo de Comprobante"; return false; }
        else
        {
            TipoComprobante = Factura.TipoDeComprobante.ToString();
            if (TipoComprobante == "E")
            {
                if (Factura.CfdiRelacionados == null) { HttpContext.Current.Session["Error"] = "El Comprobante no contiene Cfdi Relacionado,revise."; return false; }
                var cta = Factura.CfdiRelacionados.CfdiRelacionado.Length;

                for (int r = 0; r < cta; r++)
                {
                    UUIDRel = Factura.CfdiRelacionados.CfdiRelacionado[r].UUID.ToString();
                }
            }
        }
        if (TipoComprobante == "E")
        {
            if (!ConsultaFacturaDB(UUIDRel))
            {
                HttpContext.Current.Session["Error"] = "El Comprobante al que hace referencia la nota de crédito, no está registrado.";
                return false;
            }
        }

        //Insertar Encabezado en BD
        Tuple<int, int> resutl = InsertInvoiceDB(VendKey, RFCEmisor, RFCReceptor, NombreEmisor, NombreReceptor, folio, serie, OdeC, UUID, FechaTransaccion, FechaTimbre, Moneda, UsoCFDI, TipoComprobante, FormaPago, MetodoPago, ImpuestoImporteTraslado, ImpuestoImporteRetenido, Descuento, Subtotal, Total, Cancelado, Timbrado, Status, UserKey, CompanyID, LogKey);
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

            if (Factura.Conceptos == null) { HttpContext.Current.Session["Error"] = "El XML no contiene Conceptos"; return false; }
            if (Factura.Conceptos.Length == 0) { HttpContext.Current.Session["Error"] = "El XML no contiene Conceptos"; return false; }
            else
            {
                string[] ListaID = new string[Factura.Conceptos.Length];
                int[] Partida = new int[Factura.Conceptos.Length];

                for (int i = 0; i < Factura.Conceptos.Length; i++)
                {
                    if (Factura.Conceptos[i].ValorUnitario.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El Concepto " + i + " no contiene Valor Unitario"; return false; } else { UnitCost = Factura.Conceptos[i].ValorUnitario; }
                    if (Factura.Conceptos[i].Importe.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El Concepto " + i + " no contiene Importe"; return false; } else { Monto = Factura.Conceptos[i].Importe; }
                    if (Factura.Conceptos[i].Cantidad.ToString().Length == 0) { HttpContext.Current.Session["Error"] = "El Concepto " + i + " no contiene Cantidad"; return false; } else { Cantidad = Factura.Conceptos[i].Cantidad; }
                    if (Factura.Conceptos[i].Descripcion == null) { HttpContext.Current.Session["Error"] = "El Concepto " + i + " no contiene Descripción"; return false; }
                    else
                    {
                        string strDescrr = Factura.Conceptos[i].Descripcion.ToString();
                        string[] Arreglo = strDescrr.Split(' '); //Genera Arreglo de la Descripcion y obtiene los Datos 
                        string Art = Arreglo[0].ToString(); // ID De Articulo
                        string Cuentas;
                        //int Conteo = 0;

                        //Verifica Impuestos de los Conceptos
                        if (Factura.Conceptos[i].Impuestos != null)
                        {
                            uCFDsLib.v40.ComprobanteConceptoImpuestos Imp = Factura.Conceptos[i].Impuestos;
                            if (Imp.Traslados != null)
                            {
                                longImpTsl = Imp.Traslados.Length;
                                if (longImpTsl > 0)
                                {
                                    for (int j = 0; j < longImpTsl; j++)
                                    {

                                        importimp = Imp.Traslados[j].Importe;
                                        tasaimp = Imp.Traslados[j].TasaOCuota;
                                        baseimp = Imp.Traslados[j].Base;
                                        tipoImp = Imp.Traslados[j].Impuesto.ToString();

                                        if (!Enum.IsDefined(typeof(c_Impuesto), tipoImp.ToString()))
                                        {
                                            DelInvFiles(vkey, InvcFileKey);
                                            HttpContext.Current.Session["Error"] = "El impuesto del artículo" + Art + " no es válido:" + tipoImp;
                                            return false;
                                        }

                                        if (tasaimp > 0)
                                        {
                                            if (importimp == 0)
                                            {
                                                DelInvFiles(vkey, InvcFileKey);
                                                HttpContext.Current.Session["Error"] = "El impuesto del artículo" + Art + " no es válido:" + tipoImp;
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }

                            if (Imp.Retenciones != null)
                            {
                                longImpRtn = Imp.Retenciones.Length;
                                if (longImpTsl > 0)
                                {
                                    for (int j = 0; j < longImpRtn; j++)
                                    {
                                        importimp = Imp.Retenciones[j].Importe;
                                        tasaimp = Imp.Retenciones[j].TasaOCuota;
                                        tipoImp = Imp.Retenciones[j].Impuesto.ToString();

                                        if (!Enum.IsDefined(typeof(c_Impuesto), tipoImp.ToString()))
                                        {
                                            DelInvFiles(vkey, InvcFileKey);
                                            HttpContext.Current.Session["Error"] = "El impuesto del artículo" + strItemIdSAT + " no es válido:" + tipoImp;
                                            return false;
                                        }

                                        if (tasaimp > 0)
                                        {
                                            if (importimp == 0)
                                            {
                                                DelInvFiles(vkey, InvcFileKey);
                                                HttpContext.Current.Session["Error"] = "El impuesto del artículo" + strItemIdSAT + " no es válido:" + tipoImp;
                                                return false;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        if (Factura.Conceptos[i].DescuentoSpecified)
                        {
                            DescuentoItm = Factura.Conceptos[i].Descuento;
                        }

                        SqlConnection sqlConnection1 = new SqlConnection();
                        sqlConnection1 = SqlConnectionDB("ConnectionString");
                        sqlConnection1.Open();

                        try
                        {
                            string sql1 = @"select Count(*) From timItem where ItemID = '" + Art + "' and CompanyID = '" + pCompanyID + "'";
                            using (var sqlQuery = new SqlCommand(sql1, sqlConnection1))
                            {
                                sqlQuery.CommandType = CommandType.Text;
                                sqlQuery.CommandText = sql1;
                                Cuentas = sqlQuery.ExecuteScalar().ToString();
                            }

                            if (Convert.ToInt16(Cuentas) == 0)
                            {
                                DelInvFiles(vkey, InvcFileKey);
                                HttpContext.Current.Session["Error"] = "No se encontro articulo con ID " + Art + " ,Verificalo con la Orden de compra.";
                                Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                return false;
                            }

                        }
                        catch (Exception ex)
                        {
                            LogError(pLogKey, pUserKey, "Carga-Factura:ConsultaTipoArt", ex.Message, pCompanyID);
                            string err;
                            err = ex.Message;
                            DelInvFiles(vkey, InvcFileKey);
                            HttpContext.Current.Session["Error"] = "No se encontro articulo con ID " + Art + ", el comprobante no puede ser procesado, Verificalo con la Orden de compra.";
                            Label4.Text = HttpContext.Current.Session["Error"].ToString();
                            return false;
                        }

                        try
                        {
                            string Ssql = "Select Status From TimItem Where ItemID = '" + Art + "' AND CompanyID = '" + pCompanyID + "'";
                            string Estado = string.Empty;

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
                                DelInvFiles(vkey, InvcFileKey);
                                HttpContext.Current.Session["Error"] = "El articulo  " + Art + "  se encuentra con estatus Inactivo, Comunícate con el área de Compras.";
                                Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                return false;
                            }

                            if (Estado == "4")
                            {
                                DelInvFiles(vkey, InvcFileKey);
                                HttpContext.Current.Session["Error"] = "El articulo  " + Art + "  se encuentra con estatus Borrado, Comunícate con el área de Compras.";
                                Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(pLogKey, pUserKey, "Carga-Factura:ConsultaTipoArt", ex.Message, pCompanyID);
                            string err;
                            err = ex.Message;
                            DelInvFiles(vkey, InvcFileKey);
                            HttpContext.Current.Session["Error"] = "El articulo con ID " + Art + " Se encuentra con estatus Invalido, el comprobante no puede ser procesado, Verificalo con la Orden de compra.";
                            Label4.Text = HttpContext.Current.Session["Error"].ToString();
                            return false;

                        }


                        string Clas = ConsultaTipoArt(Arreglo[0].ToString());
                        if (Clas == "")
                        {
                            LogError(pLogKey, pUserKey, "Carga-Factura:CargaXML", "No se encontro clase de Producto para la Partida " + Art, pCompanyID);
                            DelInvFiles(vkey, InvcFileKey);
                            HttpContext.Current.Session["Error"] = "El artículo " + Art + " no cuenta con una clase de Producto valida en SAGE,el comprobante no puede ser procesado, contacte al Área de compras.";
                            return false;
                        }
                        else
                        {
                            if (Clas == "S" || Clas == "s")
                            {
                                try
                                {
                                    int Part = Convert.ToInt32(Arreglo[1].ToString()); //Obtiene Numero de Partida del Servicio
                                    ContServ = ContServ + 1; //Contador de Servicios

                                    if (Part == 0)
                                    {
                                        DelInvFiles(vkey, InvcFileKey);
                                        HttpContext.Current.Session["Error"] = "El Comprobante contiene el Numeros de Partida 0, esto no esta permitido , el comprobante no puede ser procesado, Verificalo con la Orden de Compra";
                                        Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                        return false;
                                    }

                                    for (int a = 0; a <= i; a++)
                                    {
                                        if (Partida[a] == Part)
                                        {
                                            DelInvFiles(vkey, InvcFileKey);
                                            HttpContext.Current.Session["Error"] = "El Comprobante contiene el Numeros de Partida Duplicados , el comprobante no puede ser procesado, Verificalo con la Orden de Compra";
                                            Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                            return false;
                                        }
                                    }

                                    Partida[i] = Convert.ToInt32(Arreglo[1].ToString()); //Obtiene Numero de Partida del Servicio
                                }
                                catch (Exception ex)
                                {
                                    string err = ex.Message;
                                    DelInvFiles(vkey, InvcFileKey);
                                    LogError(pLogKey, pUserKey, "Carga-Factura:CargaXML", "Error al Obtener Partida en Item " + Art + "Factura " + folio, pCompanyID);
                                    HttpContext.Current.Session["Error"] = "Se Genero un Error al intententar obtener el numero de partida en el Articulo " + Art + " ,verificalo con la Orden de Compra" + "<br/>";
                                    return false;
                                }
                            }
                            else
                            {
                                ContPro = ContPro + 1; //Contador de Productivos

                                for (int a = 0; a <= i; a++)
                                {
                                    if (ListaID[a] == Art)
                                    {
                                        DelInvFiles(vkey, InvcFileKey);
                                        HttpContext.Current.Session["Error"] = "El Comprobante contiene partes de productos Duplicados, el comprobante no puede ser procesado, Verificalo con la Orden de Compra";
                                        Label4.Text = HttpContext.Current.Session["Error"].ToString();
                                        return false;
                                    }
                                }

                                ListaID[i] = Art; //ID de Articulo
                                Partida[i] = 0; // Asigna Valor 0 Por ser Productivo y se sabe que es Productivo
                            }

                        }

                        // Determina si Todas las partidas son Productivo o son Servicios
                        if (ContPro >= 1 && ContServ >= 1)
                        {
                            DelInvFiles(vkey, InvcFileKey);
                            HttpContext.Current.Session["Error"] = "El Comprobante contiene partes de productos y de servicios, no puede ser procesado, contacte al área de compras.";
                            return false;
                        }

                        //Inserta Detalle en BS
                        if (Factura.Conceptos[i].ClaveProdServ == null) { strItemIdSAT = "01010101"; } else { strItemIdSAT = Factura.Conceptos[i].ClaveProdServ.ToString().Substring(Factura.Conceptos[i].ClaveProdServ.ToString().Length - 8, 8); }

                        Tuple<int, int> resutlDtl = InsertInvoiceDtlDB(vkey, Art, strItemIdSAT, strUnidad, strDescr, UnitCost, Cantidad, Monto, DescuentoItm, Status, UserKey, CompanyID, LogKey, Partida[i]);
                        vkeydtl = resutlDtl.Item1;
                        resultado = resutlDtl.Item2;

                        //VERIFICA la Insercion sea Correcta
                        if (vkeydtl > 0 & resultado == 1)
                        {
                            if (Factura.Conceptos[i].Impuestos != null)
                            {
                                uCFDsLib.v40.ComprobanteConceptoImpuestos Imp = Factura.Conceptos[i].Impuestos;
                                if (Imp.Traslados != null)
                                {
                                    longImpTsl = Imp.Traslados.Length;

                                    if (longImpTsl > 0)
                                    {
                                        for (int j = 0; j < longImpTsl; j++)
                                        {
                                            importimp = Imp.Traslados[j].Importe;
                                            tasaimp = Imp.Traslados[j].TasaOCuota;
                                            tipoImp = Imp.Traslados[j].Impuesto.ToString().Substring(Imp.Traslados[j].Impuesto.ToString().Length - 3, 3);

                                            Tuple<int, int> resutlDtlTaxT = InsertInvoiceDtlTaxDB(vkey, vkeydtl, importimp, tipoImp, tasaimp, "T", Status, UserKey, CompanyID, LogKey);
                                            int vkeydtltx = resutlDtlTaxT.Item1;
                                            resultado = resutlDtlTaxT.Item2;

                                            if (!(vkeydtltx > 0 & resultado == 1))
                                            {
                                                DelInvFiles(vkey, InvcFileKey);
                                                HttpContext.Current.Session["Error"] = "Error en impuesto del artículo" + strItemIdSAT;
                                                return false;
                                            }

                                        }
                                    }
                                }
                                if (Imp.Retenciones != null)
                                {

                                    longImpRtn = Imp.Retenciones.Length;

                                    if (longImpTsl > 0)
                                    {
                                        for (int j = 0; j < longImpRtn; j++)
                                        {
                                            importimp = Imp.Retenciones[j].Importe;
                                            tasaimp = Imp.Retenciones[j].TasaOCuota;
                                            tipoImp = Imp.Retenciones[j].Impuesto.ToString().Substring(Imp.Retenciones[j].Impuesto.ToString().Length - 3, 3);

                                            Tuple<int, int> resutlDtlTaxR = InsertInvoiceDtlTaxDB(vkey, vkeydtl, importimp, tipoImp, tasaimp, "R", Status, UserKey, CompanyID, LogKey);
                                            int vkeydtltx = resutlDtlTaxR.Item1;
                                            resultado = resutlDtlTaxR.Item2;

                                            if (!(vkeydtltx > 0 & resultado == 1))
                                            {
                                                DelInvFiles(vkey, InvcFileKey);
                                                HttpContext.Current.Session["Error"] = "Error en impuesto del artículo" + strItemIdSAT;
                                                return false;
                                            }

                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            DelInvFiles(vkey, InvcFileKey);
                            HttpContext.Current.Session["Error"] = "Error al Insertar artículo de Factura";
                            return false;
                        }
                    }

                } // Termina For
            }

            //Valida la inserscion de Datos Segun el Tipo de Comprobante
            if (TipoComprobante == "I")
            {
                SincNcInv(vkey, 0, "IN");
                validacion = ValidatetInvoiceDB(VendKey, vkey, CompanyID);
            }

            else if (TipoComprobante == "E")
            {
                int invk = ConsultaFacturaKeyDB(UUIDRel);
                SincNcInv(vkey, invk, "CM");
                validacion = ValidateNCDB(VendKey, vkey, CompanyID);
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
                BindLogGridView(vkey, pVendKey, pCompanyID);
                DelInvFiles(vkey, InvcFileKey);
                return false;
            }
        }
        #endregion

        return valor;
    }

    private int ConsultaFacturaStatusDB(string UUID)
    {

        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();


            sql = @"SELECT Status FROM Invoice WHERE UUID ='" + UUID + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            return Convert.ToInt32(Cuenta);
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return 0;
        }

    }

    private bool DelInvFilesCancel(int InvoiceKey)
    {
        try
        {

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "Delete from InvoiceFile WHERE InvoiceKey = '" + InvoiceKey + "'";
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

    private int ConsultaFacturaKeyDB(string UUID)
    {

        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();


            sql = @"SELECT InvoiceKey FROM Invoice WHERE UUID ='" + UUID + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }


            sqlConnection1.Close();

            if (Convert.ToInt32(Cuenta) > 0)
                return Convert.ToInt32(Cuenta);
            else
                return Convert.ToInt32(Cuenta);
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return 0;
        }

    }

    private string ConsultaTipoArt(string Art)
    {
       string Clase = string.Empty;

        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("ConnectionString");
            sqlConnection1.Open();

            sql = @"select UserFld2 from timItem where ItemID = '" + Art + "' and CompanyID = '" + pCompanyID + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            if (Convert.ToString(Cuenta) != "")
            {
                if (Convert.ToString(Cuenta).Substring(0, 1) == "S" || Convert.ToString(Cuenta).Substring(0, 1) == "s" || Convert.ToString(Cuenta).Substring(0, 1) == "P" || Convert.ToString(Cuenta).Substring(0, 1) == "p")
                {
                    Clase = Cuenta.Substring(0, 1);
                }
                else
                {
                  Clase = "";
                }
                    
            }
            else
            {
                Clase = "";
            }
            return Clase;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:ConsultaTipoArt", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = "Se generó un Erro al Intentar Obtener la Clase de Articulo, Comunicate con el Área de Sistemas para ofrecerte una Solución ";
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return Clase;
        }

    }

    private string ConsultaTipoArticulo(string Art)
    {
        string Clase = string.Empty;

        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("ConnectionString");
            sqlConnection1.Open();

            sql = @"select UserFld2 from timItem where ItemID = '" + Art + "' and CompanyID = '" + pCompanyID + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            if (Convert.ToString(Cuenta) != "")
            {
                Clase = Cuenta;
            }
            else
            {
                Clase = "";
            }
            return Clase;
        }
        catch (Exception ex)
        {
            LogError(pLogKey, pUserKey, "Carga-Factura:ConsultaTipoArt", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = "Se generó un Erro al Intentar Obtener la Clase de Articulo, Comunicate con el Área de Sistemas para ofrecerte una Solución ";
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return Clase;
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

    private int ValidateNCDB(int VendorKey, int InvoiceKey, String CompanyID)
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

            sSQL = "spapiValidateCN";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@InvoiceKey", InvoiceKey));
            parsT.Add(new SqlParameter("@VendKey", VendorKey));
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

    private int databaseFilePutDB(Stream fs, Stream fs2, Stream fs3, int VendorKey, int InvoiceKey, String CompanyID)
    {
        try
        {

            string val = "";
            int id = 0;

            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            byte[] bytes = FileUpload1.FileBytes;

            System.IO.BinaryReader br2 = new System.IO.BinaryReader(fs2);
            byte[] bytes2 = FileUpload2.FileBytes;

            System.IO.BinaryReader br3 = new System.IO.BinaryReader(fs3);
            byte[] bytes3 = FileUpload3.FileBytes;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            using (var sqlWrite = new SqlCommand("Insert Into InvoiceFile (InvoiceKey,VendorKey,FileBinary1,FileBinary2,FileBinary3,FileType1,FileType2,FileType3,UpdateDate,UpdateUserKey,CompanyId) VALUES (@InvoiceKey,@VendorKey,@FileBinary1,@FileBinary2,@FileBinary3,@FileType1,@FileType2,@FileType3,@UpdateDate,@UpdateUserKey,@CompanyId);SELECT SCOPE_IDENTITY();", sqlConnection1))
            {
                sqlWrite.Parameters.Add("@InvoiceKey", SqlDbType.Int).Value = InvoiceKey;
                sqlWrite.Parameters.Add("@VendorKey", SqlDbType.Int).Value = VendorKey;
                sqlWrite.Parameters.Add("@FileBinary1", SqlDbType.VarBinary, bytes.Length).Value = bytes;
                sqlWrite.Parameters.Add("@FileBinary2", SqlDbType.VarBinary, bytes2.Length).Value = bytes2;
                sqlWrite.Parameters.Add("@FileBinary3", SqlDbType.VarBinary, bytes3.Length).Value = bytes3;
                sqlWrite.Parameters.Add("@FileType1", SqlDbType.VarChar, 5).Value = "xml";
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
      

}