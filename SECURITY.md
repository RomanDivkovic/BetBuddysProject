# 🔒 Security Setup Guide

## API Key Management

### Development Setup

1. **Copy the example environment file:**
   ```bash
   cp .env.example .env
   ```

2. **Add your actual API keys to `.env`:**
   ```bash
   # Get your MMA API key from https://rapidapi.com/api-sports/api/mma
   MMA_API_KEY=your_actual_api_key_here
   ENVIRONMENT=Development
   ```

3. **NEVER commit the `.env` file to Git!** 
   - The `.env` file is already in `.gitignore`
   - Only commit `.env.example` with placeholder values

### Production Deployment

For production deployments, set environment variables directly in your hosting platform:

#### Railway
```bash
railway variables set MMA_API_KEY=your_production_api_key
```

#### Heroku
```bash
heroku config:set MMA_API_KEY=your_production_api_key
```

#### Azure App Service
```bash
az webapp config appsettings set --name your-app-name --resource-group your-rg --settings MMA_API_KEY=your_production_api_key
```

#### Docker
```bash
docker run -e MMA_API_KEY=your_production_api_key myapp
```

#### Azure App Service
```bash
az webapp config appsettings set --name myapp --resource-group mygroup --settings MMA_API_KEY=din_riktiga_api_nyckel
```

## Setup för nya utvecklare

1. **Kopiera environment template:**
   ```bash
   cp .env.example .env
   ```

2. **Lägg till din API-nyckel i .env:**
   ```
   MMA_API_KEY=din_api_nyckel_här
   ```

3. **VIKTIGT:** Kontrollera att `.env` finns i `.gitignore`

## Varför detta är säkert?

✅ **API-nycklar checkas aldrig in i Git**  
✅ **Development och production är separerade**  
✅ **Environment variables i produktion**  
✅ **Fallback-hantering för konfiguration**  

## Kontroll av säkerhet

Innan du commitar, kontrollera:
```bash
# Dessa ska INTE finnas med hemliga nycklar:
git status
grep -r "70657fde93dac699f8ab6e4a8cea01d9" . --exclude-dir=.git
```
