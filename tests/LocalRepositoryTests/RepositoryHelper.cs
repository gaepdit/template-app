using MyApp.LocalRepository.Identity;
using MyApp.LocalRepository.Repositories;
using MyApp.TestData;
using MyApp.TestData.Identity;

namespace LocalRepositoryTests;

public static class RepositoryHelper
{
    public static LocalCustomerRepository GetCustomerRepository()
    {
        ClearAllStaticData();
        return new LocalCustomerRepository(new LocalContactRepository());
    }

    public static LocalOfficeRepository GetOfficeRepository()
    {
        ClearAllStaticData();
        return new LocalOfficeRepository();
    }

    public static LocalUserStore GetLocalUserStore()
    {
        ClearAllStaticData();
        return new LocalUserStore();
    }

    private static void ClearAllStaticData()
    {
        ContactData.ClearData();
        CustomerData.ClearData();
        OfficeData.ClearData();
        UserData.ClearData();
    }
}
