@echo off
echo Re-creating solution file...
echo .
del template-app.sln
dotnet new sln
dotnet sln add --in-root .\src\Domain\Domain.csproj
dotnet sln add --in-root .\src\TestData\TestData.csproj
dotnet sln add --in-root .\src\LocalRepository\LocalRepository.csproj
dotnet sln add --in-root .\src\Infrastructure\Infrastructure.csproj
dotnet sln add --in-root .\src\WebApp\WebApp.csproj
dotnet sln add --in-root .\src\AppServices\AppServices.csproj
dotnet sln add -s tests .\tests\DomainTests\DomainTests.csproj
dotnet sln add -s tests .\tests\LocalRepositoryTests\LocalRepositoryTests.csproj
dotnet sln add -s tests .\tests\IntegrationTests\IntegrationTests.csproj
dotnet sln add -s tests .\tests\AppServicesTests\AppServicesTests.csproj
dotnet sln add -s tests .\tests\WebAppTests\WebAppTests.csproj
echo .
echo Finished creating a new solution file. This batch file will now be deleted.
@pause
del %0
