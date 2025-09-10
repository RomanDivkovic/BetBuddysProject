# 🚀 BetBuddys Backend Deployment Guide

## Quick Start Options (Free Tier)

### 🏆 Option 1: Railway (Recommended)
**Cost:** $5/month free credit (covers small apps)
**Setup Time:** 5-10 minutes

1. **Create Railway Account**
   ```bash
   # Go to railway.app and sign up with GitHub
   ```

2. **Deploy from GitHub**
   ```bash
   # In Railway dashboard:
   1. "New Project" → "Deploy from GitHub repo"
   2. Select your BetBuddys repository
   3. Railway will auto-detect .NET and deploy
   ```

3. **Add PostgreSQL Database**
   ```bash
   # In your Railway project:
   1. "New Service" → "Database" → "PostgreSQL"
   2. Copy the connection string from Variables tab
   ```

4. **Set Environment Variables**
   ```bash
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__ProductionConnection=postgresql://user:pass@host:port/db
   MMA_API_KEY=your_mma_api_key_here
   ```

### 🆓 Option 2: Render + Neon (100% Free)
**Cost:** Completely free
**Setup Time:** 10-15 minutes

#### Step A: Setup Database (Neon)
1. Go to [neon.tech](https://neon.tech)
2. Sign up with GitHub
3. Create project "betbuddys"
4. Copy connection string

#### Step B: Deploy Backend (Render)
1. Go to [render.com](https://render.com)
2. Sign up with GitHub
3. "New" → "Web Service"
4. Connect your GitHub repo
5. Configure:
   ```
   Name: betbuddys-backend
   Environment: Docker
   Build Command: dotnet publish -c Release -o /app
   Start Command: dotnet MyDotNetProject.dll
   ```

### 🔧 Option 3: Fly.io (Advanced)
**Cost:** Free tier available
**Setup Time:** 15-20 minutes
- More control but requires Dockerfile setup

## Environment Variables Needed

For any platform, you'll need these environment variables:

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__ProductionConnection=your_postgres_connection_string
MMA_API_KEY=your_mma_api_key
```

## Pre-Deployment Checklist

- [ ] Remove API keys from appsettings.json files
- [ ] Test build locally: `dotnet build`
- [ ] Test database migrations work
- [ ] Commit and push to GitHub
- [ ] Have your MMA API key ready

## Testing Your Deployment

Once deployed, test these endpoints:
```bash
# Health check
GET https://your-app.railway.app/api/users

# Swagger docs
GET https://your-app.railway.app/swagger

# MMA API integration
GET https://your-app.railway.app/api/mma/fights?date=2025-07-15
```

## Database Migration

Your app will automatically run migrations on startup thanks to this code in Program.cs:
```csharp
// Database migration happens automatically
```

## Next Steps After Deployment

1. Test all API endpoints
2. Update your frontend to use the new backend URL
3. Set up custom domain (optional)
4. Monitor usage and performance

## Troubleshooting

**Build Fails?**
- Check .NET version compatibility
- Ensure all packages are restored

**Database Connection Issues?**
- Verify connection string format
- Check if database allows external connections
- Ensure SSL is properly configured

**API Key Issues?**
- Make sure MMA_API_KEY environment variable is set
- Test API key directly with MMA API

## Cost Estimates

| Platform | Free Tier | Paid Start |
|----------|-----------|------------|
| Railway | $5 credit/month | $5/month |
| Render | 750 hours/month | $7/month |
| Neon DB | 3GB forever | $19/month |
| Supabase | 500MB x2 projects | $25/month |

**Recommendation:** Start with Railway or Render + Neon for free tier, then upgrade as needed.
