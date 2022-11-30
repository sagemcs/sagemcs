//PORTAL DE PROVEDORES T|SYS|
//08 ABRIL, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA P
//PANTALLA PARA CARGA DE DOCUMENTOS DE PROVEEDORES

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Carga_Doc : System.Web.UI.Page
{
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
                Context.GetOwinContext().Authentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
            else
            {
                if (HttpContext.Current.Session["RolUser"].ToString() == "Proveedor")
                {
                    if (UsuarioP.Items.Count == 0)
                    {
                        string Compnay = HttpContext.Current.Session["IDCompany"].ToString();
                        //User_Empresas(UsuarioP);
                        User_Company(UsuarioP);
                        Comprueba();
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

    private int RevDocs()
    {
        int Msj;
        try
        {
            string sql;
            string Cuenta;
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            string Vendk = HttpContext.Current.Session["VendKey"].ToString();
            string company = HttpContext.Current.Session["IDCompany"].ToString();
            sqlConnection1.Open();
            sql = @"Select UpdateDate From Documents where DocID = 9 And VendorKey = " + Vendk + " And CompanyID = '" + company + "'";
            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            DateTime Hoy = DateTime.Now.Date;    //Fecha Hoy 
            DateTime UpdateDoc = Convert.ToDateTime(Cuenta).Date;  // Fecha ultima Actualización
            DateTime UpdateL = UpdateDoc.AddMonths(6); // Fecha ultima Actualización
            DateTime UpdateL1 = Hoy.AddMonths(-6); // Fecha ultima Actualización


            if (UpdateL1 > UpdateDoc)
            {
                if (UpdateD(Vendk, company) == true)
                { Msj = 11; }
                else { Msj = -1; }
            }
            else
            {
                DateTime DateDays = UpdateL.AddDays(-10);
                TimeSpan Days = UpdateL - Hoy;
                int Dias = Days.Days;
                if (Hoy >= DateDays)
                {
                    if (UpdateD(Vendk, company) == true)
                    { Msj = Dias; }
                    else { Msj = -1; }
                }
                else
                {
                    Msj = 0;
                }
            }

            // if (Hoy >= UpdateL)
            // {
            //     if (UpdateD(Vendk, company) == true)
            //     {Msj = 11;} else {Msj = -1; }           
            // }
            // else
            // {
            //     DateTime DateDays = UpdateL.AddDays(-10);
            //     TimeSpan Days = UpdateL - Hoy;
            //     int Dias = Days.Days;
            //     if (Hoy >= DateDays)
            //     {
            //         if (UpdateD(Vendk, company) == true)
            //         { Msj = Dias;}
            //         else { Msj = -1; }   
            //     }
            //     else
            //     {
            //         Msj = 0;
            //     }
            //}
            sqlConnection1.Close();
        }
        catch (Exception)
        {
            Msj = -1;
        }
        return Msj;
    }

    private bool UpdateD(string Vendk, string Company)
    {
        bool ret = false;
        try
        {

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "UPDATE Documents SET Status = 1 where DocID = 9 And VendorKey = " + Vendk + " And CompanyID = '" + Company + "'";
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.ExecuteNonQuery();
                ret = true;
            }


        }
        catch
        {

        }
        return ret;
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

            //bool Adenda = Global.VerTipoProv();
            //if (Adenda == true)
            //{
            //    //HttpContext.Current.Session.RemoveAll();
            //    //Context.GetOwinContext().Authentication.SignOut();
            //    Response.Redirect("~/Logged/Proveedores/Carga_Doc_Prov.aspx", false);
            //}

            //bool Extranjero = Global.VerTipoProvE();
            //if (Extranjero == true) 
            //{
            //    //HttpContext.Current.Session.RemoveAll();
            //    //Context.GetOwinContext().Authentication.SignOut();
            //    Response.Redirect("~/Logged/Proveedores/Carga_Doc_Extranjeros.aspx",false);
            //}

            Global.Docs();
            //string Valor = HttpContext.Current.Session["Docs"].ToString();

            int Valor = Convert.ToInt16(HttpContext.Current.Session["Docs"].ToString());
            int Dias = Valor;
            if (Dias == 0)
            {
                Page.MasterPageFile = "MenuP.master";
            }
            else if (Dias < 0)
            {
                Page.MasterPageFile = "MenuP.master";
            }
            else if (Dias == 30)
            {
                Page.MasterPageFile = "MenuPreP.master";
            }
            else if (Dias == 25)
            {
                Page.MasterPageFile = "MenuP.master";
            }
            else if (Dias == 26)
            {
                Page.MasterPageFile = "MenuP.master";
            }
            else if (Dias == 27)
            {
                Page.MasterPageFile = "MenuP.master";
            }
            else if (Dias == 28)
            {
                Page.MasterPageFile = "MenuPreP.master";
            }
            else if (Dias == 22)
            {
                Page.MasterPageFile = "MenuP.master";
            }
            else if (Dias <= 10 && Dias > 0)
            {
                Page.MasterPageFile = "MenuPreP.master";
            }
            else if (Dias > 10)
            {
                Page.MasterPageFile = "MenuPreP.master";
            }
            //    if (HttpContext.Current.Session["Docs"].ToString() == "1")
            //{
            //        Global.RevDocs();
            //        if ((HttpContext.Current.Session["Docs"].ToString() == "1"))
            //        {
            //            //Page.MasterPageFile = "MenuP.master";
            //            Page.MasterPageFile = "MenuP.master";
            //            //Response.Redirect("~/Logged/Proveedores/Default.aspx", false);

            //        }
            //        else
            //        {
            //            Page.MasterPageFile = "MenuPreP.master";
            //        }
            //        //Page.MasterPageFile = "MenuP.master";  //Poner el Menu de Solo Carga de Docs
            //}

            //if (HttpContext.Current.Session["Docs"].ToString() == "0")
            //{
            //    Page.MasterPageFile = "MenuP.master";  //Poner el Menu de Solo Carga de Docs
            //}



            //else if ((HttpContext.Current.Session["Status"].ToString() == "Activo" && HttpContext.Current.Session["Docs"].ToString() != "0"))
            //{
            //        string titulo, Msj, tipo;
            //        int Dias = RevDocs();
            //        if (Dias == 0)
            //        {
            //            HttpContext.Current.Session["UpDoc"] = "0";
            //            //Page.MasterPageFile = "MenuPreP.master";
            //        }
            //        //else if (Dias == 11)
            //        //{
            //        //    HttpContext.Current.Session["UpDoc"] = "1";
            //        //    tipo = "error";
            //        //    Msj = "Has superado el tiempo límite para la actualización de Opinión de Cumplimiento de Obligaciones Fiscales, Deberás cargarlo cuanto antes para seguir teniendo acceso a las actividades del portal";
            //        //    titulo = "Notificación de actualización de documentos";
            //        //    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            //        //    Page.MasterPageFile = "MenuP.master";
            //        //}
            //        //else if (Dias == -1)
            //        //{
            //        //    tipo = "error";
            //        //    Msj = "Error el Obtener datos";
            //        //    titulo = "Notificación de actualización de documentos";
            //        //    //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            //        //}
            //        //else
            //        //{
            //        //    HttpContext.Current.Session["UpDoc"] = "0";
            //        //    tipo = "warning";
            //        //    Msj = "Te recordamos tienes " + Dias + " para actualizar tu Opinión de Cumplimiento de Obligaciones Fiscales, ya que de lo contrario no podrás tener acceso a las actividades del portal";
            //        //    titulo = "Notificación de actualización de documentos";
            //        //    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            //        //    Page.MasterPageFile = "MenuPreP.master";
            //        //}

            //        if (HttpContext.Current.Session["UpDoc"].ToString() == "1") { Page.MasterPageFile = "MenuP.master"; }
            //        else { Page.MasterPageFile = "MenuPreP.master"; }
            //    }



        }
        catch(Exception ex)
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

    protected void Upload()
    {
        string titulo, Msj, tipo, Guardar, Company, Prove;
        Msj = string.Empty;
        Global.VarSesion(User.Identity.Name.ToString(), UsuarioP.SelectedItem.ToString());
        string Vedn = HttpContext.Current.Session["VendKey"].ToString();

        int Docs = ConsultaDocs();
        if (Docs == 0)
        {
                    
            try
            {
                int Contt = 0;
                Company = UsuarioP.SelectedItem.ToString();
                Prove = User.Identity.Name.ToString();

                Guardar = Execute("5", FileUpload1, Company, Prove, "", 1).ToString();  //Acta Constitutiva   5
                if (Guardar != "1")
                {
                    Contt = Contt + 1;
                    Msj = "Error al cargar ACTA CONSTITUTIVA , Verifica el Archivo";
                }
                Guardar = Execute("6", FileUpload2, Company, Prove, "", 1).ToString(); //RFC   6
                if (Guardar != "1")
                {
                    Contt = Contt + 1;
                    Msj = "Error al cargar REGISTRO FEDEREAL DEL CONTRIBUYENTE, Verifica el Archivo";
                }
                Guardar = Execute("7", FileUpload3, Company, Prove, "", 1).ToString(); //Poder Notarial  7
                if (Guardar != "1")
                {
                    Contt = Contt + 1;
                    Msj = "Error al cargar PODER NOTARIAL , Verifica el Archivo";
                }
                Guardar = Execute("8", FileUpload4, Company, Prove, "", 1).ToString(); //IFE 8
                if (Guardar != "1")
                {
                    Contt = Contt + 1;
                    Msj = "Error al cargar IDENTIFICACIÓN OFICIAL , Verifica el Archivo";
                }
                Guardar = Execute("9", FileUpload5, Company, Prove, "", 1).ToString(); //Obligaciones Fiscales   9
                if (Guardar != "1")
                {
                    Contt = Contt + 1;
                    Msj = "Error al cargar OPINIÓN DE OBLIGACIONES FÍSCALES , Verifica el Archivo";
                }
                Guardar = Execute("10", FileUpload6, Company, Prove, "", 1).ToString(); //Carta Instruccion      10
                if (Guardar != "1")
                {
                    Contt = Contt + 1;
                    Msj = "Error al cargar CARTA INSTRUCCIÓN , Verifica el Archivo";
                }
                Guardar = Execute("11", FileUpload7, Company, Prove, "", 1).ToString(); //Contrato       11
                if (Guardar != "1")
                {
                    Contt = Contt + 1;
                    Msj = "Error al cargar CONTRATO Y/O CARTA DE PRESTACIÓN , Verifica el Archivo";
                }

                Guardar = Execute("13", FileUpload9, Company, Prove, "", 1).ToString(); //Contrato       11
                if (Guardar != "1")
                {
                    Contt = Contt + 1;
                    Msj = "Error al cargar CARTA DE NO ADEUDO , Verifica el Archivo";
                }

                Guardar = Execute("14", FileUpload8, Company, Prove, "", 1).ToString(); //Contrato       11
                if (Guardar != "1")
                {
                    Contt = Contt + 1;
                    Msj = "Error al cargar TERMINOS Y CONDICIONES , Verifica el Archivo";
                }

                if (Contt >= 1)
                {
                    tipo = "error";
                    titulo = "Error Al Cargar Documentos";
                    Guardar = Execute("11", FileUpload1, Company, Prove, "", 0).ToString();
                    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
                else
                {
                    //ListEm();
                    tipo = "success";
                    Msj = "Has completado la carga de documentos exitosamente, en cuanto el administrador los apruebe, se te notificará por correo electrónico para seguir con el proceso de registro, Gracias";
                    titulo = "Carga de Documentos Exitosa";
                    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
            }
            catch
            {

            }
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
            Label4.Text = HttpContext.Current.Session["Error"].ToString();
            // ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alert(B6);", true);

        }
    }

    public string Execute(string KeyFile,FileUpload Caja,string Company,string Prove,string Desc,int Opcion)
    {
        string res;
        
        try
        {
            string Filename = string.Empty;
            byte[] bytes3 = null;
            try
            {
                Filename = Caja.FileName.ToString();
                //Fabian
                Stream fsr = Caja.PostedFile.InputStream;
                System.IO.BinaryReader br3 = new System.IO.BinaryReader(fsr);
                bytes3 = br3.ReadBytes((Int32)fsr.Length);
            }
            catch (Exception ex)
            {
                int Log = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
                int User = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
                string Companys = HttpContext.Current.Session["IDCompany"].ToString();
                LogError(Log, User, "Error de Conversion del Documento a Binario", ex.Message, Companys);
            }
            

            string Ext = ".pdf";
            string Var1 = HttpContext.Current.Session["IDComTran"].ToString();
            string Var2 = HttpContext.Current.Session["VendKey"].ToString();
            string Var3 = HttpContext.Current.Session["UserKey"].ToString();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                string Cades = "spInsertDoc";
                string Result = string.Empty;
                SqlCommand cmd = new SqlCommand(Cades, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@FKey", Value = KeyFile });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@File", Value = bytes3 });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@FileNAme", Value = Filename });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@EmpresaC", Value = HttpContext.Current.Session["IDComTran"].ToString() });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Vendedor", Value = HttpContext.Current.Session["VendKey"].ToString() });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@UserKey", Value = HttpContext.Current.Session["UserKey"].ToString() });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Ext", Value = Ext });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Desc", Value = Desc });

                cmd.Parameters.Add(new SqlParameter()
                { ParameterName = "@Opcion", Value = Opcion });


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
        catch(Exception Rt)
        {
            int Log = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
            int User = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());
            string Companys = HttpContext.Current.Session["IDCompany"].ToString();
            LogError(Log, User, "Error de Conversion del Documento a Binario", Rt.Message, Companys);
            res = Rt.Message;
            return res;
        }  
    }

    protected void Revisa()
    {
        try
        {
            string VB = ID.Text;
            string Cadena = string.Empty;

            string Flw = FileUpload1.FileName.ToString();


            if (FileUpload1.PostedFile.ContentType.ToString() != "application/pdf")
            {
                Label4.Text = "Acta Constitutiva, Archivo debe ser PDF";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload2.PostedFile.ContentType.ToString() != "application/pdf")
            {
                Label4.Text = "Registro Federal del Contribuyente, Archivo debe ser PDF";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload3.PostedFile.ContentType.ToString() != "application/pdf")
            {
                Label4.Text = "Poder Notarial, Archivo debe ser PDF";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload4.PostedFile.ContentType.ToString() != "application/pdf")
            {
                Label4.Text = "Identificación Oficial, Archivo debe ser PDF";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;

            }

            if (FileUpload5.PostedFile.ContentType.ToString() != "application/pdf")
            {
                Label4.Text = "Opinion de Cumplimiento de Obligaciones Fiscales, Archivo debe ser PDF";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload6.PostedFile.ContentType.ToString() != "application/pdf")
            {
                Label4.Text = "Carta Instrucción, Archivo debe ser PDF";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload7.PostedFile.ContentType.ToString() != "application/pdf")
            {
                Label4.Text = "Contrato y/o Carta de Prestación, Archivo debe ser PDF";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload8.PostedFile.ContentType.ToString() != "application/pdf")
            {
                Label4.Text = "Terminos y Condiciones, Archivo debe ser PDF";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload9.PostedFile.ContentType.ToString() != "application/pdf")
            {
                Label4.Text = "Carta de No Adeudo, Archivo debe ser PDF";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }



            if (FileUpload1.FileBytes.Length < 1)
            {
                Label4.Text = "Acta Constitutiva, Archivo corrupto o dañado, favor de verificar";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload2.FileBytes.Length < 1)
            {
                Label4.Text = "Registro Federal del Contribuyente, Archivo corrupto o dañado, favor de verificar";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload3.FileBytes.Length < 1)
            {
                Label4.Text = "Poder Notarial, Archivo corrupto o dañado, favor de verificar";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload4.FileBytes.Length < 1)
            {
                Label4.Text = "Identificación Oficial, favor de verificar";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;

            }

            if (FileUpload5.FileBytes.Length < 1)
            {
                Label4.Text = "Opinion de Cumplimiento de Obligaciones Fiscales, Archivo corrupto o dañado, favor de verificar";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload6.FileBytes.Length < 1)
            {
                Label4.Text = "Carta Instrucción, Archivo corrupto o dañado, favor de verificar";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload7.FileBytes.Length < 1)
            {
                Label4.Text = "Contrato y/o Carta de Prestación, Archivo corrupto o dañado, favor de verificar";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload8.FileBytes.Length < 1)
            {
                Label4.Text = "Terminos y Condiciones, Archivo corrupto o dañado, favor de verificar";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }

            if (FileUpload9.FileBytes.Length < 1)
            {
                Label4.Text = "Carta de No Adeudo, Archivo corrupto o dañado, favor de verificar";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(B9);", true);
                return;
            }


            if (FileUpload1.FileName.ToString() == "" || FileUpload2.FileName.ToString() == "" || FileUpload3.FileName.ToString() == "" || FileUpload4.FileName.ToString() == "" || FileUpload5.FileName.ToString() == "" || FileUpload6.FileName.ToString() == "" || FileUpload7.FileName.ToString() == "" || FileUpload8.FileName.ToString() == "" || FileUpload9.FileName.ToString() == "")
            {
                Cadena = "B1";
            }
            else if (FileUpload1.PostedFile.ContentLength > 15000000)
            {
                Cadena = "B2";
            }
            else if (FileUpload2.PostedFile.ContentLength > 15000000)
            {
                Cadena = "B3";
            }
            else if (FileUpload3.PostedFile.ContentLength > 15000000)
            {
                Cadena = "B4";
            }
            else if (FileUpload4.PostedFile.ContentLength > 15000000)
            {
                Cadena = "B5";
            }
            else if (FileUpload5.PostedFile.ContentLength > 15000000)
            {
                Cadena = "B6";
            }
            else if (FileUpload6.PostedFile.ContentLength > 15000000)
            {
                Cadena = "B7";
            }
            else if (FileUpload7.PostedFile.ContentLength > 15000000)
            {
                Cadena = "B8";
            }
            else if (FileUpload8.PostedFile.ContentLength > 15000000)
            {
                Cadena = "B12";
            }
            else if (FileUpload9.PostedFile.ContentLength > 15000000)
            {
                Cadena = "B11";
            }

            if (Cadena != "")
            {
               ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alerte(" + Cadena + ");", true);
            }
            else
            {
              Upload();
            }
        }
        catch
        {
            //lblError.Text = "Database connection error. Please try again.";
        }

    }

    protected void Btn_Buscar(object sender, EventArgs e)
    {
        try
        {
            Revisa();
            Comprueba();
        }
        catch
        {
            //lblError.Text = "Database connection error. Please try again.";
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
                    System.Data.DataTable GV = new System.Data.DataTable();
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

    protected void User_Company(DropDownList Caja)
    {
        try
        {
            string sql;
            string Cuenta;
            Caja.Items.Clear();
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            int VK = Convert.ToInt16(HttpContext.Current.Session["VendKey"].ToString());
            string Company = HttpContext.Current.Session["IDCompany"].ToString();
            sqlConnection1.Open();

            sql = @"select top 1 CompanyID + ' - ' + CompanyName from Company Where CompanyID ='" + Company + "'";

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

    private int ConsultaDocs()
    {
        int res = -0;
        try
        {
            string sql;
            string Cuenta;
            
            string Compnay = HttpContext.Current.Session["IDCompany"].ToString();
            int VK = Convert.ToInt16(HttpContext.Current.Session["VendKey"].ToString());
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            sql = @"select COUNT(*) from Documents where VendorKey = '" + VK + "' and CompanyID = '" + Compnay + "'";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            res = Convert.ToInt32(Cuenta);

            return res;
        }
        catch (Exception ex)
        {
            //LogError(pLogKey, pUserKey, "Carga-Factura:ConsultaTipoArt", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            //HttpContext.Current.Session["Error"] = "Se generó un Erro al Intentar Obtener la Clase de Articulo, Comunicate con el Área de Sistemas para ofrecerte una Solución ";
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return res;
        }

    }

    private int ConsultaDocsNuevos()
    {
        int res = -0;
        try
        {
            string sql;
            string Cuenta;

            string Compnay = HttpContext.Current.Session["IDCompany"].ToString();
            int VK = Convert.ToInt16(HttpContext.Current.Session["VendKey"].ToString());
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();

            sql = @"select COUNT(*) from Documents where VendorKey = '" + VK + "' AND UpdateDate IS NULL And Status >=3";

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            sqlConnection1.Close();

            res = Convert.ToInt32(Cuenta);

            return res;
        }
        catch (Exception ex)
        {
            //LogError(pLogKey, pUserKey, "Carga-Factura:ConsultaTipoArt", ex.Message, pCompanyID);
            string err;
            err = ex.Message;
            //HttpContext.Current.Session["Error"] = "Se generó un Erro al Intentar Obtener la Clase de Articulo, Comunicate con el Área de Sistemas para ofrecerte una Solución ";
            //Label4.Text = HttpContext.Current.Session["Error"].ToString();
            return res;
        }

    }

    protected void Comprueba()
    {
        try
        {
            if (UsuarioP.SelectedItem.ToString() != "")
            {
                Global.VarSesion(User.Identity.Name.ToString(), UsuarioP.SelectedItem.ToString());
                int Cot = 0;
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spComprDoc", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@User", Value = HttpContext.Current.Session["VendKey"].ToString() });

                    cmd.Parameters.Add(new SqlParameter()
                    { ParameterName = "@Company", Value = HttpContext.Current.Session["IDCompany"].ToString() });

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                    conn.Open();
                    string Errores = string.Empty;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    System.Data.DataTable dt = new System.Data.DataTable();

                    dt.Columns.Add("Nombre");
                    dt.Columns.Add("NombreFile");
                    dt.Columns.Add("Fecha");
                    dt.Columns.Add("Fecha2");
                    dt.Columns.Add("Status");
                    //dt.Columns.Add("Coment");

                    while (rdr.Read())
                        {

                            DataRow row = dt.NewRow();
                            row["Nombre"] = rdr["Nombre"].ToString();
                            row["NombreFile"] = rdr["NombreFile"].ToString();
                            row["Fecha"] = rdr["Fecha"].ToString();
                            if (rdr["Fecha2"].ToString() == null)
                            {
                                row["Fecha2"] = "No Disponible";
                            }
                            else
                            {
                                row["Fecha2"] = rdr["Fecha2"].ToString();
                            }
                            row["Status"] = rdr["status"].ToString();
                            if (rdr["status"].ToString()== "Aprobado") { Cot = Cot + 1; }
                            if (rdr["status"].ToString() == "En Revisión") { Cot = Cot -1; }
                            if (rdr["status"].ToString() == "Rechazado") { Cot = Cot + 1; }
                        dt.Rows.Add(row);
                        }

                    if (dt.Rows.Count >= 1)
                    {
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                        Carga.Visible = false;
                        Info.Visible = true;
                    }
                    else
                    {
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                        Carga.Visible = true;
                        Info.Visible = false;
                    }

                    string Var = GridView1.Rows[0].Cells[4].Text.ToString();
                    int nv = 0;
                    if (Cot >= 1)
                    {
                        foreach (GridViewRow gvr in GridView1.Rows)
                        {
                            FileUpload Carga = (FileUpload)gvr.Cells[5].FindControl("BDoc");
                            System.Web.UI.WebControls.TextBox Caja = (System.Web.UI.WebControls.TextBox)gvr.Cells[5].FindControl("CDoc");

                            string un = gvr.Cells[4].Text.ToString();

                            if (HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString()) == "Aprobado" || HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString()) == "En Revisión")
                            {
                                Caja.Visible = false;
                                Carga.Visible = false;
                                nv = nv + 1;
                            }
                                
                        }
                    }
                    else
                    {
                        btnEnv.Visible = false;
                        GridView1.Columns[5].Visible = false;
                    }

                    if (nv >=9)
                    {
                        btnEnv.Visible = false;
                    }

                }
            }
        }
        catch(Exception ex)
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

            GridViewRow row = GridView1.Rows[index];
            archivo = HttpUtility.HtmlDecode(row.Cells[1].Text);


             if (e.CommandName == "Documento_1")
            {
                //FileUpload Caja = new FileUpload();
                //Caja.ID = "CajaV";
                //Page.Controls.Add(Caja);
                
                //InsertSage(row.Cells[0].Text, row.Cells[1].Text);
            }

        }
        catch (Exception ex)
        {
            
        }
    }

    protected void Nuevos(object sender,EventArgs e)
    {
        try
        {
            string Msj = string.Empty;
            string titulo, tipo;
            int Cont = 0;

            foreach (GridViewRow gvr in GridView1.Rows)
            {
                //if (gvr.Cells[4].Text.ToString() == "Pendiente" || HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString()) == "En Revisión")
                if (gvr.Cells[4].Text.ToString() == "Pendiente" || gvr.Cells[4].Text.ToString() == "Rechazado")
                {
                    string Archivo = HttpUtility.HtmlDecode(gvr.Cells[0].Text.ToString());
                    FileUpload PCarga = (FileUpload)gvr.Cells[5].FindControl("BDoc");
                    if (PCarga.PostedFile.ContentType.ToString() != "application/pdf")
                    {
                        Msj = "Error al cargar " + Archivo + " , el archivo debe ser en formato PDF";
                        Cont = Cont + 1;
                    }

                    if (PCarga.PostedFile.ContentLength > 15000000)
                    {
                        Msj = "Error al cargar " + Archivo + " , el archivo no debe ser Superior a 15 MB de Tamaño";
                        Cont = Cont + 1;
                    }

                }

            }



            if (Cont == 0)
            {
                int Nvos = ConsultaDocsNuevos();

                if (Nvos >=1)
                {
                    foreach (GridViewRow gvr in GridView1.Rows)
                    {
                        //if (gvr.Cells[4].Text.ToString() == "Pendiente" || HttpUtility.HtmlDecode(gvr.Cells[4].Text.ToString()) == "En Revisión")
                        if (gvr.Cells[4].Text.ToString() == "Pendiente" || gvr.Cells[4].Text.ToString() == "Rechazado" || gvr.Cells[4].Text.ToString() == "Expirado" || gvr.Cells[4].Text.ToString() == "Por expirar")
                        {
                            string Archivo = HttpUtility.HtmlDecode(gvr.Cells[0].Text.ToString());
                            FileUpload Carga = (FileUpload)gvr.Cells[5].FindControl("BDoc");
                            if (!Carga.HasFile)
                            {
                                Msj = "Error al cargar " + Archivo + ", selecciona un archivo";
                                tipo = "error";
                                titulo = "Error Al Cargar Documentos";
                                ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                                return;
                            }

                            Cont = Cont + 1;
                            if (Carga.PostedFile.ContentType.ToString() == "application/pdf")
                            {
                                if (Carga.FileName.ToString() != "")
                                {

                                    string Guardar = string.Empty;
                                    string Company = UsuarioP.SelectedItem.ToString();
                                    string Prove = User.Identity.Name.ToString();

                                    Guardar = Execute("7", Carga, Company, Prove, Archivo, 2).ToString(); //Poder Notarial  7
                                    if (Guardar != "1")
                                    {
                                        Msj = "Error al cargar " + Archivo + " , Verifica el Archivo";
                                        Cont = Cont + 1;
                                    }
                                }
                            }
                            else
                            {
                                Msj = "Error al cargar " + Archivo + " , el archivo debe ser en formato PDF";
                                Cont = Cont + 1;
                            }

                        }

                    }
                }                    
            }

            if (Cont >= 1)
            {
                if (Msj != "")
                {
                    tipo = "error";
                    titulo = "Error Al Cargar Documentos";
                    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    Comprueba();
                }

                else
                {
                    tipo = "success";
                    Msj = "Has completado la carga de documentos exitosamente, en cuanto el administrador los apruebe, se te notificará por correo electrónico para seguir con el proceso de registro, Gracias";
                    titulo = "Carga de Documentos Exitosa";
                    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                    Comprueba();
                }                
            }
            else
            {
                Comprueba();
            }





        }
        catch
        {

        }
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
        //    string HyperLinkValue = e.Row.Cells[1].Text;

        //    string Doc = e.Row.Cells[0].Text;

        //    HyperLink myLink = new HyperLink();
        //    string Location = ResolveUrl("~/ViewPdfPage.aspx") + "?ColumName=" + HyperLinkValue;
        //    myLink.Text = HyperLinkValue;
        //    e.Row.Style["cursor"] = "pointer";
        //    e.Row.Attributes["onClick"] = string.Format("javascript:window.open('" + HyperLinkValue + "', '_blank');", Location);
        //    e.Row.Attributes.Add("onclick", ClientScript.RegisterStartupScript(GetType(), "ramdomtext", "alertme('Hola');"));
        //    Doc = "{name: }";
        //    e.Row.Attributes.Add("OnClientClick", "Web()");
        //    e.Row.Cells[1].Controls.Add(myLink);
        }
    }

    protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {

    }

    protected void UsuarioP_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //Global.VarSesion(User.Identity.Name.ToString(), UsuarioP.SelectedItem.ToString());
            Comprueba();
        }
        catch(Exception ex)
        {

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

                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/ConfirmacionDocs.html")))
                    {
                        body = reader.ReadToEnd();
                        body = body.Replace("{PassTemp}", PassNew);

                    }
            Global.EmailGlobal(Destinatario, body, "CARGA DE DOCUMENTOS");  
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
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Tarea", Value = "Carga de Documentos" });
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