using Microsoft.AspNetCore.Mvc;
using SmartAuditDemo.Api.Common;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Application.Services;

namespace SmartAuditDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationsController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<OrganizationDto>>>> GetOrganizations(
        [FromQuery] OrganizationFilterDto filter)
    {
        try
        {
            var result = await _organizationService.GetPagedAsync(filter);
            return Ok(ApiResponse<PagedResultDto<OrganizationDto>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PagedResultDto<OrganizationDto>>.ErrorResult(
                "خطا در دریافت اطلاعات", new List<string> { ex.Message }));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> GetOrganization(Guid id)
    {
        try
        {
            var organization = await _organizationService.GetByIdAsync(id);
            
            if (organization == null)
            {
                return NotFound(ApiResponse<OrganizationDto>.ErrorResult("سازمان یافت نشد"));
            }

            return Ok(ApiResponse<OrganizationDto>.SuccessResult(organization));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrganizationDto>.ErrorResult(
                "خطا در دریافت اطلاعات", new List<string> { ex.Message }));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> CreateOrganization(
        [FromBody] CreateOrganizationDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(ApiResponse<OrganizationDto>.ErrorResult("اطلاعات وارد شده معتبر نیست", errors));
        }

        try
        {
            var organization = await _organizationService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetOrganization), new { id = organization.Id },
                ApiResponse<OrganizationDto>.SuccessResult(organization, "سازمان با موفقیت ایجاد شد"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<OrganizationDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrganizationDto>.ErrorResult(
                "خطا در ایجاد سازمان", new List<string> { ex.Message }));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> UpdateOrganization(
        Guid id, [FromBody] UpdateOrganizationDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest(ApiResponse<OrganizationDto>.ErrorResult("شناسه در URL و بدنه درخواست مطابقت ندارد"));
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(ApiResponse<OrganizationDto>.ErrorResult("اطلاعات وارد شده معتبر نیست", errors));
        }

        try
        {
            var organization = await _organizationService.UpdateAsync(dto);
            return Ok(ApiResponse<OrganizationDto>.SuccessResult(organization, "سازمان با موفقیت به‌روزرسانی شد"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<OrganizationDto>.ErrorResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<OrganizationDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrganizationDto>.ErrorResult(
                "خطا در به‌روزرسانی سازمان", new List<string> { ex.Message }));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteOrganization(Guid id)
    {
        try
        {
            var result = await _organizationService.DeleteAsync(id);
            
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResult("سازمان یافت نشد"));
            }

            return Ok(ApiResponse<bool>.SuccessResult(true, "سازمان با موفقیت حذف شد"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(
                "خطا در حذف سازمان", new List<string> { ex.Message }));
        }
    }
}



