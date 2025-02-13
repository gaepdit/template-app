# Entity Framework Database Migrations

Any time changes are made to the `DbContext` or the domain entities that would require a modification to the database,
this process can be managed using Entity Framework Migrations.

## Setup

Install the EF command line tool globally. This only needs to be done once.

`dotnet tool install --global dotnet-ef`

## Create a migration

1. Build the solution.

2. Open a command prompt to the `./src/EfRepository/` folder.

3. Run the following command with an appropriate migration name in place of `NAME_OF_MIGRATION`:

   `dotnet ef migrations add NAME_OF_MIGRATION --msbuildprojectextensionspath ..\..\.artifacts\EfRepository\obj\`

The above command generates database migration files in the `./src/EfRepository/Migrations` folder. (These should be
committed to the Git repository.) New migrations will be automatically applied to the database the first time the 
updated application is run.
