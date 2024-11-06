#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebCoreTest/WebCoreTest.csproj", "WebCoreTest/"]
RUN dotnet restore "WebCoreTest/WebCoreTest.csproj"
COPY . .
WORKDIR "/src/WebCoreTest"
RUN dotnet build "WebCoreTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebCoreTest.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebCoreTest.dll"]