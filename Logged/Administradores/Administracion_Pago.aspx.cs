//PORTAL DE PROVEDORES T|SYS|
//13 DE ENERO, 2020
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA ADMINISTRACIÓN DE COMPLEMENTOS DE PAGOS CARGADOR POR PROVEEDOR

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
        try
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
                    if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
                    {
                        ChkFechas.Checked = true;
                        ChkFechas.Checked = false;
                        List.SelectedValue = "1";
                        CargarCombos();
                        BindGridView();
                    }
                    else
                    {
                        HttpContext.Current.Session.RemoveAll();
                        Context.GetOwinContext().Authentication.SignOut();
                        Response.Redirect("~/Account/Login.aspx");
                    }
                }
            }
            else
            {

            }
        }
        catch (Exception ex)
        {

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
            }
        }
        catch (Exception ex)
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
        Cadena = "Select c.PaymentKey,a.Folio,c.Serie,c.Folio,CONVERT(varchar, convert(money, c.Total), 1) As Total,e.VendName,CONVERT( VARCHAR , c.CreateDate , 103 ) as CreateDate,CONVERT( VARCHAR , c.PaymentDate , 103 ) as PaymentDate,g.Descripcion from Invoice a INNER JOIN PaymentAppl b ON b.ApplInvoiceKey = a.InvoiceKey INNER JOIN Payment c ON b.PaymentKey = c.PaymentKey INNER JOIN Company d ON c.CompanyID = d.CompanyID INNER JOIN Vendors e ON c.VendorKey = e.VendorKey INNER JOIN Users f ON e.UserKey = f.UserKey INNER JOIN StatusDocs g ON c.Status = g.Status Where g.Descripcion = '";

        Cadena = Cadena + List.SelectedItem.ToString() + "'";
        string Company = HttpContext.Current.Session["IDCompany"].ToString();
        Cadena = Cadena + " AND d.CompanyID  = '" + Company + "' ";


        if (Proveedores.Items.Count >= 1)
        {
            if (Proveedores.SelectedItem.ToString() != "Todos")
            {
                Cadena = Cadena + " AND e.VendName = '" + Proveedores.SelectedItem.ToString() + "' ";
            }

            //if (SelProv.SelectedItem.ToString() != "Todas")
            //{
            //    //Cadena = Cadena + " AND b.CompanyName = '" + SelProv.SelectedItem.ToString() + "' ";
            //    string Company = HttpContext.Current.Session["IDCompany"].ToString();
            //    Cadena = Cadena + " AND b.CompanyName = '" + Company + "' ";
            //}
        }

        if (Folio.Text != "")
        {
            Cadena = Cadena + " AND c.Folio LIKE '%" + Folio.Text + "%' ";
        }

        if (Factura.Text != "")
        {
            Cadena = Cadena + " AND a.Folio LIKE '%" + Factura.Text + "%' ";
        }

        if (Monto.Text != "")
        {
            string Total = Monto.Text.Replace("$", "");
            Cadena = Cadena + " AND c.Total LIKE '%" + Total + "%' ";
        }

        if (ChkFechas.Checked == true)
        {
            if (LFechas.SelectedValue == "Factura")
            {
                if (F1.Text != "")
                {
                    Cadena = Cadena + " AND c.CreateDate >= '" + F1.Text + "' ";
                }
                if (F2.Text != "")
                {
                    Cadena = Cadena + " AND c.CreateDate <= '" + F2.Text + " 23:59:59' ";
                }
            }
            if (LFechas.SelectedValue == "Pago")
            {
                if (F1.Text != "")
                {
                    Cadena = Cadena + " AND c.PaymentDate >= '" + F1.Text + "' ";
                }
                if (F2.Text != "")
                {
                    Cadena = Cadena + " AND c.PaymentDate <= '" + F2.Text + " 23:59:59' ";
                }
            }


        }
        return Cadena;

    }

    private MemoryStream databaseFileRead(string consulta, string InvoiceKey, string columna, string tipo)
    {
        try
        {
            string sql = "";
            MemoryStream memoryStream = new MemoryStream();
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            if (consulta == "InvoiceKey")
                sql = @"SELECT " + columna + " FROM PaymentFile WHERE PaymentKey = @varID AND FileType = @Typo";
            else
                sql = @"SELECT " + columna + " FROM PaymentFile WHERE PaymentKey = @varID AND FileType = @Typo";


            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.Parameters.AddWithValue("@varID", InvoiceKey);
                sqlQuery.Parameters.AddWithValue("@Typo", tipo);
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
            //RutinaError(ex);
            return null;
        }
    }

    private bool StatusPago(String InvoiceKey)
    {
        try
        {

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            string Status = "2";
            sqlConnection1.Open();
            string UsK = HttpContext.Current.Session["UserKey"].ToString();


            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                if (Status == "1")
                    sqlQuery.CommandText = "UPDATE Payment SET Status=" + Status + " WHERE PaymentKey =" + InvoiceKey;
                else
                    sqlQuery.CommandText = "UPDATE Payment SET Status=" + Status + " ,AprovUserKey=" + UsK + ",AprovDate='" + DateTime.Now.ToString("MM/dd/yyyy") + "' WHERE PaymentKey = " + InvoiceKey;

                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
            }

            return true;
        }
        catch (Exception ex)
        {
            //RutinaError(ex);
            //LogError(pLogKey, pUserKey, "Carga-Factura:Page_Load", ex.Message, pCompanyID);
            return false;
        }
    }

    private void RevisaSaldo(String InvoiceKey)
    {
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            int User = Convert.ToInt16(HttpContext.Current.Session["UserKey"]);
            string sSQL;

            sSQL = "UpdatePayment";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@UserKey", User));
            parsT.Add(new SqlParameter("@FacKey", InvoiceKey));
            parsT.Add(new SqlParameter("@Opcion", 2));

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
                sqlConnection1.Close();

            }
        }
        catch (Exception ex)
        {
            throw new MulticonsultingException(ex.Message);
        }
    }

    protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
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
                    memoryStream = databaseFileRead("InvoiceKey", row.Cells[0].Text, "FileBinary", ".XML");
                    archivo += " Complemento de Pago - " + HttpUtility.HtmlDecode(row.Cells[3].Text);
                    archivo += ".xml";
                    HttpContext.Current.Response.ContentType = "text/xml";
                }
                if (e.CommandName == "Documento_2")
                {
                    memoryStream = databaseFileRead("InvoiceKey", row.Cells[0].Text, "FileBinary", ".pdf");
                    archivo += " Complemento de Pago - " + HttpUtility.HtmlDecode(row.Cells[3].Text);
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
                StatusPago(folio);
                RevisaSaldo(folio);
                BindGridView();
                lblMsj1.Text = "Complemento de Pago Aprobado.";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(AL2);", true);

            }
            else if (e.CommandName == "Cancelar")
            {
                int Vrs = Convert.ToInt32(folio);
                if ((CancelarD(Vrs, 0, 1, 2) == 0))
                {
                    int Fila = row.RowIndex + 1;
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "Pregunta('" + folio + "','" + llave + "','" + Fila + "');", true);
                }
                else
                {
                    Response.Redirect("Administracion_Pago.aspx");
                }
            }

        }
        catch (Exception ex)
        {

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

    private void BindGridView()
    {
        DatosV.Visible = false;
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            string SpFiltros;
            SpFiltros = Filtros();
            DataSet dsProveedores = new DataSet();
            SqlCommand cmd = new SqlCommand(SpFiltros, conn);
            if (conn.State == ConnectionState.Open){conn.Close();}
            conn.Open();
            string Errores = string.Empty;
            SqlDataReader rdr = cmd.ExecuteReader();

            dt.Columns.Add("PKey");
            dt.Columns.Add("Factura");
            dt.Columns.Add("Serie");
            dt.Columns.Add("Folio");
            dt.Columns.Add("Total");
            dt.Columns.Add("Proveedor");
            //dt.Columns.Add("Company");
            dt.Columns.Add("FechaR");
            dt.Columns.Add("FechaP");
            dt.Columns.Add("Status");

            while (rdr.Read())
            {
                DataRow row = dt.NewRow();

                row["PKey"] = Convert.ToString(rdr.GetValue(0));
                row["Factura"] = Convert.ToString(rdr.GetValue(1));
                row["Serie"] = Convert.ToString(rdr.GetValue(2));
                row["Folio"] = Convert.ToString(rdr.GetValue(3));
                row["Total"] = "$" + " " + Convert.ToString(rdr.GetValue(4));
                row["Proveedor"] = Convert.ToString(rdr.GetValue(5));
                //row["Company"] = Convert.ToString(rdr.GetValue(5));
                row["FechaR"] = Convert.ToString(rdr.GetValue(6));
                row["FechaP"] = Convert.ToString(rdr.GetValue(7));
                row["Status"] = Convert.ToString(rdr.GetValue(8));
                dt.Rows.Add(row);
            }
        }

        //GridView1.DataSource = dt;
        //GridView1.DataBind();


        gvFacturas.DataSource = dt;
        gvFacturas.DataBind();


        if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas" || List.SelectedItem.ToString() != "Pendiente")
        {
            gvFacturas.Columns[11].Visible = false;
            gvFacturas.Columns[12].Visible = false;
        }
        else
        {
            gvFacturas.Columns[11].Visible = true;
            gvFacturas.Columns[12].Visible = true;

        }

        //gvFacturas.Columns[0].Visible = false;

        if (dt.Rows.Count == 0) { DatosV.Visible = true;}



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

    protected int ObtenerVK(string razon,string company)
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

    private MemoryStream databaseFileRead(string consulta, string InvoiceKey, string columna, string Tipo,int Vkey)
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
            Proveedores.Items.Clear();
            //SelProv.Items.Clear();
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
                    //SelProv.Items.Insert(0, Lin);
                    //SelProv.Items.Insert(0, "Todas");
                }
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelPayF", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = 1 });

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
                    Lin.Value = (rdr["VendName"].ToString());  //  VendName
                    Proveedores.Items.Insert(0, Lin);                    
                }

                Proveedores.Items.Insert(0, "Todos");
            }
        }
        catch
        {

        }

    }

    protected void Limpia(object sender,EventArgs e)
    {
        //SelProv.Text = "Todas";
        Proveedores.Text = "Todos";
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