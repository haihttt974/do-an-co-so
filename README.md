# Driving School Management System

This repository contains an ASP.NET Core MVC application used to manage a driving school. The source code lives inside the `doan3` directory and targets **.NET 9**.

## Features

- **User authentication** with cookie-based login and email OTP registration.
- **Course and class management** including attendance tracking and instructor assignments.
- **Exam registration** with scheduling and payment tracking.
- **Localization** support (Vietnamese and English) via resource files.
- **Background service** to automatically update course status.

## Project Structure

```
doan3/
├── Controllers/   # MVC controllers
├── Models/        # Entity Framework Core models
├── ViewModel/     # View models for the UI
├── Views/         # Razor views
├── Services/      # Hosted services (e.g. KhoaHocStatusUpdater)
├── wwwroot/       # Static files
├── appsettings.json            # Main configuration (connection string)
├── appsettings.Development.json
└── doan3.csproj
```

The solution file `doan3.sln` references the single project above.

## Requirements

- [.NET SDK 9](https://dotnet.microsoft.com/en-us/download) or newer
- SQL Server instance accessible via the connection string in `doan3/appsettings.json`

## Getting Started

1. **Clone** the repository.
2. Update `doan3/appsettings.json` with your SQL Server connection string.
3. Restore dependencies and build the project:
   ```bash
   dotnet restore doan3.sln
   dotnet build doan3.sln
   ```
4. Run the application:
   ```bash
   dotnet run --project doan3/doan3.csproj
   ```
   The site will be available at the URLs configured in `launchSettings.json` (by default `http://localhost:5005`).

## Usage

After starting, browse to the home page and register or log in. Administrative functionality (course management, payments, reports…) is restricted based on user roles. The UI language can be switched via the `CultureController`, which stores the preferred culture in a cookie.

## Notes

- The `KhoaHocStatusUpdater` service checks course dates daily and updates their status accordingly.
- Static assets are served from `wwwroot`. Custom styles are defined in `Views/Shared/_Layout.cshtml`.

Feel free to fork this project and adapt it for your own driving school or training center.
