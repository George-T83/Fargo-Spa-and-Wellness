FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Family and Spa Wellness.csproj", "./"]
RUN dotnet restore "Family and Spa Wellness.csproj"
COPY . .
RUN dotnet publish "Family and Spa Wellness.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Family_and_Spa_Wellness.dll"]
