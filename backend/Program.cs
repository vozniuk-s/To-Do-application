using backend.Data;
using backend.Interfaces;
using backend.Middlewares;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using Microsoft.AspNetCore.HttpLogging;

var logger = NLog.LogManager.GetCurrentClassLogger();
logger.Info("Launching");

var builder = WebApplication.CreateBuilder(args);

// Nlog settings
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// SQlite
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite("Data Source=appbase.db");
});

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseStatusCode |
                            HttpLoggingFields.Duration;
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

logger.Info("Launched");

app.Run();