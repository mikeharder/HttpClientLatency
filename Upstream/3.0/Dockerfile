FROM microsoft/dotnet-nightly:3.0-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore -p:TargetFramework=netcoreapp3.0

# copy everything else and build app
COPY . .
RUN dotnet publish -c release -o out -p:TargetFramework=netcoreapp3.0

FROM microsoft/dotnet-nightly:3.0-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Upstream.dll"]
