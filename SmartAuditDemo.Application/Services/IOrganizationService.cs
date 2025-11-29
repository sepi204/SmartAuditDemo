using SmartAuditDemo.Application.DTOs;

namespace SmartAuditDemo.Application.Services;

public interface IOrganizationService
{
    Task<OrganizationDto?> GetByIdAsync(Guid id);
    Task<PagedResultDto<OrganizationDto>> GetPagedAsync(OrganizationFilterDto filter);
    Task<OrganizationDto> CreateAsync(CreateOrganizationDto dto);
    Task<OrganizationDto> UpdateAsync(UpdateOrganizationDto dto);
    Task<bool> DeleteAsync(Guid id);
}



