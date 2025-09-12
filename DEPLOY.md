Supabase + Render deployment guide

Goal
- Host the Postgres DB on Supabase (free tier)
- Host the ASP.NET Core API on Render (or another host) and run EF migrations
- Keep Firebase client-side auth for now; when ready migrate frontend off Firebase or issue backend JWTs from Firebase tokens

Prerequisites
- Supabase account
- Render account (or other host with Docker/.NET support)
- Git repo accessible to Render (GitHub/GitLab)
- Environment variables stored in Render

Steps

1) Create Supabase project
- Go to https://app.supabase.com and create a free project
- Note the Postgres connection string (Settings → Database → Connection string). It will look like:
  postgres://<user>:<password>@<host>:5432/<db>
- In Supabase, create a "service_role" key if you need server-side admin operations (optional)

2) Set up database and run migrations
- On your local machine, ensure migrations are committed (we added migrations to the Migrations/ folder).
- You can run migrations locally against Supabase by running:

```bash
export ProductionConnection="<your_supabase_postgres_connection_string>"
dotnet ef database update --project /Users/romandivkovic/MyDotNetProject
```

- Alternatively, let the app run `context.Database.Migrate()` at startup in Render (recommended for early stages). Ensure `ProductionConnection` is set before the service starts.

3) Configure Render service
- Connect your Git repo to Render and create a new Web Service
- Build command: `dotnet build`
- Start command: `dotnet run --urls "http://0.0.0.0:5000"`
- Environment variables (add these in Render UI):
  - `ProductionConnection` = your Supabase Postgres connection string
  - `Jwt:Key` = a long secret (generate 32+ random chars)
  - `Jwt:Issuer` = your-app
  - `Jwt:Audience` = your-app-users
  - `ASPNETCORE_ENVIRONMENT` = Production

4) CORS and frontend
- On Render ensure your `ALLOWED_HOSTS` / CORS policy restricts allowed origins to your frontend domain when you go live.

5) Email and password resets
- If you rely on Firebase for auth in frontend, keep it until you fully migrate.
- For backend-based auth, configure an SMTP provider or transactional email service to send reset/verification emails.

Render-specific

1) Push your repo to GitHub and connect to Render
- In Render, create a new Web Service and select "Deploy from a GitHub repo".
- Choose the `render.yaml` in the repo; Render will configure the service from it.

2) Environment variables to set in Render (names match `appsettings` hierarchical keys):
- `ProductionConnection` = postgres connection string from Supabase
- `Jwt__Key` = secret key (note: use double-underscore to map to `Jwt:Key`)
- `Jwt__Issuer` = issuer
- `Jwt__Audience` = audience
- `ASPNETCORE_ENVIRONMENT` = Production

3) Verify startup & migrations
- Because `Program.cs` calls `Database.Migrate()`, the app will apply migrations automatically on start. Ensure `ProductionConnection` is set before the first deploy.

4) Testing after deploy
- After deployment, point your frontend to the Render URL.
- Test register/login to ensure JWTs are issued correctly and DB records are created in Supabase.

CI / Manual migration option
- If you prefer to run migrations explicitly in CI, add a step to your pipeline to run:

```bash
dotnet ef database update --project MyDotNetProject
```

- Or create a small one-off task on Render that runs the command with `ProductionConnection` present.

Notes
- The Dockerfile exposes port 8080 but the app is configured to listen on 5000 in the run command; Render will bind to the port the process listens on. We use `--urls "http://0.0.0.0:5000"` in `render.yaml` start command to ensure it binds correctly.
- I can also add health check endpoints and a `render.yaml` tweak to enable automatic health checks if you want.

Support
- I can create a Render YAML or `render.yaml` example and CI steps to run migrations during deploy.
- I can also implement an endpoint to exchange Firebase ID tokens for backend JWTs, if you want to keep the frontend untouched.
