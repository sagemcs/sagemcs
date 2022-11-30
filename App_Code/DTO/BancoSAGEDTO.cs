//PORTAL DE PROVEDORES T|SYS|
//08 ABRIL DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA P.

//REFERENCIAS UTILIZADAS
using SAGE_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BancoSAGEDTO
/// </summary>
public class BancoSAGEDTO : EstadoDTO
{
    public BancoSAGEDTO()
    {
    }

    public BancoSAGEDTO(string Id, string Descripcion) : base(Id, Descripcion)
    {
    }

    public BancoSAGEDTO(int Id, string Descripcion) : base(Id, Descripcion)
    {
    }
}

public class BancoSAGE
{
    public BancoSAGE()
    {
    }

    public static List<BancoSAGEDTO> Obtener()
    {
        try
        {

            sage500_appEntities db_sage = new sage500_appEntities();
            List<SAGE_Model.tcmBank> list = db_sage.tcmBank.ToList();
            List<BancoSAGEDTO> list_dto = new List<BancoSAGEDTO>();
            string company = HttpContext.Current.Session["IDCompany"].ToString();

            foreach (var item in list)
                if (item.CompanyID == company.Trim())
                {
                    list_dto.Add(new BancoSAGEDTO(item.BankID, item.BankID));
                }
            return list_dto;
        }
        catch (Exception e)
        {
            throw new MulticonsultingException(e.ToString());
        }
    }
}