#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BMC.IoTDashboard/BMC.IoTDashboard.csproj", "BMC.IoTDashboard/"]
RUN dotnet restore "BMC.IoTDashboard/BMC.IoTDashboard.csproj"
COPY . .
WORKDIR "/src/BMC.IoTDashboard"
RUN dotnet build "BMC.IoTDashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BMC.IoTDashboard.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BMC.IoTDashboard.dll"]