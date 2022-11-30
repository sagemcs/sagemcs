//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : ADRIAN QUIALA

//REFERENCIAS UTILIZADAS

using System;
using System.IO;
using System.Net;
using System.Net.Mail;

public static class ConfiguracionCorreoElectronico
{
    public static string server_address { get { return "smtp.gmail.com"; } }
    public static int server_port { get { return Convert.ToInt16("587"); } }
    public static string user { get { return "lgarcia@multiconsulting.com"; } }
    public static string password { get { return "Resistance8"; } }
    public static bool enable_ssl { get { return true; } }
}
public class CorreoElectronico
{
    SmtpClient smtpClient = new SmtpClient();

    #region Constructor

    public CorreoElectronico(string server_address, int server_port, string user, string password, bool enable_ssl = true)
    {
        this.smtpClient.Credentials = new NetworkCredential(user, password);
        this.smtpClient.Host = server_address;
        this.smtpClient.Port = server_port;
        this.smtpClient.EnableSsl = enable_ssl;
    }

    #endregion

    #region Métodos Públicos

    public bool Enviar(string from, string to, string subject, string text, bool is_text_html, Stream attachment_data = null, string attachment_name = null)
    {
        try
        {
            MailMessage message = new MailMessage(from, to, subject, text);
            message.Priority = MailPriority.Normal;
            message.IsBodyHtml = is_text_html;

            if (attachment_data != null && !string.IsNullOrWhiteSpace(attachment_name))
            {
                Attachment attachment_file = new Attachment(attachment_data, attachment_name);
                message.Attachments.Add(attachment_file);
            }

            this.smtpClient.Send(message);
            return true;
        }
        catch (Exception Ex)
        {
            string ss = Ex.ToString();
            return false;
        }
    }

    #endregion
}