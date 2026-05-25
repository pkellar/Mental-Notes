# Mental Notes

Mental Notes is a Blazor Server web application (Interactive Server render mode) for a podcast and companion site. It uses .NET 9, Entity Framework Core and SQL Server for data persistence.

## Key components
- `Mental-Notes` — main Blazor application (UI, pages, migrations)
- `SharedData` — shared models and DTOs used by the app

## Prerequisites
- .NET 9 SDK
- SQL Server for production (or LocalDB for local development)
- Optional: `dotnet-ef` tools for applying migrations

## Quick start (local development)
1. Clone the repository.
2. Open a terminal and set required environment variables for local development instead of committing secrets:
   - `ConnectionStrings__DefaultConnection` — a SQL Server connection string (or leave empty and use LocalDB)
   - `RESEND_APITOKEN` — (optional) API token used by the Resend email client
   You can use `dotnet user-secrets` while developing locally instead of environment variables.

3. Apply EF Core migrations (run from solution or project folder):

   ```dotnet ef database update --project Mental-Notes```

4. Run the application from the `Mental-Notes` project folder:

   ```dotnet run --project Mental-Notes```

5. Open the app in your browser at the URL shown by the run command (typically `https://localhost:<port>`).

## Security and repository notes
- Do NOT commit secrets (connection strings, API keys, certificates or publish profiles).
- The repository's `.gitignore` is configured to ignore development `appsettings*.Development.json`, publish profiles and the entire `AdminApp` folder. Keep secrets in environment variables, `dotnet user-secrets` or a secrets manager (Azure Key Vault, GitHub Secrets).
- If sensitive data was accidentally committed, rotate the exposed credentials immediately and scrub history (BFG or `git filter-repo`).

## License
- Add a license to this repository (e.g. MIT) if you want to allow reuse.