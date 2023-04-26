using AutoMapper;
using MyAppRoot.AppServices.Offices;
using MyAppRoot.AppServices.Staff;
using MyAppRoot.Domain.Entities.Offices;
using MyAppRoot.Domain.Identity;

namespace MyAppRoot.AppServices.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Office, OfficeViewDto>();
        CreateMap<Office, OfficeUpdateDto>()
            .ForMember(d => d.CurrentUserOfficeId, o => o.Ignore());

        CreateMap<ApplicationUser, StaffViewDto>();
        CreateMap<ApplicationUser, StaffUpdateDto>();
        // CreateMap<ApplicationUser, StaffSearchResultDto>();
    }
}
