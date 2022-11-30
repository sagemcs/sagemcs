//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS
using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Summary description for EstadoArticulosDTO
/// </summary>
public class EstadoArticulossDTO : EstadoDTO
{
    public EstadoArticulossDTO()
    {
    }

    public EstadoArticulossDTO(string Id, string Descripcion) : base(Id, Descripcion)
    {
    }

    public EstadoArticulossDTO(int Id, string Descripcion) : base(Id, Descripcion)
    {
    }
}

public class EstadosArticuloss
{
    public EstadosArticuloss()
    {
    }

    public static List<EstadoArticulossDTO> Obtener()
    {
        try
        {
            PortalProveedoresEntities db = new PortalProveedoresEntities();
            List<tapLFStatus> list = db.tapLFStatus.ToList();
            List<EstadoArticulossDTO> list_dto = new List<EstadoArticulossDTO>();

            foreach (var item in list)
                list_dto.Add(new EstadoArticulossDTO(item.Status, item.Descripcion));
            //list_dto.Add(new EstadoArticulossDTO("1", "Pendiente"));
            //list_dto.Add(new EstadoArticulossDTO("2", "Aprobado"));
            //list_dto.Add(new EstadoArticulossDTO("4", "Rechazado"));

            return list_dto;
        }
        catch (Exception e)
        {
            throw new MulticonsultingException(e.ToString());
        }
    }
}