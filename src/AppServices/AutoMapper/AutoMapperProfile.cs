using AutoMapper;
using MyAppRoot.AppServices.Offices;
using MyAppRoot.AppServices.Staff;
using MyAppRoot.Domain.Entities;

namespace MyAppRoot.AppServices.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Office, OfficeViewDto>().ReverseMap();
        CreateMap<Office, OfficeUpdateDto>();

        CreateMap<ApplicationUser, StaffViewDto>().ReverseMap();
        CreateMap<ApplicationUser, StaffUpdateDto>();
    }
}
