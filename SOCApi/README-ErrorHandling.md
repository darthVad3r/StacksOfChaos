# Error Handling Middleware - Implementation Guide

## Overview

This implementation provides a comprehensive error handling solution for the SOC API with standardized error responses following REST API best practices.

## Features

? **Standardized Error Responses** - All errors return consistent JSON format  
? **Custom Exception Types** - Domain-specific exceptions for common scenarios  
? **Global Exception Handler** - Centralized error handling middleware  
? **Environment-Aware** - Different error details for dev vs production  
? **Automatic Logging** - All exceptions are logged automatically  
? **Trace IDs** - Unique identifiers for debugging  
? **OpenAPI Integration** - ProducesResponseType attributes for documentation  

## Files Added

- `SOCApi/Models/ErrorResponse.cs` - Standard error response model
- `SOCApi/Exceptions/ApiExceptions.cs` - Custom exception types
- `SOCApi/Middleware/GlobalExceptionHandlerMiddleware.cs` - Exception handling middleware
- Updated `SOCApi/Program.cs` - Middleware registration
- Updated `SOCApi/Controllers/BookController.cs` - Example implementation

## Error Response Format

```json
{
  "statusCode": 404,
  "message": "Book with ID '123' was not found.",
  "traceId": "00-abc123def456...",
  "timestamp": "2025-01-10T06:00:00Z",
  "path": "/api/book/123",
  "details": "Stack trace (development only)",
  "errors": {
    "Title": ["The Title field is required."]
  }
}
```

## Custom Exceptions

### `NotFoundException` (404)
```csharp
throw new NotFoundException("Book", id);
```

### `ValidationException` (400)
```csharp
var errors = new Dictionary<string, string[]>
{
    { "Title", new[] { "Title is required" } }
};
throw new ValidationException(errors);
```

### `BusinessLogicException` (422)
```csharp
throw new BusinessLogicException("Cannot delete book that is checked out.");
```

### `UnauthorizedException` (401)
```csharp
throw new UnauthorizedException();
```

### `ForbiddenException` (403)
```csharp
throw new ForbiddenException("You don't have permission to delete this book.");
```

### `ConflictException` (409)
```csharp
throw new ConflictException("Book", isbn);
```

## Usage in Controllers

### Before
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetBookById(int id)
{
    var book = await _bookService.GetBookByIdAsync(id);
    if (book == null) return NotFound();
    return Ok(book);
}
```

### After
```csharp
[HttpGet("{id}")]
[ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetBookById(int id)
{
    var book = await _bookService.GetBookByIdAsync(id);
    
    if (book == null)
    {
        throw new NotFoundException("Book", id);
    }

    return Ok(book);
}
```

## HTTP Status Code Mapping

| Status | Exception Type | Use Case |
|--------|---------------|----------|
| 400 | `ValidationException` | Invalid request data |
| 401 | `UnauthorizedException` | Missing authentication |
| 403 | `ForbiddenException` | Insufficient permissions |
| 404 | `NotFoundException` | Resource not found |
| 409 | `ConflictException` | Resource conflict |
| 422 | `BusinessLogicException` | Business rule violation |
| 500 | Generic Exception | Unexpected error |

## Client-Side Integration

### TypeScript/Angular
```typescript
interface ErrorResponse {
  statusCode: number;
  message: string;
  traceId: string;
  timestamp: Date;
  path: string;
  errors?: Record<string, string[]>;
}

// HTTP Interceptor
intercept(req: HttpRequest<any>, next: HttpHandler) {
  return next.handle(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const errorResponse = error.error as ErrorResponse;
      this.handleError(errorResponse);
      return throwError(() => errorResponse);
    })
  );
}
```

### C# MAUI
```csharp
try
{
    return await _httpClient.GetFromJsonAsync<Book>($"/api/book/{id}");
}
catch (HttpRequestException ex)
{
    var error = await ex.Content.ReadFromJsonAsync<ErrorResponse>();
    await DisplayAlert("Error", error.Message, "OK");
}
```

## Testing

Test your error handling using:

1. **Swagger/Scalar UI** - `/scalar/v1`
2. **Invalid requests** - Try non-existent IDs, invalid data
3. **Unauthorized requests** - Access protected endpoints without auth

### Example Requests

```bash
# Test 404
curl -X GET https://localhost:5001/api/book/99999

# Test 400
curl -X POST https://localhost:5001/api/book \
  -H "Content-Type: application/json" \
  -d '{"title":"","author":""}'

# Test 401
curl -X GET https://localhost:5001/api/book
```

## Best Practices

1. ? Use specific exception types instead of generic exceptions
2. ? Provide context in error messages
3. ? Never expose sensitive data in errors
4. ? Log exceptions before throwing (if needed)
5. ? Use appropriate HTTP status codes
6. ? Include ProducesResponseType for API documentation

## Migration Guide

To update existing controllers:

1. Replace `return NotFound()` with `throw new NotFoundException()`
2. Replace `return BadRequest(ModelState)` with `throw new ValidationException(errors)`
3. Add XML documentation and ProducesResponseType attributes
4. Remove manual exception handling (let middleware handle it)

## Configuration

No additional configuration needed! The middleware is automatically registered in `Program.cs`:

```csharp
app.UseGlobalExceptionHandler(); // Must be first in pipeline
```

## Troubleshooting

**Q: Errors not being caught?**  
A: Ensure `UseGlobalExceptionHandler()` is called before other middleware

**Q: Getting 500 instead of specific status codes?**  
A: Make sure you're throwing the correct exception type

**Q: Not seeing error details?**  
A: Details only show in Development environment

## Next Steps

- [ ] Update other controllers to use custom exceptions
- [ ] Add integration tests for error scenarios
- [ ] Update API documentation
- [ ] Train team on new error handling approach
- [ ] Monitor error logs in production

## Support

For questions or issues, contact the development team or refer to:
- API Documentation: `/scalar/v1`
- GitHub Issues: [Repository Issues](https://github.com/darthVad3r/StacksOfChaos/issues)
