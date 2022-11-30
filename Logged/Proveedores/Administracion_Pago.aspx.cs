//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA PARA PROVEEDOR DE CONSULTA DE PAGOS CARGADOS AL PORTAL

//REFERENCIAS UTILIZADAS
using System;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Web.UI;
using System.Collections.Generic;

public partial class Logged_Administrar : System.Web.UI.Page
{

    string eventName = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        if (!IsPostBack)
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
                    Global.Docs();
                    if ((HttpContext.Current.Session["Docs"].ToString() == "1"))
                    {
                        //Page.MasterPageFile = "MenuP.master";
                        Global.RevDocs();
                        if ((HttpContext.Current.Session["Docs"].ToString() == "1"))
                        {
                            //Page.MasterPageFile = "MenuP.master";
                            Response.Redirect("~/Logged/Proveedores/Default.aspx",false);

                        }
                        //Response.Redirect("~/Account/Default.aspx");
                    }
                    else
                    {
                        List.SelectedValue = "1";
                        CargarCombos();
                        BindGridView();
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

            //Global.Docs();
            //if ((HttpContext.Current.Session["Docs"].ToString() == "1"))
            //{
            //    //Page.MasterPageFile = "MenuP.master";
            //    //Response.Redirect("~/Logged/Proveedores/Default.aspx",false);

            //}
            //else if ((HttpContext.Current.Session["Status"].ToString() == "Activo"))
            //{
            //    Page.MasterPageFile = "MenuPreP.master";
            //}
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

    private string Filtros()
    {
        string Cadena = string.Empty;
        string Vendor = HttpContext.Current.Session["UserKey"].ToString();
        //int Vedn = int.Parse(Vendor.ToString());

        Cadena = "Select a.Folio,c.Serie,c.Folio,convert(decimal(12, 2), c.Total),e.VendName,d.CompanyName,c.CreateDate,c.PaymentDate,g.Descripcion from Invoice a INNER JOIN PaymentAppl b ON b.ApplInvoiceKey = a.InvoiceKey INNER JOIN Payment c ON b.PaymentKey = c.PaymentKey INNER JOIN Company d ON c.CompanyID = d.CompanyID INNER JOIN Vendors e ON c.VendorKey = e.VendorKey INNER JOIN Users f ON e.UserKey = f.UserKey INNER JOIN StatusDocs g ON c.Status = g.Status Where g.Descripcion = '";

        Cadena = Cadena + List.SelectedItem.ToString() + "'";

        Cadena = Cadena + " AND f.UserKey = " + Vendor + " ";

        //if (Proveedores.SelectedItem.ToString() != "Todos")
        //{
        //    Cadena = " AND c.VendName = '" + Proveedores.SelectedItem.ToString() + "' ";
        //}
        if (SelProv.Items.Count >= 1)
        {
            if (SelProv.SelectedItem.ToString() != "Todas")
            {
                Cadena = Cadena + " AND b.CompanyName = '" + SelProv.SelectedItem.ToString() + "' ";
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

        GridView1.DataSource = dt;
        GridView1.DataBind();

        if (dt.Rows.Count == 0) { DatosV.Visible = true; }
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
            GridViewRow row = GridView1.Rows[index];
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

    private MemoryStream databaseFileRead(string consulta, string InvoiceKey, string columna,string Tipo,int Vkey)
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
                        memoryStream.Write(blob, 0, blob.Length);
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
            //Label4.Text = err;
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

    protected void CargarCombos()
    {
        try
        {
            //Proveedores.Items.Clear();
            SelProv.Items.Clear();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelPayF", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = 2 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Status", Value = List.SelectedItem.ToString() });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = "" });

                if (conn.State == ConnectionState.Open)
                { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ListItem Lin = new ListItem();
                    Lin.Value = (rdr["CompanyName"].ToString());  //  VendName
                    SelProv.Items.Insert(0, Lin);
                    SelProv.Items.Insert(0, "Todas");
                }
            }
        }
        catch
        {

        }

    }

    protected void Limpia(object sender,EventArgs e)
    {
        SelProv.Text = "Todas";
        List.SelectedValue = "1";
        Folio.Text = "";
        Factura.Text = "";
        F1.Text = "";
        F2.Text = "";
        Monto.Text = "";
        ChkFechas.Checked = false;
        BindGridView();
    }

    protected void List_SelectedIndexChanged(object sender, EventArgs e)
    {
        CargarCombos();
        BindGridView();
    }
}