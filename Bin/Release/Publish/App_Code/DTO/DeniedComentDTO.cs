using System;

/// <summary>
/// Summary description for DeniedComentDTO
/// </summary>
public class DeniedComentDTO
{
    public int Id { get; set; }
    public int ExpenseId { get; set; }
    public string DeniedReason { get; set; }
    public int UserKey { get; set; }
    public DateTime CreateDate { get; set; }
    public int ApprovalLevel { get; set; }
    public string CompanyId { get; set; }


}