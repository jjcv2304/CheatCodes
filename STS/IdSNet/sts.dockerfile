FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201-alpine3.11 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore STS/IdSNet/IdSNet.csproj
RUN dotnet publish STS/IdSNet/IdSNet.csproj -o /publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.3-alpine3.11
WORKDIR /app
COPY --from=build-env /publish .
#COPY --from=build-env /app/STS/IdSNet/appsettings.json .
ENTRYPOINT ["dotnet", "IdSNet.dll"]

# From Solution folder
 # docker build -t cheatcodes-sts -f .\STS\IdSNet\sts.dockerfile .

# docker run --rm -it -p 5002:80 cheatcodes-sts




