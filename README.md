# HRMS - Human Resource Management System

A lightweight system to manage employee data and leave requests for small businesses.

## Features

- Add and view employees (ID, Name, Department)
- Submit and view leave requests (Employee ID, Date, Reason)
- Dashboard to display employees and their details

## Technology Stack

- **Frontend**: React JS (with Vite) + Tailwind CSS
- **Backend**: ASP.NET Core Web API
- **Database**: PostgreSQL

## Project Structure

- `/frontend` - React frontend application
- `/backend` - ASP.NET Core Web API backend

## Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/)
- [PostgreSQL](https://www.postgresql.org/download/)

### Backend Setup

1. Navigate to the backend directory:
   ```
   cd backend/HrmsApi
   ```

2. Update the database connection string in `appsettings.json` if needed.

3. Apply database migrations:
   ```
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. Run the backend API:
   ```
   dotnet run
   ```
   The API will be available at `https://localhost:7080`

### Frontend Setup

1. Navigate to the frontend directory:
   ```
   cd frontend
   ```

2. Install dependencies:
   ```
   npm install
   ```

3. Run the development server:
   ```
   npm run dev
   ```
   The frontend will be available at `http://localhost:5173`

## API Endpoints

- `GET /api/employees` - Get all employees
- `POST /api/employees` - Add a new employee
- `GET /api/leaverequests` - Get all leave requests
- `POST /api/leaverequests` - Submit a new leave request
- `GET /api/leaverequests/employee/{id}` - Get leave requests for a specific employee

## Flow Diagram

1. User accesses the app via browser
2. Dashboard displays employees and leave requests
3. User can navigate to Add Employee or Request Leave pages
4. Add Employee: User fills form and submits to add a new employee
5. Request Leave: User selects an employee, date, and reason to submit a leave request
6. Backend API handles data storage and retrieval with PostgreSQL

## Business Value

This application simplifies employee and leave management for small teams, reducing manual effort and providing a centralized system for HR data.
