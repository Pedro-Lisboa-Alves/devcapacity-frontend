FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5084

ENV ASPNETCORE_URLS=http://+:5084

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["DevCapacityWebApp/DevCapacityWebApp.csproj", "DevCapacityWebApp/"]
RUN dotnet restore "DevCapacityWebApp/DevCapacityWebApp.csproj"
COPY . .
WORKDIR "/src/DevCapacityWebApp"
RUN dotnet build "DevCapacityWebApp.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "DevCapacityWebApp.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevCapacityWebApp.dll"]
