# StacksOfChaos
SOCApi is a .NET 9 Web API for managing home libraries, books, users, and related data. It is part of the StacksOfChaos suite, which also includes web and mobile clients.

Features
RESTful API endpoints for books, users, authentication, and more
Entity Framework Core for data access and migrations
Identity for authentication and authorization
Swagger/OpenAPI documentation
Modular service and repository structure
Project Structure
Controllers – API endpoints for various resources
Models – Entity models for database tables
DTOs – Data transfer objects for API requests/responses
Services – Business logic and helper services
Data – Database context and migrations
Configuration – Strongly-typed configuration classes
Getting Started
Prerequisites
.NET 9 SDK
SQL Server (or use InMemory for development)
Setup
Clone the repository.
Update appsettings.json with your database connection string.
Run database migrations:

dotnet ef database update
Start the API:

dotnet run
API Documentation
Once running, navigate to /swagger for interactive API docs.

Contributing
Pull requests are welcome! Please open issues for suggestions or bugs.
