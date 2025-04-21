# Reporting Services

## Project Description

This project is a reporting service with a backend implemented in .NET and a frontend in React. The backend consists of multiple services, repositories, and controllers for managing reports and report sources. The frontend is a React application located in the `src/reporting` directory.

### Backend Services

- `IReportService`
- `IReportRepository`
- `IReportSourceRepository`

### Backend Controllers

- `ReportingController` which handles API requests related to reports.

### Database Access

- The project uses Dapper for database access and SQL scripts for database setup.

### Docker Support

- The project includes Docker support with a `Dockerfile` in the `src/Reporting.Server` directory.

## Project Setup

### Prerequisites

- .NET SDK
- Node.js and npm
- Docker (optional, for containerized deployment)

### Installation Steps

1. Clone the repository:
   ```sh
   git clone https://github.com/DavisKolakowski/ReportingServices.git
   cd ReportingServices
   ```

2. Install backend dependencies:
   ```sh
   dotnet restore
   ```

3. Install frontend dependencies:
   ```sh
   cd src/reporting
   npm install
   ```

## Available Scripts

### Backend

- `dotnet build`: Builds the .NET backend.
- `dotnet run`: Runs the .NET backend.

### Frontend

- `npm start`: Runs the React app in development mode.
- `npm run build`: Builds the React app for production.

## Deploying Changes

### Building the Project

1. Build the backend:
   ```sh
   dotnet build
   ```

2. Build the frontend:
   ```sh
   cd src/reporting
   npm run build
   ```

### Running Docker

1. Build the Docker image:
   ```sh
   cd src/Reporting.Server
   docker build -t reporting-services .
   ```

2. Run the Docker container:
   ```sh
   docker run -p 8080:8080 reporting-services
   ```

## Additional Information

- [React documentation](https://reactjs.org/)
- [Dapper documentation](https://dapper-tutorial.net/)
- [Docker documentation](https://docs.docker.com/)
