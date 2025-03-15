FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SOCApi.csproj", "./"]
RUN dotnet restore "SOCApi.csproj"
COPY . .
RUN dotnet build "SOCApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SOCApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_URLS=http://+:80;https://+:443
ENV ASPNETCORE_ENVIRONMENT=Production

COPY ["certs/SOCApi.pfx", "certs/SOCApi.pfx"]
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/certs/socapi.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=123456

ENTRYPOINT ["dotnet", "SOCApi.dll"]