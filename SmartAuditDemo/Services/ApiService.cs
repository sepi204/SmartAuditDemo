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
}


