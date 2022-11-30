using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CorporateCardDTO
/// </summary>
public class CorporateCardDTO : Document
{
    public int CorporateCardId { get; set; }
    public DateTime Date { get; set; }
    public string Currency { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public string DeniedReason { get; set; }
    public int? PackageId { get; set; }
    public DateTime CreateDate { get; set; }
    public string FileNameXml { get; set; }
    public string FileNamePdf { get; set; }
    public string FileNamePdfVoucher { get; set; }
    public int ApprovalLevel { get; set; }
    public string ExpenseReason { get; set; }
    public bool SageIntegration { get; set; }
    public CorporateCardDTO()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public class CorporateCardReportDTO
    {
        private decimal _total;
        public DateTime Fecha { get; set; }
        public decimal Aereo { get; set; }
        public decimal Terrestre { get; set; }
        public decimal Casetas { get; set; }
        public decimal Gasolina { get; set; }
        public decimal Estacionamiento { get; set; }
        public decimal Alimentos { get; set; }
        public decimal Hospedaje { get; set; }
        public decimal GExtra { get; set; }
        public decimal Total { get { return get_total(); } set { _total = value; } }
        public int PackageId { get; set; }

        public decimal get_total()
        {
            return this.Aereo + this.Terrestre + this.Casetas + this.Gasolina + this.Estacionamiento + this.Alimentos + this.Hospedaje + this.GExtra;
        }
    }
}

public class CorporateCardReportDTO
{
    public string Tipo { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoMoneda { get; set; }
    public decimal Importe { get; set; }
    public string Estado { get; set; }
    public CorporateCardReportDTO()
    {


    }
}

public class CorporateCardGroupedDTO
{
    private decimal _total;
    public DateTime Fecha { get; set; }
    public decimal Aereo { get; set; }
    public decimal Terrestre { get; set; }
    public decimal Casetas { get; set; }
    public decimal Gasolina { get; set; }
    public decimal Estacionamiento { get; set; }
    public decimal Alimentos { get; set; }
    public decimal Hospedaje { get; set; }
    public decimal GExtra { get; set; }
    public decimal Total { get { return get_total(); } set { _total = value; } }

    public decimal get_total()
    {
        return this.Aereo + this.Terrestre + this.Casetas + this.Gasolina + this.Estacionamiento + this.Alimentos + this.Hospedaje + this.GExtra;
    }
    public CorporateCardGroupedDTO()
    {


    }
}