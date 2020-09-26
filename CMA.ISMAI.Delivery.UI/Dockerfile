#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CMA.ISMAI.Delivery.UI/CMA.ISMAI.Delivery.UI.csproj", "CMA.ISMAI.Delivery.UI/"]
COPY ["CMA.ISMAI.Delivery.Logging.Log/CMA.ISMAI.Delivery.Logging.csproj", "CMA.ISMAI.Delivery.Logging.Log/"]
RUN dotnet restore "CMA.ISMAI.Delivery.UI/CMA.ISMAI.Delivery.UI.csproj"
COPY . .
WORKDIR "/src/CMA.ISMAI.Delivery.UI"
RUN dotnet build "CMA.ISMAI.Delivery.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMA.ISMAI.Delivery.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMA.ISMAI.Delivery.UI.dll"]