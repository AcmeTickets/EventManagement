FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

ARG PORT=5271
ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE ${PORT}

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release


WORKDIR /src
# Copy nuget.config to the root of the source directory
COPY nuget.config .
# Copy nuget.config to the Api project directory
COPY nuget.config src/Api/

ARG GITHUB_TOKEN
ENV GITHUB_TOKEN=$GITHUB_TOKEN

COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/InternalContracts/InternalContracts.csproj", "src/InternalContracts/"]
RUN dotnet restore "src/Api/Api.csproj"
COPY src/Api/. /src/Api/
COPY src/InternalContracts/. /src/InternalContracts/
WORKDIR /src/Api
RUN ls -l /src/Api
RUN dotnet build "Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]