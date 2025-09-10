# 🚀 Render Deployment Guide - Steg för Steg

## 🎯 Snabb Start (5 minuter)

### Steg 1: Förbered ditt projekt
```bash
# Kontrollera att allt fungerar lokalt
cd /Users/romandivkovic/MyDotNetProject
dotnet build
dotnet run
```

### Steg 2: Pusha till GitHub
```bash
git add .
git commit -m "Ready for deployment"
git push origin main
```

### Steg 3: Skapa Render konto
1. Gå till [render.com](https://render.com)
2. Klicka "Get Started for Free"
3. Registrera med GitHub

### Steg 4: Deploy Backend
1. **I Render dashboard:**
   - Klicka "New +" knappen
   - Välj "Web Service"
   - Klicka "Connect" vid ditt GitHub repo
   - Välj "MyDotNetProject" repository

2. **Konfiguration:**
   ```
   Name: betbuddys-backend
   Environment: Docker
   Region: Oregon
   Branch: main
   ```

3. **Render kommer automatiskt:**
   - Hitta din Dockerfile
   - Bygga din app
   - Deploya den
   - Ge dig en URL: `https://betbuddys-backend.onrender.com`

### Steg 5: Lägg till Environment Variables
I Render dashboard → din service → Environment:
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:10000
MMA_API_KEY=din_api_nyckel_här
```

### Steg 6: Skapa gratis databas (Neon)
1. Gå till [neon.tech](https://neon.tech)
2. Registrera med GitHub
3. Skapa projekt "betbuddys"
4. Kopiera connection string
5. Lägg till i Render environment variables:
   ```
   ConnectionStrings__ProductionConnection=postgresql://user:pass@host:port/db
   ```

## 🎉 Klart!
Din app kommer att vara live på: `https://betbuddys-backend.onrender.com`

## 📋 Checklista
- [ ] Projekt bygger lokalt
- [ ] Pushat till GitHub
- [ ] Render konto skapat
- [ ] Web service konfigurerad
- [ ] Environment variables inställda
- [ ] Databas skapad på Neon
- [ ] Connection string konfigurerad
- [ ] App deployad och fungerande

## 🔍 Testa din deployment
```bash
# Testa att API:t fungerar
curl https://betbuddys-backend.onrender.com/api/users

# Kolla Swagger docs
# Gå till: https://betbuddys-backend.onrender.com/swagger
```

## 💡 Tips
1. **Första deployment** tar 5-10 minuter
2. **Automatisk deployment** sker vid varje push till main
3. **Logs** finns i Render dashboard
4. **Custom domain** kan läggas till gratis senare
5. **SSL** är automatiskt aktiverat

## 🔧 Troubleshooting
**Build misslyckas?**
- Kolla logs i Render dashboard
- Kontrollera att Dockerfile finns
- Verifiera att projektet bygger lokalt

**App startar inte?**
- Kontrollera environment variables
- Kolla att ASPNETCORE_URLS är satt till http://0.0.0.0:10000
- Verifiera databas connection string

**Databas connection fel?**
- Kontrollera connection string från Neon
- Säkerställ att SSL är aktiverat i connection string
- Testa connection string lokalt först
