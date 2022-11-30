using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logged_Administradores_TipoC : System.Web.UI.Page
{
    string titulo,Msj,tipo;

    protected void Page_Load(object sender, EventArgs e)
    {
        //txtdtp.Visible = false;
    }

    protected void Obtener(object sender, EventArgs e)
    {
        //if (txtdtp.Text == "")
        //{
        //    tipo = "error";
        //    Msj = "Selecciona una fecha para el tipo de cambio";
        //    titulo = "Tipo de Cambio Banxico";
        //    ClientScript.RegisterStartupScript(this.GetType(), "ramdomtext", "alertme('" + titulo + "','" + Msj + "','" + tipo + "');", true);
        //}
        //else
        //{
           //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "TipoC();", true);

           ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ramdomtext", "getDataWebService();", true);
        //}
    }
}