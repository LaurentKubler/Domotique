From microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

COPY Domotique/Domotique.csproj ./Domotique/
COPY Messages/Messages.csproj ./Messages/

WORKDIR /app/Domotique
RUN dotnet restore

WORKDIR /app
COPY Domotique/. ./Domotique/
COPY Messages/. ./Messages/

WORKDIR /app/Domotique
RUN dotnet publish  -c Release -o out

#tests
#FROM build as testrunner
#WORKDIR /app/tests
#COPY tests/. .
#ENTRYPOINT ["dotnet", "test","--logger:trx"]

FROM microsoft/dotnet:2.1-runtime AS runtime
WORKDIR /app
COPY --from=build /app/Domotique/out ./

ENTRYPOINT ["dotnet", "Domotique.dll"]
