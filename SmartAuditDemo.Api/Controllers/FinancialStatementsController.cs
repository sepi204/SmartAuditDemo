using Microsoft.AspNetCore.Mvc;
using SmartAuditDemo.Application.Services;

namespace SmartAuditDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FinancialStatementsController : ControllerBase
{
    private readonly IFinancialStatementService _financialStatementService;

    public FinancialStatementsController(IFinancialStatementService financialStatementService)
    {
        _financialStatementService = financialStatementService;
    }

    [HttpPost]
    public async Task<IActionResult> GenerateFinancialStatement([FromBody] GenerateFinancialStatementRequest request)
    {
        try
        {
            if (request.DocumentId == Guid.Empty)
            {
                return BadRequest(new { message = "شناسه سند الزامی است" });
            }

            var excelStream = await _financialStatementService.GenerateFinancialStatementAsync(request.DocumentId);
            
            var fileName = $"صورت_مالی_{request.DocumentId:N}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            
            return File(excelStream, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "خطا در تولید صورت مالی", error = ex.Message });
        }
    }
}

public class GenerateFinancialStatementRequest
{
    public Guid DocumentId { get; set; }
}

