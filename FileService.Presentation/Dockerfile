FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FileService/FileService.Presentation/FileService.Presentation.csproj", "FileService/FileService.Presentation/"]

RUN dotnet restore "FileService/FileService.Presentation/FileService.Presentation.csproj"

COPY . .

WORKDIR "/src/FileService/FileService.Presentation"
RUN dotnet publish "FileService.Presentation.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FileService.Presentation.dll"]