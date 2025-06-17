namespace Currency.Api.Models;

public static class ErrorMessage
{
    public static string InternalServerError => "operation_failed";
    public static string ValidationError => "validation_failed";
    public static string Forbidden => "forbidden";
    public static string InvalidExternalApiResponse => "invalid_external_api_response";
    public static string Unauthorized => "unauthorized";
}