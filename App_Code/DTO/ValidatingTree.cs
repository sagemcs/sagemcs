using System.Collections.Generic;

/// <summary>
/// Summary description for ValidatingTree
/// </summary>
public class ValidatingTree
{
    public enum AreaType
    {
        RecursosHumanos = 1, GerenciaArea = 2, CuentasxPagar = 3, Tesoreria = 4, Finanzas = 5
    }
    public int TypeKey { get; set; }
    public string Description { get; set; }
    public bool RecursosHumanos { get; set; }
    public bool GerenciaArea { get; set; }
    public bool CuentasxPagar { get; set; }
    public bool Tesoreria { get; set; }
    public bool Finanzas { get; set; }

    public Dictionary<int, string> Get_Orden(ValidatingTree tree)
    {
        var dict = new Dictionary<int, string>();
        if (tree.RecursosHumanos)
        {
            dict.Add(dict.Count + 1, "RecursosHumanos");
        }
        if (tree.GerenciaArea)
        {
            dict.Add(dict.Count + 1, "GerenciaArea");
        }
        if (tree.CuentasxPagar)
        {
            dict.Add(dict.Count + 1, "CuentasxPagar");
        }
        if (tree.Tesoreria)
        {
            dict.Add(dict.Count + 1, "Tesoreria");
        }
        if (tree.Finanzas)
        {
            dict.Add(dict.Count + 1, "Finanzas");
        }

        return dict;
    }

    public ValidatingTree()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}