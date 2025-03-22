📋 Order Management API
A RESTful API for managing orders, built with ASP.NET Core 3.1 and MySQL.

🚀 Setup Instructions
Follow these steps to set up the project on your local machine.
Netcore 3.1 & MySQL
1. Clone the Repository
Clone the project from GitHub to your local machine:

git clone https://github.com/thanhtuanlk11/order-management-api
cd order-management-api

2. Install Required Packages
The project uses several NuGet packages. Install them using the dotnet add package command. Run the following commands in the terminal from the root directory of the project (order-management-api):

📦 ASP.NET Core and Entity Framework Core for MySQL:


3. Configure MySQL Connection
Update the MySQL connection string in the WebApplication1/appsettings.json file:

4. Run Migrations
Create and apply database migrations to set up the database schema:
dotnet ef migrations add InitialCreate --project WebApplication1/WebApplication1.csproj
dotnet ef database update --project WebApplication1/WebApplication1.csproj

5. Run the Application
Start the application:
dotnet run --project WebApplication1/WebApplication1.csproj

6. Access the API
Once the application is running, you can access the API:

🌐 API Base URL: http://localhost:5001/
📖 Swagger UI: http://localhost:5001/swagger
✨ Features
🛠️ CRUD operations for Orders and Order Details
✅ Input validation with FluentValidation
🗺️ DTO mapping with AutoMapper
📖 API documentation with Swagger
📝 Notes