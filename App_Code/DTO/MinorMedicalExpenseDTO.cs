using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MinorMedicalExpenseDTO
/// </summary>

    public class MinorMedicalExpenseDTO : Document
    {
        public int MinorMedicalExpenseId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string DeniedReason { get; set; }
        public int? PackageId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string FileNameXml { get; set; }
        public string FileNamePdf { get; set; }
        public int ApprovalLevel { get; set; }
        public bool SageIntegration { get; set; }
        public MinorMedicalExpenseDTO()
            {
                //
                // TODO: Add constructor logic here
                //
            }
        }

    public class MinorMedicalExpenseReportDTO
    {
        public DateTime Fecha { get; set; }
        public decimal Importe { get; set; }
        public string Estado { get; set; }
        public MinorMedicalExpenseReportDTO()
        {


        }
    }
