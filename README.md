# LunaEdgeTestTask

## Overview

LunaEdgeTestTask is a .NET 8 Web API for user authentication and task management.
It uses JWT for secure authentication and Entity Framework Core for data access.
The project demonstrates clean architecture principles, separation of concerns, and modern C# features.

---

## Setup Instructions

1. **Clone the repository**
   You can use any tool you want for this.

2. **Configure the database**
   You must have SQL Server and Docker (Desktop) installed.
   If necessary, you can also update the connection string in `appsettings.json`:

   ```
   "DefaultConnection": "Server=localhost,1433;Database=LunaEdgeTestTaskDb;User=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;"
   ```

3. **Restore NuGet packages**
   Use `dotnet restore` or install them manually.

   For the entire project: <img width="795" height="598" alt="image" src="https://github.com/user-attachments/assets/7589f329-acfc-40c3-915d-22c22e93bf18" />

   For tests: <img width="788" height="390" alt="image" src="https://github.com/user-attachments/assets/bb52c32b-02c5-48d4-8ea5-0e2471a29e8e" />

> Note: This is not the newest version because we are working with an older .NET version.

4. **Apply migrations and update the database**
   Run `dotnet ef database update` if you are using VSCode, or `Update-Database` if you are using Visual Studio.

5. **Create a Docker multi-container setup**
   Run:

   ```bash
   docker-compose up --build
   ```

6. **(Optional) SQL Server Management Studio (SSMS)**
   If you want, you can install SSMS to manually manage your database.
   For correct connection, use:

   ```
   Server Name: localhost,1433
   Authentication: SQL Server Authentication
   Login: sa
   Password: YourStrong!Passw0rd
   ```

## **LunaEdgeTestTask API Documentation**

**Base URL:**

```
[http://localhost:8080/swagger/index.html]
```

> **Note:** Endpoints under **/api/tasks** require authentication via JWT. Include in header:

```
Authorization: Bearer <JWT_TOKEN>
```

---

### **User Authentication (AuthController)**

| Endpoint          | Method | Request Body                                                             | Response                                                                 | Description                     |
| ----------------- | ------ | ------------------------------------------------------------------------ | ------------------------------------------------------------------------ | ------------------------------- |
| `/users/register` | POST   | `json { "userName": "string", "email": "string", "password": "string" }` | `json { "token": "JWT_TOKEN", "refreshToken": "REFRESH_TOKEN" }`         | Register a new user             |
| `/users/login`    | POST   | `json { "email": "string", "password": "string" }`                       | `json { "token": "JWT_TOKEN", "refreshToken": "REFRESH_TOKEN" }`         | Login with existing credentials |
| `/users/refresh`  | POST   | `json { "refreshToken": "string" }`                                      | `json { "token": "NEW_JWT_TOKEN", "refreshToken": "NEW_REFRESH_TOKEN" }` | Refresh JWT token               |

**Errors:** `401 Unauthorized` if registration, login, or refresh fails.

---

Click the Authorize button:
<img width="229" height="79" alt="image" src="https://github.com/user-attachments/assets/e7c15871-715f-4b4d-9f37-27c575b46d3a" />
Enter your token obtained from the response of any user endpoint:
<img width="655" height="292" alt="image" src="https://github.com/user-attachments/assets/f68b423f-e43f-44d1-add5-665f64422211" />
Click Authorize again.
After this, you can use all endpoints under Task Management.

---

### **Task Management (TaskController)**

| Endpoint      | Method | Request Body                                                                                                   | Response                                                                                                                                             | Description                                                                                                                                              |                                   |
| ------------- | ------ | -------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------- |
| `/tasks`      | POST   | `json { "title": "string", "description": "string", "dueDate": "2025-08-29T12:00:00Z" }`                       | `json { "taskId": "GUID", "title": "string", "description": "string", "dueDate": "2025-08-29T12:00:00Z", "createdBy": "GUID", "status": "Pending" }` | Create a new task                                                                                                                                        |                                   |
| `/tasks`      | GET    | Query parameters (optional): \`status=Pending                                                                  | Completed`, `dueDate=YYYY-MM-DD\`                                                                                                                    | `json [ { "taskId": "GUID", "title": "string", "description": "string", "dueDate": "2025-08-29T12:00:00Z", "createdBy": "GUID", "status": "Pending" } ]` | Get all tasks (filtered optional) |
| `/tasks/{id}` | GET    | –                                                                                                              | `json { "taskId": "GUID", "title": "string", "description": "string", "dueDate": "2025-08-29T12:00:00Z", "createdBy": "GUID", "status": "Pending" }` | Get task details by ID                                                                                                                                   |                                   |
| `/tasks/{id}` | PUT    | \`\`\`json { "title": "string", "description": "string", "dueDate": "2025-08-29T12:00:00Z", "status": "Pending | Completed" }\`\`\`                                                                                                                                   | `json { "taskId": "GUID", "title": "string", "description": "string", "dueDate": "2025-08-29T12:00:00Z", "status": "Pending" }`                          | Update task by ID                 |
| `/tasks/{id}` | DELETE | `json { "id": "GUID" }`                                                                                        | `true`                                                                                                                                               | Delete task by ID                                                                                                                                        |                                   |

**Errors:** `401 Unauthorized` if the user is not authenticated or not the owner of the task.

---

#### Example: Authorization Header

```http
GET /api/tasks HTTP/1.1
Host: localhost:5000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

