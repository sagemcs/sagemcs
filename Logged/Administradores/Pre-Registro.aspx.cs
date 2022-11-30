//PORTAL DE PROVEDORES T|SYS|
//31 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA REGISTRO DE USUARIOS AL PORTAL (PROVEEDORES Y USUARIOS T|SYS|)

//REFERENCIAS UTILIZADAS
using Microsoft.AspNet.Identity;
using System;
using System.Web.UI;
using WebSite1;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Net;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

public partial class Pre_Registro : System.Web.UI.Page
{   

    private readonly Page Me;

    string eventName = String.Empty;

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
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
                {

                    if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
                    {
                        if (Cclientes.Items.Count == 0)
                        {
                            Cclientes.Visible = false;
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

        Cam8.Visible = false;
        //Cam9.Visible = false;
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        try
        {
            bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (!isAuth)
            {
                HttpContext.Current.Session.RemoveAll();
                Response.AppendHeader("Pragma", "no-cache");
                Response.AppendHeader("Cache-Control", "no-cache");
                Response.CacheControl = "no-cache"; Response.Expires = -1;
                Response.ExpiresAbsolute = new DateTime(1900, 1, 1);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }

            if (HttpContext.Current.Session["RolUser"].ToString() != "T|SYS| - Admin")
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

    protected void VerificaCorreo(object sender, EventArgs e)
    {
        try
        {
            string Resulta = string.Empty;
            int err = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spValUserPortal", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = Email.Text
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Resulta = (rdr["Resultado"].ToString());
                }
            }


            if (Resulta == "0")
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    if (Email.Text == gvr.Cells[4].Text.ToString())
                    {
                        err = err + 1;
                    }
                }
            }

            else
            {
                CMail.Text = "Este Correo ya Fue Registrado";
            }



            if (err >= 1)
            {

            }



        }
        catch
        {


        }
    }

    //Validar estado en SAGE
    protected int EstausSAGE()
    {
        int status = 0;
        string sql;
        string EStatus;
        string Vkey = "";
        string RazonV = Cclientes.SelectedItem.ToString();
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spGetVendorsPortal", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = Email.Text
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable Registros = new DataTable();
                Registros.Load(rdr);

                string[] VRazon = new string[Registros.Rows.Count];
                int Fila = 0;
                foreach (DataRow row in Registros.Rows)
                {
                    VRazon[Fila] = (row["Nombre"].ToString());
                    Fila = Fila + 1;
                }
                int rep = 0;
                foreach (var grouping in VRazon.GroupBy(t => t).Where(t => t.Count() != 1))
                {
                    rep = grouping.Count();
                }

                if (rep >= 1)
                {

                    int cnt = 0;
                    int fin = 0;
                    for (int i = 0; i < 10; i++)
                    {
                        if (RazonV.Substring(i, 1) == "-") { i = 11; }
                        else { Vkey = Vkey + RazonV.Substring(i, 1).ToString(); cnt = cnt + 1; }
                    }
                }
                else
                {
                    SqlConnection sqlConnection2 = new SqlConnection();
                    sqlConnection2 = SqlConnectionDB("ConnectionString");
                    sqlConnection2.Open();

                    //sql = @"SELECT VendorKey from tapvendor Where Vendkey = '" + Vkey + "' And CompanyID = '" + CompanyID + "'";
                    sql = "";
                    using (var sqlQuery = new SqlCommand(sql, sqlConnection2))
                    {
                        sqlQuery.CommandType = CommandType.Text;
                        sqlQuery.CommandText = sql;
                        Vkey = sqlQuery.ExecuteScalar().ToString();
                    }
                    sqlConnection2.Close();
                }

                }

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("ConnectionString");

            sqlConnection1.Open();

            sql = @"SELECT Status from tapvendor Where Vendkey = '" + Vkey + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                EStatus = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            status = Convert.ToInt32(EStatus);

        }
        catch (Exception ex)
        {
            //Log Errores
        }
        return status;
    }

    protected void Campos(object sender, EventArgs e)
    {
        string Caja = string.Empty;
        string VRazon = string.Empty;
        string VEmpresa = string.Empty;

        if (Cclientes.Visible == true)
        {
            VRazon = Cclientes.SelectedItem.ToString();
        }
        else
        {
            VRazon = Razon.Text;
        }

        if (VRazon == "" && IDSAGE.Text == "" && RFC.Text == "" && Email.Text == "" && VEmpresa == "")
        {
            Caja = "ATD";
        }
        else if (RevisaPro() == false)
        {
            Caja = "AID2";
        }
        else if (VRazon == "")
        {
            Caja = "AID";
            
        }
        // *** AddedControl Luis Angel Mayo 2022 *** \\\
        else if (STipoProv.Text == "")
        {
            Caja = "AID3";
            tplabel.Text = "El tipo proveedor no puede estar vacio";
        }
        // *** AddedControl Luis Angel Mayo 2022 *** \\\
        else if (IDSAGE.Text == "")
        {
            Caja = "AID";
        }
        else if (RFC.Text == "")
        {
            Caja = "ARF2";
        }
        else if (RFC.Text != "" && RFC.Text.Length < 12)
        {
            Caja = "ARF1";
        }
        else if (Email.Text == "")
        {
            Caja = "AEM";
        }
        else if (Email.Text == "")
        {
            Caja = "AEM";
        }
        //else if (EstausSAGE() > 1)
        //{

        //}
        else if (EmpresaP.SelectedItem.ToString() == "")
        {
            Caja = "AEM";
        }

        if (Caja != "")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
        }
        else
        {
            //string cadena = string.Empty;
            //if (Cclientes.Visible == true) { cadena = Cclientes.SelectedItem.ToString(); }
            //else { cadena = Razon.Text.ToString(); }

            //string Vkey = "";
            //int cnt = 0;
            //for (int i = 0; i < 10; i++)
            //{
            //    if (cadena.Substring(i, 1) == "-") { i = 11; }
            //    else { Vkey = Vkey + cadena.Substring(i, 1).ToString(); cnt = cnt + 1; }
            //}
            //if (Convert.ToInt16(VendorID(Vkey)) >= 1)
            //{
            //    //rev = false;
            //    //Lsage.Text = "El Proveedor ya se encuentra registrado, para una actualización de Correo Electronico realizala desde el modulo Administrar Usuarios";
            //}
            //else
            //{
                VEmpresa = EmpresaP.SelectedItem.ToString();
                llenado();
            //}

        }
        Email.ReadOnly = false;

    }

    protected void Unnamed1_Clean(object sender, EventArgs e)
    {
        Razon.Text = "";
        IDSAGE.Text = "";
        RFC.Text = "";
        //TipoProv.Text = "";
        STipoProv.SelectedValue = "";
        Email.Text = "";
        Razon.Visible = true;
        Cclientes.Visible = false;
        Cclientes.Items.Clear();
        EmpresaP.Items.Clear();
        Email.ReadOnly = false;
        Email.Focus();
        
        //EmailC(GridView2, 2);
    }

    protected void Btn_Buscar(object sender, EventArgs e)
    {
        try
        {   
            Razon.Text = "";
            RFC.Text = "";
            //TipoProv.Text = "";
            STipoProv.SelectedValue = "";
            IDSAGE.Text = "";
            EmpresaP.Items.Clear();
            Razon.Visible = true;
            Cclientes.Visible = false;
            Cclientes.Items.Clear();

            if (Email_Ok(Email.Text) == true)
            {
                if (Email.Text != "")
                {
                    
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand("spGetVendorsPortal", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@Email",
                            Value = Email.Text
                        });

                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }

                        conn.Open();

                        SqlDataReader rdr = cmd.ExecuteReader();
                        DataTable Registros = new DataTable();
                        Registros.Load(rdr);

                        int Filas = Registros.Rows.Count;

                        if (Filas == 0)
                        {
                            conn.Close();
                            string Caja = "AID1";
                            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
                        }
                        else if (Filas == 1)
                        {
                            Email.ReadOnly = true;
                            DataRow Fila = Registros.Rows[0];
                            Razon.Visible = true;
                            Cclientes.Visible = false;
                            Razon.Text = Fila["Nombre"].ToString();
                            RFC.Text = Fila["RFC"].ToString();

                            //Add Luis Angel Mayo 2022
                            //TipoProv.Text = Fila["Tipo"].ToString();
                            //Add Luis Angel Mayo 2022

                            IDSAGE.Text = Fila["VendID"].ToString();
                            EmpresaP.Items.Clear();
                            ListItem Lin = new ListItem();
                            Lin.Value = (Fila["Empresa"].ToString());
                            EmpresaP.Items.Insert(0, Lin);
                        }
                        else if (Filas >= 2)
                        {
                            Email.ReadOnly = true;
                            string[] VRazon = new string[Registros.Rows.Count] ;
                            int Fila = 0;
                            foreach (DataRow row in Registros.Rows)
                            {
                                VRazon[Fila] = (row["Nombre"].ToString());
                                //Add Luis Angel Mayo 2022
                                //TipoProv.Text = row["Tipo"].ToString();
                                //Add Luis Angel Mayo 2022
                                Fila = Fila + 1;
                            }
                            int rep = 0;
                            foreach (var grouping in VRazon.GroupBy(t => t).Where(t => t.Count() != 1))
                            {
                                rep= grouping.Count();
                            }
                            RFC.Text = "";
                            //TipoProv.Text = "";
                            IDSAGE.Text = "";
                            foreach (DataRow row in Registros.Rows)
                            {
                                ListItem Linea = new ListItem();
                                if (rep >= 1) { Linea.Value = (row["Nombre2"].ToString()); }
                                else { Linea.Value = (row["Nombre"].ToString());}
                                Cclientes.Items.Insert(0, Linea);

                                //Add Luis Angel Mayo 2022
                                //TipoProv.Text = row["Tipo"].ToString();
                                //Add Luis Angel Mayo 2022

                                //RFC.Text = (row["RFC"].ToString());
                                //IDSAGE.Text = (row["VendID"].ToString());

                            }
                            Razon.Visible = false;
                            Cclientes.Visible = true;
                            Empres(rep);


                        }
                        conn.Close();
                    }
                }
                else
                {
                    string Caja = "AID";
                    CMail.Text = "Email No Encontrado, Verificalo con SAGE";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
                }
            }
            else
            {
                string Caja = "AID";
                CMail.Text = "Formato de Email Invalido";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
            }
        }
        catch (Exception a)
        {
            string Eror = a.Message.ToString();
        }

    }

    protected void btn_Enviar(object sender, EventArgs e)
    {
        try
        {   
            string ID,Tipo, Razon, RFC, Email, Empresa,Lista;
            Lista = "";
            int Contador = 0;
            int Total = GridView2.Rows.Count;
            if (GridView2.Rows.Count >= 1)
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                   
                    ID = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    Tipo = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    Razon = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    RFC = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                    Email = HttpUtility.HtmlDecode(gvr.Cells[5].Text.ToString());
                    Empresa = HttpUtility.HtmlDecode(gvr.Cells[6].Text.ToString());

                    //string key = User.Identity.Name.ToString();
                    string key = HttpContext.Current.Session["UserKey"].ToString();
                    string Rol = "Proveedor";

                    //return;

                    // Generate a new 12-character password with at least 1 non-alphanumeric character.
                    ApplicationDbContext context = new ApplicationDbContext();
                    UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
                    UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);

                    //UserValidator<ApplicationUser> Val = new UserValidator<ApplicationUser>(UserManager);
                    //Val.AllowOnlyAlphanumericUserNames = false;

                    string password = Membership.GeneratePassword(12, 1);
                    password = CrearPassword(8);
                    string hashedNewPassword = UserManager.PasswordHasher.HashPassword(password);

                    //Generate User
                    var manager = new UserManager();
                    var newuser = new ApplicationUser() { UserName = Email };
                    //manager.UserValidator = Val;

                    IdentityResult result = manager.Create(newuser, password);

                    Tipo = Tipo.Replace("Proveedor ", "");
                    
                    if (result.Succeeded)
                    {
                        manager.AddToRole(newuser.Id.ToString(), Rol);

                        if (Registro(ID, Tipo,Razon, Email, hashedNewPassword, key, Empresa, Rol, 1) == true)
                        {
                            Contador = Contador + 1;
                        }
                        else
                        {
                            Lista = Lista + Email + ",";
                        }
                    }
                    else
                    {
                        if (Registro(ID, Tipo,Razon, Email, hashedNewPassword, key, Empresa, Rol, 2) == true)
                        {
                            Contador = Contador + 1;
                        }
                        else
                        {
                            Lista = Lista + Email + ",";
                        }

                    }
                }

                if (Lista == "")
                {
                    ListEm(GridView2, 2);
                    string titulo, Msj, tipo;
                    DataTable dt = new DataTable();
                    GridView2.DataSource = dt;
                    GridView2.DataBind();
                    tipo = "success";
                    Msj = "Registro Exitoso";
                    titulo = "T|SYS|";
                    ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
                else
                {
                    string titulo, Msj, tipo;
                    tipo = "error";
                    Msj = "Algunos datos son invalidos no se pudieron registrar los correos: " + Lista + " Verifica que no se hayan registrado anteriormente";
                    titulo = "T|SYS|";
                    ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
            }
        }
        catch (Exception)
        {
            string titulo, Msj, tipo;
            tipo = "error";
            Msj = "Algunos Datos son Invalidos";
            titulo = "T|SYS| - Error de Ejecucion SP";
            ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }
    }

    public string CrearPassword(int longitud)
    {
        string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890$%&#!@";
        StringBuilder res = new StringBuilder();
        Random rnd = new Random();
        while (0 < longitud--)
        {
            res.Append(caracteres[rnd.Next(caracteres.Length)]);
        }
        return res.ToString();
    }

    protected void dpEstatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //ActualizarFacturas();
            int id = Convert.ToInt32(UsersR.SelectedValue);
            if (UsersR.SelectedValue == "4" || UsersR.SelectedValue == "5" || UsersR.SelectedValue == "6" || UsersR.SelectedValue == "7")
            {
                //cargarjefes(id);
                //Cam9.Visible = true;
            }
            else
            {
                //Cam9.Visible = false;
            }

            if (UsersR.SelectedValue == "4")
            {
                cblElemetos.Items.Clear();

                ListItem i;
                i = new ListItem("Carga de Facturas", "1");
                cblElemetos.Items.Add(i);

                i = new ListItem("Reembolso de Gastos", "2");
                cblElemetos.Items.Add(i);

                i = new ListItem("Anticipo de Gastos", "3");
                cblElemetos.Items.Add(i);

                i = new ListItem("Tarjeta Corporativa", "4");
                cblElemetos.Items.Add(i);

                i = new ListItem("Gastos Médicos Mayores", "5");
                cblElemetos.Items.Add(i);

                Cam8.Visible = true;

            }
            else if(UsersR.SelectedValue == "3" || UsersR.SelectedValue == "5" || UsersR.SelectedValue == "6" || UsersR.SelectedValue == "7" || UsersR.SelectedValue == "8")
            {
                cblElemetos.Items.Clear();

                ListItem i;
                i = new ListItem("Reembolso de Gastos", "1");
                cblElemetos.Items.Add(i);

                i = new ListItem("Anticipo de Gastos", "2");
                cblElemetos.Items.Add(i);

                i = new ListItem("Tarjeta Corporativa", "3");
                cblElemetos.Items.Add(i);

                i = new ListItem("Gastos Médicos Mayores", "4");
                cblElemetos.Items.Add(i);

                Cam8.Visible = true;
            }
            else
            {
                Cam8.Visible = false;
            }

            if (id < 4 || id == 8) 
            { 
                //DropDownList1.Items.Clear(); 
                //ListItem Linea1 = new ListItem();
                //Linea1.Value = ("");
                //DropDownList1.Items.Insert(0, Linea1);
            }


        }
        catch (Exception ex)
        {
           
        }
    }

    protected void cargarjefes(int id) 
    {
        try
        {   
            string sql;
            string superior = string.Empty;

            if (id == 4) { superior = "T|SYS| - Gerente"; }
            if (id == 5) { superior = "T|SYS| - Tesoreria"; }
            if (id == 6) { superior = "T|SYS| - Finanzas"; }
            if (id == 7) { superior = "T|SYS| - Gerencia de Capital Humano"; }

            //DropDownList1.Items.Clear();
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            sqlConnection1.Open();

            sql = @"select c.Username As Nombre From Roles a inner join UsersInRoles b on a.RoleKey = b.RoleKey inner join Users c on c.UserKey = b.UserKey Where a.RoleID ='" + superior + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                //Cuenta = sqlQuery.ExecuteScalar().ToString();

                SqlDataReader rdr = sqlQuery.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Nombre"].ToString().Length > 1)
                    {
                        ListItem Linea = new ListItem();
                        Linea.Value = (rdr["Nombre"].ToString());
                        //DropDownList1.Items.Insert(0, Linea);
                    }
                }
                sqlConnection1.Close();
            }       

            //if (DropDownList1.Items.Count == 0)
            //{
            //    ListItem Linea1 = new ListItem();
            //    Linea1.Value = ("Sin Usuarios Con Rol Superior");
            //    DropDownList1.Items.Insert(0, Linea1);
            //}
        }
        catch 
        {
            ListItem Linea1 = new ListItem();
            Linea1.Value = ("Error al cargar usuarios");
            //DropDownList1.Items.Insert(0, Linea1);
        }
    }

    protected void LimpiarLista() 
    {
        try
        {
            for (int i = 0; i < cblElemetos.Items.Count; i++)
            {
                cblElemetos.Items[i].Selected = false;               
            }
        }
        catch (Exception ex) 
        {
        
        }
    }

    protected bool Registro(string vID, string Tipo, string vRazon, string vEmail, string Pass, string vKey, string Empresa, string vRol, int vUserid)
    {
        bool Res = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spInUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@VendID", Value = vID });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Tipo", Value = Tipo });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Razon", Value = vRazon });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Email", Value = vEmail });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Status", Value = 3 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserKey", Value = vKey });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Empresa });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Rol", Value = vRol });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = vUserid });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                //while (rdr.Read())
                //{
                //    if (rdr["Total"].ToString() == "4")
                //    {
                        Res = true;
                //    }
                //    else
                //    {
                //        Res = false;
                //    }
                //}

                
                conn.Close();
            }
        }
        catch(Exception ex)
        {
            Res = false;
        }
        return Res;
    }

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView2.PageIndex = e.NewPageIndex;
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

    protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        try
        {
            int i, x;
            i = e.RowIndex;
            DataTable dt = new DataTable();

            dt.Columns.Add("ID SAGE");
            dt.Columns.Add("Razon Social");
            dt.Columns.Add("R.F.C.");
            dt.Columns.Add("Email");
            //dt.Columns.Add("Empresa");

            x = GridView2.Rows.Count;

            if (x == 1)
            {
                GridView2.DataSource = "";
            }
            else
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                    if (gvr.RowIndex != i)
                    {
                        DataRow dr = dt.NewRow();
                        dr["ID SAGE"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["Razon Social"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["R.F.C."] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        dr["Email"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[5].Text.ToString());
                        dt.Rows.Add(dr);

                    }
                }
            }

            GridView2.DataSource = dt;
            GridView2.DataBind();
        }
        catch (Exception v)
        {

        }
    }

    protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {

        }
    }

    private bool Email_Ok(string email)
    {
        string expresion;
        expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

        if (Regex.IsMatch(email, expresion))
        {
            if (Regex.Replace(email, expresion, string.Empty).Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    protected void llenado() {

        DataTable dt = new DataTable();

        dt.Columns.Add("ID SAGE");
        dt.Columns.Add("Tipo");
        dt.Columns.Add("Razon Social");
        dt.Columns.Add("R.F.C.");
        dt.Columns.Add("Email");
        dt.Columns.Add("Empresa");


        if (GridView2.Rows.Count >= 1)
        {
            foreach (GridViewRow gvr in GridView2.Rows)
            {
                DataRow dr = dt.NewRow();
                dr["ID SAGE"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                dr["Tipo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                dr["Razon Social"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                dr["R.F.C."] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                dr["Email"] = HttpUtility.HtmlDecode(gvr.Cells[5].Text.ToString());
                dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[6].Text.ToString());
                dt.Rows.Add(dr);
            }

        }
        string s = string.Empty;

        if (Razon.Visible == true && Razon.Text != "")
        {
            s = Razon.Text;
        }
        else if (Cclientes.Visible == true && Cclientes.SelectedItem.ToString() != "")
        {
            s = Cclientes.SelectedItem.ToString();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spGetVendorsPortal", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@Email",
                        Value = Email.Text
                    });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    DataTable Registros = new DataTable();
                    Registros.Load(rdr);

                    string[] VRazon = new string[Registros.Rows.Count];
                    int Fila = 0;
                    foreach (DataRow rol in Registros.Rows)
                    {
                        VRazon[Fila] = (rol["Nombre"].ToString());
                        Fila = Fila + 1;
                    }
                    int rep = 0;
                    foreach (var grouping in VRazon.GroupBy(t => t).Where(t => t.Count() != 1))
                    {
                        rep = grouping.Count();
                    }
                    if (rep >= 1)
                    {
                        string Vkey = "";
                        int cnt = 0;
                        int fin = 0;
                        string Cdf = Cclientes.SelectedItem.ToString();
                        for (int i = 0; i < 10; i++)
                        {
                            if (Cdf.Substring(i, 1) == "-") { i = 11; }
                            else { Vkey = Vkey + Cdf.Substring(i, 1).ToString(); cnt = cnt + 1; }
                        }

                        cnt = cnt + 2;
                        fin = Cdf.Length - cnt;
                        s = Cdf.Substring(cnt, fin);
                    }
                    else
                    {
                        s = Cclientes.SelectedItem.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        else
        {
            s = "";
        }
        string tipo;
        tipo = STipoProv.SelectedItem.ToString();

        DataRow row = dt.NewRow();
        row["ID SAGE"] = IDSAGE.Text;
        row["Tipo"] = tipo;
        row["Razon Social"] = s;
        row["R.F.C."] = RFC.Text;
        row["Email"] = Email.Text;
        row["Empresa"] = EmpresaP.SelectedItem.ToString();

        dt.Rows.Add(row);
        GridView2.DataSource = dt;
        GridView2.DataBind();

        Razon.Text = "";
        IDSAGE.Text = "";
        RFC.Text = "";
        //TipoProv.Text = "";
        STipoProv.SelectedValue = "";
        Email.Text = "";
        EmpresaP.Items.Clear();
        Cclientes.Items.Clear();
        Cclientes.Visible = false;
        Razon.Visible = true;
        Email.Focus();


    }

    protected void Empres(int res)
    {
        try
        {
            if (Cclientes.SelectedItem.ToString() != "")
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {

                    EmpresaP.Items.Clear();
                    IDSAGE.Text = "";
                    RFC.Text = "";
                    //TipoProv.Text = "";
                    string SP =string.Empty;
                    SqlCommand cmd = new SqlCommand(SP,conn);
                    string RazonV = Cclientes.SelectedItem.ToString();
                    if (res >= 1)
                    {
                    string Vkey = "";
                    int cnt = 0;
                    int fin = 0;
                    for (int i = 0; i < 10; i++)
                    {
                        if (RazonV.Substring(i, 1) == "-") { i = 11; }
                        else { Vkey = Vkey + RazonV.Substring(i, 1).ToString(); cnt = cnt + 1; }
                    }

                        cnt = cnt + 2;
                        fin = RazonV.Length - cnt;
                        //Razon = Razon.Substring(cnt, fin);
                        string Raz = RazonV.Substring(cnt, fin);
                        Vkey.Replace(" ", "");
                        SP = "spGetVendorsCompanyKey";
                        cmd = new SqlCommand(SP, conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@Razon",
                            Value = Raz
                        });

                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@Email",
                            Value = Email.Text
                        });
                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@Venkey",
                            Value = Vkey
                        });

                    }
                    else
                    {
                        SP = "spGetVendorsCompany";
                        cmd = new SqlCommand(SP, conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@Razon",
                            Value = RazonV
                        });

                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@Email",
                            Value = Email.Text
                        });
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ListItem Lin = new ListItem();
                        Lin.Value = (rdr["Empresa"].ToString());
                        EmpresaP.Items.Insert(0, Lin);
                        RFC.Text = (rdr["RFC"].ToString());
                        //TipoProv.Text = (rdr["Tipo"].ToString());
                        IDSAGE.Text = (rdr["VendID"].ToString());
                    }
                }
            }
        }
        catch(Exception ex)
        {

        }
    }

    protected void Roles(DropDownList Caja)
    {
        try
        {
            Caja.Items.Clear();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectRol", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Rol"].ToString() == "T|SYS| - Admin" || rdr["Rol"].ToString() == "T|SYS| - Validador" || rdr["Rol"].ToString() == "T|SYS| - Consultas")
                    {
                        ListItem Rol = new ListItem();
                        Rol.Value = (rdr["Rol"].ToString());
                        Caja.Items.Insert(0, Rol);
                    }
                }
                conn.Close();
            }
        }
        catch
        {

        }
    }

    protected void Cclientes_SelectedIndexChanged(object sender, EventArgs e)
    {

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spGetVendorsPortal", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = Email.Text
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable Registros = new DataTable();
                Registros.Load(rdr);

                string[] VRazon = new string[Registros.Rows.Count];
                int Fila = 0;
                foreach (DataRow row in Registros.Rows)
                {
                    VRazon[Fila] = (row["Nombre"].ToString());
                    Fila = Fila + 1;
                }
                int rep = 0;
                foreach (var grouping in VRazon.GroupBy(t => t).Where(t => t.Count() != 1))
                {
                    rep = grouping.Count();
                }

                Empres(rep);
            }
        }
        catch (Exception ex)
        {

        }
            
    }

    protected void LlenadoTsys()
    {
        DataTable dt = new DataTable();

        dt.Columns.Add("Nombre");
        dt.Columns.Add("Correo");
        dt.Columns.Add("Rol");
        //dt.Columns.Add("Superior");

        string[] Lista = new string[99];
        int i = 0;
        if (GridView1.Rows.Count >= 1)
        {
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                DataRow dr = dt.NewRow();
                CheckBox Checka = (CheckBox)gvr.Cells[4].FindControl("Check1");
                CheckBox Checke = (CheckBox)gvr.Cells[5].FindControl("Check2");
                CheckBox Checki = (CheckBox)gvr.Cells[6].FindControl("Check3");
                CheckBox Checko = (CheckBox)gvr.Cells[7].FindControl("Check4");
                CheckBox Checku = (CheckBox)gvr.Cells[8].FindControl("Check5");

                dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                dr["Rol"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());

                if (HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Empleado")
                {
                    Lista[i] = Checka.Checked.ToString() + "," + Checke.Checked.ToString() + "," + Checki.Checked.ToString() + "," + Checko.Checked.ToString() + "," + Checku.Checked.ToString();
                    i++;
                }
                else if(HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Validador" || HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Gerente" || HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Tesoreria" || HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Finanzas" || HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Gerencia de Capital Humano")
                {
                    Lista[i] = Checke.Checked.ToString() + "," + Checki.Checked.ToString() + "," + Checko.Checked.ToString() + "," + Checku.Checked.ToString();
                    i++;
                }

                //dr["Superior"] = HttpUtility.HtmlDecode(gvr.Cells[9].Text.ToString());
                dt.Rows.Add(dr);
            }

        }

        DataRow row = dt.NewRow();
        row["Nombre"] = NombreT.Text;
        row["Correo"] = EmailT.Text;
        row["Rol"] = UsersR.SelectedItem.ToString();
        //row["Superior"] =  DropDownList1.SelectedItem.ToString();

        if (UsersR.SelectedItem.ToString() == "T|SYS| - Empleado" || UsersR.SelectedItem.ToString() == "T|SYS| - Validador" || UsersR.SelectedItem.ToString() == "T|SYS| - Gerente" || UsersR.SelectedItem.ToString() == "T|SYS| - Tesoreria" || UsersR.SelectedItem.ToString() == "T|SYS| - Finanzas" || UsersR.SelectedItem.ToString() == "T|SYS| - Gerencia de Capital Humano") 
        {                                      
            for (int b = 0; b < cblElemetos.Items.Count; b++)
            {
                if (cblElemetos.Items[b].Selected == true)
                {
                    Lista[i] = Lista[i] + "true,";
                }
                else 
                {
                    Lista[i] = Lista[i] + "false,";
                }
            }
            //Lista[i] = "";
        }

        dt.Rows.Add(row);
        GridView1.DataSource = dt;
        GridView1.DataBind();

        i = 0;
        foreach (GridViewRow gvr in GridView1.Rows) 
        {
            CheckBox Checka = (CheckBox)gvr.Cells[4].FindControl("Check1");
            CheckBox Checke = (CheckBox)gvr.Cells[5].FindControl("Check2");
            CheckBox Checki = (CheckBox)gvr.Cells[6].FindControl("Check3");
            CheckBox Checko = (CheckBox)gvr.Cells[7].FindControl("Check4");
            CheckBox Checku = (CheckBox)gvr.Cells[8].FindControl("Check5");

            try
            {
                if (HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Empleado")
                {
                    string[] valores = Lista[i].Split(',');
                    Checka.Checked = Convert.ToBoolean(valores[0]);
                    Checke.Checked = Convert.ToBoolean(valores[1]);
                    Checki.Checked = Convert.ToBoolean(valores[2]);
                    Checko.Checked = Convert.ToBoolean(valores[3]);
                    Checku.Checked = Convert.ToBoolean(valores[4]);
                    i++;
                }
                else if(HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Validador" || HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Gerente" || HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Tesoreria" || HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Finanzas" || HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Gerencia de Capital Humano")
                {
                    string[] valores = Lista[i].Split(',');
                    Checka.Visible = false;
                    Checke.Checked = Convert.ToBoolean(valores[0]);
                    Checki.Checked = Convert.ToBoolean(valores[1]);
                    Checko.Checked = Convert.ToBoolean(valores[2]);
                    Checku.Checked = Convert.ToBoolean(valores[3]);
                    i++;
                }
                else
                {
                    Checka.Visible = false;
                    Checke.Visible = false;
                    Checki.Visible = false;
                    Checko.Visible = false;
                    Checku.Visible = false;
                }
            }
            catch (Exception ex) 
            {
            
            }
        }


        NombreT.Text = "";
        EmailT.Text = "";
        LimpiarLista();
        UsersR.SelectedValue = "2";
        NombreT.Focus();

    }

    protected void Unnamed2_Click(object sender, EventArgs e)
    {
        string Caja = string.Empty;
        try
        {
            int valor = Convert.ToInt32(UsersR.SelectedValue);
            if (NombreT.Text == "" && EmailT.Text == "")
            {
                Caja = "ATT";
            }
            else if (NombreT.Text == "")
            {
                Caja = "AUT";
            }
            else if (Revisa() == false)
            {
                CMT.Text = "El Campo Email Ingresado Ya se Encuentra Registrado o ya está dentro de la Tabla, Verificalo.";
                Caja = "AET";
            }
            else if (RevisaNoProv() == true)
            {
                CMT.Text = "ERROR Al Agregar,el email ingresado se encuentra registrado con los datos de un provedor, verificalo";
                Caja = "AET";
            }
            else if (EmailT.Text == "")
            {
                CMT.Text = "El Campo Email es Obligatorio.";
                Caja = "AET";
            }
            else if (Email_Ok(EmailT.Text) == false)
            {
                CMT.Text = "Formato de Email Invalido.";
                Caja = "AET";
            }
            else if (UsersR.SelectedValue == "4" && RevisaLista() == 0)
            {
                CMT.Text = "Al seleccionar Rol Empleado debe seleccionar al menos una Actividad para este Rol";
                Caja = "AET";
            }
            //else if (DropDownList1.SelectedItem.ToString() == "Sin Usuarios Con Rol Superior")
            //{
            //    CMT.Text = "Debe seleccionar un usuario con un Rol superior";
            //    Caja = "AET";
            //}
            //else if (DropDownList1.SelectedItem.ToString() == "Error al cargar usuarios")
            //{
            //    CMT.Text = "Debe seleccionar un usuario con un Rol superior";
            //    Caja = "AET";
            //}
            if (Caja == "")
            {
                LlenadoTsys();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
                UsersR.SelectedValue = "2";
                //DropDownList1.Items.Clear();
                //ListItem Linea1 = new ListItem();
                //Linea1.Value = ("");
                //DropDownList1.Items.Insert(0, Linea1);
            }
        }
        catch
        {

        }
    }

    protected int RevisaLista() 
    {
        int total = 0;
        try
        {            
            for (int i = 0; i < cblElemetos.Items.Count; i++)
            {
                if (cblElemetos.Items[i].Selected == true) 
                {
                    total++;
                }
                
            }
        }
        catch (Exception ex) 
        {
            return 0;
        }
        return total;
    }

    protected bool RevisaNoProv()
    {
        bool res = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spGetVendorsPortal", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@Email",
                        Value = EmailT.Text
                    });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    DataTable Registros = new DataTable();
                    Registros.Load(rdr);
                    int Filas = Registros.Rows.Count;

                    if (Filas >=1)
                    {
                        res = true;
                    }
              }

            }
            catch(Exception ex)
            {

            }
        return res;
        }

    protected bool Revisa()
    {
        bool rev = false;

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spValUserPortalT", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = EmailT.Text
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Company",
                    Value = ""
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Opcion",
                    Value = 2
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Resultado"].ToString() == "0")
                    {
                        rev = true;
                    }
                }
                conn.Close();
            }

            if (GridView1.Rows.Count >=1)
            {
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                //if (gvr.Cells[2].Text.ToString() == EmailT.Text && gvr.Cells[4].Text.ToString() == EmpersasT.SelectedItem.ToString())
                if (gvr.Cells[2].Text.ToString() == EmailT.Text)
                {
                    rev = false;
                }
            }
          }
        }
        catch (Exception b)
        {
            rev = false;
        }

        return rev;
    }

    protected bool RevisaPro()
    {
        bool rev = true;
        //string cadena = string.Empty;
        //if (Cclientes.Visible == true) { cadena = Cclientes.SelectedItem.ToString(); }
        //else { cadena = Razon.Text.ToString(); }

        //string Vkey = "";
        //int cnt = 0;
        //for (int i = 0; i < 10; i++)
        //{
        //    if (cadena.Substring(i, 1) == "-") { i = 11; }
        //    else { Vkey = Vkey + cadena.Substring(i, 1).ToString(); cnt = cnt + 1; }
        //}

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spValUserPortalT", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Email",
                    Value = Email.Text
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Company",
                    Value = EmpresaP.Text
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Opcion",
                    Value = 1
                });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Resultado"].ToString() != "0")
                    {
                        rev = false;
                        Lsage.Text = "El proveedor ya se encuentra Registrado";
                    }
                }
                conn.Close();
            }

            if (GridView2.Rows.Count >= 1)
            {
                foreach (GridViewRow gvr in GridView2.Rows)
                {
                    if (gvr.Cells[4].Text.ToString() == Email.Text && gvr.Cells[5].Text.ToString() == EmpresaP.SelectedItem.ToString())
                    {
                        rev = false;
                        Lsage.Text = "Email ingresado ya se encuentra dentro de la tabla de registros a procesar";
                    }
                }
            }

        }
        catch (Exception b)
        {
            rev = false;
        }

        return rev;
    }

    protected string VendorID(string VendorKey)
    {
        try
        {

            SqlCommand sqlSelectCommand1 = new SqlCommand();
            SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter();
            SqlConnection sqlConnection1 = new SqlConnection();

            sqlConnection1 = SqlConnectionDB("ConnectionString");

            string sSQL;

            sqlConnection1.Open();

            sSQL = @"select count(*) from Vendors Where VendorKey  = @varID";


            string VendorId = "";

            using (var sqlQuery = new SqlCommand(sSQL, sqlConnection1))
            {
                sqlQuery.Parameters.AddWithValue("@varID", VendorKey);
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
            return "99";
        }

    }

    protected void Unnamed2_Clean(object sender, EventArgs e)
    {
        NombreT.Text = "";
        EmailT.Text = "";
        NombreT.Focus();
        LimpiarLista();
        UsersR.SelectedValue = "2";
        //DropDownList1.Items.Clear();
        //ListItem Linea1 = new ListItem();
        //Linea1.Value = ("");
        //DropDownList1.Items.Insert(0, Linea1);
        //EmailC(GridView1,1);
    }

    protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set the index of the new display page.  
        GridView1.PageIndex = e.NewPageIndex;

        // Rebind the GridView control to  
        // show data in the new page. 
        //BindGridView();
    }

    protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

        }
    }

    protected void GridView2_RowCreated(object sender, GridViewRowEventArgs e)
    {

    }

    protected void GridView2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        try
        {
            int i, x;
            i = e.RowIndex;

            string[] Lista = new string[99];
            int iv = 0;

            DataTable dt = new DataTable();

            dt.Columns.Add("Nombre");
            dt.Columns.Add("Correo");
            dt.Columns.Add("Rol");
            //dt.Columns.Add("Superior");
            //dt.Columns.Add("Empresa");

            x = GridView1.Rows.Count;

            if (x == 1)
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

                        CheckBox Checka = (CheckBox)gvr.Cells[4].FindControl("Check1");
                        CheckBox Checke = (CheckBox)gvr.Cells[5].FindControl("Check2");
                        CheckBox Checki = (CheckBox)gvr.Cells[6].FindControl("Check3");
                        CheckBox Checko = (CheckBox)gvr.Cells[7].FindControl("Check4");
                        CheckBox Checku = (CheckBox)gvr.Cells[8].FindControl("Check5");

                        dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                        dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                        dr["Rol"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                        //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
                        if (HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Empleado")
                        {
                            Lista[iv] = Checka.Checked.ToString() + "," + Checke.Checked.ToString() + "," + Checki.Checked.ToString() + "," + Checko.Checked.ToString() + "," + Checku.Checked.ToString();
                            iv++;
                        }
                        //dr["Superior"] = HttpUtility.HtmlDecode(gvr.Cells[9].Text.ToString());
                        dt.Rows.Add(dr);

                    }
                }
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();

            iv = 0;
            foreach (GridViewRow gvr in GridView1.Rows)
            {
                CheckBox Checka = (CheckBox)gvr.Cells[4].FindControl("Check1");
                CheckBox Checke = (CheckBox)gvr.Cells[5].FindControl("Check2");
                CheckBox Checki = (CheckBox)gvr.Cells[6].FindControl("Check3");
                CheckBox Checko = (CheckBox)gvr.Cells[7].FindControl("Check4");
                CheckBox Checku = (CheckBox)gvr.Cells[8].FindControl("Check5");

                if (HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Empleado")
                {
                    string[] valores = Lista[iv].Split(',');
                    Checka.Checked = Convert.ToBoolean(valores[0].ToString());
                    Checke.Checked = Convert.ToBoolean(valores[1].ToString());
                    Checki.Checked = Convert.ToBoolean(valores[2].ToString());
                    Checko.Checked = Convert.ToBoolean(valores[3].ToString());
                    Checku.Checked = Convert.ToBoolean(valores[4].ToString());
                    iv++;
                }
                else 
                {
                    Checka.Visible = false;
                    Checke.Visible = false;
                    Checki.Visible = false;
                    Checko.Visible = false;
                    Checku.Visible = false;
                }

            }

            NombreT.Text = "";
            EmailT.Text = "";
            LimpiarLista();
            UsersR.SelectedValue = "2";
            NombreT.Focus();

        }
        catch (Exception v)
        {

        }
    }

    protected void GridView2_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {

        }
    }

    protected bool RegistroTsys(string vID,string vEmail, string Pass, string vKey, string vRol, string vUserid,string cadena)
    {
        bool Res = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spInUserTsys", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Nombre", Value = vID });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Email", Value = vEmail });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserKey", Value = vKey });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Cadena", Value = cadena });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@superior", Value = ""});

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Rol", Value = vRol });

                //cmd.Parameters.Add(new SqlParameter()
                //{ ParameterName = "@KeyId", Value = vUserid });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                //DataTable Tbl = new DataTable();
                //Tbl.Load(rdr);
                //DataRow Fila = Tbl.Rows[0];

                //if (Fila["Total"].ToString() == "3")
                //{
                    Res = true;
                //}
                //else
                //{
                //    Res = false;
                //}
                conn.Close();
            }
        }
        catch(Exception ex)
        {
            Res = false;
        }
        return Res;
    }

    protected void btn_EnviarTsy(object sender, EventArgs e)
    {
        try
        {
            string Nombre, Email, RolT,Lista, superior;
            Lista = "";
            int Contador = 0;
            int Total = GridView1.Rows.Count;
            if (GridView1.Rows.Count >= 1)
            {
                foreach (GridViewRow gvr in GridView1.Rows)
                {
                    Nombre = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
                    Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
                    RolT = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
                    //superior = HttpUtility.HtmlDecode(gvr.Cells[9].Text.ToString());

                    //Empresa = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());

                    //string key = User.Identity.Name.ToString();
                    string key = HttpContext.Current.Session["UserKey"].ToString();

                    // Generate a new 12-character password with at least 1 non-alphanumeric character.
                    ApplicationDbContext context = new ApplicationDbContext();
                    UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
                    UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
                    //UserValidator<ApplicationUser> Val = new UserValidator<ApplicationUser>(UserManager);
                    //Val.AllowOnlyAlphanumericUserNames = false;


                    string password = CrearPassword(8);
                    string hashedNewPassword = UserManager.PasswordHasher.HashPassword(password);


                    
                    //Generate User
                    var manager = new UserManager();
                    //manager.UserValidator = Val;
                    var newuser = new ApplicationUser() { UserName = Email };
                    

                    IdentityResult result = manager.Create(newuser, password);
                    if (result.Succeeded)
                    //if(userName1.ToString() =! "")
                    {
                        manager.AddToRole(newuser.Id.ToString(), RolT);
                        string userid = newuser.Id.ToString();
                        string cadena = "";
                        try
                        {

                            if (HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Empleado" || HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Validador" || HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Gerente" || HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Tesoreria" || HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Finanzas" || HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Gerencia de Capital Humano")
                            {
                                CheckBox Checka = (CheckBox)gvr.Cells[4].FindControl("Check1");
                                CheckBox Checke = (CheckBox)gvr.Cells[5].FindControl("Check2");
                                CheckBox Checki = (CheckBox)gvr.Cells[6].FindControl("Check3");
                                CheckBox Checko = (CheckBox)gvr.Cells[7].FindControl("Check4");
                                CheckBox Checku = (CheckBox)gvr.Cells[8].FindControl("Check5");
                                cadena = Checka.Checked.ToString() + "," + Checke.Checked.ToString() + "," + Checki.Checked.ToString() + "," + Checko.Checked.ToString() + "," + Checku.Checked.ToString();
                                cadena = cadena.Replace("True", "1").Replace("False", "0");
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        if (RegistroTsys(Nombre, Email, password, key, RolT, userid,cadena) == true)
                        {
                            Contador = Contador + 1;
                        }
                        else
                        {
                            Lista = Lista + Email + ",";
                        }
                    }
                    else
                    {
                        Lista = Lista + Email + ",";
                    }
                }

                if (Lista == "")
                {
                    ListEm(GridView1, 1);
                    string titulo, Msj, tipo;
                    DataTable dts = new DataTable();
                    GridView1.DataSource = dts;
                    GridView1.DataBind();
                    tipo = "success";
                    Msj = "Registro Exitoso";
                    titulo = "T|SYS|";
                    ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
                else
                {
                    string titulo, Msj, tipo;
                    tipo = "error";
                    Msj = "Algunos Datos son Invalidos no se puedieron registrar los correos: " + Lista + " Verifica que no se hayan registrado anteriormente";
                    titulo = "T|SYS| - Error de Ejecucion SP";

                    ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    protected void Empresa(DropDownList Caja)
    {
        try
        {
            Caja.Items.Clear();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spSelectCompany", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    //Caja.Items.Add(rdr["Empresas"].ToString());
                    ListItem Lin = new ListItem();
                    Lin.Value = (rdr["Empresas"].ToString());
                    Caja.Items.Insert(0, Lin);
                }
                conn.Close();
            }
        }
        catch (Exception b)
        {

        }

    }

    private bool EmailC(GridView Tabla,int Opcion,string Destinatario)
    {
        bool Resut = false;
        try
        {
         string PassNew = "<div ID='Tabla'><div class='panel panel-success'><div class='table-responsive'><table class='table table-striped' id='table-to-result-ass-indic'><tbody> ";

         foreach (GridViewRow gvr in Tabla.Rows)
         {
              if (Opcion == 2)
              {
               PassNew = PassNew + "<tr class='table-primary'><th>Clase : Proveedor     </th><td>  Email Registrado :  " + HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString()) +"</td></tr><br/>";
              }
              if (Opcion == 1)
              {
              PassNew = PassNew + "<tr class='table-primary'><th>Clase : Usuario Tsys     </th><td>     " + HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString()) + "</td>  Rol Asignado : " + HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) + "<td></td>  </tr><br/>";
              }
         }
          PassNew = PassNew + "</tbody></ table ></ div ></ div ></ div > ";
          string body = string.Empty;

          using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionNO.html")))
          {
            body = reader.ReadToEnd();
            body = body.Replace("{PassTemp}", PassNew);
          }

          Resut = Global.EmailGlobal(Destinatario, body, "REGISTRO DE USUARIOS NUEVOS");

        }
        catch (Exception b)
        {
            Resut = false;
        }
        return Resut;
    }

    protected bool ListEm(GridView Tabla, int Opcion)
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
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = "Usuario Nuevo" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 1 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string Desti = HttpUtility.HtmlDecode(rdr["Destinatario"].ToString());

                    if (Desti != "" && Desti != "Usuario")
                    {
                        EmailC(Tabla, Opcion,Desti);
                    } 
                }
            }
        }
        catch
        {

        }
        return uno;
    }

    protected bool RegistroEx(string vID, string vRazon, string vEmail, string Pass, string vKey, string Empresa, string vRol, string vUserid)
    {
        bool Res = false;
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spExcel", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@VendID", Value = vID });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Razon", Value = vRazon });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Email", Value = vEmail });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Status", Value = 3 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserKey", Value = vKey });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Company", Value = Empresa });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Rol", Value = vRol });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@KeyId", Value = vUserid });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                //DataTable Tbl = new DataTable();
                //Tbl.Load(rdr);
                //DataRow Fila = Tbl.Rows[0];

                //if (Fila["Total"].ToString() == "4")
                //{
                //    Res = true;
                //}
                //else
                //{
                    Res = true;
                //}
                conn.Close();
            }
        }
        catch
        {
            Res = false;
        }
        return Res;
    }

    protected bool UpdatePAss(string Email, string NewPass)
    {
        bool Resultado = false;
        try
        {
            string oldPass = string.Empty;
            string IdUser = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd1 = new SqlCommand("spResetPass", conn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.Parameters.Add(new SqlParameter()
                { ParameterName = "@Pass", Value = NewPass });

                cmd1.Parameters.Add(new SqlParameter()
                { ParameterName = "@Email", Value = Email });

                cmd1.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = 2 });

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Open();
                SqlDataReader rdr1 = cmd1.ExecuteReader();
                Resultado = true;
                conn.Close();
            }
        }

        catch (Exception ex)
        {
            string Error = ex.Message;
            Resultado = false;
        }
        return Resultado;
    }

    private string Emailexc(string CorreoUs, string Company)
    {
        string Resut = "Error al enviar correo de confirmación a " + CorreoUs;
        try
        {
          string PassNew = Membership.GeneratePassword(8, 1);
          string PassHAs = PassNew.ToString();
          ApplicationDbContext context = new ApplicationDbContext();
          UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
          UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
          UserValidator<ApplicationUser> Val = new UserValidator<ApplicationUser>(UserManager);
          Val.AllowOnlyAlphanumericUserNames = false;

          string hashedNewPassword = UserManager.PasswordHasher.HashPassword(PassHAs);
          bool UpdatePass = UpdatePAss(CorreoUs, hashedNewPassword);
          bool send = false;

          string body = string.Empty;
          using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionOkUser.html")))
          {
             body = reader.ReadToEnd();
             body = body.Replace("{UserTemp}", CorreoUs);
             body = body.Replace("{PassTemp}", PassHAs);
           }

            send = Global.EmailGlobal(CorreoUs, body, "BIENVENIDO PORTAL T|SYS|");
            if (send == true) { Resut = "Ok"; }
            else
            {
             int pLogKey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
             int pUserKey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
             LogError(pLogKey, pUserKey, "Envio de Correo de Registro de Usuario Excel" , Resut, Company);
           }
        }
        catch (Exception b)
        {
            Resut = b.Message;
        }
        return Resut;
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
            //HttpContext.Current.Session["Error"] = err;
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
            // ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B6);", true);

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

}