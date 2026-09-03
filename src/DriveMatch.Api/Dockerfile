FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/DriveMatch.Api/DriveMatch.Api.csproj", "src/DriveMatch.Api/"]
COPY ["src/DriveMatch.Application/DriveMatch.Application.csproj", "src/DriveMatch.Application/"]
COPY ["src/DriveMatch.Domain/DriveMatch.Domain.csproj", "src/DriveMatch.Domain/"]
COPY ["src/DriveMatch.Infrastructure/DriveMatch.Infrastructure.csproj", "src/DriveMatch.Infrastructure/"]

RUN dotnet restore "src/DriveMatch.Api/DriveMatch.Api.csproj"

COPY . .

RUN dotnet publish "src/DriveMatch.Api/DriveMatch.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DriveMatch.Api.dll"]