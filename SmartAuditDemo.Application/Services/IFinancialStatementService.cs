namespace SmartAuditDemo.Application.Services;

public interface IFinancialStatementService
{
    Task<MemoryStream> GenerateFinancialStatementAsync(Guid documentId);
}

