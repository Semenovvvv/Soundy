FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5006

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/CatalogService/Soundy.CatalogService/Soundy.CatalogService.csproj", "Services/CatalogService/Soundy.CatalogService/"]
COPY ["SharedLibrary/Soundy.SharedLibrary/Soundy.SharedLibrary.csproj", "SharedLibrary/Soundy.SharedLibrary/"]
RUN dotnet restore "./Services/CatalogService/Soundy.CatalogService/Soundy.CatalogService.csproj"
COPY . .
WORKDIR "/src/Services/CatalogService/Soundy.CatalogService"
RUN dotnet build "./Soundy.CatalogService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Soundy.CatalogService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Soundy.CatalogService.dll"]