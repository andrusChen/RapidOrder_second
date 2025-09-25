using Microsoft.EntityFrameworkCore;
using RapidOrder.Infrastructure;
using RapidOrder.Api.Hubs;
using RapidOrder.Api.Services;
using Microsoft.OpenApi.Models;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Enums;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddSignalR();

// ✅ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RapidOrder API",
        Version = "v1"
    });
});

// ✅ EF Core
builder.Services.AddDbContext<RapidOrderDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=rapidorder.db"));

builder.Services.AddScoped<MissionAppService>(); 
builder.Services.AddSingleton<LearningModeService>();  // <- needed
builder.Services.AddSingleton<MissionNotifier>();  // <- SignalR wrapper

builder.Services.AddHostedService<SignalProcessorService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000") // your frontend URL
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// ✅ Swagger Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RapidOrder API v1");
        c.RoutePrefix = string.Empty; // ✅ opens Swagger at root "/"
    });
}



using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<RapidOrderDbContext>();
// Apply any pending EF Core migrations automatically on startup
db.Database.Migrate();







app.UseCors("AllowFrontend");

app.MapControllers();
app.MapHub<MissionHub>("/hubs/missions");

app.Run();
