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
/// Summary description for EstadoDocumentoDTO
/// </summary>
public class EstadoDocumentosDTO : EstadoDTO
{
    public EstadoDocumentosDTO()
    {
    }

    public EstadoDocumentosDTO(string Id, string Descripcion) : base(Id, Descripcion)
    {
    }

    public EstadoDocumentosDTO(int Id, string Descripcion) : base(Id, Descripcion)
    {
    }
}

public class EstadosDocumentos
{
    public EstadosDocumentos()
    {
    }

    public static List<EstadoDocumentosDTO> Obtener()
    {
        try
        {
            PortalProveedoresEntities db = new PortalProveedoresEntities();
            List<StatusDocs> list = db.StatusDocs.ToList();
            List<EstadoDocumentosDTO> list_dto = new List<EstadoDocumentosDTO>();

            //foreach (var item in list)
            //list_dto.Add(new EstadoDocumentosDTO(item.Status, item.Descripcion));
            list_dto.Add(new EstadoDocumentosDTO("1", "Pendiente"));
            list_dto.Add(new EstadoDocumentosDTO("2", "Aprobado"));
            list_dto.Add(new EstadoDocumentosDTO("3", "Rechazado"));

            return list_dto;
        }
        catch (Exception e)
        {
            throw new MulticonsultingException(e.ToString());
        }
    }
}