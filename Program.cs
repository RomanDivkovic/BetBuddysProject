﻿using Microsoft.EntityFrameworkCore;
using MyDotNetProject.Data;
using MyDotNetProject.Services;

// Load environment variables from .env file FIRST (before creating builder)
if (File.Exists(".env"))
{
    foreach (var line in File.ReadAllLines(".env"))
    {
        var parts = line.Split('=', 2);
        if (parts.Length == 2 && !parts[0].StartsWith("#"))
        {
            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
if (builder.Environment.IsDevelopment())
{
    // Development: Use SQLite
    builder.Services.AddDbContext<BetBuddysDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    // Production: Use PostgreSQL (free tier on Railway/Supabase/Neon)
    builder.Services.AddDbContext<BetBuddysDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("ProductionConnection")));
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add custom services
builder.Services.AddHttpClient<IMmaApiService, MmaApiService>();
builder.Services.AddScoped<IBettingService, BettingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IPredictionService, PredictionService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BetBuddysDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
