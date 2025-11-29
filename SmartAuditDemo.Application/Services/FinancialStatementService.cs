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
        // Output data variables:
        //درآمد های عملیاتی
        string operatingIncome = string.Empty;
        //هزینه های اداری و عمومی
        string administrativeAndGeneralExpenses = string.Empty;
        //هزینه های پرسنلی
        string personnelCosts = string.Empty;
        //مازاد در آمد بر هزینه
        string excessOfIncomeOverExpenditure = string.Empty;
        //سایر درآمد ها و هزینه های غیر عملیاتی
        string otherNonOperatingIncomeAndExpenses = string.Empty;
        //مازاد درآمد و هزینه قبل از مالیات
        string excessOfIncomeOverExpensesBeforeTax = string.Empty;
        //مالیات
        string tax = string.Empty;
        //خالص مازاد درآمد بر هزینه
        string netExcessOfIncomeOverExpenditure = string.Empty;


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
        var inputWorksheet = inputWorkbook.Worksheets.First();
        
        return await ProcessExcelAndGenerateOutput(inputWorksheet, operatingIncome, administrativeAndGeneralExpenses, 
            personnelCosts, excessOfIncomeOverExpenditure, otherNonOperatingIncomeAndExpenses, 
            excessOfIncomeOverExpensesBeforeTax, tax, netExcessOfIncomeOverExpenditure);
    }

    public async Task<MemoryStream> GenerateFinancialStatementFromFileAsync(Stream fileStream)
    {
        // Output data variables:
        string operatingIncome = string.Empty;
        string administrativeAndGeneralExpenses = string.Empty;
        string personnelCosts = string.Empty;
        string excessOfIncomeOverExpenditure = string.Empty;
        string otherNonOperatingIncomeAndExpenses = string.Empty;
        string excessOfIncomeOverExpensesBeforeTax = string.Empty;
        string tax = string.Empty;
        string netExcessOfIncomeOverExpenditure = string.Empty;

        // باز کردن فایل Excel از Stream
        fileStream.Position = 0; // Reset stream position
        using var inputWorkbook = new XLWorkbook(fileStream);
        var inputWorksheet = inputWorkbook.Worksheets.First();
        
        return await ProcessExcelAndGenerateOutput(inputWorksheet, operatingIncome, administrativeAndGeneralExpenses, 
            personnelCosts, excessOfIncomeOverExpenditure, otherNonOperatingIncomeAndExpenses, 
            excessOfIncomeOverExpensesBeforeTax, tax, netExcessOfIncomeOverExpenditure);
    }

    private async Task<MemoryStream> ProcessExcelAndGenerateOutput(
        IXLWorksheet inputWorksheet,
        string operatingIncome,
        string administrativeAndGeneralExpenses,
        string personnelCosts,
        string excessOfIncomeOverExpenditure,
        string otherNonOperatingIncomeAndExpenses,
        string excessOfIncomeOverExpensesBeforeTax,
        string tax,
        string netExcessOfIncomeOverExpenditure)
    {
        // خواندن Header row (ردیف اول)
        var headerRow = inputWorksheet.Row(1);
        var headerColumns = new Dictionary<int, string>();
        
        foreach (var cell in headerRow.CellsUsed())
        {
            var columnIndex = cell.Address.ColumnNumber;
            var columnTitle = cell.Value.IsBlank ? string.Empty : cell.Value.ToString();
            headerColumns[columnIndex] = columnTitle;
        }

        foreach (var row in inputWorksheet.RowsUsed().Skip(1))
        {
            string kolKod = row.Cell(headerColumns.First(x => x.Value.Contains("کد کل")).Key).GetString();
            string bedehkarStr = row.Cell(headerColumns.First(x => x.Value.Contains("بدهکار")).Key).GetString();
            string bestankarStr = row.Cell(headerColumns.First(x => x.Value.Contains("بستانکار")).Key).GetString();

            decimal bedehkar = string.IsNullOrWhiteSpace(bedehkarStr) ? 0 : decimal.Parse(bedehkarStr);
            decimal bestankar = string.IsNullOrWhiteSpace(bestankarStr) ? 0 : decimal.Parse(bestankarStr);

            // ----------------------------
            // درآمدهای عملیاتی (کد کل 41)
            // ----------------------------
            if (kolKod.StartsWith("41"))
            {
                operatingIncome += bestankar;
            }

            // ----------------------------
            // هزینه‌های اداری و عمومی (کد کل 61 → غیرپرسنلی)
            // ----------------------------
            if (kolKod.StartsWith("61"))
            {
                administrativeAndGeneralExpenses += bedehkar;
            }

            // ----------------------------
            // هزینه‌های پرسنلی (اگر کد 51 یا 52 باشد)
            // ----------------------------
            if (kolKod.StartsWith("51") || kolKod.StartsWith("52"))
            {
                personnelCosts += bedehkar;
            }

            // ----------------------------
            // سایر درآمدها و هزینه‌های غیرعملیاتی (کد کل 7)
            // ----------------------------
            if (kolKod.StartsWith("7"))
            {
                // اگر درآمد است، بستانکار؛ اگر هزینه است، بدهکار
                otherNonOperatingIncomeAndExpenses += (bestankar - bedehkar);
            }

            // ----------------------------
            // مالیات (مثلاً کد کل 34)
            // ----------------------------
            if (kolKod.StartsWith("34"))
            {
                tax += bedehkar;
            }
        }

        // مازاد درآمد بر هزینه
        excessOfIncomeOverExpenditure =
            ((decimal.Parse(operatingIncome) - (decimal.Parse(administrativeAndGeneralExpenses) + decimal.Parse(personnelCosts)))).ToString();

        // مازاد قبل از مالیات
        excessOfIncomeOverExpensesBeforeTax =
            excessOfIncomeOverExpenditure + otherNonOperatingIncomeAndExpenses;

        // خالص مازاد درآمد بر هزینه
        netExcessOfIncomeOverExpenditure =
            (decimal.Parse(excessOfIncomeOverExpensesBeforeTax) - decimal.Parse(tax)).ToString();

        // ایجاد فایل اکسل خروجی
        using var outputWorkbook = new XLWorkbook();
        var outputWorksheet = outputWorkbook.Worksheets.Add("سود و زیان");
        
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

        // ستون‌های صورت مالی و مقادیر مربوطه
        var financialItems = new[]
        {
        new { Description = "درآمدهای عملیاتی", Value = operatingIncome },
        new { Description = "هزینه‌های پرسنلی", Value = personnelCosts },
        new { Description = "هزینه‌های اداری و عمومی", Value = administrativeAndGeneralExpenses },
        new { Description = "مازاد (کسری) درآمد بر هزینه", Value = excessOfIncomeOverExpenditure },
        new { Description = "سایر درآمدها و هزینه‌های غیر عملیاتی", Value = otherNonOperatingIncomeAndExpenses },
        new { Description = "مازاد درآمد و هزینه قبل از مالیات", Value = excessOfIncomeOverExpensesBeforeTax },
        new { Description = "مالیات", Value = tax },
        new { Description = "خالص مازاد درآمد بر هزینه", Value = netExcessOfIncomeOverExpenditure }
        };

        int rowIndex = 4;
        for (int i = 0; i < financialItems.Length; i++)
        {
            outputWorksheet.Cell(rowIndex, 1).Value = i + 1;
            outputWorksheet.Cell(rowIndex, 2).Value = financialItems[i].Description;
            outputWorksheet.Cell(rowIndex, 3).Value = financialItems[i].Value;
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

