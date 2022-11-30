using System;

/// <summary>
/// Summary description for EmpleadoDTO
/// </summary>
public class EmpleadoDTO
{
    public int IdEmpleado { get; set; }
    public int UserKey { get; set; }
    public string Correo { get; set; }
    public string Nombre { get; set; }
    public string Puesto { get; set; }
    public string Area { get; set; }
    public string GefeInmediato { get; set; }
    public string CorreoGefe { get; set; }
    public string Motivos { get; set; }
    public DateTime Fecha { get; set; }
    public EmpleadoDTO()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}