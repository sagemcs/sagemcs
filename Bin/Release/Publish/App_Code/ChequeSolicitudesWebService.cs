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
/// Summary description for ChequeSolicitudesWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class ChequeSolicitudesWebService : System.Web.Services.WebService
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    public ChequeSolicitudesWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void listar(string order_col, string order_dir, string Serie, string VendID, string UserID, string Total, string ChkReqDate, string PagoDate, int start, int length)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null && HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
            {
                List<ChequeSolicitudesDTO> list_dto = new List<ChequeSolicitudesDTO>();
                ChkReqDate = Tools.ObtenerFechaEnFormatoNew(ChkReqDate);
                PagoDate = Tools.ObtenerFechaEnFormatoNew(PagoDate);

                list_dto = ChequeSolicitudess.ObtenerSolicitudesCheque(Serie, VendID, UserID, Total, ChkReqDate, PagoDate);

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


                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (list_dto != null)
                {
                    int total = list_dto.Count();

                    if (order_col == "1")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Serie).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Serie).ToList();
                    else if (order_col == "2")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Proveedor).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Proveedor).ToList();
                    else if (order_col == "3")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Solicitante).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Solicitante).ToList();
                    else if (order_col == "4")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Total_In_Double).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Total_In_Double).ToList();
                    else if (order_col == "5")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Date).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Date).ToList();
                    else if (order_col == "6")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.DatePr).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.DatePr).ToList();

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
    public void generar(string ids, string comentario)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null && HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
            {
                var js = new JavaScriptSerializer();
                var UUIDs_obj = (IEnumerable<object>)js.DeserializeObject(ids);
                List<int> UUIDs = new List<int>();
                foreach (var item in UUIDs_obj)
                {
                    UUIDs.Add(Convert.ToInt32(item.ToString()));
                }

                bool success = true;
                int id = -1;
                string msg = "";
                try
                {
                    string rpt_path = Path.Combine(Server.MapPath("~/Reports"), "SolicitudChequeReport.rpt");
                    id = ChequeSolicitudes.SalvarSolicitudCheque(UUIDs, comentario, rpt_path);
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
