//PORTAL DE PROVEDORES T|SYS|
//24 SEPTIEMBRE, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA PARA CARGA DE COMPLEMENTO DE PAGOS DE PROVEEDORES

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using uCFDsLib.v33;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.Globalization;

public partial class Pagos_Empleados : System.Web.UI.Page
{
    int err = 0;
    string eventName = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        //string Variable = HttpContext.Current.Session["IDCompany"].ToString();
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        if (!IsPostBack)
        {
            //Session["Proveedor"] = "000001";
            if (HttpContext.Current.Session["IDCompany"] == null)
            {
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Empleado")
                {
                    //if (UsuarioP.Items.Count == 0)
                    //{
                    //    User_Empresas(UsuarioP);
                    //}
                    CargaProv();
                    Panel2.Visible = false;
                    GridView2.Visible = false;
                    CambiaProv();
                }
                else
                {
                    HttpContext.Current.Session.RemoveAll();
                    Context.GetOwinContext().Authentication.SignOut();
                    Response.Redirect("~/Account/Login.aspx");
                }
            }
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

            string costoz = GridView1.Rows[index].Cells[9].Text.ToString(); // total
            string costov = GridView1.Rows[index].Cells[10].Text.ToString(); // rest

            if (cant > 0)
            {
                decimal costo = Convert.ToDecimal(GridView1.Rows[index].Cells[9].Text);
                try
                {
                    GridView1.Rows[index].Cells[10].Text = Math.Round(Convert.ToDecimal(costo - cant), 4).ToString();
                }
                catch (Exception ex) 
                {
                    GridView1.Rows[index].Cells[10].Text = Math.Round(0.00, 4).ToString();
                }
            }
            Conteo();

        }
        catch (Exception ex)
        {

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
            string costoz = GridView1.Rows[index].Cells[9].Text.ToString(); // total
            string costov = GridView1.Rows[index].Cells[10].Text.ToString(); // rest


            if (cb1.Checked == true)
            {
                tex.Text = Math.Round(Convert.ToDecimal(costoz), 4).ToString();
                tex.Enabled = true;
            }
            else
            {
                tex.Text = Math.Round(Convert.ToDecimal(costoz), 4).ToString();
                GridView1.Rows[index].Cells[10].Text = costoz;
                tex.Enabled = false;
            }
            Conteo();
        }
        catch (Exception ex)
        {
            int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());

            LogError(pLogKey, pUserKey, "Carga-Factura:GridView2_RowCommand", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Mensajes.Text = HttpContext.Current.Session["Error"].ToString();
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B1);", true);
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

            //bool baja = Bloqueo();
            //if (baja)
            //{
            //    Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            //}
            //    Global.Docs();
            //if ((HttpContext.Current.Session["Docs"].ToString() == "1"))
            //{
            //        //Page.MasterPageFile = "MenuP.master";
            //        //Response.Redirect("~/Logged/Proveedores/Default.aspx",false);
            //        Global.RevDocs();
            //        if ((HttpContext.Current.Session["Docs"].ToString() == "1"))
            //        {
            //            //Page.MasterPageFile = "MenuP.master";
            //            Response.Redirect("~/Logged/Proveedores/Default.aspx", false);

            //        }

            //    }
            //Global.Docs();
            //int Dias = Convert.ToInt16(HttpContext.Current.Session["Docs"].ToString());
            //if (Dias == 0)
            //{
            //    Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            //}
            //else if (Dias < 0)
            //{
            //    Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            //}
            //else if (Dias == 30)
            //{
            //    Page.MasterPageFile = "MenuPreP.master";
            //}
            //else if (Dias == 25)
            //{
            //    Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            //}
            //else if (Dias == 26)
            //{
            //    Response.Redirect("~/Logged/Proveedores/Default.aspx", false);
            //}
            //else if (Dias == 27)
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

    protected void Unnamed1_Click(object sender, EventArgs e)
    {
        Verifica();
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
                    if (rdr["PagoIN"].ToString() == "1")
                    {
                        Desde = Convert.ToDateTime(rdr["Desde1"].ToString());
                        Hasta = Convert.ToDateTime(rdr["Hasta1"].ToString());
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
                string stado = doc.FirstChild.FirstChild.FirstChild.FirstChild.ChildNodes[2].InnerText;
                Respuesta = stado;
            }
        }
        catch
        {
            Respuesta = "Error de de Consulta SAT, error de conexión";
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

    protected void Verifica()
    {

        Panel2.Visible = false;
        string text= string .Empty;
        string sld = string.Empty;
        sld = Conteo3();

        if (!FileUpload2.HasFile && !FileUpload1.HasFile)
        {
            text = "No se Encontraron Archivos";
        }
        else if (!FileUpload2.HasFile)
        {
            text = "Ingresa un XML";
        }
        else if (!FileUpload1.HasFile)
        {
            text = "Ingresa un PDF";
        }
        else if (sld != "") 
        {
            text = sld;
        }
        else if (Conteo2() == 0)
        {
            text = "Selecciona al menos un documento a pagar";
        }
        if (text != "")
        {
            Mensajes.Text = text;
            string Caja = "B1";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(" + Caja + ");", true);
        }
        else
        {
            string tipo, Msj, titulo;
            string Msj1 = cargarArchivosNew();
            if (err >=1)
            {
                tipo = "error";
                Msj = Msj1;
                titulo = "Carga De Pagos";
                Label2.Text = Msj1;
                if (Msj1 !="")
                {
                   ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);
                }  
            }
            else
            {
                CargaProv();
                Panel2.Visible = false;
                GridView2.Visible = false;
                CambiaProv();
                tipo = "success";
                Msj = Msj1;
                titulo = "Carga de Pagos";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }

        }
    }

    protected void Variables()
    {
        try
        {
            if (UsuarioP.SelectedItem.ToString() != "")
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spGetvar", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@EmpresaV",
                        Value = UsuarioP.SelectedItem.ToString()
                    });

                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@User",
                        Value = HttpContext.Current.Session["UserKey"].ToString()
                    });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    string Prueba = HttpContext.Current.Session["UserKey"].ToString();
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    DataTable GV = new DataTable();
                    GV.Load(rdr);

                    if (GV.Rows.Count == 1)
                    {
                        DataRow Fila = GV.Rows[0];
                        HttpContext.Current.Session["VendKey"] = Fila["VendKey"].ToString();
                        HttpContext.Current.Session["IDComTran"] = Fila["Company"].ToString();
                    }
                    else
                    {
                        HttpContext.Current.Session["VendKey"] = "";
                        HttpContext.Current.Session["IDComTran"] = "";
                    }
                    conn.Close();
                }

            }
        }
        catch
        {

        }
    }

    protected void User_Empresas(DropDownList Caja)
    {
        try
        {
            Caja.Items.Clear();
            string Userkey = User.Identity.Name.ToString();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectCompanyLog", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = Userkey });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                string Errores = string.Empty;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Empresas"].ToString() != "")
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Empresas"].ToString());
                        Caja.Items.Insert(0, Linea);
                        Caja.Visible = true;
                        Variables();
                    }
                    else
                    {
                        throw new Exception(Errores);
                    }


                }
                if (Caja.Items.Count == 0)
                {
                    ListItem Linea = new ListItem();
                    Linea.Value = ("N/D");
                    Caja.Items.Insert(0, Linea);
                    Caja.Visible = true;
                    HttpContext.Current.Session["IDComTran"] = "";
                }
                conn.Close();
            }
        }
        catch (Exception b)
        {
            Caja.Items.Clear();
        }
    }

    //Rutina Manejar Errores
    private void LogError(int LogKey, int UpdateUserKey, String proceso, String mensaje, String CompanyID)
    {
        try
        {

            int val1;
            val1 = 0;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string pCompanyID = HttpContext.Current.Session["IDCompany"].ToString();
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

    private bool ConsultaFacturaDB(string UUID)
    {
        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            sql = @"SELECT COUNT(*) As 'Cuenta' FROM Payment WHERE UUID ='" + UUID + "'";

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
            //LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            //string err;
            //err = ex.Message;
            //HttpContext.Current.Session["Error"] = err;
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return false;
        }

    }

    private int ObtenKey(string UUID)
    {
        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            sql = @"SELECT PaymentKey As 'Cuenta' FROM Payment WHERE UUID ='" + UUID + "'";

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
            //LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            //string err;
            //err = ex.Message;
            //HttpContext.Current.Session["Error"] = err;
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return 0;
        }

    }

    private bool ActualizaStado(string UUID)
    {
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "Update Invoice Set Status = 8 Where UUID ='" + UUID + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }

    }

    private bool ActualizaStado2(string UUID)
    {
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string compant = HttpContext.Current.Session["IDCompany"].ToString();
            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "Update Invoice Set Status = 8 Where UUID ='" + UUID + "' AND CompanyID = '" + compant + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }

    }

    private int RevisaRechazo(string UUID)
    {
        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            sql = @"Select Status From Payment Where UUID ='" + UUID + "'";

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
            return 2;
        }

    }

    private bool ConsultaFacturaFolio(string Folio)
    {
        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            string Vendor = HttpContext.Current.Session["VendKey"].ToString();
            sqlConnection1.Open();

            sql = @"SELECT COUNT(*) As 'Cuenta' FROM Payment WHERE Folio ='" + Folio + "' And VendorKey = '" + Vendor + "'";

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
            //LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            //string err;
            //err = ex.Message;
            //HttpContext.Current.Session["Error"] = err;
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return false;
        }

    }

    protected string cargarArchivos()
    {
        string Resultado = string.Empty;
        try
        {
            string complemento = "";
            string CompleT = "";
            int iLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int iUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string iCompanyID = HttpContext.Current.Session["IDCompany"].ToString();
            err = 0;

            //Deserializa XML
            XmlDocument xmlDoc = new XmlDocument();
            Comprobante Factura = new Comprobante();
            StreamReader objStreamReader;
            XmlSerializer Xml;

            xmlDoc.Load(FileUpload2.PostedFile.InputStream);

            try
            {
                StringReader reader = new StringReader(xmlDoc.InnerXml);
                Xml = new XmlSerializer(Factura.GetType());
                Factura = (Comprobante)Xml.Deserialize(reader);
            }
            catch (Exception ex)
            {
                string Mensaje1 = "Error al deserializar el XML, tu archivo no contiene la estructura valida por el SAT";
                string Mensaje = "Tu archivo no contiene la estructura valida por el SAT.";
                Mensaje = Mensaje + ex.Message;
                err = err + 1;
                if (ex.InnerException != null)
                {
                    Mensaje = Mensaje + " || " + ex.InnerException;
                }
                LogError(iLogKey, iUserKey, "Carga de Complemento de Pagos_Cargarchivos() Linea 454", Mensaje1, iCompanyID);
                return Mensaje1;
            }


            //Busca Nodos de Pagos y Timbre
            if (Factura.Complemento[0].Any == null || Factura.Complemento[0].Any.Length == 0)
            { 
                Resultado = "Se generaron problemas al cargar el complemento, verifica que tu XML contenga el nodo Complemento.";
                err = err + 1;
                return Resultado;
            }

            if (Factura.Complemento[0].Any.Count() < 2) { throw new Exception("El complemento no cuenta con los nodos requeridos para su validación, verifica  que contenga los nodos de Pago y Timbre Fiscal Digital e intenta nuevamente."); }

            int err2 = 0;
            if (Factura.Complemento[0].Any[0].Name == "pago10:Pagos")
            {
                complemento = Factura.Complemento[0].Any[0].OuterXml;
            }
            else
            {
                if (Factura.Complemento[0].Any[0].Name == "tfd:TimbreFiscalDigital")
                {
                    CompleT = Factura.Complemento[0].Any[0].OuterXml;
                }
                else
                {
                    err2 = err2 + 1;
                }
            }

            if (Factura.Complemento[0].Any[1].Name == "tfd:TimbreFiscalDigital")
            {
                CompleT = Factura.Complemento[0].Any[1].OuterXml;
            }
            else
            {
                if (Factura.Complemento[0].Any[1].Name == "pago10:Pagos")
                {
                    complemento = Factura.Complemento[0].Any[1].OuterXml;
                }
                else
                {
                    err2 = err2 + 1;
                }
            }

            if (err2 >=1) { throw new Exception("Error al leer el comprobante, verifica que tu comprobante cuente con los nodos pagos10:Pagos  y tfd:TimbreFiscalDigital"); }

                Stream s = new MemoryStream(ASCIIEncoding.Default.GetBytes(complemento));
                Stream x = new MemoryStream(ASCIIEncoding.Default.GetBytes(CompleT));
                uCFDsLib.v33.TimbreFiscalDigital timbre = new uCFDsLib.v33.TimbreFiscalDigital();
                uCFDsLib.v33.Pagos Pagos = new uCFDsLib.v33.Pagos();

                try
                {
                
                    objStreamReader = new StreamReader(s);
                    Xml = new XmlSerializer(Pagos.GetType());
                    Pagos = (uCFDsLib.v33.Pagos)Xml.Deserialize(objStreamReader);
                    objStreamReader.Close();
                }
                catch (Exception ex)
                {
                    string Mensaje1 = "Error al Deserializar el complemento de Pago, tu archivo no contiene la estructura valida por el SAT ";
                    string Mensaje = "El Nodo Pagos:10 no contiene la estructura valida por el SAT.";
                    err = err + 1;
                    Mensaje = Mensaje + ex.Message;
                    if (ex.InnerException != null)
                    {
                        Mensaje = Mensaje + " || " + ex.InnerException;
                    }
                    LogError(iLogKey, iUserKey, "Carga de Complemento de Pagos_Cargarchivos() Linea 492", Mensaje1, iCompanyID);
                    return Mensaje1;
                }

                try
                {
                
                    objStreamReader = new StreamReader(x);
                    Xml = new XmlSerializer(timbre.GetType());
                    timbre = (uCFDsLib.v33.TimbreFiscalDigital)Xml.Deserialize(objStreamReader);
                    objStreamReader.Close();
                }
                catch (Exception ex)
                {
                    string Mensaje1 = "Error al Deserializar el Timbre Fiscal Digital, tu archivo no contiene la estructura valida por el SAT ";
                    string Mensaje = "El nodo:TimbreFiscalDigital no contiene la estructura valida por el SAT.";
                    Mensaje = Mensaje + ex.Message;
                    err = err + 1;
                    if (ex.InnerException != null)
                    {
                        Mensaje = Mensaje + " || " + ex.InnerException;
                    }
                    LogError(iLogKey, iUserKey, "Carga de Complemento de Pagos_Cargarchivos() Linea 514", Mensaje1, iCompanyID);
                    return Mensaje1;
                }

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.InnerXml = complemento;

                if (timbre.UUID == null ) { Resultado = "Tu complemento no cuenta con el nodo UUID"; }

                //Verifica que no se haya cargado anteriormente el Archivo
                if (ConsultaFacturaDB(timbre.UUID))
                {
                    int Rechazo = RevisaRechazo(timbre.UUID);
                    if (Rechazo == 3)
                    {
                        DellPagoCancel(timbre.UUID);
                    }
                    else
                    {
                        Resultado = "El UUID " + timbre.UUID + " ya se encuentra registrado, el complemento no puede ser procesado";
                        err = err + 1;
                        return Resultado;
                    }
                }

                 if (Factura.Folio == null) { Resultado = "Tu complemento no cuenta con un el nodo Folio"; }

                //Verifica que no se haya cargado anteriormente el Archivo
                if (ConsultaFacturaFolio(Factura.Folio))
                {
                    Resultado = "Ya has ingresado un complemento de Pago con Folio (Folio), el complemento no puede ser procesado.";
                    err = err + 1;
                    return Resultado;
                }


                //Valida contra el SAT
                string SAT = NuevoSAT(Factura.Emisor.Rfc, Factura.Receptor.Rfc, Factura.Total, timbre.UUID);
                SAT = "Vigente";
                if (SAT != "Vigente")
                {
                    string titulo, Msj, tipo1;
                    if (SAT == "Cancelado") { Resultado = "Comprobante registrado ante el SAT como CANCELADO, verifícalo."; }
                    else { Resultado = "Comprobante no registrado ante el SAT, verifica tu archivo"; }
                    titulo = "Servicio SAT";
                    Msj = Resultado;
                    tipo1 = "error";
                    err = err + 1;
                    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo1 + "');", true);
                    Resultado = "";
                    return Resultado;
                }

                string Feca = Pagos.Pago.FechaPago.ToString();
                string fechaD = Factura.Fecha.ToString();

                DateTime fechaF = Convert.ToDateTime(Feca).Date;  // Mes de Pago
                DateTime fechaDoc = Convert.ToDateTime(fechaD).Date; // Timbrado

                int dia = Convert.ToInt32(Convert.ToString(fechaDoc.Day)); // Dia de Timbrado
                int mes = Convert.ToInt32(Convert.ToString(fechaF.Month)); //Mes de Pago
                int mesT = Convert.ToInt32(Convert.ToString(fechaDoc.Month)); //Mes de Timbrado
                string Result = string.Empty;

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    string Cades = "VerFecPagos";                    
                    SqlCommand cmd = new SqlCommand(Cades, conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@FechaPago", Value = fechaF });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@FechaFac", Value = fechaDoc });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                      Result = rdr["Resultado"].ToString();
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            if (Result != "")
            {
                Resultado = Result; err = err + 1; return Resultado;
            }
            //Valida Pago en SAGE
            string ReSAGE = RevisaSAGE(Factura, Pagos);
            //ReSAGE = "";
            if (ReSAGE != "") { err = err + 1; return ReSAGE; }

                //Comprueba Nodos para Presentar
                string UUID, Fol, Serie, F1, F2, Ver, Totl;

                if (Factura.Folio == null) { Fol = ""; } else { Fol = Factura.Folio.ToString(); }
                if (Factura.Serie == null) { Serie = ""; } else { Serie = Factura.Serie.ToString(); }
                if (timbre.UUID == null) { UUID = ""; } else { UUID = timbre.UUID.ToString(); }
                if (Factura.Fecha == null) { F1 = ""; } else { F1 = Factura.Fecha.ToShortDateString(); }
                if (Pagos.Pago.FechaPago == null) { F2 = ""; } else { F2 = Pagos.Pago.FechaPago.ToShortDateString(); }
                if (Factura.Version == null) { Ver = ""; } else { Ver = Factura.Version.ToString(); }
                if (Pagos.Pago.Monto.ToString() == null) { Totl = ""; } else { Totl = Pagos.Pago.Monto.ToString(); }

                //Sube Archivos al Portal
                string Respues = Execute(FileUpload2, FileUpload1, UUID, Fol, Serie, F1, F2, Ver, Totl).ToString();
                if (Respues != "1")
                {   ResetPago(UUID);
                    Resultado = "Se encontrarón problemas al cargar los datos generales del comprobante, Verifica que estén correctos y/o que no se haya cargado anteriormente este documento ";
                    err = err + 1;
                    return Resultado;
                }

                //Realiza Vaciado en Pantalla
                DoctoRelacionado Docu1 = new DoctoRelacionado();
                int Total = Pagos.Pago.DoctoRelacionado.Count();
                int Conteo = 0;
                for (int j = 0; j < Pagos.Pago.DoctoRelacionado.Count(); j++)
                {
                    Docu1 = Pagos.Pago.DoctoRelacionado[j];
                    string UUIP = Docu1.IdDocumento;
                    string FolioP = Docu1.Folio;
                    string Mon = Docu1.MonedaDR.ToString();
                    string Meto = Docu1.MetodoDePagoDR;
                    string PArc = Docu1.NumParcialidad;
                    string SalT = Docu1.ImpSaldoAnt.ToString();
                    string PAgo = Docu1.ImpPagado.ToString();
                    string Saldo = Docu1.ImpSaldoInsoluto.ToString();
                    if (Desglose(UUIP, UUID, PAgo, PArc, SalT, Saldo, FolioP, Mon, Meto) == false)
                    {
                        ResetPago(UUID);
                        Resultado = "Se encontrarón problemas al cargar el desglose del comprobante, Verifica que los datos estén correctos";
                        err = err + 1;
                        return Resultado;
                    }
                    else
                    {
                        Conteo =+ 1;
                    }
                }

                if (Conteo == Total)
                {
                    for (int j = 0; j < Pagos.Pago.DoctoRelacionado.Count(); j++)
                    {
                      string UUIP = Docu1.IdDocumento;
                      ActualizaStado(UUIP);  
                    }
                }

                if (Pagos.Pago.FechaPago == null) {txt2.Text = "N/D";} else {txt2.Text = Pagos.Pago.FechaPago.ToShortDateString();}
                if (Pagos.Pago.MonedaP.ToString() == "") {txt3.Text = "N/D";} else {txt3.Text = Pagos.Pago.MonedaP.ToString();}
                if (Pagos.Pago.FormaDePagoP.ToString() == null) {txt5.Text = "N/D";} else {txt5.Text = Pagos.Pago.FormaDePagoP.ToString();}
                if (Pagos.Pago.Monto.ToString("C") == null) {txt1.Text = "N/D";} else {txt1.Text = Pagos.Pago.Monto.ToString("C");}
                if (Factura.Fecha == null) {txt6.Text = "N/D";} else {txt6.Text = Factura.Fecha.ToShortDateString();}
                if (Factura.Serie == null) {txt8.Text = "";} else {txt8.Text = Factura.Serie.ToString();}
                if (timbre.UUID == null) { txt7.Text = "N/D";} else { txt7.Text = timbre.UUID.ToString();}
                if (Factura.Folio == null) { txt9.Text = "N/D";} else { txt9.Text = Factura.Folio.ToString();}
                if (Factura.Version == null) {txt10.Text = "";} else {txt10.Text = Factura.Version.ToString();}
                if (Pagos.Pago.TipoCambioP.ToString() != "0") {txt4.Text = Pagos.Pago.TipoCambioP.ToString();} else {txt4.Text = "1";}

                if (Pagos.Pago.NumOperacion == null) { TextBox1.Text = "N/D"; } else { TextBox1.Text = Pagos.Pago.NumOperacion.ToString(); }
                if (Pagos.Pago.RfcEmisorCtaOrd == null) { TextBox2.Text = "N/D"; } else { TextBox2.Text = Pagos.Pago.RfcEmisorCtaOrd.ToString(); }
                if (Pagos.Pago.NomBancoOrdExt == null) { TextBox3.Text = "N/D"; } else { TextBox3.Text = Pagos.Pago.NomBancoOrdExt.ToString(); }
                if (Pagos.Pago.CtaOrdenante == null) { TextBox4.Text = "N/D"; } else { TextBox4.Text = Pagos.Pago.CtaOrdenante.ToString(); }
                if (Pagos.Pago.RfcEmisorCtaBen == null) { TextBox5.Text = "N/D"; } else { TextBox5.Text = Pagos.Pago.RfcEmisorCtaBen.ToString(); }


                //Vaciado de Documentos Relacionados

                DataTable dt = new DataTable();
                dt.Columns.Add("UUID");
                dt.Columns.Add("Folio");
                dt.Columns.Add("Moneda");
                dt.Columns.Add("Metodo Pago");
                dt.Columns.Add("Parcialidad");
                dt.Columns.Add("SaldoAnt");
                dt.Columns.Add("Pago");
                dt.Columns.Add("NvoSaldo");

                DoctoRelacionado Docu = new DoctoRelacionado();

                for (int j = 0; j < Pagos.Pago.DoctoRelacionado.Count(); j++)
                {
                    DataRow row = dt.NewRow();
                    Docu = Pagos.Pago.DoctoRelacionado[j];

                    row["UUID"] = Docu.IdDocumento;
                    row["Folio"] = Docu.Folio;
                    row["Moneda"] = Docu.MonedaDR;
                    row["Metodo Pago"] = Docu.MetodoDePagoDR;
                    row["Parcialidad"] = Docu.NumParcialidad;
                    row["SaldoAnt"] = Docu.ImpSaldoAnt.ToString("C");
                    row["Pago"] = Docu.ImpPagado.ToString("C");
                    row["NvoSaldo"] = Docu.ImpSaldoInsoluto.ToString("C");
                    dt.Rows.Add(row);
                }

                Panel2.Visible = true;
                GridView2.DataSource = dt;
                GridView2.DataBind();
                GridView2.Visible = true;

                err = 0;
                Resultado = "Carga de complemento de pago exitosa";

        }
        catch (Exception ex)
        {   
            err = err + 1;
            if (ex.Message == null)
            {
                Resultado = "Documento Invalido";
            }
            else
            {
                if (ex.Message != null) { Resultado = ex.Message; }
            }
            
        }

        return Resultado;
    }

    protected string cargarArchivosNew()
    {
        string Resultado = string.Empty;
        try
        {
            int iLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int iUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string iCompanyID = HttpContext.Current.Session["IDCompany"].ToString();

            //Valida Pago en SAGE
            //string ReSAGE = RevisaSAGE(Factura, Pagos);
            //ReSAGE = "";
            //if (ReSAGE != "") { err = err + 1; return ReSAGE; }

            foreach (GridViewRow gvr in GridView1.Rows)
            {
                TextBox cantidades = (TextBox)gvr.Cells[1].FindControl("cant");
                CheckBox check = (CheckBox)gvr.Cells[1].FindControl("Check");
                if (check.Checked == true)
                {

                    string Folio = gvr.Cells[5].Text.ToString();
                    string OCom = gvr.Cells[4].Text.ToString();
                    string fec1 = gvr.Cells[1].Text.ToString();
                    string fec2 = gvr.Cells[1].Text.ToString();
                    string Totsl = cantidades.Text.ToString();
                    Folio = Folio.TrimEnd(' ');
                    string UUID = Folio + "-" + OCom;



                    string UUIDF = GETUUID(Folio, OCom);

                    //Sube Archivos al Portal
                    string Respues = Execute(FileUpload2, FileUpload1, UUID, Folio, "", fec1, fec2, "4.0", Totsl).ToString();
                    if (Respues != "1")
                    {
                        ResetPago(UUID);
                        Resultado = "Se encontrarón problemas al cargar los datos generales del comprobante, Verifica que estén correctos y/o que no se haya cargado anteriormente este documento ";
                        err = err + 1;
                        return Resultado;
                    }

                    if (Desglose(UUID, UUIDF, Totsl, Totsl, Totsl, Totsl, Folio, "MXN", "PUE") == false)
                    {
                        ResetPago(UUID);
                        Resultado = "Se encontrarón problemas al cargar el desglose del comprobante, Verifica que los datos estén correctos";
                        err = err + 1;
                        return Resultado;
                    }
                    ActualizaStado2(UUIDF);
                }
            }
            Resultado = "Carga de complemento de pago exitosa";
        }
        catch (Exception ex)
        {
            err = err + 1;
            if (ex.Message == null)
            {
                Resultado = "Documento Invalido";
            }
            else
            {
                if (ex.Message != null) { Resultado = ex.Message; }
            }

        }

        return Resultado;
    }

    private bool Desglose(string UUIDP,string Fac,string pago, string Parcial,string SalT, string SAlN,string FolP, string Mon,string Meto)
    {   
        bool Res = false;
        int key = 0;
        int UUID = 0;
        string  Fk = string.Empty;
        string Pk = string.Empty;

        try
        {
            // foreach (GridViewRow gvr in GridView2.Rows)
            //{
            SalT = SalT.Replace(",", ".");
            SAlN = SAlN.Replace(",", ".");
            pago = pago.Replace(",", ".");
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                    string Caden = "spGetPayAppl";
                    SqlCommand cmd1 = new SqlCommand(Caden, conn);
                    cmd1.CommandType = CommandType.StoredProcedure;

                    cmd1.Parameters.Add(new SqlParameter()
                    { ParameterName = "@UPago", Value = Fac });

                    cmd1.Parameters.Add(new SqlParameter()
                    { ParameterName = "@Factura", Value = UUIDP });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    SqlDataReader rdr1 = cmd1.ExecuteReader();

                    while (rdr1.Read())
                    {
                        Pk = rdr1["PKey"].ToString();
                        Fk = rdr1["FKey"].ToString();
                    }

                    key = int.Parse(Pk.ToString());
                    UUID = int.Parse(Fk.ToString());


                    if (UUID != 0)
                    {
                    /////////////////////////////////////////////////////////////////////////////////

                    string Cades = "spInsertPayAppl";
                    string Result = string.Empty;
                    SqlCommand cmd = new SqlCommand(Cades, conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@PaymentKey", Value = key });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@ApplUUID", Value = UUIDP });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@ApplFolio", Value = FolP });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@PaymtApplied", Value = pago });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@PartialNumber", Value = 1 });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@BalanceAnt", Value = SalT });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@BalanceOut", Value = "0.00" });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@Moneda", Value = Mon });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@MetodoPago", Value = Meto });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@Folio", Value = UUID });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        if (rdr["Resultado"].ToString() == "1")
                        {
                           Res = true;
                        }
                        else
                        {
                            Res = false;
                        }
                    }
                 }
               }
            //}
        }
        catch(Exception ex)
        {
            string Error = ex.Message;
            Res = false;
        }
        return Res;
    }

    protected void Carga()
    {
        string tipo, Msj, titulo;
        string Respues = string.Empty;


        if (FileUpload1.FileName.ToString() != "" && FileUpload2.FileName.ToString() != "")
        {           
            if (Respues != "1")
            {   
               
                tipo = "succes";
                Msj = "Carga Exitosa";
                titulo = "Carga de Pagos";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
            else
            {
                tipo = "error";
                Msj = "Se encontraron Errores al Subir el Archivo, Validalos";
                titulo = "Carga De Pagos";
                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
        }
        else
        {
            tipo = "error";
            Msj = "Se encontraron Errores al Subir el Archivo, Verifica que se haya Cargado el Archivo";
            titulo = "Carga De Pagos";
            ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }




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
                string users = HttpContext.Current.Session["UserKey"].ToString();
                int Opc = 12;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = Opc });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = users });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Company });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SelProv.Items.Clear();
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

            if (SelProv.Items.Count == 0)
            {
                DatosV.Visible = true;
                Datos.Visible = false;
            }
            else
            {
                DatosV.Visible = false;
                Datos.Visible = true;
            }
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

    protected void CambiaProv()
    {
        try
        {
            string SQL = string.Empty;
            string prov = SelProv.SelectedItem.ToString();
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            string users = HttpContext.Current.Session["UserKey"].ToString();
            int Filas = 0;

            SQL = "";
            SQL = "spGetDocsEmpleados";
            //SQL = " Select CONVERT(VARCHAR(10), AprovDate, 103) As Fecha,RFCEmisor,NodeOc,Folio,Moneda,SubTotal,Total ";
            //SQL = SQL + " From invoice a inner join vendors c on a.VendorKey = c.VendorKey Where a.Status = 6 ";
            //SQL = SQL + " AND a.CompanyID = '" + Company + "' AND c.vendname ='" + SelProv.SelectedItem.ToString() + "' ";

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 2 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = users });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Prov", Value = prov });
                if (conn.State == ConnectionState.Open) {conn.Close();} conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable Registros = new DataTable();
                Registros.Load(rdr);
                Filas = Registros.Rows.Count;
                conn.Close();

                if (Filas == 0)
                {
                    DatosV.Visible = true;
                    Datos.Visible = false;
                }
                else
                {
                    llenatabla(Registros);
                    DatosV.Visible = false;
                    Datos.Visible = true;
                }

                if (GridView1.Rows.Count == 0)
                {
                    DatosV.Visible = true;
                    Datos.Visible = false;
                }
                else
                {
                    DatosV.Visible = false;
                    Datos.Visible = true;
                }
                Conteo();
            }
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

    protected void SelProv_SelectedIndexChanged1(object sender, EventArgs e)
    {
        try
        {
            CambiaProv();
        }
        catch (Exception ex) 
        {
            RutinaError(ex);
        }        
     }

    protected void llenatabla(DataTable dts)
    {
        try
        {
            DataTable dt = new DataTable();
            GridView1.DataSource = dt;
            GridView1.DataBind();
            GridView1.Visible = false;
            HttpContext.Current.Session["Error"] = "";
            Mensajes.Text = "";
            int i = 0;            
            dt.Columns.Add("Fecha");
            dt.Columns.Add("VendID");
            dt.Columns.Add("RFC");
            dt.Columns.Add("OC");
            dt.Columns.Add("Folio");
            dt.Columns.Add("Moneda");
            dt.Columns.Add("Saldo");
            dt.Columns.Add("Total");
            dt.Columns.Add("Resto");

            string[] cantidad = new string[dts.Rows.Count];
            foreach (DataRow row in dts.Rows)
            {
                DataRow dr = dt.NewRow();
                decimal Total = Convert.ToDecimal(HttpUtility.HtmlDecode(row["Total"].ToString()));
                decimal Pago = Convert.ToDecimal(HttpUtility.HtmlDecode(row["Saldo"].ToString()));
                decimal Resto = Convert.ToDecimal(Total - 0);
                cantidad[i] = HttpUtility.HtmlDecode(row["Saldo"].ToString());

                dr["Fecha"] = HttpUtility.HtmlDecode(row["Fecha"].ToString());
                dr["VendID"] = HttpUtility.HtmlDecode(row["VendID"].ToString());
                dr["RFC"] = HttpUtility.HtmlDecode(row["RFC"].ToString());
                dr["OC"] = HttpUtility.HtmlDecode(row["OC"].ToString());
                dr["Folio"] = HttpUtility.HtmlDecode(row["Folio"].ToString());
                dr["Moneda"] = HttpUtility.HtmlDecode(row["Moneda"].ToString());
                dr["Saldo"] = Math.Round(Convert.ToDecimal(Pago), 4).ToString();
                dr["Total"] = Math.Round(Convert.ToDecimal(Total), 4).ToString();
                dr["Resto"] = Math.Round(Convert.ToDecimal(Resto), 4).ToString();
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
            RutinaError(ex);
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"]);
            int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"]);
            string pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"]);
            LogError(pLogKey, pUserKey, "Carga-Factura:btnSage_Click", ex.Message, pCompanyID);
            HttpContext.Current.Session["Error"] = err;
            Mensajes.Text = HttpContext.Current.Session["Error"].ToString();
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B1);", true);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "hideshow();", true);
            return;
        }
    }

    protected void Conteo()
    {
        try
        {
            decimal total = 00;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                TextBox cantidades = (TextBox)gvr.Cells[1].FindControl("cant");
                CheckBox check = (CheckBox)gvr.Cells[1].FindControl("Check");

                if (check.Checked == true) 
                {
                    total += Convert.ToDecimal(cantidades.Text);
                }
            }

            if (total == 0)
            {
                Tot.InnerText = "$ 0.00";
            }
            else 
            {
                try
                {
                    double total2 = Math.Round(Convert.ToDouble(total),2);
                    decimal total3 = Math.Round(Convert.ToDecimal(total), 2);
                    Tot.InnerText = "$ " + total3.ToString();
                }
                catch (Exception ex) 
                {
                    Tot.InnerText = "$ 0.01";
                }
            }

        }
        catch (Exception ex)
        {
            string err;
            err = ex.Message;
            RutinaError(ex);
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"]);
            int pLogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]);
            string pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"]);
            LogError(pLogKey, pUserKey, "Carga-Factura:btnSage_Click", ex.Message, pCompanyID);
            HttpContext.Current.Session["Error"] = err;
            Mensajes.Text = HttpContext.Current.Session["Error"].ToString();
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B1);", true);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "hideshow();", true);
            return;
        }
    }

    protected int Conteo2()
    {
        int conteo = 0;
        try
        {
            
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                CheckBox check = (CheckBox)gvr.Cells[1].FindControl("Check"); if (check.Checked == true) { conteo += 1; }
            }
        }
        catch (Exception ex)
        {
            string err;
            err = ex.Message;
            RutinaError(ex);
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"]);
            int pLogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]);
            string pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"]);
            LogError(pLogKey, pUserKey, "Carga-Factura:btnSage_Click", ex.Message, pCompanyID);
            HttpContext.Current.Session["Error"] = err;
            Mensajes.Text = HttpContext.Current.Session["Error"].ToString();
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B1);", true);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "hideshow();", true);
        }
        return conteo;
    }

    protected string Conteo3()
    {
        string resultado = string.Empty;
        try
        {
            resultado = "";
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                TextBox cantidades = (TextBox)gvr.Cells[1].FindControl("cant");
                CheckBox check = (CheckBox)gvr.Cells[1].FindControl("Check");

                if (check.Checked == true)
                {
                    decimal totals = Convert.ToDecimal(cantidades.Text);
                    decimal saldos = Convert.ToDecimal(gvr.Cells[9].Text.ToString());
                    string folios = gvr.Cells[9].Text.ToString();

                    if (totals > saldos) 
                    {
                        resultado = "El salbo abonado a la Factura " + folios + " , No debe de ser superior al resldo restante, verificalo.";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            string err;
            err = ex.Message;
            RutinaError(ex);
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"]);
            int pLogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]);
            string pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"]);
            LogError(pLogKey, pUserKey, "Carga-Factura:btnSage_Click", ex.Message, pCompanyID);
            HttpContext.Current.Session["Error"] = err;
            Mensajes.Text = HttpContext.Current.Session["Error"].ToString();
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertB(B1);", true);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "hideshow();", true);
        }
        return resultado;
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

    protected void RutinaError(Exception ex)
    {
        string Msj = string.Empty;
        StackTrace st = new StackTrace(ex, true);
        StackFrame frame = st.GetFrame(st.FrameCount - 1);
        int LogKey, Userk;
        string Company = string.Empty;
        if (HttpContext.Current.Session["UserKey"] == null) { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
        if (HttpContext.Current.Session["IDCompany"] == null) { Msj = Msj + "," + "Variable IDCompany null"; Company = "TSM"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
        //if (HttpContext.Current.Session["VendKey"] == null) { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
        if (HttpContext.Current.Session["LogKey"] == null) { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
        Msj = Msj + ex.Message;
        string nombreMetodo = frame.GetMethod().Name.ToString();
        int linea = frame.GetFileLineNumber();
        Msj = Msj + " || Metodo : Administracion_Doc.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
        LogError(LogKey, Userk, " Administracion_Doc.aspx.cs_" + nombreMetodo, Msj, Company);
        //lblMsj.Text = ex.Message;
    }

    public string Execute(FileUpload Caja,FileUpload Caja2,string UUID,string Folio,string Serie,string F1,string F2,string Version,string Total)
     {
        string res;

        try
        {
            string Filename = Caja.FileName.ToString();

            //Fabian
            Byte[] bytes1 = FileUpload2.FileBytes;
            Byte[] bytes2 = FileUpload1.FileBytes;
            //}
            string Ext = ".XML";
            string Ext2 = ".pdf";
            string Var1 = HttpContext.Current.Session["IDComTran"].ToString();
            string Var2 = HttpContext.Current.Session["VendKey"].ToString();
            string Var3 = HttpContext.Current.Session["UserKey"].ToString();
            int VendKey = GetVkey();
            int UserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());

            Total = Total.Replace(",", ".");

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Cades = "spInsertPay";
                string Result = string.Empty;
                SqlCommand cmd = new SqlCommand(Cades, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@File", Value = bytes1 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@File2", Value = bytes2 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Vendedor", Value = VendKey });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = HttpContext.Current.Session["IDCompany"].ToString() });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserKey", Value = UserKey });

                //cmd.Parameters.Add(new SqlParameter()
                //{ ParameterName = "@F1", Value = F1 });

                //cmd.Parameters.Add(new SqlParameter()
                //{ ParameterName = "@F2", Value = F2 });
                DateTime Fe1 = DateTime.ParseExact(F1, "dd/MM/yyyy", null);
                DateTime Fe2 = DateTime.ParseExact(F2, "dd/MM/yyyy", null);

                cmd.Parameters.Add(new SqlParameter("@F1", SqlDbType.DateTime) { Value = Fe1.ToShortDateString() });
                cmd.Parameters.Add(new SqlParameter("@F2", SqlDbType.DateTime) { Value = Fe2.ToShortDateString() });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UUID", Value = UUID });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Serie", Value = Serie });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Folio", Value = Folio });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Version", Value = Version });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Total", Value = Total });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Ext", Value = Ext });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Ext2", Value = Ext2 });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    if (rdr["Resultado"].ToString() == "1")
                    {
                        Result = rdr["Resultado"].ToString();
                    }
                    else
                    {
                        Result = rdr["Resultado"].ToString();
                        throw new Exception(Result);
                    }
                }
                res = Result;
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return res;
        }
        catch (Exception Rt)
        {
            res = Rt.Message;
            return res;
        }
    }

    protected void ResetPago(string UU)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Caden = "spResetPay";
                SqlCommand cmd1 = new SqlCommand(Caden, conn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.Parameters.Add(new SqlParameter()
                { ParameterName = "@UUID", Value = UU });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
            }
        }
        catch(Exception ex)
        {

        }

    }

    private bool DellPagoCancel(string UUID)
    {
        try
        {

            int llave = ObtenKey(UUID);

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "Delete from PaymentFile Where PaymentKey = '" + llave + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "Delete from PaymentAppl Where PaymentKey =  '" + llave + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "Delete from Payment Where PaymentKey = '" + llave + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();

                sqlQuery.CommandText = "Update Notificaciones_Payment Set Payment = null Where Payment = '" + llave + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    protected string RevisaSAGE(Comprobante xml, uCFDsLib.v33.Pagos Pagos)
    {
        string Res = string.Empty;
        try
        {
            /// DATOS GENERALES XML
            if (xml.Fecha == null) { Res = "El encabezado XML no contiene fecha." + "<br/>"; }
            if (xml.Version == null) { Res = "El encabezado XML no contiene versión." + "<br/>";}
            if (xml.LugarExpedicion.ToString() == null) { Res = "El encabezado XML no contiene lugar de expedición." + "<br/>"; }
            if (xml.Moneda.ToString() != "XXX") { Res = "El encabezado XML no contiene moneda valida." + "<br/>"; }
            if (xml.Certificado == null) { Res = "El encabezado XML no contiene certificado." + "<br/>"; }
            if (xml.NoCertificado == null) { Res = "El encabezado XML no contiene N° de certificado." + "<br/>"; }
            if (xml.Sello == null) { Res = "El encabezado XML no contiene sello." + "<br/>"; }
            if (xml.TipoDeComprobante.ToString() != "P") { Res = "El tipo de comprobante debe ser 'P'." + "<br/>"; }
            if (Convert.ToDecimal(xml.Total.ToString()) >0) { Res = Res + "El Total de comprobante debe ser 0." + "<br/>"; }
            if (xml.SubTotal.ToString() != "0") { Res = Res + "El Subtotal de comprobante debe ser 0." + "<br/>"; }
            if (xml.Emisor.Rfc == null) { Res = Res + "El emisor no contiene RFC. " + " < br/>"; }
            if (xml.Emisor.RegimenFiscal.ToString() == null) { Res = Res + "El emisor no contiene régimen fiscal." + " < br/>"; }
            if (xml.Receptor.Rfc == null) { Res = Res + "El receptor no contiene RFC. " + " < br/>"; }
            if (xml.Receptor.UsoCFDI.ToString() == null) { Res = Res + "El receptor no contiene Uso de CFDI." + " < br/>"; }

            ComprobanteConcepto Conceptos = new ComprobanteConcepto();
            Conceptos = xml.Conceptos[0];
            string strItemIdSAT = xml.Conceptos[0].ClaveProdServ.ToString().Substring(xml.Conceptos[0].ClaveProdServ.ToString().Length - 8, 8);
            //Conceptos
            if (Convert.ToDecimal(Conceptos.Importe.ToString()) > 1) { Res = Res + "La cantidad en el concepto del comprobante debe ser 1." + "<br/>"; }
            if (Convert.ToDecimal(Conceptos.Importe.ToString()) > 0) { Res = Res + "El importe en el concepto del comprobante debe ser 0." + "<br/>"; }
            if (Convert.ToDecimal(Conceptos.ValorUnitario.ToString()) > 0) { Res = Res + "El valor unitario en el concepto del comprobante debe ser 0." + "<br/>"; }
            if (Conceptos.Descripcion.ToString() != "Pago") { Res = Res + "La descripción en el concepto del comprobante debe ser 'Pago'." + "<br/>"; }
            if (Conceptos.ClaveUnidad.ToString() != "ACT") { Res = Res + "La clave unidad en el concepto del comprobante debe ser 'ACT'." + "<br/>"; }
            if (strItemIdSAT != "84111506") { Res = Res + "La clave de producto y servicio en el concepto del comprobante debe ser '84111506'." + "<br/>"; }

            //Facturas Pagadas
            DoctoRelacionado Docu1 = new DoctoRelacionado();
            if (Pagos.Pago.FechaPago == null) { Res = "El Nodo Pagos:10 no contiene fecha de pago en el nodo pago10:Pago" + "<br/>"; }
            if (Pagos.Pago.MonedaP.ToString() == null) { Res = "El comprobante  no cuenta con el atributo MonedaP en el nodo pago10:Pago" + "<br/>"; }
            if (Pagos.Pago.FormaDePagoP.ToString() == null) { Res = "El comprobante  no cuenta con el atributo FormaDePagoP en el nodo pago10:Pago" + "<br/>"; }
            //if (Pagos.Pago.Monto.ToString() == null) { Res = "El comprobante  no cuenta con el atributo Monto" + "<br/>"; }
            //if (Pagos.Pago.Monto.ToString() == null) { Res = "El comprobante  no cuenta con el atributo Monto" + "<br/>"; }
            //if (Pagos.Pago.CtaOrdenante == null) { Res = Res + "El comprobante no cuenta con el atributo CtaOrdenante en el nodo de pagos" + "<br/>"; }
            //if (Pagos.Pago.NomBancoOrdExt == null) { Res = Res + "El comprobante no cuenta con el atributo NomBancoOrdExt en el nodo de pagos" + "<br/>"; }
            //if (Pagos.Pago.NumOperacion == null) { Res = Res + "El comprobante no cuenta con el atributo NumOperacion en el nodo de pagos" + "<br/>"; }
            //if (Pagos.Pago.RfcEmisorCtaBen == null) { Res = Res + "El comprobante no cuenta con el atributo RfcEmisorCtaBen en el nodo de pagos" + "<br/>"; }
            //if (Pagos.Pago.RfcEmisorCtaOrd == null) { Res = Res + "El comprobante no cuenta con el atributo RfcEmisorCtaOrd en el nodo de pagos" + "<br/>"; }

            if (Res != "") { return Res; }

            //if (ReDoc(Pagos.Pago.Monto, Pagos.Pago.FechaPago.ToString()) == false) { Res = Res + "Los datos Bancarios no coinciden con los registrados en T|SYS| o la Empresa destino no coincide con los datos del pago,Revisalos con los proporcionados en el Email de Confirmación" + "<br/>"; return Res; }
            if (Pagos.Pago.DoctoRelacionado.Count() == 0) { Res = Res + "El comprobante no cuenta documentos relacionados en el nodo de pagos." + "<br/>"; }
            else { 
            for (int j = 0; j < Pagos.Pago.DoctoRelacionado.Count(); j++)
            {  
                Docu1 = Pagos.Pago.DoctoRelacionado[j];

             if (Docu1.IdDocumento.ToString() == null) { Res = Res + "El comprobante no cuenta con el atributo IdDocumento en el nodo de pagos." + "<br/>"; }
             if (Docu1.MonedaDR.ToString() == null) { Res = Res + "El comprobante no cuenta con el atributo MonedaDR en el nodo pago10:DoctoRelacionado." + "<br/>"; }
             if (Docu1.MetodoDePagoDR.ToString() == null) { Res = Res + "El comprobante no cuenta con el atributo MetodoDePagoDR en el nodo pago10:DoctoRelacionado." + "<br/>"; }
             if (Docu1.ImpSaldoAnt.ToString() == null) { Res = Res + "El comprobante no cuenta con el atributo ImpSaldoAnt en el nodo pago10:DoctoRelacionado." + "<br/>"; }
             if (Docu1.ImpSaldoInsoluto.ToString() == null) { Res = Res + "El comprobante no cuenta con el atributo ImpSaldoInsoluto en el nodo pago10:DoctoRelacionado." + "<br/>"; }
             if (Docu1.ImpPagado.ToString() == null) { Res = Res + "El comprobante no cuenta con el atributo ImpPagado en el nodo pago10:DoctoRelacionado." + "<br/>"; }

             if (Res != "") { return Res; }

             int VF = VerFechaP(Pagos.Pago.FechaPago.ToShortDateString());
             if (VF == -1) { Res = Res + "Ocurrió un problema al intentar verificar la fecha de pago, verifica que se encuentre en el formato correcto, en caso de persistir el problema comunícate con el área de soporte." + " <br/>"; }
             if (VF == 0) { Res = Res + "No se encontró ningún comprobante con la fecha de pago ingresada, verifícalo con el email de confirmación." + "<br/>"; }
             if (Res != "") { return Res; }
             int VP = VerMontoP(Pagos.Pago.Monto);
             if (VP == -1) { Res = Res + "Ocurrió un problema al intentar verificar el monto de pago, verifica que se encuentre en el formato correcto, en caso de persistir el problema comunícate con el área de soporte." + " <br/>"; }
             if (VP == 0) { Res = Res + "No se encontró ningún comprobante con la cantidad de pago total ingresada, verifícalo con el email de confirmación." + "<br/>"; }

             if (Res != "") { return Res; }


                string UUIP = Docu1.IdDocumento;
                string FolioP = Docu1.Folio;
                string Mon = Docu1.MonedaDR.ToString();
                //string Cta = Pagos.Pago.CtaOrdenante.ToString();
                string Meto = Docu1.MetodoDePagoDR;
                string SalT = Docu1.ImpSaldoAnt.ToString();
                string SalN = Docu1.ImpSaldoInsoluto.ToString();
                string PAgo = Docu1.ImpPagado.ToString();
                decimal Pag = (Convert.ToDecimal(PAgo.ToString()));
                decimal SaldoN = (Convert.ToDecimal(SalN.ToString()));
                string FolioCont = string.Empty;
                string RFCE = xml.Emisor.Rfc;
                string RFCR = xml.Receptor.Rfc;
                string Ms = string.Empty;

                if (FolioP == "") { Ms = " con el UUID " + UUIP; } else { Ms = " con el Folio " + FolioP; }
                //{ Res = Res + "No se encontró relación del pago hacia la factura con UUDI " + UUIP + " , verifícalo con los datos proporcionados en el email de confirmación" + "<br/>"; return Res; }
                Res = ReDocs(Pag, Pagos.Pago.Monto, Pagos.Pago.FechaPago.ToShortDateString(), UUIP, Ms);
                if (Res != "") { return Res; }

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                  string Caden = "spGetPmtDetail";
                  SqlCommand cmd1 = new SqlCommand(Caden, conn);
                  cmd1.CommandType = CommandType.StoredProcedure;
                  cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UUID", Value = UUIP });
                  if (conn.State == ConnectionState.Open) { conn.Close(); }
                  conn.Open();
                  SqlDataReader rdr1 = cmd1.ExecuteReader();
                  if (rdr1.HasRows)
                  {
                   while (rdr1.Read())
                    {
                     if (rdr1["RFCEmisor"].ToString() != RFCE) { Res = Res + "El RFC emisor del comprobante no coincide con el RFC emisor registrado para el comprobante con folio " + FolioP + ".<br/>"; }
                     if (rdr1["RFCReceptor"].ToString() != RFCR) { Res = Res + "El RFC receptor del comprobante no coincide con el RFC receptor registrado para el comprobante con folio " + FolioP + ".<br/>"; }
                     int Sta = Convert.ToInt32(rdr1["Status"].ToString());
                     if (Sta <= 6) { Res = Res + "El comprobante no está habilitado para recibir complemento de pago." + "<br/>";}
                     if (Sta == 9) { Res = Res + "El comprobante ya se encuentra registrado como pagado." + "<br/>";}
                     //string fl = rdr1["Folio"].ToString();
                     //if (rdr1["Folio"].ToString() != FolioP) { Res = Res + "El Folio ingresado para el comprobante Asociado al UUID  " + UUIP + " no coincide con el registrado en T|SYS|" + "<br/>";}
                     if (rdr1["MetodoPago"].ToString() != Meto) { Res = Res + "El método de pago ingresado para el comprobante con folio  " + FolioP + " no coincide con el registrado en T|SYS|" + "<br/>"; }
                     if (rdr1["Moneda"].ToString() != Mon.ToString()) { Res = Res + "El tipo de moneda registrado para el comprobante con folio " + FolioP + " no coincide con el registrado en T|SYS|" + "<br/>"; }
                     decimal Sal = (Convert.ToDecimal(rdr1["Total"].ToString()));
                     decimal Resta = (Sal - Pag);
                     if (Resta < 0) { Res = Res + "El importe registrado para el comprobante con UUID " + UUIP + " excede contra el saldo total registrado en T|SYS|" + "<br/>"; }
                     if (Resta != SaldoN) { Res = Res + "El ImpSaldoInsoluto registrado en el comprobante no coincide con el nuevo saldo registrado en T|SYS|" + "<br/>"; }

                   }
                 }
                 else
                 {
                  Res = Res + "No se encontro ningún registro de alguna factura con el UUID " + UUIP + "<br/>";
                  conn.Close();
                  return Res;
                 }
              }
            }
          }
        }
        catch (Exception ex)
        {
          Res = ex.Message;
        }
        return Res;
    }

    protected string GETUUID(string Fecha,string OC)
    {
        string Cuenta = "";
        try
        {
            string sql;
            

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            string Vendor = HttpContext.Current.Session["VendKey"].ToString();
            Vendor = SelProv.SelectedItem.ToString();
            sqlConnection1.Open();

            sql = @"SELECT top 1 UUID From Invoice a inner join Vendors b on a.VendorKey = b.vendorkey Where Folio ='" + Fecha + "' And b.VendName = '" + Vendor + "' AND NodeOC = '" + OC + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            //if (Convert.ToInt32(Cuenta) > 0)
            //    return Rest = 1;
            //else
            //    return Rest = 0;
        }
        catch (Exception ex)
        {
            int iLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int iUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string CompanyID = HttpContext.Current.Session["IDCompany"].ToString();
            LogError(iLogKey, iUserKey, "Carga de Complemento de Pagos_VerFechaP", ex.Message, CompanyID);
            Cuenta = "";
        }
        //return Rest;
        return Cuenta;
    }

    protected int VerFechaP(string Fecha)
    {
        int Rest = 0;
        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("ConnectionString");
            string Vendor = HttpContext.Current.Session["VendKey"].ToString();
            sqlConnection1.Open();
            Fecha = Tools.ObtenerFechaEnFormatoNewPago(Fecha);
            Fecha = Fecha.Substring(0, 10);
            Fecha = Fecha.Replace("/", "-");

            sql = @"SELECT COUNT(*) As 'Cuenta' FROM tapVendPmt Where TranDate ='" + Fecha + "' And VendKey = '" + Vendor + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            if (Convert.ToInt32(Cuenta) > 0)
                return Rest = 1;
            else
                return Rest = 0;
        }
        catch(Exception ex)
        {
            int iLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int iUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string CompanyID = HttpContext.Current.Session["IDCompany"].ToString();
            LogError(iLogKey, iUserKey, "Carga de Complemento de Pagos_VerFechaP", ex.Message, CompanyID);
            Rest = -1;
        }
        return Rest;        
    }

    protected int VerMontoP(decimal Monto)
    {
        int Rest = 0;
        try
        {
            string sql;
            string Cuenta;

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("ConnectionString");
            string Vendor = HttpContext.Current.Session["VendKey"].ToString();
            sqlConnection1.Open();

            sql = @"SELECT COUNT(*) As 'Cuenta' FROM tapVendPmt Where TranAmt ='" + Monto + "' And VendKey = '" + Vendor + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            if (Convert.ToInt32(Cuenta) > 0)
                return Rest = 1;
            else
                return Rest = 0;
        }
        catch(Exception ex)
        {
            int iLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int iUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string CompanyID = HttpContext.Current.Session["IDCompany"].ToString();
            LogError(iLogKey, iUserKey, "Carga de Complemento de Pagos_VerFechaP", ex.Message, CompanyID);
            Rest = -1;
        }
        return Rest;
    }

    protected bool ReDoc(decimal Pago,string Fecha,string UUID)
    {
        bool Doc = false;
        try
        {
            Fecha = Tools.ObtenerFechaEnFormatoNewPago(Fecha);
            Fecha = Fecha.Substring(0, 10);
            Fecha = Fecha.Replace("/", "-");
            string UserKey = HttpContext.Current.Session["UserKey"].ToString();
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            string Key = string.Empty;

            //Verifica Monto Total del Pago
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Caden = "spGetPmtAppl";
                SqlCommand cmd1 = new SqlCommand(Caden, conn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add(new SqlParameter(){ ParameterName = "@UserKey", Value = UserKey });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Compan", Value = Company });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Pago", Value = Pago });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Fecha", Value = Fecha });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UUID", Value = UUID });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Key", Value = 0 });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 1 });
                if (conn.State == ConnectionState.Open){conn.Close();}
                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
                if (rdr1.HasRows) { Doc = true; Key = rdr1["VendPmtKey"].ToString(); } else { Doc = false; }
            }

            //Verifica el Pago relacionado con la Factura
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Caden = "spGetPmtAppl";
                SqlCommand cmd1 = new SqlCommand(Caden, conn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = UserKey });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Compan", Value = Company });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Pago", Value = Pago });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Fecha", Value = Fecha });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UUID", Value = UUID });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Key", Value = Key });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 2 });
                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
                if (rdr1.HasRows) { Doc = true; }
            }

            //Verifica Monto del Pago hacia la Factura
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Caden = "spGetPmtAppl";
                SqlCommand cmd1 = new SqlCommand(Caden, conn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = UserKey });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Compan", Value = Company });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Pago", Value = Pago });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Fecha", Value = Fecha });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UUID", Value = UUID });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Key", Value = Key });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 3 });
                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
                if (rdr1.HasRows) { Doc = true; }
            }

            //Verifica Fecha del Pago hacia la Factura
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Caden = "spGetPmtAppl";
                SqlCommand cmd1 = new SqlCommand(Caden, conn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = UserKey });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Compan", Value = Company });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Pago", Value = Pago });                
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Fecha", Value = Fecha });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UUID", Value = UUID });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Key", Value = Key });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 4 });
                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
                if (rdr1.HasRows) { Doc = true; }
            }
        }
        catch(Exception ex)
        {
            string mess = ex.Message;
        }
        return Doc;

    }

    protected string ReDocs(decimal Pago, decimal PagoG, string Fecha, string UUID,string Ms)
    {
        string Doc = string.Empty;
        try
        {
            Fecha = Tools.ObtenerFechaEnFormatoNewPago(Fecha);
            Fecha = Fecha.Substring(0, 10);
            Fecha = Fecha.Replace("/", "-");
            string UserKey = HttpContext.Current.Session["UserKey"].ToString();
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            string Key = string.Empty;

            //Verifica Monto Total del Pago
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Caden = "spGetPmtAppl";
                SqlCommand cmd1 = new SqlCommand(Caden, conn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = UserKey });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Compan", Value = Company });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Pago", Value = PagoG });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Fecha", Value = Fecha });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UUID", Value = UUID });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Key", Value = 0 });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 1 });
                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
                if (rdr1.HasRows) { Doc = ""; while (rdr1.Read()) { Key = rdr1["VendPmtKey"].ToString(); } } else { Doc = "No se encontró ningún pago con los datos ingresados, verifícalo con los datos proporcionados en el email de confirmación."; return Doc; }
            }

            //Verifica el Pago relacionado con la Factura
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Caden = "spGetPmtAppl";
                SqlCommand cmd1 = new SqlCommand(Caden, conn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = UserKey });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Compan", Value = Company });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Pago", Value = Pago });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Fecha", Value = Fecha });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UUID", Value = UUID });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Key", Value = Key });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 2 });
                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
                if (rdr1.HasRows) { Doc = ""; } else { Doc = "El comprobante " + Ms + " no pertenece al pago registrado en TSYS con la fecha y monto ingresado, verifícalo con los datos proporcionados en el email de confirmación"; return Doc; }
            }

            //Verifica Monto del Pago hacia la Factura
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Caden = "spGetPmtAppl";
                SqlCommand cmd1 = new SqlCommand(Caden, conn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = UserKey });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Compan", Value = Company });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Pago", Value = Pago });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Fecha", Value = Fecha });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UUID", Value = UUID });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Key", Value = Key });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 3 });
                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
                if (rdr1.HasRows) { Doc = ""; } else { Doc = "El monto registrado para el comprobante " + Ms + " no coincide con el registrado en TSYS, verifícalo con los datos proporcionados en el email de confirmación."; return Doc; }
            }

            //Verifica Fecha del Pago hacia la Factura
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Caden = "spGetPmtAppl";
                SqlCommand cmd1 = new SqlCommand(Caden, conn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = UserKey });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Compan", Value = Company });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Pago", Value = Pago });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Fecha", Value = Fecha });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@UUID", Value = UUID });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Key", Value = Key });
                cmd1.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 4 });
                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
                if (rdr1.HasRows) { Doc = ""; } else { Doc = "La fecha registrada para el comprobante " + Ms + " no coincide con el registrado en TSYS, verifícalo con los datos proporcionados en el email de confirmación."; return Doc; }
            }
        }
        catch (Exception ex)
        {
            string mess = ex.Message;
        }
        return Doc;

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

                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionPagos.html")))
                    {
                        body = reader.ReadToEnd();
                        body = body.Replace("{PassTemp}", PassNew);

                    }

            Global.EmailGlobal(Destinatario, body, "CARGA DE COMPLEMENTO DE PAGOS");

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
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = "Notificación Pagos" });
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

}