using SmartAuditDemo.Application.DTOs;

namespace SmartAuditDemo.Application.Services;

public interface IAccountingDocumentService
{
    Task<AccountingDocumentDto?> GetByIdAsync(Guid id);
    Task<PagedResultDto<AccountingDocumentDto>> GetPagedAsync(AccountingDocumentFilterDto filter);
    Task<List<AccountingDocumentDto>> GetGroupedByTypeAsync();
    Task<AccountingDocumentDto> CreateAsync(CreateAccountingDocumentDto dto, string webRootPath);
    Task<AccountingDocumentDto> UpdateAsync(UpdateAccountingDocumentDto dto, string webRootPath);
    Task<bool> DeleteAsync(Guid id, string webRootPath);
}
