# Database Migration Guide

## 1. Initial Migration (Local Development)
```bash
# I din projekt-folder:
cd /Users/romandivkovic/MyDotNetProject

# Installera EF Core CLI tools om du inte har det
dotnet tool install --global dotnet-ef

# Skapa initial migration
dotnet ef migrations add InitialCreate

# Applicera migration till SQLite (development)
dotnet ef database update
```

## 2. Production Migration
```bash
# Set environment to Production
export ASPNETCORE_ENVIRONMENT=Production

# Applicera migration till PostgreSQL
dotnet ef database update

# Eller specifiera connection string direkt:
dotnet ef database update --connection "postgresql://username:password@host:port/database"
```

## 3. Seed Data (Optional)
Om du vill ha initial data, skapa en seed metod:

```csharp
// I Program.cs, efter app.Build():
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BetBuddysDbContext>();
    
    // Kör endast i produktion och om databasen är tom
    if (!app.Environment.IsDevelopment() && !context.Users.Any())
    {
        // Lägg till initial data här
        context.Users.Add(new User 
        { 
            FirebaseUid = "admin", 
            Username = "admin", 
            Email = "admin@betbuddys.com",
            CreatedAt = DateTime.UtcNow 
        });
        
        await context.SaveChangesAsync();
    }
}
```

## 4. Backup Strategy
```bash
# Backup PostgreSQL database
pg_dump $DATABASE_URL > backup.sql

# Restore from backup
psql $DATABASE_URL < backup.sql
```

## 5. Environment-Specific Migrations
```bash
# Development (SQLite)
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update

# Production (PostgreSQL)
ASPNETCORE_ENVIRONMENT=Production dotnet ef database update
```
