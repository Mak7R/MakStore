FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MakStore.Shared/MakStore.SharedComponents/MakStore.SharedComponents.csproj", "src/MakStore.Shared/MakStore.SharedComponents/"]

COPY ["src/MakStore.ClientApps/EmployeeWebClient/EmployeeWebClient.csproj", "src/MakStore.ClientApps/EmployeeWebClient/"]
RUN dotnet restore "src/MakStore.ClientApps/EmployeeWebClient/EmployeeWebClient.csproj"
COPY . .
WORKDIR "/src/src/MakStore.ClientApps/EmployeeWebClient"
RUN dotnet build "EmployeeWebClient.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EmployeeWebClient.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeWebClient.dll"]
