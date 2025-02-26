using AutoMapper;
using MyApp.AppServices.AutoMapper;

namespace AppServicesTests;

[SetUpFixture]
public class AppServicesTestsSetup
{
    internal static IMapper? Mapper;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // AutoMapper profiles are added here.
        Mapper = new MapperConfiguration(expression => expression.AddProfile(new AutoMapperProfile())).CreateMapper();

        AssertionConfiguration.Current.Equivalency.Modify(options => options
            // Setting this option globally since our DTOs generally exclude properties, e.g., audit properties.
            // See: https://fluentassertions.com/objectgraphs/#matching-members
            .ExcludingMissingMembers()
        );
    }
}
