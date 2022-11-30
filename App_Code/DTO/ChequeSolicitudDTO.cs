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
public class ChequeSolicitudDTO
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

    public ChequeSolicitudDTO()
    { }
    public ChequeSolicitudDTO(string Id, string Serie, string Proveedor, string Solicitante, string Total, string Fecha, string Fecha_Programada_Pago)
    {
        this.Id = Id;
        this.Serie = Serie;
        this.Proveedor = Proveedor;
        this.Solicitante = Solicitante;
        this.Total = Total;
        this.Total_In_Double = Convert.ToDouble(Total);
        this.Fecha = Fecha;
        this.Fecha_Programada_Pago = Fecha_Programada_Pago;

        //this.Date = string.IsNullOrWhiteSpace(this.Fecha) ? DateTime.MinValue : Convert.ToDateTime(this.Fecha);
        //this.DatePr = string.IsNullOrWhiteSpace(this.Fecha_Programada_Pago) ? DateTime.MinValue : Convert.ToDateTime(this.Fecha_Programada_Pago);
    }
}

public class ChequeSolicitudes
{
    public ChequeSolicitudes()
    { }

    public static List<ChequeSolicitudDTO> ObtenerSolicitudesCheque(bool directo_en_vista = false)
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
            List<ChequeSolicitudDTO> solicitudes = new List<ChequeSolicitudDTO>();
            foreach (CheckRequest creq in list.Where(a => a.CompanyID == company.CompanyID))
                try
                {
                    solicitudes.Add(new ChequeSolicitudDTO(
                        creq.ChkReqKey.ToString(),
                        creq.Serie,
                        creq.Vendors != null ? creq.Vendors.VendName : "",
                        user.UserName,
                        Math.Round(Convert.ToDecimal(creq.Total), 2).ToString(),
                        creq.ChkReqDate != null ? creq.ChkReqDate.Date.ToString("dd/MM/yyyy") : "",
                        creq.ChkReqPmtDate != null ? creq.ChkReqPmtDate.Value.ToString("dd/MM/yyyy") : ""));
                }
                catch { }

            return solicitudes;
        }
        catch (Exception exp)
        {
            if (directo_en_vista)
                throw new MulticonsultingException(exp.ToString());
            return new List<ChequeSolicitudDTO>();
        }
    }

    public static List<ChequeSolicitudDTO> ObtenerSolicitudesCheque(string Serie, string Proveedor, string Solicitante, string Total, string Fecha, string FechaPago, bool directo_en_vista = false)
    {
        try
        {
            List<ChequeSolicitudDTO> solicitudes = ObtenerSolicitudesCheque();

            if (!string.IsNullOrWhiteSpace(Serie) && Serie != "null")
                solicitudes = solicitudes.Where(s => s.Serie.ToUpper().Contains(Serie.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Proveedor) && Proveedor != "null")
                solicitudes = solicitudes.Where(s => s.Proveedor.ToUpper().Contains(Proveedor.ToUpper())).ToList();
            if (!string.IsNullOrWhiteSpace(Solicitante) && Solicitante != "null")
                solicitudes = solicitudes.Where(s => s.Solicitante.ToUpper().Contains(Solicitante.ToUpper())).ToList();
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
            return new List<ChequeSolicitudDTO>();
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
            ContrareciboDTO first = contrarecibos.FirstOrDefault();

            CheckRequest nueva_solicitud_de_cheque = new CheckRequest()
            {
                UserKey = usuario.UserKey,
                VendorKey = vendor.VendorKey,
                CompanyID = company.CompanyID,
                Serie = serie,
                ChkReqDate = DateTime.Now,
                //ChkReqPmtDate = Convert.ToDateTime(first.Fecha_Programada_Pago),
                PmtTerms = Terminos_de_Pago,
                Total = total,
                Concept = "Pago de factura",
                Comment = comentarios,
                CreateDate = DateTime.Now,
                CreateUserkey = usuario.UserKey,
                Moneda = first != null ? first.Moneda.ToString() : string.Empty
            };
            if (first != null)
            {
                if (first.Fecha_Programada_Pago != null)
                {
                    nueva_solicitud_de_cheque.ChkReqPmtDate = Convert.ToDateTime(first.Fecha_Programada_Pago);
                }
            }

            db.CheckRequest.Add(nueva_solicitud_de_cheque);
            db.SaveChanges();

            foreach (ContrareciboDTO con in contrarecibos)
            {
                ChkReqDetail chkReqDetail = new ChkReqDetail()
                {
                    ChkReqKey = nueva_solicitud_de_cheque.ChkReqKey,
                    VendorKey = vendor.VendorKey,
                    InvcRcptKey = con.Id,
                    Total = con.Total != null ? Convert.ToDecimal(con.Total) : 0,
                };
                db.ChkReqDetail.Add(chkReqDetail);
            }
            db.SaveChanges();
            //--------------agregado---------------------//
            //CheckRequest solicitud_de_cheque = db.CheckRequest.Where(c => c.ChkReqKey == nueva_solicitud_de_cheque.ChkReqKey).FirstOrDefault();
            List<NotaDTO> Notas = new List<NotaDTO>();
            List<FacturaDTO> facturas = new List<FacturaDTO>();

            //for(int i = 0; i < 20; i++)// Solo activar esta línea cuando se quiera ver comportamiento del documento con muchos datos cargados
            foreach (ChkReqDetail detail in nueva_solicitud_de_cheque.ChkReqDetail)
            {
                decimal Total = 0;
                //foreach (InvcRcptDetails fac in detail.InvoiceReceipt.InvcRcptDetails)
                foreach (InvcRcptDetails fac in db.InvcRcptDetails.Where(i => i.InvcRcptKey == detail.InvcRcptKey))
                {
                    if (fac == null)
                    {
                        continue;
                    }
                    //------------------agregando las notas--------//
                    Invoice invoice = fac.Invoice;

                    Int32 AprovUserKey = Convert.ToInt32(invoice.AprovUserKey);
                    Users user = db.Users.Where(u => u.UserKey == AprovUserKey).FirstOrDefault();
                    Total += invoice.Total;

                    //List<NotaDTO> Notas = new List<NotaDTO>();

                    bool sin_notas = true;

                    foreach (Invoice nota in invoice.Invoice1.Where(i => i.TranType == "CM" || i.TranType == "DM"))
                    //foreach (Invoice nota in invoice.Invoice1.Where(i => i.TipoComprobante == "E"))
                    {
                        sin_notas = false;
                        Total += nota.TranType == "CM" ? -(nota.Total) : nota.Total; // La nota afectando el valor del contrarecibo

                        NotaDTO Nota = new NotaDTO(nota.InvoiceKey, invoice.InvoiceKey);
                        Nota.Total = "$ " + Nota.Total;
                        Nota.Traslados = "$ " + Nota.Traslados;
                        Nota.Subtotal = "$ " + Nota.Subtotal;
                        Nota.Orden_de_Compra = nota.NodeOc;
                        //Nota.Tipo = "Nota de " + (nota.TranType == "CM" ? "Crédito" : "Débito");
                        Nota.Tipo = nota.TipoComprobante;
                        Notas.Add(Nota);


                    }
                    if (sin_notas)
                    {
                        NotaDTO Nota_Vacia = new NotaDTO();
                        Nota_Vacia.ApplyToInvcKey = invoice.InvoiceKey;
                        Notas.Add(Nota_Vacia);
                    }

                    //-----------------fin agregando notas--------//
                    if (fac.Invoice != null)
                    {
                        FacturaDTO factura = new FacturaDTO(invoice.InvoiceKey)
                        {
                            Folio = fac.Invoice.Folio,
                            Compania = fac.Invoice.CompanyID,
                            Orden_de_Compra = fac.Invoice.NodeOc,
                            Tipo = fac.Invoice.TipoComprobante,
                            Total = Math.Round(fac.Invoice.Total, 2).ToString()
                        };
                        facturas.Add(factura);
                    }



                }
            }
            ReportDocument report_document = new ReportDocument();
            report_document.Load(rpt_path);
            report_document.Database.Tables[0].SetDataSource(facturas);
            //report_document.Database.Tables[1].SetDataSource(Notas);
            //------fin agregado-----------------//


            //report_document.SetDataSource(contrarecibos);
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
            Facturas.ActualizarEstadoFacturas();
            return nueva_solicitud_de_cheque.ChkReqKey;
        }
        catch (Exception ex)
        {
            throw new MulticonsultingException(ex.Message, ex.InnerException);
        }
        return -1;
    }
}