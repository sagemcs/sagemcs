//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS
using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class ProveedorDocumentoDTO
{
    public string ID_Documento { get; set; }
    public string ID_Proveedor { get; set; }
    public string Social { get; set; }
    public string Compania { get; set; }
    public string Descripcion { get; set; }
    public string Fecha { get; set; }
    public DateTime Date { get; set; }
    public string Actualizacion { get; set; }
    public string Estado { get; set; }
    public string Estado_Id { get; set; }
    public string Usuario { get; set; }

    public ProveedorDocumentoDTO()
    { }

    public ProveedorDocumentoDTO(string ID_Documento, string ID_Proveedor, string Social, string Compañía, string Descripción, string Fecha, string Actualización, string Estado_Id, string Estado, string Usuario, DateTime Dat)
    {
        this.ID_Documento = ID_Documento;
        this.ID_Proveedor = ID_Proveedor;
        this.Social = Social;
        this.Compania = Compañía;
        this.Descripcion = Descripción;
        this.Fecha = Fecha;
        this.Date = Dat == null ? DateTime.MinValue : Convert.ToDateTime(Dat);
        this.Actualizacion = Actualización;
        this.Estado_Id = Estado_Id;
        this.Estado = Estado;
        this.Usuario = Usuario;
    }
}

public class ProveedorDocumentos
{
    public ProveedorDocumentos()
    {
    }

    public static List<ProveedorDocumentoDTO> ObtenerDocumentos(bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();

            if (company == null)
                return null;
            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            Expression<Func<Documents, bool>> predicate = null;

            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
            {
                predicate = (a => a.CompanyID == company.CompanyID);
            }
            else
            {
                return null;
            }
            //else
            //{
            //    List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
            //    predicate = (a => a.CompanyID == company.CompanyID && VendorsIds.Contains(a.VendorKey));
            //}

            PortalProveedoresEntities db = new PortalProveedoresEntities();


            List<Documents> list = db.Documents.Where(predicate).ToList();
            List<ProveedorDocumentoDTO> documentos = new List<ProveedorDocumentoDTO>();
            foreach (Documents doc in list)
            {
                try
                {
                    StatusDocs status = null;
                    if (doc.Status != null)
                        status = db.StatusDocs.Where(s => s.Status == doc.Status).FirstOrDefault();
                    //status = db.StatusDocs.Where(s => 4 == doc.Status).FirstOrDefault();

                    documentos.Add(new ProveedorDocumentoDTO(
                        doc.DocID,
                        doc.Vendors != null ? doc.Vendors.VendorID : "",
                        doc.Vendors != null ? doc.Vendors.VendName : "",
                        company.CompanyID,
                        doc.DocDescription,
                        doc.CreateDate != null ? doc.CreateDate.Date.ToString("dd/MM/yyyy") : "",
                        doc.UpdateDate != null ? doc.UpdateDate.Value.ToString("dd/MM/yyyy") : "",
                        status != null ? status.Status.ToString() : "0",
                        status != null ? status.Descripcion : "No definido",
                        doc.Users != null ? doc.Users.UserID : "",
                        doc.UpdateDate != null ? doc.UpdateDate.Value : DateTime.MinValue));
                }
                catch
                { }
            }

            return documentos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ProveedorDocumentoDTO>();
        }
    }

    public static List<ProveedorDocumentoDTO> ObtenerDocumentos(string ID, string Proveedor, string Social, string Estado, bool directo_en_vista = false)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Estado) && Estado == "0")
                Estado = "";
            List<ProveedorDocumentoDTO> documentos = ObtenerDocumentos();

            if (documentos == null)
                return null;

            if (!string.IsNullOrWhiteSpace(ID) && ID != "null")
                documentos = documentos.Where(d => d.ID_Documento.ToUpper().Contains(ID.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Proveedor) && Proveedor != "null")
                documentos = documentos.Where(d => d.ID_Proveedor.ToUpper().Contains(Proveedor.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Social) && Social != "null")
                documentos = documentos.Where(d => d.Social.ToUpper().Contains(Social.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Estado) && Estado != "null")
                documentos = documentos.Where(d => d.Estado_Id == Estado).ToList();

            return documentos;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ProveedorDocumentoDTO>();
        }
    }
}
