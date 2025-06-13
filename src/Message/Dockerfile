FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
# Copy nuget.config to the root of the source directory
COPY nuget.config .
# Copy nuget.config to the Message project directory
COPY nuget.config src/Message/

ARG GITHUB_TOKEN
ENV GITHUB_TOKEN=$GITHUB_TOKEN

COPY ["src/Message/Message.csproj", "src/Message/"]
COPY ["src/InternalContracts/InternalContracts.csproj", "src/InternalContracts/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
RUN dotnet restore "src/Message/Message.csproj"
COPY src/Message/. /src/Message/
COPY src/InternalContracts/. /src/InternalContracts/
COPY src/Application/. /src/Application/
COPY src/Infrastructure/. /src/Infrastructure/
COPY src/Domain/. /src/Domain/
# Copy License.xml from the root of the repo to the build context
COPY ../../License.xml /License.xml
WORKDIR /src/Message
RUN ls -l /src/Message
RUN dotnet build "Message.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Message.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Message.dll"]
