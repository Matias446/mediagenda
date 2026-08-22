FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY mediAgenda.Dominio/mediAgenda.Dominio.csproj mediAgenda.Dominio/
COPY mediAgenda.IDataAccess/mediAgenda.IDataAccess.csproj mediAgenda.IDataAccess/
COPY mediAgenda.DataAccess/mediAgenda.DataAccess.csproj mediAgenda.DataAccess/
COPY mediAgenda.ILogicaNegocio/mediAgenda.ILogicaNegocio.csproj mediAgenda.ILogicaNegocio/
COPY mediAgenda.LogicaNegocio/mediAgenda.LogicaNegocio.csproj mediAgenda.LogicaNegocio/
COPY mediAgenda.WebAPI/mediAgenda.WebAPI.csproj mediAgenda.WebAPI/

RUN dotnet restore mediAgenda.WebAPI/mediAgenda.WebAPI.csproj

COPY . .

RUN dotnet publish mediAgenda.WebAPI/mediAgenda.WebAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "mediAgenda.WebAPI.dll"]