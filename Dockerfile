#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VUCE3.Catalogos.Presentacion/VUCE3.Catalogos.Presentacion.csproj", "VUCE3.Catalogos.Presentacion/"]
COPY ["VUCE3.Catalogos.Infraestructura/VUCE3.Catalogos.Infraestructura.csproj", "VUCE3.Catalogos.Infraestructura/"]
COPY ["VUCE3.Catalogos.Aplicacion/VUCE3.Catalogos.Aplicacion.csproj", "VUCE3.Catalogos.Aplicacion/"]
COPY ["VUCE3.Catalogos.Dominio/VUCE3.Catalogos.Dominio.csproj", "VUCE3.Catalogos.Dominio/"]
RUN dotnet restore "./VUCE3.Catalogos.Presentacion/./VUCE3.Catalogos.Presentacion.csproj"
COPY . .
WORKDIR "/src/VUCE3.Catalogos.Presentacion"
RUN dotnet build "./VUCE3.Catalogos.Presentacion.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VUCE3.Catalogos.Presentacion.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VUCE3.Catalogos.Presentacion.dll"]