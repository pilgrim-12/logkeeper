using Microsoft.EntityFrameworkCore;
using LogKeeper.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавляем Entity Framework
builder.Services.AddDbContext<LogContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавляем CORS для фронтенда
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Автоматическая миграция базы данных при запуске
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<LogContext>();

        // Ждем доступности базы данных
        var retryCount = 0;
        while (retryCount < 30)
        {
            try
            {
                context.Database.EnsureCreated();
                Console.WriteLine("Database created successfully!");
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                Console.WriteLine($"Database not ready, retrying... ({retryCount}/30)");
                Console.WriteLine($"Error: {ex.Message}");
                Thread.Sleep(2000);
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Database initialization failed: {ex.Message}");
}

// Configure the HTTP request pipeline.
// Включаем Swagger для тестирования
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

// Добавляем простой health check
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });

app.Run();