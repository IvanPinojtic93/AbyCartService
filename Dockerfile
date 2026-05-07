FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/CartService.API/CartService.API.csproj",                         "src/CartService.API/"]
COPY ["src/CartService.Application/CartService.Application.csproj",         "src/CartService.Application/"]
COPY ["src/CartService.Domain/CartService.Domain.csproj",                   "src/CartService.Domain/"]
COPY ["src/CartService.Infrastructure/CartService.Infrastructure.csproj",   "src/CartService.Infrastructure/"]
RUN dotnet restore "src/CartService.API/CartService.API.csproj"
COPY . .
RUN dotnet build "src/CartService.API/CartService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/CartService.API/CartService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CartService.API.dll"]
