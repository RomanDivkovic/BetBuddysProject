# Supabase Setup Guide

## 1. Create Account
- Gå till [supabase.com](https://supabase.com)
- Registrera dig med GitHub

## 2. Create Project
```bash
1. New Project
2. Välj organization
3. Namnge projektet "betbuddys-backend"
4. Generera lösenord
5. Välj region (Europe för bästa prestanda)
```

## 3. Get Connection String
I Supabase dashboard:
```
Settings → Database → Connection string → URI
```

## 4. Connection String Format
```
postgresql://postgres:[YOUR-PASSWORD]@db.[PROJECT-REF].supabase.co:5432/postgres
```

## 5. Environment Variables
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__ProductionConnection=postgresql://postgres:password@db.projectref.supabase.co:5432/postgres
MMA_API_KEY=your_api_key_here
```

## 6. Additional Features
Supabase erbjuder också:
- Row Level Security (RLS)
- Real-time subscriptions
- Edge Functions
- Auth (om du vill migrera från Firebase Auth senare)
