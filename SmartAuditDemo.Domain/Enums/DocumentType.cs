namespace SmartAuditDemo.Domain.Enums;

public enum DocumentType
{
    Voucher = 1,      // سند حسابداری
    Invoice = 2,      // فاکتور یا صورتحساب
    Contract = 3,     // قرارداد
    Letter = 4,        // مکاتبات
    Report = 5,        // گزارشات
    Ledger = 6,        // دفاتر (ضروری)
    Other = 7          // سایر
}
