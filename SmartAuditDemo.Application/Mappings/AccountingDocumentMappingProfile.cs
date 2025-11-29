using AutoMapper;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Domain.Entities;
using SmartAuditDemo.Domain.Enums;

namespace SmartAuditDemo.Application.Mappings;

public class AccountingDocumentMappingProfile : Profile
{
    public AccountingDocumentMappingProfile()
    {
        CreateMap<AccountingDocument, AccountingDocumentDto>()
            .ForMember(dest => dest.DocumentTypeName, opt => opt.MapFrom(src => GetDocumentTypeName(src.DocumentType)))
            .ForMember(dest => dest.UploaderUserName, opt => opt.MapFrom(src => "کاربر سیستم")); // TODO: Get from UserService

        CreateMap<CreateAccountingDocumentDto, AccountingDocument>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Extension, opt => opt.Ignore())
            .ForMember(dest => dest.FilePath, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.SizeInKb, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<UpdateAccountingDocumentDto, AccountingDocument>()
            .ForMember(dest => dest.Extension, opt => opt.Ignore())
            .ForMember(dest => dest.FilePath, opt => opt.Ignore())
            .ForMember(dest => dest.UploaderUserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.SizeInKb, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }

    private static string GetDocumentTypeName(DocumentType type)
    {
        return type switch
        {
            DocumentType.Voucher => "سند حسابداری",
            DocumentType.Invoice => "فاکتور یا صورتحساب",
            DocumentType.Contract => "قرارداد",
            DocumentType.Letter => "مکاتبات",
            DocumentType.Report => "گزارشات",
            DocumentType.Ledger => "دفاتر",
            DocumentType.Other => "سایر",
            _ => type.ToString()
        };
    }
}
