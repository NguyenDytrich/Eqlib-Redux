#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["EqlibApi/EqlibApi.csproj", "EqlibApi/"]
RUN dotnet restore "EqlibApi/EqlibApi.csproj"
COPY . .
WORKDIR "/src/EqlibApi"
RUN dotnet build "EqlibApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EqlibApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > wait_for_it.sh
ENTRYPOINT ["dotnet", "EqlibApi.dll"]
