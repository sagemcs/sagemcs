using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logged_Administradores_Validador_Default : System.Web.UI.Page
{
    string Texr = string.Empty;
    int Im = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        RevisaNotificaciones();
    }
    protected void RevisaNotificaciones()
    {
        try
        {
            Im = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PortalConnection"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("spNotificaciones", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Titulo", Value = "11" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Desde", Value = "01/01/2020" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Hasta", Value = "01/01/2020" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Estilo", Value = "11" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Estatus", Value = "1" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Mensaje", Value = "1" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Userkey", Value = "1" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Url", Value = "1" });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Opcion", Value = 4 });

                if (conn.State == ConnectionState.Open) { conn.Close(); }
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                string titulo, Msj, tipo, Img;

                while (rdr.Read())
                {
                    Msj = Convert.ToString(rdr.GetValue(0));
                    tipo = Convert.ToString(rdr.GetValue(1));
                    titulo = Convert.ToString(rdr.GetValue(2));
                    if (Convert.ToString(rdr.GetValue(3)) != "")
                    {
                        Img = Convert.ToString(rdr.GetValue(3));
                        Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "', imageUrl: '" + Img + "'  },";
                        Im = +1;
                    }
                    else
                    {
                        Texr = Texr + "{ title: '" + titulo + "',text: '" + Msj + "',icon: '" + tipo + "'  },";
                    }

                }
            }

        }
        catch (Exception)
        {

        }
    }
}