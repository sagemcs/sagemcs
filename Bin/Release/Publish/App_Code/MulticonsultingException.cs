//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : ADRIAN QUIALA

//REFERENCIAS UTILIZADAS
using System;

/// <summary>
/// Summary description for MulticonsultingException
/// </summary>
public class MulticonsultingException : Exception
{
    public MulticonsultingException()
    {
    }

    public MulticonsultingException(string message) : base(message)
    {
    }

    public MulticonsultingException(string message, Exception innerException) : base(message, innerException)
    {
    }
}