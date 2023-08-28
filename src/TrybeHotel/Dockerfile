# estágio de compilação
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app-backend

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

#estágio de Empacotamento
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app-backend
COPY --from=build-env /app-backend/out .
ENTRYPOINT ["dotnet", "TrybeHotel.dll"]
