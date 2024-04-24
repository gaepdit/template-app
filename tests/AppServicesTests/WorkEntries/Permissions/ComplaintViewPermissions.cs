﻿using Microsoft.AspNetCore.Authorization;
using MyApp.AppServices.WorkEntries.Permissions;
using MyApp.AppServices.WorkEntries.QueryDto;
using MyApp.AppServices.Staff.Dto;
using MyApp.Domain.Identity;
using System.Security.Claims;

namespace AppServicesTests.WorkEntries.Permissions;

public class WorkEntryViewPermissions
{
    [Test]
    public async Task ManageDeletions_WhenAllowed_Succeeds()
    {
        var requirements = new[] { WorkEntryOperation.ManageDeletions };
        // The value for the `authenticationType` parameter causes
        // `ClaimsIdentity.IsAuthenticated` to be set to `true`.
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new Claim[] { new(ClaimTypes.Role, RoleName.Manager) },
            "Basic"));
        var resource = new WorkEntryViewDto();
        var context = new AuthorizationHandlerContext(requirements, user, resource);
        var handler = new WorkEntryViewRequirements();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Test]
    public async Task ManageDeletions_WhenNotAuthenticated_DoesNotSucceed()
    {
        var requirements = new[] { WorkEntryOperation.ManageDeletions };
        // This `ClaimsPrincipal` is not authenticated.
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new Claim[] { new(ClaimTypes.Role, RoleName.Manager) }));
        var resource = new WorkEntryViewDto();
        var context = new AuthorizationHandlerContext(requirements, user, resource);
        var handler = new WorkEntryViewRequirements();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    [Test]
    public async Task ManageDeletions_WhenNotAllowed_DoesNotSucceed()
    {
        var requirements = new[] { WorkEntryOperation.ManageDeletions };
        var user = new ClaimsPrincipal(new ClaimsIdentity("Basic"));
        var resource = new WorkEntryViewDto();
        var context = new AuthorizationHandlerContext(requirements, user, resource);
        var handler = new WorkEntryViewRequirements();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    // Original reporter can edit for 1 hour.
    [Test]
    public async Task RecentReporter_IfEnteredWithinPastHour_Succeeds()
    {
        var requirements = new[] { WorkEntryOperation.EditWorkEntry };
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new Claim[]
            {
                new(ClaimTypes.NameIdentifier, Guid.Empty.ToString()),
                new(ClaimTypes.Role, RoleName.Staff),
            },
            "Basic"));
        var resource = new WorkEntryViewDto
        {
            ReceivedDate = DateTimeOffset.Now.AddMinutes(-30),
            ReceivedBy = new StaffViewDto { Id = Guid.Empty.ToString() },
        };
        var context = new AuthorizationHandlerContext(requirements, user, resource);
        var handler = new WorkEntryViewRequirements();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Test]
    public async Task RecentReporter_IfEnteredBeforePastHour_DoesNotSucceed()
    {
        var requirements = new[] { WorkEntryOperation.EditWorkEntry };
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new Claim[]
            {
                new(ClaimTypes.NameIdentifier, Guid.Empty.ToString()),
                new(ClaimTypes.Role, RoleName.Staff),
            },
            "Basic"));
        var resource = new WorkEntryViewDto
        {
            ReceivedDate = DateTimeOffset.Now.AddMinutes(-90),
            ReceivedBy = new StaffViewDto { Id = Guid.Empty.ToString() },
        };
        var context = new AuthorizationHandlerContext(requirements, user, resource);
        var handler = new WorkEntryViewRequirements();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }
}
