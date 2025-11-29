using Microsoft.AspNetCore.Mvc;
using SmartAuditDemo.Api.Common;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Application.Services;

namespace SmartAuditDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountingDocumentsController : ControllerBase
{
    private readonly IAccountingDocumentService _documentService;
    private readonly IWebHostEnvironment _environment;

    public AccountingDocumentsController(
        IAccountingDocumentService documentService,
        IWebHostEnvironment environment)
    {
        _documentService = documentService;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<AccountingDocumentDto>>>> GetDocuments(
        [FromQuery] AccountingDocumentFilterDto filter)
    {
        try
        {
            var result = await _documentService.GetPagedAsync(filter);
            
            // Add full URL to file paths
            foreach (var doc in result.Data)
            {
                doc.FileUrl = $"{Request.Scheme}://{Request.Host}{doc.FilePath}";
            }

            return Ok(ApiResponse<PagedResultDto<AccountingDocumentDto>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PagedResultDto<AccountingDocumentDto>>.ErrorResult(
                "خطا در دریافت اطلاعات", new List<string> { ex.Message }));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<AccountingDocumentDto>>> GetDocument(Guid id)
    {
        try
        {
            var document = await _documentService.GetByIdAsync(id);
            
            if (document == null)
            {
                return NotFound(ApiResponse<AccountingDocumentDto>.ErrorResult("سند یافت نشد"));
            }

            document.FileUrl = $"{Request.Scheme}://{Request.Host}{document.FilePath}";
            return Ok(ApiResponse<AccountingDocumentDto>.SuccessResult(document));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<AccountingDocumentDto>.ErrorResult(
                "خطا در دریافت اطلاعات", new List<string> { ex.Message }));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AccountingDocumentDto>>> CreateDocument(
        [FromForm] CreateAccountingDocumentDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(ApiResponse<AccountingDocumentDto>.ErrorResult("اطلاعات وارد شده معتبر نیست", errors));
        }

        if (dto.File == null || dto.File.Length == 0)
        {
            return BadRequest(ApiResponse<AccountingDocumentDto>.ErrorResult("فایل الزامی است"));
        }

        try
        {
            // TODO: Get from authentication context
            dto.UploaderUserId = Guid.NewGuid();

            var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var document = await _documentService.CreateAsync(dto, webRootPath);
            
            document.FileUrl = $"{Request.Scheme}://{Request.Host}{document.FilePath}";
            
            return CreatedAtAction(nameof(GetDocument), new { id = document.Id },
                ApiResponse<AccountingDocumentDto>.SuccessResult(document, "سند با موفقیت ایجاد شد"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<AccountingDocumentDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<AccountingDocumentDto>.ErrorResult(
                "خطا در ایجاد سند", new List<string> { ex.Message }));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<AccountingDocumentDto>>> UpdateDocument(
        Guid id, [FromForm] UpdateAccountingDocumentDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(ApiResponse<AccountingDocumentDto>.ErrorResult("شناسه در URL و بدنه درخواست مطابقت ندارد"));
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(ApiResponse<AccountingDocumentDto>.ErrorResult("اطلاعات وارد شده معتبر نیست", errors));
        }

        try
        {
            var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var document = await _documentService.UpdateAsync(dto, webRootPath);
            
            document.FileUrl = $"{Request.Scheme}://{Request.Host}{document.FilePath}";
            
            return Ok(ApiResponse<AccountingDocumentDto>.SuccessResult(document, "سند با موفقیت به‌روزرسانی شد"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<AccountingDocumentDto>.ErrorResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<AccountingDocumentDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<AccountingDocumentDto>.ErrorResult(
                "خطا در به‌روزرسانی سند", new List<string> { ex.Message }));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteDocument(Guid id)
    {
        try
        {
            var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var result = await _documentService.DeleteAsync(id, webRootPath);
            
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResult("سند یافت نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResult(true, "سند با موفقیت حذف شد"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(
                "خطا در حذف سند", new List<string> { ex.Message }));
        }
    }
}

