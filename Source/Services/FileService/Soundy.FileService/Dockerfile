
# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 5007

RUN apt-get update && \
    apt-get install -y ffmpeg && \
    apt-get clean

USER $APP_UID

# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/FileService/Soundy.FileService/Soundy.FileService.csproj", "Services/FileService/Soundy.FileService/"]
COPY ["SharedLibrary/Soundy.SharedLibrary.csproj", "SharedLibrary/"]
RUN dotnet restore "./Services/FileService/Soundy.FileService/Soundy.FileService.csproj"
COPY . .
WORKDIR "/src/Services/FileService/Soundy.FileService"
RUN dotnet build "./Soundy.FileService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Soundy.FileService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Soundy.FileService.dll"]