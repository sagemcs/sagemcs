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

public partial class EditPreRegistro : System.Web.UI.Page
{   

    private readonly Page Me;
    private int iUser;

    public int idUser
    {
        get
        {
            return this.iUser;
        }
        set
        {
            this.iUser = value;
        }
    }

    string eventName = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        if (!IsPostBack)
        {
            llenado();
            if (HttpContext.Current.Session["IDCompany"] == null)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                //if (HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
                if (true)
                {

                    //if (HttpContext.Current.Session["RolUser"].ToString() == "T|SYS| - Admin")
                    if (true)
                    {
                        
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

        //Cam8.Visible = false;
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

            //if (HttpContext.Current.Session["RolUser"].ToString() != "T|SYS| - Admin")
            if (!true)
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
                //foreach (GridViewRow gvr in GridView1.Rows)
                //{
                //    if (Email.Text == gvr.Cells[4].Text.ToString())
                //    {
                //        err = err + 1;
                //    }
                //}
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

    protected List<UserEditDTO> Get_Users(int page, out int count)
    {
        var users = new List<UserEditDTO>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = new SqlCommand("SELECT u.UserKey, u.UserID, u.UserName, uir.RoleKey, dbo.Roles.RoleID, u.CreateDate, u.UpdateDate, (select us.Email from AspNetUsers AS us where us.UserKey = u.UserKey) FROM dbo.Users AS u INNER JOIN dbo.UsersInRoles AS uir ON u.UserKey = uir.UserKey INNER JOIN dbo.Roles ON uir.RoleKey = dbo.Roles.RoleKey WHERE(u.Status = 1 and RoleID <> 'Proveedor')", conn);
            cmd.Connection.Open();
            SqlDataReader lector = cmd.ExecuteReader();
            while(lector.Read())
            {
                //SqlCommand email = new SqlCommand("SELECT u.Email FROM dbo.AspNetUsers AS u WHERE UserKey = @userkey", conn);
                //email.Parameters.AddWithValue("@UserKey", lector.GetInt32(0));
                //SqlDataReader emailL = email.ExecuteReader();
                //var val = lector.Read();

                var user = new UserEditDTO();
                user.UserKey = lector.GetInt32(0);
                user.UserID = lector.GetString(1);
                user.UserName = lector.GetString(2);
                user.RoleKey = lector.GetInt32(3);
                user.Rol = lector.GetString(4);
                user.CreateDate = !lector.IsDBNull(5) ? (DateTime?)lector.GetDateTime(5) : null;
                user.UpdateDate = !lector.IsDBNull(6) ? (DateTime?)lector.GetDateTime(6) : null;
                user.Email = !lector.IsDBNull(7) ? lector.GetString(7) : lector.GetString(1);
                users.Add(user);
            }
            count = users.Count();
            
            return users.Skip((page -1) * 10).Take(10).ToList();
;        }
    }

    private void BindGridView()
    {
        GridView2.DataSource = null;
        GridView2.Visible = true;
        if (HttpContext.Current.Session["page"] == null)
        {
            HttpContext.Current.Session["page"] = 1;
        }
        int count = 0;
        int page = (int)HttpContext.Current.Session["page"];
        GridView2.DataSource = Get_Users(page, out count);
        int abs = Math.Abs(count / 10);
        int pages = (count % 100) == 0 ? abs : abs + 1;
        if (pages > page)
        {
            btn_siguiente.Enabled = true;
        }
        if (page <= 1)
        {
            btn_anterior.Enabled = false;
        }
        lbl_pages.Text = string.Format("Pagina {0} de {1}", page, pages);

        GridView2.DataBind();

        //GridView2.DataSource = ReadFromDb(pUserKey);
       
    }

    //private List<CorporateCardDTO> ReadFromDb(int user_id)
    //{
    //    List<CorporateCardDTO> gastos = new List<CorporateCardDTO>();

    //    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
    //    {
    //        SqlCommand cmd = conn.CreateCommand();
    //        cmd.CommandText = "SELECT u.UserKey, u.UserID, u.UserName, uir.RoleKey, dbo.Roles.RoleID, u.CreateDate, u.UpdateDate, (select us.Email from AspNetUsers AS us where us.UserKey = u.UserKey) FROM dbo.Users AS u INNER JOIN dbo.UsersInRoles AS uir ON u.UserKey = uir.UserKey INNER JOIN dbo.Roles ON uir.RoleKey = dbo.Roles.RoleKey WHERE(u.Status = 1)";
    //        cmd.Connection.Open();
    //        SqlDataReader dataReader = cmd.ExecuteReader();
    //        while (dataReader.Read())
    //        {
    //            var tarjeta = new CorporateCardDTO();
    //            tarjeta.CorporateCardId = dataReader.GetInt32(0);
    //            tarjeta.Type = dataReader.GetString(1);
    //            tarjeta.Date = dataReader.GetDateTime(2);
    //            tarjeta.Currency = Dict_moneda().First(x => x.Key == dataReader.GetInt32(3)).Value;
    //            tarjeta.Amount = dataReader.GetDecimal(4);
    //            tarjeta.Status = Dict_status().First(x => x.Key == dataReader.GetInt32(5)).Value;
    //            tarjeta.FileNameXml = dataReader.GetString(6);
    //            tarjeta.FileNamePdf = dataReader.GetString(7);
    //            tarjeta.FileNamePdfVoucher = dataReader.GetString(8);
    //            gastos.Add(tarjeta);
    //        }
    //    }

    //    return gastos;

    //}


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
            //if (UsersR.SelectedValue == "4")
            //{
            //    Cam8.Visible = true;
            //}
            //else
            //{
            //    Cam8.Visible = false;
            //}
        }
        catch (Exception ex)
        {
           
        }
    }

    protected void LimpiarLista() 
    {
        chkFacturas.Checked = false;
        chkReembolso.Checked = false;
        chkAnticipo.Checked = false;
        chkTarjeta.Checked = false;
        chkMédicos.Checked = false;
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

    protected void llenado() 
    {
        //DataTable dt = new DataTable();

        //dt.Columns.Add("ID SAGE");
        //dt.Columns.Add("Tipo");
        //dt.Columns.Add("Razon Social");
        //dt.Columns.Add("R.F.C.");
        //dt.Columns.Add("Email");
        //dt.Columns.Add("Empresa");


        //if (GridView2.Rows.Count >= 1)
        //{
        //    foreach (GridViewRow gvr in GridView2.Rows)
        //    {
        //        DataRow dr = dt.NewRow();
        //        dr["ID SAGE"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
        //        dr["Tipo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
        //        dr["Razon Social"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
        //        dr["R.F.C."] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
        //        dr["Email"] = HttpUtility.HtmlDecode(gvr.Cells[5].Text.ToString());
        //        dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[6].Text.ToString());
        //        dt.Rows.Add(dr);
        //    }

        //}
        //string s = string.Empty;

        //if (Razon.Visible == true && Razon.Text != "")
        //{
        //    s = Razon.Text;
        //}
        //else if (Cclientes.Visible == true && Cclientes.SelectedItem.ToString() != "")
        //{
        //    s = Cclientes.SelectedItem.ToString();
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        //        {
        //            SqlCommand cmd = new SqlCommand("spGetVendorsPortal", conn);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add(new SqlParameter()
        //            {
        //                ParameterName = "@Email",
        //                Value = Email.Text
        //            });

        //            if (conn.State == ConnectionState.Open)
        //            {
        //                conn.Close();
        //            }

        //            conn.Open();

        //            SqlDataReader rdr = cmd.ExecuteReader();
        //            DataTable Registros = new DataTable();
        //            Registros.Load(rdr);

        //            string[] VRazon = new string[Registros.Rows.Count];
        //            int Fila = 0;
        //            foreach (DataRow rol in Registros.Rows)
        //            {
        //                VRazon[Fila] = (rol["Nombre"].ToString());
        //                Fila = Fila + 1;
        //            }
        //            int rep = 0;
        //            foreach (var grouping in VRazon.GroupBy(t => t).Where(t => t.Count() != 1))
        //            {
        //                rep = grouping.Count();
        //            }
        //            if (rep >= 1)
        //            {
        //                string Vkey = "";
        //                int cnt = 0;
        //                int fin = 0;
        //                string Cdf = Cclientes.SelectedItem.ToString();
        //                for (int i = 0; i < 10; i++)
        //                {
        //                    if (Cdf.Substring(i, 1) == "-") { i = 11; }
        //                    else { Vkey = Vkey + Cdf.Substring(i, 1).ToString(); cnt = cnt + 1; }
        //                }

        //                cnt = cnt + 2;
        //                fin = Cdf.Length - cnt;
        //                s = Cdf.Substring(cnt, fin);
        //            }
        //            else
        //            {
        //                s = Cclientes.SelectedItem.ToString();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        //else
        //{
        //    s = "";
        //}
        //string tipo;
        //tipo = STipoProv.SelectedItem.ToString();

        //DataRow row = dt.NewRow();
        //row["ID SAGE"] = IDSAGE.Text;
        //row["Tipo"] = tipo;
        //row["Razon Social"] = s;
        //row["R.F.C."] = RFC.Text;
        //row["Email"] = Email.Text;
        //row["Empresa"] = EmpresaP.SelectedItem.ToString();

        //dt.Rows.Add(row);
        int count = 0;
        if(HttpContext.Current.Session["page"]==null)
        {
            HttpContext.Current.Session["page"] = 1;
        }
        int page = (int)HttpContext.Current.Session["page"];
        GridView2.DataSource = Get_Users(page, out count);
        int abs = Math.Abs(count / 10);
        int pages = (count % 100) == 0 ? abs : abs + 1;
        if (pages > page)
        {
            btn_siguiente.Enabled = true;
        }
        if (page <= 1)
        {
            btn_anterior.Enabled = false;
        }
        lbl_pages.Text = string.Format("Pagina {0} de {1}", page, pages);

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

    //protected void LlenadoTsys()
    //{
    //    DataTable dt = new DataTable();

    //    dt.Columns.Add("Nombre");
    //    dt.Columns.Add("Correo");
    //    dt.Columns.Add("Rol");

    //    string[] Lista = new string[99];
    //    int i = 0;
    //    if (GridView1.Rows.Count >= 1)
    //    {
    //        foreach (GridViewRow gvr in GridView1.Rows)
    //        {
    //            DataRow dr = dt.NewRow();
    //            CheckBox Checka = (CheckBox)gvr.Cells[4].FindControl("Check1");
    //            CheckBox Checke = (CheckBox)gvr.Cells[5].FindControl("Check2");
    //            CheckBox Checki = (CheckBox)gvr.Cells[6].FindControl("Check3");
    //            CheckBox Checko = (CheckBox)gvr.Cells[7].FindControl("Check4");
    //            CheckBox Checku = (CheckBox)gvr.Cells[8].FindControl("Check5");

    //            dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
    //            dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
    //            dr["Rol"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());

    //            if (HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Empleado")
    //            {
    //                Lista[i] = Checka.Checked.ToString() + "," + Checke.Checked.ToString() + "," + Checki.Checked.ToString() + "," + Checko.Checked.ToString() + "," + Checku.Checked.ToString();
    //                i++;
    //            }
    //            dt.Rows.Add(dr);
    //        }

    //    }

    //    DataRow row = dt.NewRow();
    //    row["Nombre"] = NombreT.Text;
    //    row["Correo"] = EmailT.Text;
    //    row["Rol"] = UsersR.SelectedItem.ToString();

    //    if (UsersR.SelectedItem.ToString() == "T|SYS| - Empleado") 
    //    {                                      
    //        //for (int b = 0; b < cblElemetos.Items.Count; b++)
    //        //{
    //        //    if (cblElemetos.Items[b].Selected == true)
    //        //    {
    //        //        Lista[i] = Lista[i] + "true,";
    //        //    }
    //        //    else 
    //        //    {
    //        //        Lista[i] = Lista[i] + "false,";
    //        //    }
    //        //}
    //        //Lista[i] = "";
    //    }

    //    dt.Rows.Add(row);
    //    GridView1.DataSource = dt;
    //    GridView1.DataBind();

    //    i = 0;
    //    foreach (GridViewRow gvr in GridView1.Rows) 
    //    {
    //        CheckBox Checka = (CheckBox)gvr.Cells[4].FindControl("Check1");
    //        CheckBox Checke = (CheckBox)gvr.Cells[5].FindControl("Check2");
    //        CheckBox Checki = (CheckBox)gvr.Cells[6].FindControl("Check3");
    //        CheckBox Checko = (CheckBox)gvr.Cells[7].FindControl("Check4");
    //        CheckBox Checku = (CheckBox)gvr.Cells[8].FindControl("Check5");

    //        try
    //        {
    //            if (HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Empleado")
    //            {
    //                string[] valores = Lista[i].Split(',');
    //                Checka.Checked = Convert.ToBoolean(valores[0]);
    //                Checke.Checked = Convert.ToBoolean(valores[1]);
    //                Checke.Checked = Convert.ToBoolean(valores[1]);
    //                Checki.Checked = Convert.ToBoolean(valores[2]);
    //                Checko.Checked = Convert.ToBoolean(valores[3]);
    //                Checku.Checked = Convert.ToBoolean(valores[4]);
    //                i++;
    //            }
    //            else
    //            {
    //                Checka.Visible = false;
    //                Checke.Visible = false;
    //                Checki.Visible = false;
    //                Checko.Visible = false;
    //                Checku.Visible = false;
    //            }
    //        }
    //        catch (Exception ex) 
    //        {
            
    //        }
    //    }


    //    NombreT.Text = "";
    //    EmailT.Text = "";
    //    LimpiarLista();
    //    UsersR.SelectedValue = "2";
    //    NombreT.Focus();

    //}

    protected void Unnamed2_Click(object sender, EventArgs e)
    {
        string Caja = string.Empty;
        try
        {
            if (NombreT.Text == "" && EmailT.Text == "")
            {
                Caja = "ATT";
            }
            else if (NombreT.Text == "")
            {
                Caja = "AUT";
            }
            //else if (Revisa() == false)
            //{
            //    CMT.Text = "El Campo Email Ingresado Ya se Encuentra Registrado o ya está dentro de la Tabla, Verificalo.";
            //    Caja = "AET";
            //}
            //else if (RevisaNoProv() == true)
            //{
            //    CMT.Text = "ERROR Al Agregar,el email ingresado se encuentra registrado con los datos de un provedor, verificalo";
            //    Caja = "AET";
            //}
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
            else if (!RevisaLista())
            {
                CMT.Text = "Al seleccionar Rol Empleado debe seleccionar al menos un permiso para este Rol";
                Caja = "AET";
            }

            if (Caja == "")
            {
                //LlenadoTsys();
                try
                {
                    UpdateUser(NombreT.Text, EmailT.Text, UsersR.SelectedValue);
                }
                catch (Exception exc)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(A ocurrido un error al actualizar los datos del usuario);", true);
                    UsersR.SelectedValue = "2";
                }
                finally
                {
                    try
                    {
                        UpdatePermissions();
                    }
                    catch (Exception exc)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(A ocurrido un error al actualizar los permisos del usuario);", true);
                        UsersR.SelectedValue = "2";
                    }
                    finally
                    {
                        BindGridView();
                        NombreT.Text = "";
                        EmailT.Text = "";
                        UsersR.SelectedValue = "5";
                        TextBoxSearchEmail.Text = "";
                        TextBoxSearchName.Text = "";

                        LimpiarLista();
                    }
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(" + Caja + ");", true);
                UsersR.SelectedValue = "2";
            }
        }
        catch(Exception ek)
        {
            var a = ek.Data;
        }
    }

    protected bool UpdateUser(string nombre, string email, string idRol)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "update Users set UserName = @username WHERE UserKey = @idUser;";
            cmd.Parameters.Add("@idUser", SqlDbType.Int).Value = HttpContext.Current.Session["idUser"];
            cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = nombre;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            if (email != (string)HttpContext.Current.Session["emailUser"])
            {
                cmd.CommandText = "update AspNetUsers set Email = @email WHERE UserKey = @idUser;";
                cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
                cmd.ExecuteNonQuery();
            }
            if (idRol != (string)HttpContext.Current.Session["roleUser"])
            {
                cmd.CommandText = "update UsersInRoles set RoleKey = @role WHERE UserKey = @idUser;";
                cmd.Parameters.Add("@role", SqlDbType.Int).Value = idRol;
                cmd.ExecuteNonQuery();
            }
            cmd.Connection.Close();
        }
        return true;
    }

    protected bool UpdatePermissions()
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "update dbo.PermEmpleados set Facturas = @facturas, Reembolso = @reembolso, Anticipo = @anticipo, Tarjeta = @tarjeta, GMedicos = @gmedicos  WHERE UserKey = @idUser;";
            cmd.Parameters.Add("@idUser", SqlDbType.Int).Value = HttpContext.Current.Session["idUser"];
            cmd.Parameters.Add("@facturas", SqlDbType.Int).Value = chkFacturas.Visible ? chkFacturas.Checked : false;
            cmd.Parameters.Add("@reembolso", SqlDbType.Int).Value = chkReembolso.Checked;
            cmd.Parameters.Add("@anticipo", SqlDbType.Int).Value = chkAnticipo.Checked;
            cmd.Parameters.Add("@tarjeta", SqlDbType.Int).Value = chkTarjeta.Checked;
            cmd.Parameters.Add("@gmedicos", SqlDbType.Int).Value = chkMédicos.Checked;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
        return true;
    }

    protected bool RevisaLista() 
    {
        List<CheckBox> lista = new List<CheckBox>
        {
            chkTarjeta,
            chkAnticipo,
            chkFacturas,
            chkReembolso,
            chkMédicos
        };

        return lista.Any(x => x.Checked == true); 
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

          //  if (GridView1.Rows.Count >=1)
          //  {
          //  foreach (GridViewRow gvr in GridView1.Rows)
          //  {
          //      //if (gvr.Cells[2].Text.ToString() == EmailT.Text && gvr.Cells[4].Text.ToString() == EmpersasT.SelectedItem.ToString())
          //      if (gvr.Cells[2].Text.ToString() == EmailT.Text)
          //      {
          //          rev = false;
          //      }
          //  }
          //}
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
        TextBoxSearchEmail.Text = "";
        TextBoxSearchName.Text = "";
        LimpiarLista();
        UsersR.ClearSelection();
        //EmailC(GridView1,1);
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

    protected Dictionary<string,bool> get_permisions(int userkey)
    {
        var dict = new Dictionary<string, bool>();
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
        {
            SqlCommand cmd = new SqlCommand("SELECT Facturas,Reembolso,Anticipo,Tarjeta,GMedicos FROM dbo.PermEmpleados where UserKey = @UserKey", conn);
            cmd.Parameters.AddWithValue("@UserKey", userkey); 
            conn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dict.Add("Facturas", rdr.GetBoolean(0));
                dict.Add("Reembolso", rdr.GetBoolean(1));
                dict.Add("Anticipo", rdr.GetBoolean(2));
                dict.Add("Tarjeta", rdr.GetBoolean(3));
                dict.Add("GMedicos", rdr.GetBoolean(4));               
            }
            conn.Close();
        }
        return dict;
    }

    protected void GridView2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        try
        {

            int i, x;
            i = e.RowIndex;
            //Llenar los campos del usuario para edicion

            GridViewRow gvr = GridView2.Rows[i];
            int userkey = int.Parse(gvr.Cells[1].Text);
            //idUser = userkey;
            HttpContext.Current.Session["idUser"] = userkey;
            NombreT.Text = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
            EmailT.Text = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
            HttpContext.Current.Session["emailUser"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
            UsersR.SelectedValue = HttpUtility.HtmlDecode(gvr.Cells[6].Text.ToString());
            HttpContext.Current.Session["roleUser"] = HttpUtility.HtmlDecode(gvr.Cells[6].Text.ToString());

            Dictionary<string, bool> permisos = get_permisions(userkey);
            int iv = 0;

            //x = GridView1.Rows.Count;
            chkAnticipo.Checked = permisos.FirstOrDefault(d => d.Key == "Anticipo").Value;
            chkReembolso.Checked = permisos.FirstOrDefault(d => d.Key == "Reembolso").Value;
            chkFacturas.Checked = permisos.FirstOrDefault(d => d.Key == "Facturas").Value;
            chkMédicos.Checked = permisos.FirstOrDefault(d => d.Key == "GMedicos").Value;
            chkTarjeta.Checked = permisos.FirstOrDefault(d => d.Key == "Tarjeta").Value;

            if (gvr.Cells[6].Text.ToString() != "8")
                chkFacturas.Visible = false;
            else
                chkFacturas.Visible = true;
            //if (x == 1)
            //{
            //    GridView1.DataSource = "";
            //}
            //else
            //{
            //    foreach (GridViewRow gvr in GridView1.Rows)
            //    {
            //        if (gvr.RowIndex != i)
            //        {
            //            DataRow dr = dt.NewRow();

            //            CheckBox Checka = (CheckBox)gvr.Cells[4].FindControl("Check1");
            //            CheckBox Checke = (CheckBox)gvr.Cells[5].FindControl("Check2");
            //            CheckBox Checki = (CheckBox)gvr.Cells[6].FindControl("Check3");
            //            CheckBox Checko = (CheckBox)gvr.Cells[7].FindControl("Check4");
            //            CheckBox Checku = (CheckBox)gvr.Cells[8].FindControl("Check5");

            //            dr["Nombre"] = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
            //            dr["Correo"] = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
            //            dr["Rol"] = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());
            //            //dr["Empresa"] = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());
            //            if (HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Empleado")
            //            {
            //                Lista[iv] = Checka.Checked.ToString() + "," + Checke.Checked.ToString() + "," + Checki.Checked.ToString() + "," + Checko.Checked.ToString() + "," + Checku.Checked.ToString();
            //                iv++;
            //            }

            //            dt.Rows.Add(dr);

            //        }
            //    }
            //}

            //GridView1.DataSource = dt;
            //GridView1.DataBind();

            iv = 0;
            //foreach (GridViewRow gvr in GridView1.Rows)
            //{
            //    CheckBox Checka = (CheckBox)gvr.Cells[4].FindControl("Check1");
            //    CheckBox Checke = (CheckBox)gvr.Cells[5].FindControl("Check2");
            //    CheckBox Checki = (CheckBox)gvr.Cells[6].FindControl("Check3");
            //    CheckBox Checko = (CheckBox)gvr.Cells[7].FindControl("Check4");
            //    CheckBox Checku = (CheckBox)gvr.Cells[8].FindControl("Check5");

            //    if (HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Empleado")
            //    {
            //        string[] valores = Lista[iv].Split(',');
            //        Checka.Checked = Convert.ToBoolean(valores[0].ToString());
            //        Checke.Checked = Convert.ToBoolean(valores[1].ToString());
            //        Checki.Checked = Convert.ToBoolean(valores[2].ToString());
            //        Checko.Checked = Convert.ToBoolean(valores[3].ToString());
            //        Checku.Checked = Convert.ToBoolean(valores[4].ToString());
            //        iv++;
            //    }
            //    else 
            //    {
            //        Checka.Visible = false;
            //        Checke.Visible = false;
            //        Checki.Visible = false;
            //        Checko.Visible = false;
            //        Checku.Visible = false;
            //    }

            //}

            //NombreT.Text = "";
            //EmailT.Text = "";
            //LimpiarLista();
            //UsersR.SelectedValue = "2";
            //NombreT.Focus();

        }
        catch (Exception v)
        {

        }
    }

    protected void CharginUserName(object sender, EventArgs e)
    {
        try
        {
            var nombre = TextBoxSearchName.Text;

            var user = new UserEditDTO();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("SELECT u.UserKey, u.UserID, u.UserName, uir.RoleKey, dbo.Roles.RoleID, u.CreateDate, u.UpdateDate, (select us.Email from AspNetUsers AS us where us.UserKey = u.UserKey) FROM dbo.Users AS u INNER JOIN dbo.UsersInRoles AS uir ON u.UserKey = uir.UserKey INNER JOIN dbo.Roles ON uir.RoleKey = dbo.Roles.RoleKey WHERE(u.Status = 1 and RoleID <> 'Proveedor' and UserName like @data)", conn);
                cmd.Parameters.Add("@data", SqlDbType.VarChar).Value = nombre;
                cmd.Connection.Open();
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    user.UserKey = lector.GetInt32(0);
                    user.UserID = lector.GetString(1);
                    user.UserName = lector.GetString(2);
                    user.RoleKey = lector.GetInt32(3);
                    user.Rol = lector.GetString(4);
                    user.CreateDate = !lector.IsDBNull(5) ? (DateTime?)lector.GetDateTime(5) : null;
                    user.UpdateDate = !lector.IsDBNull(6) ? (DateTime?)lector.GetDateTime(6) : null;
                    user.Email = !lector.IsDBNull(7) ? lector.GetString(7) : lector.GetString(1);
                }

            }
            if (user.Email != null)
            {
                NombreT.Text = user.UserName;
                EmailT.Text = user.Email;
                UsersR.SelectedValue = user.RoleKey.ToString();
                HttpContext.Current.Session["idUser"] = user.UserKey;
                HttpContext.Current.Session["emailUser"] = user.Email;
                HttpContext.Current.Session["roleUser"] = user.RoleKey.ToString();
                Dictionary<string, bool> permisos = get_permisions(user.UserKey);

                chkAnticipo.Checked = permisos.FirstOrDefault(d => d.Key == "Anticipo").Value;
                chkReembolso.Checked = permisos.FirstOrDefault(d => d.Key == "Reembolso").Value;
                chkFacturas.Checked = permisos.FirstOrDefault(d => d.Key == "Facturas").Value;
                chkMédicos.Checked = permisos.FirstOrDefault(d => d.Key == "GMedicos").Value;
                chkTarjeta.Checked = permisos.FirstOrDefault(d => d.Key == "Tarjeta").Value;
            }
            else
            {
                NombreT.Text = "";
                EmailT.Text = "";
                UsersR.SelectedValue = "5";

                chkAnticipo.Checked = false;
                chkReembolso.Checked = false;
                chkFacturas.Checked = false;
                chkMédicos.Checked = false;
                chkTarjeta.Checked = false;
            }


        }
        catch (Exception v)
        {

        }
    }

    protected void CharginEmail(object sender, EventArgs e)
    {
        try
        {
            var email = TextBoxSearchEmail.Text;

            var user = new UserEditDTO();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("SELECT u.UserKey, u.UserID, u.UserName, uir.RoleKey, dbo.Roles.RoleID, u.CreateDate, u.UpdateDate, (select us.Email from AspNetUsers AS us where us.UserKey = u.UserKey) FROM dbo.Users AS u INNER JOIN dbo.UsersInRoles AS uir ON u.UserKey = uir.UserKey INNER JOIN dbo.Roles ON uir.RoleKey = dbo.Roles.RoleKey WHERE(u.Status = 1 and RoleID <> 'Proveedor' and u.UserID like @data)", conn);
                cmd.Parameters.Add("@data", SqlDbType.VarChar).Value = email;
                cmd.Connection.Open();
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    user.UserKey = lector.GetInt32(0);
                    user.UserID = lector.GetString(1);
                    user.UserName = lector.GetString(2);
                    user.RoleKey = lector.GetInt32(3);
                    user.Rol = lector.GetString(4);
                    user.CreateDate = !lector.IsDBNull(5) ? (DateTime?)lector.GetDateTime(5) : null;
                    user.UpdateDate = !lector.IsDBNull(6) ? (DateTime?)lector.GetDateTime(6) : null;
                    user.Email = !lector.IsDBNull(7) ? lector.GetString(7) : lector.GetString(1);
                }

            }
            if (user.Email != null)
            {
                NombreT.Text = user.UserName;
                EmailT.Text = user.Email;
                UsersR.SelectedValue = user.RoleKey.ToString();
                HttpContext.Current.Session["idUser"] = user.UserKey;
                HttpContext.Current.Session["emailUser"] = user.Email;
                HttpContext.Current.Session["roleUser"] = user.RoleKey.ToString();
                Dictionary<string, bool> permisos = get_permisions(user.UserKey);

                chkAnticipo.Checked = permisos.FirstOrDefault(d => d.Key == "Anticipo").Value;
                chkReembolso.Checked = permisos.FirstOrDefault(d => d.Key == "Reembolso").Value;
                chkFacturas.Checked = permisos.FirstOrDefault(d => d.Key == "Facturas").Value;
                chkMédicos.Checked = permisos.FirstOrDefault(d => d.Key == "GMedicos").Value;
                chkTarjeta.Checked = permisos.FirstOrDefault(d => d.Key == "Tarjeta").Value;
            }
            else
            {
                NombreT.Text = "";
                EmailT.Text = "";
                UsersR.SelectedValue = "5";

                chkAnticipo.Checked = false;
                chkReembolso.Checked = false;
                chkFacturas.Checked = false;
                chkMédicos.Checked = false;
                chkTarjeta.Checked = false;
            }


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

    //protected void btn_EnviarTsy(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        string Nombre, Email, RolT,Lista;
    //        Lista = "";
    //        int Contador = 0;
    //        int Total = GridView1.Rows.Count;
    //        if (GridView1.Rows.Count >= 1)
    //        {
    //            foreach (GridViewRow gvr in GridView1.Rows)
    //            {
    //                Nombre = HttpUtility.HtmlDecode(gvr.Cells[1].Text.ToString());
    //                Email = HttpUtility.HtmlDecode(gvr.Cells[2].Text.ToString());
    //                RolT = HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString());

    //                //Empresa = HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString());

    //                //string key = User.Identity.Name.ToString();
    //                string key = HttpContext.Current.Session["UserKey"].ToString();

    //                // Generate a new 12-character password with at least 1 non-alphanumeric character.
    //                ApplicationDbContext context = new ApplicationDbContext();
    //                UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
    //                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
    //                //UserValidator<ApplicationUser> Val = new UserValidator<ApplicationUser>(UserManager);
    //                //Val.AllowOnlyAlphanumericUserNames = false;


    //                string password = CrearPassword(8);
    //                string hashedNewPassword = UserManager.PasswordHasher.HashPassword(password);


                    
    //                //Generate User
    //                var manager = new UserManager();
    //                //manager.UserValidator = Val;
    //                var newuser = new ApplicationUser() { UserName = Email };
                    

    //                IdentityResult result = manager.Create(newuser, password);
    //                if (result.Succeeded)
    //                //if(userName1.ToString() =! "")
    //                {
    //                    manager.AddToRole(newuser.Id.ToString(), RolT);
    //                    string userid = newuser.Id.ToString();
    //                    string cadena = "";
    //                    try
    //                    {

    //                        if (HttpUtility.HtmlDecode(gvr.Cells[3].Text.ToString()) == "T|SYS| - Empleado")
    //                        {
    //                            CheckBox Checka = (CheckBox)gvr.Cells[4].FindControl("Check1");
    //                            CheckBox Checke = (CheckBox)gvr.Cells[5].FindControl("Check2");
    //                            CheckBox Checki = (CheckBox)gvr.Cells[6].FindControl("Check3");
    //                            CheckBox Checko = (CheckBox)gvr.Cells[7].FindControl("Check4");
    //                            CheckBox Checku = (CheckBox)gvr.Cells[8].FindControl("Check5");
    //                            cadena = Checka.Checked.ToString() + "," + Checke.Checked.ToString() + "," + Checki.Checked.ToString() + "," + Checko.Checked.ToString() + "," + Checku.Checked.ToString();
    //                            cadena = cadena.Replace("True", "1").Replace("False", "0");
    //                        }
    //                    }
    //                    catch (Exception ex)
    //                    {

    //                    }

    //                    if (RegistroTsys(Nombre, Email, password, key, RolT, userid,cadena) == true)
    //                    {
    //                        Contador = Contador + 1;
    //                    }
    //                    else
    //                    {
    //                        Lista = Lista + Email + ",";
    //                    }
    //                }
    //                else
    //                {
    //                    Lista = Lista + Email + ",";
    //                }
    //            }

    //            if (Lista == "")
    //            {
    //                ListEm(GridView1, 1);
    //                string titulo, Msj, tipo;
    //                DataTable dts = new DataTable();
    //                GridView1.DataSource = dts;
    //                GridView1.DataBind();
    //                tipo = "success";
    //                Msj = "Registro Exitoso";
    //                titulo = "T|SYS|";
    //                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
    //            }
    //            else
    //            {
    //                string titulo, Msj, tipo;
    //                tipo = "error";
    //                Msj = "Algunos Datos son Invalidos no se puedieron registrar los correos: " + Lista + " Verifica que no se hayan registrado anteriormente";
    //                titulo = "T|SYS| - Error de Ejecucion SP";

    //                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //}

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


    protected void Excel(object sender, EventArgs e)
    {

        try
        { 

        if (FileUpload1.HasFile)
        {

            DataTable dts = new DataTable();
            dts.Columns.Add("No");
            dts.Columns.Add("ID");
            dts.Columns.Add("Nombre");
            dts.Columns.Add("Company");
            dts.Columns.Add("Email");
            dts.Columns.Add("Notif");
            dts.Columns.Add("Anot");

            using (XLWorkbook workBook = new XLWorkbook(FileUpload1.PostedFile.InputStream))
            {
                var workbook = new XLWorkbook(FileUpload1.PostedFile.InputStream);
                IXLWorksheet workSheet = workBook.Worksheet(1);
                DataTable dt = new DataTable();

                bool firstRow = true;
                string Col = string.Empty;
                int Cont = 0;
                foreach (IXLRow row in workSheet.Rows())
                {
                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            dt.Columns.Add(cell.Value.ToString());
                        }

                        if (dt.Columns[0].ColumnName != "PrimaryCntctKey") { Col = "Celda A1 debe ser columna PrimaryCntctKey"; }
                        else if (dt.Columns[1].ColumnName != "VendID") { Col = "Celda A2 debe ser columna VendID"; }
                        else if (dt.Columns[2].ColumnName != "CompanyID") { Col = "Celda A3 debe ser columna CompanyID"; }
                        else if (dt.Columns[3].ColumnName != "VendName") { Col = "Celda A4 debe ser columna VendName"; }
                        else if (dt.Columns[4].ColumnName != "Correo") { Col = "Celda A5 debe ser columna Correo"; }

                        if (Col != "")
                        {
                            dt = null;
                            string titulo, Msj, tipo;
                            tipo = "error";
                            Msj = Col;
                            titulo = "T|SYS| - Error de Lectura Excel";
                            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                            return;
                        }
                        else
                        {
                            firstRow = false;
                        }
                    }
                    else
                    {
                        dt.Rows.Add();
                        int i = 0;
                        Cont = Cont + 1;
                        int Fila = 0;
                        DataRow rowt = dts.NewRow();


                        foreach (IXLCell cell in row.Cells())
                        {
                            Fila = dt.Rows.Count - 1;
                            dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                            i++;
                        }

                        string Primary = dt.Rows[Fila][0].ToString();
                        string VendID = dt.Rows[Fila][1].ToString();
                        string Company = dt.Rows[Fila][2].ToString();
                        string VendName = dt.Rows[Fila][3].ToString();
                        string Correo = dt.Rows[Fila][4].ToString();

                        if (Correo != "")
                        {
                            if (Email_Ok(Correo) == true)
                            {
                                ApplicationDbContext context = new ApplicationDbContext();
                                UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
                                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
                                UserValidator<ApplicationUser> Val = new UserValidator<ApplicationUser>(UserManager);
                                Val.AllowOnlyAlphanumericUserNames = false;

                                string password = Membership.GeneratePassword(12, 1);
                                password = CrearPassword(8);
                                string hashedNewPassword = UserManager.PasswordHasher.HashPassword(password);
                                string key = User.Identity.Name.ToString();
                                string Rol = "Proveedor";

                                var manager = new UserManager();
                                manager.UserValidator = Val;
                                var newuser = new ApplicationUser() { UserName = Correo };

                                IdentityResult result = manager.Create(newuser, password);
                                if (result.Succeeded)
                                {
                                    manager.AddToRole(newuser.Id.ToString(), Rol);
                                    string userid = newuser.Id.ToString();

                                    if (RegistroEx(VendID, VendName, Correo, hashedNewPassword, key, Company, Rol, userid) == true)
                                    {
                                        //string SenM = Emailexc(Correo, Company);
                                        string SenM = "Ok";
                                        string Not = "OK";
                                        string Anot = "Registro exitoso";

                                        if (SenM != "Ok") { Not = "X"; Anot = "Registro exitoso, error al enviar Correo"; }
                                        rowt["No"] = Cont;
                                        rowt["ID"] = VendID;
                                        rowt["Nombre"] = VendName;
                                        rowt["Company"] = Company;
                                        rowt["Email"] = Correo;
                                        rowt["Notif"] = Not;
                                        rowt["Anot"] = Anot;
                                        dts.Rows.Add(rowt);
                                    }
                                }
                                else
                                {

                                    rowt["No"] = Cont;
                                    rowt["ID"] = VendID;
                                    rowt["Nombre"] = VendName;
                                    rowt["Company"] = Company;
                                    rowt["Email"] = Correo;
                                    rowt["Notif"] = "X";
                                    rowt["Anot"] = "Error al registrar , usuario ya registrado";
                                    dts.Rows.Add(rowt);
                                }
                            }
                            else
                            {
                                rowt["No"] = Cont;
                                rowt["ID"] = VendID;
                                rowt["Nombre"] = VendName;
                                rowt["Company"] = Company;
                                rowt["Email"] = Correo;
                                rowt["Notif"] = "X";
                                rowt["Anot"] = "Formato de Email Inocrrecto";
                                dts.Rows.Add(rowt);
                            }
                        }


                    }  // Cierra Else
                }
            }

            if (dts.Rows.Count >= 1)
            {
                GridView3.DataSource = dts;
                GridView3.DataBind();

            }
        }
        else
        {
                GridView3.DataSource = "";
                GridView3.DataBind();
                string titulo, Msj, tipo;
                tipo = "error";
                Msj = "Selecciona un archivo para iniciar la Carga ";
                titulo = "T|SYS| - Error de Ejecucion SP";
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }

    }
        catch (Exception ex)
        {
            string titulo, Msj, tipo;
            tipo = "error";
            Msj = "Algunos datos son Invalidos, verificalos con SAGE,en caso de persistir el problema comúnicate con el Area de Sistemas ";
            titulo = "T|SYS| - Error de Ejecucion SP";
            ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        }

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

    protected void btn_anterior_Click(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["page"] == null)
        {
            HttpContext.Current.Session["page"] = 1;
        }
        int count = 0;
        int page = (int)HttpContext.Current.Session["page"];
        page = page - 1;
        var users = Get_Users(page, out count);
        HttpContext.Current.Session["page"] = page;
        GridView2.DataSource = users;
        HttpContext.Current.Session["count"] = count;
        int abs = Math.Abs(count / 10);
        int pages = (count % 100) == 0 ? abs : abs + 1;
        btn_siguiente.Enabled = pages > page;
        btn_anterior.Enabled = page > 1;
        lbl_pages.Text = string.Format("Pagina {0} de {1}", page, pages);
        GridView2.DataBind();
    }

    protected void btn_siguiente_Click(object sender, EventArgs e)
    {        
        if(HttpContext.Current.Session["page"]==null)
        {
            HttpContext.Current.Session["page"] = 1;
        }
        int count = 0;
        int page = (int)HttpContext.Current.Session["page"];
        page = page + 1;
        var users = Get_Users(page, out count);
        HttpContext.Current.Session["page"] = page;
        GridView2.DataSource = users;
        HttpContext.Current.Session["count"] = count;
        int abs = Math.Abs(count / 10);
        int pages = (count % 100) == 0 ? abs : abs + 1;
        btn_siguiente.Enabled = pages > page;
        btn_anterior.Enabled = page > 1;
        lbl_pages.Text = string.Format("Pagina {0} de {1}", page, pages);
        GridView2.DataBind();
    }
}