//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS

using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

/// <summary>
/// Summary description for ContrarecibosWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class ContrarecibosWebService : System.Web.Services.WebService
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    public ContrarecibosWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void listar(string order_col, string order_dir, string Folio, string VendID, string AliasDBA, string Total, string RcptDate, string RcptPago, int start, int length, bool sin_solicitud = false)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<ContrareciboDTO> list_dto = new List<ContrareciboDTO>();

                RcptDate = Tools.ObtenerFechaEnFormatoNew(RcptDate);
                RcptPago = Tools.ObtenerFechaEnFormatoNew(RcptPago);

                list_dto = Contrarecibos.ObtenerContrarecibos(Folio, VendID, AliasDBA, Total, RcptDate, RcptPago, sin_solicitud);

                //list_dto = Contrarecibos.ObtenerContrarecibos_2_0(order_col, order_dir, Folio, VendID, AliasDBA, Total, RcptDate, RcptPago, sin_solicitud);
                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (list_dto != null)
                {
                    for (int i = 0; i <= list_dto.Count() - 1; i++)
                    {
                        string regin = "es-MX";
                        if (list_dto[i].Moneda == "USD")
                        {
                            regin = "en-US";
                        }
                        else if (list_dto[i].Moneda == "EUR")
                        {
                            regin = "fr-FR";
                        }

                        CultureInfo gbCulture = new CultureInfo(regin);
                        string Simbol = gbCulture.NumberFormat.CurrencySymbol;
                        list_dto[i].Total = Simbol + " " + Convert.ToDecimal(list_dto[i].Total).ToString("#,##0.00");
                    }


                    int total = list_dto.Count();

                    if (order_col == "1")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Folio).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Folio).ToList();
                    else if (order_col == "2")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Proveedor).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Proveedor).ToList();
                    else if (order_col == "3")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.RFC).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.RFC).ToList();
                    else if (order_col == "4")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Condiciones).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Condiciones).ToList();
                    else if (order_col == "5")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.DateAr).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.DateAr).ToList();
                    else if (order_col == "6")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.DatePr).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.DatePr).ToList();
                    else if (order_col == "7")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Total_In_Double).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Total_In_Double).ToList();


                    list_dto = length == -1 ? list_dto.Skip(start).ToList() : list_dto.Skip(start).Take(length).ToList();
                    int cantidad = list_dto.Count();

                    var result = new
                    {
                        recordsTotal = cantidad,
                        recordsFiltered = total,
                        data = list_dto
                    };
                    Context.Response.Write(js.Serialize(result));
                }
                else
                {
                    var result = new
                    {
                        error = "No se obtubieron los datos peticionados"
                    };
                    Context.Response.Write(js.Serialize(result));
                }
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
    public void listar2(string order_col, string order_dir, string Folio, string VendID, string AliasDBA, string Total, string RcptDate, string RcptPago, int start, int length, bool sin_solicitud = false)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<ContrareciboDTO2> list_dto = new List<ContrareciboDTO2>();

                RcptDate = Tools.ObtenerFechaEnFormatoNew(RcptDate);
                RcptPago = Tools.ObtenerFechaEnFormatoNew(RcptPago);
                list_dto = Contrarecibos.ObtenerContrarecibos_2_0(order_col, order_dir, Folio, VendID, AliasDBA, Total, RcptDate, RcptPago, sin_solicitud);

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (list_dto != null)
                {
                    for (int i = 0; i <= list_dto.Count() - 1; i++)
                    {
                        string regin = "es-MX";
                        if (list_dto[i].Moneda == "USD")
                        {
                            regin = "en-US";
                        }
                        else if (list_dto[i].Moneda == "EUR")
                        {
                            regin = "fr-FR";
                        }

                        CultureInfo gbCulture = new CultureInfo(regin);
                        string Simbol = gbCulture.NumberFormat.CurrencySymbol;
                        list_dto[i].Total = Simbol + " " + Convert.ToDecimal(list_dto[i].Total).ToString("#,##0.00");
                    }


                    int total = list_dto.Count();
                    list_dto = length == -1 ? list_dto.Skip(start).ToList() : list_dto.Skip(start).Take(length).ToList();
                    int cantidad = list_dto.Count();

                    var result = new
                    {
                        recordsTotal = cantidad,
                        recordsFiltered = total,
                        data = list_dto
                    };
                    Context.Response.Write(js.Serialize(result));
                }
                else
                {
                    var result = new
                    {
                        error = "No se obtubieron los datos peticionados"
                    };
                    Context.Response.Write(js.Serialize(result));
                }
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
    public void listarC(string order_col, string order_dir, string Folio, string VendID, string AliasDBA, string Total, string RcptDate, string RcptPago, int start, int length, bool sin_solicitud = false)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<ContrareciboDTO> list_dto = new List<ContrareciboDTO>();

                RcptDate = Tools.ObtenerFechaEnFormatoNew(RcptDate);
                RcptPago = Tools.ObtenerFechaEnFormatoNew(RcptPago);

                string usker = HttpContext.Current.Session["UserKey"].ToString();
                string roles = HttpContext.Current.Session["RolUser"].ToString();

                if (roles == "T|SYS| - Empleado")
                {
                    list_dto = Contrarecibos.ObtenerContrarecibosEmpleado(Folio, VendID, AliasDBA, Total, RcptDate, RcptPago, sin_solicitud);
                }
                else
                {
                    list_dto = Contrarecibos.ObtenerContrarecibos(Folio, VendID, AliasDBA, Total, RcptDate, RcptPago, sin_solicitud);
                }
                //list_dto = Contrarecibos.ObtenerContrarecibos(Folio, VendID, AliasDBA, Total, RcptDate, RcptPago, sin_solicitud);

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (list_dto != null)
                {
                    for (int i = 0; i <= list_dto.Count() - 1; i++)
                    {
                        string regin = "es-MX";
                        if (list_dto[i].Moneda == "USD")
                        {
                            regin = "en-US";
                        }
                        else if (list_dto[i].Moneda == "EUR")
                        {
                            regin = "fr-FR";
                        }

                        CultureInfo gbCulture = new CultureInfo(regin);
                        string Simbol = gbCulture.NumberFormat.CurrencySymbol;
                        list_dto[i].Total = Simbol + " " + Convert.ToDecimal(list_dto[i].Total).ToString("#,##0.00");
                    }


                    int total = list_dto.Count();

                    if (order_col == "1")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Folio).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Folio).ToList();
                    else if (order_col == "2")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Proveedor).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Proveedor).ToList();
                    else if (order_col == "3")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.RFC).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.RFC).ToList();
                    else if (order_col == "4")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Condiciones).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Condiciones).ToList();
                    else if (order_col == "5")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.DateRx).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.DateRx).ToList();
                    else if (order_col == "6")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.DatePr).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.DatePr).ToList();
                    else if (order_col == "7")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Total_In_Double).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Total_In_Double).ToList();

                    list_dto = length == -1 ? list_dto.Skip(start).ToList() : list_dto.Skip(start).Take(length).ToList();
                    int cantidad = list_dto.Count();

                    var result = new
                    {
                        recordsTotal = cantidad,
                        recordsFiltered = total,
                        data = list_dto
                    };
                    Context.Response.Write(js.Serialize(result));
                }
                else
                {
                    var result = new
                    {
                        error = "No se obtubieron los datos peticionados"
                    };
                    Context.Response.Write(js.Serialize(result));
                }
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
    public void getrol(string ids)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                string usker = HttpContext.Current.Session["UserKey"].ToString();
                string roles = HttpContext.Current.Session["RolUser"].ToString();
                string token = Contrarecibos.CrearToken();

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                var result = new
                {
                    success = true,
                    rol = roles,
                    user = usker,
                    token = token
                };
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
    public void newtoken(string ids)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                string usker = HttpContext.Current.Session["UserKey"].ToString();
                string roles = HttpContext.Current.Session["RolUser"].ToString();
                string token = "error";
                token = Contrarecibos.GeneraryEnviar();

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                var result = new
                {
                    success = true,
                    rol = roles,
                    user = usker,
                    token = token
                };
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
    public void updateSOL(string ids)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                string usker = HttpContext.Current.Session["UserKey"].ToString();
                string roles = HttpContext.Current.Session["RolUser"].ToString();

                string token = Contrarecibos.updatetoken(ids);
                string Genera = "NO";
                if (roles.Contains("Admin") || roles.Contains("Finanzas")) { Genera = "SI"; }

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                var result = new
                {
                    success = true,
                    rol = roles,
                    user = usker,
                    token = token,
                    Genera = Genera
                };
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
    public void RevToken(string ids)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                string usker = HttpContext.Current.Session["UserKey"].ToString();
                string roles = HttpContext.Current.Session["RolUser"].ToString();
                string token = Contrarecibos.RevToken(ids);

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                var result = new
                {
                    success = true,
                    rol = roles,
                    user = usker,
                    token = token
                };
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
    public void generar(string ids)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();
                var UUIDs_obj = (IEnumerable<object>)js.DeserializeObject(ids);
                List<string> UUIDs = new List<string>();
                foreach (var item in UUIDs_obj)
                {
                    UUIDs.Add(item.ToString());
                }


                bool success = true;
                int id = -1;
                string msg = "";
                try
                {
                    string rpt_path = Path.Combine(Server.MapPath("~/Reports"), "ContrareciboReport.rpt");
                    id = Contrarecibos.GenerarContrarecibo(UUIDs, rpt_path);
                }
                catch (Exception e)
                {
                    msg = e.Message;
                    success = false;
                }
                if (id == -1)
                    success = false;
                var result = new
                {
                    success = success,
                    id = id,
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
        catch
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Context.Response.End();
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public bool es_proveedor()
    {
        return Tools.EsProveedor();
    }
}
