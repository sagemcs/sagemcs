//PORTAL DE PROVEDORES T|SYS|
//15 - Marzo, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
//PANTALLA PRINCIPAL PARA PROVEEDOR LOGGUEADO

//REFERENCIAS UTILIZADAS
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;

public partial class _Default : Page
{
    string eventName = string.Empty;
    string Texr = string.Empty;
    int Im = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
        Page.Response.Cache.SetNoStore();
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        bool isAuth = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        string tipoa = System.Web.HttpContext.Current.User.Identity.AuthenticationType;

        if (!isAuth)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
            Page.Response.Cache.SetAllowResponseInBrowserHistory(false);
            Page.Response.Cache.SetNoStore();
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }

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
                    string var = HttpContext.Current.Session["Passw"].ToString();
                    string Compnay = HttpContext.Current.Session["IDCompany"].ToString();                    

                    if ((HttpContext.Current.Session["Passw"].ToString() == null)) { }
                    else if (HttpContext.Current.Session["Passw"].ToString() == "1")
                    {
                        string titulo, Msj, tipo;
                        tipo = "warning";
                        Msj = "Tu Password esta a punto de vencer,te invitamos a renovarlo a la brevedad ya que de lo contrario este se actualizará y tendrás que ejecutar la recuperación de contraseña nuevamente ";
                        titulo = "Actualización de Password";
                        Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                    }

                    RevisaPagos();
                    RevisaNotificaciones();

                    if (Texr !="") 
                    {
                        string Mensaje;
                        if (Im>0) { Mensaje = "swal.mixin({confirmButtonText: 'Ok',showCancelButton: false,customClass: 'swal-widei'}).queue([ "; }
                        else { Mensaje = "swal.mixin({confirmButtonText: 'Ok',showCancelButton: false,customClass: 'swal-wide'}).queue([ "; }                        
                        Mensaje = Mensaje + Texr.Substring(0, (Texr.Length) - 1);
                        Mensaje = Mensaje + " ])";
                        ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "Varis(" + Mensaje + ");", true);
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
            Global.Docs();
            int Valor = Convert.ToInt16(HttpContext.Current.Session["Docs"].ToString());

            if ((HttpContext.Current.Session["Docs"].ToString() == "0"))
            {
                Page.MasterPageFile = "MenuP.master";
            }
            string Doc = HttpContext.Current.Session["Docs"].ToString();
            Global.RevDocs();
            if ((HttpContext.Current.Session["Status"].ToString() == "Activo" && HttpContext.Current.Session["Docs"].ToString() != "0"))
            {
                string titulo, Msj, tipo;
                //int Dias = RevDocs();
                string Vendk = HttpContext.Current.Session["VendKey"].ToString();
                string company = HttpContext.Current.Session["IDCompany"].ToString();
                int Dias = Valor;

                if (Dias == 0)
                {
                    Page.MasterPageFile = "MenuP.master";
                }
                else if (Dias == 88)
                {
                    Page.MasterPageFile = "MenuPreP.master";
                    if (UpdateD(Vendk, company, "5") == true) // Actualiza a Pendiente
                    {

                    }
                    tipo = "error";
                    Msj = "Te recordamos que hoy es el último día del tiempo límite para la actualización de opinión de cumplimiento de obligaciones fiscales, deberás cargarlo cuanto antes para seguir teniendo acceso a las actividades del portal.";
                    titulo = "Notificación de actualización de documentos";
                    Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                    //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
                else if (Dias < 0)
                {
                    Page.MasterPageFile = "MenuP.master";

                    if (UpdateD(Vendk, company, "6") == true) // Actualiza a Pendiente
                    {

                    }
                    tipo = "error";
                    Msj = "Has superado el tiempo límite para la actualización de opinión de cumplimiento de obligaciones fiscales, deberás cargarlo cuanto antes para seguir teniendo acceso a las actividades del portal.";
                    titulo = "Notificación de actualización de documentos";
                    Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                    //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
                else if (Dias == 30)
                {
                    Page.MasterPageFile = "MenuPreP.master";
                    tipo = "warning";
                    Msj = "Alguno de tus Documentos se encuentra en revisión, en los próximos días se te enviara un Email con el resultado de la aprobación te recomendamos estar pendiente";
                    titulo = "Notificación de actualización de documentos";
                    Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                    //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
                else if (Dias == 25)
                {
                    Page.MasterPageFile = "MenuP.master";
                    tipo = "warning";
                    Msj = "Alguno de tus documentos fue rechazado, te recomendamos cargarlos cuanto antes para tener acceso a las actividades del portal.";
                    titulo = "Notificación de actualización de documentos";
                    Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                    //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);

                }
                else if (Dias == 26)
                {
                    Page.MasterPageFile = "MenuP.master";
                    tipo = "warning";
                    Msj = "Alguno de tus documentos se encuentra en revisión, en los próximos días se te enviara un Email con el resultado de la aprobación te recomendamos estar pendiente";
                    titulo = "Notificación de actualización de documentos";
                    Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                    //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);

                }
                else if (Dias == 27)
                {
                    Page.MasterPageFile = "MenuP.master";
                    tipo = "error";
                    Msj = "Tu documento de opinión de cumplimiento de obligaciones fiscales fue rechazado, te recomendamos renovarlo cuanto antes para seguir teniendo acceso a las actividades del portal.";
                    titulo = "Notificación de actualización de documentos";
                    Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                    //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
                else if (Dias == 28)
                {
                    Page.MasterPageFile = "MenuPreP.master";
                    tipo = "error";
                    Msj = "Tu documento de opinión de cumplimiento de obligaciones fiscales fue rechazado, por lo que te recomendamos renovarlo cuanto antes, ya que de lo contrario no podrás tener acceso a las actividades del portal.";
                    titulo = "Notificación de actualización de documentos";
                    Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                    //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
                else if (Dias == 22)
                {
                    Page.MasterPageFile = "MenuP.master";

                }
                else if (Dias <= 10 && Dias > 0)
                {
                    Page.MasterPageFile = "MenuPreP.master";
                    if (UpdateD(Vendk, company, "5") == true) // Actualiza a Pendiente
                    {

                    }

                    tipo = "warning";
                    Msj = "Te recordamos tienes " + Dias + " dias para actualizar tu Opinión de Cumplimiento de Obligaciones Fiscales, ya que de lo contrario no podrás tener acceso a las actividades del portal";
                    titulo = "Notificación de actualización de documentos";
                    Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                    //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
                }
                else if (Dias > 10)
                {
                    Page.MasterPageFile = "MenuPreP.master";
                }
            }
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

    protected void DownloadFile(object sender, EventArgs e) 
    {
        try
        {
            MemoryStream memoryStream = new MemoryStream();
            string archivo;
            archivo = "Formato de No Adeudo";
            memoryStream = databaseFileRead("TSYS", "TSYS", "Formato Carta", "TSYS", "IEP");

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
                archivo = archivo + ".docx";
                HttpContext.Current.Response.ContentType = "application/pdf";
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + archivo + "\"");
                HttpContext.Current.Response.AppendHeader("Content-Length", memoryStream.Length.ToString());
                HttpContext.Current.Response.BinaryWrite(memoryStream.ToArray());
                HttpContext.Current.Response.End();
            }
        }
        catch (Exception ex)
        {

        }
    }

    protected void DownloadCFiscal(object sender, EventArgs e)
    {
        try
        {
            //string Url = "https://multiconsulting.com.mx/SAT10072022.pdf";
            //Response.Write("<script language='javascript'>window.open('" + Url + "','_blank','');");

            //MemoryStream memoryStream = new MemoryStream();
            //string archivo;
            //archivo = "Constancia de Situacion Fiscal";
            //memoryStream = databaseFileRead("TSYS", "TSYS", "Constancia de Situacion Fiscal", "TSYS", "IEP");
            ////memoryStream = databaseFileRead("TSYS", "TSYS", "Constancia de Situacion Fiscal", "TSYS", "IEP");

            //if (memoryStream == null || memoryStream.Length == 0)
            //{
            //    string titulo, Msj, tipo;
            //    tipo = "error";
            //    Msj = "Se genero error al cargar el documento, archivo corrupto, comunícate con el área de sistemas para ofrecerte una Solución";
            //    titulo = "Notificaciones T|SYS|";
            //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            //    return;
            //}
            //else
            //{
            //    archivo = archivo.Replace(".", "").Replace(",", "");
            //    archivo = archivo + ".pdf";
            //    HttpContext.Current.Response.ContentType = "application/pdf";
            //    HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + archivo + "\"");
            //    HttpContext.Current.Response.AppendHeader("Content-Length", memoryStream.Length.ToString());
            //    HttpContext.Current.Response.BinaryWrite(memoryStream.ToArray());
            //    HttpContext.Current.Response.End();
            //}
        }
        catch (Exception ex)
        {

        }
    }

    private MemoryStream databaseFileRead(string ID, string Razon, string Doc, string Email, string Company)
    {
        try
        {
            MemoryStream memoryStream = new MemoryStream();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("DowFil", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@ID", Value = ID });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Name", Value = Razon });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Doc", Value = Doc });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Email", Value = Email });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@compan", Value = Company });

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
            return null;
        }
    }

    protected void RevisaPagos1()
    {
        string IDCom = HttpContext.Current.Session["IDCompany"].ToString();
        int Logkey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
        int Uskey = Convert.ToInt32(HttpContext.Current.Session["UserKey"].ToString());

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spRevPago", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 6 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@VendKey", Value = 0 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@IDC", Value = IDCom });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Fol", Value = 0 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
            }

        }
        catch (Exception ex)
        {
            LogError(Logkey, Uskey, "Actualización Estado Factura a Pagada", ex.Message, IDCom);
        }
    }

    protected void RevisaPagos()
    {
        RevisaPagos1();
        int VendK = Convert.ToInt16(HttpContext.Current.Session["VendKey"].ToString());
        string IDCom = HttpContext.Current.Session["IDCompany"].ToString();
        int Logkey = Convert.ToInt32(HttpContext.Current.Session["LogKey"].ToString());
        int Uskey = Convert.ToInt16(HttpContext.Current.Session["UserKey"].ToString());
        int Cont = 0;

        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spRevPago", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 4 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@VendKey", Value = VendK });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@IDC", Value = IDCom });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Fol", Value = 0 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable Registros = new DataTable();
                Registros.Load(rdr);
                int Filas = Registros.Rows.Count;
                foreach (DataRow row in Registros.Rows)
                { 
                    string Fech = string.Empty;
                    string Pago = string.Empty;
                    string Fac = string.Empty;
                    string Cta = string.Empty;
                    string Banco = string.Empty;
                    string RFCB = string.Empty;
                    string Moneda = string.Empty;
                    Fech = (row["FechaPago"].ToString());
                    Pago = (row["Monto"].ToString());
                    Fac = (row["Factura"].ToString());
                    Cta = (row["NoCta"].ToString());
                    Banco = (row["Banco"].ToString());
                    RFCB = (row["RFC_Banco"].ToString());
                    Moneda = (row["Moneda"].ToString());

                    string Msj = string.Empty;
                    string Tipo = "es-MX";
                    if (Moneda == "EUR") { Tipo = "en-US"; }
                    if (Moneda == "USD") { Tipo = "fr-FR"; }
                    CultureInfo gbCulture = new CultureInfo(Tipo);
                    string Simbol = gbCulture.NumberFormat.CurrencySymbol;
                    string total = Simbol + " " + Pago;
                    string Destinartario = User.Identity.Name.ToString();
                    //Destinartario = "lgarcia@multiconsulting.com";
                    Msj = EmailPagos(Destinartario, Fech, total, Fac, Cta, Banco, RFCB,Filas).ToString();
                    if (Msj == "Ok")
                    {
                        Update(VendK, IDCom, Fac);
                        Cont = Cont + 1;
                    }
                    else
                    {
                        LogError(Logkey, Uskey, "Notificación de Pago", Msj, IDCom);
                    }
                }
            }
            if (Cont >= 1)
            {
                string titulo, Msj, tipo;
                if (Cont == 1) { Msj = "Se ha realizado un nuevo pago a tu cuenta, Revisa tu correo en el encontraras los datos para a generación del Complemento de pago"; }
                else { Msj = "Se han realizado nuevos pagos a tu cuenta, Revisa tu correo en el encontraras los datos para a generación del Complemento de pago "; }
                tipo = "success";
                titulo = "Notificación de Pago";
                Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                //ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
            }
        }
        catch (Exception ex)
        {
            LogError(Logkey, Uskey, "Notificación de Pago", ex.Message, IDCom);
        }
    }

    protected void RevisaNotificaciones()
    {
        try
        {
            Im = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spNotificaciones", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Titulo", Value = "11" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Desde", Value = "01/01/2020" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Hasta", Value = "01/01/2020" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Estilo", Value = "11" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Estatus", Value = "1" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Mensaje", Value = "1" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Userkey", Value = "1" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Url", Value = "1" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 4 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                string titulo, Msj, tipo,Img;

                while (rdr.Read())
                {
                    Msj = Convert.ToString(rdr.GetValue(0));
                    tipo = Convert.ToString(rdr.GetValue(1));
                    titulo = Convert.ToString(rdr.GetValue(2));
                    if (Convert.ToString(rdr.GetValue(3)) != "")
                    {
                        Img = Convert.ToString(rdr.GetValue(3));
                        Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "', imageUrl: '" + Img + "'  },";
                        Im =+1;
                    }
                    else 
                    {
                        Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                    }
                   
                } 
            }

        }
        catch (Exception)
        {

        }
    }

    private string EmailPagos(string Destinatario,string Fecha, string Monto, string Fac, string Cta, string Bank, string RFC,int Pagos)
    {
        string Resut = "Error";
        try
        {
          string body = string.Empty;
            string Facturas = string.Empty;
            if (Pagos >= 1)
            {
                Facturas = "<p>Te notificamos que el día de hoy se realizó la aplicación de un pago correspondiente a las Facturas : <strong>" + Fac + "</strong></p>";
            }
            else
            {
                Facturas = "<p>Te notificamos que el día de hoy se realizó la aplicación de un pago correspondiente a la Factura : <strong>" + Fac + "</strong></p>";
            }
          using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/NPagos.html")))
          {
            body = reader.ReadToEnd();
            body = body.Replace("{Date_Fecha}", Fecha);
            body = body.Replace("{Date_Monto}", Monto);
            body = body.Replace("{Date_Fac}", Facturas);
            body = body.Replace("{Date_Cta}", Cta);
            body = body.Replace("{Date_Banco}", Bank);
            body = body.Replace("{Date_RFC}", RFC);
         }
          if (Global.EmailGlobal(Destinatario, body, "NOTIFICACIÓN DE PAGO") == true) { Resut = "Ok"; }
        }
        catch (Exception b)
        {
            Resut = b.Message;
        }
        return Resut;
    }

    private void Update(int vendK, string Company,string Folio)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spRevPago", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 2 });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@VendKey", Value = vendK });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@IDC", Value = Company });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Fol", Value = Folio });

                if (conn.State == ConnectionState.Open) { conn.Close(); }

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
            }

        }
        catch (Exception Bv)
        {


        }
    }

    private int RevDocs()
    {
        int Msj;
        try
        {
            string sql;
            string Cuenta;
            string Cuentas;

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

            sqlConnection1.Close();

            DateTime UpdateDoc;
            DateTime Hoy = DateTime.Now.Date;    //Fecha Hoy 

            if (Cuenta == "")
            {
                sqlConnection1.Open();
                sql = @"Select CreateDate From Documents where DocID = 9 And VendorKey = " + Vendk + " And CompanyID = '" + company + "'";
                using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
                {
                    sqlQuery.CommandType = CommandType.Text;
                    sqlQuery.CommandText = sql;
                    Cuenta = sqlQuery.ExecuteScalar().ToString();
                }
                sqlConnection1.Close();
            }
            
            UpdateDoc = Convert.ToDateTime(Cuenta).Date;  // Fecha ultima Actualización
            DateTime UpdateL = UpdateDoc.AddMonths(6); // Fecha ultima Actualización + 6 Meses
            DateTime UpdateL1 = Hoy.AddMonths(-6); // Fecha Hoy - 6 Meses


            if (UpdateL <= Hoy)  // Si La Fecha de Carga + 6 Meses es Menor a la Fecha actual, ya expiro el tiempo de Prorroga
            {
                if (UpdateD(Vendk, company,"4") == true)
                { Msj = 11;
                  HttpContext.Current.Session["Docs"] = "1";
                }
                else { Msj = -1; }
            }
            else
            {
                DateTime DateDays = UpdateL.AddDays(-10); //Obtenemos la Fecla limite - 10 Dias
                TimeSpan Days = UpdateL - Hoy; // Obtenemos la diferencia de dias entre la fecha limite y Hoy
                int Dias = Days.Days; //Los dias en Numero
                if (Dias <=10)
                {
                    if (UpdateD(Vendk, company,"4") == true) // Actualiza a Pendiente
                    {
                        Msj = Dias;
                        HttpContext.Current.Session["Docs"] = "0";// Muestra el Menu completo
                    }
                    else { Msj = -1; }
                }
                else
                {
                    int rev = RevisaStatus();
                    if (rev == 1)
                    {
                        Msj = 0;
                        HttpContext.Current.Session["Docs"] = "0";// Pasa Normal
                    }
                    else
                    {
                        Msj = 10;
                        HttpContext.Current.Session["Docs"] = "1"; //Envia Alerta
                    }

                }
            }
          
        }
        catch (Exception)
        {
            Msj = -1;
        }
        return Msj;
    }

    private int RevisaStatus()
    {
        int Stado = 0;
        try
        {
            string sql;
            string Cuenta;
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            string Vendk = HttpContext.Current.Session["VendKey"].ToString();
            string company = HttpContext.Current.Session["IDCompany"].ToString();
            sqlConnection1.Open();
            sql = @"Select ISNULL(UpdateUserKey,0) AS Cuenta From Documents Where DocID = 9 And VendorKey = " + Vendk + " And CompanyID = '" + company + "'";
            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Cuenta = sqlQuery.ExecuteScalar().ToString();
            }

            if (Convert.ToInt32(Cuenta) >=1)
            {
                Stado = 1;
            }
        }
        catch (Exception ex)
        {


        }
        return Stado;
    }

    private bool UpdateD(string Vendk,string Company,string status)
    {
        bool ret = false;
        try
        {

            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");

            sqlConnection1.Open();

            using (var sqlQuery = new SqlCommand("", sqlConnection1))
            {
                sqlQuery.CommandText = "UPDATE Documents SET Status = " + status + " ,UpdateDate = NULL,UpdateUserKey = NULL where DocID = 9 And VendorKey = " + Vendk + " And CompanyID = '" + Company + "'";
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

}