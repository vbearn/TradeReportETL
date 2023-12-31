#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Api/TradeReportETL.Pipeline.Extract/TradeReportETL.Pipeline.Extract.csproj", "Api/TradeReportETL.Pipeline.Extract/"]
COPY ["Common/TradeReportETL.Common/TradeReportETL.Common.csproj", "Common/TradeReportETL.Common/"]
COPY ["Services/TradeReportETL.Pipeline/TradeReportETL.Pipeline.csproj", "Services/TradeReportETL.Pipeline/"]
COPY ["Services/TradeReportETL.Shared/TradeReportETL.Shared.csproj", "Services/TradeReportETL.Shared/"]
RUN dotnet restore "Api/TradeReportETL.Pipeline.Extract/TradeReportETL.Pipeline.Extract.csproj"
COPY . .
WORKDIR "/src/Api/TradeReportETL.Pipeline.Extract"
RUN dotnet build "TradeReportETL.Pipeline.Extract.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TradeReportETL.Pipeline.Extract.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TradeReportETL.Pipeline.Extract.dll"]