using Microsoft.EntityFrameworkCore;
using LogKeeper.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ВРЕМЕННО ОТКЛЮЧАЕМ базу данных для тестирования
// builder.Services.AddDbContext<LogContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавляем CORS для фронтенда
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://frontend:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ВРЕМЕННО ОТКЛЮЧАЕМ миграцию базы данных
// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<LogContext>();
//     context.Database.EnsureCreated();
// }

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