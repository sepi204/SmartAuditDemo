using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Domain.Entities;
using SmartAuditDemo.Infrastructure.Data;

namespace SmartAuditDemo.Application.Services;

public class OrganizationService : IOrganizationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public OrganizationService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrganizationDto?> GetByIdAsync(Guid id)
    {
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == id);

        if (organization == null)
            return null;

        return _mapper.Map<OrganizationDto>(organization);
    }

    public async Task<PagedResultDto<OrganizationDto>> GetPagedAsync(OrganizationFilterDto filter)
    {
        var query = _context.Organizations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(o => o.Name.Contains(filter.Name));
        }

        if (!string.IsNullOrWhiteSpace(filter.EconomicCode))
        {
            query = query.Where(o => o.EconomicCode.Contains(filter.EconomicCode));
        }

        if (filter.CompanyType.HasValue)
        {
            query = query.Where(o => o.CompanyType == filter.CompanyType.Value);
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(o => o.IsActive == filter.IsActive.Value);
        }

        var totalCount = await query.CountAsync();

        var organizations = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<OrganizationDto>>(organizations);

        return new PagedResultDto<OrganizationDto>
        {
            Data = dtos,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<OrganizationDto> CreateAsync(CreateOrganizationDto dto)
    {
        var organization = _mapper.Map<Organization>(dto);
        organization.Id = Guid.NewGuid();
        organization.CreatedAt = DateTime.UtcNow;
        organization.Validate();

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();

        return _mapper.Map<OrganizationDto>(organization);
    }

    public async Task<OrganizationDto> UpdateAsync(UpdateOrganizationDto dto)
    {
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == dto.Id);

        if (organization == null)
            throw new KeyNotFoundException("سازمان یافت نشد");

        _mapper.Map(dto, organization);
        organization.UpdatedAt = DateTime.UtcNow;
        organization.Validate();

        await _context.SaveChangesAsync();

        return _mapper.Map<OrganizationDto>(organization);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == id);

        if (organization == null)
            return false;

        _context.Organizations.Remove(organization);
        await _context.SaveChangesAsync();

        return true;
    }
}

