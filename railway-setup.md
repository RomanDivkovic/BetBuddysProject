# Render Setup Guide (Rekommenderad - Gratis!)

## Varför Render?
- ✅ **750 timmar/månad gratis** (mer än nog för 24/7 drift)
- ✅ **Automatisk SSL** och custom domains
- ✅ **Ingen kreditkort behövs** för gratis tier
- ✅ **Bra prestanda** och enkelt att använda
- ✅ **Automatisk deploy** från GitHub

## 1. Skapa Render Account
- Gå till [render.com](https://render.com)
- Registrera dig med GitHub (gratis)

## 2. Deploy Backend
```bash
# I Render dashboard:
1. Klicka "New +" → "Web Service"
2. Anslut ditt GitHub repo
3. Välj "MyDotNetProject" repository
4. Konfigurera:
   - Name: betbuddys-backend
   - Environment: Docker
   - Region: Oregon (närmast Europa)
   - Branch: main
```

## 3. Konfiguration
```bash
# Render kommer automatiskt att:
- Använda din Dockerfile
- Bygga och deploya din app
- Ge dig en gratis .onrender.com URL
```

## 4. Environment Variables
I Render dashboard, lägg till:
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__ProductionConnection=postgresql://user:pass@host:port/db
MMA_API_KEY=your_api_key_here
ASPNETCORE_URLS=http://0.0.0.0:10000
```

## 5. Databas (Neon - Gratis PostgreSQL)
```bash
# Gå till neon.tech:
1. Skapa gratis konto
2. Skapa ny databas "betbuddys"
3. Kopiera connection string
4. Klistra in i Render environment variables
```

## 6. Automatisk Deploy
```bash
# Render deployar automatiskt när du:
- Pushar till main branch
- Inga extra steg behövs!
```

## Fördelar med Render vs Railway:
| Feature | Render | Railway |
|---------|---------|---------|
| Gratis tier | 750h/månad | $5 kredit |
| Kreditkort | Inte nödvändigt | Krävs |
| SSL | Automatisk | Automatisk |
| Custom domains | Gratis | Gratis |
| Prestanda | Bra | Bra |
| Deployment | GitHub auto | GitHub auto |

## Kostnad:
- **Render**: 750 timmar/månad gratis (≈ 24/7)  
- **Neon DB**: 3GB gratis för alltid
- **Total**: 0 SEK/månad 💰
