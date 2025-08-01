# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todos los archivos .csproj primero para aprovechar la caché de Docker
COPY LLaveremos.ProductApiSolution/ProductApi.Presentation/ProductApi.Presentation.csproj ProductApi.Presentation/
COPY LLaveremos.ProductApiSolution/ProductApi.Application/ProductApi.Application.csproj ProductApi.Application/
COPY LLaveremos.ProductApiSolution/ProductApi.Domain/ProductApi.Domain.csproj ProductApi.Domain/
COPY LLaveremos.ProductApiSolution/ProductApi.Infrastructure/ProductApi.Infrastructure.csproj ProductApi.Infrastructure/
COPY LLaveremos.SharedLibrarySolution/Llaveremos.SharedLibrary/Llaveremos.SharedLibrary.csproj Llaveremos.SharedLibrary/

# Restaurar dependencias
RUN dotnet restore ProductApi.Presentation/ProductApi.Presentation.csproj

# Copiar todo el código fuente (después de la restauración)
COPY LLaveremos.ProductApiSolution/ ./LLaveremos.ProductApiSolution/
COPY LLaveremos.SharedLibrarySolution/ ./LLaveremos.SharedLibrarySolution/

# Cambiar al directorio del proyecto principal
WORKDIR /src/LLaveremos.ProductApiSolution/ProductApi.Presentation

# Compilar y publicar la aplicación
RUN dotnet publish ProductApi.Presentation.csproj -c Release -o /app/publish

# Etapa final: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Configurar el punto de entrada
ENTRYPOINT ["dotnet", "ProductApi.Presentation.dll"]
