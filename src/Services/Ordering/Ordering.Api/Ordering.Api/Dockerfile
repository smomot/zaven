#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Services/Ordering/Ordering.Api/Ordering.Api/Ordering.Api.csproj", "src/Services/Ordering/Ordering.Api/Ordering.Api/"]
RUN dotnet restore "src/Services/Ordering/Ordering.Api/Ordering.Api/Ordering.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Ordering/Ordering.Api/Ordering.Api"
RUN dotnet build "Ordering.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ordering.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ordering.Api.dll"]