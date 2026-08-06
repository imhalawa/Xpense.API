# syntax=docker/dockerfile:1

# The build context is the repository root, not this directory -- the build needs the whole solution.
# Compose passes `context: .` with `dockerfile: docker/notifications/Dockerfile`.
#
# The stanza up to `dotnet restore` is byte-identical to docker/api/Dockerfile and
# docker/migrations/Dockerfile on purpose. BuildKit keys its cache on layer content rather than on
# which Dockerfile produced it, so the restore happens once and the other two reuse it. Keep the
# three in step: if they drift, the builds still work, they just quietly stop sharing and get slower.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Xpense/Xpense.sln ./
COPY src/Xpense/Xpense.Domain/Xpense.Domain.csproj Xpense.Domain/
COPY src/Xpense/Xpense.Persistence/Xpense.Persistence.csproj Xpense.Persistence/
COPY src/Xpense/Xpense.API/Xpense.API.csproj Xpense.API/
COPY src/Xpense/Xpense.Notifications/Xpense.Notifications.csproj Xpense.Notifications/
COPY src/Xpense/Xpense.Tests/Xpense.Tests.csproj Xpense.Tests/
RUN dotnet restore Xpense.sln

COPY src/Xpense/ ./
RUN dotnet publish Xpense.Notifications/Xpense.Notifications.csproj -c Release -o /app --no-restore

# runtime, not aspnet: this is a worker. It serves no HTTP, binds no port, and has no reason to carry
# ASP.NET Core.
FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine AS final
WORKDIR /app

# Nothing here writes to disk or binds a port, so it has no reason to be root.
RUN adduser -D -u 1001 xpense
USER xpense

COPY --from=build /app ./

ENTRYPOINT ["dotnet", "Xpense.Notifications.dll"]
