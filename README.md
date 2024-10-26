**How to run migration:
- cd /src
- dotnet ef migrations add <migration-name> --startup-project ./Presentation --project ./Infrastructure --output-dir ../Infrastructure/Persistence/Migrations
- dotnet ef update database --startup-project ./Presentation --project ./Infrastructure

**Database connection:
- This project is using MSSQL, if you want to use another database, you can add/change it in "/Infrastructure/Configurations/ApplicationDBContextConfigurations".
- Note:
  - Some entity attributes might be different from multiple databases, so you must change it to fit with your database.
  - Change your DefaultConnectionString in appsettings (/Presentation/appsettings*.json) to fit with your database.
  - This project might be not compatible with NoSQL.

**Logging:
- Logging is configured in "/Infrastructure/Configurations/LoggingConfiguration". I'm using Seq for storing and monitoring logs, you must run docker image for Seq (I configured it in docker-compose.yml file), or set up your logging service your self in LoggingConfiguration.

**Docker Compose:
- The docker-compose file are just used for demo. I set up a MSSQL and Seq image. You should implement yourself to fit with your project. To run the docker-compose file:
  - cd /src
  - docker-compose -f docker-compose.yml up -d 
