#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CMA.ISMAI.Delivery.API.UI/CMA.ISMAI.Delivery.API.UI.csproj", "CMA.ISMAI.Delivery.API.UI/"]
COPY ["CMA.ISMAI.Delivery.API.CrossCutting.IoC/CMA.ISMAI.Delivery.API.CrossCutting.IoC.csproj", "CMA.ISMAI.Delivery.API.CrossCutting.IoC/"]
COPY ["CMA.ISMAI.Delivery.API.CrossCutting.Bus/CMA.ISMAI.Delivery.API.CrossCutting.Bus.csproj", "CMA.ISMAI.Delivery.API.CrossCutting.Bus/"]
COPY ["CMA.ISMAI.Delivery.API.CrossCutting.FileSaver/CMA.ISMAI.Delivery.API.CrossCutting.FileSaver.csproj", "CMA.ISMAI.Delivery.API.CrossCutting.FileSaver/"]
COPY ["CMA.ISMAI.Delivery.Logging.Log/CMA.ISMAI.Delivery.Logging.csproj", "CMA.ISMAI.Delivery.Logging.Log/"]
COPY ["CMA.ISMAI.Delivery.API.Domain/CMA.ISMAI.Delivery.API.Domain.csproj", "CMA.ISMAI.Delivery.API.Domain/"]
COPY ["CMA.ISMAI.Delivery.Core/CMA.ISMAI.Core.csproj", "CMA.ISMAI.Delivery.Core/"]
COPY ["CMA.ISMAI.Delivery.EventStore/CMA.ISMAI.Delivery.EventStore.csproj", "CMA.ISMAI.Delivery.EventStore/"]
COPY ["CMA.ISMAI.Delivery.API.CrossCutting.HttpRequest/CMA.ISMAI.Delivery.API.CrossCutting.HttpRequest.csproj", "CMA.ISMAI.Delivery.API.CrossCutting.HttpRequest/"]
COPY ["CMA.ISMAI.Delivery.API.CrossCutting.Queue/CMA.ISMAI.Delivery.API.CrossCutting.Queue.csproj", "CMA.ISMAI.Delivery.API.CrossCutting.Queue/"]
COPY ["CMA.ISMAI.Delivery.API.CrossCutting.Zip/CMA.ISMAI.Delivery.API.CrossCutting.Zip.csproj", "CMA.ISMAI.Delivery.API.CrossCutting.Zip/"]
RUN dotnet restore "CMA.ISMAI.Delivery.API.UI/CMA.ISMAI.Delivery.API.UI.csproj"
COPY . .
WORKDIR "/src/CMA.ISMAI.Delivery.API.UI"
RUN dotnet build "CMA.ISMAI.Delivery.API.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMA.ISMAI.Delivery.API.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMA.ISMAI.Delivery.API.UI.dll"]