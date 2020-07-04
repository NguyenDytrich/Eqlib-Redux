"%userprofile%"\.nuget\packages\OpenCover\4.7.922\tools\OpenCover.Console.exe^
 -target:"C:\Program Files\dotnet\dotnet.exe"^
 -targetargs:"test EqlibApi.Tests/EqlibApi.Tests.csproj"^
 -filter:"+[EqlibApi*]* -[EqlibApi.Tests]* -[EqlibApi]*.Migrations.*"^
 -register^
 -output:"CoverageResult.xml"

"%userprofile%"\.nuget\packages\ReportGenerator\4.5.6\tools\netcoreapp3.0\ReportGenerator.exe^
 -reports:CoverageResult.xml^
 -targetdir:coverage

start coverage/index.htm