# Peer-graded-Assignment-Project-Building-a-Simple-API-with-Copilot

Peer-graded Assignment Microsoft Fullstack via Cousera

# UserManagementAPI

A simple ASP.NET Core Web API for TechHive Solutions internal user management.

## Run

```powershell
dotnet run
```

The API listens on the HTTP and HTTPS URLs printed by the application. The included `UserManagementAPI.http` file contains authenticated requests for every CRUD operation and middleware edge cases.

All `/api` routes require `Authorization: Bearer techhive-demo-token`. Set `ApiToken` in `appsettings.json` or an environment-specific configuration file to change the token.

## Middleware

The pipeline is configured in this order:

1. `ExceptionHandlingMiddleware` catches unhandled exceptions and returns `{ "error": "Internal server error." }` with status `500`.
2. `TokenAuthenticationMiddleware` validates the bearer token and returns `{ "error": "Unauthorized." }` with status `401` when it is missing or invalid.
3. `RequestResponseLoggingMiddleware` logs the HTTP method, path, response status code, and elapsed time for requests that pass authentication.

The `/api/users/diagnostics/error` route is hidden from API discovery and is available only in the Development environment to verify the error-handling middleware during this activity.

## Endpoints

| Method | Route             | Purpose           |
| ------ | ----------------- | ----------------- |
| GET    | `/api/users`      | List all users    |
| GET    | `/api/users/{id}` | Retrieve one user |
| POST   | `/api/users`      | Create a user     |
| PUT    | `/api/users/{id}` | Update a user     |
| DELETE | `/api/users/{id}` | Delete a user     |

Data is stored in memory for this activity, so it resets when the application restarts.

## Copilot assistance

Microsoft Copilot assisted with:

- Scaffolding the ASP.NET Core Web API project and `Program.cs` startup boilerplate.
- Generating the controller route structure and CRUD action signatures.
- Suggesting request validation attributes for required fields, lengths, and email format.
- Creating an in-memory store and sample data so the endpoints can be tested immediately.
- Producing the HTTP request examples used to exercise successful and not-found CRUD cases.
- Finding and fixing whitespace-only input acceptance with a custom validation attribute.
- Adding centralized problem-details handling for unexpected exceptions and a `409 Conflict` response for duplicate email addresses.
- Replacing the deferred user enumeration with a materialized, ordered snapshot and protecting mutations against concurrent updates.
- Generating middleware for standardized exception responses, bearer-token validation, and request/response audit logging.
- Configuring the middleware pipeline in the required error-handling, authentication, and logging order.

## Manual test checklist

Run the API, then execute the requests in `UserManagementAPI.http` with VS Code REST Client, Postman, or another HTTP client. Confirm that list/retrieve return `200`, create returns `201`, update returns `200`, delete returns `204`, and missing IDs return `404`.

Debugging checks also confirm that whitespace-only names and malformed request data return `400`, while duplicate email addresses return `409` without crashing the API. Middleware checks should confirm valid tokens return normal CRUD responses, missing or invalid tokens return `401`, and the diagnostic route returns the standardized `500` JSON error.
