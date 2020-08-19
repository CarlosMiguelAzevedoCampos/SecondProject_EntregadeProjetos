#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CMA.ISMAI.Delivery.FileLoading.UI/CMA.ISMAI.Delivery.FileLoading.UI.csproj", "CMA.ISMAI.Delivery.FileLoading.UI/"]
COPY ["CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileVerifier/CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileVerifier.csproj", "CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileVerifier/"]
COPY ["CMA.ISMAI.Delivery.Logging.Log/CMA.ISMAI.Delivery.Logging.csproj", "CMA.ISMAI.Delivery.Logging.Log/"]
COPY ["CMA.ISMAI.Delivery.FileLoading.Domain/CMA.ISMAI.Delivery.FileLoading.Domain.csproj", "CMA.ISMAI.Delivery.FileLoading.Domain/"]
COPY ["CMA.ISMAI.Delivery.Core/CMA.ISMAI.Core.csproj", "CMA.ISMAI.Delivery.Core/"]
COPY ["CMA.ISMAI.Delivery.API.Domain/CMA.ISMAI.Delivery.API.Domain.csproj", "CMA.ISMAI.Delivery.API.Domain/"]
COPY ["CMA.ISMAI.Delivery.EventStore/CMA.ISMAI.Delivery.EventStore.csproj", "CMA.ISMAI.Delivery.EventStore/"]
COPY ["CMA.ISMAI.Delivery.FileLoading.Bus/CMA.ISMAI.Delivery.FileLoading.Bus.csproj", "CMA.ISMAI.Delivery.FileLoading.Bus/"]
COPY ["CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileIdentifier/CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileIdentifier.csproj", "CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileIdentifier/"]
COPY ["CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda/CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda.csproj", "CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda/"]
COPY ["CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileDownload/CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileDownload.csproj", "CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileDownload/"]
COPY ["CMA.ISMAI.Delivery.FileLoading.CrossCutting.Queue/CMA.ISMAI.Delivery.FileLoading.CrossCutting.Queue.csproj", "CMA.ISMAI.Delivery.FileLoading.CrossCutting.Queue/"]
RUN dotnet restore "CMA.ISMAI.Delivery.FileLoading.UI/CMA.ISMAI.Delivery.FileLoading.UI.csproj"
COPY . .
WORKDIR "/src/CMA.ISMAI.Delivery.FileLoading.UI"
RUN dotnet build "CMA.ISMAI.Delivery.FileLoading.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMA.ISMAI.Delivery.FileLoading.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMA.ISMAI.Delivery.FileLoading.UI.dll"]