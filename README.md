# StacksOfChaos
This is an application for managing home libraries using Angular, .Net Core, .Net Maui and MS SQL Server. Here are the projects that make up this application. 
1. Name: SOCApi Type: .NET 9 Web API
 a. Purpose: Provides RESTful API endpoints for managing data related to spots and titles.
 b. Key Features:
 -- Implements controllers for handling HTTP requests.
 -- Uses Swagger for API documentation and testing.
2. Name: SOCData: Type: Data Access Layer (DAL) MSSQL
 a. Purpose: Manages database interactions and data access logic.
 b. Key Features:
 -- Contains entity models representing database tables.
 -- Implements repository patterns for data operations.
 -- Manages database context and migrations.
3. Name: SOCWeb: Type: Angular version 19 Web Application
 a. Purpose: Provides a web-based user interface for interacting with the API.
 b. Key Features:
 -- Built with Angular framework.
 -- Implements components and services for data retrieval and manipulation.
 -- Provides user authentication and authorization.
4. Name: SOCMobile: Type: .NET MAUI Application
 a. Purpose: Provides a cross-platform mobile application for interacting with the API.
 b. Key Features:
 -- Implements a user-friendly mobile interface.
 -- Integrates with the SOCApi for data retrieval and manipulation.
 -- Supports multiple platforms (iOS, Android, Windows).
