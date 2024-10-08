﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/MakStore.Shared/MakStore.Domain/MakStore.Domain.csproj", "src/MakStore.Shared/MakStore.Domain/"]
COPY ["src/MakStore.Shared/MakStore.SharedComponents/MakStore.SharedComponents.csproj", "src/MakStore.Shared/MakStore.SharedComponents/"]

COPY ["src/MakStore.Microservices/ProductsService/ProductsService.csproj", "src/MakStore.Microservices/ProductsService/"]
RUN dotnet restore "src/MakStore.Microservices/ProductsService/ProductsService.csproj"
COPY . .
WORKDIR "/src/src/MakStore.Microservices/ProductsService"
RUN dotnet build "ProductsService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProductsService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductsService.dll"]
