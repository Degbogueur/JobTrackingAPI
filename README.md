# 📄 Application Tracker API

This RESTful API helps you **track** and **manage** job applications you have submitted.

Built with **ASP.NET Core**, **Entity Framework Core**, and **FluentValidation**.

---

## 🚀 Main Features

- Track job applications
- CRUD operations

---

## 🛠️ Technologies

- ASP.NET Core 8.0
- Entity Framework Core
- FluentValidation
- Swagger (Swashbuckle)
- SQL Server

---

## ⚙️ Getting Started

1. **Clone the repository**

2. **Configure the database** in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DatabaseConnection": "Server=ServerName;Database=DatabaseName;Trusted_Connection=True;"
     }
   }
   ```

3. **Apply migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the API**
   ```bash
   dotnet run --project src/API
   ```

---

## 📥 Key Endpoints

| Method | URL | Description |
|:---|:---|:---|
| GET | `/api/applications` | List job applications |
| GET | `/api/applications/{id}` | Get application by ID |
| POST | `/api/applications` | Add a new application |
| PUT | `/api/applications/{id}` | Update an application |
| PATCH | `/api/applications/{id}/soft-delete` | Delete an application |
| PATCH | `/api/applications/{id}/restore` | Restore an application |

---

## 🛡️ Validation

Requests are validated automatically:

- Required fields
- Email and other formats

Validation errors return HTTP 400 with details.

---

## 📜 License

MIT License.

---

## 🎯 Notes

This API is lightweight and ready for future improvements like authentication, notifications, or reporting tools.

