using Microsoft.AspNetCore.Authorization;
using MyApp.AppServices.Notifications;
using MyApp.AppServices.UserServices;
using MyApp.AppServices.WorkEntries;
using MyApp.AppServices.WorkEntries.CommandDto;
using MyApp.Domain.Entities.EntryTypes;
using MyApp.Domain.Entities.WorkEntries;
using MyApp.Domain.Identity;
using MyApp.TestData.Constants;

namespace AppServicesTests.WorkEntries;

public class Create
{
    private readonly ApplicationUser _user = new() { Id = Guid.Empty.ToString(), Email = TextData.ValidEmail };

    [Test]
    public async Task OnSuccessfulInsert_ReturnsSuccessfully()
    {
        // Arrange
        var userServiceMock = Substitute.For<IUserService>();
        userServiceMock.GetCurrentUserAsync()
            .Returns(_user);

        var id = Guid.NewGuid();
        var workEntryManagerMock = Substitute.For<IWorkEntryManager>();
        workEntryManagerMock.Create(Arg.Any<ApplicationUser?>())
            .Returns(new WorkEntry(id));

        userServiceMock.GetUserAsync(Arg.Any<string>())
            .Returns(_user);
        userServiceMock.FindUserAsync(Arg.Any<string>())
            .Returns(_user);

        var notificationMock = Substitute.For<INotificationService>();
        notificationMock
            .SendNotificationAsync(Arg.Any<Template>(), Arg.Any<string>(), Arg.Any<WorkEntry>(),
                Arg.Any<CancellationToken>())
            .Returns(NotificationResult.SuccessResult());

        var appService = new WorkEntryService(AppServicesTestsSetup.Mapper!, Substitute.For<IWorkEntryRepository>(),
            Substitute.For<IEntryTypeRepository>(), workEntryManagerMock, notificationMock, userServiceMock,
            Substitute.For<IAuthorizationService>());

        var item = new WorkEntryCreateDto { EntryTypeId = Guid.Empty, Notes = TextData.Phrase };

        // Act
        var result = await appService.CreateAsync(item, CancellationToken.None);

        // Assert
        using var scope = new AssertionScope();
        result.HasWarnings.Should().BeFalse();
        result.WorkEntryId.Should().Be(id);
    }
}
