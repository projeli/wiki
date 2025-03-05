FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["wiki/Projeli.WikiService.Api/Projeli.WikiService.Api.csproj", "wiki/Projeli.WikiService.Api/"]
COPY ["shared/Projeli.Shared.Domain/Projeli.Shared.Domain.csproj", "shared/Projeli.Shared.Domain/"]
COPY ["shared/Projeli.Shared.Application/Projeli.Shared.Application.csproj", "shared/Projeli.Shared.Application/"]
COPY ["shared/Projeli.Shared.Infrastructure/Projeli.Shared.Infrastructure.csproj", "shared/Projeli.Shared.Infrastructure/"]
RUN dotnet restore "wiki/Projeli.WikiService.Api/Projeli.WikiService.Api.csproj"
COPY ["wiki", "wiki"]
COPY ["shared", "shared"]
WORKDIR "/src/wiki/Projeli.WikiService.Api"
RUN dotnet build "Projeli.WikiService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Projeli.WikiService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Projeli.WikiService.Api.dll"]