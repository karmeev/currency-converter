namespace Currency.Api.Models;

public static class ErrorMessage
{
    public static string InternalServerError => "operation_failed";
    public static string ValidationError => "validation_failed";
    public static string Forbidden => "forbidden";
    public static string FailedDependency => "external_api_does_not_respond";
    public static string Unauthorized => "unauthorized";
}