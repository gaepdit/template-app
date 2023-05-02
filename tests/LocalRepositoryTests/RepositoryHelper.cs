using MyAppRoot.LocalRepository.Identity;
using MyAppRoot.LocalRepository.Repositories;
using MyAppRoot.TestData;
using MyAppRoot.TestData.Identity;

namespace LocalRepositoryTests;

public static class RepositoryHelper
{
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
        OfficeData.ClearData();
        UserData.ClearData();
    }
}
