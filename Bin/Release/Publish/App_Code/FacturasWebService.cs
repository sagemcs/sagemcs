//PORTAL DE PROVEDORES T|SYS|
//08 ABRIL DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA P.

//REFERENCIAS UTILIZADAS

using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.IO;

/// <summary>
/// Summary description for FacturasWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
//[System.Web.Script.Services.ScriptService]
public class FacturasWebService : System.Web.Services.WebService
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    public FacturasWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void listar_estado_factura(string order_col, string order_dir, string VendorId, string Folio, string Fecha, string FechaR, string contrarecibo, string solicitud, string Estado, int start, int length)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<FacturaDTO> list_dto = new List<FacturaDTO>();

                Fecha = Tools.ObtenerFechaEnFormatoNew(Fecha);
                FechaR = Tools.ObtenerFechaEnFormatoNew(FechaR);
                Facturas.ActualizarEstadoFacturasSql();

                list_dto = Facturas.ObtenerFacturas(VendorId, Folio, Fecha, FechaR, contrarecibo, solicitud, Estado);

                for (int i = 0; i <= list_dto.Count() - 1; i++)
                {
                    string regin = "";
                    if (list_dto[i].Moneda == "MXN")
                    {
                        regin = "es-MX";
                    }
                    else if (list_dto[i].Moneda == "USD")
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

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (list_dto != null)
                {
                    foreach (FacturaDTO factura in list_dto)
                    {
                        string path = "..\\..\\Img\\estados\\" + (string.IsNullOrWhiteSpace(factura.Estado_Id) ? "0.png" : factura.Estado_Id + ".png");
                        factura.Estado_Img = "<img src=\"" + path + "\"style=\"width: 50px; height 50px \">";
                    }

                    int total = list_dto.Count();

                    if (order_col == "1")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Folio).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Folio).ToList();
                    else if (order_col == "2")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Serie).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Serie).ToList();
                    else if (order_col == "3")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Proveedor_Nombre).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Proveedor_Nombre).ToList();
                    else if (order_col == "4")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Date).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Date).ToList();
                    else if (order_col == "5")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Fecha).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Fecha).ToList();
                    else if (order_col == "6")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Total).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Total).ToList();
                    else if (order_col == "7")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Contrarecibo_Folio).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Contrarecibo_Folio).ToList();
                    else if (order_col == "8")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Solicitud_Folio).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Solicitud_Folio).ToList();
                    else if (order_col == "9" || order_col == "10")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Estado_Id).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Estado_Id).ToList();


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
    public void listar(string order_col, string order_dir, string Folio, string Serie, string Fecha, string FechaR, string VendID, string Total, string UUID, string Status, int start, int length)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<FacturaDTO> list_dto = new List<FacturaDTO>();
                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                Fecha = Tools.ObtenerFechaEnFormatoNew(Fecha);
                FechaR = Tools.ObtenerFechaEnFormatoNew(FechaR);

                list_dto = Facturas.ObtenerFacturas(VendID, Folio, Serie, Fecha, FechaR, Total, UUID, Status);

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
                    list_dto[i].Subtotal = Simbol + " " + Convert.ToDecimal(list_dto[i].Subtotal).ToString("#,##0.00");
                    list_dto[i].Total = Simbol + " " + Convert.ToDecimal(list_dto[i].Total).ToString("#,##0.00");
                    list_dto[i].Traslados = Simbol + " " + Convert.ToDecimal(list_dto[i].Traslados).ToString("#,##0.00");
                }


                if (list_dto != null)
                {
                    int total = list_dto.Count();

                    if (order_col == "1")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Proveedor).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Proveedor).ToList();
                    else if (order_col == "2")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Folio).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Folio).ToList();
                    else if (order_col == "3")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Serie).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Serie).ToList();
                    else if (order_col == "4")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.DateR).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.DateR).ToList();
                    else if (order_col == "5")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Date).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Date).ToList();
                    else if (order_col == "6")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Subtotal_In_Double).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Subtotal_In_Double).ToList();
                    else if (order_col == "7")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Traslados_In_Double).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Traslados_In_Double).ToList();
                    else if (order_col == "8")
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
    public void listar_sin_contrarecibo(string order_col, string order_dir, string Folio, string Serie, string Fecha, string FechaAP, string VendID, string Total, string UUID, string Status, int start, int length)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<FacturaDTO> list_dto = new List<FacturaDTO>();

                Fecha = Tools.ObtenerFechaEnFormatoNew(Fecha);
                FechaAP = Tools.ObtenerFechaEnFormatoNew(FechaAP);

                list_dto = Facturas.ObtenerFacturasSinContrarecibo(VendID, Folio, Serie, Fecha, FechaAP, Total, UUID, Status);

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
                        list_dto[i].Subtotal = Simbol + " " + Convert.ToDecimal(list_dto[i].Subtotal).ToString("#,##0.00");
                        list_dto[i].Total = Simbol + " " + Convert.ToDecimal(list_dto[i].Total).ToString("#,##0.00");
                        list_dto[i].Traslados = Simbol + " " + Convert.ToDecimal(list_dto[i].Traslados).ToString("#,##0.00");
                        list_dto[i].Retenciones = Simbol + " " + Convert.ToDecimal(list_dto[i].Retenciones).ToString("#,##0.00");
                    }

                    int total = list_dto.Count();

                    if (order_col == "1")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Compania).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Compania).ToList();
                    else if (order_col == "2")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Folio).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Folio).ToList();
                    else if (order_col == "3")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Serie).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Serie).ToList();
                    else if (order_col == "4")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.DateR).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.DateR).ToList();
                    else if (order_col == "5")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.DateA).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.DateA).ToList();
                    else if (order_col == "6")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Proveedor).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Proveedor).ToList();
                    else if (order_col == "7")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Subtotal_In_Double).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Subtotal_In_Double).ToList();
                    else if (order_col == "8")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Retenciones_In_Double).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Retenciones_In_Double).ToList();
                    else if (order_col == "9")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Traslados_In_Double).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Traslados_In_Double).ToList();
                    else if (order_col == "10")
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
    public bool es_proveedor()
    {
        return Tools.EsProveedor();
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void listar_estado_factura_All(string order_col, string order_dir, string VendorId, string Folio, string Fecha, string FechaR, string FechaPP, string FechaP, string FolioP, string Banco, string contrarecibo, string solicitud, string Estado, int start, int length, string Cont)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<FacturasAllDTO> list_dto = new List<FacturasAllDTO>();

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (Cont == "1")
                {
                    var result = new
                    {
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = list_dto
                        //error = "Ingrese Filtros para la Busqueda"
                    };
                    Context.Response.Write(js.Serialize(result));
                }
                else
                {
                    Fecha = Tools.ObtenerFechaEnFormatoNew(Fecha);
                    FechaR = Tools.ObtenerFechaEnFormatoNew(FechaR);
                    FechaP = Tools.ObtenerFechaEnFormatoNew(FechaP);
                    FechaPP = Tools.ObtenerFechaEnFormatoNew(FechaPP);

                    Facturas.ActualizarEstadoFacturasSql();

                    list_dto = FacturasAll.ObtenerFacturas(VendorId, Folio, Fecha, FechaR, FechaPP, FechaP, FolioP, Banco, contrarecibo, solicitud, Estado);

                    for (int i = 0; i <= list_dto.Count() - 1; i++)
                    {
                        string regin = "";
                        if (list_dto[i].Moneda == "MXN")
                        {
                            regin = "es-MX";
                        }
                        else if (list_dto[i].Moneda == "USD")
                        {
                            regin = "en-US";
                        }
                        else if (list_dto[i].Moneda == "EUR")
                        {
                            regin = "fr-FR";
                        }

                        CultureInfo gbCulture = new CultureInfo(regin);
                        string Simbol = gbCulture.NumberFormat.CurrencySymbol;
                        list_dto[i].Subtotal = Simbol + " " + Convert.ToDecimal(list_dto[i].Subtotal).ToString("#,##0.00");
                        list_dto[i].Traslados = Simbol + " " + Convert.ToDecimal(list_dto[i].Traslados).ToString("#,##0.00");
                        list_dto[i].Total = Simbol + " " + Convert.ToDecimal(list_dto[i].Total).ToString("#,##0.00");
                    }

                    if (list_dto != null)
                    {
                        foreach (FacturasAllDTO factura in list_dto)
                        {
                            string path = "..\\..\\Img\\estados\\" + (string.IsNullOrWhiteSpace(factura.Estado_Id) ? "0.png" : factura.Estado_Id + ".png");
                            factura.Estado_Img = "<img src=\"" + path + "\"style=\"width: 50px; height 50px \">";
                        }

                        int total = list_dto.Count();

                        if (order_col == "1")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.Serie).ToList();
                            else
                                list_dto = list_dto.OrderBy(l => l.Serie).ToList();
                        else if (order_col == "2")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.Folio).ToList();
                            else
                                list_dto = list_dto.OrderBy(l => l.Folio).ToList();
                        else if (order_col == "3")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.Proveedor_Nombre).ToList();
                            else
                                list_dto = list_dto.OrderBy(l => l.Proveedor_Nombre).ToList();
                        else if (order_col == "4")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.DateFF).ToList(); // Fecha de Factura
                            else
                                list_dto = list_dto.OrderBy(l => l.DateFF).ToList();
                        else if (order_col == "5")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.DateRc).ToList(); // Fecha de Recepcion
                            else
                                list_dto = list_dto.OrderBy(l => l.DateRc).ToList();
                        else if (order_col == "6")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.DateAr).ToList(); // Fecha de Aprobacion
                            else
                                list_dto = list_dto.OrderBy(l => l.DateAr).ToList();
                        else if (order_col == "7")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.Subtotal_In_Doouble).ToList();
                            else
                                list_dto = list_dto.OrderBy(l => l.Subtotal_In_Doouble).ToList();
                        else if (order_col == "8")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.Traslados_In_Double).ToList();
                            else
                                list_dto = list_dto.OrderBy(l => l.Traslados_In_Double).ToList();
                        else if (order_col == "9")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.Total_In_Double).ToList();
                            else
                                list_dto = list_dto.OrderBy(l => l.Total_In_Double).ToList();
                        else if (order_col == "10")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.Contrarecibo_Folio).ToList();
                            else
                                list_dto = list_dto.OrderBy(l => l.Contrarecibo_Folio).ToList();
                        else if (order_col == "11")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.Solicitud_Folio).ToList();
                            else
                                list_dto = list_dto.OrderBy(l => l.Solicitud_Folio).ToList();

                        else if (order_col == "12")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.DatePr).ToList(); // Fecha Programada de Pago
                            else
                                list_dto = list_dto.OrderBy(l => l.DatePr).ToList();

                        else if (order_col == "13")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.DatePag).ToList(); // Fecha de Pago
                            else
                                list_dto = list_dto.OrderBy(l => l.DatePag).ToList();

                        else if (order_col == "14")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.Banco_Pago).ToList();  // Banco de Pago
                            else
                                list_dto = list_dto.OrderBy(l => l.Banco_Pago).ToList();


                        else if (order_col == "15")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.Cuenta_Pago).ToList();  // Cuenta
                            else
                                list_dto = list_dto.OrderBy(l => l.Cuenta_Pago).ToList();

                        else if (order_col == "16")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.DateNP).ToList(); // Fecha Notificacion de Pago
                            else
                                list_dto = list_dto.OrderBy(l => l.DateNP).ToList();

                        else if (order_col == "17")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.Folio_Pago).ToList(); // Folio de Pago
                            else
                                list_dto = list_dto.OrderBy(l => l.Folio_Pago).ToList();

                        else if (order_col == "18")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.DateRcP).ToList(); // Fecha Recepcion de Pago
                            else
                                list_dto = list_dto.OrderBy(l => l.DateRcP).ToList();

                        else if (order_col == "19")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.DateArP).ToList(); // Fecha Aprobacion de Pago
                            else
                                list_dto = list_dto.OrderBy(l => l.DateArP).ToList();

                        else if (order_col == "20" || order_col == "21")
                            if (order_dir == "desc")
                                list_dto = list_dto.OrderByDescending(l => l.Estado_Id).ToList();  // Estado
                            else
                                list_dto = list_dto.OrderBy(l => l.Estado_Id).ToList();


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
    public void listar20(string order_col, string order_dir, string Folio, string Serie, string Fecha, string FechaR, string VendID, string Total, string UUID, string Status, int start, int length, string Cont)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                List<FacturaDTOs> list_dto = new List<FacturaDTOs>();
                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (Cont == "1")
                {
                    var result = new
                    {
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = list_dto
                        //error = "Ingrese Filtros para la Busqueda"
                    };
                    Context.Response.Write(js.Serialize(result));
                }
                else
                {


                    Fecha = Tools.ObtenerFechaEnFormatoNew(Fecha);
                    FechaR = Tools.ObtenerFechaEnFormatoNew(FechaR);

                    list_dto = Facturas.ObtenerFacturas20(VendID, Folio, Serie, Fecha, FechaR, Total, UUID, Status, order_col, order_dir);

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
                        list_dto[i].Subtotal = Simbol + " " + Convert.ToDecimal(list_dto[i].Subtotal).ToString("#,##0.00");
                        list_dto[i].Total = Simbol + " " + Convert.ToDecimal(list_dto[i].Total).ToString("#,##0.00");
                        list_dto[i].Traslados = Simbol + " " + Convert.ToDecimal(list_dto[i].Traslados).ToString("#,##0.00");
                    }

                    if (list_dto != null)
                    {
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
    public void listar_estado_factura_All20(string order_col, string order_dir, string VendorId, string Folio, string Fecha, string FechaR, string FechaPP, string FechaP, string FolioP, string Banco, string contrarecibo, string solicitud, string Estado, int start, int length, string Cont)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                //List<FacturasAllDTOs> list_dto = new List<FacturasAllDTOs>();
                List<Inv2020> Lista_Facts = new List<Inv2020>();

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (Cont == "1")
                {
                    var result = new
                    {
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        //data = list_dto
                        data = Lista_Facts
                        //error = "Ingrese Filtros para la Busqueda"
                    };
                    Context.Response.Write(js.Serialize(result));
                }
                else
                {
                    Fecha = Tools.ObtenerFechaEnFormatoNew(Fecha);
                    FechaR = Tools.ObtenerFechaEnFormatoNew(FechaR);
                    FechaP = Tools.ObtenerFechaEnFormatoNew(FechaP);
                    FechaPP = Tools.ObtenerFechaEnFormatoNew(FechaPP);

                    Facturas.ActualizarEstadoFacturasSql();

                    Lista_Facts = FacturasAll.ObtenerFacturas20(order_col, order_dir, VendorId, Folio, Fecha, FechaR, FechaPP, FechaP, FolioP, Banco, contrarecibo, solicitud, Estado);

                    //for (int i = 0; i <= list_dto.Count() - 1; i++)
                    //{
                    //    string regin = "";
                    //    if (list_dto[i].Moneda == "MXN")
                    //    {
                    //        regin = "es-MX";
                    //    }
                    //    else if (list_dto[i].Moneda == "USD")
                    //    {
                    //        regin = "en-US";
                    //    }
                    //    else if (list_dto[i].Moneda == "EUR")
                    //    {
                    //        regin = "fr-FR";
                    //    }

                    //    CultureInfo gbCulture = new CultureInfo(regin);
                    //    string Simbol = gbCulture.NumberFormat.CurrencySymbol;
                    //    list_dto[i].Subtotal = Simbol + " " + Convert.ToDecimal(list_dto[i].Subtotal).ToString("#,##0.00");
                    //    list_dto[i].Traslados = Simbol + " " + Convert.ToDecimal(list_dto[i].Traslados).ToString("#,##0.00");
                    //    list_dto[i].Total = Simbol + " " + Convert.ToDecimal(list_dto[i].Total).ToString("#,##0.00");
                    //}

                    if (Lista_Facts != null)
                    {
                        //foreach (FacturasAllDTOs factura in list_dto)
                        //{
                        //    string path = "..\\..\\Img\\estados\\" + (string.IsNullOrWhiteSpace(factura.Estado_Id) ? "0.png" : factura.Estado_Id + ".png");
                        //    factura.Estado_Img = "<img src=\"" + path + "\"style=\"width: 50px; height 50px \">";
                        //}

                        int total = Lista_Facts.Count();

                        //if (order_col == "1")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.Serie).ToList();
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.Serie).ToList();
                        //else if (order_col == "2")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.Folio).ToList();
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.Folio).ToList();
                        //else if (order_col == "3")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.Proveedor_Nombre).ToList();
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.Proveedor_Nombre).ToList();
                        //else if (order_col == "4")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.DateFF).ToList(); // Fecha de Factura
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.DateFF).ToList();
                        //else if (order_col == "5")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.DateRc).ToList(); // Fecha de Recepcion
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.DateRc).ToList();
                        //else if (order_col == "6")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.DateAr).ToList(); // Fecha de Aprobacion
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.DateAr).ToList();
                        //else if (order_col == "7")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.Subtotal_In_Doouble).ToList();
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.Subtotal_In_Doouble).ToList();
                        //else if (order_col == "8")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.Traslados_In_Double).ToList();
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.Traslados_In_Double).ToList();
                        //else if (order_col == "9")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.Total_In_Double).ToList();
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.Total_In_Double).ToList();
                        //else if (order_col == "10")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.Contrarecibo_Folio).ToList();
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.Contrarecibo_Folio).ToList();
                        //else if (order_col == "11")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.Solicitud_Folio).ToList();
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.Solicitud_Folio).ToList();

                        //else if (order_col == "12")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.DatePr).ToList(); // Fecha Programada de Pago
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.DatePr).ToList();

                        //else if (order_col == "13")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.DatePag).ToList(); // Fecha de Pago
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.DatePag).ToList();

                        //else if (order_col == "14")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.Banco_Pago).ToList();  // Banco de Pago
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.Banco_Pago).ToList();


                        //else if (order_col == "15")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.Cuenta_Pago).ToList();  // Cuenta
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.Cuenta_Pago).ToList();

                        //else if (order_col == "16")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.DateNP).ToList(); // Fecha Notificacion de Pago
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.DateNP).ToList();

                        //else if (order_col == "17")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.Folio_Pago).ToList(); // Folio de Pago
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.Folio_Pago).ToList();

                        //else if (order_col == "18")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.DateRcP).ToList(); // Fecha Recepcion de Pago
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.DateRcP).ToList();

                        //else if (order_col == "19")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.DateArP).ToList(); // Fecha Aprobacion de Pago
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.DateArP).ToList();

                        //else if (order_col == "20" || order_col == "21")
                        //    if (order_dir == "desc")
                        //        list_dto = list_dto.OrderByDescending(l => l.Estado_Id).ToList();  // Estado
                        //    else
                        //        list_dto = list_dto.OrderBy(l => l.Estado_Id).ToList();


                        Lista_Facts = length == -1 ? Lista_Facts.Skip(start).ToList() : Lista_Facts.Skip(start).Take(length).ToList();
                        int cantidad = Lista_Facts.Count();

                        //list_dto = length == -1 ? list_dto.Skip(start).ToList() : list_dto.Skip(start).Take(length).ToList();
                        //int cantidad = list_dto.Count();

                        var result = new
                        {
                            recordsTotal = cantidad,
                            recordsFiltered = total,
                            //data = list_dto
                            data = Lista_Facts
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
    public void Actulizar22(string bytes1, string Cont)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null)
            {
                var js = new JavaScriptSerializer();

                string usker = HttpContext.Current.Session["UserKey"].ToString();

                var Pago = Facturas.ActualizarPagoFacturas(bytes1, Cont);

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (Pago == "Ok")
                {
                    var result = new
                    {
                        success = true,
                        user = usker,
                    };
                    Context.Response.Write(js.Serialize(result));
                }
                else
                {

                    var result = new
                    {
                        error = Pago
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
}
