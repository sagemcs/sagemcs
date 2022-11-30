//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
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

/// <summary>
/// Summary description for PagosWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class PagosWebService : System.Web.Services.WebService
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    public PagosWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void listar(string order_col, string order_dir, string Folio, string Serie, string Fecha, string FechaR, string VendID, string Total, string UUID, string Status, int start, int length)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null && HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
            {

                List<PagoDTO> list_dto = new List<PagoDTO>();

                Fecha = Tools.ObtenerFechaEnFormatoNew(Fecha);
                FechaR = Tools.ObtenerFechaEnFormatoNew(FechaR);

                list_dto = Pagos.ObtenerPagos(Folio, Serie, Fecha, FechaR, VendID, Total, UUID, Status);

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (list_dto != null)
                {
                    int total = list_dto.Count();

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
                    }

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
                            list_dto = list_dto.OrderByDescending(l => l.Total_In_Double).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Total_In_Double).ToList();
                    else if (order_col == "8")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Estado).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Estado).ToList();

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

}
