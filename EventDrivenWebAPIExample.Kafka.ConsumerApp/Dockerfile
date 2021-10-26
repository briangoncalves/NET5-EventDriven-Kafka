#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["EventDrivenWebAPIExample.Kafka.ConsumerApp/EventDrivenWebAPIExample.Kafka.ConsumerApp.csproj", "EventDrivenWebAPIExample.Kafka.ConsumerApp/"]
RUN dotnet restore "EventDrivenWebAPIExample.Kafka.ConsumerApp/EventDrivenWebAPIExample.Kafka.ConsumerApp.csproj"
COPY . .
WORKDIR "/src/EventDrivenWebAPIExample.Kafka.ConsumerApp"
RUN dotnet build "EventDrivenWebAPIExample.Kafka.ConsumerApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventDrivenWebAPIExample.Kafka.ConsumerApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventDrivenWebAPIExample.Kafka.ConsumerApp.dll"]