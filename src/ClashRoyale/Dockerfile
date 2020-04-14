FROM microsoft/dotnet:3.1-runtime-stretch-slim AS base
WORKDIR /app


FROM microsoft/dotnet:3.1-sdk-stretch AS build
WORKDIR /src
COPY ["ClashRoyale/ClashRoyale.csproj", "ClashRoyale/"]
RUN dotnet restore "ClashRoyale/ClashRoyale.csproj"
COPY . .
WORKDIR "/src/ClashRoyale"
RUN dotnet build "ClashRoyale.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ClashRoyale.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ClashRoyale.dll"]
