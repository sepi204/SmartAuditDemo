using System.Text;
using System.Text.Json;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Models;

namespace SmartAuditDemo.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        
        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001";
        _httpClient.BaseAddress = new Uri(apiBaseUrl);
    }

    public async Task<PagedResultDto<OrganizationDto>?> GetOrganizationsAsync(OrganizationFilterDto filter)
    {
        try
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(filter.Name))
                queryParams.Add($"Name={Uri.EscapeDataString(filter.Name)}");
            if (!string.IsNullOrWhiteSpace(filter.EconomicCode))
                queryParams.Add($"EconomicCode={Uri.EscapeDataString(filter.EconomicCode)}");
            if (filter.CompanyType.HasValue)
                queryParams.Add($"CompanyType={(int)filter.CompanyType.Value}");
            if (filter.IsActive.HasValue)
                queryParams.Add($"IsActive={filter.IsActive.Value}");
            queryParams.Add($"PageNumber={filter.PageNumber}");
            queryParams.Add($"PageSize={filter.PageSize}");

            var url = $"/api/organizations?{string.Join("&", queryParams)}";
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResultDto<OrganizationDto>>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                return apiResponse?.Data;
            }

            _logger.LogError("خطا در دریافت سازمان‌ها: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت سازمان‌ها");
            return null;
        }
    }

    public async Task<OrganizationDto?> GetOrganizationByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/organizations/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<OrganizationDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                return apiResponse?.Data;
            }

            _logger.LogError("خطا در دریافت سازمان: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت سازمان");
            return null;
        }
    }

    public async Task<(bool Success, string Message, OrganizationDto? Data)> CreateOrganizationAsync(CreateOrganizationDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/organizations", content);
            var responseJson = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<OrganizationDto>>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (response.IsSuccessStatusCode && apiResponse?.Success == true)
            {
                return (true, apiResponse.Message, apiResponse.Data);
            }

            return (false, apiResponse?.Message ?? "خطا در ایجاد سازمان", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ایجاد سازمان");
            return (false, "خطا در ارتباط با سرور", null);
        }
    }

    public async Task<(bool Success, string Message, OrganizationDto? Data)> UpdateOrganizationAsync(UpdateOrganizationDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"/api/organizations/{dto.Id}", content);
            var responseJson = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<OrganizationDto>>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (response.IsSuccessStatusCode && apiResponse?.Success == true)
            {
                return (true, apiResponse.Message, apiResponse.Data);
            }

            return (false, apiResponse?.Message ?? "خطا در به‌روزرسانی سازمان", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در به‌روزرسانی سازمان");
            return (false, "خطا در ارتباط با سرور", null);
        }
    }

    public async Task<(bool Success, string Message)> DeleteOrganizationAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/organizations/{id}");
            var responseJson = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (response.IsSuccessStatusCode && apiResponse?.Success == true)
            {
                return (true, apiResponse.Message);
            }

            return (false, apiResponse?.Message ?? "خطا در حذف سازمان");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در حذف سازمان");
            return (false, "خطا در ارتباط با سرور");
        }
    }

    // Accounting Documents Methods
    public async Task<PagedResultDto<AccountingDocumentDto>?> GetDocumentsAsync(AccountingDocumentFilterDto filter)
    {
        try
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(filter.Title))
                queryParams.Add($"Title={Uri.EscapeDataString(filter.Title)}");
            if (filter.DocumentType.HasValue)
                queryParams.Add($"DocumentType={(int)filter.DocumentType.Value}");
            if (!string.IsNullOrWhiteSpace(filter.Extension))
                queryParams.Add($"Extension={Uri.EscapeDataString(filter.Extension)}");
            queryParams.Add($"PageNumber={filter.PageNumber}");
            queryParams.Add($"PageSize={filter.PageSize}");

            var url = $"/api/accountingdocuments?{string.Join("&", queryParams)}";
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResultDto<AccountingDocumentDto>>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                return apiResponse?.Data;
            }

            _logger.LogError("خطا در دریافت اسناد: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت اسناد");
            return null;
        }
    }

    public async Task<AccountingDocumentDto?> GetDocumentByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/accountingdocuments/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountingDocumentDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                return apiResponse?.Data;
            }

            _logger.LogError("خطا در دریافت سند: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت سند");
            return null;
        }
    }

    public async Task<(bool Success, string Message, AccountingDocumentDto? Data)> CreateDocumentAsync(
        CreateAccountingDocumentDto dto, IFormFile file)
    {
        try
        {
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(dto.Title), "Title");
            formData.Add(new StringContent(((int)dto.DocumentType).ToString()), "DocumentType");
            formData.Add(new StringContent(dto.UploaderUserId.ToString()), "UploaderUserId");
            
            if (file != null)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                formData.Add(fileContent, "File", file.FileName);
            }
            
            var response = await _httpClient.PostAsync("/api/accountingdocuments", formData);
            var responseJson = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountingDocumentDto>>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (response.IsSuccessStatusCode && apiResponse?.Success == true)
            {
                return (true, apiResponse.Message, apiResponse.Data);
            }

            return (false, apiResponse?.Message ?? "خطا در ایجاد سند", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ایجاد سند");
            return (false, "خطا در ارتباط با سرور", null);
        }
    }

    public async Task<(bool Success, string Message, AccountingDocumentDto? Data)> UpdateDocumentAsync(
        UpdateAccountingDocumentDto dto, IFormFile? file)
    {
        try
        {
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(dto.Id.ToString()), "Id");
            formData.Add(new StringContent(dto.Title), "Title");
            formData.Add(new StringContent(((int)dto.DocumentType).ToString()), "DocumentType");
            
            if (file != null)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                formData.Add(fileContent, "File", file.FileName);
            }
            
            var response = await _httpClient.PutAsync($"/api/accountingdocuments/{dto.Id}", formData);
            var responseJson = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AccountingDocumentDto>>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (response.IsSuccessStatusCode && apiResponse?.Success == true)
            {
                return (true, apiResponse.Message, apiResponse.Data);
            }

            return (false, apiResponse?.Message ?? "خطا در به‌روزرسانی سند", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در به‌روزرسانی سند");
            return (false, "خطا در ارتباط با سرور", null);
        }
    }

    public async Task<(bool Success, string Message)> DeleteDocumentAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/accountingdocuments/{id}");
            var responseJson = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (response.IsSuccessStatusCode && apiResponse?.Success == true)
            {
                return (true, apiResponse.Message);
            }

            return (false, apiResponse?.Message ?? "خطا در حذف سند");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در حذف سند");
            return (false, "خطا در ارتباط با سرور");
        }
    }

    // Financial Statement Methods
    public async Task<(bool Success, string Message, byte[]? Data, string? FileName)> GenerateFinancialStatementAsync(Guid documentId)
    {
        try
        {
            var request = new { DocumentId = documentId };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/financial-statements", content);
            
            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') 
                    ?? $"صورت_مالی_{documentId:N}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                
                return (true, "صورت مالی با موفقیت تولید شد", fileBytes, fileName);
            }

            var errorJson = await response.Content.ReadAsStringAsync();
            string errorMessage = "خطا در تولید صورت مالی";
            
            try
            {
                // سعی می‌کنیم پیام خطا را از JSON استخراج کنیم
                using var doc = JsonDocument.Parse(errorJson);
                if (doc.RootElement.TryGetProperty("message", out var messageElement))
                {
                    errorMessage = messageElement.GetString() ?? errorMessage;
                }
            }
            catch
            {
                // اگر JSON نبود، از متن خام استفاده می‌کنیم
                if (!string.IsNullOrWhiteSpace(errorJson))
                {
                    errorMessage = errorJson;
                }
            }

            return (false, errorMessage, null, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تولید صورت مالی");
            return (false, "خطا در ارتباط با سرور", null, null);
        }
    }

    // Upload and Generate Financial Statement
    public async Task<(bool Success, string Message, byte[]? Data, string? FileName)> GenerateFinancialStatementFromUploadAsync(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return (false, "فایل الزامی است", null, null);
            }

            // بررسی نوع فایل
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return (false, "فقط فایل‌های Excel (.xlsx, .xls) مجاز هستند", null, null);
            }

            using var formData = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            formData.Add(fileContent, "file", file.FileName);

            var response = await _httpClient.PostAsync("/api/financial-statements/upload", formData);

            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                    ?? $"صورت_مالی_{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return (true, "صورت مالی با موفقیت تولید شد", fileBytes, fileName);
            }

            var errorJson = await response.Content.ReadAsStringAsync();
            string errorMessage = "خطا در تولید صورت مالی";

            try
            {
                using var doc = JsonDocument.Parse(errorJson);
                if (doc.RootElement.TryGetProperty("message", out var messageElement))
                {
                    errorMessage = messageElement.GetString() ?? errorMessage;
                }
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(errorJson))
                {
                    errorMessage = errorJson;
                }
            }

            return (false, errorMessage, null, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در آپلود و تولید صورت مالی");
            return (false, "خطا در ارتباط با سرور", null, null);
        }
    }
}


