using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Domain.Entities;
using SmartAuditDemo.Infrastructure.Data;

namespace SmartAuditDemo.Application.Services;

public class AccountingDocumentService : IAccountingDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AccountingDocumentService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AccountingDocumentDto?> GetByIdAsync(Guid id)
    {
        var document = await _context.AccountingDocuments
            .FirstOrDefaultAsync(d => d.Id == id);

        if (document == null)
            return null;

        var dto = _mapper.Map<AccountingDocumentDto>(document);
        dto.FileUrl = $"/uploads/documents/{Path.GetFileName(document.FilePath)}";
        return dto;
    }

    public async Task<PagedResultDto<AccountingDocumentDto>> GetPagedAsync(AccountingDocumentFilterDto filter)
    {
        var query = _context.AccountingDocuments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            query = query.Where(d => d.Title.Contains(filter.Title));
        }

        if (filter.DocumentType.HasValue)
        {
            query = query.Where(d => d.DocumentType == filter.DocumentType.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Extension))
        {
            query = query.Where(d => d.Extension == filter.Extension);
        }

        var totalCount = await query.CountAsync();

        var documents = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<AccountingDocumentDto>>(documents);
        foreach (var dto in dtos)
        {
            dto.FileUrl = $"/uploads/documents/{Path.GetFileName(dto.FilePath)}";
        }

        return new PagedResultDto<AccountingDocumentDto>
        {
            Data = dtos,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<AccountingDocumentDto> CreateAsync(CreateAccountingDocumentDto dto, string webRootPath)
    {
        var uploadsPath = Path.Combine(webRootPath, "uploads", "documents");
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var extension = Path.GetExtension(dto.File.FileName).TrimStart('.');
        var uniqueFileName = $"{Guid.NewGuid()}.{extension}";
        var filePath = Path.Combine(uploadsPath, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        var sizeInKb = dto.File.Length / 1024;

        var document = new AccountingDocument
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            DocumentType = dto.DocumentType,
            Extension = extension,
            FilePath = filePath,
            UploaderUserId = dto.UploaderUserId,
            CreatedAt = DateTime.UtcNow,
            SizeInKb = sizeInKb
        };

        document.Validate();

        _context.AccountingDocuments.Add(document);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<AccountingDocumentDto>(document);
        result.FileUrl = $"/uploads/documents/{uniqueFileName}";
        return result;
    }

    public async Task<AccountingDocumentDto> UpdateAsync(UpdateAccountingDocumentDto dto, string webRootPath)
    {
        var document = await _context.AccountingDocuments
            .FirstOrDefaultAsync(d => d.Id == dto.Id);

        if (document == null)
            throw new KeyNotFoundException("سند یافت نشد");

        document.Title = dto.Title;
        document.DocumentType = dto.DocumentType;

        if (dto.File != null)
        {
            // حذف فایل قدیمی
            if (File.Exists(document.FilePath))
            {
                File.Delete(document.FilePath);
            }

            // آپلود فایل جدید
            var uploadsPath = Path.Combine(webRootPath, "uploads", "documents");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var extension = Path.GetExtension(dto.File.FileName).TrimStart('.');
            var uniqueFileName = $"{Guid.NewGuid()}.{extension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            document.Extension = extension;
            document.FilePath = filePath;
            document.SizeInKb = dto.File.Length / 1024;
        }

        document.UpdatedAt = DateTime.UtcNow;
        document.Validate();

        await _context.SaveChangesAsync();

        var result = _mapper.Map<AccountingDocumentDto>(document);
        result.FileUrl = $"/uploads/documents/{Path.GetFileName(document.FilePath)}";
        return result;
    }

    public async Task<bool> DeleteAsync(Guid id, string webRootPath)
    {
        var document = await _context.AccountingDocuments
            .FirstOrDefaultAsync(d => d.Id == id);

        if (document == null)
            return false;

        // حذف فایل فیزیکی
        if (File.Exists(document.FilePath))
        {
            File.Delete(document.FilePath);
        }

        _context.AccountingDocuments.Remove(document);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<AccountingDocumentDto>> GetGroupedByTypeAsync()
    {
        var documents = await _context.AccountingDocuments
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

        var dtos = _mapper.Map<List<AccountingDocumentDto>>(documents);
        foreach (var dto in dtos)
        {
            dto.FileUrl = $"/uploads/documents/{Path.GetFileName(dto.FilePath)}";
        }

        return dtos;
    }
}
