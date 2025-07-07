# Neon Setup Guide

## 1. Create Account
- Gå till [neon.tech](https://neon.tech)
- Registrera dig med GitHub

## 2. Create Database
```bash
1. Create your first project
2. Namnge databasen "betbuddys"
3. Välj region
4. Skapa projekt
```

## 3. Connection Details
I Neon dashboard:
```
Dashboard → Connection Details → Copy connection string
```

## 4. Connection String Format
```
postgresql://username:password@ep-name.region.neon.tech/database?sslmode=require
```

## 5. Environment Variables
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__ProductionConnection=postgresql://username:password@ep-name.region.neon.tech/database?sslmode=require
MMA_API_KEY=your_api_key_here
```

## 6. Fördelar
- Serverless (betalar endast för användning)
- Branching (olika versioner av din databas)
- Autoscaling
- Point-in-time recovery
