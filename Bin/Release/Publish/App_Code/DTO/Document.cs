using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Doc_Tools;

/// <summary>
/// Summary description for Document
/// </summary>
public class Document
{
    public decimal Amount { get; set; }
    public int UpdateUserKey { get; set; }
    public string CompanyId { get; set; }
    public string Status { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public int? ApprovalLevel { get; set; }
    

    public DateTime GetDate(DocumentType type)
    {
        var date = new DateTime();
        switch (type)
        {
            case DocumentType.Advance:
               var anticipo =  (AdvanceDTO)this;
                date = anticipo.CreateDate;
                break;
            case DocumentType.Expense:
                var reembolso = (ExpenseDTO)this;
                date = reembolso.Date;
                break;
            case DocumentType.CorporateCard:
                var tarjeta = (CorporateCardDTO)this;
                date = tarjeta.CreateDate;
                break;
            case DocumentType.MinorMedicalExpense:
                var medico = (MinorMedicalExpenseDTO)this;
                date = medico.CreateDate;
                break;           
        }
        return date;
    }
}