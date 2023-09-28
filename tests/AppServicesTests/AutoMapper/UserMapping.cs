using FluentAssertions.Execution;
using MyApp.AppServices.Staff.Dto;
using MyApp.Domain.Entities.Offices;
using MyApp.Domain.Identity;
using MyApp.TestData.Constants;

namespace AppServicesTests.AutoMapper;

public class UserMapping
{
    private readonly ApplicationUser _item = new()
    {
        Id = Guid.NewGuid().ToString(),
        GivenName = TextData.ValidName,
        FamilyName = TextData.NewValidName,
        Email = TextData.ValidEmail,
        Phone = TextData.ValidPhoneNumber,
        Office = new Office(Guid.NewGuid(), TextData.ValidName),
    };

    [Test]
    public void StaffViewMappingWorks()
    {
        var result = AppServicesTestsSetup.Mapper!.Map<StaffViewDto>(_item);

        using (new AssertionScope())
        {
            result.Id.Should().Be(_item.Id);
            result.GivenName.Should().Be(_item.GivenName);
            result.FamilyName.Should().Be(_item.FamilyName);
            result.Email.Should().Be(_item.Email);
            result.Phone.Should().Be(_item.Phone);
            result.Office.Should().BeEquivalentTo(_item.Office);
            result.Active.Should().BeTrue();
        }
    }

    [Test]
    public void StaffSearchResultMappingWorks()
    {
        var result = AppServicesTestsSetup.Mapper!.Map<StaffSearchResultDto>(_item);

        using (new AssertionScope())
        {
            result.Id.Should().Be(_item.Id);
            result.SortableFullName.Should().Be($"{_item.FamilyName}, {_item.GivenName}");
            result.Email.Should().Be(_item.Email);
            result.OfficeName.Should().Be(_item.Office!.Name);
            result.Active.Should().BeTrue();
        }
    }

    [Test]
    public void StaffUpdateMappingWorks()
    {
        var result = AppServicesTestsSetup.Mapper!.Map<StaffUpdateDto>(_item);

        using (new AssertionScope())
        {
            result.Phone.Should().Be(_item.Phone);
            result.OfficeId.Should().Be(_item.Office!.Id);
            result.Active.Should().BeTrue();
        }
    }

    [Test]
    public void NullStaffViewMappingWorks()
    {
        ApplicationUser? item = null;
        var result = AppServicesTestsSetup.Mapper!.Map<StaffViewDto?>(item);
        result.Should().BeNull();
    }
}
