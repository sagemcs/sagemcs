using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ExpenseDTO
/// </summary>
/// 

    public class ExpenseDTO : Document
    {

        public int ExpenseId { get; set; }       
        public DateTime Date { get; set; }
        public string Currency { get; set; }       
        public string DeniedReason { get; set; }
        public int? PackageId { get; set; }
        public int AdvanceId { get; set; }
        public string FileNameXml { get; set; }
        public string FileNamePdf { get; set; }
        public string FileNamePdfVoucher { get; set; }          
        public string ExpenseReason { get; set; }
        public bool SageIntegration { get; set; }

    public ExpenseDTO()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }

    public class ExpenseReportDTO
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


        public ExpenseReportDTO()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    
}