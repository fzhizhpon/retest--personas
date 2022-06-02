FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim
WORKDIR /app

COPY ./publish/ /app
ENV TZ=America/Guayaquil
ENV ASPNETCORE_URLS=http://+:5010
ENV ASPNETCORE_ENVIRONMENT Development
EXPOSE 5010

ENTRYPOINT ["dotnet", "/app/Personas.Api.dll"]