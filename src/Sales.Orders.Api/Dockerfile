FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Sales.Orders.Api/Sales.Orders.Api.csproj", "Sales.Orders.Api/"]
COPY ["Sales.Orders.Application/Sales.Orders.Application.csproj", "Sales.Orders.Application/"]
COPY ["Sales.Orders.Domain/Sales.Orders.Domain.csproj", "Sales.Orders.Domain/"]
COPY ["Sales.Orders.Infrastructure/Sales.Orders.Infrastructure.csproj", "Sales.Orders.Infrastructure/"]
RUN dotnet restore "Sales.Orders.Api/Sales.Orders.Api.csproj"

COPY . .
WORKDIR /src/Sales.Orders.Api
RUN dotnet publish "Sales.Orders.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sales.Orders.Api.dll"]
