FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PosMonitor.csproj", "."]
RUN dotnet restore "PosMonitor.csproj"
COPY . .
RUN dotnet build "PosMonitor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PosMonitor.csproj" -c Release -o /app/publish

EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PosMonitor.dll", "--environment=Development"]
