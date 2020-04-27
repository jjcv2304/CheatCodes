FROM mcr.microsoft.com/dotnet/core/sdk:2.1-alpine3.10 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore Api/Api.csproj
RUN dotnet publish Api/Api.csproj -o /publish

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1.17-alpine
WORKDIR /app
COPY --from=build-env /publish .
COPY --from=build-env /app/Api/appsettings.json .

ENV IDENTITY_SERVER_AUTHORITY=https://localhost:5002

ENTRYPOINT ["dotnet", "Api.dll"]

# From Solution folder
 # docker build -t cheatcodes-api -f .\Api\api.dockerfile .

# docker run --rm -it -p 44326:80 cheatcodes-api




