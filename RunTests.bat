dotnet test --filter TestCategory="Unit" ^
    --results-directory ./Coverage ^
    /p:CollectCoverage=true ^
    /p:CoverletOutput=..\Coverage\ ^
    /p:CoverletOutputFormat=cobertura ^
    /p:Exclude=\"[*]EqlibApi.Migrations.*,[EqlibApi.Program]\" ^
    EqlibApi.Tests

dotnet "%userprofile%"\.nuget\packages\ReportGenerator\4.5.6\tools\netcoreapp3.0\ReportGenerator.dll^
    "-reports:Coverage\coverage.cobertura.xml" ^
    "-targetdir:Coverage"
    -reporttypes:HTML

start .\Coverage\index.htm\