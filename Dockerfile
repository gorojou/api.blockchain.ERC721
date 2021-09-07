FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /my.gallery-api-blockchain
COPY ["WebApiMyGalleryPolygon.csproj", "./"]
RUN dotnet restore "WebApiMyGalleryPolygon.csproj" --disable-parallel
COPY . .
WORKDIR "/my.gallery-api-blockchain"
RUN dotnet build "WebApiMyGalleryPolygon.csproj" -c Release -o /app
RUN dotnet publish "WebApiMyGalleryPolygon.csproj" -c Release -o /app

FROM build AS final
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "WebApiMyGalleryPolygon.dll"]
ENV ASPNETCORE_URLS=http://0.0.0.0:5003