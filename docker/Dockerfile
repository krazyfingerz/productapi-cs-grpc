# BUILD STAGE
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution + project files
COPY ../ProductAPI/ProductAPI.csproj ProductAPI/
COPY ../ProductAPI/Protos ProductAPI/Protos/

RUN dotnet restore ProductAPI/ProductAPI.csproj

# Copy full source
COPY ../ProductAPI ProductAPI/

WORKDIR /src/ProductAPI

RUN dotnet publish -c Release -o /app/publish

# RUNTIME STAGE
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

# gRPC requires HTTP/2
ENV ASPNETCORE_URLS=http://+:5000

EXPOSE 5000

ENTRYPOINT ["dotnet", "ProductAPI.dll"]