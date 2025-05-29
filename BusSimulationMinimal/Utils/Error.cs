namespace BusSimulationMinimal.Utils;

/**
 * *
 * * {
 * "error": {
 * "code": "VALIDATION_ERROR", // Or "RESOURCE_NOT_FOUND", "INVALID_OPERATION", "SERVER_ERROR"
 * "message": "A human-readable description of the error.",
 * "details": [ // Optional: an array of more specific error details, e.g., for field-level validation errors
 * {
 * "field": "capacity",
 * "issue": "Capacity must be a positive integer."
 * }
 * ]
 * }
 * }
 */
public class Error(string code, string message, Error.ErrorDetails[] details)
{
    public string Code { get; set; }
    public string Message { get; set; }
    public ErrorDetails[] Details { get; set; }

    public class ErrorDetails(string field, string issue)
    {
        public string Field { get; set; } = field;
        public string Issue { get; set; } = issue;
    }
}