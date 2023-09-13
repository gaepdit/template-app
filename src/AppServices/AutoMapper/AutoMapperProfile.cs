using AutoMapper;
using MyApp.AppServices.Customers.Dto;
using MyApp.AppServices.Offices;
using MyApp.AppServices.Staff.Dto;
using MyApp.Domain.Entities.Contacts;
using MyApp.Domain.Entities.Customers;
using MyApp.Domain.Entities.Offices;
using MyApp.Domain.Identity;

namespace MyApp.AppServices.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Contacts
        CreateMap<Contact, ContactViewDto>();
        CreateMap<Contact, ContactUpdateDto>();

        // Customers
        CreateMap<Customer, CustomerViewDto>()
            .ForMember(dto => dto.DeletedBy, expression => expression.Ignore());
        CreateMap<Customer, CustomerUpdateDto>();
        CreateMap<Customer, CustomerSearchResultDto>();

        // Offices
        CreateMap<Office, OfficeViewDto>();
        CreateMap<Office, OfficeUpdateDto>();

        // Staff
        CreateMap<ApplicationUser, StaffViewDto>();
        CreateMap<ApplicationUser, StaffUpdateDto>();
        CreateMap<ApplicationUser, StaffSearchResultDto>();
    }
}
