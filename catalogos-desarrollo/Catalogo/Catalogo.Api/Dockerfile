FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim
WORKDIR /app

COPY ./publish/ /app
ENV ASPNETCORE_URLS=http://+:5003
ENV ASPNETCORE_ENVIRONMENT Development
EXPOSE 5003

ENTRYPOINT ["dotnet", "/app/Catalogo.Api.dll"]