//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA APROBACION DE DOCUMENTOS CARGADOS POR EL PROVEEDOR

//REFERENCIAS UTILIZADAS
using System;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Logged_Administrar : System.Web.UI.Page
{
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

                        if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin" || HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Validador")
                        {
                           DropDownList1.SelectedValue = "1";
                           User_Company(SelComp);
                           CargaProv();
                        }
                        else
                        {
                            HttpContext.Current.Session.RemoveAll();
                            Context.GetOwinContext().Authentication.SignOut();
                            Response.Redirect("~/Account/Login.aspx");
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
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

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

            if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Consultas" || HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
            {
                HttpContext.Current.Session.RemoveAll();
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
        }
        catch(Exception ex)
        {
            RutinaError(ex);
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

    protected void BindGridInvoices(string Id, string EMail)
    {
        string sSQL = "";

        SqlConnection sqlConnection1 = new SqlConnection();
        sqlConnection1 = SqlConnectionDB("PortalConnection");
        sqlConnection1.Open();

        using (SqlCommand Cmd = new SqlCommand(sSQL, sqlConnection1))
        {
            try
            {
                sSQL = "spRevDoc";

                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = sSQL;

                List<SqlParameter> parsT = new List<SqlParameter>();
                parsT.Add(new SqlParameter("@ID", Id));
                parsT.Add(new SqlParameter("@Email", EMail));
                parsT.Add(new SqlParameter("@Company", SelComp.SelectedItem.ToString()));

                SqlParameter[] sqlParameter = parsT.ToArray();
                foreach (SqlParameter par in sqlParameter)
                {
                    Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);
                }

                SqlDataAdapter Data = new SqlDataAdapter(Cmd);
                DataTable Table = new DataTable();

                Data.Fill(Table);
                gvFacturas.DataSource = Table;
                gvFacturas.DataBind();


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

    protected void Comprueba(string Id,string EMail)
    {
        try
        {
            if (IDSAGE.Text != "" && Mail.Text != "")
            {
                //Global.VarSesion(User.Identity.Name.ToString(), SelProv.SelectedItem.ToString());

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spRevDoc", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@ID", Value = Id});

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@Email", Value = EMail});

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@Company", Value = SelComp.SelectedItem.ToString() });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    string Errores = string.Empty;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();

                    dt.Columns.Add("Nombre");
                    dt.Columns.Add("NombreFile");
                    dt.Columns.Add("Fecha");
                    dt.Columns.Add("Fecha1");
                    dt.Columns.Add("Status");
                    dt.Columns.Add("Coment");

                    while (rdr.Read())
                    {

                        DataRow row = dt.NewRow();
                        row["Nombre"] = HttpUtility.HtmlDecode(rdr["Nombre"].ToString());
                        row["NombreFile"] = HttpUtility.HtmlDecode(rdr["NombreFile"].ToString());
                        row["Fecha"] = rdr["Fecha"].ToString();
                        row["Fecha1"] = rdr["Fecha1"].ToString();
                        row["Status"] = rdr["status"].ToString();
                        row["Coment"] = HttpUtility.HtmlDecode(rdr["Comentarios"].ToString());
                        dt.Rows.Add(row);
                    }

                    if (dt.Rows.Count >= 5)
                    {
                        gvFacturas.DataSource = dt;
                        gvFacturas.DataBind();
                    }
                    else
                    {
                        gvFacturas.DataSource = dt;
                        gvFacturas.DataBind();
                    }

                }
            }
        }
        catch(Exception ex)
        {
            RutinaError(ex);
        }

    }

    protected void User_Company(DropDownList Caja)
    {
        try
        {
            string sql;
            string Cuenta;
            Caja.Items.Clear();
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            sqlConnection1.Open();

            sql = @"select top 1 CompanyName from Company Where CompanyID ='" + Company + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            ListItem Linea = new ListItem();
            Linea.Value = (Cuenta);
            Caja.Items.Insert(0, Linea);
            Caja.Visible = true;


            if (Caja.Items.Count == 0)
            {
                ListItem Linea1 = new ListItem();
                Linea1.Value = ("N/D");
                Caja.Items.Insert(0, Linea1);
                Caja.Visible = true;
                HttpContext.Current.Session["IDComTran"] = "";
            }


        }
        catch (Exception ex)
        {

        }
    }

    protected void CargaProv()
    {
        try
        {
            int Cuenta = 0;
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectUserDoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                string Company = HttpContext.Current.Session["IDCompany"].ToString();
                int Opc = 1;
                if (DropDownList1.SelectedItem.ToString() == "Aprobado") { Opc = 3;}
                

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
                    if (rdr["Nombre"].ToString().Length  >1)
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Nombre"].ToString());
                        SelProv.Items.Insert(0, Linea);
                        Cuenta = Cuenta + 1;
                    }
                    
                }
                
                conn.Close();
            }

            if (Cuenta == 0)
            {
                gvFacturas.DataSource = dt;
                gvFacturas.DataBind();
                Button1.Visible = false;
                Button2.Visible = false;
                Button3.Visible = false;
                Comentarios.Visible = false;
                lc.Visible = false;
                DatosV.Visible = true;
                Mail.Text = "";
                Comentarios.Text = "";
                IDSAGE.Text = "";
            }
            else
            {
                DatosV.Visible = false;
                Comentarios.Text = "";
                lc.Visible = true;
                Button1.Visible = true;
                Button2.Visible = true;
                Button3.Visible = true;
                Comentarios.Visible = true;
                int bloqueo = 0;
                Datos(SelProv.SelectedItem.ToString());

                foreach (GridViewRow gvr in gvFacturas.Rows)
                {
                    CheckBox Checka = (CheckBox)gvr.Cells[1].FindControl("Check");
                    string  status = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());

                    LinkButton Cancela = (LinkButton)gvr.Cells[6].FindControl("Documento_D");
                    Cancela.Visible = false;

                    if (status == "Aprobado")
                    {
                        Checka.Checked = true;
                        Checka.Enabled = false;
                        Cancela.Visible = true;
                    }

                    if (status == "Expirado")
                    {
                        Checka.Checked = false;
                        Checka.Enabled = false;
                    }

                    if (status == "Por expirar")
                    {
                        Checka.Checked = true;
                        Checka.Enabled = false;
                    }

                    if (status == "Rechazado")
                    {
                        Checka.Checked = false;
                        Checka.Enabled = false;
                    }

                    if (Checka.Enabled == true)
                    {
                        bloqueo = bloqueo + 1;
                    }

                }

                if (bloqueo == 0)
                {
                    Button1.Visible = false;
                    Button2.Visible = false;
                    Button3.Visible = false;
                    Comentarios.Visible = false;
                    lc.Visible = false;
                }
                else
                {
                    Button1.Visible = true;
                    Button2.Visible = true;
                    Button3.Visible = true;
                    lc.Visible = true;
                    Comentarios.Visible = true;
                }


            } 
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

    protected void Datos(string ID)
    {

        try
        {
            int Contador = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectUserDoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                string Company = HttpContext.Current.Session["IDCompany"].ToString();
                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = 2 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@User", Value = ID });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Company });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ListItem Linea = new ListItem();
                    Linea.Value = (rdr["Fecha"].ToString());
                    IDSAGE.Text = rdr["ID"].ToString();
                    Mail.Text = rdr["Email"].ToString();
                    Contador = Contador + 1;
                }

                if (Contador >= 1)
                {
                 BindGridInvoices(IDSAGE.Text, Mail.Text);
                }
            }
        }
        catch(Exception ex)
        {
            RutinaError(ex);
        }
 }

    protected void SelProv_SelectedIndexChanged1(object sender, EventArgs e)
    {
        try
        {
            //if (IDSAGE.Text != "" && Mail.Text != "")
            //{                DatosV.Visible = false;
                DatosV.Visible = false;
                string Prov = SelProv.SelectedItem.ToString();
                Datos(SelProv.SelectedItem.ToString());
                int bloqueo = 0;
                foreach (GridViewRow gvr in gvFacturas.Rows)
                {
                    CheckBox Checka = (CheckBox)gvr.Cells[1].FindControl("Check");
                    string status = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    LinkButton Cancela = (LinkButton)gvr.Cells[6].FindControl("Documento_D");

                Cancela.Visible = false;

                if (status == "Aprobado")
                {
                    Checka.Checked = true;
                    Checka.Enabled = false;
                    Cancela.Visible = true;
                }

                if (status == "Expirado")
                {
                    Checka.Checked = false;
                    Checka.Enabled = false;
                }

                if (status == "Por expirar")
                {
                    Checka.Checked = true;
                    Checka.Enabled = false;
                }

                if (status == "Rechazado")
                    {
                        Checka.Checked = false;
                        Checka.Enabled = false;
                    }


                    if (Checka.Enabled == true)
                    {
                    bloqueo = bloqueo + 1;
                    }

                }

            if (bloqueo == 0)
            {
                Button1.Visible = false;
                Button2.Visible = false;
                Button3.Visible = false;
                lc.Visible = false;
                Comentarios.Visible = false;
            }
            else
            {
                Button1.Visible = true;
                Button2.Visible = true;
                Button3.Visible = true;
                lc.Visible = true;
                Comentarios.Visible = true;
            }

            //Datos2();
            //}
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

    protected void Aceptar(object sender, EventArgs e)
    {
        try
        {
            int cont = 0;
            int i =0;
            int x = 0;
            string docs = string.Empty;
            string Prov = SelProv.SelectedItem.ToString();
            string Com = SelComp.SelectedItem.ToString();

            foreach (GridViewRow gvr in gvFacturas.Rows)
            {
                CheckBox Checka = (CheckBox)gvFacturas.Rows[i].Cells[1].FindControl("Check");
                docs = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());

                if (Checka.Checked)
                {
                    if (Checka.Enabled == true)
                    {
                        if (Alta(2, docs, Prov, Com) == false)
                        {
                            cont = cont + 1;
                        }
                    }  
                }
                else
                {
                    if (Alta(3, docs, Prov, Com) == false)
                    {
                        cont = cont + 1;
                    }
                    x = x + 1;
                }
                i = i + 1;
            }

            if (cont >= 1)
            {
                string titulo, Msj, tipo;
                tipo = "error";
                Msj = "Se Generaron Errores al Guardar Cambios en Base de Datos";
                titulo = "Notificaciones T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                CargaProv();
            }
            else
            {
                Email("Aprobado");
                string titulo, Msj, tipo;
                tipo = "success";
                Msj = "Notificacion de Proveedor Exitosa";
                titulo = "Notificaciones T|SYS|";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                CargaProv();
            }

        }
        catch(Exception ex)
        {
            RutinaError(ex);
        }
    }

    protected void Del()
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spUpdateDocD", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter(){ ParameterName = "@ID", Value = IDSAGE.Text });
                cmd.Parameters.Add(new SqlParameter(){ ParameterName = "@Name", Value = SelProv.SelectedItem.ToString()});
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Compan", Value = SelComp.SelectedItem.ToString()});

                if (conn.State == ConnectionState.Open) {conn.Close();}

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                conn.Close();
            }
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }

    }

    protected bool Alta(int status,string Doc,string Prov,string Com)
    {
        bool alt = false;
        try
        {
            string Key = HttpContext.Current.Session["UserKey"].ToString();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spUpdateDoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@ID",
                    Value = IDSAGE.Text
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Name",
                    Value = Prov
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@UserKey",
                    Value = Key
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Status",
                    Value = status
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Doc",
                    Value = Doc
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Compan",
                    Value = Com
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                alt = true;
                conn.Close();
            }
        }
        catch (Exception ex)
        {
            RutinaError(ex);
            alt = false;
        }
        return alt;
    }

    public static string correo3(string VendID)
    {
        string correo = string.Empty;
        try
        {
            string SQL = "  Select ISNULL(f.UserID,(Select UserID from vendors a LEFT JOIN Users f on a.UserKey = f.UserKey Where VendorID = @VendID)) ";
            SQL += " As Correo from vendors a LEFT JOIN Users f on a.superior = f.UserKey Where VendorID = @VendID";
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand(SQL, sqlConnection1))
            {
                sqlQuery.Parameters.AddWithValue("@VendID", VendID);
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        while (sqlQueryResult.Read())
                        {
                            correo = Convert.ToString(sqlQueryResult.GetValue(0));
                        }
                    }
            }

            sqlConnection1.Close();

        }
        catch (Exception ec)
        {

        }
        return correo;

    }

    private bool Email(string PassNew)
    {
        bool Resut = false;
        try
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionOk.html")))
            {
                body = reader.ReadToEnd();
                string OK = "='https://www.multiconsulting.com.mx/Ok.png'";
                string NO = "='https://www.multiconsulting.com.mx/X.png'";
                string Coment = Comentarios.Text;

                CheckBox Checka1 = (CheckBox)gvFacturas.Rows[0].Cells[1].FindControl("Check");
                CheckBox Checka2 = (CheckBox)gvFacturas.Rows[1].Cells[1].FindControl("Check");
                CheckBox Checka3 = (CheckBox)gvFacturas.Rows[2].Cells[1].FindControl("Check");
                CheckBox Checka4 = (CheckBox)gvFacturas.Rows[3].Cells[1].FindControl("Check");
                CheckBox Checka5 = (CheckBox)gvFacturas.Rows[4].Cells[1].FindControl("Check");
                CheckBox Checka6 = (CheckBox)gvFacturas.Rows[5].Cells[1].FindControl("Check");
                CheckBox Checka7 = (CheckBox)gvFacturas.Rows[6].Cells[1].FindControl("Check");
                CheckBox Checka8 = (CheckBox)gvFacturas.Rows[7].Cells[1].FindControl("Check");
                CheckBox Checka9 = (CheckBox)gvFacturas.Rows[8].Cells[1].FindControl("Check");

                if (Checka1.Checked == true) { body = body.Replace("{Status1}", "Aprobado"); body = body.Replace("{PassTemp1}", OK); } else { body = body.Replace("{Status1}", "Rechazado"); body = body.Replace("{PassTemp1}", NO); }
                if (Checka2.Checked == true) { body = body.Replace("{Status2}", "Aprobado"); body = body.Replace("{PassTemp2}", OK); } else { body = body.Replace("{Status2}", "Rechazado"); body = body.Replace("{PassTemp2}", NO); }
                if (Checka3.Checked == true) { body = body.Replace("{Status3}", "Aprobado"); body = body.Replace("{PassTemp3}", OK); } else { body = body.Replace("{Status3}", "Rechazado"); body = body.Replace("{PassTemp3}", NO); }
                if (Checka4.Checked == true) { body = body.Replace("{Status4}", "Aprobado"); body = body.Replace("{PassTemp4}", OK); } else { body = body.Replace("{Status4}", "Rechazado"); body = body.Replace("{PassTemp4}", NO); }
                if (Checka5.Checked == true) { body = body.Replace("{Status5}", "Aprobado"); body = body.Replace("{PassTemp5}", OK); } else { body = body.Replace("{Status5}", "Rechazado"); body = body.Replace("{PassTemp5}", NO); }
                if (Checka6.Checked == true) { body = body.Replace("{Status6}", "Aprobado"); body = body.Replace("{PassTemp6}", OK); } else { body = body.Replace("{Status6}", "Rechazado"); body = body.Replace("{PassTemp6}", NO); }
                if (Checka7.Checked == true) { body = body.Replace("{Status7}", "Aprobado"); body = body.Replace("{PassTemp7}", OK); } else { body = body.Replace("{Status7}", "Rechazado"); body = body.Replace("{PassTemp7}", NO); }
                if (Checka8.Checked == true) { body = body.Replace("{Status8}", "Aprobado"); body = body.Replace("{PassTemp8}", OK); } else { body = body.Replace("{Status8}", "Rechazado"); body = body.Replace("{PassTemp8}", NO); }
                if (Checka9.Checked == true) { body = body.Replace("{Status9}", "Aprobado"); body = body.Replace("{PassTemp9}", OK); } else { body = body.Replace("{Status9}", "Rechazado"); body = body.Replace("{PassTemp9}", NO); }
                
                body = body.Replace("{Comentarios}", Coment);

            }

            string corre = Mail.Text;
            corre = correo3(SelProv.SelectedItem.ToString());
            if (corre == "")
            {
                corre = Mail.Text;
            }

            Resut = Global.EmailGlobal(corre, body, "CARGA DE DOCUMENTOS PORTAL TSYS");
            
        }
        catch (Exception ex)
        {
            RutinaError(ex);
            Resut = false;
        }
        return Resut;
    }

    protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
    }

    protected void OkAll(object sender, EventArgs e)
    {
        try
        {

        foreach (GridViewRow row in gvFacturas.Rows)
        {
            CheckBox check = row.FindControl("Check") as CheckBox;

            if (check.Checked != true)
            {
                check.Checked = true;
            }
        }

        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }

    }

    protected void NoAll(object sender, EventArgs e)
    {
        try
        {
            foreach (GridViewRow row in gvFacturas.Rows)
            {
                CheckBox check = row.FindControl("Check") as CheckBox;
                if (check.Enabled == true)
                {
                    if (check.Checked == true)
                    {
                        check.Checked = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
        //Descargar();
    }

    protected void Check(int index)
    {
        try
        {
            CheckBox Checka = (CheckBox)gvFacturas.Rows[index].Cells[1].FindControl("Check");
            DataTable dt = new DataTable();

            dt.Columns.Add("Status");
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Archivo");
            dt.Columns.Add("Fecha");
            dt.Columns.Add("Fecha1");

            foreach (GridViewRow gvr in gvFacturas.Rows)
            {
                if (Checka.Checked == true)
                {
                    DataRow dr = dt.NewRow();
                    dr["Status"] = "Aprobado";
                    dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    dr["Archivo"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    dr["Fecha"] = gvr.Cells[4].Text.ToString();
                    dr["Fecha"] = gvr.Cells[5].Text.ToString();
                    dt.Rows.Add(dr);
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    dr["Status"] = "No Aprobado";
                    dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    dr["Archivo"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    dr["Fecha"] = gvr.Cells[4].Text.ToString();
                    dr["Fecha"] = gvr.Cells[5].Text.ToString();
                    dt.Rows.Add(dr);

                }
            }

            gvFacturas.DataSource = dt;
            gvFacturas.DataBind();
        }
        catch (Exception ex)
        {
            RutinaError(ex);
        }
    }

    protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {

            MemoryStream memoryStream = new MemoryStream();

            int index = Convert.ToInt32(e.CommandArgument);
            string archivo,ID,Razon,Company,Email2,IDSAG;

            GridViewRow row = gvFacturas.Rows[index];
            ID = HttpUtility.HtmlDecode(row.Cells[3].Text.ToString()); // Archivo
            Razon = HttpUtility.HtmlDecode(SelProv.SelectedItem.ToString()); //Razon
            Company = HttpUtility.HtmlDecode(SelComp.SelectedItem.ToString());
            Email2 = HttpUtility.HtmlDecode(Mail.Text);
            IDSAG = HttpUtility.HtmlDecode(IDSAGE.Text);

            archivo = ID + " - " + Razon;

            if (e.CommandName == "Documento_1")
            {
                if (e.CommandName == "Documento_1")
                {
                    memoryStream = databaseFileRead(IDSAG, Razon, ID, Email2, Company);
                }

                if (memoryStream == null || memoryStream.Length == 0)
                {
                    string titulo, Msj, tipo;
                    tipo = "error";
                    Msj = "Se genero error al cargar el documento, archivo corrupto, comunícate con el área de sistemas para ofrecerte una Solución";
                    titulo = "Notificaciones T|SYS|";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    return;
                }

                else
                {
                    archivo = archivo.Replace(".", "").Replace(",", "");
                    archivo = archivo + ".pdf";
                    HttpContext.Current.Response.ContentType = "application/pdf";
                    HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + archivo + "\"");
                    HttpContext.Current.Response.AppendHeader("Content-Length", memoryStream.Length.ToString());
                    HttpContext.Current.Response.BinaryWrite(memoryStream.ToArray());
                    HttpContext.Current.Response.End();
                }


            }

            else if (e.CommandName == "Documento_D")
            {
                string Provedor = HttpUtility.HtmlDecode(SelProv.SelectedItem.ToString());
                string ProvedID = HttpUtility.HtmlDecode(IDSAGE.Text.ToString());
                string Doc = HttpUtility.HtmlDecode(row.Cells[3].Text);
                string CompanyID = HttpContext.Current.Session["IDCompany"].ToString();

                int Dock = Obtenerllave_doc(Provedor, ProvedID, Doc, CompanyID);

                if (Dock <= 0)
                {
                    return;
                }

                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "Pregunta('" + Dock + "','" + Doc + "','" + 1 + "');", true);
            }

            else if (e.CommandName == "Check")
            {

            }

        }
        catch (Exception ex)
        {
            //string respuesta = ex.Message;
            //int pLogKey = 1;
            //int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            //string pCompanyID = Convert.ToString(HttpContext.Current.Session["IDCompany"].ToString());
            //LogError(pLogKey, pUserKey, "Aprobacion de Documentos:GridView1_RowCommand", ex.Message, pCompanyID);
            RutinaError(ex);
        }
    }

    protected int Obtenerllave_doc(string Nombre,string IdProv,string Doc, string Company)
    {
        int Llave = 0;
        try
        {

            string Ssql = "Select a.DocKey from Documents a inner join Vendors b on a.VendorKey = b.VendorKey Inner Join TypeFiles c on a.DocID = c.DocKey ";
            Ssql = Ssql + "Where b.VendName LIKE '%" + Nombre + "%' AND b.VendorID = '" + IdProv + "' AND b.CompanyID = '" + Company + "'AND c.Descripcion LIKE '%" + Doc + "%'";

            string Estado = string.Empty;

            SqlConnection sqlConnection2 = new SqlConnection();
            sqlConnection2 = SqlConnectionDB("PortalConnection");
            sqlConnection2.Open();
            using (SqlCommand Cmdd = new SqlCommand(Ssql, sqlConnection2))
            {
                Cmdd.CommandType = CommandType.Text;
                Cmdd.CommandText = Ssql;
                Llave = Convert.ToInt32(Cmdd.ExecuteScalar().ToString());
            }
            sqlConnection2.Close();

        }
        catch
        {

        }

        return Llave;
    }

    protected void chkview_CheckedChanged(object sender, EventArgs e)
    {
        IDSAGE.Text = "";
        //Define your code here.
    }


    private MemoryStream databaseFileRead(string ID, string Razon, string Doc,string Email,string Company)
    {
        try
        {
            MemoryStream memoryStream = new MemoryStream();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("DowFil", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter(){ParameterName = "@ID",Value = ID});
                cmd.Parameters.Add(new SqlParameter(){ParameterName = "@Name",Value = Razon});
                cmd.Parameters.Add(new SqlParameter(){ParameterName = "@Doc", Value = Doc });
                cmd.Parameters.Add(new SqlParameter(){ParameterName = "@Email", Value = Email });
                cmd.Parameters.Add(new SqlParameter(){ParameterName = "@compan", Value = Company });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var blob = new Byte[(rdr.GetBytes(0, 0, null, 0, int.MaxValue))];
                    rdr.GetBytes(0, 0, blob, 0, blob.Length);
                    memoryStream.Write(blob, 0, blob.Length);
                }
            }
            return memoryStream;
        }
        catch (Exception ex)
        {
            RutinaError(ex);
            return null;
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
            int LogKey2, Userk, VendK;
            string Company = string.Empty;
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            if (HttpContext.Current.Session["IDCompany"].ToString() == "") { Msj = Msj + "," + "Variable IDCompany null"; Company = "MGS"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey2 = 0; } else { LogKey2 = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Administracion_Doc.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey2, Userk, "Administracion_Doc.aspx.cs_" + nombreMetodo, Msj, Company);

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
            if (HttpContext.Current.Session["UserKey"].ToString() == "") { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
            if (HttpContext.Current.Session["IDCompany"].ToString() == "") { Msj = Msj + "," + "Variable IDCompany null"; Company = "MGS"; } else { Company = Convert.ToString(HttpContext.Current.Session["IDCompany"]); }
            if (HttpContext.Current.Session["VendKey"].ToString() == "") { Msj = Msj + "," + "Variable VendKey null"; VendK = 0; } else { VendK = Convert.ToUInt16(HttpContext.Current.Session["VendKey"]); }
            if (HttpContext.Current.Session["LogKey"].ToString() == "") { Msj = Msj + "," + "Variable LogKey null"; LogKey = 0; } else { LogKey = Convert.ToUInt16(HttpContext.Current.Session["LogKey"]); }
            Msj = Msj + ex.Message;
            string nombreMetodo = frame.GetMethod().Name.ToString();
            int linea = frame.GetFileLineNumber();
            Msj = Msj + " || Metodo : Administracion_Doc.aspx.cs_" + nombreMetodo + " Linea " + Convert.ToString(linea);
            LogError(LogKey, Userk, " Administracion_Doc.aspx.cs_" + nombreMetodo, Msj, Company);
            return null;
        }
    }

    //Todos los Catch
    protected void RutinaError(Exception ex)
    {
        string Msj = string.Empty;
        StackTrace st = new StackTrace(ex, true);
        StackFrame frame = st.GetFrame(st.FrameCount - 1);
        int LogKey, Userk, VendK;
        string Company = string.Empty;
        if (HttpContext.Current.Session["UserKey"]== null) { Msj = "Variable UserKey null"; Userk = 0; } else { Userk = Convert.ToUInt16(HttpContext.Current.Session["UserKey"]); }
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

    protected void DropDownList1_SelectedIndexChanged1(object sender, EventArgs e)
    {
        try
        {
            CargaProv();
            //Datos2();
        }
        catch (Exception ex)
        {

        }
     
    }

    protected void Datos2()
    {
        try
        {
            string sql;
            int Contador = 0;
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            int Rec = 0;
            string CuentK = string.Empty;
            string VK = SelProv.SelectedItem.ToString();
            string IDC = SelComp.SelectedItem.ToString();
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            sqlConnection1.Open();

            sql = @"Select top 1 userkey from Vendors Where VendName = '" + SelProv.SelectedItem.ToString() + "' And CompanyID ='" + Company + "'"; ;
            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                CuentK = sqlQuery.ExecuteScalar().ToString();
            }

            sql = @"Select Distinct a.VendorID AS ID,d.CompanyName AS Fecha,b.UserID as Email From Vendors a inner join Users b on a.UserKey = b.UserKey inner join Company d on d.CompanyID = a.CompanyID Where a.UserKey = '" + CuentK + "' AND d.CompanyName= '" + SelComp.SelectedItem.ToString() + "'";
            //sql = @"Select Distinct b.VendorID AS ID,e.CompanyName AS Fecha,c.UserID As Email From Documents a INNER JOIN Vendors b ON a.VendorKey = b.VendorKey INNER JOIN Users c ON b.UserKey = c.UserKey INNER JOIN UsersInCompany d ON c.UserKey = d.UserKey  INNER JOIN Company e ON e.CompanyID = d.CompanyID  Where b.VendName = '" + VK + "' AND e.CompanyName  LIKE '%" + IDC + "%'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;

                System.Data.SqlClient.SqlDataReader rdr = null;
                rdr = sqlQuery.ExecuteReader();
                while (rdr.Read())
                {
                    IDSAGE.Text = rdr["ID"].ToString();
                    Mail.Text = rdr["Email"].ToString();
                    Contador = Contador + 1;
                }

                if (Contador >= 1)
                {
                    BindGridInvoices(IDSAGE.Text, Mail.Text);
                    int ck = 0;

                    if (gvFacturas.Rows.Count>1)
                    {
                    foreach (GridViewRow gvr in gvFacturas.Rows)
                    {
                        CheckBox Checka = (CheckBox)gvr.Cells[1].FindControl("Check");
                        string status = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        LinkButton Cancela = (LinkButton)gvr.Cells[6].FindControl("Documento_D");
                        Cancela.Visible = false;

                            if (status == "Aprobado")
                            {
                                Checka.Checked = true;
                                Checka.Enabled = false;
                                Cancela.Visible = true;
                                ck = ck + 1;
                            }

                            if (status == "Expirado")
                            {
                                Checka.Checked = false;
                                Checka.Enabled = false;
                            }

                            if (status == "Por expirar")
                            {
                                Checka.Checked = true;
                                Checka.Enabled = false;
                            }

                            if (status == "Rechazado")
                        {
                            Checka.Checked = false;
                            Checka.Enabled = false;
                            ck = ck + 1;
                            Rec = 1;
                        }
                    }

                    if (ck==7)
                    {
                        if (Rec == 1) { DropDownList1.SelectedValue = "1"; }
                        else { DropDownList1.SelectedValue = "2"; }
                        Button1.Visible = false;
                        Button2.Visible = false;
                        Button3.Visible = false;
                        lc.Visible = false;
                        Comentarios.Visible = false;
                            DatosV.Visible = false;
                        }
                    else 
                    {
                        DropDownList1.SelectedValue = "1";
                        Button1.Visible = true;
                        Button2.Visible = true;
                        Button3.Visible = true;
                        lc.Visible = true;
                        Comentarios.Visible = true;
                        DatosV.Visible = false;
                        }



                    }
                    else
                    {
                        Button1.Visible = false;
                        Button2.Visible = false;
                        Button3.Visible = false;
                        lc.Visible = false;
                        Comentarios.Visible = false;
                        DatosV.Visible = true;
                    }


                }

            }


            sqlConnection1.Close();

        }
        catch (Exception ex)
        {

        }

        //try
        //{
        //    int Contador = 0;
        //    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        //    {
        //        SqlCommand cmd = new SqlCommand("spSelectUserDoc", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.Parameters.Add(new SqlParameter()
        //        { ParameterName = "@Opcion", Value = 2 });

        //        cmd.Parameters.Add(new SqlParameter()
        //        { ParameterName = "@User", Value = ID });

        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }

        //        conn.Open();
        //        SelComp.Items.Clear();
        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            ListItem Linea = new ListItem();
        //            Linea.Value = (rdr["Fecha"].ToString());
        //            SelComp.Items.Insert(0, Linea);
        //            IDSAGE.Text = rdr["ID"].ToString();
        //            Mail.Text = rdr["Email"].ToString();
        //            Contador = Contador + 1;
        //        }

        //        if (Contador >= 1)
        //        {
        //            BindGridInvoices(IDSAGE.Text, Mail.Text);
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    RutinaError(ex);
        //}
    }


    protected void SelComp_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Datos2();
    }
}