/// <summary>
/// Summary description for ValidatingUserDTO
/// </summary>
public class ValidatingUserDTO
{
    public int UserKey { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public int ValidadorCx { get; set; }
    public int Gerente { get; set; }
    public int Rh { get; set; }
    public int Tesoreria { get; set; }
    public int Finanzas { get; set; }


    public ValidatingUserDTO()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}