FROM microsoft/dotnet-nightly:2.2-sdk-stretch AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore -p:TargetFramework=netcoreapp2.2

# copy everything else and build app
COPY . .
RUN dotnet publish -c release -o out -p:TargetFramework=netcoreapp2.2

FROM microsoft/dotnet-nightly:2.2-aspnetcore-runtime-stretch-slim AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Upstream.dll"]
