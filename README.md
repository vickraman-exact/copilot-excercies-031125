# HRPay-Dezk

HRPay-Dezk is an HR & Payroll System designed for organizations to efficiently manage their employee and payroll operations.

## Getting Started

This project uses VS Code tasks to simplify the development workflow. Follow these instructions to run the application locally.

### Prerequisites

- .NET SDK 9.0 or later
- Node.js 18 or later
- NPM 9 or later
- Visual Studio Code
- SQL Server (local or remote instance)

### First Time Setup

To run the application locally for the first time, follow these steps:

1. **Clone the repository** (if you haven't already):
   ```sh
   git clone <repo-url>
   cd hrpay-dezk
   ```

2. **Install frontend dependencies:**
   - Open VS Code terminal or use VS Code tasks.
   - Run the following task or command:
     - VS Code Task: `frontend: install dependencies`
     - Or manually:
       ```sh
       cd frontend
       npm install
       cd ..
       ```

3. **Build the backend:**
   - Run the following task or command:
     - VS Code Task: `backend: build`
     - Or manually:
       ```sh
       dotnet build backend/HRPayroll.sln
       ```

4. **Configure your database (Windows - localdb\\mssqldb):**
   - Ensure SQL Server LocalDB is installed (comes with Visual Studio or can be installed separately).
   - Use the following connection string in `backend/src/HR.API/appsettings.Development.json`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqldb;Database=HRPayDezkDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
     ```
   - Update the connection string if your instance name or database name differs.

5. **Seed the database:**
   - Open a terminal in the `backend` directory.
   - Run the following command to apply migrations and seed initial data:
     ```pwsh
     dotnet run --project src/HR.API/HR.API.csproj --launch-profile Seed
     ```
   - ⚠️ **Note:** If the seeding fails, it may be because the database schema is not up to date. In that case, you need to apply the latest Entity Framework migrations first. See the `Migrations` folder or use the following command:
     ```pwsh
     dotnet ef database update --project src/HR.API/HR.API.csproj
     ```
   - Alternatively, if a dedicated seeding script or task exists, use that as per project documentation.

6. **Run the application:**
   - You can start both backend and frontend together or separately:
     - VS Code Task: `start: full stack` (recommended)
     - Or run individually:
       - Backend: `backend: run` or `dotnet run --project backend/src/HR.API/HR.API.csproj --launch-profile https`
       - Frontend: `frontend: dev` or `cd frontend && npm run dev`

7. **Access the application:**
   - Backend API: [https://localhost:7120/swagger](https://localhost:7120/swagger)
   - Frontend: [http://localhost:5173](http://localhost:5173)

### Running the Application

#### Using VS Code Tasks

1. Open the project in VS Code
2. Press `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac) to open the Command Palette
3. Type "Tasks: Run Task" and select it
4. Choose one of the following tasks:
   - `backend: run` - Runs only the backend API
   - `frontend: dev` - Runs only the frontend dev server
   - `start: full stack` - Runs both backend and frontend

#### Debugging the Application

1. Press `F5` or go to Run > Start Debugging
2. Select the "Full Stack: Backend + Frontend" configuration
3. This will start both the backend and frontend with debugging enabled

### Available Tasks

- **backend: build** - Builds the backend solution
- **backend: run** - Runs the backend API
- **frontend: install dependencies** - Installs frontend NPM packages
- **frontend: dev** - Runs the frontend development server
- **frontend: build** - Builds the frontend for production
- **start: full stack** - Runs both backend and frontend

## API Endpoints

The backend API is accessible at:
- HTTP: http://localhost:5156
- HTTPS: https://localhost:7120

The Swagger UI is available at: https://localhost:7120/swagger

## Frontend Development

The frontend development server runs at: http://localhost:5173