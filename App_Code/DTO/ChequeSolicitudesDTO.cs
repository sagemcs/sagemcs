//PORTAL DE PROVEDORES T|SYS|
//12 MARZO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

//REFERENCIAS UTILIZADAS
using CrystalDecisions.CrystalReports.Engine;
using Proveedores_Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;


/// <summary>
/// Summary description for SolicitudChequeDTO
/// </summary>
public class ChequeSolicitudesDTO
{
    public string Id { get; set; }
    public string Serie { get; set; }
    public string Proveedor { get; set; }
    public string Solicitante { get; set; }
    public string Total { get; set; }
    public double Total_In_Double { get; set; }
    public string Fecha { get; set; }
    public DateTime Date { get; set; }
    public string Fecha_Programada_Pago { get; set; }
    public DateTime DatePr { get; set; }
    public string NombreSolicitante { get; set; }
    public string Moneda { get; set; }

    public ChequeSolicitudesDTO()
    { }

    public ChequeSolicitudesDTO(string Id, string Serie, string Proveedor, string Solicitante, string Total, string Fecha, string Fecha_Programada_Pago, string NombreSolicitante, string Moneda, DateTime Date, DateTime DateR)
    {
        this.Id = Id;
        this.Serie = Serie;
        this.Proveedor = Proveedor;
        this.Solicitante = Solicitante;
        this.Total = Total;
        this.Total_In_Double = Convert.ToDouble(Total);
        this.Fecha = Fecha;
        this.Fecha_Programada_Pago = Fecha_Programada_Pago;
        this.NombreSolicitante = NombreSolicitante;
        this.Moneda = Moneda;
        this.Date = Date == null ? DateTime.MinValue : Convert.ToDateTime(Date);
        this.DatePr = DateR == null ? DateTime.MinValue : Convert.ToDateTime(DateR);
    }
}

public class ChequeSolicitudess
{
    public ChequeSolicitudess()
    { }

    public static List<ChequeSolicitudesDTO> ObtenerSolicitudesCheque(bool directo_en_vista = false)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                return null;

            Users user = Tools.UsuarioAutenticado();
            if (user == null)
                return null;

            Expression<Func<CheckRequest, bool>> predicate;
            bool is_tsys_user = user.UsersInRoles.Where(r => r.Roles.RoleID.Contains("T|SYS|")).FirstOrDefault() != null;
            if (is_tsys_user)
                predicate = (a => a.CompanyID == company.CompanyID);
            else
            {
                List<int> VendorsIds = user.Vendors.Select(v => v.VendorKey).ToList();
                predicate = (a => a.CompanyID == company.CompanyID && VendorsIds.Contains(a.VendorKey));
            }

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            List<CheckRequest> list = db.CheckRequest.Where(predicate).ToList();
            List<ChequeSolicitudesDTO> solicitudes = new List<ChequeSolicitudesDTO>();
            foreach (CheckRequest creq in list.Where(a => a.CompanyID == company.CompanyID))
                try
                {   //tapLFStatus status = item.status == null ? null : db.tapLFStatus.Where(s => s.Status == item.status).FirstOrDefault();
                    Users status = creq.CreateUserkey == 0 ? null : db.Users.Where(s => s.UserKey == creq.CreateUserkey).FirstOrDefault();
                    solicitudes.Add(new ChequeSolicitudesDTO(
                        creq.ChkReqKey.ToString(),
                        creq.Serie,
                        creq.Vendors != null ? creq.Vendors.VendName : string.Empty,
                        user.UserName,
                        Math.Round(Convert.ToDecimal(creq.Total), 2).ToString(),
                        creq.ChkReqDate != null ? creq.ChkReqDate.Date.ToString("dd/MM/yyyy") : string.Empty,
                        creq.ChkReqPmtDate != null ? creq.ChkReqPmtDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                        status.UserName,
                        creq.Moneda,
                        creq.ChkReqDate == null ? DateTime.MinValue : creq.ChkReqDate.Date,
                        creq.ChkReqPmtDate == null ? DateTime.MinValue : creq.ChkReqPmtDate.Value));
                }
                catch { }

            return solicitudes;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ChequeSolicitudesDTO>();
        }
    }

    public static List<ChequeSolicitudesDTO> ObtenerSolicitudesCheque(string Serie, string Proveedor, string Solicitante, string Total, string Fecha, string FechaPago, bool directo_en_vista = false)
    {
        try
        {
            List<ChequeSolicitudesDTO> solicitudes = ObtenerSolicitudesCheque();

            if (!string.IsNullOrWhiteSpace(Serie) && Serie != "null")
                solicitudes = solicitudes.Where(s => s.Serie.ToUpper().Contains(Serie.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Proveedor) && Proveedor != "null")
                solicitudes = solicitudes.Where(s => s.Proveedor.ToUpper().Contains(Proveedor.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Solicitante) && Solicitante != "null")
                solicitudes = solicitudes.Where(s => s.NombreSolicitante.ToUpper().Contains(Solicitante.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Total) && Total != "null")
            {
                try
                {
                    double total = Convert.ToDouble(Total.Replace(",", "."));
                    //solicitudes = solicitudes.Where(f => f.Total_In_Double == total).ToList();
                    solicitudes = solicitudes.Where(f => f.Total_In_Double.ToString().Contains(total.ToString())).ToList();
                }
                catch
                {
                    throw new MulticonsultingException("El dato brindado como valor de total a filtrar no es válido");
                }
            }
            if (!string.IsNullOrWhiteSpace(Fecha) && Fecha != "null")
                solicitudes = solicitudes.Where(s => s.Fecha == Fecha).ToList();

            if (!string.IsNullOrWhiteSpace(FechaPago) && FechaPago != "null")
                solicitudes = solicitudes.Where(s => s.Fecha_Programada_Pago == FechaPago).ToList();

            return solicitudes;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ChequeSolicitudesDTO>();
        }
    }

    public static int SalvarSolicitudCheque(List<int> contrarecibos_key_list, string comentarios, string rpt_path)
    {
        try
        {
            Company company = Tools.EmpresaAutenticada();
            if (company == null)
                throw new MulticonsultingException("No está autenticado por ninguna empresa");

            List<ContrareciboDTO> contrarecibos = new List<ContrareciboDTO>();
            contrarecibos = Contrarecibos.ObtenerContrarecibos(contrarecibos_key_list, true);
            if (contrarecibos.Count == 0)
                throw new MulticonsultingException("No hay contrarecibos con las llaves pasadas");
            if (contrarecibos.Count > 1 && (contrarecibos.Where(l => l.Fecha_Programada_Pago != contrarecibos.First().Fecha_Programada_Pago).FirstOrDefault() != null || contrarecibos.Where(l => l.Proveedor != contrarecibos.First().Proveedor).FirstOrDefault() != null))
                throw new MulticonsultingException("Hay contrarecibos que son de diferentes fechas o diferentes proveedores"); // Son de diferentes fechas o diferentes proveedores

            PortalProveedoresEntities db = new PortalProveedoresEntities();
            var primero = contrarecibos.First();
            Vendors vendor = db.Vendors.Where(v => v.VendorID == primero.Proveedor).FirstOrDefault();
            if (vendor == null)
                throw new MulticonsultingException("No existe el proveedor");

            string Terminos_de_Pago;
            ProveedorDTO proveedor = Proveedores.BuscarProveedorEnSAGE(contrarecibos.First().Proveedor);
            if (proveedor != null)
            {
                if (string.IsNullOrWhiteSpace(proveedor.Condiciones))
                    throw new MulticonsultingException("El proveedor no tiene definido las condiciones de pago");

                Terminos_de_Pago = proveedor.Condiciones;
            }
            else
                throw new MulticonsultingException("No existe el proveedor"); // Vendor no encontrado en SAGE

            CheckRequest ultima_solicitud_de_cheque = null;
            string serie = company.LetraFolios;
            ultima_solicitud_de_cheque = db.CheckRequest.Where(s => s.Serie.Contains(company.LetraFolios)).OrderByDescending(s => s.CreateDate).FirstOrDefault();
            if (ultima_solicitud_de_cheque != null)
            {
                int sn = Convert.ToInt32(ultima_solicitud_de_cheque.Serie.Substring(1));
                serie += Tools.RellenoConCeros(sn + 1);
            }
            if (ultima_solicitud_de_cheque == null)
                serie += Tools.RellenoConCeros(1);

            decimal total = 0;
            foreach (ContrareciboDTO con in contrarecibos)
            {
                total += Convert.ToDecimal(con.Total);
            }

            Users usuario = Tools.UsuarioAutenticado();

            CheckRequest nueva_solicitud_de_cheque = new CheckRequest()
            {
                UserKey = usuario.UserKey,
                VendorKey = vendor.VendorKey,
                CompanyID = company.CompanyID,
                Serie = serie,
                ChkReqDate = DateTime.Now,
                ChkReqPmtDate = Convert.ToDateTime(contrarecibos.First().Fecha_Programada_Pago),
                PmtTerms = Terminos_de_Pago,
                Total = total,
                Concept = "PAGO DE FACTURA",
                Comment = comentarios,
                CreateDate = DateTime.Now,
                CreateUserkey = usuario.UserKey,
            };

            db.CheckRequest.Add(nueva_solicitud_de_cheque);
            db.SaveChanges();

            foreach (ContrareciboDTO con in contrarecibos)
            {
                ChkReqDetail chkReqDetail = new ChkReqDetail()
                {
                    ChkReqKey = nueva_solicitud_de_cheque.ChkReqKey,
                    VendorKey = vendor.VendorKey,
                    InvcRcptKey = con.Id,
                    Total = Convert.ToDecimal(con.Total),
                };
                db.ChkReqDetail.Add(chkReqDetail);
            }

            ReportDocument report_document = new ReportDocument();
            report_document.Load(rpt_path);
            report_document.SetDataSource(contrarecibos);
            report_document.SetParameterValue("razon_social_empresa", company.CompanyName.ToUpper());
            report_document.SetParameterValue("direccion_gerencia", "");
            report_document.SetParameterValue("condiciones_pago", nueva_solicitud_de_cheque.PmtTerms + "DIAS");
            report_document.SetParameterValue("proveedor", proveedor.Social);
            report_document.SetParameterValue("importe", total.ToString());
            report_document.SetParameterValue("folio", nueva_solicitud_de_cheque.Serie);
            report_document.SetParameterValue("solicitante", usuario.UserName);
            report_document.SetParameterValue("concepto", nueva_solicitud_de_cheque.Concept);
            report_document.SetParameterValue("comentarios", nueva_solicitud_de_cheque.Comment);
            NumLetra nl = new NumLetra();
            report_document.SetParameterValue("importe_letras", nl.Convertir(total.ToString(), true, nueva_solicitud_de_cheque.Moneda.ToString()));
            report_document.SetParameterValue("fecha_programada_pago", Tools.FechaEnEspañol(nueva_solicitud_de_cheque.ChkReqPmtDate.Value));
            report_document.SetParameterValue("fecha_solicitud", Tools.FechaCortaEsp(nueva_solicitud_de_cheque.ChkReqDate));
            //report_document.SetParameterValue("fecha_programada_pago", nueva_solicitud_de_cheque.ChkReqPmtDate.ToString());
            report_document.SetParameterValue("logo", "~/Img/TSYS.png");
            report_document.SetParameterValue("title", "Solicitud de Cheque");
            Stream stream = report_document.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            BinaryReader br = new BinaryReader(stream);

            ChkReqFile chkReqFile = new ChkReqFile()
            {
                CheckRequestKey = nueva_solicitud_de_cheque.ChkReqKey,
                FileType = "pdf",
                FileBinary = br.ReadBytes(Convert.ToInt32(stream.Length)),
            };

            db.ChkReqFile.Add(chkReqFile);
            db.SaveChanges();
            Facturas.ActualizarEstadoFacturasSql();
            return nueva_solicitud_de_cheque.ChkReqKey;
        }
        catch (Exception ex)
        {
            throw new MulticonsultingException(ex.Message, ex.InnerException);
        }
        return -1;
    }
}