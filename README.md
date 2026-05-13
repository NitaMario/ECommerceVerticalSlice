# E-Commerce Platform

A full-stack e-commerce application demonstrating a vertical slice architecture. This project features .NET Web API backend utilizing raw ADO.NET and a reactive Angular frontend utilizing standalone components and Signals for state management.

##  Technology Stack
*   **Backend:** .NET Core Web API (C#), MediatR (CQRS pattern)
*   **Database:** Microsoft SQL Server (Raw ADO.NET)
*   **Frontend:** Angular (TypeScript, Standalone Components, Signals)
*   **Testing:** xUnit, Moq, FluentAssertions (C#) & Jasmine, Karma (Angular)

---

##  Getting Started

Follow these instructions to get a copy of the project running on your local machine for development and testing purposes.

### Prerequisites
*   [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) & [SSMS](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
*   [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download) 
*   [Node.js](https://nodejs.org/)
*   [Angular CLI](https://angular.io/cli) (`npm install -g @angular/cli`)

### 1. Database Setup
1. Open SQL Server Management Studio (SSMS).
2. Right-click the **Databases** node and select **Restore Database...**
3. Select **Device** and browse to the `Database/ECommerceSliceDB.bak` file included in this repository.
4. Click **OK** to restore the database. It comes pre-populated with sample products and testing data.

### 2. Backend Setup (.NET Web API)
1. Open a terminal and navigate to the backend folder:
   ```bash
   cd Backend/ECommerce.API
   ```
2. Open the **appsettings.json** file and update the **DefaultConnection** string to point to your local SQL Server instance:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ECommerceSliceDB;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```
3. Restore the NuGet packages and run the API:
   ```bash
   dotnet restore
   dotnet run
   ```

*The API will start on `https://localhost:7119`. Leave this terminal open.*

### 3. Frontend Setup (Angular)
1. Open a new terminal and navigate to the frontend folder:
   ```bash
   cd Frontend
   ```
2. Install the required Node dependencies:
   ```bash
   npm install
   ```
3. Start the Angular development server:
   ```bash
   ng serve
   ```
4. Open your browser and navigate to `http://localhost:4200`.
---
###  4. Test Credentials
To quickly explore the application without registering a new account, you can use the following pre-seeded test accounts:

**Test User 1** 
* **Email:** test2@example.com
* **Password:** Password123!

**Test User 2**
* **Email:** test3@example.com
* **Password:** Password123
---
### 5. Unit Testing
The Angular application includes unit tests for its standalone components, services, and state management logic using Jasmine and Karma.

To run the test suite and view the live results in a browser:
1. Ensure you are in the `Frontend` directory.
2. Run the following command:
   ```bash
   ng test
   ```
3. To generate a complete HTML coverage report without leaving the browser open, run:
   ```bash
   ng test --code-coverage --watch=false
   ```

## Architecture & Engineering Decisions
As part of the development process, several specific architectural constraints were embraced. Below is an overview of these decisions and how they would be adapted for a true enterprise production environment.

### Backend Testing Architecture
To fulfill the requirement of demonstrating a deep understanding of database interactions, this project utilizes **raw ADO.NET** instead of an ORM like Entity Framework Core. 
*   **The Constraint:** Because core ADO.NET classes are sealed, they cannot be natively mocked using standard libraries like `Moq` without writing extensive custom wrappers. 
*   **The Production Approach:** While the included C# unit tests demonstrate the Arrange, Act, Assert pattern and dependency injection, in a production CI/CD pipeline, these data-access layers would be validated using **Integration Tests**.

### Authentication Strategy & JWT Secrets
(Note: User passwords are securely hashed using BCrypt before being saved to the database).
For demonstration purposes and ease of local testing, the JWT Secret Key used to sign the authentication tokens is hardcoded and commited to this repository.
*   **The Production Approach:** Hardcoding secrets and committing them to source control is a major security vulnerability. In a real-world scenario, the JWT Secret Key would be removed from the codebase entirely and managed securely via **Environment Variables**.

### Client-Side Token Storage
Currently, the frontend uses `sessionStorage` to hold the JWT.
*   **The Production Approach:** To prevent token theft from malicious scripts (XSS attacks), a production environment would use **HttpOnly cookies**. This method hides the token from frontend JavaScript entirely, letting the browser automatically and securely handle authentication.
