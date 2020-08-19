#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CMA.ISMAI.Delivery.Payment.UI/CMA.ISMAI.Delivery.Payment.UI.csproj", "CMA.ISMAI.Delivery.Payment.UI/"]
COPY ["CMA.ISMAI.Delivery.Payment.CrossCutting.FileReader/CMA.ISMAI.Delivery.Payment.CrossCutting.FileReader.csproj", "CMA.ISMAI.Delivery.Payment.CrossCutting.FileReader/"]
COPY ["CMA.ISMAI.Delivery.Logging.Log/CMA.ISMAI.Delivery.Logging.csproj", "CMA.ISMAI.Delivery.Logging.Log/"]
COPY ["CMA.ISMAI.Delivery.Payment.Domain/CMA.ISMAI.Delivery.Payment.Domain.csproj", "CMA.ISMAI.Delivery.Payment.Domain/"]
COPY ["CMA.ISMAI.Delivery.Core/CMA.ISMAI.Core.csproj", "CMA.ISMAI.Delivery.Core/"]
COPY ["CMA.ISMAI.Delivery.EventStore/CMA.ISMAI.Delivery.EventStore.csproj", "CMA.ISMAI.Delivery.EventStore/"]
COPY ["CMA.ISMAI.Delivery.Payment.CrossCutting.Bis/CMA.ISMAI.Delivery.Payment.CrossCutting.Bus.csproj", "CMA.ISMAI.Delivery.Payment.CrossCutting.Bis/"]
COPY ["CMA.ISMAI.Delivery.Payment.CrossCutting.Camunda/CMA.ISMAI.Delivery.Payment.CrossCutting.Camunda.csproj", "CMA.ISMAI.Delivery.Payment.CrossCutting.Camunda/"]
COPY ["CMA.ISMAI.Delivery.Payment.CrossCutting.Queue/CMA.ISMAI.Delivery.Payment.CrossCutting.Queue.csproj", "CMA.ISMAI.Delivery.Payment.CrossCutting.Queue/"]
RUN dotnet restore "CMA.ISMAI.Delivery.Payment.UI/CMA.ISMAI.Delivery.Payment.UI.csproj"
COPY . .
WORKDIR "/src/CMA.ISMAI.Delivery.Payment.UI"
RUN dotnet build "CMA.ISMAI.Delivery.Payment.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMA.ISMAI.Delivery.Payment.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMA.ISMAI.Delivery.Payment.UI.dll"]
