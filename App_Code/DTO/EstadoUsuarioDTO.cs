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
/// Summary description for EstadoUsuarioDTO
/// </summary>
public class EstadoUsuarioDTO : EstadoDTO
{
    public EstadoUsuarioDTO()
    {

    }

    public EstadoUsuarioDTO(string Id, string Descripcion) : base(Id, Descripcion)
    {
    }

    public EstadoUsuarioDTO(int Id, string Descripcion) : base(Id, Descripcion)
    {
    }
}

public class EstadosUsuario
{
    public EstadosUsuario()
    {
    }

    public static List<EstadoUsuarioDTO> Obtener(bool directo_en_vista = false)
    {
        try
        {
            PortalProveedoresEntities db = new PortalProveedoresEntities();
            List<StatusUsers> list = db.StatusUsers.ToList();
            List<EstadoUsuarioDTO> list_dto = new List<EstadoUsuarioDTO>();

            foreach (var item in list)
                list_dto.Add(new EstadoUsuarioDTO(item.Status, item.Descripcion));

            return list_dto;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<EstadoUsuarioDTO>();
        }
    }
}