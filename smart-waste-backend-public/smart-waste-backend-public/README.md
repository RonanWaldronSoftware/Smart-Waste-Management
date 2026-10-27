# SmartWaste — Smart Waste Management

SmartWaste is a web-based system for monitoring smart waste bins and managing their day-to-day operation. It brings information from connected bins into one place so operators can check fill levels, identify bins needing attention, review problems, and organize collection routes.

The software has two parts: a browser-based management application (frontend) and server services (backend) that receive device readings, store information, and provide it to the application.

## What the system does

| Area | Function |
| --- | --- |
| Dashboard | Summarizes bin status, including fill-level groups and waste categories. |
| Map | Shows bin locations and allows operators to inspect bins geographically. |
| Bin management | Lists bins, shows individual details, and supports adding, editing, and installing bins according to user permissions. |
| Sensor monitoring | Displays current and historical readings such as fill level, temperature, battery condition, and signal strength where supported by the device. |
| Alerts | Provides configurable bin-related alerts and warning information, including conditions such as full bins or device faults. |
| Damage reports | Records and tracks reported damage, including status, location, and assigned personnel. |
| Collection routes | Allows authorized users to create, view, change, and delete routes associated with bins and users. |
| Reports and exports | Supports reviewing records and exporting users, bins, and bin-reading information to PDF or spreadsheet files. |
| User management | Provides sign-in, password reset, user accounts, and role-based access to management functions. |

Not every reading or control is available for every bin: this depends on its hardware and configuration.

## How it works

1. **Bins send readings.** Connected devices report information through supported communications methods, including Sigfox and direct network connections handled by the backend.
2. **The backend processes the readings.** Incoming messages are queued, interpreted, and saved alongside the relevant bin records in the database.
3. **The system evaluates operating conditions.** It maintains bin status and supports alerts for configured conditions. Background jobs handle maintenance tasks, including checks for devices that have stopped reporting.
4. **Staff use the web application.** Authorized users sign in to see maps, status summaries, sensor history, routes, reports, and account information.
5. **Operators act on the information.** They can investigate full or faulty bins, manage damage reports, update bin records, and organize routes.

The aim is to give waste-collection teams a clearer picture of which bins need service and where issues are occurring. Route management is included; automatic route optimization has not been established from the supplied code.

## Main software components

The **frontend** is a React web application. It presents the dashboard, map, bin records, alerts, reports, routes, and account-management screens.

The **backend** is built with .NET 8 and is divided into services with different responsibilities:

| Component | Responsibility |
| --- | --- |
| Account service | Login, password management, user accounts, permissions, and application settings. |
| Unit service | Bin records, readings, routes, damage reports, and alert subscriptions. |
| Handler service | Incoming device data and report exports. |
| TCP listener | Receives readings directly from supported network-connected devices. |
| Background job service | Performs scheduled maintenance and monitoring tasks. |
| Shared data and services | Database access, processing logic, notifications, and integrations. |

The backend includes connections for email, SMS, and push notifications, as well as integrations for external device information. Actual notification delivery depends on the deployed configuration.

## Access and users

The application requires users to sign in for operational screens. Permissions vary by role; for example, adding bins, managing users, and changing routes are restricted in the application and backend. The code also includes password recovery and access-token renewal.

## Requirements and operation

This is a multi-service application rather than a standalone program. To operate it, an administrator needs:

- A web-hosting environment for the React frontend.
- A .NET 8 environment for the backend services.
- A configured relational database (the code includes SQL Server database access).
- RabbitMQ for handling incoming device messages.
- Appropriate network connectivity to the bin devices and any external services in use.
- Configuration for application addresses, authentication, database access, and enabled integrations.
- A Google Maps API key for the map features.

The frontend uses `VITE_SMARTWASTE_BASE_URL` to locate the backend and `VITE_GOOGLE_MAPS_KEY` for maps. Backend configuration is held in environment-specific `appsettings` files. Credentials and keys must be supplied securely for each deployment.

For developers, the frontend includes standard commands:

```bash
npm install
npm run dev
npm run build
```

The backend solution is `SmartWaste.sln`, with its individual services configured and started for the target environment. A complete production deployment also requires database setup, service addresses, device connectivity, and integration settings; it is not defined by these commands alone.

## Project structure

The supplied software is split across two repositories or archives:

- **`smart-waste-front-react`** — the browser-based user interface.
- **`smartwaste-backend`** — the .NET services, data models, database access, and device-processing components.

## Scope of this README

This overview is based on the source code in the two supplied archives. It describes functions found in the software, not a verified live deployment. Device availability, alert delivery, integrations, and the exact operational configuration depend on the installed environment.
