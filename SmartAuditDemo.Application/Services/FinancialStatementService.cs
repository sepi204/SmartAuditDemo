using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SmartAuditDemo.Domain.Entities;
using SmartAuditDemo.Infrastructure.Data;

namespace SmartAuditDemo.Application.Services;

public class FinancialStatementService : IFinancialStatementService
{
    private readonly ApplicationDbContext _context;

    public FinancialStatementService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MemoryStream> GenerateFinancialStatementAsync(Guid documentId)
    {
        // دریافت سند از دیتابیس
        var document = await _context.AccountingDocuments
            .FirstOrDefaultAsync(d => d.Id == documentId);

        if (document == null)
        {
            throw new KeyNotFoundException($"سند با شناسه {documentId} یافت نشد");
        }

        // بررسی وجود فایل
        if (string.IsNullOrWhiteSpace(document.FilePath) || !File.Exists(document.FilePath))
        {
            throw new FileNotFoundException($"فایل سند در مسیر {document.FilePath} یافت نشد");
        }

        // باز کردن فایل Excel انتخاب شده
        using var inputWorkbook = new XLWorkbook(document.FilePath);
        var inputWorksheet = inputWorkbook.Worksheets.First(); // استفاده از اولین worksheet
        
        // خواندن Header row (ردیف اول)
        var headerRow = inputWorksheet.Row(1);
        var headerColumns = new Dictionary<int, string>();
        
        foreach (var cell in headerRow.CellsUsed())
        {
            var columnIndex = cell.Address.ColumnNumber;
            var columnTitle = cell.Value.IsBlank ? string.Empty : cell.Value.ToString();
            headerColumns[columnIndex] = columnTitle;
        }

        // Iterate روی تمام ردیف‌ها (به جز Header که ردیف اول است)
        foreach (var row in inputWorksheet.RowsUsed().Skip(1))
        {
            foreach (var cell in row.CellsUsed())
            {
                // مقدار سلول
                var cellValue = cell.Value;
                
                // عنوان ستون مربوطه از Header
                var columnIndex = cell.Address.ColumnNumber;

                //HERE

                var relatedColumnTitle = headerColumns.ContainsKey(columnIndex) 
                    ? headerColumns[columnIndex] 
                    : string.Empty;

                // در اینجا می‌توانید از cellValue و relatedColumnTitle استفاده کنید
                // فعلاً هیچ تغییری اعمال نمی‌شود
                _ = cellValue;
                _ = relatedColumnTitle;
            }
        }

        // ایجاد فایل اکسل خروجی (همان قبلی)
        using var outputWorkbook = new XLWorkbook();
        var outputWorksheet = outputWorkbook.Worksheets.Add("صورت مالی");
        
        // تنظیم راست‌چین برای ستون‌ها
        outputWorksheet.Style.Font.FontName = "Tahoma";
        outputWorksheet.Style.Font.FontSize = 11;
        
        // اضافه کردن هدر
        outputWorksheet.Cell(1, 1).Value = "صورت مالی هوشمند";
        outputWorksheet.Cell(1, 1).Style.Font.Bold = true;
        outputWorksheet.Cell(1, 1).Style.Font.FontSize = 14;
        outputWorksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        outputWorksheet.Range(1, 1, 1, 2).Merge();
        
        outputWorksheet.Cell(3, 1).Value = "ردیف";
        outputWorksheet.Cell(3, 2).Value = "شرح";
        outputWorksheet.Cell(3, 3).Value = "مبلغ";
        
        // ستون‌های صورت مالی
        var rows = new[]
        {
            "درآمدهای عملیاتی",
            "هزینه‌های پرسنلی",
            "هزینه‌های اداری و عمومی",
            "مازاد (کسری) درآمد بر هزینه",
            "سایر درآمدها و هزینه‌های غیر عملیاتی",
            "مازاد درآمد و هزینه قبل از مالیات",
            "مالیات",
            "خالص مازاد درآمد بر هزینه"
        };
        
        int rowIndex = 4;
        for (int i = 0; i < rows.Length; i++)
        {
            outputWorksheet.Cell(rowIndex, 1).Value = i + 1;
            outputWorksheet.Cell(rowIndex, 2).Value = rows[i];
            outputWorksheet.Cell(rowIndex, 3).Value = ""; // خالی برای محاسبه بعدی
            rowIndex++;
        }
        
        // تنظیم عرض ستون‌ها
        outputWorksheet.Column(1).Width = 10;
        outputWorksheet.Column(2).Width = 50;
        outputWorksheet.Column(3).Width = 20;
        
        // راست‌چین کردن ستون مبلغ
        outputWorksheet.Column(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        
        // استایل هدر
        var headerRange = outputWorksheet.Range(3, 1, 3, 3);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#6f42c1"); // بنفش
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        
        // ایجاد MemoryStream و بازگشت
        var memoryStream = new MemoryStream();
        outputWorkbook.SaveAs(memoryStream);
        memoryStream.Position = 0;
        
        return await Task.FromResult(memoryStream);
    }
}

