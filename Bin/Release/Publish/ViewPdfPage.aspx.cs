using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ViewPdfPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            string filePath = Request.QueryString["fileName"];
            string ext;

            if (filePath.Length == 0)
                {
                return;
            }

            int startIndex = filePath.Length - 3;
            ext=filePath.Substring(startIndex, 3);

            if (ext =="pdf")
            {
                Response.ContentType = "Application/pdf";
                Response.WriteFile(Server.MapPath("./") + @"Archivos\Proveedores\" + filePath);
            }
            if (ext == "xml")
            {
                Response.ContentType = "text/xml";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.WriteFile(Server.MapPath("./") + @"Archivos\Proveedores\" + filePath);

            }

            Response.End();
        }
    }
}