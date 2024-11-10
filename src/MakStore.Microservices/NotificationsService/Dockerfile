FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# add allowed https certificate
ARG CERT_PATH=./https/hostdockerinternal.crt
USER root
COPY ${CERT_PATH} /usr/local/share/ca-certificates/hostdockerinternal.crt

RUN chmod 644 /usr/local/share/ca-certificates/hostdockerinternal.crt && \
    update-ca-certificates
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MakStore.Shared/MakStore.Domain/MakStore.Domain.csproj", "src/MakStore.Shared/MakStore.Domain/"]
COPY ["src/MakStore.Shared/MakStore.SharedComponents/MakStore.SharedComponents.csproj", "src/MakStore.Shared/MakStore.SharedComponents/"]

COPY ["src/MakStore.Microservices/NotificationsService/NotificationsService.csproj", "src/MakStore.Microservices/NotificationsService/"]
RUN dotnet restore "src/MakStore.Microservices/NotificationsService/NotificationsService.csproj"
COPY . .
WORKDIR "/src/src/MakStore.Microservices/NotificationsService"
RUN dotnet build "NotificationsService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NotificationsService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationsService.dll"]
