FROM mcr.microsoft.com/dotnet/sdk:8.0 AS installer-env

#ARG SOLUTION_DIR=.

COPY .. /src/dotnet-function-app
RUN cd /src/dotnet-function-app && \
mkdir -p /home/site/wwwroot && \
dotnet publish FancyToDo.Functions/FancyToDo.Functions.csproj --output /home/site/wwwroot

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
    
#RUN apt install -y curl
    

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]

#CMD ["curl --insecure https://cosmosDbEmulator:8081/_explorer/emulator.pem > ~/emulatorcert.crt"]

