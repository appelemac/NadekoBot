
rm .\bin\Debug\netcoreapp1.0\NadekoDB.db
if (!( Test-Path .\bin\Debug\netcoreapp1.0\NadekoDB.db)) {
dotnet ef migrations remove
if (!(Test-Path .\Migrations\*)) {
dotnet ef migrations add NadekoDB
dotnet ef database update
}
}

