FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# copy only the project first for cache
COPY ["LibraryApi.csproj", "./"]
RUN dotnet restore "./LibraryApi.csproj"
# now copy the rest
COPY . .
RUN dotnet publish "LibraryApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LibraryApi.dll"]
