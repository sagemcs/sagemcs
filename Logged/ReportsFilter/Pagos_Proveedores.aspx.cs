//PORTAL DE PROVEDORES T|SYS|
//20 NOVIEMBRE, 2022
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA EXPRESS PARA CARGA DE COMPLEMENTO DE PAGOS DE PROVEEDORES

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

public partial class Pagos_Proveedores : System.Web.UI.Page
{
    int err = 0;
    string eventName = String.Empty;

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
                string rol = HttpContext.Current.Session["RolUser"].ToString();
                if (rol.Contains("T|SYS| "))
                {
                    CargaProv();
                    CargaProvs();
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
            int variabl = 0;

            if (cb1.Checked == true)
            {
                int Sel = revisaProv();
                if (Sel == 0)
                {
                    tex.Text = Math.Round(Convert.ToDecimal(costoz), 4).ToString();
                    tex.Enabled = true;
                }
                else 
                {
                    cb1.Checked = false;
                    variabl = 1;

                }
            }
            else
            {
                tex.Text = Math.Round(Convert.ToDecimal(costoz), 4).ToString();
                GridView1.Rows[index].Cells[10].Text = costoz;
                tex.Enabled = false;
            }
            Conteo();

            if (variabl == 1) 
            {
                Label2.Text = "Ha seleccionado facturas que son de diferentes proveedores.";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL);", true);
            }
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

    protected int revisaProv() 
    {
        int Conteo = 0;
        try
        {
            string valor = "";
            foreach (GridViewRow row in GridView1.Rows)
            {
                int index = row.RowIndex;
                CheckBox cb1 = (CheckBox)GridView1.Rows[index].FindControl("Check");
                string Prov = GridView1.Rows[index].Cells[2].Text.ToString(); // total

                if (cb1.Checked == true)
                {
                    if (valor == "")
                    {
                        valor = Prov;
                    }
                    else 
                    {
                        if (valor == Prov) 
                        {  }
                        else 
                        {
                            Conteo = Conteo + 1;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());

            LogError(pLogKey, pUserKey, "Carga-Factura:revisaProv ", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            HttpContext.Current.Session["Error"] = err;
            Mensajes.Text = HttpContext.Current.Session["Error"].ToString();
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B1);", true);
        }

        return Conteo;
    }

    protected void Unnamed1_Click(object sender, EventArgs e)
    {
        Verifica();
    }

    protected void Verifica()
    {

        Panel2.Visible = false;
        string text= string .Empty;
        string sld = string.Empty;
        sld = Conteo3();

        //if (!FileUpload2.HasFile && !FileUpload1.HasFile)
        //{
        //    text = "No se Encontraron Archivos";
        //}
        //else if (!FileUpload2.HasFile)
        //{
        //    text = "Ingresa un XML";
        //}
        if (!FileUpload1.HasFile)
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
                //CargaProv();
                CargaProvs();
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
                    
                    string Vendr = HttpUtility.HtmlDecode(gvr.Cells[2].Text);
                    string UUIDF = GETUUID(Folio, OCom, Vendr);

                    //Sube Archivos al Portal
                    string Respues = Execute(FileUpload1, FileUpload1, UUID, Folio, "", fec1, fec2, "4.0", Totsl, Vendr).ToString();
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

    protected void CargaProvs()
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
                int Opc = 20;

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
                SelP.Items.Clear();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Nombre"].ToString().Length > 1)
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Nombre"].ToString());
                        SelP.Items.Insert(0, Linea);
                    }

                }

                ListItem Lineas = new ListItem();
                Lineas.Value = "Todos";
                SelP.Items.Insert(0, Lineas);

                conn.Close();
            }

            //if (SelProv.Items.Count == 0)
            //{
            //    DatosV.Visible = true;
            //    //Datos.Visible = false;
            //}
            //else
            //{
            //    DatosV.Visible = false;
            //    //Datos.Visible = true;
            //}
        }
        catch (Exception ex)
        {
            //RutinaError(ex);
        }
    }

    protected void List_SelectProvs(object sender, EventArgs e)
    {
        CambiaProv();
    }

    protected void CambiaProv()
    {
        try
        {
            string SQL = string.Empty;
            //string prov = SelProv.SelectedItem.ToString();
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            string users = HttpContext.Current.Session["UserKey"].ToString();
            int Filas = 0;
            string Vend = "0";
            SQL = "";
            SQL = "spGetDocsEmpleados";

            if (SelP.SelectedItem.ToString() != "Todos")
            {
                Vend = SelP.SelectedItem.ToString();
            }

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 3 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@UserKey", Value = users });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Prov", Value = Vend });
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

    protected int GetVkeys()
    {
        int vkey = 0;
        try
        {
            string SQL = string.Empty;
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            //string prov = SelProv.SelectedItem.ToString();
            //string Company = iCompanyID;
            //Company = "IEP";

            SQL = "";
            //SQL = SQL + " Select VendorKey From Vendors Where VendName =  '" + prov + "' AND CompanyID = '" + Company + "'";

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

    protected string GetVkey(string prov)
    {
        int vkey = 0;
        try
        {
            string SQL = string.Empty;
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            //string prov = SelProv.SelectedItem.ToString();
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
        return vkey.ToString();
    }

    protected string GetUskey(string prov)
    {
        int vkey = 0;
        try
        {
            string SQL = string.Empty;
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            //string prov = SelProv.SelectedItem.ToString();
            //string Company = iCompanyID;
            //Company = "IEP";

            SQL = "";
            SQL = SQL + " Select UserKey From Vendors Where VendName =  '" + prov + "' AND CompanyID = '" + Company + "'";

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
        return vkey.ToString();
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

    public string Execute(FileUpload Caja,FileUpload Caja2,string UUID,string Folio,string Serie,string F1,string F2,string Version,string Total,string Vendr)
     {
        string res;

        try
        {
            string Filename = Caja.FileName.ToString();

            //Fabian
            Byte[] bytes1 = FileUpload1.FileBytes;
            Byte[] bytes2 = FileUpload1.FileBytes;
            //}
            //string Ext = ".XML";
            string Ext  = ".pdf";
            string Ext2 = ".pdf";
            string Var1 = HttpContext.Current.Session["IDComTran"].ToString();
            string Var2 = HttpContext.Current.Session["VendKey"].ToString();
            string Var3 = HttpContext.Current.Session["UserKey"].ToString();

            //int VendKey = GetVkey();
            string Vendor = GetVkey(Vendr);
            int VendKey = Convert.ToInt32(Vendor);
            string Usario = GetUskey(Vendr);
            int UserKey = Convert.ToInt32(Usario);
            //int UserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());

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

    protected string GETUUID(string Fecha,string OC,string vend)
    {
        string Cuenta = "";
        try
        {
            string sql;
            

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            //string Vendor = HttpContext.Current.Session["VendKey"].ToString();
            //Vendor = SelProv.SelectedItem.ToString();
            string Vendor = GetVkey(vend);
            sqlConnection1.Open();

            sql = @"SELECT top 1 UUID From Invoice a inner join Vendors b on a.VendorKey = b.vendorkey Where Folio ='" + Fecha + "' And b.vendorkey = '" + Vendor + "' AND NodeOC = '" + OC + "'";

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

    /////////////////////////////////////////////// Consulta \\\\\\\\\\\\\\\\\\\\\\\
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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
                int Opc = 19;

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

                ListItem Lineas = new ListItem();
                Lineas.Value = "Todos";
                SelProv.Items.Insert(0, Lineas);

                conn.Close();
            }

            //if (SelProv.Items.Count == 0)
            //{
            //    DatosV.Visible = true;
            //    //Datos.Visible = false;
            //}
            //else
            //{
            //    DatosV.Visible = false;
            //    //Datos.Visible = true;
            //}
        }
        catch (Exception ex)
        {
            //RutinaError(ex);
        }
    }

    private string Filtros()
    {
        string Cadena = string.Empty;
        string Vendor = HttpContext.Current.Session["UserKey"].ToString();
        //int Vedn = int.Parse(Vendor.ToString());

        Cadena = "Select a.Folio,c.Serie,c.Folio,convert(decimal(12, 2), c.Total),e.VendName,d.CompanyName,c.CreateDate,c.PaymentDate,g.Descripcion from Invoice a INNER JOIN PaymentAppl b ON b.ApplInvoiceKey = a.InvoiceKey INNER JOIN Payment c ON b.PaymentKey = c.PaymentKey INNER JOIN Company d ON c.CompanyID = d.CompanyID INNER JOIN Vendors e ON c.VendorKey = e.VendorKey INNER JOIN StatusDocs g ON c.Status = g.Status Where g.Descripcion = '";

        Cadena = Cadena + List.SelectedItem.ToString() + "'";

        //Cadena = Cadena + " AND f.UserKey = " + Vendor + " ";

        //if (SelProv.SelectedItem.ToString() != "Todos")
        //{
        //    Cadena = " AND c.VendName = '" + SelProv.SelectedItem.ToString() + "' ";
        //}
        if (SelProv.Items.Count >= 1)
        {
            if (SelProv.SelectedItem.ToString() != "Todos")
            {
                Cadena = Cadena + " AND e.VendName = '" + SelProv.SelectedItem.ToString() + "' ";
            }
        }

        if (Folio.Text != "")
        {
            Cadena = Cadena + " AND a.Folio LIKE '%" + Folio.Text + "%' ";
        }

        if (Factura.Text != "")
        {
            Cadena = Cadena + " AND f.Folio LIKE '%" + Factura.Text + "%' ";
        }

        if (Monto.Text != "")
        {
            string Total = Monto.Text.Replace("$", "");
            Cadena = Cadena + " AND a.Total LIKE '%" + Total + "%' ";
        }

        if (ChkFechas.Checked == true)
        {
            if (LFechas.SelectedValue == "Factura")
            {
                if (F1.Text != "")
                {
                    Cadena = Cadena + " AND a.CreateDate >= '" + F1.Text + "' ";
                }
                if (F2.Text != "")
                {
                    Cadena = Cadena + " AND a.CreateDate <= '" + F2.Text + " 23:59:59' ";
                }
            }
            if (LFechas.SelectedValue == "Pago")
            {
                if (F1.Text != "")
                {
                    Cadena = Cadena + " AND a.PaymentDate >= '" + F1.Text + "' ";
                }
                if (F2.Text != "")
                {
                    Cadena = Cadena + " AND a.PaymentDate <= '" + F2.Text + " 23:59:59' ";
                }
            }
        }

        Cadena = Cadena + " Order By a.InvoiceKey DESC ";

        return Cadena;

    }

    private void BindGridView()
    {
        DataTable dt = new DataTable();
        DatosV.Visible = false;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            // Create a DataSet object. 
            string SpFiltros;
            SpFiltros = Filtros();
            DataSet dsProveedores = new DataSet();
            SqlCommand cmd = new SqlCommand(SpFiltros, conn);
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.Add(new SqlParameter()
            //{ ParameterName = "@Cadena", Value = SpFiltros });

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            conn.Open();

            string Errores = string.Empty;
            SqlDataReader rdr = cmd.ExecuteReader();

            dt.Columns.Add("Factura");
            dt.Columns.Add("Serie");
            dt.Columns.Add("Folio");
            dt.Columns.Add("Total");
            dt.Columns.Add("Proveedor");
            dt.Columns.Add("Company");
            dt.Columns.Add("FechaR");
            dt.Columns.Add("FechaP");
            dt.Columns.Add("Status");

            while (rdr.Read())
            {
                DataRow row = dt.NewRow();

                row["Factura"] = Convert.ToString(rdr.GetValue(0));
                row["Serie"] = Convert.ToString(rdr.GetValue(1));
                row["Folio"] = Convert.ToString(rdr.GetValue(2));
                row["Total"] = "$" + " " + Convert.ToString(rdr.GetValue(3));
                row["Proveedor"] = Convert.ToString(rdr.GetValue(4));
                row["Company"] = Convert.ToString(rdr.GetValue(5));
                row["FechaR"] = Convert.ToString(rdr.GetValue(6));
                row["FechaP"] = Convert.ToString(rdr.GetValue(7));
                row["Status"] = Convert.ToString(rdr.GetValue(8));
                dt.Rows.Add(row);
            }
        }

        GridView3.DataSource = dt;
        GridView3.DataBind();

        if (dt.Rows.Count == 0) { Panel1.Visible = true; } else { Panel1.Visible = false; }
    }

    protected void Buscar(object sender, EventArgs e)
    {
        try
        {
            BindGridView();
        }
        catch
        {

        }
    }

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView1.PageIndex = e.NewPageIndex;
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

        }
    }

    protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {

    }

    protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView1.EditIndex = e.NewEditIndex;
        try
        {
            int x;
            DataTable dt = new DataTable();

            dt.Columns.Add("Factura");
            dt.Columns.Add("Serie");
            dt.Columns.Add("Folio");
            dt.Columns.Add("Total");
            dt.Columns.Add("Proveedor");
            dt.Columns.Add("Company");
            dt.Columns.Add("FechaR");
            dt.Columns.Add("FechaP");
            dt.Columns.Add("Status");

            x = GridView1.Rows.Count;

            if (x == 0)
            {
                GridView1.DataSource = "";
            }
            else
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["Factura"] = gvr.Cells[2].Text.ToString();
                    dr["serie"] = gvr.Cells[3].Text.ToString();
                    dr["Folio"] = gvr.Cells[4].Text.ToString();
                    dr["Total"] = gvr.Cells[5].Text.ToString();
                    dr["Proveedor"] = gvr.Cells[6].Text.ToString();
                    dr["company"] = gvr.Cells[7].Text.ToString();
                    dr["FechaR"] = gvr.Cells[8].Text.ToString();
                    dr["FechaP"] = gvr.Cells[9].Text.ToString();
                    dr["Status"] = gvr.Cells[10].Text.ToString();
                    dt.Rows.Add(dr);
                }
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
        catch (Exception v)
        {

        }

    }

    protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        // Exit edit mode. 
        try
        {
            int i, x;
            i = e.RowIndex;
            GridView1.EditIndex = -1;
            DataTable dt = new DataTable();

            dt.Columns.Add("Factura");
            dt.Columns.Add("Serie");
            dt.Columns.Add("Folio");
            dt.Columns.Add("Total");
            dt.Columns.Add("Proveedor");
            dt.Columns.Add("Company");
            dt.Columns.Add("FechaR");
            dt.Columns.Add("FechaP");
            dt.Columns.Add("Status");

            x = GridView1.Rows.Count;

            if (x == 0)
            {
                GridView1.DataSource = "";
            }
            else
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();

                        string var1 = gvr.Cells[1].Text.ToString();
                        string var2 = gvr.Cells[2].Text.ToString();
                        string var3 = gvr.Cells[3].Text.ToString();
                        string var4 = gvr.Cells[4].Text.ToString();


                        dr["Factura"] = gvr.Cells[2].Text.ToString();
                        dr["serie"] = gvr.Cells[3].Text.ToString();
                        dr["Folio"] = gvr.Cells[4].Text.ToString();
                        dr["Total"] = gvr.Cells[5].Text.ToString();
                        dr["Proveedor"] = gvr.Cells[6].Text.ToString();
                        dr["company"] = gvr.Cells[7].Text.ToString();
                        dr["FechaR"] = gvr.Cells[8].Text.ToString();
                        dr["FechaP"] = gvr.Cells[9].Text.ToString();
                        dr["Status"] = gvr.Cells[10].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["Factura"] = gvr.Cells[2].Text.ToString();
                        dr["serie"] = gvr.Cells[3].Text.ToString();
                        dr["Folio"] = gvr.Cells[4].Text.ToString();
                        dr["Total"] = gvr.Cells[5].Text.ToString();
                        dr["Proveedor"] = gvr.Cells[6].Text.ToString();
                        dr["company"] = gvr.Cells[7].Text.ToString();
                        dr["FechaR"] = gvr.Cells[8].Text.ToString();
                        dr["FechaP"] = gvr.Cells[9].Text.ToString();
                        dr["Status"] = "No Aprobado";
                        dt.Rows.Add(dr);
                    }
                }
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
        catch (Exception v)
        {

        }
    }

    protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        try
        {
            int i, x;
            i = e.RowIndex;
            GridView1.EditIndex = -1;
            DataTable dt = new DataTable();

            dt.Columns.Add("Factura");
            dt.Columns.Add("Serie");
            dt.Columns.Add("Folio");
            dt.Columns.Add("Total");
            dt.Columns.Add("Proveedor");
            dt.Columns.Add("Company");
            dt.Columns.Add("FechaR");
            dt.Columns.Add("FechaP");
            dt.Columns.Add("Status");

            x = GridView1.Rows.Count;

            if (x == 0)
            {
                GridView1.DataSource = "";
            }
            else
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();

                        dr["Factura"] = gvr.Cells[2].Text.ToString();
                        dr["serie"] = gvr.Cells[3].Text.ToString();
                        dr["Folio"] = gvr.Cells[4].Text.ToString();
                        dr["Total"] = gvr.Cells[5].Text.ToString();
                        dr["Proveedor"] = gvr.Cells[6].Text.ToString();
                        dr["company"] = gvr.Cells[7].Text.ToString();
                        dr["FechaR"] = gvr.Cells[8].Text.ToString();
                        dr["FechaP"] = gvr.Cells[9].Text.ToString();
                        dr["Status"] = gvr.Cells[10].Text.ToString();
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["Factura"] = gvr.Cells[2].Text.ToString();
                        dr["serie"] = gvr.Cells[3].Text.ToString();
                        dr["Folio"] = gvr.Cells[4].Text.ToString();
                        dr["Total"] = gvr.Cells[5].Text.ToString();
                        dr["Proveedor"] = gvr.Cells[6].Text.ToString();
                        dr["company"] = gvr.Cells[7].Text.ToString();
                        dr["FechaR"] = gvr.Cells[8].Text.ToString();
                        dr["FechaP"] = gvr.Cells[9].Text.ToString();
                        dr["Status"] = "Aprobado";
                        dt.Rows.Add(dr);
                    }
                }
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
        catch (Exception v)
        {

        }

    }

    protected int ObtenerVK(string razon, string company)
    {
        int VK = 0;
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            string sql;
            string Cuenta;
            sqlConnection1.Open();

            sql = @"Select VendorKey as Cuenta from Vendors a inner join Company b on a.CompanyID = b.CompanyID Where b.CompanyName Like '%" + company + "%' And a.VendName Like '%" + razon + "%'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }
            VK = Convert.ToInt16(Cuenta);
            sqlConnection1.Close();

        }
        catch (Exception ex)
        {
            VK = 0;
        }
        return VK;
    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {

            MemoryStream memoryStream = new MemoryStream();

            int index = Convert.ToInt32(e.CommandArgument);
            string Pago, archivo, Folio, Cte;
            GridViewRow row = GridView3.Rows[index];
            string vend = HttpUtility.HtmlDecode(row.Cells[6].Text);
            string vens = HttpUtility.HtmlDecode(row.Cells[7].Text);
            int vkey = ObtenerVK(HttpUtility.HtmlDecode(row.Cells[6].Text), HttpUtility.HtmlDecode(row.Cells[7].Text));
            Folio = HttpUtility.HtmlDecode(row.Cells[3].Text);
            Pago = HttpUtility.HtmlDecode(row.Cells[5].Text);
            Cte = HttpUtility.HtmlDecode(row.Cells[7].Text);


            if (e.CommandName == "Documento_2")
            {
                string tipo = ".pdf";
                if (e.CommandName == "Documento_2")
                {
                    memoryStream = databaseFileRead("InvoiceKey", row.Cells[4].Text, "FileBinary", tipo, vkey);
                }

                if (memoryStream == null || memoryStream.Length == 0)
                {
                    string titulo, Msj, tipo2;
                    tipo2 = "error";
                    Msj = "Se genero error al intentar descargar el archivo, comunícate con el área de sistemas para ofrecerte una Solución";
                    titulo = "Notificaciones T|SYS|";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo2 + "');", true);
                    return;
                }
                else
                {
                    archivo = Cte + "- Pago Folio " + Pago + " - Factura " + Folio;
                    archivo = archivo + tipo;
                    HttpContext.Current.Response.ContentType = "application/pdf";
                    HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + archivo + "\"");
                    HttpContext.Current.Response.AppendHeader("Content-Length", memoryStream.Length.ToString());
                    HttpContext.Current.Response.BinaryWrite(memoryStream.ToArray());
                    HttpContext.Current.Response.End();
                }



            }

            if (e.CommandName == "Documento_3")
            {
                string tipo = ".xml";
                if (e.CommandName == "Documento_3")
                {
                    memoryStream = databaseFileRead("InvoiceKey", row.Cells[4].Text, "FileBinary", tipo, vkey);
                }

                if (memoryStream == null || memoryStream.Length == 0)
                {
                    string titulo, Msj, tipo2;
                    tipo2 = "error";
                    Msj = "Se genero error al intentar descargar el archivo, comunícate con el área de sistemas para ofrecerte una Solución";
                    titulo = "Notificaciones T|SYS|";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo2 + "');", true);
                    return;
                }
                else
                {
                    archivo = Cte + "- Pago Folio " + Pago + " - Factura " + Folio;
                    archivo = archivo + tipo;
                    HttpContext.Current.Response.ContentType = "text/xml";
                    HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + archivo + "\"");
                    HttpContext.Current.Response.AppendHeader("Content-Length", memoryStream.Length.ToString());
                    HttpContext.Current.Response.BinaryWrite(memoryStream.ToArray());
                    HttpContext.Current.Response.End();
                }
            }

        }
        catch (Exception ex)
        {
            string respuesta = ex.Message;
            int pLogKey = 1;
            int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
            LogError(pLogKey, pUserKey, "Aprobacion de Documentos:GridView1_RowCommand", ex.Message, pCompanyID);
        }
    }

    private MemoryStream databaseFileRead(string consulta, string InvoiceKey, string columna, string Tipo, int Vkey)
    {
        try
        {
            string sql = "";
            MemoryStream memoryStream = new MemoryStream();
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            if (consulta == "InvoiceKey")
                sql = @"SELECT " + columna + " From PaymentFile a INNER JOIN Payment b ON a.PaymentKey = b.PaymentKey Where b.Folio = @varID And a.FileType = @Type  AND b.VendorKey = @VKey";
            else
                sql = @"SELECT " + columna + " From PaymentFile a INNER JOIN Payment b ON a.PaymentKey = b.PaymentKey Where b.Folio = @varID And a.FileType = @Type  AND b.VendorKey = @VKey";


            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.Parameters.AddWithValue("@varID", InvoiceKey);
                sqlQuery.Parameters.AddWithValue("@Type", Tipo);
                sqlQuery.Parameters.AddWithValue("@VKey", Vkey);
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        sqlQueryResult.Read();
                        var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                        sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                        //using (var fs = new MemoryStream(memoryStream, FileMode.Create, FileAccess.Write)) {
                        memoryStream.Write(blob, 0, blob.Length);
                        //sqlQueryResult.GetValue(0);
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

    protected void ChkFechas_CheckedChanged(object sender, EventArgs e)
    {
        if (ChkFechas.Checked == false)
        {
            F1.ReadOnly = true;
            F2.ReadOnly = true;
        }
        else
        {
            F1.ReadOnly = false;
            F2.ReadOnly = false;

        }
    }

    protected void Limpia(object sender, EventArgs e)
    {
        try
        {
            SelProv.Items.Clear();
            CargaProv();
            List.SelectedValue = "1";
            //SelProv.SelectedValue = "1";
            Folio.Text = "";
            Factura.Text = "";
            F1.Text = "";
            F2.Text = "";
            Monto.Text = "";
            ChkFechas.Checked = false;
            BindGridView();
        }
        catch (Exception ex) 
        {
        
        }
    }

    protected void List_SelectedIndexChanged(object sender, EventArgs e)
    {
        //CargarCombos();
        BindGridView();
    }

}