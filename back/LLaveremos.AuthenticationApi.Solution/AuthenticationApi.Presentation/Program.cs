using AuthenticationApi.Infrastructure.DependencyInjection;
using AuthenticationApi.Infrastructure.Data; // Para el contexto de datos
using Microsoft.EntityFrameworkCore; // Para Migrate()

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

// Aplicar migraciones pendientes
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
    dbContext.Database.Migrate(); // Esto asegura que las migraciones pendientes se apliquen automáticamente
}

app.UseInfrastructurePolicy();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
