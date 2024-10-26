**How to run migration:
- cd /src
- dotnet ef migrations add <migration-name> --startup-project ./Presentation --project ./Infrastructure --output-dir ../Infrastructure/Persistence/Migrations
- dotnet ef update database --startup-project ./Presentation --project ./Infrastructure
