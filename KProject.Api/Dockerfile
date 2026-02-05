FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["KProject.Api/KProject.Api.csproj", "KProject.Api/"]
COPY ["KProject.Infrastructure/KProject.Infrastructure.csproj", "KProject.Infrastructure/"]
RUN dotnet restore "KProject.Api/KProject.Api.csproj"
COPY . .
WORKDIR "/src/KProject.Api"
RUN dotnet build "./KProject.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KProject.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM node:20-alpine AS angular-build
WORKDIR /app

COPY KProject.Web/package.json KProject.Web/package-lock.json ./
RUN npm ci

COPY KProject.Web/ ./

RUN npm run prod

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
COPY --from=angular-build /app/dist /app/wwwroot

ENTRYPOINT ["dotnet", "KProject.Api.dll"]
