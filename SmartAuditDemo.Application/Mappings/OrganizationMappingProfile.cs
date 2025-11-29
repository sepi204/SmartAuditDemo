using AutoMapper;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Domain.Entities;
using SmartAuditDemo.Domain.Enums;

namespace SmartAuditDemo.Application.Mappings;

public class OrganizationMappingProfile : Profile
{
    public OrganizationMappingProfile()
    {
        CreateMap<Organization, OrganizationDto>()
            .ForMember(dest => dest.CompanyTypeName, opt => opt.MapFrom(src => GetCompanyTypeName(src.CompanyType)))
            .ForMember(dest => dest.BusinessFieldName, opt => opt.MapFrom(src => GetBusinessFieldName(src.BusinessField)));

        CreateMap<CreateOrganizationDto, Organization>();
        CreateMap<UpdateOrganizationDto, Organization>();
    }

    private static string GetCompanyTypeName(CompanyType type)
    {
        return type switch
        {
            CompanyType.PrivateCompany => "شرکت خصوصی",
            CompanyType.PublicCompany => "شرکت سهامی عام",
            CompanyType.LimitedLiability => "شرکت با مسئولیت محدود",
            CompanyType.Cooperative => "شرکت تعاونی",
            CompanyType.Individual => "شخص حقیقی",
            _ => type.ToString()
        };
    }

    private static string GetBusinessFieldName(BusinessField field)
    {
        return field switch
        {
            BusinessField.Technology => "فناوری",
            BusinessField.Construction => "ساخت و ساز",
            BusinessField.Trading => "بازرگانی",
            BusinessField.Manufacturing => "تولیدی",
            BusinessField.Services => "خدماتی",
            BusinessField.Finance => "مالی",
            BusinessField.Agriculture => "کشاورزی",
            _ => field.ToString()
        };
    }
}

