FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["KProject.Api/KProject.Api.csproj", "KProject.Api/"]
COPY ["KProject.Infrastructure/KProject.Infrastructure.csproj", "KProject.Infrastructure/"]
RUN dotnet restore "KProject.Api/KProject.Api.csproj"
COPY . .
WORKDIR "/src/KProject.Api"
RUN dotnet build "./KProject.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KProject.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KProject.Api.dll"]
