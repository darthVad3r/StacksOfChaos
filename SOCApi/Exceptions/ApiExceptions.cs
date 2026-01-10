namespace SOCApi.Exceptions
{
    /// <summary>
    /// Base exception for domain-specific errors.
    /// </summary>
    public abstract class ApiException : Exception
    {
        public int StatusCode { get; }

        protected ApiException(string message, int statusCode = 500) 
            : base(message)
        {
            StatusCode = statusCode;
        }

        protected ApiException(string message, Exception innerException, int statusCode = 500) 
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// Exception for resource not found (404).
    /// </summary>
    public class NotFoundException : ApiException
    {
        public NotFoundException(string message) 
            : base(message, 404)
        {
        }

        public NotFoundException(string resourceType, object id) 
            : base($"{resourceType} with ID '{id}' was not found.", 404)
        {
        }
    }

    /// <summary>
    /// Exception for validation errors (400).
    /// </summary>
    public class ValidationException : ApiException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(string message) 
            : base(message, 400)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(Dictionary<string, string[]> errors) 
            : base("One or more validation errors occurred.", 400)
        {
            Errors = errors;
        }
    }

    /// <summary>
    /// Exception for unauthorized access (401).
    /// </summary>
    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message = "Unauthorized access.") 
            : base(message, 401)
        {
        }
    }

    /// <summary>
    /// Exception for forbidden access (403).
    /// </summary>
    public class ForbiddenException : ApiException
    {
        public ForbiddenException(string message = "You do not have permission to access this resource.") 
            : base(message, 403)
        {
        }
    }

    /// <summary>
    /// Exception for business logic errors (422).
    /// </summary>
    public class BusinessLogicException : ApiException
    {
        public BusinessLogicException(string message) 
            : base(message, 422)
        {
        }
    }

    /// <summary>
    /// Exception for conflict errors (409).
    /// </summary>
    public class ConflictException : ApiException
    {
        public ConflictException(string message) 
            : base(message, 409)
        {
        }

        public ConflictException(string resourceType, object id) 
            : base($"{resourceType} with ID '{id}' already exists.", 409)
        {
        }
    }
}
