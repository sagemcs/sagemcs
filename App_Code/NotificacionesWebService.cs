//PORTAL DE PROVEDORES T|SYS|
//30 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : ADRIAN QUIALA

//REFERENCIAS UTILIZADAS

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using WebSite1;

/// <summary>
/// Summary description for NotificacionesWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class NotificacionesWebService : System.Web.Services.WebService
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    public NotificacionesWebService()
    {
        Context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void listar()
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<String> menssagesFacturas = Tools.Mensaje_Estado_de_Facturas();

                List<String> menssagesPagos = Tools.Mensaje_Estado_de_Pagos();

                List<String> menssagesDocumentos = Tools.Mensaje_Estado_de_Documentos_de_Proveedor();

                var result = new
                {
                    facturas = menssagesFacturas,
                    pagos = menssagesPagos,
                    documentos = menssagesDocumentos
                };
                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void Actualizar_Variable(string bts, string Prv, string Fol, string NOC, string Sts, string Chk, string Fe1, string Fe2)
    {
        HttpContext.Current.Session["Evento"] = bts;

        HttpContext.Current.Session["Prv"] = null;
        HttpContext.Current.Session["Fol"] = null;
        HttpContext.Current.Session["NOC"] = null;
        HttpContext.Current.Session["Sts"] = null;
        HttpContext.Current.Session["Chk"] = null;
        HttpContext.Current.Session["Fe1"] = null;
        HttpContext.Current.Session["Fe2"] = null;

        if (Prv != "") { HttpContext.Current.Session["Prv"] = Prv; }
        if (Fol != "") { HttpContext.Current.Session["Fol"] = Fol; }
        if (NOC != "") { HttpContext.Current.Session["NOC"] = NOC; }
        if (Sts != "") { HttpContext.Current.Session["Sts"] = Sts; }
        if (Chk != "") { HttpContext.Current.Session["Chk"] = Chk; }
        if (Fe1 != "") { HttpContext.Current.Session["Fe1"] = Fe1; }
        if (Fe2 != "") { HttpContext.Current.Session["Fe2"] = Fe2; }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void enviar_notificacion_de_rechazo_de_factura(string folio, string texto, string llave)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string Tabla = string.Empty;
                string msg = texto;
                try
                {
                    if (!string.IsNullOrWhiteSpace(folio))
                    {
                        int FFolio = Convert.ToInt16(folio);
                        var facturas_dto = Facturas.ObtenerPorFolio(FFolio);

                        if (facturas_dto != null && facturas_dto.Count > 0)
                        {
                            int FacKey = facturas_dto.First().Key;
                            int User = Convert.ToInt16(HttpContext.Current.Session["UserKey"]);
                            int Not = 0;

                            string Email = string.Empty;
                            Email = CorreoProvedor(facturas_dto.First().VendKey);
                            ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(facturas_dto.First().Proveedor);

                            success = false;
                            msg = "Se ha generado un error intentar cancelar la factura, intenta nuevamente si el problema persiste comunicate con el área de soporte";

                            if (Cancelar(FacKey, User))
                            {
                                int Valor1 = Convert.ToInt32(folio);
                                if ((CancelarD1(Valor1, 0, 1, 1) == 0))
                                {

                                    string Body;
                                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/FacturaRechazada.html")))
                                    {
                                        Body = reader.ReadToEnd();
                                        Body = Body.Replace("{Fac}", llave).Replace("{Comments}", texto);
                                    }

                                    bool SendEmail = Global.EmailGlobal(Email, Body, "Notificación de Rechazo de Factura");
                                    //bool SendEmail = true;
                                    if (SendEmail == true)
                                    {
                                        success = true;
                                        msg = "Se ha enviado un correo electronico con la notificación de rechazo de la factura de folio: " + llave;
                                        Not = 1;
                                    }
                                    else
                                    {
                                        success = false;
                                        msg = "Se ha generado un error al enviar el correo, no se ha podido notificar el rechazo de la factura de folio: " + llave;
                                        Not = 0;
                                    }

                                    int Valor = Convert.ToInt32(folio);
                                    bool fav = CancelarD(Valor, Not, 2, 1);
                                }
                                else
                                {
                                    success = true;
                                    msg = "Ya se ha enviado un correo electronico con la notificación de rechazo de la factura con el folio: " + llave;
                                }
                            }
                            else
                            {
                                success = false;
                                msg = "Se ha generado un error intentar cancelar la factura, intenta nuevamente si el problema persiste comunicate con el área de soporte";
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg,
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }

    protected int CancelarD1(int FacKey, int Not, int Op, int Tipo)
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

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void NOenviar_notificacion_de_rechazo_de_factura(string folio, string texto, string llave)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string msg = texto;
                try
                {
                    int FFolio = Convert.ToInt16(folio);
                    var facturas_dto = Facturas.ObtenerPorFolio(FFolio);

                    if (facturas_dto != null && facturas_dto.Count > 0)
                    {
                        int FacKey = facturas_dto.First().Key;
                        int User = Convert.ToInt16(HttpContext.Current.Session["UserKey"]);
                        ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(facturas_dto.First().Proveedor);
                        if (Cancelar(FacKey, User) == true)
                        {
                            success = true;
                            msg = "Se ha cancelado la Factura " + llave;
                        }
                        else
                        {
                            success = false;
                            msg = "Se ha generado un error intentar cancelar la factura, intenta nuevamente si el problema persiste comunicate con el área de soporte";
                        }

                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch (Exception ex)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }

    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void Rechazar_Doc(string folio, string texto, string llave)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string msg = texto;
                try
                {
                    int User = Convert.ToInt16(HttpContext.Current.Session["UserKey"]);
                    if (CancelarDoc(Convert.ToInt32(folio), User) == true)
                    {


                        string Body;
                        using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/DocumentoRechazado.html")))
                        {
                            Body = reader.ReadToEnd();
                            Body = Body.Replace("{Fac}", llave).Replace("{Comments}", texto);
                        }

                        //string Email = CorreoProvedorDoc(Convert.ToInt32(folio));
                        //bool SendEmail = Global.EmailGlobal(Email, Body, "Notificación de Rechazo de Factura");
                        bool SendEmail = true;
                        if (SendEmail == true)
                        {
                            success = true;
                            msg = "Se ha enviado un correo electronico con la notificación de rechazo del documento : " + llave;
                        }
                        else
                        {
                            success = false;
                            msg = "Se ha rechazado el documento pero sin embargo se ha generado un error al enviar el correo al proveedor, no se ha podido notificar el rechazo del Documento: " + llave;
                        }
                    }
                    else
                    {
                        success = false;
                        msg = "Se ha generado un error intentar cancelar el documento, intenta nuevamente si el problema persiste comunicate con el área de soporte";
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch (Exception ex)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void Rechazar_NoDoc(string folio, string texto, string llave)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string msg = texto;
                try
                {

                    int User = Convert.ToInt16(HttpContext.Current.Session["UserKey"]);
                    if (CancelarDoc(Convert.ToInt32(folio), User) == true)
                    {
                        success = true;
                        msg = "Se ha rechazado el Documento " + llave;
                    }
                    else
                    {
                        success = false;
                        msg = "Se ha generado un error intentar cancelar el documento, intenta nuevamente si el problema persiste comunicate con el área de soporte";
                    }

                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch (Exception ex)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void Prog_Alerta(string Userkey, string Password, string Titulo, string Estilo, string Estatus, string Desde, string Hasta, string Mensaje, string Url)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string msg = string.Empty;
                try
                {
                    ApplicationDbContext context = new ApplicationDbContext();
                    UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
                    UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
                    var manager = new UserManager();
                    ApplicationUser Busca = manager.Find(Userkey, Password);

                    if (Busca != null)
                    {
                        DataTable dt = new DataTable();
                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                        {
                            SqlCommand cmd = new SqlCommand("spNotificaciones", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            string Company = HttpContext.Current.Session["IDCompany"].ToString();
                            int Opcion = 1;

                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Titulo", Value = Titulo });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Desde", Value = Desde });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Hasta", Value = Hasta });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Estilo", Value = Estilo });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Estatus", Value = Estatus });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Mensaje", Value = Mensaje });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Userkey", Value = Userkey });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Url", Value = Url });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = Opcion });

                            if (conn.State == ConnectionState.Open) { conn.Close(); }

                            conn.Open();
                            SqlDataReader rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                if (rdr["Resultado"].ToString() == "Ok")
                                {
                                    success = true;
                                    msg = "La notificación ha sido programada exitosamente, esta será visualizada por los proveedores durante el tiempo indicado.";
                                }
                                else
                                {
                                    success = false;
                                    msg = "Ocurrio un error al intentar guardar el mensaje, comunicate con el área de soporte";
                                }
                            }
                            conn.Close();
                        }
                    }
                    else
                    {
                        success = false;
                        msg = "Password Incorrecto";
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch (Exception ex)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void Suspender(string Userkey, string Password, string In1, string In2, string Desde, string Hasta, string In3, string In4, string Desde1, string Hasta1)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string msg = string.Empty;
                try
                {
                    ApplicationDbContext context = new ApplicationDbContext();
                    UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
                    UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
                    var manager = new UserManager();
                    ApplicationUser Busca = manager.Find(Userkey, Password);

                    if (Busca != null)
                    {
                        DataTable dt = new DataTable();
                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                        {
                            SqlCommand cmd = new SqlCommand("spSuspender", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            int Opcion = 1;

                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Userkey", Value = Userkey });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@In1", Value = In1 });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@In2", Value = In2 });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Desde", Value = Desde });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Hasta", Value = Hasta });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@In3", Value = In3 });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@In4", Value = In4 });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Desde1", Value = Desde1 });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Hasta1", Value = Hasta1 });
                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = Opcion });

                            if (conn.State == ConnectionState.Open) { conn.Close(); }

                            conn.Open();
                            SqlDataReader rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                if (rdr["Resultado"].ToString() == "Ok")
                                {
                                    success = true;
                                    msg = "¡ La actualización ha sido programada exitosamente !";
                                }
                                else
                                {
                                    success = false;
                                    msg = "Ocurrio un error al intentar guardar los cambios, comunicate con el área de soporte";
                                }
                            }
                            conn.Close();
                        }
                    }
                    else
                    {
                        success = false;
                        msg = "Password Incorrecto";
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch (Exception ex)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void Vincular_Prov(string prov, string user)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string msg = string.Empty;
                try
                {
                    string SQL = "update Vendors Set Superior = (Select top 1 userkey From Users Where UserName = '" + user + "') Where VendorKey = (Select top 1 VendorKey From vendors Where VendName = '" + prov + "')";
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand(SQL, conn);
                        cmd.CommandType = CommandType.Text;
                        if (conn.State == ConnectionState.Open) { conn.Close(); }
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        success = true;
                        msg = "¡ La actualización ha sido exitosa !";
                        conn.Close();
                    }

                    string SQL2 = "update users Set Status = 2 Where userkey = (Select top 1 userkey From vendors Where VendName = '" + prov + "')";
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand(SQL2, conn);
                        cmd.CommandType = CommandType.Text;
                        if (conn.State == ConnectionState.Open) { conn.Close(); }
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        success = true;
                        msg = "¡ La actualización ha sido exitosa !";
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    msg = "¡ Ocurrio un error al intentar vincular el Proveedor con el Usuario !";
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch (Exception ex)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void Actualizar_Prov(string prov, string valo, string user)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string msg = string.Empty;
                try
                {
                    string SQL = "";
                    if (valo == "0") // 0 = null
                    {
                        SQL = "update Vendors Set Superior = null Where VendorKey = (Select top 1 VendorKey From vendors Where VendName = '" + prov + "')";
                    }
                    else
                    {
                        SQL = "update Vendors Set Superior = (Select top 1 userkey From Users Where UserName = '" + user + "') Where VendorKey = (Select top 1 VendorKey From vendors Where VendName = '" + prov + "')";
                    }

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand(SQL, conn);
                        cmd.CommandType = CommandType.Text;
                        if (conn.State == ConnectionState.Open) { conn.Close(); }
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        success = true;
                        msg = "¡ La actualización ha sido exitosa !";
                        conn.Close();
                    }

                    string SQL2 = "";
                    if (valo == "0") // 0 = null
                    {
                        SQL2 = "update users Set Status = 1 Where userkey = (Select top 1 userkey From vendors  Where VendName = '" + prov + "')";
                    }
                    else
                    {
                        SQL2 = "update users Set Status = 2 Where userkey = (Select top 1 userkey From vendors  Where VendName = '" + prov + "')";
                    }

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand(SQL2, conn);
                        cmd.CommandType = CommandType.Text;
                        if (conn.State == ConnectionState.Open) { conn.Close(); }
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        success = true;
                        msg = "¡ La actualización ha sido exitosa !";
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    msg = "¡ Ocurrio un error al intentar vincular el Proveedor con el Usuario !";
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch (Exception ex)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void Asignar_Prov(string prov, string user)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string msg = string.Empty;
                try
                {
                    string SQL = "update Vendors Set Aprobador = (Select top 1 userkey From Users Where UserName = '" + user + "') Where VendorKey = (Select top 1 VendorKey From vendors Where VendName = '" + prov + "')";
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand(SQL, conn);
                        cmd.CommandType = CommandType.Text;
                        if (conn.State == ConnectionState.Open) { conn.Close(); }
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        success = true;
                        msg = "¡ La actualización ha sido exitosa !";
                        conn.Close();
                    }

                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    msg = "¡ Ocurrio un error al intentar vincular el Proveedor con el Usuario !";
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch (Exception ex)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void Actualizar_Asignacion(string prov, string valo, string user)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string msg = string.Empty;
                try
                {
                    string SQL = "";
                    if (valo == "0") // 0 = null
                    {
                        SQL = "update Vendors Set Aprobador = null Where VendorKey = (Select top 1 VendorKey From vendors Where VendName = '" + prov + "')";
                    }
                    else
                    {
                        SQL = "update Vendors Set Aprobador = (Select top 1 userkey From Users Where UserName = '" + user + "') Where VendorKey = (Select top 1 VendorKey From vendors Where VendName = '" + prov + "')";
                    }

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand(SQL, conn);
                        cmd.CommandType = CommandType.Text;
                        if (conn.State == ConnectionState.Open) { conn.Close(); }
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        success = true;
                        msg = "¡ La actualización ha sido exitosa !";
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    msg = "¡ Ocurrio un error al intentar vincular el Proveedor con el Usuario !";
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch (Exception ex)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void Vincular_user(string prov, string user)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string msg = string.Empty;
                try
                {
                    string SQL = "update Users Set Superior = (Select top 1 userkey From Users Where UserName = '" + user + "') Where userkey = (Select top 1 userkey From Users Where UserName = '" + prov + "')";
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand(SQL, conn);
                        cmd.CommandType = CommandType.Text;
                        if (conn.State == ConnectionState.Open) { conn.Close(); }
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        success = true;
                        msg = "¡ La actualización ha sido exitosa !";
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    //msg = ex.Message;
                    msg = "¡ Ocurrio un error al intentar vincular el Proveedor con el Usuario !";
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch (Exception ex)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void Actualizar_user(string prov, string valo, string user)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string msg = string.Empty;
                try
                {
                    string SQL = "";
                    if (valo == "0") // 0 = null
                    {
                        SQL = "update Users Set Superior = null Where userkey = (Select top 1 userkey From Users Where UserName = '" + prov + "')";
                    }
                    else
                    {
                        SQL = "update Users Set Superior = (Select top 1 userkey From Users Where UserName = '" + user + "') Where userkey = (Select top 1 userkey From Users Where UserName = '" + prov + "')";
                    }

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand(SQL, conn);
                        cmd.CommandType = CommandType.Text;
                        if (conn.State == ConnectionState.Open) { conn.Close(); }
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        success = true;
                        msg = "¡ La actualización ha sido exitosa !";
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    msg = "¡ Ocurrio un error al intentar vincular el Proveedor con el Usuario !";
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch (Exception ex)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void enviar_notificacion_de_rechazo_de_pago(string folio, string texto, string llave)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string Tabla = string.Empty;
                string msg = texto;
                try
                {
                    if (!string.IsNullOrWhiteSpace(folio))
                    {
                        int FFolio = Convert.ToInt16(folio);
                        int User = Convert.ToInt16(HttpContext.Current.Session["UserKey"]);
                        int Not = 0;

                        string Email = string.Empty;
                        Email = CorreoProvedorPago(FFolio);
                        //Email = "lgarcia@multiconsulting.com";
                        success = false;
                        msg = "Se ha generado un error intentar cancelar la factura, intenta nuevamente si el problema persiste comunicate con el área de soporte";

                        if (CancelarPago(FFolio, User))
                        {

                            int Valor1 = Convert.ToInt32(folio);
                            if ((CancelarD1(Valor1, 0, 1, 2) == 0))
                            {

                                string Body;
                                using (StreamReader reader = new StreamReader(Server.MapPath("~/Account/Templates Email/PagoRechazado.html")))
                                {
                                    Body = reader.ReadToEnd();
                                    Body = Body.Replace("{Fac}", llave).Replace("{Comments}", texto);
                                }

                                bool SendEmail = Global.EmailGlobal(Email, Body, "Notificación de Rechazo de Complemento de Pago");
                                if (SendEmail == true)
                                {
                                    success = true;
                                    msg = "Se ha enviado un correo electronico con la notificación de rechazo del Compleneto de Pago con folio: " + llave;
                                    Not = 1;
                                }
                                else
                                {
                                    success = false;
                                    msg = "Se ha generado un error al enviar el correo, no se ha podido notificar el rechazo del Complemento de Pago con folio: " + llave;
                                    Not = 0;
                                }

                                int Valor = Convert.ToInt32(folio);
                                bool fav = CancelarD(Valor, Not, 2, 2);

                            }
                            else
                            {
                                success = true;
                                msg = "Ya se ha enviado un correo electronico con la notificación de rechazo de la factura con el folio: " + llave;
                            }

                        }
                        else
                        {
                            success = false;
                            msg = "Se ha generado un error intentar cancelar el complemento de Pago, intenta nuevamente si el problema persiste comunicate con el área de soporte";
                        }
                    }
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg,
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void NOenviar_notificacion_de_rechazo_de_pago(string folio, string texto, string llave)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                bool success = true;
                string msg = texto;
                try
                {
                    int FFolio = Convert.ToInt16(folio);

                    int User = Convert.ToInt16(HttpContext.Current.Session["UserKey"]);
                    if (CancelarPago(FFolio, User) == true)
                    {
                        success = true;
                        msg = "Se ha cancelado el Complemento de Pago con Folio :" + llave;
                    }
                    else
                    {
                        success = false;
                        msg = "Se ha generado un error intentar cancelar el complemento, intenta nuevamente si el problema persiste comunicate con el área de soporte";
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    success = false;
                }
                var result = new
                {
                    success = success,
                    msg = msg
                };

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.Write(js.Serialize(result));
            }
            else
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Context.Response.End();
            }
        }
        catch (Exception ex)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }

    }


    protected bool CancelarD(int FacKey, int Not, int Op, int Tipo)
    {
        bool vr = false;
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string sSQL;

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
                //while (rdr.Read())
                //{
                //    val1 = rdr.GetInt32(0); // 0 ok
                //}
                sqlConnection1.Close();
                vr = true;

            }
        }
        catch (Exception ex)
        {
            throw new MulticonsultingException(ex.Message);
        }
        return vr;
    }

    protected bool Cancelar(int FacKey, int user)
    {
        bool Cancel = true;
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            int val1 = 1;
            string sSQL;

            sSQL = "UpdateInvoice";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@UserKey", user));
            parsT.Add(new SqlParameter("@FacKey", FacKey));

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
                //while (rdr.Read())
                //{
                //    val1 = rdr.GetInt32(0); // 0 ok
                //}
                sqlConnection1.Close();

            }
        }
        catch (Exception ex)
        {
            Cancel = false;
            throw new MulticonsultingException(ex.Message);
        }
        return Cancel;
    }

    protected bool CancelarDoc(int DKey, int user)
    {
        bool Cancel = true;
        try
        {
            string Ssql = "Update Documents set Status = 3, UpdateDate = NULL, UpdateUserKey = NULL Where DocKey = " + DKey;

            SqlConnection sqlConnection2 = new SqlConnection();
            sqlConnection2 = SqlConnectionDB("PortalConnection");
            sqlConnection2.Open();
            using (SqlCommand Cmdd = new SqlCommand(Ssql, sqlConnection2))
            {
                Cmdd.CommandType = CommandType.Text;
                Cmdd.CommandText = Ssql;
                Cmdd.ExecuteScalar();
                //Llave = Convert.ToInt32(Cmdd.ExecuteScalar().ToString());
            }
            sqlConnection2.Close();

        }
        catch (Exception ex)
        {
            Cancel = false;
            throw new MulticonsultingException(ex.Message);
        }
        return Cancel;
    }

    protected bool CancelarPago(int FacKey, int user)
    {
        bool Cancel = true;
        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            int val1 = 1;
            string sSQL;

            sSQL = "UpdatePayment";
            List<SqlParameter> parsT = new List<SqlParameter>();
            parsT.Add(new SqlParameter("@UserKey", user));
            parsT.Add(new SqlParameter("@FacKey", FacKey));
            parsT.Add(new SqlParameter("@Opcion", 1));

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
            Cancel = false;
            throw new MulticonsultingException(ex.Message);
        }
        return Cancel;
    }

    protected string CorreoProvedor(int VendorKey)
    {
        string Correo = string.Empty;

        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string sql = @"Select top 1 b.UserName From Vendors a inner join AspNetUsers b on a.UserKey = b.UserKey Where VendorKey = " + VendorKey;
            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Correo = sqlQuery.ExecuteScalar().ToString();
            }
        }
        catch (Exception ex)
        {
            throw new MulticonsultingException(ex.Message);
        }

        return Correo;
    }

    protected string CorreoProvedorDoc(int VendorKey)
    {
        string Correo = string.Empty;

        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string sql = @"Select top 1 c.UserName from AspNetUsers c ";
            sql = sql + " inner join Vendors a  on c.UserKey = a.UserKey inner join Documents b on a.VendorKey =b.VendorKey ";
            sql = sql + " Where b.DocKey = " + VendorKey;

            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Correo = sqlQuery.ExecuteScalar().ToString();
            }
        }
        catch (Exception ex)
        {
            throw new MulticonsultingException(ex.Message);
        }

        return Correo;
    }

    protected string CorreoProvedorPago(int VendorKey)
    {
        string Correo = string.Empty;

        try
        {
            SqlConnection sqlConnection1 = new SqlConnection();
            sqlConnection1 = SqlConnectionDB("PortalConnection");
            sqlConnection1.Open();
            string sql = @"Select top 1 b.UserName From Vendors a inner join AspNetUsers b on a.UserKey = b.UserKey inner join Payment c on a.VendorKey = c.VendorKey Where c.PaymentKey = " + VendorKey;
            using (var sqlQuery = new SqlCommand(sql, sqlConnection1))
            {
                sqlQuery.CommandType = CommandType.Text;
                sqlQuery.CommandText = sql;
                Correo = sqlQuery.ExecuteScalar().ToString();
            }
        }
        catch (Exception ex)
        {
            throw new MulticonsultingException(ex.Message);
        }

        return Correo;
    }

    private static DataSet GetData(string VendId, string CFolio, string NoOdc, string Estatus, string Opcion, string Fecha1, string Fecha2)
    {
        SqlConnection sqlConnection1 = new SqlConnection();
        sqlConnection1 = SqlConnectionDB("PortalConnection");
        sqlConnection1.Open();

        string RFC = "";
        string UUID = "";
        string CompanyID = HttpContext.Current.Session["IDCompany"].ToString();
        string sSQL = "spSelectInvoice";
        DataSet dss = new DataSet();
        using (SqlCommand Cmd = new SqlCommand(sSQL, sqlConnection1))
        {
            try
            {
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = sSQL;

                List<SqlParameter> parsT = new List<SqlParameter>();
                parsT.Add(new SqlParameter("@VendId", VendId));
                parsT.Add(new SqlParameter("@RFC", RFC));
                parsT.Add(new SqlParameter("@Folio", CFolio));
                parsT.Add(new SqlParameter("@UUID", UUID));
                parsT.Add(new SqlParameter("@NodeOC", NoOdc));
                parsT.Add(new SqlParameter("@estatus", Estatus));
                parsT.Add(new SqlParameter("@opcion", Opcion));
                parsT.Add(new SqlParameter("@FechaInicial", Fecha1));
                parsT.Add(new SqlParameter("@FechaFinal", Fecha2));
                parsT.Add(new SqlParameter("@CompanyID", CompanyID));

                SqlParameter[] sqlParameter = parsT.ToArray();
                foreach (SqlParameter par in sqlParameter)
                {
                    Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);
                }

                SqlDataAdapter Data = new SqlDataAdapter(Cmd);
                DataTable Table = new DataTable();
                Data.Fill(dss);
                return dss;

            }
            catch (Exception exz)
            {
                return dss;
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
            throw new MulticonsultingException(ex.Message);

        }
    }

}
