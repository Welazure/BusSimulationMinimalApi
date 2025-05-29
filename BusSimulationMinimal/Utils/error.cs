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
public class Error(string code, string message, Error.Details[] details)
{
    public string code { get; set; }
    public string message { get; set; }
    public Details[] details { get; set; }

    public class Details(string field, string issue)
    {
        public string field { get; set; } = field;
        public string issue { get; set; } = issue;
    }
}