FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CurrencyConverter.Api/CurrencyConverter.Api.csproj", "CurrencyConverter.Api/"]
RUN dotnet restore "CurrencyConverter.Api/CurrencyConverter.Api.csproj"
COPY . .
WORKDIR "/src/CurrencyConverter.Api"
RUN dotnet build "./CurrencyConverter.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CurrencyConverter.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CurrencyConverter.Api.dll"]
