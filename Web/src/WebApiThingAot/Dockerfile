FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

RUN apt update
RUN apt install -y clang zlib1g-dev

ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["src/WebApiThingAot/WebApiThingAot.csproj", "src/WebApiThingAot/"]
RUN dotnet restore -r linux-x64 "src/WebApiThingAot/WebApiThingAot.csproj"
COPY . .
WORKDIR "/src/src/WebApiThingAot"
RUN dotnet build "WebApiThingAot.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebApiThingAot.csproj" -c $BUILD_CONFIGURATION -o /app/publish -r linux-x64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV SERVER_URLS=http://*:8080

ENTRYPOINT ["./WebApiThingAot"]
