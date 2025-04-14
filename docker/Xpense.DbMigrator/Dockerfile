FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . ./

# Optional: only copy if newer (handled by Docker layer caching automatically)
RUN dotnet publish Xpense.Utils.Postgres.Migrator.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

COPY --from=build /app/publish .

# If you want to make sure appsettings.json is included explicitly:
# COPY ./Xpense.Utils.Postgres.Migrator/appsettings.json ./appsettings.json

ENTRYPOINT ["dotnet", "Xpense.Utils.Postgres.Migrator.dll"]