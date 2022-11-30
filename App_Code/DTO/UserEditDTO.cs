using System;

/// <summary>
/// Summary description for UserEditDTO
/// </summary>
public class UserEditDTO
{
    //UserKey,UserID,UserName,CreateDate,UpdateDate
    public int UserKey { get; set; }
    public string UserID { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public int RoleKey { get; set; }
    public string Rol { get; set; }
    public string RazonSocial { get; set; }
    public string RFC { get; set; }
    public string EmpresaDestino { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public UserEditDTO()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}