using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.UI;
using WebSite1;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
public partial class Account_Register : Page
{
    protected void CreateUser_Click(object sender, EventArgs e)
    {
        var manager = new UserManager();
        var user = new ApplicationUser() { UserName = UserName.Text };
        string RFCu = RFC.Text;
        if(!VerificarRFC(user.Id.ToString(), RFCu))
        {   
            IdentityResult result = manager.Create(user, Password.Text);
            if (result.Succeeded)
            {
                IdentityHelper.SignIn(manager, user, isPersistent: false);

                manager.AddToRole(user.Id.ToString(), "Manager");
                string userid = user.UserName.ToString();
                CreateLoginSage(userid, RFCu, "Manager");
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);

            }
            else
            {
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }
        else
        {
            ErrorMessage.Text = "Ya está registrado el RFC";
        }

    }

    public static bool VerificarRFC( string userid, string RFCu)
    {
        bool res = false;
        string sSQL;
        //Proveedor = Session["Proveedor"].ToString();

        //Label4.Text = "";

        SqlCommand sqlSelectCommand1 = new SqlCommand();
        SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter();

        SqlConnection sqlConnection1 = new SqlConnection();
        ConnectionStringSettings connSettings = ConfigurationManager.ConnectionStrings["ConnectionString"];
        if ((connSettings != null) && (connSettings.ConnectionString != null))
        {
            sqlConnection1.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }

        sSQL = "Select VendRefNo from tapVendorPortalLogin WHERE VendRefNo =" + "'" + RFCu + "'";
        System.Data.DataTable TablaP = new System.Data.DataTable();

        using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
        {
            try
            {

                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = sSQL;
                System.Data.SqlClient.SqlDataAdapter Datos = new System.Data.SqlClient.SqlDataAdapter(Cmd);

                sqlConnection1.Open();
                Datos.Fill(TablaP);
                sqlConnection1.Close();
            }
            catch (Exception ex)
            {
                string err;
                err = ex.Message;
                res = false;
                return res;
            }
        }

        //string sHabilitado = TablaP.Rows[0]["Habilitado"].ToString();

        //if (sHabilitado !="" || sHabilitado.Trim().Length == 0)
        if(TablaP.Rows.Count != 0)
        { 
            if(TablaP.Rows[0]["VendRefNo"].ToString().Trim() != "")
            {
                //List<SqlParameter> pars = new List<SqlParameter>();

                //sSQL = "INSERT INTO tapVendorPortalLogin (VendRefNo, UserLogin, Rol) VALUES (@iVendRefNo,@iUserLogin,@iRol)";
                //pars.Clear();

                res = true;
                //return res;
                //pars.Add(new SqlParameter("@iVendRefNo", Lote));
                //pars.Add(new SqlParameter("@iBatchType", 401));
                //pars.Add(new SqlParameter("@iOrigUserID", "admin"));
                //pars.Add(new SqlParameter("@iPostDate", Factura.Fecha.ToShortDateString()));
                //pars.Add(new SqlParameter("@iSourceCompanyID", "NCR"));
                //pars.Add(new SqlParameter("@iTranCtrl", Convert.ToInt32(0)));
                //pars.Add(new SqlParameter("@iHold", Convert.ToInt32(0)));
                //pars.Add(new SqlParameter("@iPrivate", Convert.ToInt32(0)));

            }
        }
        else
        {
            res = false;
        }
        return res;
        //if (sHabilitado == "False")
        //{
        //    Label4.Text = "El proveedor se encuentra deshabilitado para cargar facturas.";
        //    return;
        //}
    }

    public void CreateLoginSage(string userid, string RFCu, string RolU)
    {
        string sSQL;

        SqlCommand sqlSelectCommand1 = new SqlCommand();
        SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter();

        SqlConnection sqlConnection1 = new SqlConnection();
        ConnectionStringSettings connSettings = ConfigurationManager.ConnectionStrings["ConnectionString"];
        if ((connSettings != null) && (connSettings.ConnectionString != null))
        {
            sqlConnection1.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }

        List<SqlParameter> pars = new List<SqlParameter>();
        sSQL = "INSERT INTO tapVendorPortalLogin (VendRefNo, UserLogin, Rol) VALUES (@iVendRefNo,@iUserLogin,@iRol)";
        pars.Clear();

        //return res;
        pars.Add(new SqlParameter("@iVendRefNo", RFCu));
        pars.Add(new SqlParameter("@iUserLogin", userid));
        pars.Add(new SqlParameter("@iRol", RolU));

        using (System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand(sSQL, sqlConnection1))
        {
            try
            {
                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = sSQL;

                foreach (System.Data.SqlClient.SqlParameter par in pars)
                {
                    Cmd.Parameters.AddWithValue(par.ParameterName, par.Value);
                }

                sqlConnection1.Open();

                Cmd.ExecuteNonQuery();

                sqlConnection1.Close();

            }
            catch (Exception ex)
            {
                string err;
                err = ex.Message;
                //MessageBox.Show("Error al insertar en Log de timbrado, Error: " + ex.Message);
            }
        }

    }
}