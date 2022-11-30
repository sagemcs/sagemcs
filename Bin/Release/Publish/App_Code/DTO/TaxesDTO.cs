using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TaxesDTO
/// </summary>
public class TaxesDTO
{

    public int? TaxId { get; set; }
    public int STaxClassKey { get; set; }
    public string STaxClassID { get; set; }
    public short STaxCodeClass { get; set; }
    public int UpdateCounter { get; set; }
    public int STaxCodeKey { get; set; }
    public string STaxCodeID { get; set; } 
    public string Description { get; set; }
    public decimal Rate { get; set; }
    public TaxesDTO()
    {
        // dbo.tciSTaxSubjClassDt.STaxCodeKey, dbo.tciSTaxCode.STaxCodeID, dbo.tciSTaxClass.STaxClassKey, dbo.tciSTaxCode.Description, dbo.tciSTaxSubjClassDt.Rate

        // TODO: Add constructor logic here
        //
    }
}