﻿//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS

using Proveedores_Model;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

/// <summary>
/// Summary description for ProveedoresWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class ProveedoresWebService : System.Web.Services.WebService
{
    private PortalProveedoresEntities db = new PortalProveedoresEntities();
    public ProveedoresWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true)]
    public void listar(string order_col, string order_dir, string VendID, string UserID, string VendName, string AliasDBA, string Status, int start, int length)
    {
        try
        {
            string supuesto_token = Context.Request.Headers.GetValues("Authorization").First();

            if (Tools.EsTokenValido(supuesto_token) && Tools.UsuarioAutenticado() != null && HttpContext.Current.Session["RolUser"].ToString() != "Proveedor")
            {
                List<ProveedorDTO> list_dto = new List<ProveedorDTO>();
                list_dto = Proveedores.ObtenerProveedores(VendID, UserID, VendName, AliasDBA, Status);

                var js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                if (list_dto != null)
                {
                    int total = list_dto.Count();

                    if (order_col == "1")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Nombre).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Nombre).ToList();
                    else if (order_col == "2")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Social).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Social).ToList();
                    else if (order_col == "3")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.RFC).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.RFC).ToList();
                    else if (order_col == "4")
                        if (order_dir == "desc")
                            list_dto = list_dto.OrderByDescending(l => l.Date).ToList();
                        else
                            list_dto = list_dto.OrderBy(l => l.Date).ToList();
                    else if (order_col == "5")
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

}
