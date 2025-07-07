# Railway Setup Guide

## 1. Create Account
- Gå till [railway.app](https://railway.app)
- Registrera dig med GitHub

## 2. Create PostgreSQL Database
```bash
# I Railway dashboard:
1. New Project → PostgreSQL
2. Kopiera connection string från Variables tab
```

## 3. Environment Variables
I Railway, lägg till följande environment variables:
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__ProductionConnection=postgresql://user:password@host:port/database
MMA_API_KEY=your_api_key_here
```

## 4. Deploy
```bash
# Railway CLI (optional)
npm install -g @railway/cli
railway login
railway link
railway up
```

## 5. Connection String Format
```
postgresql://username:password@hostname:port/database
```

Exempel:
```
postgresql://postgres:password123@containers-us-west-1.railway.app:5432/railway
```
