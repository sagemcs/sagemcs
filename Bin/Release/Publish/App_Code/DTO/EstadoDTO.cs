//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS

/// <summary>
/// Summary description for EstadoDTO
/// </summary>
public abstract class EstadoDTO
{
    public string Id { get; set; }
    public string Descripcion { get; set; }
    public EstadoDTO()
    {
    }

    public EstadoDTO(string Id, string Descripcion)
    {
        this.Id = Id;
        this.Descripcion = Descripcion;
    }

    public EstadoDTO(int Id, string Descripcion)
    {
        this.Id = Id.ToString();
        this.Descripcion = Descripcion;
    }
}